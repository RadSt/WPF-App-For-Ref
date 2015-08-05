using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualPacker.Models;
using _3DTools;

namespace VisualPacker.ViewModels
{
    static class Drawing
    {
        public static void DrawLine(Viewport3D mainViewport, Point3D p1, Point3D p2)
        {
            ScreenSpaceLines3D line = new ScreenSpaceLines3D {Color = Colors.Red};
            line.Points.Add(p1);
            line.Points.Add(p2);
            line.Thickness = 1;
            mainViewport.Children.Add(line);
        }
        public static void DrawVehicle2(Viewport3D mainViewport, Vehicle v)
        {

            ScreenSpaceLines3D wireVehicle = new ScreenSpaceLines3D();
            Color color = Colors.Red;
            int width = 1;

            Point3D p0 = new Point3D(v.FirstPoint.X, v.FirstPoint.Z, v.FirstPoint.Y);
            Point3D p1 = new Point3D(v.FirstPoint.X + v.Length, v.FirstPoint.Z, v.FirstPoint.Y);
            Point3D p2 = new Point3D(v.FirstPoint.X + v.Length, v.FirstPoint.Z,v.FirstPoint.Y+ v.Width);
            Point3D p3 = new Point3D(v.FirstPoint.X, v.FirstPoint.Z, v.FirstPoint.Y+v.Width);
            Point3D p4 = new Point3D(v.FirstPoint.X, v.FirstPoint.Z + v.Height, v.FirstPoint.Y);
            Point3D p5 = new Point3D(v.FirstPoint.X + v.Length, v.FirstPoint.Z + v.Height, v.FirstPoint.Y);
            Point3D p6 = new Point3D(v.FirstPoint.X + v.Length, v.FirstPoint.Z + v.Height, v.FirstPoint.Y+v.Width);
            Point3D p7 = new Point3D(v.FirstPoint.X, v.FirstPoint.Z + v.Height, v.FirstPoint.Y+v.Width);

            wireVehicle.Thickness = width;
            wireVehicle.Color = color;
            wireVehicle.Points.Add(p0);
            wireVehicle.Points.Add(p1);
            wireVehicle.Points.Add(p1);
            wireVehicle.Points.Add(p2);
            wireVehicle.Points.Add(p2);
            wireVehicle.Points.Add(p3);
            wireVehicle.Points.Add(p3);
            wireVehicle.Points.Add(p0);
            wireVehicle.Points.Add(p4);
            wireVehicle.Points.Add(p5);
            wireVehicle.Points.Add(p5);
            wireVehicle.Points.Add(p6);
            wireVehicle.Points.Add(p6);
            wireVehicle.Points.Add(p7);
            wireVehicle.Points.Add(p7);
            wireVehicle.Points.Add(p4);
            wireVehicle.Points.Add(p0);
            wireVehicle.Points.Add(p4);
            wireVehicle.Points.Add(p1);
            wireVehicle.Points.Add(p5);
            wireVehicle.Points.Add(p2);
            wireVehicle.Points.Add(p6);
            wireVehicle.Points.Add(p3);
            wireVehicle.Points.Add(p7);
            mainViewport.Children.Add(wireVehicle);
 
            foreach (RowBlock rBlock in v.Blocks)
            {
                //MessageBox.Show("Печатаем rowBlocks");
                foreach (VerticalBlock vBlock in rBlock.Blocks)
                {
                    //MessageBox.Show("Печатаем verticalBlocks");
                    foreach (Container c in vBlock.Blocks)
                    {
                        // MessageBox.Show("Печатаем Container");
                        // MessageBox.Show(c.FirstPoint.ToString()+" "+c.Length+" "+c.Width+" "+c.Height);


                        GeometryModel3D geomod = new GeometryModel3D();

                        Brush brush = new SolidColorBrush();
                        if (c.Color == "" | c.Kind == "VerticalPallet")
                        {

                            brush = Brushes.Blue;
                        }
                        else
                        {
                            brush = new BrushConverter().ConvertFromString(c.Color) as SolidColorBrush;
                            //brush = new BrushConverter().ConvertFromString(c.Color) as SolidColorBrush;
                        }
                        geomod.Material = new DiffuseMaterial(brush);
                        DrawCube(c, geomod);
                        // Create ModelVisual3D for GeometryModel3D.
                        ModelVisual3D modvis = new ModelVisual3D();
                        modvis.Content = geomod;
                        mainViewport.Children.Add(modvis);
                        ScreenSpaceLines3D wireCube = DrawWireCube(c);
                        mainViewport.Children.Add(wireCube);
                        //break;

                    }
                    //break;
                }
                //break;
            } 
        }
        public static ScreenSpaceLines3D DrawWireCube( Container v)
        {
            ScreenSpaceLines3D wireCube = new ScreenSpaceLines3D();
            Color c = Colors.Black;
            int width = 2;
            Point3D p0 = new Point3D(v.FirstPoint.X, v.FirstPoint.Z, v.FirstPoint.Y);
            Point3D p1 = new Point3D(v.FirstPoint.X + v.Width, v.FirstPoint.Z, v.FirstPoint.Y);
            Point3D p2 = new Point3D(v.FirstPoint.X + v.Width, v.FirstPoint.Z, v.FirstPoint.Y + v.Length);
            Point3D p3 = new Point3D(v.FirstPoint.X, v.FirstPoint.Z, v.FirstPoint.Y + v.Length);
            Point3D p4 = new Point3D(v.FirstPoint.X, v.FirstPoint.Z + v.Height, v.FirstPoint.Y);
            Point3D p5 = new Point3D(v.FirstPoint.X + v.Width, v.FirstPoint.Z + v.Height, v.FirstPoint.Y);
            Point3D p6 = new Point3D(v.FirstPoint.X + v.Width, v.FirstPoint.Z + v.Height, v.FirstPoint.Y + v.Length);
            Point3D p7 = new Point3D(v.FirstPoint.X, v.FirstPoint.Z + v.Height, v.FirstPoint.Y + v.Length);

            wireCube.Thickness = width;
            wireCube.Color = c;
            //рисуем нижнюю грань
            wireCube.Points.Add(p0);
            wireCube.Points.Add(p1);
            wireCube.Points.Add(p1);
            wireCube.Points.Add(p2);
            wireCube.Points.Add(p2);
            wireCube.Points.Add(p3);
            wireCube.Points.Add(p3);
            wireCube.Points.Add(p0);
            //рисуем верхнюю грань
            wireCube.Points.Add(p4);
            wireCube.Points.Add(p5);
            wireCube.Points.Add(p5);
            wireCube.Points.Add(p6);
            wireCube.Points.Add(p6);
            wireCube.Points.Add(p7);
            wireCube.Points.Add(p7);
            wireCube.Points.Add(p4);
            
            //рисуем Боковую грань
            wireCube.Points.Add(p0);
            wireCube.Points.Add(p4);
            wireCube.Points.Add(p1);
            wireCube.Points.Add(p5);
            wireCube.Points.Add(p2);
            wireCube.Points.Add(p6);
            wireCube.Points.Add(p3);
            wireCube.Points.Add(p7);
            if (v.Kind == "VerticalPallet")
            {//рисуем диагональные линии на боковую грань
                wireCube.Points.Add(p6);
                wireCube.Points.Add(p3);
                wireCube.Points.Add(p2);
                wireCube.Points.Add(p7);
            }
            return wireCube;
        }
        public static void DrawCube(Container item, GeometryModel3D geomod)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(item.FirstPoint.X, item.FirstPoint.Z, item.FirstPoint.Y));
            mesh.Positions.Add(new Point3D(item.FirstPoint.X + item.Width, item.FirstPoint.Z, item.FirstPoint.Y));
            mesh.Positions.Add(new Point3D(item.FirstPoint.X, item.FirstPoint.Z + item.Height, item.FirstPoint.Y));
            mesh.Positions.Add(new Point3D(item.FirstPoint.X + item.Width, item.FirstPoint.Z + item.Height, item.FirstPoint.Y));
            mesh.Positions.Add(new Point3D(item.FirstPoint.X, item.FirstPoint.Z, item.FirstPoint.Y + item.Length));
            mesh.Positions.Add(new Point3D(item.FirstPoint.X + item.Width, item.FirstPoint.Z, item.FirstPoint.Y + item.Length));
            mesh.Positions.Add(new Point3D(item.FirstPoint.X, item.FirstPoint.Z + item.Height, item.FirstPoint.Y + item.Length));
            mesh.Positions.Add(new Point3D(item.FirstPoint.X + item.Width, item.FirstPoint.Z + item.Height, item.FirstPoint.Y + item.Length));
            mesh.TriangleIndices = new Int32Collection(new int[] { 0, 2, 1, 1, 2, 3, 0, 4, 2, 2, 4, 6, 0, 1, 4, 1, 5, 4, 1, 7, 5, 1, 3, 7, 4, 5, 6, 7, 6, 5, 2, 6, 3, 3, 6, 7 });
            geomod.Geometry = mesh;
        }
    }
}
