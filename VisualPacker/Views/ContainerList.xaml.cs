using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using VisualPacker.Models;
using VisualPacker.ViewModels;

namespace VisualPacker.Views

{
    /// <summary>
    /// Interaction logic for ContainerList.xaml
    /// </summary>
    public partial class ContainerList : Window
    {
        //public ObservableCollection<Vehicle> vehicles=new ObservableCollection<Vehicle>();
        public List<Vehicle> vehicles = new List<Vehicle>();
        public List<Container> containers =new List<Container>();
        public ContainerList()
        {
            InitializeComponent();
            
        }
        public void print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument doc = flowDocViewer.Document;
                doc.PagePadding = new Thickness(40, 40, 40, 40);
                doc.PageHeight = printDialog.PrintableAreaHeight-80;
                doc.PageWidth = printDialog.PrintableAreaWidth -80;
                doc.ColumnWidth = printDialog.PrintableAreaWidth-80;
                doc.ColumnGap = 0;


                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDocViewer.Document).DocumentPaginator
                , "Печать");
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
        public List<Container> unpackVehicle(Vehicle v)
        {   
            List<Container> tempList=new List<Container>();
            foreach (RowBlock rBlock in v.Blocks) 
            {
                tempList= unpackRow(tempList,rBlock);
                }
            foreach (Container c in v.SmallBlocks)  {tempList.Add(c);}

            return tempList;
         }
        public List<Container> unpackRow(List<Container> tempList, RowBlock rBlock)
        {   
            foreach (VerticalBlock vBlock in rBlock.Blocks) 
            {
                tempList= unpackVerticalBlock(tempList,vBlock);
            }
            return tempList;
         }
        
         public List<Container> unpackVerticalBlock(List<Container> tempList, VerticalBlock vBlock)
        {   
            foreach (Object cont in vBlock.Blocks) 
               {
               if (cont is VerticalBlock)  {  tempList= unpackVerticalBlock(tempList,(VerticalBlock) cont); }
               else if (cont is HorizontalBlock) { tempList = unpackHorizontalBlock(tempList, (HorizontalBlock)cont); }
               else if (cont is Container)  {tempList.Add((Container)cont);}
               else {MessageBox.Show("В процедуру unpackVerticalBlock передан неизвестный объект");}
               }
            return tempList;
         } 
        private List<Container> unpackHorizontalBlock(List<Container> tempList, HorizontalBlock hBlock)
        {   
            foreach (Object cont in hBlock.Blocks) 
               { if (cont is Container)  { tempList.Add((Container)cont);}
               // else if (cont is VerticalBlock) { tempList= unpackVerticalBlock(tempList,(VerticalBlock) cont); }
               else {MessageBox.Show("В процедуру unpackHorizontalBlock передан неизвестный объект");}
               }
            return tempList;
         }
         public void AddHeader(FlowDocument doc, String text)
         {
             Paragraph p = new Paragraph(new Run(text));
             p.FontSize = 20;
             p.FontStyle = FontStyles.Italic;
             p.TextAlignment = TextAlignment.Left;
             doc.Blocks.Add(p);
         }
         public void AddTable(FlowDocument doc, List<Container> tempList)
         {
             // Create the Table...
             Table table1 = new Table();
             // ...and add it to the FlowDocument Blocks collection.
             doc.Blocks.Add(table1);
             // Set some global formatting properties for the table.
             table1.CellSpacing = 10;
             table1.Background = Brushes.White;

             // Create 6 columns and add them to the table's Columns collection.
             int numberOfColumns = 8;
             for (int x = 0; x < numberOfColumns; x++)
             {
                 table1.Columns.Add(new TableColumn());

                 // Set alternating background colors for the middle colums.
                 if (x % 2 == 0)
                     table1.Columns[x].Background = Brushes.Beige;
                 else
                     table1.Columns[x].Background = Brushes.LightSteelBlue;
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
             TableRow currentRow = table1.RowGroups[0].Rows[0];
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
             int i = 0;
             foreach (Container c in tempList)
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
        public void ShowContainers()
        {
           
            FlowDocument doc = new FlowDocument();
            foreach (Vehicle v in vehicles)
            {  List<Container> tempList=unpackVehicle(v);
                AddMainHeader(doc, "Список контейнеров для автомобиля " + v.Name + "(" + tempList.Count() + " контейнеров; общий вес груза " + v.Mass + " кг.)");
                AddTable(doc, tempList);
            }
            flowDocViewer.Document = doc;
            if (containers.Count() > 0)
            {
                AddMainHeader(doc, "Список контейнеров не поместившихся в машины " + "(" + containers.Count() + " контейнеров)");
                AddTable(doc, containers);
            }
            else { AddMainHeader(doc, "Все контейнеры загружены."); }

        }
       
    }
}
