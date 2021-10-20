using BE;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALimp : IStoreType, IGood,IOrderBasket
    {
        


        public int AddGood(Good good)
        {
            var db = new ConnectDB();
            db.Goods.Add(good);
            db.SaveChanges();
            good.UpdatePicId();
            Firebase.UploadPictires(good.Pictures);
            return good.Id;
        }

        public ObservableCollection<Store> getAllStore()
        {
            var db = new ConnectDB();
            return new ObservableCollection<Store>(db.Stores);
        }

        public ObservableCollection<GoodType> getAllType()
        {
            var db = new ConnectDB();
            return new ObservableCollection<GoodType>(db.GoodTypes);
        }

        public Good GetGoodById(int id)
        {
            var db = new ConnectDB();
            return db.Goods.Include(p=>p.Store).Include(p=>p.GoodType).FirstOrDefault(p => p.Id == id);
        }

        public Store GetStoreById(int id)
        {
            var db = new ConnectDB();
            return db.Stores.FirstOrDefault(p => p.Id == id);
        }

        public GoodType GetTypeById(int id)
        {
            var db = new ConnectDB();
            return db.GoodTypes.FirstOrDefault(p => p.Id == id);
        }

        public void UpdateStore(ObservableCollection<Store> store)
        {
            var db = new ConnectDB();
            db.Stores.UpdateRange(store);
            db.SaveChanges();
            db.RemoveRange(db.Stores.AsQueryable().Where(p => !store.Select(s => s.Id).Contains(p.Id)));
            db.SaveChanges();
        }

        public void UpdateType(ObservableCollection<GoodType> type)
        {
            var db = new ConnectDB();
            db.GoodTypes.UpdateRange(type);
            db.SaveChanges();
            db.RemoveRange(db.GoodTypes.AsQueryable().Where(p => !type.Select(s => s.Id).Contains(p.Id)));
            db.SaveChanges();
        }
        public ObservableCollection<Good> getAllGood()
        {
            var db = new ConnectDB();
            return new ObservableCollection<Good>(db.Goods.Include(p => p.Store).Include(p => p.GoodType));
        }

        public void AddOrder(Order Order)
        {
            var db = new ConnectDB();
            Order.Date = DateTime.Now;
            db.Orders.Add(Order);
            db.SaveChanges();//добавляем заказ в бд
            foreach (var bask in Order.Basket)
            {
                if (bask.QRstring == null)
                    continue;
                Firebase.UploadQr(bask.Id, bask.QRstring);//загружаем qr код
            }
        }

        public List<List<string>> GetAllGoodsFromBasket()
        {
            var db = new ConnectDB();
            return db.Orders.AsQueryable().Include(p => p.Basket).Select(p => p.Basket.Select(s => s.GoodId.ToString()).ToList()).ToList();
        }

        public int CountBasketFromDate(DateTime date1, DateTime date2)
        {
            var db = new ConnectDB();
            return db.Baskets.Include(p => p.Order).Where(p => p.Order.Date >= date1 && p.Order.Date <= date2).Sum(p => p.Count);
        }

        public List<(int GoodId, int Count)> GetGoodFromBasketDate(DateTime date1, DateTime date2)
        {
            var db = new ConnectDB();
            List<(int GoodId, int Count)> list=new List<(int GoodId, int Count)>();
            db.Baskets.Include(p => p.Order).Where(p => p.Order.Date >= date1 && p.Order.Date <= date2).GroupBy(p => p.GoodId).Select(grp => new
            {
                Id = grp.Key,
                Count = grp.Sum(p => p.Count)
            }).ToList(); ;
            foreach (var a in db.Baskets.Include(p => p.Order).Where(p => p.Order.Date >= date1 && p.Order.Date <= date2).GroupBy(p => p.GoodId).Select(grp => new { GoodId = grp.Key, Count = grp.Sum(p => p.Count) }))
            {
                list.Add((a.GoodId, a.Count));
            }
           return  list; ;
        }

        public int CountOrderFromDate(DateTime date1, DateTime date2)
        {
            var db = new ConnectDB();
            return db.Orders.Count(p => p.Date >= date1 && p.Date <= date2);
        }
    }

    public interface IStoreType
    {

        ObservableCollection<BE.Store> getAllStore();
        ObservableCollection<BE.GoodType> getAllType();
        void UpdateStore(ObservableCollection<Store> store);
        void UpdateType(ObservableCollection<GoodType> type);
        Store GetStoreById(int id);
        GoodType GetTypeById(int id);
        
    }
    public interface IGood
    {
        int AddGood(Good good);
        Good GetGoodById(int id);
        ObservableCollection<Good> getAllGood();

    }
    public interface IOrderBasket
    {
        void AddOrder(Order order);
        List<List<string>> GetAllGoodsFromBasket();
        int CountBasketFromDate(DateTime date1, DateTime date2);
        int CountOrderFromDate(DateTime date1, DateTime date2);
        List<(int GoodId, int Count)> GetGoodFromBasketDate(DateTime date1, DateTime date2);
        
    }
}


