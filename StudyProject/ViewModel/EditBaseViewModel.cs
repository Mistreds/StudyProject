using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using StudyProject.Model;
using StudyProject.View.EditBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;

namespace StudyProject.ViewModel
{
    public class EditBaseViewModel:BaseViewModel
    {
        #region MainEditPageInit
        private UserControl _main_control;
        public UserControl MainControl
        {
            get => _main_control;
            set
            {
                _main_control = value;
                OnPropertyChanged();
            }
        }
        private bool _is_checked_open;
        public bool IsCheckedOpen
        {
            get=> _is_checked_open;
            set
            {
                _is_checked_open = value;
                if(_is_checked_open)
                {
                    MainControl = _controls[1];
                }
                else
                {
                    MainControl = _controls[0];
                }
                OnPropertyChanged();
            }
        }
        private List<UserControl> _controls;
        #endregion
        #region IntiModelData
        private ObservableCollection<Model.Store> _stores_list;
        public ObservableCollection<Model.Store> StoredList
        {
            get=> _stores_list;
            set
            {
                _stores_list = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Model.GoodType> _good_type_list;
        public ObservableCollection<Model.GoodType> GoodTypeList
        {
            get => _good_type_list;
            set
            {
                _good_type_list = value;
                OnPropertyChanged();
            }
        }
        #endregion
        #region GoodsData

        private Model.Good _add_good;

        public Good AddGood
        {
            get => _add_good;
            set
            {
                _add_good = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Model.Good> _goods_list;
        public ObservableCollection<Model.Good> GoodsList
        {
            get => _goods_list;
            set
            {
                _goods_list = value;
                OnPropertyChanged();
            }
        }
        #endregion
        public EditBaseViewModel()
        {
            
            _controls=new List<UserControl>{new EditStore(), new EditGoods()};
            IsCheckedOpen = true;
            InitData();
            
        }
        private void InitData()//Инициализация данных из БД при загрузке приложения
        {
            using (var db = new ConnectDB())
            {
                StoredList=new ObservableCollection<Store>(db.Stores.ToList());
                GoodTypeList = new ObservableCollection<GoodType>(db.GoodTypes.ToList());
                AddGood=new Good();
                _ = DownloadGoods();
            }
        }

        #region Command

        #region  GoodTypeStore

        public  ICommand UpdateStore=> new RelayCommand(() =>
        {
            using (var db=new ConnectDB())
            {
                db.Stores.UpdateRange(StoredList);
                db.SaveChanges();
                db.RemoveRange(db.Stores.AsQueryable().Where(p=>!StoredList.Select(s=>s.Id).Contains(p.Id)));
                db.SaveChanges();
            }
            
        });
        public  ICommand UpdateGoodType=> new RelayCommand(() =>
        {
            using (var db=new ConnectDB())
            {
                db.GoodTypes.UpdateRange(GoodTypeList);
                db.SaveChanges();
                db.RemoveRange(db.GoodTypes.AsQueryable().Where(p=>!GoodTypeList.Select(s=>s.Id).Contains(p.Id)));
                db.SaveChanges();
            }
            
        });
        public  ICommand CanselStore=> new RelayCommand(() =>
        {
            using (var db=new ConnectDB())
            {
                StoredList=new ObservableCollection<Store>(db.Stores.ToList());
            }
            
        });
        public  ICommand CanselType=> new RelayCommand(() =>
        {
            using (var db = new ConnectDB())
            {
                GoodTypeList = new ObservableCollection<GoodType>(db.GoodTypes.ToList());
            }
        });

        #endregion

        #region Goods

        public ICommand OpenPicture => new RelayCommand(() =>
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filter = "Image files(*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            myDialog.CheckFileExists = true;
            if (myDialog.ShowDialog() == true)
            {
                
               Model.Pictures pic=new Pictures(myDialog.FileName);
               //Firebase.UploadPictires(pic);
               AddGood.Pictures = pic;

            }

        });
        private async Task DownloadGoods()
        {
            await Task.Run(() => {
                using (var db = new ConnectDB())
                {
                    GoodsList = new ObservableCollection<Good>();
                    foreach (var good in db.Goods.Include(p=>p.Store).Include(p=>p.GoodType))
                    {
                        var good_new = new Good(good);
                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            GoodsList.Add(good_new);
                        });
                    }
                }
            });
            foreach(var good in GoodsList)
            {
                good.Pictures.UpdatePhoto();
            }
        }

        public ICommand CreateQRCodeCommand
        {
            get => new DelegateCommand<Good>(CreateQRCode);
        }

        private void CreateQRCode(Good good)
        {
            var qr_window=new View.EditBase.CreateQR(good);
            qr_window.Show();
        }
        public  ICommand AddGoodCommand=>new RelayCommand(() =>
        {
            using (var db=new ConnectDB())
            {
                db.Goods.Add(AddGood);
                db.SaveChanges();
                AddGood.UpdatePicId();
                Firebase.UploadPictires(AddGood.Pictures);
                GoodsList.Add(new Good(AddGood,true));
                AddGood=new Good();
            }
        });
        #endregion

        #endregion
    }
}