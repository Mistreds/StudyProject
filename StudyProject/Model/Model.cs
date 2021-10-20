using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyProject.Model
{
    public class Good : BE.Good
    {
        public Good(BE.GoodSerialized good_ser)
        {
            this.Id = good_ser.id;
            this._count = good_ser.count;
            UpdateGood();
        }
        public Good()
        {
        }
        private protected virtual void UpdateGood()
        {
            using (var db = new ConnectDB())
            {
                var good = MainViewModel.DALimp.GetGoodById(Id);
                this.Name = good.Name;
                this.Description = good.Description;
                this.Store = good.Store;
                this.GoodType = good.GoodType;
                this.StoreId = good.StoreId;
                if (_count == 0)
                {
                    this.Price = good.Price;
                }
                else
                {
                    this.Price = good.Price * _count;
                }

                this.GoodTypeId = good.GoodTypeId;
                var fire = new Firebase();
                this.Pictures = fire.DownloadPictires(this.Id);
                this.Pictures.UpdatePhoto();

            }
        }
        public Good(int id)
        {
            this.Id = id;
            UpdateGood();

        }
        public Good(BE.Good p)
        {
            using (var db = new ConnectDB())
            {
                this.Id = p.Id;
                this.Description = p.Description;
                this.Price = p.Price;
                this.StoreId = p.StoreId;
                if (p.Store == null)
                {
                    this.Store = MainViewModel.DALimp.GetStoreById(StoreId);
                }
                else
                {

                    this.Store = p.Store;
                }

                this.GoodTypeId = p.GoodTypeId;
                if (p.GoodType == null)
                {
                    this.GoodType = p.GoodType;
                }
                else
                {
                    this.GoodType = MainViewModel.DALimp.GetTypeById(GoodTypeId);
                }
                this.Name = p.Name;
                var fire = new Firebase();
                this.Pictures = fire.DownloadPictires(this.Id);
            }
        }
        public Good(BE.Good p, bool is_new)
        {
            using (var db = new ConnectDB())
            {
                this.Id = p.Id;
                this.Description = p.Description;
                this.Price = p.Price;
                this.StoreId = p.StoreId;
                if (p.Store == null)
                {
                    this.Store = MainViewModel.DALimp.GetStoreById(StoreId);
                }
                else
                {

                    this.Store = p.Store;
                }

                this.GoodTypeId = p.GoodTypeId;


                this.GoodType = MainViewModel.DALimp.GetTypeById(GoodTypeId);


                this.Name = p.Name;
                var fire = new Firebase();
                this.Pictures = p.Pictures;
                this.Pictures.UpdatePhoto();
            }
        }
    }
}
