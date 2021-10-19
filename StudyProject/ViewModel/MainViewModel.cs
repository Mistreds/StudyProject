using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using StudyProject.View.Basket;
using StudyProject.ViewModel;
namespace StudyProject
{
    public class MainViewModel : BaseViewModel//главный viewmodel
    {
        //Тут я разделяю для удобства логику работы c ViewModel
        public static EditBaseViewModel EditBaseViewModel;//ViewModel для работы с модулем "редактирование базы"
        public static BasketViewModel BasketViewModel;//ViewModel для работы с модулем "корзина"
        public static StatisticsViewModel StatisticsViewModel;//ViewModel для работы с модулем "статистика"
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

        private List<UserControl> Controls;
        public MainViewModel()
        {
            var firebase = new Firebase();
            EditBaseViewModel = new EditBaseViewModel();
            BasketViewModel = new BasketViewModel();
            StatisticsViewModel=new StatisticsViewModel();
            Controls = new List<UserControl> { new View.EditBase.MainEditPage(), new MainBasketPage(), new View.Statistics.StatisticsMain() };

        }
        public ICommand OpenEditBase
        {
            get => new RelayCommand(() =>
            {
                MainControl = Controls[0];

            });
        }
        public ICommand OpenBasket
        {
            get => new RelayCommand(() =>
            {
                MainControl = Controls[1];

            });
        }
        public ICommand OpenStatistic
        {
            get => new RelayCommand(() =>
            {
                MainControl = Controls[2];

            });
        }
    }
}
