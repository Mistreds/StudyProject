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

namespace StudyProject.ViewModel
{
    public class ReportMonth //класс для вывода отчета по месяцам
    {
        private string _month;
        public string Month
        {
            get => _month;

        }
        private int _year;
        public int Year
        {
            get => _year;

        }
        private MainReport _main_report;
        public MainReport MainReport { get => _main_report; }
        public ReportMonth(string month, DateTime Date1, DateTime Date2)
        {
            _month= month;
            _year = Date1.Year;
            _main_report = new MainReport(Date1, Date2); ;
        }


    }
    public class MainReport //класс для вывода отчета
    {
        private int _count;
        public int Count { get => _count; }//кол-во товаров
        private double _price;
        public double Price { get => _price; }// общая цена

        private GoodStatic _most_frequent_good;
        public GoodStatic MostFrequentGood //самый часто встречающийся товар
        { get => _most_frequent_good; }
        private ObservableCollection<GoodStatic> _good_list;
        public ObservableCollection<GoodStatic> GoodsList//список товара
        {
            get => _good_list;
        }
        private int _count_order;
        public int CountOrder { get => _count_order; }
        public void NewGoods()
        {
            _good_list = new ObservableCollection<GoodStatic>();
        }
        public MainReport(DateTime date1, DateTime date2)
        {
            using (var db = new ConnectDB())
            {
                date1 = new DateTime(date1.Year, date1.Month, date1.Day, 0, 0, 0);
                date2 = new DateTime(date2.Year, date2.Month, date2.Day, 23, 59, 59);
                int all_goods_count = db.Baskets.Include(p => p.Order).Where(p => p.Order.Date >= date1 && p.Order.Date <= date2).Sum(p => p.Count);
                var goods = db.Baskets.Include(p => p.Order).Where(p=>p.Order.Date>=date1 && p.Order.Date<=date2).GroupBy(p => p.GoodId).Select(grp => new { GoodId = grp.Key, Count = grp.Sum(p => p.Count) }).ToList();
                if (goods.Count == 0)
                {
                    _most_frequent_good = null;
                    return;
                }
                var max_good = goods.OrderByDescending(p => p.Count).FirstOrDefault();
                goods.Remove(max_good); 
                _most_frequent_good = new GoodStatic(max_good.GoodId, max_good.Count, all_goods_count);
                _count_order = db.Orders.AsQueryable().Where(p => p.Date >= date1 && p.Date <= date2).Count();
                _good_list = new ObservableCollection<GoodStatic>();
                foreach (var good in goods)
                {
                    _good_list.Add(new GoodStatic(good.GoodId, good.Count, all_goods_count));
                }
                _price = _good_list.Sum(p => p.Price)+_most_frequent_good.Price;
                _count= _good_list.Sum(p => p.Count) + _most_frequent_good.Count;
            }
        }
    }
    public class GoodStatic : Model.Good //класс, наследуемый от товара, с выводом частоты использования
    {
        private int _frequency;
        public int Frequency { get => _frequency; }
        private protected override void UpdateGood()
        {
            using (var db = new ConnectDB())
            {
                var good = db.Goods.AsQueryable().Include(p => p.Store).Include(p => p.GoodType).FirstOrDefault(p => p.Id == Id);
                this.Name = good.Name;
                this.Description = good.Description;
                this.Store = good.Store;
                this.GoodType = good.GoodType;
                this.StoreId = good.StoreId;
                this.Price = good.Price * _count;
                this.GoodTypeId = good.GoodTypeId;
                

            }
        }
        public GoodStatic(int id, int count, int all_goods_count)
        {
            this.Id = id;
            _count = count;
            var freq = (double)count / (double)all_goods_count;
            _frequency = Convert.ToInt32(freq* 100);
            UpdateGood();
            
        }

    }
    public class StatisticsViewModel : BaseViewModel
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
        public String[] Month = new String[] {"","January","February","March","April","May","June","July","August","September","October","November","December "};
        public ICommand CreateReport => new RelayCommand(() => 
        {
            MainReports = null;
            ReportMonth = null;
             MainReports = new MainReport(Date1, Date2);
            ReportMonth = new ObservableCollection<ReportMonth>();
           if(Date1.Year==Date2.Year)
            {
                if (Date1.Month != Date2.Month)
                {
                    var startDate = Date1;
                    var endDate = new DateTime(Date1.Year, Date1.Month, 1).AddMonths(1).AddDays(-1);
                    ReportMonth.Add(new ViewModel.ReportMonth(Month[startDate.Month], startDate, endDate));
                    for (int i = Date1.Month + 1; i < Date2.Month; i++)
                    {
                        var startDate1 = new DateTime(Date1.Year, i, 1);
                        var endDate1 = startDate1.AddMonths(1).AddDays(-1);
                        ReportMonth.Add(new ViewModel.ReportMonth(Month[i], startDate1, endDate1));
                    }
                    var startDate2 = new DateTime(Date2.Year, Date2.Month, 1);
                    var endDate2 = Date2;
                    ReportMonth.Add(new ViewModel.ReportMonth(Month[startDate2.Month], startDate2, endDate2));
                }
            }
            else
            {
                bool first = true;
                var startDate = Date1;
                var endDate = new DateTime(Date1.Year, Date1.Month, 1).AddMonths(1).AddDays(-1);   
                ReportMonth.Add(new ViewModel.ReportMonth(Month[startDate.Month], startDate, endDate));
                for (int i = Date1.Year ; i <= Date2.Year; i++)
                {
                    if(i!=Date2.Year)
                    {
                        if(first)
                        {
                            for (int j = Date1.Month; j <= 12; j++)
                            {
                                var startDate1 = new DateTime(i, j, 1);                             
                                var endDate1 = startDate1.AddMonths(1).AddDays(-1);              
                                ReportMonth.Add(new ViewModel.ReportMonth(Month[j], startDate1, endDate1));
                            }
                            first= false;
                        }
                        else
                        {
                            for (int j = 1; j < 12; j++)
                            {
                                var startDate1 = new DateTime(i, j, 1);
                                var endDate1 = startDate1.AddMonths(1).AddDays(-1);                           
                                ReportMonth.Add(new ViewModel.ReportMonth(Month[j], startDate1, endDate1));
                            }
                        }
                    }
                    else
                    {
                        for (int j = 1; j < Date2.Month; j++)
                        {
                            var startDate1 = new DateTime(i, j, 1);
                            var endDate1 = startDate1.AddMonths(1).AddDays(-1);
                            ReportMonth.Add(new ViewModel.ReportMonth(Month[j], startDate1, endDate1));
                        }
                    }
                }
                var startDate2 = new DateTime(Date2.Year, Date2.Month, 1);
                var endDate2 = Date2;
                ReportMonth.Add(new ViewModel.ReportMonth(Month[startDate2.Month], startDate2, endDate2));
            }
           if(ReportMonth!=null)
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
            IsOpenReport = true;
        });

