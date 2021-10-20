using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MigraDoc;
using PdfSharp;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.Windows.Input;
using BAL;

namespace StudyProject.ViewModel
{
    
    public class StatisticsViewModel : BE.BaseViewModel
    {
        private MainReport _main_report;
        public MainReport MainReports
        {
            get => _main_report;
            set
            {
                _main_report = value;
                OnPropertyChanged();
            }
        }
        private DateTime _date_1;
        public DateTime Date1
        {
            get => _date_1;
            set
            {
                _date_1 = value;
                OnPropertyChanged();
            }
        }
        private DateTime _date_2;
        public DateTime Date2
        {
            get => _date_2;
            set
            {
                _date_2 = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ReportMonth> reportMonths;
        public ObservableCollection<ReportMonth> ReportMonth
        {
            get => reportMonths;
            set
            {
                reportMonths = value;
                OnPropertyChanged();
            }
        }
        private bool _is_open_report;
        public bool IsOpenReport
        {
            get=> _is_open_report;
            set
            {
                if (value == true)
                {
                    MainControl = _controls[0];
                }
                _is_open_report = value;
                OnPropertyChanged();
            }
        }
        private bool _is_open_month;
        public bool IsOpenMonth
        {
            get => _is_open_month;
            set
            {
                if (value == true)
                {
                    MainControl = _controls[1];
                }
                _is_open_month = value;
                OnPropertyChanged();
            }
        }
            
        private bool _is_open_chart;
        public bool IsOpenChart
        {
            get => _is_open_chart;
            set
            {
                if(value==true)
                {
                    MainControl = _controls[2];
                }
                _is_open_chart = value;
                OnPropertyChanged();
            }
        }
        private bool _is_open_hist;
        public bool IsOpenHist
        {
            get => _is_open_hist;
            set
            {
                if (value == true)
                {
                    MainControl = _controls[3];
                }
                _is_open_hist = value;
                OnPropertyChanged();
            }
        }

        private UserControl _main_control;
        public UserControl MainControl
        {
            get=>_main_control;
            set
            {
                _main_control = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<UserControl> _controls;
        public StatisticsViewModel()
        {
            _controls = new ObservableCollection<UserControl> { new View.Statistics.GeneralStatistic(), new View.Statistics.MonthStatistic(), new View.Statistics.StatisticChartPage(),new View.Statistics.HistogramStatistics() };
            InitData();
        }
        private void InitData()
        {
            Date1 = DateTime.Now;
            Date2 = DateTime.Now;
        }
       
        public ICommand CreateReport => new RelayCommand(() => 
        {
             MainReports = new MainReport(Date1, Date2);

            IsOpenReport = true;
            ReportMonth = MainViewModel.BALimp.GetReportMonths(Date1, Date2);
            if (ReportMonth != null)
            {

                var a = _controls[2] as View.Statistics.StatisticChartPage;
                ReportMonth.OrderBy(p => p.Year);
                a.CreateChart1(ReportMonth);
                _controls[2] = a;
            }
            if (MainReports != null)
            {

                var a = _controls[3] as View.Statistics.HistogramStatistics;
                a.CreateChart1(MainReports);
                _controls[3] = a;
            }
        });

        [Obsolete]
        public ICommand SaveAsPdf //сохранение отчета в виде пдф
            => new RelayCommand(() => 
        {
            MainViewModel.BALimp.SaveAsPdf(Date1, Date2, ReportMonth, MainReports);
        });
    }
}
