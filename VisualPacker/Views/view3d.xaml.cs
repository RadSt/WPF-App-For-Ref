﻿using System;
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
            InitializeComponent();
            DrawScene(MainViewport, data);
        }

        public static void DrawScene(Viewport3D mainViewport, ObservableCollection<Vehicle> selectedVehicles)
        {
            var tempPoint = new Point3D(0, 0, 0);
            const int widthBetweenVehicles = 1000;
            PerspectiveCamera cam = new PerspectiveCamera(new Point3D(9000, 5000, 24000),
            new Vector3D(-1, -2, -10), new Vector3D(0, -1, 0), 50000);
            mainViewport.Camera = cam;

            // Create another ModelVisual3D for light.
            ModelVisual3D modvis = new ModelVisual3D();
            modvis.Content = new AmbientLight(Colors.White);
            mainViewport.Children.Add(modvis);

            foreach (Vehicle vehicle in selectedVehicles)
            {
                vehicle.SetFirstPoint(tempPoint);
                tempPoint.Z = tempPoint.Z - widthBetweenVehicles - vehicle.Width;
                //v.SetFirstPointForVerticalBlock(tempPoint);
                Drawing3D.DrawVehicle3D(mainViewport, vehicle);
                //tempPoint.Y = tempPoint.Y + 1000 + v.Width;

            }
            
        }
        private void saveToFile(Viewport3D v, int width, int height)
        {
            if (Directory.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData") == false)
            { Directory.CreateDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData"); }

            string destination = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\renderedData\\" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + " " +
                DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + " " + Environment.UserName + " 3dView.png";

            Rectangle vRect = new Rectangle();
            vRect.Width = width;
            vRect.Height = height;
            vRect.Fill = Brushes.White;
            vRect.Arrange(new Rect(0, 0, vRect.Width, vRect.Height));

            var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

            bmp.Render(vRect);
            bmp.Render(v);

            var png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = File.Create(destination)) { png.Save(stm); }
        }
        public void print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            printDialog.PrintTicket = printDialog.PrintQueue.DefaultPrintTicket;
            printDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(MainViewport
                , "Печать");
            }
        }

        //private void Save_Click(object sender, RoutedEventArgs e)
        //{
        //    saveToFile(MainViewport, (int)MainViewport.Width, (int)MainViewport.Height);
        //}

        private void WindowClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            saveToFile(MainViewport, (int)MainViewport.Width, (int)MainViewport.Height);
        }
    }
}
