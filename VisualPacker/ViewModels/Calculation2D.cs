using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using VisualPacker.Models;

namespace VisualPacker.ViewModels
{
    public class Calculation2D
    {
        private const int Scale = 13; //масштаб
        private ObservableCollection<Vehicle> Vehicles;
        public FlowDocument ShowVehicles(Object data)
        {
            Vehicles = (ObservableCollection<Vehicle>)data;
            var doc = new FlowDocument();
            foreach (var vehicle in Vehicles)
            {
                AddMainHeader(doc,
                    "Схема загрузки а/м " + vehicle.Name + " (" + vehicle.Length + "x" + vehicle.Width + "x" +
                    vehicle.Height + ")");
                if (!vehicle.Blocks.Any())
                {
                    AddHeader(doc, "Нет груза для отображения");
                }
                else
                {
                    AddDescription(doc, vehicle);
                    AddHeader(doc, "Вид сбоку");
                    DrawViewFront(doc, vehicle);
                    AddHeader(doc, "Вид сверху");
                    DrawViewUpper(doc, vehicle);
                }
            }
            return doc;
        }

        private void DrawContainerUp(Canvas canvas, Container container, Vehicle currentVehicle)
        {
            var length = container.Width/Scale;
            var height = container.Height/Scale;
            var x = (container.FirstPoint.X - currentVehicle.FirstPoint.X)/Scale;
            var z = container.FirstPoint.Z/Scale;
            var rectangle = new Rectangle();
            rectangle.Width = length;
            rectangle.Height = height;

            Brush brush = new SolidColorBrush();
            brush = Brushes.White;

            rectangle.Stroke = new SolidColorBrush(Colors.Black);
            rectangle.Fill = brush;
            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, canvas.Height - height - z);
            canvas.Children.Add(rectangle);

            var textBlock = new TextBlock {Text = Math.Round(container.Mass) + " кг", FontSize = 12};
            Canvas.SetLeft(textBlock, x + 2);
            var delta = 2;
            Canvas.SetTop(textBlock, canvas.Height - height - z + 2);
            canvas.Children.Add(textBlock);

            textBlock = new TextBlock {Text = container.ShortName, FontSize = 12};
            Canvas.SetLeft(textBlock, x + 2);
            delta = delta + 15;
            Canvas.SetTop(textBlock, canvas.Height - height - z + delta);
            canvas.Children.Add(textBlock);
        }

        private void DrawContainerFront(Canvas canvas, Container container, Vehicle currentVehicle)
        {
            var length = container.Width/Scale;
            var height = container.Length/Scale;
            var x = container.FirstPoint.X/Scale;
            var z = (container.FirstPoint.Y - currentVehicle.FirstPoint.Y)/Scale;

            var r = new Rectangle();
            r.Width = length;
            r.Height = height;

            Brush brush = new SolidColorBrush();
            brush = Brushes.White;

            r.Stroke = new SolidColorBrush(Colors.Black);
            r.Fill = brush;
            Canvas.SetLeft(r, x);

            Canvas.SetTop(r, z);
            canvas.Children.Add(r);

            var t = new TextBlock();
            t.Text = Math.Round(container.Mass) + " кг";
            t.FontSize = 12;
            Canvas.SetLeft(t, x + 2);
            var delta = 2;
            Canvas.SetTop(t, z + 2);
            canvas.Children.Add(t);

            t = new TextBlock {Text = container.ShortName, FontSize = 12};
            Canvas.SetLeft(t, x + 2);
            delta = delta + 15;
            Canvas.SetTop(t, z + delta);
            canvas.Children.Add(t);
        }

