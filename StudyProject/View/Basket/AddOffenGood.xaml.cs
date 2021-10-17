using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StudyProject.View.Basket
{
    /// <summary>
    /// Логика взаимодействия для AddOffenGood.xaml
    /// </summary>
    public partial class AddOffenGood : Window
    {
        private Model.Good good;
        public AddOffenGood(Model.Good good)
        {
            InitializeComponent();
            this.good = good;
        }
        private void GoodCountOnlyDigit(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0) || (e.Text == ".")
               && (!GoodCount.Text.Contains(".")
               && GoodCount.Text.Length != 0)))
            {
                e.Handled = true;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            good.UpdateCountPrice(Convert.ToInt32(GoodCount.Text));
            MainViewModel.BasketViewModel.AddToBasket(good, true);
            this.Close();
        }
    }
}
