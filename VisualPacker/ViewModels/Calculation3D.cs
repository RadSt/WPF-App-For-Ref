using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualPacker.Models;
using _3DTools;

namespace VisualPacker.ViewModels
{
    public class Calculation3D
    {
        public void DrawScene(Viewport3D mainViewport, ObservableCollection<Vehicle> selectedVehicles)
        {
            PerspectiveCamera cam = new PerspectiveCamera(new Point3D(9000, 5000, 24000),
            new Vector3D(-1, -2, -10), new Vector3D(0, -1, 0), 50000);
            mainViewport.Camera = cam;

            // Create another ModelVisual3D for light.
            ModelVisual3D modvis = new ModelVisual3D();
            modvis.Content = new AmbientLight(Colors.White);
            mainViewport.Children.Add(modvis);

            foreach (Vehicle v in selectedVehicles)
            {
                DrawVehicle3D(mainViewport, v);

            }

        }

        private void DrawVehicle3D(Viewport3D mainViewport, Vehicle vehicle)
        {
            var pointsArray = GetValue(vehicle);
            mainViewport.Children.Add(AddVehiclePoints(mainViewport, pointsArray));

            foreach (RowBlock rBlock in vehicle.Blocks)
            {
                //MessageBox.Show("Печатаем rowBlocks");
                foreach (VerticalBlock vBlock in rBlock.Blocks)
                {
                    //MessageBox.Show("Печатаем verticalBlocks");
                    foreach (Container c in vBlock.Blocks)
                    {
                        // MessageBox.Show("Печатаем Container");
                        GeometryModel3D geomod = new GeometryModel3D();
                        Brush brush;
                        if (c.Color == "" | c.Kind == "VerticalPallet")
                        {

                            brush = Brushes.Blue;
                        }
                        else
                        {
                            brush = new BrushConverter().ConvertFromString(c.Color) as SolidColorBrush;
                        }
                        geomod.Material = new DiffuseMaterial(brush);
                        DrawCube(c, geomod);
                        // Create ModelVisual3D for GeometryModel3D.
                        ModelVisual3D modvis = new ModelVisual3D {Content = geomod};
                        mainViewport.Children.Add(modvis);
                        ScreenSpaceLines3D wireCube = DrawWireCube(c);
                        mainViewport.Children.Add(wireCube);
                    }
                }
            }
        }

        private ScreenSpaceLines3D AddVehiclePoints(Viewport3D mainViewport, Point3D[] pointsArray)
        {
            ScreenSpaceLines3D wireVehicle=new ScreenSpaceLines3D();
            Color color = Colors.Red;
            const int width = 1;

            wireVehicle.Thickness = width;
            wireVehicle.Color = color;
            wireVehicle.Points.Add(pointsArray[0]);
            wireVehicle.Points.Add(pointsArray[1]);
            wireVehicle.Points.Add(pointsArray[1]);
            wireVehicle.Points.Add(pointsArray[2]);
            wireVehicle.Points.Add(pointsArray[2]);
            wireVehicle.Points.Add(pointsArray[3]);
            wireVehicle.Points.Add(pointsArray[3]);
            wireVehicle.Points.Add(pointsArray[0]);
            wireVehicle.Points.Add(pointsArray[4]);
            wireVehicle.Points.Add(pointsArray[5]);
            wireVehicle.Points.Add(pointsArray[5]);
            wireVehicle.Points.Add(pointsArray[6]);
            wireVehicle.Points.Add(pointsArray[6]);
            wireVehicle.Points.Add(pointsArray[7]);
            wireVehicle.Points.Add(pointsArray[7]);
            wireVehicle.Points.Add(pointsArray[4]);
            wireVehicle.Points.Add(pointsArray[0]);
            wireVehicle.Points.Add(pointsArray[4]);
            wireVehicle.Points.Add(pointsArray[1]);
            wireVehicle.Points.Add(pointsArray[5]);
            wireVehicle.Points.Add(pointsArray[2]);
            wireVehicle.Points.Add(pointsArray[6]);
            wireVehicle.Points.Add(pointsArray[3]);
            wireVehicle.Points.Add(pointsArray[7]);
            return wireVehicle;
        }

        private Point3D[] GetValue(Vehicle vehicle)
        {
            Point3D[] pointsArray = new Point3D[8];

            pointsArray[0] = new Point3D(vehicle.FirstPoint.X, vehicle.FirstPoint.Z, vehicle.FirstPoint.Y);
            pointsArray[1] = new Point3D(vehicle.FirstPoint.X + vehicle.Length, vehicle.FirstPoint.Z, vehicle.FirstPoint.Y);
            pointsArray[2] = new Point3D(vehicle.FirstPoint.X + vehicle.Length, vehicle.FirstPoint.Z, vehicle.FirstPoint.Y + vehicle.Width);
            pointsArray[3] = new Point3D(vehicle.FirstPoint.X, vehicle.FirstPoint.Z, vehicle.FirstPoint.Y + vehicle.Width);
            pointsArray[4] = new Point3D(vehicle.FirstPoint.X, vehicle.FirstPoint.Z + vehicle.Height, vehicle.FirstPoint.Y);
            pointsArray[5] = new Point3D(vehicle.FirstPoint.X + vehicle.Length, vehicle.FirstPoint.Z + vehicle.Height, vehicle.FirstPoint.Y);
            pointsArray[6] = new Point3D(vehicle.FirstPoint.X + vehicle.Length, vehicle.FirstPoint.Z + vehicle.Height, vehicle.FirstPoint.Y + vehicle.Width);
            pointsArray[7] = new Point3D(vehicle.FirstPoint.X, vehicle.FirstPoint.Z + vehicle.Height, vehicle.FirstPoint.Y + vehicle.Width);
            return pointsArray;
        }

        private ScreenSpaceLines3D DrawWireCube(Container v)
        {
            ScreenSpaceLines3D wireCube = new ScreenSpaceLines3D();
            Color c = Colors.Black;
            const int width = 2;
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
        private void DrawCube(Container item, GeometryModel3D geomod)
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