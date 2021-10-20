using BE;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
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
            _month = month;
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

                var dal = new DAL.DALimp();
                date1 = new DateTime(date1.Year, date1.Month, date1.Day, 0, 0, 0);
                date2 = new DateTime(date2.Year, date2.Month, date2.Day, 23, 59, 59);
                int all_goods_count = dal.CountBasketFromDate(date1, date2);
                List<(int GoodId, int Count)> goods = dal.GetGoodFromBasketDate(date1, date2);
                if (goods.Count == 0)
                {
                    _most_frequent_good = null;
                    return;
                }
                var max_good = goods.OrderByDescending(p => p.Count).FirstOrDefault();
                goods.Remove(max_good);
                _most_frequent_good = new GoodStatic(max_good.GoodId, max_good.Count, all_goods_count);
                _count_order = dal.CountOrderFromDate(date1, date2);
                _good_list = new ObservableCollection<GoodStatic>();
                foreach (var good in goods)
                {
                    _good_list.Add(new GoodStatic(good.GoodId, good.Count, all_goods_count));
                }
                _price = _good_list.Sum(p => p.Price) + _most_frequent_good.Price;
                _count = _good_list.Sum(p => p.Count) + _most_frequent_good.Count;
            
        }
    }
    public class GoodStatic : Good //класс, наследуемый от товара, с выводом частоты использования
    {
        private int _frequency;
        public int Frequency { get => _frequency; }
        private  void UpdateGood()
        {
            var dal = new DAL.DALimp();

            var good = dal.GetGoodById(Id);
                this.Name = good.Name;
                this.Description = good.Description;
                this.Store = good.Store;
                this.GoodType = good.GoodType;
                this.StoreId = good.StoreId;
                this.Price = good.Price * _count;
                this.GoodTypeId = good.GoodTypeId;


            
        }
        public GoodStatic(int id, int count, int all_goods_count)
        {
            this.Id = id;
            _count = count;
            var freq = (double)count / (double)all_goods_count;
            _frequency = Convert.ToInt32(freq * 100);
            UpdateGood();

        }

    }
    public class BALimp : IBal
    {
        public static String[] Month = new String[] { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December " };
        public List<AssociationRule> GetAssociationRules(List<List<string>> goods_lists)
        {
            int Support = 3;
            List<AssociationRule> rules = new List<AssociationRule>();
            BAL.Apriori apriori = new BAL.Apriori(goods_lists);
            int k = 1;
            List<BAL.ItemSet> ItemSets = new List<BAL.ItemSet>();
            bool next;
            do
            {
                next = false;

                var L = apriori.GetItemSet(k, Support, IsFirstItemList: k == 1);
                Console.WriteLine("L" + L.Count);
                if (L.Count > 0)
                {
                    if (k != 1)
                        rules.AddRange(apriori.GetRules(L));
                    next = true;
                    k++;
                    ItemSets.Add(L);

                }
            } while (next);
            return rules;
        }

        public List<AssociationRule> GetFindAssotiat(List<Good> GoodBasketList, List<AssociationRule> rules)
        {
            return rules.Where(p => GoodBasketList.Count >= 2 && p.Confidance >= 50 && String.Join(",", p.Label.Select(l => l).OrderBy(l => l).ToList()) == String.Join(",", GoodBasketList.Select(s => s.Id).OrderBy(s => s).ToList())).ToList();
        }

        public ObservableCollection<ReportMonth> GetReportMonths(DateTime Date1, DateTime Date2)
        {
            var ReportMonth = new ObservableCollection<ReportMonth>();
            if (Date1.Year == Date2.Year)
            {
                if (Date1.Month != Date2.Month)
                {
                    var startDate = Date1;
                    var endDate = new DateTime(Date1.Year, Date1.Month, 1).AddMonths(1).AddDays(-1);
                    ReportMonth.Add(new ReportMonth(Month[startDate.Month], startDate, endDate));
                    for (int i = Date1.Month + 1; i < Date2.Month; i++)
                    {
                        var startDate1 = new DateTime(Date1.Year, i, 1);
                        var endDate1 = startDate1.AddMonths(1).AddDays(-1);
                        ReportMonth.Add(new ReportMonth(Month[i], startDate1, endDate1));
                    }
                    var startDate2 = new DateTime(Date2.Year, Date2.Month, 1);
                    var endDate2 = Date2;
                    ReportMonth.Add(new ReportMonth(Month[startDate2.Month], startDate2, endDate2));
                }
            }
            else
            {
                bool first = true;
                var startDate = Date1;
                var endDate = new DateTime(Date1.Year, Date1.Month, 1).AddMonths(1).AddDays(-1);
                ReportMonth.Add(new ReportMonth(Month[startDate.Month], startDate, endDate));
                for (int i = Date1.Year; i <= Date2.Year; i++)
                {
                    if (i != Date2.Year)
                    {
                        if (first)
                        {
                            for (int j = Date1.Month; j <= 12; j++)
                            {
                                var startDate1 = new DateTime(i, j, 1);
                                var endDate1 = startDate1.AddMonths(1).AddDays(-1);
                                ReportMonth.Add(new ReportMonth(Month[j], startDate1, endDate1));
                            }
                            first = false;
                        }
                        else
                        {
                            for (int j = 1; j < 12; j++)
                            {
                                var startDate1 = new DateTime(i, j, 1);
                                var endDate1 = startDate1.AddMonths(1).AddDays(-1);
                                ReportMonth.Add(new ReportMonth(Month[j], startDate1, endDate1));
                            }
                        }
                    }
                    else
                    {
                        for (int j = 1; j < Date2.Month; j++)
                        {
                            var startDate1 = new DateTime(i, j, 1);
                            var endDate1 = startDate1.AddMonths(1).AddDays(-1);
                            ReportMonth.Add(new ReportMonth(Month[j], startDate1, endDate1));
                        }
                    }
                }
                var startDate2 = new DateTime(Date2.Year, Date2.Month, 1);
                var endDate2 = Date2;
                ReportMonth.Add(new ReportMonth(Month[startDate2.Month], startDate2, endDate2));
            }
            return ReportMonth;
        }

        [Obsolete]
        public void SaveAsPdf(DateTime Date1, DateTime Date2, ObservableCollection<ReportMonth> ReportMonth, MainReport MainReports)
        {
            if (MainReports == null)
            {
                System.Windows.Forms.MessageBox.Show("Отчет не может быть сформирован, так как нет данных", "Ошибка");
                return;
            }

            Document document = new Document();
            Section section = document.AddSection();
            Paragraph head_par = new Paragraph();
            head_par.Format.Alignment = ParagraphAlignment.Center;
            head_par.Format.Font.Size = 16;
            head_par.Format.Font.Bold = true;
            var text_head = new Text($"Отчет по покупкам c {Date1.ToString("dd-MM-yyyy")} по {Date2.ToString("dd-MM-yyyy")} "); ; ;
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
                System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog(); // save будет запрашивать у пользователя, место, в которое он захочет сохранить файл. 
                save.Filter = "PDF|*.pdf"; //создаём фильтр, который определяет, в каких форматах мы сможем сохранить нашу информацию. В данном случае выбираем форматы изображений. Записывается так: "название_формата_в обозревателе|*.расширение_формата")
                if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK) //если пользователь нажимает в обозревателе кнопку "Сохранить".
                {
                    try
                    {
                        pdfRenderer.PdfDocument.Save(save.FileName);
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show(e.Message, "Ошибка");
                    }
                }
                ;// сохраняем
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Отчет не может быть сформирован, так как нет данных", "Ошибка");
            }
        }
    }
    public interface IBal
    {
        ObservableCollection<ReportMonth> GetReportMonths(DateTime Date1, DateTime Date2);
        List<AssociationRule> GetAssociationRules(List<List<string>> goods_lists);
        List<AssociationRule> GetFindAssotiat(List<Good> goods, List<AssociationRule> rules);
        void SaveAsPdf(DateTime Date1, DateTime Date2, ObservableCollection<ReportMonth> ReportMonth, MainReport MainReports);
       
    }
}
