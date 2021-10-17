using System;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;

namespace StudyProject.Model
{

    //Класс инициализации товара
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
        public Good(GoodSerialized good_ser)
        {
            this.Id = good_ser.id;
            this._count = good_ser.count;
            using (var db=new ConnectDB())
            {
                var good = db.Goods.AsQueryable().Include(p=>p.Store).Include(p=>p.GoodType).FirstOrDefault(p => p.Id == Id);
                this.Name = good.Name;
                this.Description = good.Description;
                this.Store = good.Store;
                this.GoodType = good.GoodType;
                this.StoreId = good.StoreId;
                this.Price = good.Price * _count;
                this.GoodTypeId = good.GoodTypeId;
                var fire=new Firebase();
                this.Pictures =  fire.DownloadPictires(this.Id);
                this.Pictures.UpdatePhoto();

            }
        }
        public Good()
        {
        }
        public Good(int id)
        {
            this.Id = id;
            using (var db = new ConnectDB())
            {
                var good = db.Goods.AsQueryable().Include(p => p.Store).Include(p => p.GoodType).FirstOrDefault(p => p.Id == Id);
                this.Name = good.Name;
                this.Description = good.Description;
                this.Store = good.Store;
                this.GoodType = good.GoodType;
                this.StoreId = good.StoreId;
                this.Price = good.Price;
                this.GoodTypeId = good.GoodTypeId;
                var fire = new Firebase();
                this.Pictures = fire.DownloadPictires(this.Id);
                this.Pictures.UpdatePhoto();

            }
        }
        public Good(Good p)
        {
           using (var db=new ConnectDB())
           {
            this.Id = p.Id;
            this.Description = p.Description;
            this.Price = p.Price;
            this.StoreId = p.StoreId;
            if (p.Store == null)
            {
                this.Store = db.Stores.AsQueryable().FirstOrDefault(s => s.Id == StoreId);
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
                this.GoodType = db.GoodTypes.AsQueryable().FirstOrDefault(s => s.Id == GoodTypeId);
            }
            this.Name = p.Name;
            var fire=new Firebase();
            this.Pictures =  fire.DownloadPictires(this.Id);
           }
        }
        public Good(Good p, bool is_new)
        {
            using (var db = new ConnectDB())
            {
                this.Id = p.Id;
                this.Description = p.Description;
                this.Price = p.Price;
                this.StoreId = p.StoreId;
                if (p.Store == null)
                {
                    this.Store = db.Stores.AsQueryable().FirstOrDefault(s => s.Id == StoreId);
                }
                else
                {

                    this.Store = p.Store;
                }

                this.GoodTypeId = p.GoodTypeId;

                
                    this.GoodType = db.GoodTypes.AsQueryable().FirstOrDefault(s => s.Id == p.GoodTypeId);
                
                
                this.Name = p.Name;
                var fire = new Firebase();
                this.Pictures = p.Pictures;
                this.Pictures.UpdatePhoto();
            }
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
            get=> _store;
            set
            {
                _store = value;
                OnPropertyChanged();
            }
        }

        private int _count;

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
    public class Pictures:BaseViewModel
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
        public Pictures(){}
        public Pictures(string file_name,int good_id,string base64)
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
            FileBase64 =Convert.ToBase64String(File.ReadAllBytes(file));
        }
        public void UpdatePhoto()
        {
            this.Image = ViewModel.BasketViewModel.ToImage(this.FileByte);
        }
    }
}