        [Obsolete]
        public ICommand SaveAsPdf //сохранение отчета в виде пдф
            => new RelayCommand(() => 
        {

            Document document = new Document();
            Section section = document.AddSection();
            Paragraph head_par = new Paragraph();
            head_par.Format.Alignment = ParagraphAlignment.Center;
            head_par.Format.Font.Size = 16;
            head_par.Format.Font.Bold = true;
            var text_head=new Text("Отчет по покупкам");
            head_par.Add(text_head);
            section.PageSetup.BottomMargin = 10;//нижний отступ
            section.PageSetup.TopMargin = "2cm";//верхний отступ
            section.PageSetup.LeftMargin = "1.5cm";
            section.Add(head_par);
            Paragraph paragraph = new Paragraph();
            paragraph.Add(new Text("\nСводная информация"));
            paragraph.Format.Font.Size = 14;
            paragraph.Format.Font.Bold = true;
            section.Add(paragraph);
            paragraph.Format.Font.Color = Color.FromRgb(0, 0, 0);
            
            if (MainReports.MostFrequentGood != null)
            {
                //Таблица с сводным отчетом
                MigraDoc.DocumentObjectModel.Tables.Table table1 = new MigraDoc.DocumentObjectModel.Tables.Table();
                table1.Borders.Width = 1;
                table1.AddColumn("6cm");
                table1.AddColumn("6cm");
                table1.AddColumn("6cm");
                MigraDoc.DocumentObjectModel.Tables.Row row = table1.AddRow();
                MigraDoc.DocumentObjectModel.Tables.Row row1 = table1.AddRow();
                row.Format.Alignment = ParagraphAlignment.Left;
                row.Format.Font.Bold = false;
                var a = new Paragraph();
                a.AddText($"Всего товаров куплено: {MainReports.Count}");
                row.Cells[0].Add(a);
                a = new Paragraph();
                a.AddText($"Всего потрачено:  {MainReports.Price}");
                row.Cells[1].Add(a);
                a = new Paragraph();
                a.AddText($"Всего заказов: {MainReports.CountOrder}");
                row.Cells[2].Add(a);
                a = new Paragraph();
                a.AddText($"Самый популярный товар {MainReports.MostFrequentGood.Name}");
                row1.Cells[0].Add(a);
                a = new Paragraph();
                a.AddText($"Был куплен: {MainReports.MostFrequentGood.Count} раз {MainReports.MostFrequentGood.Frequency} % от общего кол-ва");
                row1.Cells[1].Add(a);
                a = new Paragraph();
                a.AddText($"Было потрачено: {MainReports.MostFrequentGood.Price}");
                row1.Cells[2].Add(a);
                section.Add(table1);
                if (MainReports.GoodsList.Count > 0)
                {
                    Paragraph most_tab_info = new Paragraph();
                    most_tab_info.Add(new Text("\nОстальные товары"));
                    most_tab_info.Format.Font.Size = 14;
                    most_tab_info.Format.Font.Bold = true;
                    section.Add(most_tab_info);
                    MigraDoc.DocumentObjectModel.Tables.Table MostCost = new MigraDoc.DocumentObjectModel.Tables.Table();
                    MostCost.AddColumn("6cm");
                    MostCost.AddColumn("6cm");
                    MostCost.AddColumn("6cm");
                    MostCost.Borders.Width = 1;
                    row = MostCost.AddRow();
                    foreach (var goods in MainReports.GoodsList)
                    {
                         row1 = MostCost.AddRow();

                        a = new Paragraph();
                        a.AddText($"Товар {goods.Name}");
                        row1.Cells[0].Add(a);
                        a = new Paragraph();
                        a.AddText($"Был куплен: {goods.Count} раз {goods.Frequency} % от общего кол-ва");
                        row1.Cells[1].Add(a);
                        a = new Paragraph();
                        a.AddText($"Было потрачено: {goods.Price}");
                        row1.Cells[2].Add(a);
                    }
                    section.Add(MostCost);
                }
                if (ReportMonth.Count > 0)
                {
                    //Таблица с отчетом по месяцам
                    MigraDoc.DocumentObjectModel.Tables.Table MonthTable = new MigraDoc.DocumentObjectModel.Tables.Table();
                    MonthTable.Borders.Width = 1;
                    MonthTable.AddColumn("6cm");
                    MonthTable.AddColumn("6cm");
                    MonthTable.AddColumn("6cm");
                    foreach (var rp in ReportMonth)
                    {
                        if (rp.MainReport.MostFrequentGood != null)
                        {
                            if (rp.MainReport.GoodsList != null)
                                rp.MainReport.GoodsList.Add(rp.MainReport.MostFrequentGood);
                            else
                            {
                                rp.MainReport.NewGoods();
                                rp.MainReport.GoodsList.Add(rp.MainReport.MostFrequentGood);
                            }
                            if (rp.MainReport.GoodsList.Count > 0)
                            {
                                row = MonthTable.AddRow();
                                row.Cells[0].MergeRight = 2;
                                var a1 = new Paragraph();
                                a1.AddText($"{rp.Month} {rp.Year}");
                                a1.Format.Font.Bold = true;
                                a1.Format.Alignment = ParagraphAlignment.Center;
                                row.Cells[0].Add(a1);
                                foreach (var goods in rp.MainReport.GoodsList)
                                {
                                    row1 = MonthTable.AddRow();

                                     a = new Paragraph();
                                    a.AddText($"Товар {goods.Name}");
                                    row1.Cells[0].Add(a);
                                    a = new Paragraph();
                                    a.AddText($"Был куплен: {goods.Count} раз {goods.Frequency} % от общего кол-ва");
                                    row1.Cells[1].Add(a);
                                    a = new Paragraph();
                                    a.AddText($"Было потрачено: {goods.Price}");
                                    row1.Cells[2].Add(a);
                                }
                            }

                        }


                    }
                    Paragraph most_tab_info = new Paragraph();
                    most_tab_info.Add(new Text("\nТаблица по месяцам"));
                    most_tab_info.Format.Font.Size = 14;
                    most_tab_info.Format.Font.Bold = true;
                    section.Add(most_tab_info);
                    section.Add(MonthTable);
                }
                var pdfRenderer = new PdfDocumentRenderer(true, PdfFontEmbedding.Always);
                var hist = _controls[3] as View.Statistics.HistogramStatistics;
                var char_stat = _controls[2] as View.Statistics.StatisticChartPage;
                char_stat.SaveChart();//cохранение графика в виде картинки
                hist.SaveChart();//сохранение гистограммы в виде картинки
                var image_sect = new Section();//Создание новой секции с изображением графиков
                image_sect.PageSetup.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
                var image1 = image_sect.AddImage("char_img.png");
                image1.Width = "28cm";
                image1.Height = "18cm";
                var image2 = image_sect.AddImage("histo_img.png");
                image2.Width = "28cm";
                image2.Height = "18cm";
                document.Add(image_sect);
                pdfRenderer.Document = document;
                long totalMemory = GC.GetTotalMemory(false);
 
                //очистка мусора и памяти, без этого выскакивает исключение out of memory
                GC.Collect();
                GC.WaitForPendingFinalizers();
                pdfRenderer.RenderDocument();
                pdfRenderer.PdfDocument.Save("D:\\1.pdf");// сохраняем
            }
           else
            {
                System.Windows.Forms.MessageBox.Show("Отчет не может быть сформирован, так как нет данных","Ошибка");
            }
         
        });
    }
}
