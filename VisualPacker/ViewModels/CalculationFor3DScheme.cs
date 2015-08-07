using System.Windows.Media.Media3D;
using VisualPacker.Models;

namespace VisualPacker.ViewModels
{
    public class CalculationFor3DScheme
    {

        public void SetFirstPoint(Point3D point, Vehicle vehicle)
        {
            //присваиваем начальные координаты для груза
            vehicle.FirstPoint = point;
            Point3D tempPoint = point;
            foreach (RowBlock r in vehicle.Blocks)
            {
                r.SetFirstPointForVerticalBlock(tempPoint);
                tempPoint.X = tempPoint.X + r.Width;
            }
        }
         
    }
}