using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using VisualPacker.Models;

namespace VisualPacker.ViewModels
{
    public class CalculationContainerList
    {
        public FlowDocument ShowContainers(List<Container> containers, List<Vehicle> vehicles)
        {
            var doc = new FlowDocument();
            foreach (var vehicle in vehicles)
            {
                var tempList = UnpackVehicle(vehicle);
                AddMainHeader(doc,
                    "Список контейнеров для автомобиля " + vehicle.Name + "(" + tempList.Count() +
                    " контейнеров; общий вес груза " + vehicle.Mass + " кг.)");
                AddTable(doc, tempList);
            }
            if (containers.Any())
            {
                AddMainHeader(doc,
                    "Список контейнеров не поместившихся в машины " + "(" + containers.Count() + " контейнеров)");
                AddTable(doc, containers);
            }
            else
            {
                AddMainHeader(doc, "Все контейнеры загружены.");
            }
            return doc;
        }

        private void AddMainHeader(FlowDocument doc, String text)
        {
            var paragraph = new Paragraph(new Run(text))
            {
                FontSize = 16,
                FontStyle = FontStyles.Italic,
                TextAlignment = TextAlignment.Center
            };
            doc.Blocks.Add(paragraph);
        }

        private List<Container> UnpackVehicle(Vehicle vehicle)
        {
            var tempList = new List<Container>();
            foreach (var rBlock in vehicle.Blocks)
            {
                tempList = UnpackRow(tempList, rBlock);
            }
            foreach (var c in vehicle.SmallBlocks)
            {
                tempList.Add(c);
            }

            return tempList;
        }

        private List<Container> UnpackRow(List<Container> tempList, RowBlock rBlock)
        {
            foreach (var vBlock in rBlock.Blocks)
            {
                tempList = UnpackVerticalBlock(tempList, vBlock);
            }
            return tempList;
        }

        private List<Container> UnpackVerticalBlock(List<Container> tempList, VerticalBlock vBlock)
        {
            foreach (var cont in vBlock.Blocks)
            {
                if (cont is VerticalBlock)
                {
                    tempList = UnpackVerticalBlock(tempList, (VerticalBlock)cont);
                }
                else if (cont is HorizontalBlock)
                {
                    tempList = unpackHorizontalBlock(tempList, (HorizontalBlock)cont);
                }
                else if (cont is Container)
                {
                    tempList.Add(cont);
                }
                else
                {
                    MessageBox.Show("В процедуру unpackVerticalBlock передан неизвестный объект");
                }
            }
            return tempList;
        }

        private List<Container> unpackHorizontalBlock(List<Container> tempList, HorizontalBlock hBlock)
        {
            foreach (var cont in hBlock.Blocks)
            {
                if (cont is Container)
                {
                    tempList.Add(cont);
                }
                // else if (cont is VerticalBlock) { tempList= unpackVerticalBlock(tempList,(VerticalBlock) cont); }
                else
                {
                    MessageBox.Show("В процедуру unpackHorizontalBlock передан неизвестный объект");
                }
            }
            return tempList;
        }

        private void AddTable(FlowDocument doc, List<Container> tempList)
        {
            // Create the Table...
            var table1 = new Table();
            // ...and add it to the FlowDocument Blocks collection.
            doc.Blocks.Add(table1);
            // Set some global formatting properties for the table.
            table1.CellSpacing = 10;
            table1.Background = Brushes.White;

            // Create 6 columns and add them to the table's Columns collection.
            const int numberOfColumns = 8;
            for (var x = 0; x < numberOfColumns; x++)
            {
                table1.Columns.Add(new TableColumn());

                // Set alternating background colors for the middle colums.
                table1.Columns[x].Background = x % 2 == 0 ? Brushes.Beige : Brushes.LightSteelBlue;
            }
            table1.Columns[0].Width = new GridLength(20);
            table1.Columns[1].Width = new GridLength(90);
            table1.Columns[2].Width = new GridLength(100);
            table1.Columns[3].Width = new GridLength(120);
            table1.Columns[4].Width = new GridLength(40);
            table1.Columns[5].Width = new GridLength(70);
            table1.Columns[6].Width = new GridLength(70);
            table1.Columns[7].Width = new GridLength(60);

            //Добавляем заголовок таблицы
            table1.RowGroups.Add(new TableRowGroup());

            // AddContainer the first (title) row.
            table1.RowGroups[0].Rows.Add(new TableRow());
            var currentRow = table1.RowGroups[0].Rows[0];
            currentRow.Background = Brushes.Silver;
            currentRow.FontSize = 14;
            currentRow.FontWeight = FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("№"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("LPN"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Тип контейнера"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Габариты"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Вес, кг"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Цена"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Приоритет"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Очередность загрузки"))));
            //Добавляем строки
            var i = 0;
            foreach (var c in tempList)
            {
                i++;
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[i];
                currentRow.Background = Brushes.White;
                currentRow.FontSize = 14;
                currentRow.FontWeight = FontWeights.Normal;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(i.ToString()))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(c.Name))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(c.ContainerType))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(c.Vgh))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(c.Mass.ToString()))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(c.Price.ToString()))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(c.PriorityString))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(c.Order.ToString()))));
            }
        } 
    }
}