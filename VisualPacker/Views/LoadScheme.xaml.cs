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
    ///     Interaction logic for LoadSchemeCalculation.xaml
    /// </summary>
    public partial class LoadScheme
    {
        public LoadScheme(Object data)
        {
            LoadSchemeCalculation loadScheme=new LoadSchemeCalculation();
            InitializeComponent();
            flowDocViewer.Document = loadScheme.RotateContainers(data);
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var doc = flowDocViewer.Document;
                doc.PagePadding = new Thickness(20, 40, 20, 40);
                doc.PageHeight = printDialog.PrintableAreaHeight;
                doc.PageWidth = printDialog.PrintableAreaWidth - 40;
                doc.ColumnWidth = printDialog.PrintableAreaWidth - 40;
                doc.ColumnGap = 0;
                printDialog.PrintDocument(((IDocumentPaginatorSource) flowDocViewer.Document).DocumentPaginator
                    , "Печать");
            }
        }
       
    }
}