using System;
using System.Collections.Generic;
using System.Text;
using app.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace app.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostArchive> PostsArchive { get; set; }
        public DbSet<Room> Rooms { get; set; }

        protected override void
       OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<UserRoom>()
                .HasKey(x => new { x.UserId, x.RoomId });
         
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole { Name = "Admin", NormalizedName = "Admin".ToUpper() });
            base.OnModelCreating(modelBuilder);
        }
    }
}
