using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using PdfSharp.Xps;
using VisualPacker.Models;
using VisualPacker.ViewModels;
using Container = VisualPacker.Models.Container;
using Path = System.IO.Path;

namespace VisualPacker.Views
{
    /// <summary>
    /// Interaction logic for view2d.xaml
    /// </summary>
    public partial class view2d : Window
    {
        public ObservableCollection<Vehicle> vehicles;
        public List<VerticalBlock> vBlocks;
        public List<RowBlock> rBlocks;
        public List<Container> containers;
        public int scale = 13;
        public enum Direction { Up, Front };

        public view2d(Object Data)
        {
            InitializeComponent();
            if (Data is ObservableCollection<Vehicle>)
            {
                vehicles = (ObservableCollection<Vehicle>)Data;
                ShowVehicles();
            }
            else if (Data is List<VerticalBlock>)
            {
                //vBlocks = (List<VerticalBlock>)Data;
                //ShowVerticalBlocks();
            }
            else if (Data is List<RowBlock>)
            {
                //rBlocks = (List<RowBlock>)Data;
                //ShowRowBlocks();
            }
            else if (Data is List<Container>)
            {
                //containers = (List<Container>)Data;
                //ShowContainers();
            }
            else { MessageBox.Show("В форму отчета передан неверный тип данных:" + Data.GetType()); }
        }
        public void DrawContainerUp(Canvas canvas, Container c, Vehicle currentVehicle)
        {
            int length = c.Width / scale;
            int height = c.Height / scale;
            double x = (c.FirstPoint.X - currentVehicle.FirstPoint.X) / scale;
            double z = c.FirstPoint.Z / scale;
            Rectangle r = new Rectangle();
            r.Width = length;
            r.Height = height;

            Brush brush = new SolidColorBrush();
            brush = Brushes.White;

            r.Stroke = new SolidColorBrush(Colors.Black);
            r.Fill = brush;
            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, canvas.Height - height - z);
            canvas.Children.Add(r);

            TextBlock t = new TextBlock();
            t.Text = Math.Round(c.Mass) + " кг";
            t.FontSize = 12;
            Canvas.SetLeft(t, x + 2);
            int delta = 2;
            Canvas.SetTop(t, canvas.Height - height - z + 2);
            canvas.Children.Add(t);

