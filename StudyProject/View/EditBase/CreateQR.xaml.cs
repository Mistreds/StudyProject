using System.Text.Json;
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

using MessagingToolkit.QRCode.Codec;
using System.Drawing;
using System.Windows.Forms;
namespace StudyProject.View.EditBase
{
    /// <summary>
    /// Логика взаимодействия для CreateQR.xaml
    /// </summary>
    public partial class CreateQR : Window
    {

        private Model.Good good;
        public CreateQR(Model.Good good)
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
            var good_ser = new Model.GoodSerialized(good, Convert.ToInt32(GoodCount.Text));
            string json = JsonSerializer.Serialize<Model.GoodSerialized>(good_ser);
            QRCodeEncoder encoder = new QRCodeEncoder(); //создаем объект класса QRCodeEncoder
            Bitmap qrcode = encoder.Encode(json);
            SaveFileDialog save = new SaveFileDialog(); // save будет запрашивать у пользователя, место, в которое он захочет сохранить файл. 
            save.Filter = "PNG|*.png|JPEG|*.jpg|GIF|*.gif|BMP|*.bmp"; //создаём фильтр, который определяет, в каких форматах мы сможем сохранить нашу информацию. В данном случае выбираем форматы изображений. Записывается так: "название_формата_в обозревателе|*.расширение_формата")
            if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK) //если пользователь нажимает в обозревателе кнопку "Сохранить".
            {
                qrcode.Save(save.FileName);
            }
            this.Close();
        }
    }
}
