﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StudyProject
{
 
    //EntityFramework соединение с базой данных
    public class ConnectDB : DbContext
    {
        
        public  DbSet<Model.Store> Stores { get; set; }
        public  DbSet<Model.GoodType> GoodTypes { get; set; }
        public  DbSet<Model.Good> Goods { get; set; }
        public DbSet<Model.Order> Orders { get; set; }
        public DbSet<Model.Basket> Baskets { get; set; }
        public ConnectDB()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            Database.Migrate();
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
            modelBuilder.Entity<Model.Good>().Ignore(p => p.Pictures);//игнорирование поле Pictures, чтобы DbContext не создал его в SQlite, так как Pictires работает с Firebase
            modelBuilder.Entity<Model.Good>().Ignore(p => p.Count);//Игнорирование поля Count
           
            #endregion
            #region Basket

            modelBuilder.Entity<Model.Order>(p => p.ToTable("order"));
            modelBuilder.Entity<Model.Basket>(p => p.ToTable("basket"));
            modelBuilder.Entity<Model.Basket>().Ignore(p => p.QRstring);//игнорирование поле QRstring, чтобы DbContext не создал его в SQlite, так как QRstring работает с Firebase
            #endregion


        }
    }
}