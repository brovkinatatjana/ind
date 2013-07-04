using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MathParser;
using System.Text.RegularExpressions;
using Microsoft.Win32;  
using System.IO;
using System.IO.Log;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections.ObjectModel;
using Gif.Components;
using System.Reflection;
using Word = Microsoft.Office.Interop.Word;

namespace dih
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Charts.Series.Clear();
            if (File.Exists("log.file"))
            {
            }
            else
            {
                StreamWriter sw = new StreamWriter("log.file", true);
                sw.Close();
            }
        }

        int step = 1;
        string funk, variable;
        string flag = "textbox";
        string a1, b1, accurate1;
        string Path = @"D:\\";
        string path = "picture";
        double a, b, accurate, f_a, f_b, x;
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        double fix_a, fix_b, fix_accurate;
        StreamWriter sw = new StreamWriter(File.Open("log.file", FileMode.Append));
        public class ChartPoint
        {
            public double Value1 { get; set; }
            public double Value2 { get; set; }
        }

        private void FileMenu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HelpClick(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("\tПрограмма предназначена для решения нелинейных уравнений с заданной точностью.\n\tПоддерживается распознавание функций: abs, acos, asin, atan, cos, cosh, floor, ln, log, sign, sin, sinh, sqrt, tan, tanh. \n\tПоддерживается ввод данных из файла. Размещение данных в файле:\n 1-я строка - левая граница\n 2-я строка - правая граница\n 3-я строка - точность\n 4-я сторка - имя переменной\n 5-я строка - выражение", "Справка");
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            sw.WriteLine(DateTime.UtcNow.ToString() + "\tПользователь закрыл приложение");
            if (Directory.Exists(path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    file.Delete();
                }
            }
            sw.Close();
            this.Close();
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            string input;
            Parser p = new Parser();
            Parser pp = new Parser();
            a1 = t_a.Text;
            b1 = t_b.Text;
            accurate1 = t_accurate.Text;
            variable = t_variable.Text;
            funk = t_function.Text;
            input = funk;
            if ((funk != "") || (a1 != "") || (b1 != "") || (accurate1 != ""))
            {
                string sPattern = @"(^(\+|\-){0,1}\d+$)|(^(\+|\-){0,1}\d+(\.|\,){1}\d+(\*10\^{0,1}\({0,1}(\+|\-){0,1}\d*\){0,1}){0,1}$)|(^\({0,1}(\+|\-){0,1}\d+\/{1}\d+\){0,1}(\*10\^{0,1}\({0,1}(\+|\-){0,1}\d*\){0,1}){0,1}$)|(^(\+|\-){0,1}\d+(\*10\^{0,1}\({0,1}(\+|\-){0,1}\d*\){0,1}){0,1}$)|(^(10\^{0,1}\({0,1}(\+|\-){0,1}\d*\){0,1}){1}$)";
                if (Regex.IsMatch(a1, sPattern) && Regex.IsMatch(b1, sPattern) && Regex.IsMatch(accurate1, sPattern))
                {
                    string p1 = @"\.";
                    if (Regex.IsMatch(a1, p1))
                    {
                        a1 = Regex.Replace(a1, p1, ",");
                    }
                    if (Regex.IsMatch(b1, p1))
                    {
                        b1 = Regex.Replace(b1, p1, ",");
                    }
                    if (Regex.IsMatch(accurate1, p1))
                    {
                        accurate1 = Regex.Replace(accurate1, p1, ",");
                    }
                    string p2 = @"abs(.*)|acos(.*)|asin(.*)|atan(.*)|cos(.*)|cosh(.*)|floor(.*)|ln(.*)|log(.*)|sign(.*)|sin(.*)|sinh(.*)|qrt(.*)|tan(.*)|tanh(.*)";
                    if (Regex.IsMatch(variable, p2))
                    {
                        System.Windows.MessageBox.Show("Недопустимое имя переменной", "Ошибка!");
                    }
                    else
                    {
                        if (p.Evaluate(a1))
                        {
                            a = p.Result;
                        }
                        if (p.Evaluate(b1))
                        {
                            b = p.Result;
                        }
                        if (p.Evaluate(accurate1))
                        {
                            accurate = p.Result;
                        }
                        sw.WriteLine(DateTime.UtcNow.ToString() + "\tПользователь ввел данные:\ta=" + a1 + "\tb=" + b1 + "\tточность" + accurate.ToString() + "\tf(" + variable + ")=" + funk);
                        if (p.Evaluate(Regex.Replace(input, variable, "(" + a.ToString() + ")")) && pp.Evaluate(Regex.Replace(input, variable, "(" + b.ToString() + ")")))
                        {
                            f_a = p.Result;
                            f_b = pp.Result;
                            if (f_a * f_b < 0)
                            {
                                start.Visibility = Visibility.Collapsed;
                                Charts.Visibility = Visibility.Visible;
                                progressBar1.Visibility = Visibility.Visible;
                                if ((s_picture.IsChecked == true) || (s_word.IsChecked == true) || (s_txt.IsChecked == true))
                                {
                                    save_path();
                                }
                                if (s_picture.IsChecked == true)
                                {
                                    if (!(Directory.Exists(Path+path)))
                                    {
                                        Directory.CreateDirectory(Path + path);
                                    }
                                    DirectoryInfo dirInfo = new DirectoryInfo(Path + path);
                                    foreach (FileInfo file in dirInfo.GetFiles())
                                    {
                                        file.Delete();
                                    }
                                }
                                fix_a = a;
                                fix_b = b;
                                fix_accurate = accurate; 
                                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                                dispatcherTimer.Tick += new EventHandler(dichotomia);
                                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                                dispatcherTimer.Start();
                            }
                            else
                            {
                                sw.WriteLine(DateTime.UtcNow.ToString() + "\tНа заданном промежутке корней нет или их несколько. При заданных параметрах метод не применим.");
                                System.Windows.MessageBox.Show("На заданном промежутке корней нет или их несколько. При заданных параметрах метод не применим.", "Ошибка!");
                            }


                        }
                        else
                        {
                            sw.WriteLine(DateTime.UtcNow.ToString() + "\tВведенная функция не может быть распознана");
                            System.Windows.MessageBox.Show("Введенная функция не может быть распознана. Проверьте правильность ввода.", "Ошибка!");
                        }
                    }


                }
                else
                {
                    sw.WriteLine(DateTime.UtcNow.ToString() + "\tДанные введены некорректно (неизвестный формат)");
                    System.Windows.MessageBox.Show("Данные введены некорректно (неизвестный формат)", "Ошибка!");
                }
            }
            else
            {
                sw.WriteLine(DateTime.UtcNow.ToString() + "\tДанные не были введены");
                System.Windows.MessageBox.Show("Введите данные", "Ошибка!");
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }


        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (radioButton_file.IsChecked == true)
            {
                sw.WriteLine(DateTime.UtcNow.ToString() + "\tПользователь выбрал считывание данных из файла");
                flag = "file";
            }
            if (radioButton_TextBox.IsChecked == true)
            {
                sw.WriteLine(DateTime.UtcNow.ToString() + "\tПользователь выбрал ввод данных вручную");
                flag = "textbox";
            }
            get_data();
        }


        public void dichotomia(object sender, EventArgs e)
        {
            double t = 0, min, max, index, flag1 = 1;
            string input;
            Random random = new Random();
            Parser p = new Parser();
            Parser pp = new Parser();
            if (p.Evaluate(Regex.Replace(funk, variable, "(" + a.ToString() + ")")) && pp.Evaluate(Regex.Replace(funk, variable, "(" + b.ToString() + ")")))
            {
                f_a = p.Result;
                f_b = pp.Result;
            }
            x = (a + b) / 2;
            min = a;
            max = b;
            LineSeries NewChart = new LineSeries();
            ObservableCollection<ChartPoint> C1 = new ObservableCollection<ChartPoint> { };
            index = a;
            while (index <= b && flag1 == 1)
            {
                input = Regex.Replace(funk, variable, "(" + index.ToString() + ")");
                if (p.Evaluate(input))
                {
                    t = p.Result;
                }
                if (t * 0 == 0)
                {
                    C1.Add(new ChartPoint { Value1 = t, Value2 = index });
                    if (t > max)
                        max = t;
                    if (t < min)
                        min = t;
                }
                else
                {
                    System.Windows.MessageBox.Show("Разрыв функции", "Ошибка!");
                    progressBar1.Visibility = Visibility.Collapsed;
                    dispatcherTimer.Stop();
                    flag1 = 0;
                    Charts.Visibility = Visibility.Collapsed;
                }
                index = index + Math.Abs(b - a) / 100;
            }
            if (flag1 != 0)
            {
                Y.Minimum = min - Math.Abs(b - a) / 100;
                X.Minimum = min - Math.Abs(b - a) / 100;
                Y.Maximum = max + Math.Abs(b - a) / 100;
                X.Maximum = max + Math.Abs(b - a) / 100;
                NewChart.ItemsSource = C1;
                NewChart.DependentValuePath = "Value1";
                NewChart.IndependentValuePath = "Value2";
                System.Windows.Style style = new System.Windows.Style(typeof(LineDataPoint));
                style.Setters.Add(new Setter(LineDataPoint.TemplateProperty, null));
                Color background = Color.FromRgb((byte)random.Next(255), (byte)random.Next(255), (byte)random.Next(255));
                style.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(background)));
                NewChart.DataPointStyle = style;
                Charts.Series.Add(NewChart);
                DriveInfo drv = new DriveInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                if (drv.AvailableFreeSpace > 1048576)
                {
                    if (s_picture.IsChecked == true)
                    {
                        if (Charts.Series[0] == null)
                        {
                            System.Windows.MessageBox.Show("there is nothing to export");
                        }
                        else
                        {
                            Rect bounds = VisualTreeHelper.GetDescendantBounds(Charts);

                            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);

                            DrawingVisual isolatedVisual = new DrawingVisual();
                            using (DrawingContext drawing = isolatedVisual.RenderOpen())
                            {
                                drawing.DrawRectangle(Brushes.White, null, new Rect(new System.Windows.Point(), bounds.Size)); // Optional Background
                                drawing.DrawRectangle(new VisualBrush(Charts), null, new Rect(new System.Windows.Point(), bounds.Size));
                            }

                            renderBitmap.Render(isolatedVisual);

                            Microsoft.Win32.SaveFileDialog uloz_obr = new Microsoft.Win32.SaveFileDialog();
                            uloz_obr.FileName =Path+path+ "\\Graf" + step.ToString() + ".png";
                            uloz_obr.DefaultExt = "png";
                            string obr_cesta = uloz_obr.FileName;
                            using (FileStream outStream = new FileStream(obr_cesta, FileMode.Create))
                            {
                                PngBitmapEncoder encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                                encoder.Save(outStream);
                            }
                        }
                    }
                }
                else
                {
                    l_s.Visibility = Visibility.Visible;
                    l_s.Content = "Сохранение изображений не возможно";
                }
                progressBar1.UpdateLayout();
                input = Regex.Replace(funk, variable, "(" + x.ToString() + ")");
                if (p.Evaluate(input))
                {
                    t = p.Result;
                    if ((Math.Abs(t) > accurate) && (f_b * f_a < 0))
                    {
                        if (t * f_a < 0)
                            b = x;
                        else
                            a = x;
                    }
                    else
                    {
                        progressBar1.Visibility = Visibility.Collapsed;
                        dispatcherTimer.Stop();
                        
                        l_s.Content = variable + "=" + x.ToString();
                        l_s.Visibility = Visibility.Visible;
                        //System.Windows.MessageBox.Show(variable + "=" + x.ToString(), "Решение");
                        if (s_picture.IsChecked == true)
                        {
                            sw.WriteLine(DateTime.UtcNow.ToString() + "\tВыбрано сохранение графика");
                            //gif();
                        }
                        if (s_word.IsChecked == true)
                        {
                            sw.WriteLine(DateTime.UtcNow.ToString() + "\tВыбрано сохранение в Word");
                            save_word(x);
                        }
                        if (s_txt.IsChecked == true)
                        {
                            sw.WriteLine(DateTime.UtcNow.ToString() + "\tВыбрано сохранение в txt");
                            save_txt(x);
                        }


                    }
                }
                else
                {

                    System.Windows.MessageBox.Show("Невозможно выполнить вычисления.", "Ошибка!");
                }

                step++;
            }
        }

        public void get_data()
        {
            if (flag == "file")
            {
                try
                {
                    string filename = "";
                    Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog() { Filter = "Текстовые файлы(*.txt)|*.txt" };
                    if (openFileDialog1.ShowDialog() != null)
                    {
                        filename = openFileDialog1.FileName;
                        FileStream stream = new FileStream(filename, FileMode.Open);
                        StreamReader reader = new StreamReader(stream);
                        t_a.Text = reader.ReadLine();
                        t_b.Text = reader.ReadLine();
                        t_accurate.Text = reader.ReadLine();
                        t_variable.Text = reader.ReadLine();
                        t_function.Text = reader.ReadLine();
                        t_function.ToolTip = "Уравнение, считаннное из файла";
                        stream.Close();
                        t_function.Visibility = Visibility.Visible;
                        canvas1.Visibility = Visibility.Visible;
                        start.Visibility = Visibility.Visible;
                        radioButton_file.Visibility = Visibility.Collapsed;
                        radioButton_TextBox.Visibility = Visibility.Collapsed;
                        next.Visibility = Visibility.Collapsed;
                        s_word.Visibility = Visibility.Visible;
                        s_picture.Visibility = Visibility.Visible;
                        s_txt.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Файл не выбран", "Ошибка!");
                    }
                }
                catch
                {
                }
            }
            else
            {
                t_function.Visibility = Visibility.Visible;
                canvas1.Visibility = Visibility.Visible;
                start.Visibility = Visibility.Visible;
                radioButton_file.Visibility = Visibility.Collapsed;
                radioButton_TextBox.Visibility = Visibility.Collapsed;
                next.Visibility = Visibility.Collapsed;
                s_word.Visibility = Visibility.Visible;
                s_picture.Visibility = Visibility.Visible;
                s_txt.Visibility = Visibility.Visible;
            }
        }


        private void reset_Click(object sender, RoutedEventArgs e)
        {
            sw.WriteLine(DateTime.UtcNow.ToString() + "\tПользовательперезапустил приложение");
            Charts.Series.Clear();
            dispatcherTimer.Stop();
            t_function.Visibility = Visibility.Collapsed;
            canvas1.Visibility = Visibility.Collapsed;
            start.Visibility = Visibility.Collapsed;
            radioButton_file.Visibility = Visibility.Visible;
            radioButton_TextBox.Visibility = Visibility.Visible;
            next.Visibility = Visibility.Visible;
            Charts.Visibility = Visibility.Collapsed;
            progressBar1.Visibility = Visibility.Collapsed;
            s_word.Visibility = Visibility.Collapsed;
            s_picture.Visibility = Visibility.Collapsed;
            s_txt.Visibility = Visibility.Collapsed;
            l_s.Visibility = Visibility.Collapsed;
            flag = "textbox";
            t_function.Text = "";
            t_a.Text = "";
            t_b.Text = "";
            t_accurate.Text = "";
            t_variable.Text = "";
            funk = "";
            variable = "";
            a1 = "";
            b1 = "";
            accurate1 = "";
            step = 1;
            path = "picture";
            s_picture.IsChecked = false;
            s_txt.IsChecked = false;
            s_word.IsChecked = false;
        }

        public void gif()
        {
            int index;
            path = "picture";
            String outputFilePath = Path + "Решение.gif";
            AnimatedGifEncoder e = new AnimatedGifEncoder();
            e.Start(outputFilePath);
            e.SetDelay(500);
            e.SetRepeat(0);
            if (Directory.Exists(Path + path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Path + path);
                int tt = dirInfo.GetFiles().Length;
                for (index = 1; index <= tt; index++)
                {
                    e.AddFrame(System.Drawing.Image.FromFile(Path+path+"\\Graf" + index.ToString() + ".png"));
                }
            }
            e.Finish();
            
        }
        public void save_word(double x)
        {
            Word.Application wordApplication = new Word.Application(); //объявили переменную типа Word
            Object template = Type.Missing;
            Object newTemplate = Type.Missing;
            Object documentType = Type.Missing;
            Object visible = Type.Missing;
            wordApplication.Documents.Add(ref template, ref newTemplate, ref documentType, ref visible);//добавили в проложение документ
            Word.Document doc = wordApplication.ActiveDocument;
            //wordApplication.Visible = true; //делаем что бы word не работал в фоновом режиме
            Object r = Type.Missing;
            Word.Paragraph par = doc.Content.Paragraphs.Add(ref r);//дабавляем в документ параграф
            Object missing = Type.Missing;
            Word.Range rng = doc.Range(ref missing, ref missing); //получаем текстовую область параграфа
            rng.Tables.Add(doc.Paragraphs[doc.Paragraphs.Count].Range, 6, 1, ref missing, ref missing);//вставляем в текстовую область таблицу
            Word.Table tbl = doc.Tables[doc.Tables.Count];//для удобства работы присваиваем таблицу переменной
            tbl.Cell(1, 1).Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            tbl.Cell(1, 1).Range.Text = "МЕТОД ПОЛОВИННОГО ДЕЛЕНИЯ ДЛЯ РЕШЕНИЯ НЕЛИНЕЙНЫХ УРАВНЕНИЙ";
            tbl.Cell(2, 1).Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            tbl.Cell(2, 1).Range.Text = "Вы ввели уравнение f(" + variable + ")=" + funk;
            tbl.Cell(3, 1).Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            tbl.Cell(3, 1).Range.Text = "Левая граница = " + fix_a;
            tbl.Cell(4, 1).Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            tbl.Cell(4, 1).Range.Text = "Правая граница = " + fix_b;
            tbl.Cell(5, 1).Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            tbl.Cell(5, 1).Range.Text = "Точность = " + accurate.ToString();
            tbl.Cell(6, 1).Range.Paragraphs.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            tbl.Cell(6, 1).Range.Text = "Ответ: " + variable + " = " + x.ToString();
            object fileName = Path + @"Решение.doc";
            doc.SaveAs(ref fileName,
            ref missing, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing, ref missing, ref missing,
            ref missing, ref missing, ref missing, ref missing, ref missing);
            doc.Close(ref missing, ref missing, ref missing);
            wordApplication.Quit();
        }

        public void save_txt(double x)
        {
            StreamWriter sw1 = new StreamWriter(Path + "Решение.txt", false);
            sw1.WriteLine("МЕТОД ПОЛОВИННОГО ДЕЛЕНИЯ ДЛЯ РЕШЕНИЯ НЕЛИНЕЙНЫХ УРАВНЕНИЙ\r\nВы ввели уравнение f(" + variable + ")=" + funk + "\r\nЛевая граница = " + fix_a + "\r\nПравая граница = " + fix_b + "\r\nТочность = " + accurate.ToString() + "\r\nОтвет: " + variable + " = " + x.ToString());
            sw1.Close();
        }
        public void save_path()
        {

            System.Windows.Forms.FolderBrowserDialog OpenFolder = new System.Windows.Forms.FolderBrowserDialog();
            // Показываем надпись в наверху диалога. 
            OpenFolder.Description = "Выбор каталога";
            // Выбираем первоначальную папку. 
            OpenFolder.SelectedPath = @"D:\";
            if (OpenFolder.ShowDialog() != 0)
            {
                Path = OpenFolder.SelectedPath;
            }
        }
    }
}
