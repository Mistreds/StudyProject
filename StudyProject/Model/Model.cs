using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyProject.Model
{
    
    //Класс инициализации товара
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
        //Возможно нужно будет удалить потом, или поменять логику, но пока оставлю так

        private double _quantity;

        public double Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }
        private int _pictures_id;

        public int PicturesId
        {
            get => _pictures_id;
            set
            {
                _pictures_id = value;
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
        // public GoodType Type { get; private set; }
    }



//Класс инициализации типа товара
    public class GoodType:BaseViewModel
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
    public class Store:BaseViewModel
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
//Класс хранения картинок
    public class Pictures
    {
        
    }
}
