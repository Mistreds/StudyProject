using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using StudyProject.Model;
using StudyProject.View.EditBase;

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
            }
        }
    }
}