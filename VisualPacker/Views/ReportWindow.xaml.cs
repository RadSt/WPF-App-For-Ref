using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using VisualPacker.Models;
using VisualPacker.ViewModels;

namespace VisualPacker.Views
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    /// 
    
    public partial class ReportWindow : Window
    {
        public ObservableCollection<Vehicle> vehicles;
        public List<VerticalBlock> vBlocks;
        public List<RowBlock> rBlocks;
        public List<Container> containers;
        public int scale = 6;
        //public ReportWindow(ObservableCollection<Vehicle> Data)
        public ReportWindow(Object Data)
        {
            InitializeComponent();
           
                if (Data is ObservableCollection<Vehicle>)
                {   vehicles =(ObservableCollection<Vehicle>) Data;
                ShowVehicles();}
                else if (Data is List<VerticalBlock>)
                { vBlocks=(List<VerticalBlock>)Data;
                ShowVerticalBlocks();}
                    else if (Data is List<RowBlock>)
                { rBlocks=(List<RowBlock>)Data;
                ShowRowBlocks();
                }
                else if (Data is List<Container>)
                {
                    containers = (List<Container>)Data;
                    ShowContainers();
                }
                else { MessageBox.Show("В форму отчета передан неверный тип данных:" + Data.GetType()); }
                //System.Collections.ObjectModel.ObservableCollection<Vehicle>
           // MessageBox.Show(Data.GetType().ToString() );

                
        }
        public void print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = flowDocViewer.Document;
                doc.PagePadding = new Thickness(20, 40, 20, 40);
                doc.PageHeight = printDialog.PrintableAreaHeight;
                doc.PageWidth = printDialog.PrintableAreaWidth-40;
                doc.ColumnWidth = printDialog.PrintableAreaWidth - 40;
                doc.ColumnGap = 0;
                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDocViewer.Document).DocumentPaginator
                ,"Печать" );
            }
        }
        public void AddMainHeader(FlowDocument doc, String text)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.FontSize = 16;
            p.FontStyle = FontStyles.Italic;
            p.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(p);
        }
        public void AddHeader(FlowDocument doc, String text)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.FontSize = 20;
            p.FontStyle = FontStyles.Italic;
            p.TextAlignment = TextAlignment.Left;
            doc.Blocks.Add(p);
        }
        public void AddSmallContainers(Table tab, List<Container> tempList, int i, int i2)
        {
             //печатаем заголовок
            int i3 = 1;
                tab.RowGroups[0].Rows.Add(new TableRow());
                TableRow currentRow = tab.RowGroups[0].Rows[i2+i3];
                currentRow.Background = Brushes.White;
                currentRow.FontSize = 18;
                currentRow.FontWeight = FontWeights.Normal;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Шаг " + i + ": Загрузите следующие контейнеры:"))));
                currentRow.Cells[0].ColumnSpan = 2;
                foreach (Container c in tempList)
                {
                    i3++;
                    tab.RowGroups[0].Rows.Add(new TableRow());
                    currentRow = tab.RowGroups[0].Rows[i2+i3];
                    currentRow.Background = Brushes.White;
                    currentRow.FontSize = 14;
                    currentRow.FontWeight = FontWeights.Normal;
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run(c.Name + ": " + c.Vgh + "; " + c.Mass + " кг."))));
                    currentRow.Cells[0].ColumnSpan = 2;
                }            
        }
        public void AddCanvas(FlowDocument doc, VerticalBlock vBlock, int cWidth,int cHeight)
         {
             //MessageBox.Show("Количество вертикальных блоков " + rowBlock.Blocks.Count.ToString());
            BlockUIContainer b = new BlockUIContainer();
            Canvas canvas = new Canvas();
            canvas.Width = cWidth / scale;
            canvas.Height = cHeight / scale + 20;

            //Рисуем рамку вокруг canvas
            Rectangle r = new Rectangle();
            r.Width = cWidth / scale;
            r.Height = cHeight / scale;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            r.Stroke = new SolidColorBrush(Colors.Red);
            //r.Fill = new SolidColorBrush(Colors.Black);
            r.Fill = brush;
            Canvas.SetLeft(r, 0);
            Canvas.SetTop(r, 20);
            canvas.Children.Add(r);

            //пишем заголовок
            TextBlock t = new TextBlock();
            t.Text = "Количество контейнеров: " + vBlock.Count + " общий вес: " + vBlock.Mass ;
            t.FontSize = 14;
            Canvas.SetLeft(t,20);
            Canvas.SetTop(t, 2);
            canvas.Children.Add(t);

            vBlock.SetFirstPointVerticalBlock(new Point3D(0, 0, 0));
            //рисуем контейнеры
                 foreach (Container c in vBlock.Blocks)
                     {

                         r = new Rectangle();
                         r.Width = c.Length / scale;
                         r.Height = c.Height / scale;
                         brush = new SolidColorBrush();
                         brush = Brushes.White;
                          r.Stroke = new SolidColorBrush(Colors.Black);
                          //r.Fill = new SolidColorBrush(Colors.Black);
                         r.Fill = brush;
                         Canvas.SetLeft(r, c.FirstPoint.Y / scale);
                         Canvas.SetTop(r, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale);
                         canvas.Children.Add(r);

                         t = new TextBlock();
                         t.Text = c.Name;
                         t.FontSize = 10;
                         Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                         Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 2);
                         canvas.Children.Add(t);

                         t = new TextBlock();
                         t.Text = "Габ: "+c.Length+"x"+c.Width+"x"+c.Height+"мм.Вес:"+c.Mass;
                         t.FontSize = 10;
                         Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                         Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 15);
                         canvas.Children.Add(t);

                         t = new TextBlock();
                         t.Text = "Цена: " + c.Price + "; Направление загрузки д: " + c.DirLength + "; ш: " + c.DirWidth + "; в: " + c.DirHeight+ "; ";
                         t.FontSize = 10;
                         Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                         Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 28);
                         canvas.Children.Add(t);

                         t = new TextBlock();
                         t.Text = "Порядок загрузки + " + c.Order + "; Допуст.давление: " + c.PressHeight + "; Хрупкость: " + c.FragilityHeight + "; ";
                         t.FontSize = 10;
                         Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                         Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 41);
                         canvas.Children.Add(t);

                       t = new TextBlock();
                         t.Text = "Уровень: " + c.Level + "; Количество: " + c.Quantity + "; На пол: " + c.Only4Bottom + "; ";
                         t.FontSize = 10;
                         Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                         Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 54);
                         canvas.Children.Add(t);
                        // break;
                     }
                     //break;
                  b.Child = canvas;
                  doc.Blocks.Add(b);
        }
        public void AddContainer(FlowDocument doc, Container c)
        {
             BlockUIContainer b = new BlockUIContainer();
            Canvas canvas = new Canvas();
            canvas.Width = c.Width / scale;
            canvas.Height = c.Height / scale;

                      
               Rectangle r = new Rectangle();
               r.Width = c.Length / scale;
               r.Height = c.Height / scale;
                Brush brush = new SolidColorBrush();
                brush = Brushes.White;
                r.Stroke = new SolidColorBrush(Colors.Black);
                r.Fill = brush;
                Canvas.SetLeft(r, c.FirstPoint.Y / scale);
                Canvas.SetTop(r, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale);
                canvas.Children.Add(r);

                TextBlock t = new TextBlock();
                t.Text = c.Name+" ("+c.ContainerType+")";
                t.FontSize = 10;
                Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 2);
                canvas.Children.Add(t);

                t = new TextBlock();
                t.Text = "Габ: " + c.Length + "x" + c.Width + "x" + c.Height + "мм.Вес:" + c.Mass;
                t.FontSize = 10;
                Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 15);
                canvas.Children.Add(t);

                t = new TextBlock();
                t.Text = "Цена: " + c.Price + "; Направление загрузки д: " + c.DirLength + "; ш: " + c.DirWidth + "; в: " + c.DirHeight + "; ";
                t.FontSize = 10;
                Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 28);
                canvas.Children.Add(t);

                t = new TextBlock();
                t.Text = "Порядок загрузки + " + c.Order + "; Допуст.давление: " + c.PressHeight + "; Хрупкость: " + c.FragilityHeight + "; ";
                t.FontSize = 10;
                Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 41);
                canvas.Children.Add(t);

                t = new TextBlock();
                t.Text = "Уровень: " + c.Level + "; Количество: " + c.Quantity + "; На пол: " + c.Only4Bottom + "; ";
                t.FontSize = 10;
                Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 54);
                canvas.Children.Add(t);
                
            b.Child = canvas;
            doc.Blocks.Add(b);
        }

        public void AddCanvas2(FlowDocument doc, RowBlock rowBlock, int cWidth, int cHeight)
        {
            //MessageBox.Show("Количество вертикальных блоков " + rowBlock.Blocks.Count.ToString());
            BlockUIContainer b = new BlockUIContainer();
            Canvas canvas = new Canvas();
            canvas.Width = cWidth / scale;
            canvas.Height = cHeight / scale + 20;

            //Рисуем рамку вокруг canvas
            Rectangle r = new Rectangle();
            r.Width = cWidth / scale;
            r.Height = cHeight / scale;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            r.Stroke = new SolidColorBrush(Colors.Red);
            //r.Fill = new SolidColorBrush(Colors.Black);
            r.Fill = brush;
            Canvas.SetLeft(r, 0);
            Canvas.SetTop(r, 20);
            canvas.Children.Add(r);

            //пишем заголовок
            TextBlock t = new TextBlock();
            t.Text = rowBlock.Name + "Количество контейнеров: " + rowBlock.Count + " общий вес: " + rowBlock.Mass + "; плотность загрузки - " + rowBlock.Fullness+"("+rowBlock.FullnessWidth+")";
            t.FontSize = 14;
            Canvas.SetLeft(t, 20);
            Canvas.SetTop(t, 2);
            canvas.Children.Add(t);

            //рисуем контейнеры 
            foreach (VerticalBlock v in rowBlock.Blocks)
            {
                //MessageBox.Show("Количество ящиков в вертикальном блоке " + v.Blocks.Count.ToString());             
                //foreach (Container c in v.Blocks)
                foreach (Object p in v.Blocks)
                {

                    Container c = (Container)p;

                        r = new Rectangle();
                        r.Width = c.Length / scale;
                        r.Height = c.Height / scale;
                        brush = new SolidColorBrush();
                        brush = Brushes.White;

                        r.Stroke = new SolidColorBrush(Colors.Black);
                        //r.Fill = new SolidColorBrush(Colors.Black);
                        r.Fill = brush;
                        Canvas.SetLeft(r, c.FirstPoint.Y / scale);
                        Canvas.SetTop(r, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale);
                        canvas.Children.Add(r);

                        t = new TextBlock();
                        t.Text = c.Name;
                        t.FontSize = 20;
                        Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                        int delta = 2;
                        Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 2);
                        canvas.Children.Add(t);

                       

                        t = new TextBlock();
                        t.Text = "Габ: " + c.Length + "x" + c.Width + "x" + c.Height;
                        t.FontSize = 18;
                        Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);

                        delta = delta + 22;
                        Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                        canvas.Children.Add(t);

                        t = new TextBlock();
                        t.Text = "Вес:" + c.Mass;
                        t.FontSize = 18;
                        Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                        delta = delta + 22;
                        Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                        canvas.Children.Add(t);
                        if (p is VerticalBlock )
                        { 
                            VerticalBlock vB=(VerticalBlock)p;
                            foreach (Container cont in vB.Blocks)
                            {
                                t = new TextBlock();
                                t.Text = cont.Name;
                                t.FontSize = 16;
                                Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                                 delta = delta+22;
                                 Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                                canvas.Children.Add(t);
                            }


                        }
                        else if (p is HorizontalBlock)
                        {
                            HorizontalBlock vB=(HorizontalBlock)p;
                            foreach (Container cont in vB.Blocks)
                            {
                                t = new TextBlock();
                                t.Text = cont.Name;
                                t.FontSize = 16;
                                Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                                delta = delta+ 22;
                                Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                                canvas.Children.Add(t);
                            }

                        }


                    /*if (c.Kind == "VerticalPallet")
                    {
                        foreach (Container d in c.Blocks)
                        {
                            t = new TextBlock();
                            t.Text = d.Name;
                            t.FontSize = 18;
                            Canvas.SetLeft(t, d.FirstPoint.Y / scale + 2);
                            delta = delta + 22;
                            Canvas.SetTop(t, canvas.Height - d.Height / scale - d.FirstPoint.Z / scale + delta);
                            canvas.Children.AddContainer(t);
                        }
                    } */

                    /*t = new TextBlock();
                    t.Text = "Цена: " + c.Price.ToString() + "; Направление загрузки д: " + c.DirLength + "; ш: " + c.DirWidth + "; в: " + c.DirHeight + "; ";
                    t.FontSize = 10;
                    Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                    delta = delta + 22;
                    Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                    canvas.Children.AddContainer(t);

                    t = new TextBlock();
                    t.Text = "Порядок загрузки + " + c.Order + "; Допуст.давление: " + c.PressHeight + "; Хрупкость: " + c.FragilityHeight + "; ";
                    t.FontSize = 10;
                    Canvas.SetLeft(t, c.FirstPoint.Y / scale+ 2);
                    delta = delta + 13;
                    Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                    canvas.Children.AddContainer(t);

                    t = new TextBlock();
                    t.Text = "Уровень: " + c.Level + "; Количество: " + c.Quantity + "; На пол: " + c.Only4bottom + "; ";
                    t.FontSize = 10;
                    Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                    delta = delta + 13;
                    Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                    canvas.Children.AddContainer(t); */
                    // break;
                        }
            } 
            //рассчитываем центр тяжести и рисуем оси центра тяжести
            /*  Point3D point = Calculations.CalculateMassCenterRow(rowBlock);
           //рисуем горизонтальную ось  
          Line myLine = new Line();
              myLine.Stroke = System.Windows.Media.Brushes.Green;

              myLine.X1 = 0;
              myLine.X2 = canvas.Width;
              myLine.Y1 = 0;
              myLine.Y2 = 0;
              myLine.HorizontalAlignment = HorizontalAlignment.Left;
              myLine.VerticalAlignment = VerticalAlignment.Center;
              myLine.StrokeThickness = 1;
              Canvas.SetLeft(myLine,0);
              Canvas.SetTop(myLine, canvas.Height-point.Z/scale);
              canvas.Children.AddContainer(myLine);

             //рисуем вертикальную ось
              myLine = new Line();
              myLine.Stroke = System.Windows.Media.Brushes.Green;
              myLine.X1 = 0;
              myLine.X2 = 0;
              myLine.Y1 = 0;
              myLine.Y2 = canvas.Height;
              myLine.HorizontalAlignment = HorizontalAlignment.Left;
              myLine.VerticalAlignment = VerticalAlignment.Center;
              myLine.StrokeThickness = 1;
              Canvas.SetLeft(myLine, point.Y/scale);
              Canvas.SetTop(myLine, 20);
              canvas.Children.AddContainer(myLine); */

            b.Child = canvas;
                doc.Blocks.Add(b);
        }

        public void AddRow(Table tab, RowBlock rowBlock, Vehicle currentVehicle,int i,int i2)
        {
            //MessageBox.Show("Количество вертикальных блоков " + rowBlock.Blocks.Count.ToString());
            BlockUIContainer b = new BlockUIContainer();
            Canvas canvas = new Canvas();
            canvas.Width = currentVehicle.Width / scale;
            canvas.Height = currentVehicle.Height / scale + 20;

            //Рисуем рамку вокруг canvas
            Rectangle r = new Rectangle();
            r.Width = currentVehicle.Width / scale;
            r.Height = currentVehicle.Height / scale;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            r.Stroke = new SolidColorBrush(Colors.Red);
            //r.Fill = new SolidColorBrush(Colors.Black);
            r.Fill = brush;
            Canvas.SetLeft(r, 0);
            Canvas.SetTop(r, 20);
            canvas.Children.Add(r);

            //пишем заголовок
            tab.RowGroups[0].Rows.Add(new TableRow());
            
            TableRow currentRow = tab.RowGroups[0].Rows[i2-1];
            currentRow.Background = Brushes.White;
            currentRow.FontSize = 18;
            currentRow.FontWeight = FontWeights.Normal;
            
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Шаг "+i+": "+  rowBlock.Name + "контейнеров: " + rowBlock.Count + ", вес: " + rowBlock.Mass + ", плотность: " + rowBlock.Fullness + "(" + rowBlock.FullnessWidth + ")"))));
            currentRow.Cells[0].ColumnSpan = 2;
            //пишем схему ряда
            TextBlock t2 = new TextBlock();
            t2.FontSize = 14;

            TextBlock t = new TextBlock();
            //рисуем контейнеры 
            foreach (VerticalBlock v in rowBlock.Blocks)
            {
                //MessageBox.Show("Количество ящиков в вертикальном блоке " + v.Blocks.Count.ToString());             
                //foreach (Container c in v.Blocks)
                foreach (Object p in v.Blocks)
                {
                    Container c = (Container)p;
                    r = new Rectangle();
                    r.Width = c.Length / scale;
                    r.Height = c.Height / scale;
                    brush = new SolidColorBrush();
                    brush = Brushes.White;
                    
                    r.Stroke = new SolidColorBrush(Colors.Black);
                    r.Fill = brush;
                    Canvas.SetLeft(r, (c.FirstPoint.Y-currentVehicle.FirstPoint.Y) / scale);
                    Canvas.SetTop(r, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale);
                    canvas.Children.Add(r);

                    t = new TextBlock();
                    t.Text = c.Name;
                    if (c.Kind == "VerticalPallet") { t.FontSize = 14; }
                    else {t.FontSize = 20; }

                    Canvas.SetLeft(t, (c.FirstPoint.Y - currentVehicle.FirstPoint.Y) / scale + 2);
                    int delta = 2;
                    Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 2);
                    canvas.Children.Add(t);

                    t = new TextBlock();
                    t.Text = "Габ: " + c.Vgh;
                    t.FontSize = 14;
                    Canvas.SetLeft(t, (c.FirstPoint.Y - currentVehicle.FirstPoint.Y) / scale + 2);

                    delta = delta + 22;
                    Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                    canvas.Children.Add(t);

                    t = new TextBlock();
                    t.Text = "Вес:" + c.Mass;
                    t.FontSize = 14;
                    Canvas.SetLeft(t, (c.FirstPoint.Y - currentVehicle.FirstPoint.Y) / scale + 2);
                    delta = delta + 22;
                    Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                    canvas.Children.Add(t);
                    if (p is VerticalBlock)
                    {
                        t2.Text = t2.Text + "\n" + c.Name + "\n";
                        t2.Text = t2.Text + "  Габариты:" + c.Vgh + "\n";
                        t2.Text = t2.Text + "  Вес:" + c.Mass + "\n"; 
                        VerticalBlock vB = (VerticalBlock)p;
                        foreach (Container cont in vB.Blocks)
                        {
                            t2.Text = t2.Text + "  * "+cont.Name + " ("+ cont.ContainerType+")"+" \n";
                           // t2.Text = t2.Text + "  Тип:" + cont.ContainerType + "\n";
                            //t2.Text = t2.Text + "  Габариты:" + cont.Vgh + "\n";
                            //t2.Text = t2.Text + "  Вес:"+ cont.Mass.ToString() + "\n"; 

                            t = new TextBlock();
                            t.Text = cont.Name;
                            t.FontSize = 16;
                            Canvas.SetLeft(t, (c.FirstPoint.Y - currentVehicle.FirstPoint.Y) / scale + 2);
                            delta = delta + 22;
                            Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                            canvas.Children.Add(t);
                        }


                    }
                    else if (p is HorizontalBlock)
                    {
                        t2.Text = t2.Text + "\n" + c.Name + " \n";
                        t2.Text = t2.Text + "  Габариты:" + c.Vgh + "\n";
                        t2.Text = t2.Text + "  Вес:" + c.Mass + "\n";
                        HorizontalBlock vB = (HorizontalBlock)p;
                        foreach (Container cont in vB.Blocks)
                        {
                            t2.Text = t2.Text + "  * " + cont.Name + " (" + cont.ContainerType + ")" + " \n";
                            //t2.Text = t2.Text + "  Тип:" + cont.ContainerType + "\n";
                            //t2.Text = t2.Text + "  Габариты:" + cont.Vgh + "\n";
                            //t2.Text = t2.Text + "  Вес:" + cont.Mass.ToString() + "\n";

                            t = new TextBlock();
                            t.Text = cont.Name;
                            t.FontSize = 16;
                            Canvas.SetLeft(t, (c.FirstPoint.Y - currentVehicle.FirstPoint.Y) / scale + 2);
                            delta = delta + 22;
                            Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                            canvas.Children.Add(t);
                        }

                    }
                    else
                    {
                        t2.Text = t2.Text + "\n" + c.Name + " (" + c.ContainerType + ")" + "\n";
                        //t2.Text = t2.Text + "  Тип:" + c.ContainerType + "\n";
                        t2.Text = t2.Text + "  Габариты:" + c.Vgh + "\n";
                        t2.Text = t2.Text + "  Вес:" + c.Mass + "\n";
                    }


                    /*if (c.Kind == "VerticalPallet")
                    {
                        foreach (Container d in c.Blocks)
                        {
                            t = new TextBlock();
                            t.Text = d.Name;
                            t.FontSize = 18;
                            Canvas.SetLeft(t, d.FirstPoint.Y / scale + 2);
                            delta = delta + 22;
                            Canvas.SetTop(t, canvas.Height - d.Height / scale - d.FirstPoint.Z / scale + delta);
                            canvas.Children.AddContainer(t);
                        }
                    } */

                    /*t = new TextBlock();
                    t.Text = "Цена: " + c.Price.ToString() + "; Направление загрузки д: " + c.DirLength + "; ш: " + c.DirWidth + "; в: " + c.DirHeight + "; ";
                    t.FontSize = 10;
                    Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                    delta = delta + 22;
                    Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                    canvas.Children.AddContainer(t);

                    t = new TextBlock();
                    t.Text = "Порядок загрузки + " + c.Order + "; Допуст.давление: " + c.PressHeight + "; Хрупкость: " + c.FragilityHeight + "; ";
                    t.FontSize = 10;
                    Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                    delta = delta + 13;
                    Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                    canvas.Children.AddContainer(t);

                    t = new TextBlock();
                    t.Text = "Уровень: " + c.Level + "; Количество: " + c.Quantity + "; На пол: " + c.Only4bottom + "; ";
                    t.FontSize = 10;
                    Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                    delta = delta + 13;
                    Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                    canvas.Children.AddContainer(t); */
                    // break;
                }
            }
           
            b.Child = canvas;

            tab.RowGroups[0].Rows.Add(new TableRow());
           
            currentRow = tab.RowGroups[0].Rows[i2];
            currentRow.Background = Brushes.White;
            currentRow.FontSize = 14;
            currentRow.FontWeight = FontWeights.Normal;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t2.Text))));
            currentRow.Cells[0].ColumnSpan=1;
              currentRow.Cells.Add(new TableCell(b));
              currentRow.Cells[1].ColumnSpan = 1;

              /*tab.RowGroups[1].Rows.AddContainer(new TableRow());
              currentRow = tab.RowGroups[1].Rows[i - 1];
              currentRow.Background = Brushes.White;
              currentRow.FontSize = 14;
              currentRow.FontWeight = System.Windows.FontWeights.Normal;
              currentRow.Cells.AddContainer(new TableCell(new Paragraph(new Run(t2.Name))));
              currentRow.Cells[0].ColumnSpan = 1;
              currentRow.Cells.AddContainer(new TableCell(b));
              currentRow.Cells[1].ColumnSpan = 1;*/
        }
           
        
       
        public void ShowVehicles()
        {
           FlowDocument doc = new FlowDocument();
           foreach (Vehicle v in vehicles)
           {
               AddMainHeader(doc, "Схема загрузки автомобиля " + v.Name + " (" + v.Count + " контейнеров; общий вес груза " + v.Mass + " кг.)");
               
               if (v.Blocks.Count == 0)
               {
                   AddHeader(doc, "Автомобиль загружать не нужно");
               }
               else
               {   int i = 0;
                   int i2 = 0;
                   Table table1 = new Table();
                   // ...and add it to the FlowDocument Blocks collection.
                   doc.Blocks.Add(table1);
                   // Set some global formatting properties for the table.
                   table1.CellSpacing = 10;
                   table1.Background = Brushes.White;

                   // Create 6 columns and add them to the table's Columns collection.
                   int numberOfColumns = 2;
                   for (int x = 0; x < numberOfColumns; x++)
                   {
                       table1.Columns.Add(new TableColumn());

                       // Set alternating background colors for the middle colums.
                       if (x % 2 == 0)
                           table1.Columns[x].Background = Brushes.Beige;
                       else
                           table1.Columns[x].Background = Brushes.LightSteelBlue;
                   }

                   table1.Columns[0].Width = new GridLength(300);



                   //Добавляем заголовок таблицы
                   table1.RowGroups.Add(new TableRowGroup());
                   table1.RowGroups.Add(new TableRowGroup());

                   // AddContainer the first (title) row.
                   table1.RowGroups[0].Rows.Add(new TableRow());
                   TableRow currentRow = table1.RowGroups[0].Rows[0];
                   currentRow.Background = Brushes.Silver;
                   currentRow.FontSize = 14;
                   currentRow.FontWeight = FontWeights.Bold;
                   currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Контейнеры"))));
                   currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Схема загрузки ряда"))));
                   //получаем список заказов
                   List<int> orderList = v.DistinctOrdersInRow(v.Blocks);
                   orderList.OrderBy(o => o);
                   foreach (int order in orderList)
                   {
                       foreach (RowBlock r in v.Blocks)
                       {

                           //AddHeader(table1, "Шаг " + i.ToString() + ":");

                           if (r.Order == order)
                           {
                               i++;
                               i2 = i2 + 2;
                               AddRow(table1, r, v, i, i2);
                           }
                       }
                       List<Container> tempList = v.SmallBlocks.Where(c => c.Order == order).ToList();
                       if (tempList.Count() > 0)
                       {
                           i++;
                           AddSmallContainers(table1, tempList, i, i2);
                           i2 = i2 + 1 + tempList.Count();
                       }
                   }
               }
           }
            flowDocViewer.Document = doc;
        }
        public void ShowRowBlocks()
        {
            FlowDocument doc = new FlowDocument();
            
                AddMainHeader(doc, "Список рядов " +  "(" + rBlocks.Count() + " рядов)");
                int i = 0;
                if (rBlocks.Count() == 0)
                {
                    AddHeader(doc, "Нет рядов для отображения");
                }
                foreach (RowBlock r in rBlocks)
                {
                    r.SetFirstPointForVerticalBlock(new Point3D(0, 0, 0));
                    i = i + 1;
                    AddHeader(doc, "Шаг " + i + ":");
                    AddCanvas2(doc, r, r.MaxLength, r.Height);
                    //break;
                }
            
            flowDocViewer.Document = doc;
        }
        public void ShowVerticalBlocks()
        {
            FlowDocument doc = new FlowDocument();
           
                AddMainHeader(doc, "Вертикальные блоки " + "(" + vBlocks.Count() + " блоков");
                int i = 0;
                if (vBlocks.Count() == 0)
                {
                    AddHeader(doc, "Нет вертикальныx блоков");
                }
                foreach (VerticalBlock v in vBlocks)
                {
                    i = i + 1;
                    AddHeader(doc, "Шаг " + i + ":");
                    AddCanvas(doc, v, v.Length, v.Height);
                    //break;
                }
            
            flowDocViewer.Document = doc;
        }
        public void ShowContainers()
        {
            FlowDocument doc = new FlowDocument();

            AddMainHeader(doc, "Контейнеры " + "(" + containers.Count() + "ед.");
            int i = 0;
            if (containers.Count() == 0)
            {
                AddHeader(doc, "Нет контейнеров для отображения");
            }
            foreach (Container c in containers)
            {
                i = i + 1;
                AddContainer(doc,c);
                //break;
            }

            flowDocViewer.Document = doc;
        }

       }
}
