
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BE
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие изменения свойств
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// <see cref="CallerMemberNameAttribute"/> автоматически определяет имя вызываемой переменной.
        /// </summary>
        /// <param name="propertyName">Имя переменной.</param>
        ///
        ///
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Order : BaseViewModel
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
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }
        private double _itog_price;
        public double ItogPrice
        {
            get => _itog_price;
            set
            {
                _itog_price = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Basket> _basket;
        public ObservableCollection<Basket> Basket
        {
            get => _basket;
            set
            {
                _basket = value;
                OnPropertyChanged();
            }
        }
        private int _itog_count;
        public int ItogCount
        {
            get => _itog_count;
            set
            {
                _itog_count = value;
                OnPropertyChanged();
            }
        }

    }
    public class Basket : BaseViewModel
    {
        public Basket() { }
        public Basket(int GoodId, int Count, byte[] file)
        {

            this.GoodId = GoodId;
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
            get => _id_order;
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
            get => _good_id;
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
    public class GoodSerialized
    {
        public int id { get; set; }
        public string name { get; set; }
        public string store_name { get; set; }
        public string type_name { get; set; }
        public int count { get; set; }
        public GoodSerialized(Good good, int count)
        {
            id = good.Id;
            name = good.Name;
            this.count = count;
            store_name = good.Store.StoreName;
            type_name = good.GoodType.TypeName;
        }
        public GoodSerialized() { }
    }
    public class Good : BaseViewModel
    {
        public Good()
        {
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
        private double _price;
        public double Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }
        private Pictures _pictures;

        public Pictures Pictures
        {
            get => _pictures;
            set
            {
                _pictures = value;
                OnPropertyChanged();
            }
        }
        private string _description;

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private int _good_type_id;

        public int GoodTypeId
        {
            get => _good_type_id;
            set
            {
                _good_type_id = value;
                OnPropertyChanged();
            }
        }

        private GoodType _good_type;

        public GoodType GoodType
        {
            get => _good_type;
            set
            {
                _good_type = value;
                OnPropertyChanged();
            }
        }

        private int _store_id;

        public int StoreId
        {
            get => _store_id;
            set
            {
                _store_id = value;
                OnPropertyChanged();
            }
        }

        private Store _store;

        public Store Store
        {
            get => _store;
            set
            {
                _store = value;
                OnPropertyChanged();
            }
        }

        protected int _count;

        public int Count
        {
            get => _count;
        }
        public void UpdateCountPrice(int count)
        {
            _count = count;
            Price = Price * _count;
        }
        public void UpdatePicId()
        {
            this.Pictures.IdGood = this.Id;
        }
        // public GoodType Type { get; private set; }
    }
    //Класс инициализации типа товара
    public class GoodType : BaseViewModel
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

        private string _type;

        public string TypeName
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged();
            }
        }

    }
    //Класс инициализации магазина
    public class Store : BaseViewModel
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

        private string _store;

        public string StoreName
        {
            get => _store;
            set
            {
                _store = value;
                OnPropertyChanged();
            }
        }
    }
    //Изображение
    public class Pictures : BaseViewModel
    {
        private int _id_good;

        public int IdGood
        {
            get => _id_good;
            set
            {
                _id_good = value;
                OnPropertyChanged();
            }
        }

        private string _file_name;
        public string FileName
        {
            get => _file_name;
            set
            {
                _file_name = value;
                OnPropertyChanged();
            }
        }

        private string _file_base64;

        public string FileBase64
        {
            get => _file_base64;
            set
            {
                _file_base64 = value;
                OnPropertyChanged();

            }
        }

        private byte[] _file_byte;

        public byte[] FileByte
        {
            get => _file_byte;
            set
            {
                _file_byte = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _image;

        public BitmapImage Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }
        public Pictures() { }
        public Pictures(string file_name, int good_id, string base64)
        {
            this.FileName = file_name;
            this.FileBase64 = base64;
            this.FileByte = Convert.FromBase64String(base64);
            this.IdGood = good_id;
        }
        public Pictures(string file)
        {
            this.FileName = Path.GetFileNameWithoutExtension(file);
            this.FileByte = File.ReadAllBytes(file);
            FileBase64 = Convert.ToBase64String(File.ReadAllBytes(file));
        }
        public void UpdatePhoto()
        {
            this.Image = ToImage(this.FileByte);
        }
        private BitmapImage ToImage(byte[] array)//Делаем из потока байтов картинку
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                try
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad; // here
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }
                catch
                {
                    return null;
                }
            }
        }
    } 
}
