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
        public View2D(Object data)
        {
            Calculation2D calculation2D = new Calculation2D();
            InitializeComponent();
            FlowDocViewer.Document = calculation2D.ShowVehicles(data);
        }
        private void print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog {PrintQueue = LocalPrintServer.GetDefaultPrintQueue()};
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
        private void SaveToFile()
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
