using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
// 1. Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("CarWashConnectionString")));

// 2. Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// 3. Register Application Services and Repositories
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IServicePackageRepository, ServicePackageRepository>();
builder.Services.AddScoped<IServicePackageService, ServicePackageService>();
builder.Services.AddScoped<IAddOnRepository, AddOnRepository>();
builder.Services.AddScoped<IAddOnService, AddOnService>();
builder.Services.AddScoped<IPromoCodeService, PromoCodeService>();
builder.Services.AddScoped<IOrderAddonService, OrderAddonService>();
builder.Services.AddScoped<IOrderAddonRepository, OrderAddonRepository>();
builder.Services.AddScoped<IPaymentReceiptService, PaymentReceiptService>();
builder.Services.AddScoped<IWasherService, WasherService>();
builder.Services.AddScoped<IPaymentReceiptRepository, PaymentReceiptRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IWashRequestRepository, WashRequestRepository>();
builder.Services.AddScoped<IWashRequestService, WashRequestService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// 4. Configure JWT Authentication
var jwtSettings = configuration.GetSection("JWT");
Console.WriteLine($"Issuer: {jwtSettings["ValidIssuer"]}");
Console.WriteLine($"Audience: {jwtSettings["ValidAudience"]}");
Console.WriteLine($"Secret: {jwtSettings["Secret"]}");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
    };
});

// 5. Add Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "CarWash API", Version = "v1" });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

// 6. Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();

    await SeedRoles(services);
    await SeedAdminUser(services, config);
}

app.Run();

// Seed Roles Method
async Task SeedRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "ADMIN", "CUSTOMER", "WASHER" };

    foreach (var role in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Seed Admin User Method
async Task SeedAdminUser(IServiceProvider serviceProvider, IConfiguration config)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var email = config["AdminCredentials:Email"];
    var password = config["AdminCredentials:Password"];

    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
    {
        throw new ArgumentNullException("Admin email or password is not configured correctly.");
    }

    var adminUser = await userManager.FindByEmailAsync(email);
    if (adminUser == null)
    {
        var user = new ApplicationUser
        {
            FullName = "Vedant Bajpai",
            Email = email,
            UserName = email,
            PhoneNumber = "8004842990",
            IsActive = true,
            IsAvailable = false
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "ADMIN");
        }
    }
}
