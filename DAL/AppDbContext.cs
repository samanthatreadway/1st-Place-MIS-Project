using System;
using Microsoft.EntityFrameworkCore;

using fa25group23final;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using fa25group23final.Models;

namespace fa25group23final.DAL
{
    //NOTE: This class definition references the user class for this project.  
    //If your User class is called something other than AppUser, you will need
    //to change it in the line below
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //this code makes sure the database is re-created on the $5/month Azure tier
            builder.HasPerformanceLevel("Basic");
            builder.HasServiceTier("Basic");
            base.OnModelCreating(builder);

            builder.Entity<Reviews>()
                .HasOne(m => m.reviewer)
                .WithMany(m => m.WrittenReviews)
                .HasForeignKey(m => m.reviewerID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Reviews>()
                .HasOne(m => m.approver)
                .WithMany(m => m.ApprovedReviews)
                .HasForeignKey(m => m.approverID)
                .OnDelete(DeleteBehavior.SetNull);

        }

        public DbSet<Books> Books { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Genres> Genres { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Shippingconfig> ShippingConfigs { get; set; }
        public DbSet<Reorders> Reorders { get; set; }
        public DbSet<ReorderDetails> ReorderDetails { get; set; }


    }
}
