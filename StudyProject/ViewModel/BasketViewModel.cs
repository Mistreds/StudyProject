using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System.Text.Json;
using BAL;
using Microsoft.EntityFrameworkCore;
using Prism.Commands;
using DAL;

namespace StudyProject.ViewModel
{
  public class BasketViewModel:BE.BaseViewModel
    {
        private ObservableCollection<Model.Good> _good_basket_list;
        public ObservableCollection<Model.Good> GoodBasketList//корзина с покупками
        {
            get => _good_basket_list;
            set
            {
                _good_basket_list = value;
                OnPropertyChanged();
            }
        }
        private BE.Order _order;
        public BE.Order Order//заказ
        {
            get => _order;
            set
            {
                _order = value;
                OnPropertyChanged();
            }
        }
        private List<AssociationRule> rules;
        private ObservableCollection<Model.Good> _often_order;
        public ObservableCollection<Model.Good> OftenOrder//предложенные товары
        {
            get => _often_order;
            set
            {
                _often_order = value;
                OnPropertyChanged();
            }
        }
        public BasketViewModel()
        {
            InitData();
        }     
        private void InitData()//инициализация данных
        {
            GoodBasketList=new ObservableCollection<Model.Good>();
            rules = MainViewModel.BALimp.GetAssociationRules(MainViewModel.DALimp.GetAllGoodsFromBasket());
            Order=new BE.Order();
            Order.Basket = new ObservableCollection<BE.Basket>();//связь заказа с товаром
            OftenOrder = new ObservableCollection<Model.Good>();
        }
        public ICommand ClearBasket => new DelegateCommand(InitData);
        #region  Command
        public ICommand AddGoodFromQr => new RelayCommand(() =>//Добавление товара из Qr кода
        {
            OpenFileDialog load = new OpenFileDialog(); //  load будет запрашивать у пользователя место, из которого он хочет загрузить файл.
            if (load.ShowDialog() == System.Windows.Forms.DialogResult.OK) // //если пользователь нажимает в обозревателе кнопку "Открыть".
            {
              var file=  File.ReadAllBytes(load.FileName);
              Bitmap bmp;
              using (var ms = new MemoryStream(file))
              {
                  bmp = new Bitmap(ms);
              }
              QRCodeDecoder decoder = new QRCodeDecoder(); // создаём "раскодирование изображения"
                try
                {
                    var a = decoder.Decode(new QRCodeBitmapImage(bmp));
                    AddToBasket(a, file);//добавляем товар в корзину
                }
              catch
                {
                    MessageBox.Show("Данное изображение не является QR кодом, или его невозможно распознать", "Ошибка");
                }
              
            }
        });
        private void AddToBasket(string qr_json, byte[] file)
        {
            var good_json = JsonSerializer.Deserialize<BE.GoodSerialized>(qr_json);
            var good=new Model.Good(good_json);
            AddToBasket(good,false);
            Order.Basket.Add(new BE.Basket(good.Id, good.Count, file));//связываем товар и заказ и добавляем qr код
        }
        public void AddToBasket(Model.Good good,bool file)
        {
            Order.ItogCount += good.Count;
            Order.ItogPrice += good.Price;
            GoodBasketList.Add(good);
            if(file)
                Order.Basket.Add(new BE.Basket(good.Id, good.Count));
            OftenOrder.Remove(OftenOrder.Where(p => p.Id == good.Id).FirstOrDefault());//если мы добавляем товар, который предложили, удаляем из предложеного
            var rul = MainViewModel.BALimp.GetFindAssotiat(GoodBasketList.Select(p => (BE.Good)p).ToList(), rules);
            List<int> new_coincidence = new List<int>();//Список id предложенных товаров
            foreach (var n_c in rul)
            {
                new_coincidence.AddRange(n_c.Label1.Select(p => Convert.ToInt32(p)).ToList());
            }
            if (new_coincidence.Count > 0)
            {

                foreach (var new_con in new_coincidence)
                {
                    OftenOrder.Add(new Model.Good(new_con));
                }
            }
        }
        public ICommand CreateOrder => new RelayCommand(() => { 
                MainViewModel.DALimp.AddOrder(Order);
                GoodBasketList = new ObservableCollection<Model.Good>();
                Order = new BE.Order();
                Order.Basket = new ObservableCollection<BE.Basket>();
            rules = MainViewModel.BALimp.GetAssociationRules(MainViewModel.DALimp.GetAllGoodsFromBasket());
        });
        public ICommand SelectOffenGoodCommand
        {
            get => new DelegateCommand<Model.Good>(SelectOffenGood);
        }
        private void SelectOffenGood(Model.Good good)
        {
            var AddOffenGood = new View.Basket.AddOffenGood(good);
            AddOffenGood.Show();
        }   
        #endregion
    }
}