            t = new TextBlock();
            t.Text = c.ShortName;
            t.FontSize = 12;
            Canvas.SetLeft(t, x + 2);
            delta = delta + 15;
            Canvas.SetTop(t, canvas.Height - height - z + delta);
            canvas.Children.Add(t);
        }
        public void DrawContainerFront(Canvas canvas, Container container, Vehicle currentVehicle)
        {
            int length = container.Width / scale;
            int height = container.Length / scale;
            double x = container.FirstPoint.X / scale;
            double z = (container.FirstPoint.Y - currentVehicle.FirstPoint.Y) / scale;

            Rectangle r = new Rectangle();
            r.Width = length;
            r.Height = height;

            Brush brush = new SolidColorBrush();
            brush = Brushes.White;

            r.Stroke = new SolidColorBrush(Colors.Black);
            r.Fill = brush;
            Canvas.SetLeft(r, x);

            Canvas.SetTop(r, z);
            canvas.Children.Add(r);

            TextBlock t = new TextBlock();
            t.Text = Math.Round(container.Mass) + " кг";
            t.FontSize = 12;
            Canvas.SetLeft(t, x + 2);
            int delta = 2;
            Canvas.SetTop(t, z + 2);
            canvas.Children.Add(t);

            t = new TextBlock();
            t.Text = container.ShortName;
            t.FontSize = 12;
            Canvas.SetLeft(t, x + 2);
            delta = delta + 15;
            Canvas.SetTop(t, z + delta);
            canvas.Children.Add(t);
        }
        public void DrawBlock(Canvas canvas, Object Data, Direction direction, Vehicle currentVehicle)
        {
            if (Data is VerticalBlock)
            {
                VerticalBlock v = (VerticalBlock)Data;
                foreach (Container c in v.blocks) { DrawBlock(canvas, c, direction, currentVehicle); }
            }
            else if (Data is RowBlock)
            {
                RowBlock r = (RowBlock)Data;
                foreach (VerticalBlock v in r.Blocks) { DrawBlock(canvas, v, direction, currentVehicle); }
            }
            else if (Data is HorizontalBlock)
            {
                HorizontalBlock h = (HorizontalBlock)Data;
            }
            else if (Data is Container)
            {
                Container container = (Container)Data;
                if (direction == Direction.Up) { DrawContainerUp(canvas, container, currentVehicle); }
                else if (direction == Direction.Front) { DrawContainerFront(canvas, container, currentVehicle); }
                else { MessageBox.Show("Направление проекции не определено"); };
            }
            else
            {
                MessageBox.Show("В процедуру рисования DrawBlock передан неверный тип данных:" + Data.GetType());
            }
        }
        public void DrawCanvas(FlowDocument doc, Vehicle v, Direction direction)
        {

            double length = v.Length;
            double height = v.Height;
            if (direction == Direction.Up)
            {
                length = v.Length / scale;
                height = v.Height / scale;
            }
            else if (direction == Direction.Front)
            {
                length = v.Length / scale;
                height = v.Width / scale;
            }
            else { MessageBox.Show("не опознан вид проекции"); };

            //создаем объект для рисования
            BlockUIContainer b = new BlockUIContainer();
            Canvas canvas = new Canvas();
            canvas.Width = length;
            canvas.Height = height;

            //Рисуем рамку вокруг canvas
            Rectangle r = new Rectangle();
            r.Width = length;
            r.Height = height;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            r.Stroke = new SolidColorBrush(Colors.Black);
            r.StrokeThickness = 2;
            r.Fill = brush;
            Canvas.SetLeft(r, 0);
            Canvas.SetTop(r, 0);
            canvas.Children.Add(r);

            //рисуем контейнеры 
            foreach (RowBlock rowBlock in v.Blocks)
            {
                DrawBlock(canvas, rowBlock, direction, v);
            }

            DrawMassCenter(v, canvas, direction);
            b.Child = canvas;
            doc.Blocks.Add(b);
        }
        public void DrawCanvasVeels(FlowDocument doc, Vehicle v)
        {
            double length = v.Length / scale;
            double height = 0.3 * v.Height / scale;
            //создаем объект для рисования
            BlockUIContainer b = new BlockUIContainer();
            Canvas canvas = new Canvas();
            canvas.Width = length;
            canvas.Height = height;

            //Рисуем  переднее колесо
            DrawVeel(canvas, height, 50);
            //DrawAxelTonnage(canvas, v.Front_axle_current_tonnage, 50);

            //Рисуем  заднее колесо
            DrawVeel(canvas, height, length - height - 50);
            //DrawAxelTonnage(canvas, v.Back_axle_current_tonnage, length - height - 50);
            b.Child = canvas;
            doc.Blocks.Add(b);
        }
        public void DrawVeel(Canvas canvas, double diameter, double firstPointX)
        {
            Ellipse r = new Ellipse();
            r.Width = diameter;
            r.Height = diameter;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            r.Stroke = new SolidColorBrush(Colors.Black);
            r.StrokeThickness = 2;
            r.Fill = brush;
            Canvas.SetLeft(r, firstPointX);
            Canvas.SetTop(r, 0);
            canvas.Children.Add(r);
        }
        public void DrawMassCenter(Vehicle v, Canvas canvas, Direction dir)
        {
            Point3D p = v.GetMassCenter();
            int circleDiameter = 20;
            double height;
            if (dir == Direction.Up)
            {
                height = canvas.Height - p.Z / scale - circleDiameter / 2;
            }
            else
            {
                height = p.Y / scale - circleDiameter / 2;
            }

            Ellipse r = new Ellipse();
            r.Width = circleDiameter;
            r.Height = circleDiameter;
            Brush brush = new SolidColorBrush();
            brush = Brushes.Black;
            r.Stroke = new SolidColorBrush(Colors.Black);
            r.StrokeThickness = 2;
            r.Fill = brush;
            Canvas.SetLeft(r, p.X / scale - circleDiameter / 2);
            Canvas.SetTop(r, height);
            canvas.Children.Add(r);
        }
        public void DrawAxelTonnage(Canvas canvas, double tonnage, double firstPointX)
        {
            TextBlock t = new TextBlock();
            t.Text = Math.Round(tonnage / 1000, 1) + " тонн";
            t.FontSize = 12;
            Canvas.SetLeft(t, firstPointX + 2);
            Canvas.SetTop(t, 20);
            canvas.Children.Add(t);
        }

