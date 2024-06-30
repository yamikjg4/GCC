using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Chat.Web.Models;
using GlobalCalendar.MVC.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserMasterModel> Users_MST { get; set; }
        public DbSet<BrodcastHistory> Tbl_BrodcastMesHistrory { get; set; }

     /*   protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }*/
    }
}
