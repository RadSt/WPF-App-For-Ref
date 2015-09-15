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
    /// Interaction logic for View2D.xaml
    /// </summary>
    public partial class View2D
    {
        private ObservableCollection<Vehicle> Vehicles;
        public List<VerticalBlock> VBlocks;
        public List<RowBlock> RBlocks;
        public List<Container> Containers;
        private int Scale = 13; //масштаб
        private enum Direction { Up, Front };

        public View2D(Object data)
        {
            InitializeComponent();
            //if (data is ObservableCollection<Vehicle>)
            //{
                Vehicles = (ObservableCollection<Vehicle>)data;
                ShowVehicles();
            //}
            //else if (data is List<VerticalBlock>)
            //{
            //    //vBlocks = (List<VerticalBlock>)Data;
            //    //ShowVerticalBlocks();
            //}
            //else if (data is List<RowBlock>)
            //{
            //    //rBlocks = (List<RowBlock>)Data;
            //    //ShowRowBlocks();
            //}
            //else if (data is List<Container>)
            //{
            //    //containers = (List<Container>)Data;
            //    //ShowContainers();
            //}
            //else { MessageBox.Show("В форму отчета передан неверный тип данных:" + data.GetType()); }
        }
        public void ShowVehicles()
        {
            FlowDocument doc = new FlowDocument();
            foreach (Vehicle vehicle in Vehicles)
            {
                AddMainHeader(doc, "Схема загрузки а/м " + vehicle.Name + " (" + vehicle.Length + "x" + vehicle.Width + "x" + vehicle.Height + ")");
                if (!vehicle.Blocks.Any()) { AddHeader(doc, "Нет груза для отображения"); }
                else
                {
                    AddDescription(doc, vehicle);
                    AddHeader(doc, "Вид сбоку");
                    DrawViewFront(doc, vehicle);
                    AddHeader(doc, "Вид сверху");
                    DrawViewUpper(doc, vehicle);
                }
            }
            FlowDocViewer.Document = doc;
        }
        private void DrawContainerUp(Canvas canvas, Container container, Vehicle currentVehicle)
        {
            int length = container.Width / Scale;
            int height = container.Height / Scale;
            double x = (container.FirstPoint.X - currentVehicle.FirstPoint.X) / Scale;
            double z = container.FirstPoint.Z / Scale;
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
            t.Text = Math.Round(container.Mass) + " кг";
            t.FontSize = 12;
            Canvas.SetLeft(t, x + 2);
            int delta = 2;
            Canvas.SetTop(t, canvas.Height - height - z + 2);
            canvas.Children.Add(t);

            t = new TextBlock();
            t.Text = container.ShortName;
            t.FontSize = 12;
            Canvas.SetLeft(t, x + 2);
            delta = delta + 15;
            Canvas.SetTop(t, canvas.Height - height - z + delta);
            canvas.Children.Add(t);
        }
        private void DrawContainerFront(Canvas canvas, Container container, Vehicle currentVehicle)
        {
            int length = container.Width / Scale;
            int height = container.Length / Scale;
            double x = container.FirstPoint.X / Scale;
            double z = (container.FirstPoint.Y - currentVehicle.FirstPoint.Y) / Scale;

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
        private void DrawBlock(Canvas canvas, Object Data, Direction direction, Vehicle currentVehicle)
        {
            if (Data is VerticalBlock)
            {
                VerticalBlock v = (VerticalBlock)Data;
                foreach (Container c in v.Blocks) { DrawBlock(canvas, c, direction, currentVehicle); }
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
        private void DrawCanvas(FlowDocument doc, Vehicle vehicle, Direction direction)
        {

            double length = vehicle.Length;
            double height = vehicle.Height;
            if (direction == Direction.Up)
            {
                length = vehicle.Length / Scale;
                height = vehicle.Height / Scale;
            }
            else if (direction == Direction.Front)
            {
                length = vehicle.Length / Scale;
                height = vehicle.Width / Scale;
            }
            else { MessageBox.Show("не опознан вид проекции"); };

            //создаем объект для рисования
            BlockUIContainer blockUiContainer = new BlockUIContainer();
            Canvas canvas = new Canvas();
            canvas.Width = length;
            canvas.Height = height;

            //Рисуем рамку вокруг canvas
            Rectangle rectangle = new Rectangle();
            rectangle.Width = length;
            rectangle.Height = height;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            rectangle.Stroke = new SolidColorBrush(Colors.Black);
            rectangle.StrokeThickness = 2;
            rectangle.Fill = brush;
            Canvas.SetLeft(rectangle, 0);
            Canvas.SetTop(rectangle, 0);
            canvas.Children.Add(rectangle);

            //рисуем контейнеры 
            foreach (RowBlock rowBlock in vehicle.Blocks)
            {
                DrawBlock(canvas, rowBlock, direction, vehicle);
            }

            DrawMassCenter(vehicle, canvas, direction);
            blockUiContainer.Child = canvas;
            doc.Blocks.Add(blockUiContainer);
        }
        private void DrawCanvasVeels(FlowDocument doc, Vehicle vehicle)
        {
            double length = vehicle.Length / Scale;
            double height = 0.3 * vehicle.Height / Scale;
            //создаем объект для рисования
            BlockUIContainer blockUiContainer = new BlockUIContainer();
            Canvas canvas = new Canvas();
            canvas.Width = length;
            canvas.Height = height;

            //Рисуем  переднее колесо
            DrawWheel(canvas, height, 50);
            //DrawAxelTonnage(canvas, v.Front_axle_current_tonnage, 50);

            //Рисуем  заднее колесо
            DrawWheel(canvas, height, length - height - 50);
            //DrawAxelTonnage(canvas, v.Back_axle_current_tonnage, length - height - 50);
            blockUiContainer.Child = canvas;
            doc.Blocks.Add(blockUiContainer);
        }
        private void DrawWheel(Canvas canvas, double diameter, double firstPointX)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = diameter;
            ellipse.Height = diameter;
            Brush brush = Brushes.White;
            ellipse.Stroke = new SolidColorBrush(Colors.Black);
            ellipse.StrokeThickness = 2;
            ellipse.Fill = brush;
            Canvas.SetLeft(ellipse, firstPointX);
            Canvas.SetTop(ellipse, 0);
            canvas.Children.Add(ellipse);
        }
        private void DrawMassCenter(Vehicle vehicle, Canvas canvas, Direction dir)
        {
            Point3D point3D = vehicle.GetMassCenter();
            int circleDiameter = 20;
            double height;
            if (dir == Direction.Up)
            {
                height = canvas.Height - point3D.Z / Scale - circleDiameter / 2;
            }
            else
            {
                height = point3D.Y / Scale - circleDiameter / 2;
            }

            Ellipse ellipse = new Ellipse();
            ellipse.Width = circleDiameter;
            ellipse.Height = circleDiameter;
            Brush brush = new SolidColorBrush();
            brush = Brushes.Black;
            ellipse.Stroke = new SolidColorBrush(Colors.Black);
            ellipse.StrokeThickness = 2;
            ellipse.Fill = brush;
            Canvas.SetLeft(ellipse, point3D.X / Scale - circleDiameter / 2);
            Canvas.SetTop(ellipse, height);
            canvas.Children.Add(ellipse);
        }

        private void DrawViewFront(FlowDocument doc, Vehicle vehicle)
        {
            DrawCanvas(doc, vehicle, Direction.Up);
            DrawCanvasVeels(doc, vehicle);
        }
        private void DrawViewUpper(FlowDocument doc, Vehicle vehicle)
        {
            DrawCanvas(doc, vehicle, Direction.Front);
        }
        private void AddHeader(FlowDocument doc, String text)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.FontSize = 20;
            p.FontStyle = FontStyles.Italic;
            p.TextAlignment = TextAlignment.Left;
            doc.Blocks.Add(p);
        }
        private void AddDescription(FlowDocument doc, Vehicle vehicle)
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
            List<Container> tempList = vehicle.VehicleToContainerList();
            tempList = fromTempListToContList.ToContainerList(tempList);
            //tempList.AddRange(v.smallBlocks);
            List<string> shipmentList = DistinctShipmentID(tempList);
            foreach (string order in shipmentList)
            {
                List<Container> tempList2 = tempList.Where(c => c.ShipmentId == order).ToList();
                AddRow(doc, "Грузоотправление: " + tempList2[0].ShipmentId + ".    Грузополучатель: " + tempList2[0].ShipToName + ".    Количество тар: " + tempList2.Count());
            }
            AddRow(doc, "Общий вес:" + vehicle.Mass + " кг.");
        }
        private List<string> DistinctShipmentID(List<Container> containers)
        {
            List<string> shipmentList = new List<string>();
            foreach (Container c in containers)
            {
                string Shipment = c.ShipmentId;
                shipmentList.Contains(Shipment);
                if (!shipmentList.Contains(Shipment))
                {
                    shipmentList.Add(Shipment);
                }
            }

            return shipmentList;
        }
        private void AddRow(FlowDocument doc, String text)
        {
            Paragraph p = new Paragraph(new Run(text));
            p.FontSize = 12;
            p.FontStyle = FontStyles.Normal;
            p.TextAlignment = TextAlignment.Left;
            doc.Blocks.Add(p);
        }
        private void AddMainHeader(FlowDocument doc, String text)
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
                FlowDocument doc = FlowDocViewer.Document;
                doc.PagePadding = new Thickness(20, 40, 20, 40);
                doc.PageHeight = printDialog.PrintableAreaHeight;
                doc.PageWidth = printDialog.PrintableAreaWidth - 40;
                doc.ColumnWidth = printDialog.PrintableAreaWidth - 40;
                doc.ColumnGap = 0;
                printDialog.PrintDocument(((IDocumentPaginatorSource)FlowDocViewer.Document).DocumentPaginator , "Печать");
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
                        var paginator = ((IDocumentPaginatorSource)FlowDocViewer.Document).DocumentPaginator;
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
