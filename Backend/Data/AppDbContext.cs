using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    
        public DbSet<Car> Cars { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ServicePackage> ServicePackages { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<AddOn> AddOns { get; set; }
        public DbSet<OrderAddOn> OrderAddOns { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Leaderboard> Leaderboards { get; set; }
        public DbSet<WashRequest> WashRequests { get; set; }
    
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
    
            modelBuilder.Entity<OrderAddOn>()
                .HasKey(oa => new { oa.OrderId, oa.AddOnId });

            modelBuilder.Entity<OrderAddOn>()
                .HasOne(oa => oa.Order) 
                .WithMany(o => o.OrderAddOns)
                .HasForeignKey(oa => oa.OrderId);

            modelBuilder.Entity<OrderAddOn>()
                .HasOne(oa => oa.AddOn)
                .WithMany(a => a.OrderAddOns)
                .HasForeignKey(oa => oa.AddOnId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Washer)
                .WithMany()
                .HasForeignKey(o => o.WasherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WashRequest>()
                .HasOne(wr => wr.User)
                .WithMany()
                .HasForeignKey(wr => wr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WashRequest>()
                .HasOne(wr => wr.Washer)
                .WithMany()
                .HasForeignKey(wr => wr.WasherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WashRequest>()
                .HasOne(wr => wr.Car)
                .WithMany()
                .HasForeignKey(wr => wr.CarId);

            modelBuilder.Entity<WashRequest>()
                .HasOne(wr => wr.Order)
                .WithMany()
                .HasForeignKey(wr => wr.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WashRequest>()
                .HasOne(wr => wr.Package)
                .WithMany()
                .HasForeignKey(wr => wr.PackageId);

            modelBuilder.Entity<WashRequest>()
                .HasOne(wr => wr.PromoCode)
                .WithMany()
                .HasForeignKey(wr => wr.PromoCodeId);

                modelBuilder.Entity<Rating>()
                .HasOne(r => r.Order)
                .WithMany()
                .HasForeignKey(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Washer)
                .WithMany()
                .HasForeignKey(r => r.WasherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }