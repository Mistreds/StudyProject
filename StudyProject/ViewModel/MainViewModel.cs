using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using StudyProject.ViewModel;
namespace StudyProject
{
   public class MainViewModel:BaseViewModel
   {
       public static EditBaseViewModel EditBaseViewModel;
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
           EditBaseViewModel=new EditBaseViewModel();
           Controls=new List<UserControl>{new View.EditBase.MainEditPage()};
       }

       public ICommand OpenEditBase
       {
           get=>new RelayCommand(() =>
           {
               MainControl = Controls[0];

           });
       }
   }
}
