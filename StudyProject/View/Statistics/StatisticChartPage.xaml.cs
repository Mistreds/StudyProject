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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;
using System.Windows.Forms.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace StudyProject.View.Statistics
{
    /// <summary>
    /// Логика взаимодействия для StatisticChartPage.xaml
    /// </summary>
    public partial class StatisticChartPage : UserControl
    {
        public StatisticChartPage()
        {
            InitializeComponent();

        }
        public void CreateChart1(ObservableCollection<BAL.ReportMonth> reports)
        {
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            chart.Titles.Clear();
            chart.Legends.Clear();
            chart.ChartAreas.Add(new ChartArea("Default"));
            var year = reports.GroupBy(p => p.Year).Select(p => p.Key).ToList();
            foreach (var a in year)
            {
                System.Windows.Forms.DataVisualization.Charting.Series series = new System.Windows.Forms.DataVisualization.Charting.Series(a.ToString());
                series.BorderWidth = 3;
                series.ChartArea = "Default";
                series.ChartType = SeriesChartType.Line;
                chart.Legends.Add(new Legend(a.ToString()));

                series.Legend = a.ToString();
                chart.Legends[a.ToString()].Position.Auto = true;
                chart.Legends[a.ToString()].Alignment = System.Drawing.StringAlignment.Near;
                series.IsVisibleInLegend = true;
                List<string> axisXData = new List<string>();
                List<double> axisYData = new List<double>();
                foreach (var r in BAL.BALimp.Month.Where(p => !string.IsNullOrEmpty(p)))
                {
                    axisXData.Add(r);
                    var rep = reports.Where(p => p.Month == r && p.Year == a).FirstOrDefault();
                    if (rep != null)
                    {
                        axisYData.Add(rep.MainReport.Price);
                    }
                    else
                        axisYData.Add(0);
                }
                series.Points.DataBindXY(axisXData, axisYData);
                chart.Series.Add(series);
            }
            SaveChart();
        }
        public void SaveChart()
        {
            var chars = chart;
            chars.Height = 1200;
            chars.Width = 1800;
            var fs = new FileStream(@"char_img.png", FileMode.Create);

            chars.SaveImage(fs, ChartImageFormat.Png);
            fs.Close();
        }
    }
}
