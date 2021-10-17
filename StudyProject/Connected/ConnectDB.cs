using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudyProject
{
    public class ConnectDB : DbContext
    {
        
        public  DbSet<Model.Store> Stores { get; set; }
        public  DbSet<Model.GoodType> GoodTypes { get; set; }
        public  DbSet<Model.Good> Goods { get; set; }
        public ConnectDB()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            //Database.Migrate();
        }

            
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite("Data Source=Base.db");

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region  GoodStoreTypes
            modelBuilder.Entity<Model.Store>(b => b.ToTable("store"));
            modelBuilder.Entity<Model.GoodType>(b => b.ToTable("good_type"));
            #endregion

            #region Goods

            modelBuilder.Entity<Model.Good>(b => b.ToTable("good"));
            modelBuilder.Entity<Model.Good>().Ignore(p => p.Pictures);

            #endregion


        }
    }
}