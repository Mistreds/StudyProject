using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace StudyProject.View.Statistics
{
    /// <summary>
    /// Логика взаимодействия для HistogramStatistics.xaml
    /// </summary>
    public partial class HistogramStatistics : UserControl
    {
        public HistogramStatistics()
        {
            InitializeComponent();

        }
        public void CreateChart1(BAL.MainReport reports)
        {
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            chart.Titles.Clear();
            chart.Legends.Clear();
            chart.ChartAreas.Add(new ChartArea("Default"));
            System.Windows.Forms.DataVisualization.Charting.Series series = new System.Windows.Forms.DataVisualization.Charting.Series("1");
            series.ChartArea = "Default";
            series.ChartType = SeriesChartType.Column;

            series.IsVisibleInLegend = true;
            List<string> axisXData = new List<string>();
            List<double> axisYData = new List<double>();
            if (reports.MostFrequentGood == null)
                return;
            var good_list = new ObservableCollection<BAL.GoodStatic>(reports.GoodsList);
            good_list.Add(reports.MostFrequentGood);

            if (good_list.Count == 0)
                return;
            foreach (var x in good_list)
            {
                axisXData.Add(x.Name);
                axisYData.Add(x.Price);

            }
            series.Points.DataBindXY(axisXData, axisYData);
            chart.Series.Add(series);


            SaveChart();


        }
        public void SaveChart()
        {
            var chars = chart;
            chars.Height = 1200;
            chars.Width = 1800;
            var fs = new FileStream(@"histo_img.png", FileMode.Create);

            chars.SaveImage(fs, ChartImageFormat.Png);
            fs.Close();
        }
    }
    }

