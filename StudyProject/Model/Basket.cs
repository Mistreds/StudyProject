using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyProject.Model
{
    public class Order:BaseViewModel
    {
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        private DateTime _date;
        public DateTime Date
        {
            get=> _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }
        private double _itog_price;
        public double ItogPrice
        {
            get=> _itog_price;
            set
            {
                _itog_price = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Basket> _basket;
        public ObservableCollection<Basket> Basket
        {
            get=> _basket;
            set
            {
                _basket = value;
                OnPropertyChanged();
            }
        }
        private int _itog_count;
        public int ItogCount
        { 
            get=> _itog_count;
            set
            {
                _itog_count = value;
                OnPropertyChanged();
            }
        }
        
    }
    public class Basket :BaseViewModel
    {
        public Basket() { }
        public Basket(int GoodId, int Count, byte[] file)
        {

            this.GoodId= GoodId;
            this.Count = Count;
            _qr_string = Convert.ToBase64String(file);
        }
        public Basket(int GoodId, int Count)
        {

            this.GoodId = GoodId;
            this.Count = Count;
            _qr_string = null; ;
        }
        private string _qr_string;
        public string QRstring
        {
            get => _qr_string;


        }
        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        private int _id_order;
        public int OrderId
        {
            get=> _id_order;
            set
            {
                _id_order = value;
                OnPropertyChanged();
            }
        }
        private Order _order;
        public Order Order
        {
            get => _order;
            set
            {
                _order = value;
                OnPropertyChanged();
            }
        }
        private int _good_id;
        public int GoodId
        {
            get=> _good_id;
            set
            {
                _good_id = value;
                OnPropertyChanged();
            }
        }
        private Good _good;
        public Good Good
        {
            get => _good;
            set
            {
                _good = value;
                OnPropertyChanged();
            }
        }
        private int _count;
        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }
    }
}
