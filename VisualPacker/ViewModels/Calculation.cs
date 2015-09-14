using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using VisualPacker.Models;
using VisualPacker.ViewModels.Helpers;

namespace VisualPacker.ViewModels
{
    public class Calculation
    {
        FromTempListToContList fromTempListToContList=new FromTempListToContList();
        public Point3D CalculateMassCenterRow(RowBlock rBlock)
        {
            double nLength = 0;
            double nWidth = 0;
            double nHeight = 0;

            var massCenterPoint = new Point3D(0, 0, 0);
            if (rBlock.Mass == 0)
            {
                return massCenterPoint;
            }
            foreach (var v in rBlock.Blocks)
            {
                foreach (var c in v.Blocks)
                {
                    nLength = nLength + c.Mass*(c.FirstPoint.Y + c.Length/2);
                    nWidth = nWidth + c.Mass*(c.FirstPoint.X + c.Width/2);
                    nHeight = nHeight + c.Mass*(c.FirstPoint.Z + c.Height/2);
                }
            }
            massCenterPoint.Y = nLength/rBlock.Mass;
            massCenterPoint.X = nWidth/rBlock.Mass;
            massCenterPoint.Z = nHeight/rBlock.Mass;
            return massCenterPoint;
        }

        public List<Container> CalculateLoadScheme(List<Container> containers,
            ObservableCollection<Vehicle> selectedVehicles, TextBox textBox, int maxTonnage)
        {
            int tempMaxTonnage;
            var widthBetweenVehicles = 1000;
            var tempPoint = new Point3D(0, 0, 0);
            textBox.Clear();
            textBox.AppendText("Протокол расчета схемы загрузки:\n");

            var tempList = RotateContainers(containers);

            foreach (var vehicle in selectedVehicles)
            {
                tempMaxTonnage = maxTonnage == 0 ? vehicle.Tonnage : maxTonnage;
                tempList = vehicle.DownloadContainers(tempList, tempMaxTonnage);
                SetFirstPoint(tempPoint,vehicle);
                tempPoint.Y = tempPoint.Y + widthBetweenVehicles + vehicle.Width;// TODO tempPoint.Z = tempPoint.Z + widthBetweenVehicles + vehicle.Width;
                PutCargoInfoInTextBox(vehicle, textBox);
                CheckOverweight(vehicle, textBox, tempMaxTonnage);
                VehicleAxisMass vehicleAxisMass = new VehicleAxisMass(vehicle, vehicle.Mass);
                PutVehAxisMassInfoInTextBox(vehicleAxisMass.AxisMassCalculate(), textBox);
            }
            PutWasteContainersInfoInTextBox(tempList, textBox);
            CheckErrors(tempList, textBox, selectedVehicles, containers);

            return tempList;
        }

        private void PutVehAxisMassInfoInTextBox(List<double> axisMassList,TextBox textBox)
        {
            foreach (var axisMass in axisMassList)
            {
                textBox.AppendText("Нагрузка на ось " + axisMassList.IndexOf(axisMass) + " " + axisMass + " :\n");
            }

        }

        private void SetFirstPoint(Point3D point, Vehicle vehicle)
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
        private List<Container> RotateContainers(List<Container> tempList)
        {
            //поворачиваем контейнеры у которых длинна меньше ширины
            foreach (var c in tempList)
            {
                if (c.Length < c.Width & c.DirLength != "x")
                {
                    c.RotateH();
                }
            }
            return tempList;
        }

        private void PutCargoInfoInTextBox(Vehicle v, TextBox textBox)
        {
            textBox.AppendText("Загрузка автомобиля " + v.Name + " :\n");
            textBox.AppendText("  количество контейнеров - " + v.Count + " :\n");
            textBox.AppendText("  вес груза - " + v.Mass + " :\n");
        }

        private void CheckOverweight(Vehicle vehicle, TextBox textBox, int MaxTonnage)
        {
            var p = vehicle.GetMassCenter();
            var maxTonnage = MaxTonnage*(vehicle.EmptyTonnage*vehicle.Length + 2*p.X*vehicle.Mass)/(vehicle.Length*(vehicle.EmptyTonnage + vehicle.Mass));
            if (vehicle.Mass > maxTonnage & p.X < vehicle.Length/2)
            {
                textBox.AppendText("Превышение нагрузки на переднюю ось \n");
            }
            if (vehicle.Mass > maxTonnage & p.X > vehicle.Length/2)
            {
                textBox.AppendText("Превышение нагрузки на заднюю ось \n");
            }
            if (vehicle.Mass > maxTonnage & p.X == vehicle.Length/2)
            {
                textBox.AppendText("Превышение нагрузки на все оси \n");
            }
        }

        private void PutWasteContainersInfoInTextBox(List<Container> tempList, TextBox textBox)
        {
            if (tempList.Count > 0)
            {
                textBox.AppendText(tempList.Count + " контейнеров не вместилось в выбранные автомобили:\n");
                foreach (var t in tempList)
                {
                    textBox.AppendText("  " + t.Name + " " + t.Vgh + " " + t.PriorityString + " приоритет " + "\n");
                }
            }
            else
            {
                textBox.AppendText("Все контейнеры успешно загружены.\n\n");
            }
        }

        private void CheckErrors(List<Container> tempList, TextBox textBox,
            ObservableCollection<Vehicle> selectedVehicles, List<Container> containers)
        {
            CheckIfPriorityContainerInWasteList(tempList, textBox);
            CheckContainerCount(containers, tempList, textBox, selectedVehicles);
        }

        private void CheckIfPriorityContainerInWasteList(List<Container> tempList, TextBox textBox)
        {
            var priorityError = 0;
            foreach (var t in tempList)
            {
                if (t.Priority == 0)
                {
                    priorityError++;
                }
            }
            if (priorityError > 0)
            {
                textBox.AppendText("Ошибка.Один или несколько приоритетных контейнеров не загрузились.\n");
            }
        }

        private void CheckContainerCount(List<Container> containers, List<Container> tempList, TextBox textBox,
            ObservableCollection<Vehicle> selectedVehicles)
        {
            var contCount = XmlHelper.ContainersCount(tempList);

            var newList = fromTempListToContList.ToContainerList(tempList);
            foreach (var v in selectedVehicles)
            {
                contCount = contCount + v.Count;
                newList.AddRange(v.VehicleToContainerList());
            }
            if (containers.Count() != contCount)
            {
                textBox.AppendText("Ошибка. Расхождение количества контейнеров: было " + containers.Count() + ", стало" +
                                   contCount + ".\n");
                PutContainerVarianceInTextBox(newList, containers, textBox);
            }
        }

        private void PutContainerVarianceInTextBox(List<Container> newList, List<Container> containers,
            TextBox textBox)
        {
            foreach (var c in containers)
            {
                var newList2 = newList.Where(t => t.Name == c.Name).ToList();
                if (!newList2.Any())
                {
                    textBox.AppendText("Пропавший контейнер: " + c.Name + ".\n");
                }
                if (newList2.Count() > 1)
                {
                    textBox.AppendText("Задублированный контейнер: " + c.Name + ".\n");
                }
            }
        }
    }
}