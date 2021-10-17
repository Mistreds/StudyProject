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

namespace StudyProject.ViewModel
{
  public class BasketViewModel:BaseViewModel
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
        private Model.Order _order;
        public Model.Order Order//заказ
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
            rules=new List<AssociationRule>();
            Order=new Model.Order();
            Order.Basket = new ObservableCollection<Model.Basket>();//связь заказа с товаром
            OftenOrder = new ObservableCollection<Model.Good>();
            DoThings();//получение списка прошлых покупок через алгоритма apriori
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
            var good_json = JsonSerializer.Deserialize<Model.GoodSerialized>(qr_json);
            var good=new Model.Good(good_json);
            AddToBasket(good,false);
            Order.Basket.Add(new Model.Basket(good.Id, good.Count, file));//связываем товар и заказ и добавляем qr код
        }
        public void AddToBasket(Model.Good good,bool file)
        {
            Order.ItogCount += good.Count;
            Order.ItogPrice += good.Price;
            GoodBasketList.Add(good);
            if(file)
                Order.Basket.Add(new Model.Basket(good.Id, good.Count));
            OftenOrder.Remove(OftenOrder.Where(p => p.Id == good.Id).FirstOrDefault());//если мы добавляем товар, который предложили, удаляем из предложеного
            var rul = rules.Where(p => GoodBasketList.Count >= 2 && p.Confidance >= 50 && String.Join(",", p.Label.Select(l => l).OrderBy(l => l).ToList()) == String.Join(",", GoodBasketList.Select(s => s.Id).OrderBy(s => s).ToList())).ToList();//находим в алгоритме apriori товары, которые покупают вместе с теми которые лежат в корзине
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
            using (var db = new ConnectDB())
            {
                Order.Date = DateTime.Now;
                db.Orders.Add(Order);
                db.SaveChanges();//добавляем заказ в бд
                foreach(var bask in Order.Basket)
                {
                    if (bask.QRstring == null)
                        continue;
                    Firebase.UploadQr(bask.Id, bask.QRstring);//загружаем qr код
                }
                GoodBasketList = new ObservableCollection<Model.Good>();
                Order = new Model.Order();
                Order.Basket = new ObservableCollection<Model.Basket>();
            }
        
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
        private void DoThings()
        {
            //алгоритм apriori, немного модернизировал его исходный код с гитхаба
            int Support = 3;
            var db=new ConnectDB();
            List<List<string>> goods_lists = db.Orders.AsQueryable().Include(p => p.Basket).Select(p => p.Basket.Select(s => s.GoodId.ToString()).ToList()).ToList();
            List<string> buck_id = GoodBasketList.Select(p => p.Id.ToString()).ToList();
            List<int> find_buck_id = new List<int>();
            BAL.Apriori apriori = new BAL.Apriori(goods_lists,buck_id);
            int k = 1;
            List<BAL.ItemSet> ItemSets = new List<BAL.ItemSet>();
            bool next;
            do
            {
                next = false;
                
                var L = apriori.GetItemSet(k, Support, IsFirstItemList: k == 1);
                Console.WriteLine("L"+L.Count);
                if (L.Count > 0)
                {
                    if (k != 1)
                        rules.AddRange(apriori.GetRules(L));
                    next = true;
                    k++;
                    ItemSets.Add(L);

                }
            } while (next);
            foreach(var a in ItemSets)
            {
                Console.WriteLine(a.Keys);
            }    
            
        }
        public  static  BitmapImage ToImage(byte[] array)//Делаем из потока байтов картинку
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
        #endregion
    }
}
