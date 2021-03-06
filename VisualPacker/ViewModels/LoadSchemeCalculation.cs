﻿using System;
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

namespace VisualPacker.ViewModels
{

    public class LoadSchemeCalculation
    {
        private List<Container> containers;
        private List<RowBlock> rBlocks;
        private int scale = 6;
        private List<VerticalBlock> vBlocks;
        private ObservableCollection<Vehicle> vehicles;

        public FlowDocument RotateContainers(Object data)
        {
            var doc = new FlowDocument();
            if (data is ObservableCollection<Vehicle>)
            {
                vehicles = (ObservableCollection<Vehicle>)data;
                doc = ShowVehicles();
            }
            else if (data is List<VerticalBlock>)
            {
                vBlocks = (List<VerticalBlock>)data;
                doc = ShowVerticalBlocks();
            }
            else if (data is List<RowBlock>)
            {
                rBlocks = (List<RowBlock>)data;
                doc = ShowRowBlocks();
            }
            else if (data is List<Container>)
            {
                containers = (List<Container>)data;
                doc = ShowContainers();
            }
            else
            {
                MessageBox.Show("В форму отчета передан неверный тип данных:" + data.GetType());
                return null;
            }
            return doc;
        }
        private void AddMainHeader(FlowDocument doc, String text)
        {
            var p = new Paragraph(new Run(text));
            p.FontSize = 16;
            p.FontStyle = FontStyles.Italic;
            p.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(p);
        }
        private void AddHeader(FlowDocument doc, String text)
        {
            var p = new Paragraph(new Run(text));
            p.FontSize = 20;
            p.FontStyle = FontStyles.Italic;
            p.TextAlignment = TextAlignment.Left;
            doc.Blocks.Add(p);
        }
        private void AddSmallContainers(Table tab, List<Container> tempList, int i, int i2)
        {
            //печатаем заголовок
            var i3 = 1;
            tab.RowGroups[0].Rows.Add(new TableRow());
            var currentRow = tab.RowGroups[0].Rows[i2 + i3];
            currentRow.Background = Brushes.White;
            currentRow.FontSize = 18;
            currentRow.FontWeight = FontWeights.Normal;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Шаг " + i + ": Загрузите следующие контейнеры:"))));
            currentRow.Cells[0].ColumnSpan = 2;
            foreach (var c in tempList)
            {
                i3++;
                tab.RowGroups[0].Rows.Add(new TableRow());
                currentRow = tab.RowGroups[0].Rows[i2 + i3];
                currentRow.Background = Brushes.White;
                currentRow.FontSize = 14;
                currentRow.FontWeight = FontWeights.Normal;
                currentRow.Cells.Add(
                new TableCell(new Paragraph(new Run(c.Name + ": " + c.Vgh + "; " + c.Mass + " кг."))));
                currentRow.Cells[0].ColumnSpan = 2;
            }
        }
        private void AddCanvas(FlowDocument doc, VerticalBlock vBlock, int cWidth, int cHeight)
        {
            //MessageBox.Show("Количество вертикальных блоков " + rowBlock.Blocks.Count.ToString());
            var b = new BlockUIContainer();
            var canvas = new Canvas();
            canvas.Width = cWidth / scale;
            canvas.Height = cHeight / scale + 20;

            //Рисуем рамку вокруг canvas
            var r = new Rectangle();
            r.Width = cWidth / scale;
            r.Height = cHeight / scale;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            r.Stroke = new SolidColorBrush(Colors.Red);
            r.Fill = brush;
            Canvas.SetLeft(r, 0);
            Canvas.SetTop(r, 20);
            canvas.Children.Add(r);

            //пишем заголовок
            var t = new TextBlock();
            t.Text = "Количество контейнеров: " + vBlock.Count + " общий вес: " + vBlock.Mass;
            t.FontSize = 14;
            Canvas.SetLeft(t, 20);
            Canvas.SetTop(t, 2);
            canvas.Children.Add(t);

            vBlock.SetFirstPointVerticalBlock(new Point3D(0, 0, 0));
            //рисуем контейнеры
            foreach (var c in vBlock.Blocks)
            {
                r = new Rectangle();
                r.Width = c.Length / scale;
                r.Height = c.Height / scale;
                brush = new SolidColorBrush();
                brush = Brushes.White;
                r.Stroke = new SolidColorBrush(Colors.Black);
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
                t.Text = "Габ: " + c.Length + "x" + c.Width + "x" + c.Height + "мм.Вес:" + c.Mass;
                t.FontSize = 10;
                Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 15);
                canvas.Children.Add(t);

                t = new TextBlock();
                t.Text = "Цена: " + c.Price + "; Направление загрузки д: " + c.DirLength + "; ш: " + c.DirWidth +
                         "; в: " + c.DirHeight + "; ";
                t.FontSize = 10;
                Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 28);
                canvas.Children.Add(t);

