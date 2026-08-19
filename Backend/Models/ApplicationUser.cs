using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string FullName{get;set;}

        public bool IsAvailable{get;set;}=true;

        public bool IsActive{get;set;}=true;

        public ICollection<Car>Cars{get;set;}

        public ICollection<Order>Orders{get;set;}
    }