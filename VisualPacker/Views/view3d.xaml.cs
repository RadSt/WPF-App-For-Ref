using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Printing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using VisualPacker.Models;
using VisualPacker.ViewModels;
using Path = System.IO.Path;

namespace VisualPacker.Views
{
    /// <summary>
    /// Interaction logic for View3D.xaml
    /// </summary>
    public partial class View3D
    {
        public View3D(ObservableCollection<Vehicle> data)
        {
            Calculation3D calculation3D = new Calculation3D();
            calculation3D.DrawScene(MainViewport, data);
            InitializeComponent();
        }
        private void WindowClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            SaveToFile(MainViewport, (int)MainViewport.Width, (int)MainViewport.Height);
        }
        private void print_Click(object sender, RoutedEventArgs e)
        {
            Print3DView();
        }
        private void Print3DView()
        {
            PrintDialog printDialog = new PrintDialog { PrintQueue = LocalPrintServer.GetDefaultPrintQueue() };
            printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;
            printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(MainViewport
                    , "Печать");
            }
        }
        private void SaveToFile(Viewport3D v, int width, int height)
        {
            if (Directory.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData") == false)
            { Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData"); }

            string destination = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData\\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + " " +
                DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + " " + Environment.UserName + " 3dView.png";

            Rectangle vRect = new Rectangle { Width = width, Height = height, Fill = Brushes.White };
            vRect.Arrange(new Rect(0, 0, vRect.Width, vRect.Height));

            var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

            bmp.Render(vRect);
            bmp.Render(v);

            var png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = File.Create(destination)) { png.Save(stm); }
        }
    }
}