        private void DrawBlock(Canvas canvas, Object Data, Direction direction, Vehicle currentVehicle)
        {
            if (Data is VerticalBlock)
            {
                var v = (VerticalBlock) Data;
                foreach (var c in v.Blocks)
                {
                    DrawBlock(canvas, c, direction, currentVehicle);
                }
            }
            else if (Data is RowBlock)
            {
                var r = (RowBlock) Data;
                foreach (var v in r.Blocks)
                {
                    DrawBlock(canvas, v, direction, currentVehicle);
                }
            }
            else if (Data is HorizontalBlock)
            {
                var h = (HorizontalBlock) Data;
            }
            else if (Data is Container)
            {
                var container = (Container) Data;
                if (direction == Direction.Up)
                {
                    DrawContainerUp(canvas, container, currentVehicle);
                }
                else if (direction == Direction.Front)
                {
                    DrawContainerFront(canvas, container, currentVehicle);
                }
                else
                {
                    MessageBox.Show("Направление проекции не определено");
                }
                ;
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
                length = vehicle.Length/Scale;
                height = vehicle.Height/Scale;
            }
            else if (direction == Direction.Front)
            {
                length = vehicle.Length/Scale;
                height = vehicle.Width/Scale;
            }
            else
            {
                MessageBox.Show("не опознан вид проекции");
            }
            ;

            //создаем объект для рисования
            var blockUiContainer = new BlockUIContainer();
            var canvas = new Canvas();
            canvas.Width = length;
            canvas.Height = height;

            //Рисуем рамку вокруг canvas
            var rectangle = new Rectangle();
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
            foreach (var rowBlock in vehicle.Blocks)
            {
                DrawBlock(canvas, rowBlock, direction, vehicle);
            }

            DrawMassCenter(vehicle, canvas, direction);
            blockUiContainer.Child = canvas;
            doc.Blocks.Add(blockUiContainer);
        }

        private void DrawCanvasVeels(FlowDocument doc, Vehicle vehicle)
        {
            double length = vehicle.Length/Scale;
            var height = 0.3*vehicle.Height/Scale;
            //создаем объект для рисования
            var blockUiContainer = new BlockUIContainer();
            var canvas = new Canvas();
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
            var ellipse = new Ellipse();
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
            var point3D = vehicle.GetMassCenter();
            var circleDiameter = 20;
            double height;
            if (dir == Direction.Up)
            {
                height = canvas.Height - point3D.Z/Scale - circleDiameter/2;
            }
            else
            {
                height = point3D.Y/Scale - circleDiameter/2;
            }

            var ellipse = new Ellipse();
            ellipse.Width = circleDiameter;
            ellipse.Height = circleDiameter;
            Brush brush = new SolidColorBrush();
            brush = Brushes.Black;
            ellipse.Stroke = new SolidColorBrush(Colors.Black);
            ellipse.StrokeThickness = 2;
            ellipse.Fill = brush;
            Canvas.SetLeft(ellipse, point3D.X/Scale - circleDiameter/2);
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
            var p = new Paragraph(new Run(text));
            p.FontSize = 20;
            p.FontStyle = FontStyles.Italic;
            p.TextAlignment = TextAlignment.Left;
            doc.Blocks.Add(p);
        }

        private void AddDescription(FlowDocument doc, Vehicle vehicle)
        {
            var fromTempListToContList = new FromTempListToContList();
            var tempList = vehicle.VehicleToContainerList();
            tempList = fromTempListToContList.ToContainerList(tempList);
            //tempList.AddRange(v.smallBlocks);
            var shipmentList = DistinctShipmentID(tempList);
            foreach (var order in shipmentList)
            {
                var tempList2 = tempList.Where(c => c.ShipmentId == order).ToList();
                AddRow(doc,
                    "Грузоотправление: " + tempList2[0].ShipmentId + ".    Грузополучатель: " + tempList2[0].ShipToName +
                    ".    Количество тар: " + tempList2.Count());
            }
            AddRow(doc, "Общий вес:" + vehicle.Mass + " кг.");

            VehicleAxisMass vehicleAxisMass = new VehicleAxisMass(vehicle, vehicle.Mass);
            var axisMassList=vehicleAxisMass.AxisMassCalculate();
            for (int i = 0; i < axisMassList.Count; i++)
            {
                AddRow(doc, String.Format("Нагрузка на ось{0} - {1:0.000} \n", (i + 1), axisMassList[i]));
            }
        }

        private List<string> DistinctShipmentID(List<Container> containers)
        {
            var shipmentList = new List<string>();
            foreach (var c in containers)
            {
                var Shipment = c.ShipmentId;
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
            var p = new Paragraph(new Run(text));
            p.FontSize = 12;
            p.FontStyle = FontStyles.Normal;
            p.TextAlignment = TextAlignment.Left;
            doc.Blocks.Add(p);
        }

        private void AddMainHeader(FlowDocument doc, String text)
        {
            var p = new Paragraph(new Run(text));
            p.FontSize = 16;
            p.FontStyle = FontStyles.Italic;
            p.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(p);
        }

        private enum Direction
        {
            Up,
            Front
        };
    }
}