                t = new TextBlock();
                t.Text = "Порядок загрузки + " + c.Order + "; Допуст.давление: " + c.PressHeight + "; Хрупкость: " +
                         c.FragilityHeight + "; ";
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
            }
            b.Child = canvas;
            doc.Blocks.Add(b);
        }
        private void AddContainer(FlowDocument doc, Container c)
        {
            var b = new BlockUIContainer();
            var canvas = new Canvas();
            canvas.Width = c.Width / scale;
            canvas.Height = c.Height / scale;


            var r = new Rectangle();
            r.Width = c.Length / scale;
            r.Height = c.Height / scale;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            r.Stroke = new SolidColorBrush(Colors.Black);
            r.Fill = brush;
            Canvas.SetLeft(r, c.FirstPoint.Y / scale);
            Canvas.SetTop(r, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale);
            canvas.Children.Add(r);

            var t = new TextBlock();
            t.Text = c.Name + " (" + c.ContainerType + ")";
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
            t.Text = "Цена: " + c.Price + "; Направление загрузки д: " + c.DirLength + "; ш: " + c.DirWidth + "; в: " +
                     c.DirHeight + "; ";
            t.FontSize = 10;
            Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
            Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + 28);
            canvas.Children.Add(t);

            t = new TextBlock();
            t.Text = "Порядок загрузки + " + c.Order + "; Допуст.давление: " + c.PressHeight + "; Хрупкость: " +
                     c.FragilityHeight + "; ";
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
        private void AddCanvas2(FlowDocument doc, RowBlock rowBlock, int cWidth, int cHeight)
        {
            var b = new BlockUIContainer();
            var canvas = new Canvas();
            canvas.Width = cWidth / scale;
            canvas.Height = cHeight / scale + 20;

            //Рисуем рамку вокруг canvas
            var r = new Rectangle();
            r.Width = cWidth / scale;
            r.Height = cHeight / scale;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            r.Stroke = new SolidColorBrush(Colors.Red);
            r.Fill = brush;
            Canvas.SetLeft(r, 0);
            Canvas.SetTop(r, 20);
            canvas.Children.Add(r);

            //пишем заголовок
            var t = new TextBlock();
            t.Text = rowBlock.Name + "Количество контейнеров: " + rowBlock.Count + " общий вес: " + rowBlock.Mass +
                     "; плотность загрузки - " + rowBlock.Fullness + "(" + rowBlock.FullnessWidth + ")";
            t.FontSize = 14;
            Canvas.SetLeft(t, 20);
            Canvas.SetTop(t, 2);
            canvas.Children.Add(t);

            //рисуем контейнеры 
            foreach (var v in rowBlock.Blocks)
            {
                foreach (Object p in v.Blocks)
                {
                    var c = (Container)p;

                    r = new Rectangle();
                    r.Width = c.Length / scale;
                    r.Height = c.Height / scale;
                    brush = new SolidColorBrush();
                    brush = Brushes.White;

                    r.Stroke = new SolidColorBrush(Colors.Black);
                    r.Fill = brush;
                    Canvas.SetLeft(r, c.FirstPoint.Y / scale);
                    Canvas.SetTop(r, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale);
                    canvas.Children.Add(r);

                    t = new TextBlock();
                    t.Text = c.Name;
                    t.FontSize = 20;
                    Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                    var delta = 2;
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
                    if (p is VerticalBlock)
                    {
                        var vB = (VerticalBlock)p;
                        foreach (var cont in vB.Blocks)
                        {
                            t = new TextBlock();
                            t.Text = cont.Name;
                            t.FontSize = 16;
                            Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                            delta = delta + 22;
                            Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                            canvas.Children.Add(t);
                        }
                    }
                    else if (p is HorizontalBlock)
                    {
                        var vB = (HorizontalBlock)p;
                        foreach (var cont in vB.Blocks)
                        {
                            t = new TextBlock();
                            t.Text = cont.Name;
                            t.FontSize = 16;
                            Canvas.SetLeft(t, c.FirstPoint.Y / scale + 2);
                            delta = delta + 22;
                            Canvas.SetTop(t, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale + delta);
                            canvas.Children.Add(t);
                        }
                    }
                }
            }
            b.Child = canvas;
            doc.Blocks.Add(b);
        }
        private void AddRow(Table tab, RowBlock rowBlock, Vehicle currentVehicle, int i, int i2)
        {
            //MessageBox.Show("Количество вертикальных блоков " + rowBlock.Blocks.Count.ToString());
            var b = new BlockUIContainer();
            var canvas = new Canvas();
            canvas.Width = currentVehicle.Width / scale;
            canvas.Height = currentVehicle.Height / scale + 20;

            //Рисуем рамку вокруг canvas
            var r = new Rectangle();
            r.Width = currentVehicle.Width / scale;
            r.Height = currentVehicle.Height / scale;
            Brush brush = new SolidColorBrush();
            brush = Brushes.White;
            r.Stroke = new SolidColorBrush(Colors.Red);
            r.Fill = brush;
            Canvas.SetLeft(r, 0);
            Canvas.SetTop(r, 20);
            canvas.Children.Add(r);

            //пишем заголовок
            tab.RowGroups[0].Rows.Add(new TableRow());

            var currentRow = tab.RowGroups[0].Rows[i2 - 1];
            currentRow.Background = Brushes.White;
            currentRow.FontSize = 18;
            currentRow.FontWeight = FontWeights.Normal;

            currentRow.Cells.Add(
                new TableCell(
                    new Paragraph(
                        new Run("Шаг " + i + ": " + rowBlock.Name + "контейнеров: " + rowBlock.Count + ", вес: " +
                                rowBlock.Mass + ", плотность: " + rowBlock.Fullness + "(" + rowBlock.FullnessWidth + ")"))));
            currentRow.Cells[0].ColumnSpan = 2;
            //пишем схему ряда
            var t2 = new TextBlock();
            t2.FontSize = 14;

            var t = new TextBlock();
            //рисуем контейнеры 
            foreach (var v in rowBlock.Blocks)
            {
                //MessageBox.Show("Количество ящиков в вертикальном блоке " + v.Blocks.Count.ToString());             
                foreach (Object p in v.Blocks)
                {
                    var c = (Container)p;
                    r = new Rectangle();
                    r.Width = c.Length / scale;
                    r.Height = c.Height / scale;
                    brush = new SolidColorBrush();
                    brush = Brushes.White;

                    r.Stroke = new SolidColorBrush(Colors.Black);
                    r.Fill = brush;
                    Canvas.SetLeft(r, (c.FirstPoint.Y - currentVehicle.FirstPoint.Y) / scale);
                    Canvas.SetTop(r, canvas.Height - c.Height / scale - c.FirstPoint.Z / scale);
                    canvas.Children.Add(r);

                    t = new TextBlock();
                    t.Text = c.Name;
                    if (c.Kind == "VerticalPallet")
                    {
                        t.FontSize = 14;
                    }
                    else
                    {
                        t.FontSize = 20;
                    }

                    Canvas.SetLeft(t, (c.FirstPoint.Y - currentVehicle.FirstPoint.Y) / scale + 2);
                    var delta = 2;
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
                        var vB = (VerticalBlock)p;
                        foreach (var cont in vB.Blocks)
                        {
                            t2.Text = t2.Text + "  * " + cont.Name + " (" + cont.ContainerType + ")" + " \n";
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
                        var vB = (HorizontalBlock)p;
                        foreach (var cont in vB.Blocks)
                        {
                            t2.Text = t2.Text + "  * " + cont.Name + " (" + cont.ContainerType + ")" + " \n";
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
                        t2.Text = t2.Text + "  Габариты:" + c.Vgh + "\n";
                        t2.Text = t2.Text + "  Вес:" + c.Mass + "\n";
                    }
                }
            }

            b.Child = canvas;

            tab.RowGroups[0].Rows.Add(new TableRow());

            currentRow = tab.RowGroups[0].Rows[i2];
            currentRow.Background = Brushes.White;
            currentRow.FontSize = 14;
            currentRow.FontWeight = FontWeights.Normal;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(t2.Text))));
            currentRow.Cells[0].ColumnSpan = 1;
            currentRow.Cells.Add(new TableCell(b));
            currentRow.Cells[1].ColumnSpan = 1;
        }
        private FlowDocument ShowVehicles()
        {
            var doc = new FlowDocument();
            foreach (var v in vehicles)
            {
                AddMainHeader(doc,
                    "Схема загрузки автомобиля " + v.Name + " (" + v.Count + " контейнеров; общий вес груза " + v.Mass +
                    " кг.)");

                if (v.Blocks.Count == 0)
                {
                    AddHeader(doc, "Автомобиль загружать не нужно");
                }
                else
                {
                    var i = 0;
                    var i2 = 0;
                    var table1 = new Table();
                    // ...and add it to the FlowDocument Blocks collection.
                    doc.Blocks.Add(table1);
                    // Set some global formatting properties for the table.
                    table1.CellSpacing = 10;
                    table1.Background = Brushes.White;

                    // Create 6 columns and add them to the table's Columns collection.
                    const int numberOfColumns = 2;
                    for (var x = 0; x < numberOfColumns; x++)
                    {
                        table1.Columns.Add(new TableColumn());

                        // Set alternating background colors for the middle colums.
                        table1.Columns[x].Background = x % 2 == 0 ? Brushes.Beige : Brushes.LightSteelBlue;
                    }

                    table1.Columns[0].Width = new GridLength(300);


                    //Добавляем заголовок таблицы
                    table1.RowGroups.Add(new TableRowGroup());
                    table1.RowGroups.Add(new TableRowGroup());

                    // AddContainer the first (title) row.
                    table1.RowGroups[0].Rows.Add(new TableRow());
                    var currentRow = table1.RowGroups[0].Rows[0];
                    currentRow.Background = Brushes.Silver;
                    currentRow.FontSize = 14;
                    currentRow.FontWeight = FontWeights.Bold;
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Контейнеры"))));
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Схема загрузки ряда"))));
                    //получаем список заказов
                    var orderList = DistinctOrdersInRow(v.Blocks);
                    orderList.OrderBy(o => o);
                    foreach (var order in orderList)
                    {
                        foreach (var r in v.Blocks)
                        {
                            if (r.Order == order)
                            {
                                i++;
                                i2 = i2 + 2;
                                AddRow(table1, r, v, i, i2);
                            }
                        }
                        var tempList = v.SmallBlocks.Where(c => c.Order == order).ToList();
                        if (tempList.Any())
                        {
                            i++;
                            AddSmallContainers(table1, tempList, i, i2);
                            i2 = i2 + 1 + tempList.Count();
                        }
                    }
                }
            }
            return doc;
        }
        private FlowDocument ShowRowBlocks()
        {
            var doc = new FlowDocument();

            AddMainHeader(doc, "Список рядов " + "(" + rBlocks.Count() + " рядов)");
            var i = 0;
            if (!rBlocks.Any())
            {
                AddHeader(doc, "Нет рядов для отображения");
            }
            foreach (var r in rBlocks)
            {
                r.SetFirstPointForVerticalBlock(new Point3D(0, 0, 0));
                i = i + 1;
                AddHeader(doc, "Шаг " + i + ":");
                AddCanvas2(doc, r, r.MaxLength, r.Height);
            }

            return doc;
        }
        private FlowDocument ShowVerticalBlocks()
        {
            var doc = new FlowDocument();

            AddMainHeader(doc, "Вертикальные блоки " + "(" + vBlocks.Count() + " блоков");
            var i = 0;
            if (!vBlocks.Any())
            {
                AddHeader(doc, "Нет вертикальныx блоков");
            }
            foreach (var v in vBlocks)
            {
                i = i + 1;
                AddHeader(doc, "Шаг " + i + ":");
                AddCanvas(doc, v, v.Length, v.Height);
            }

            return doc;
        }
        private FlowDocument ShowContainers()
        {
            var doc = new FlowDocument();

            AddMainHeader(doc, "Контейнеры " + "(" + containers.Count() + "ед.");
            var i = 0;
            if (!containers.Any())
            {
                AddHeader(doc, "Нет контейнеров для отображения");
            }
            foreach (var c in containers)
            {
                i = i + 1;
                AddContainer(doc, c);
            }
            return doc;
        }
        public List<int> DistinctOrdersInRow(List<RowBlock> rBlocks)
        {
            List<int> orderList = new List<int>();

            foreach (RowBlock r in rBlocks)
            {
                if (!orderList.Contains(r.Order))
                {
                    orderList.Add(r.Order);
                }
            }
            return orderList;
        }
    }
}