        public void DrawViewFront(FlowDocument doc, Vehicle v)
        {
            DrawCanvas(doc, v, Direction.Up);
            DrawCanvasVeels(doc, v);
        }
        public void DrawViewUpper(FlowDocument doc, Vehicle v)
        {
            DrawCanvas(doc, v, Direction.Front);
        }
        public void ShowVehicles()
        {
            FlowDocument doc = new FlowDocument();
            foreach (Vehicle v in vehicles)
            {
                AddMainHeader(doc, "Схема загрузки а/м " + v.Name + " (" + v.Length + "x" + v.Width + "x" + v.Height + ")");
                if (v.Blocks.Count() == 0) { AddHeader(doc, "Нет груза для отображения"); }
                else
                {
                    AddDescription(doc, v);
                    AddHeader(doc, "Вид сбоку");
                    DrawViewFront(doc, v);
                    AddHeader(doc, "Вид сверху");
                    DrawViewUpper(doc, v);
                }
            }
            flowDocViewer.Document = doc;
        }
        public void AddHeader(FlowDocument doc, String text)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.FontSize = 20;
            p.FontStyle = FontStyles.Italic;
            p.TextAlignment = TextAlignment.Left;
            doc.Blocks.Add(p);
        }
        public void AddDescription(FlowDocument doc, Vehicle v)
        {
            List<Container> tempList = v.VehicleToContainerList();
            tempList = Calculation.ListToContainerListIncludeVerticalPallet(tempList);
            //tempList.AddRange(v.smallBlocks);
            List<string> ShipmentList = DistinctShipmentID(tempList);
            foreach (string order in ShipmentList)
            {
                List<Container> tempList2 = tempList.Where(c => c.ShipmentId == order).ToList();
                AddRow(doc, "Грузоотправление: " + tempList2[0].ShipmentId + ".    Грузополучатель: " + tempList2[0].ShipToName + ".    Количество тар: " + tempList2.Count());
            }
            AddRow(doc, "Общий вес:" + v.Mass + " кг.");
        }
        public List<string> DistinctShipmentID(List<Container> containers)
        {
            List<string> ShipmentList = new List<string>();
            foreach (Container c in containers)
            {
                string Shipment = c.ShipmentId;
                ShipmentList.Contains(Shipment);
                if (ShipmentList.Contains(Shipment))
                {
                    //ничего не делаем
                }
                else
                {
                    ShipmentList.Add(Shipment);
                }
            }

            return ShipmentList;
        }
        public void AddRow(FlowDocument doc, String text)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.FontSize = 12;
            p.FontStyle = FontStyles.Normal;
            p.TextAlignment = TextAlignment.Left;
            doc.Blocks.Add(p);
        }
        public void AddMainHeader(FlowDocument doc, String text)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.FontSize = 16;
            p.FontStyle = FontStyles.Italic;
            p.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(p);
        }
        public void print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;
            printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = flowDocViewer.Document;
                doc.PagePadding = new Thickness(20, 40, 20, 40);
                doc.PageHeight = printDialog.PrintableAreaHeight;
                doc.PageWidth = printDialog.PrintableAreaWidth - 40;
                doc.ColumnWidth = printDialog.PrintableAreaWidth - 40;
                doc.ColumnGap = 0;
                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDocViewer.Document).DocumentPaginator , "Печать");
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveToFile();
        }
        public  void SaveToFile()
        {
            if (Directory.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData") == false)
            { Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData"); }

            string destination = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData\\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + " " +
                DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + " " + Environment.UserName + " 2dView";

            using (var stream = new FileStream(destination+".xps", FileMode.Create))
            {
                using (var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (var xpsDoc = new XpsDocument(package, CompressionOption.Maximum))
                    {
                        var rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
                        var paginator = ((IDocumentPaginatorSource)flowDocViewer.Document).DocumentPaginator;
                        rsm.SaveAsXaml(paginator);
                        rsm.Commit();
                    }
                }
                stream.Position = 0;

                var pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(stream);
                XpsConverter.Convert(pdfXpsDoc, destination + ".pdf", 0);
            }


        }
    }
}
