using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using VisualPacker.ViewModels;
using VisualPacker.ViewModels.Helpers;

namespace VisualPacker.Models
{
    public class Vehicle : ICloneable
    {
        Calculation calculation=new Calculation();
        public const int MinHeighCont=800;
        public Vehicle()
        {
            Length = 6000;
            Width = 2400;
            Height = 2400;
            MaxHeight = 0;
            Tonnage = 24000;
            FullTonnage = 24000;
            Mass = 0;
            Count = 0;
            DoorWidth = 2000;
            DoorHeight = 2000;
            DoorCenterDistance = 0;
            Roof = 200;
        }

        public int MaxLength { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxHeight { get; set; }
        public int Tonnage { get; set; }
        public int FullTonnage { get; set; }

        public int EmptyTonnage
        {
            get { return FullTonnage - Tonnage; }
        }

        public double Mass { get; set; }
        public List<RowBlock> Blocks = new List<RowBlock>();
        public List<Container> SmallBlocks = new List<Container>();
        public int Count { get; set; }
        /* //-- Параметры передней оси 
        private int front_axle_distance = 1600;
        public int Front_axle_distance { get { return front_axle_distance; } set { front_axle_distance = value; } } //Расстояние от оси, до начала грузового отсека
        private int front_axle_max_tonnage = 16000;
        public int Front_axle_max_tonnage { get { return front_axle_max_tonnage; } set { front_axle_max_tonnage = value; } }
        private int front_axle_empty_tonnage = 8900;
        public int Front_axle_empty_tonnage { get { return front_axle_empty_tonnage; } set { front_axle_empty_tonnage = value; } }
        private double front_axle_current_tonnage = 0;
        public double Front_axle_current_tonnage { get { return front_axle_current_tonnage; } set { front_axle_current_tonnage = value; } }
        //!-- Параметры задней оси -->
        private int back_axle_distance = 2000;
        public int Back_axle_distance { get { return back_axle_distance; } set { back_axle_distance = value; } } //Расстояние от оси, до конца грузового отсека
        private double back_axle_max_tonnage = 24000;
        public double Back_axle_max_tonnage { get { return back_axle_max_tonnage; } set { back_axle_max_tonnage = value; } }
        private double back_axle_empty_tonnage = 5700;
        public double Back_axle_empty_tonnage { get { return back_axle_empty_tonnage; } set { back_axle_empty_tonnage = value; } }
        private double back_axle_current_tonnage = 0;
        public double Back_axle_current_tonnage { get { return back_axle_current_tonnage; } set { back_axle_current_tonnage = value; } }
        private int back_axle_count = 1;
        public int Back_axle_count { get { return back_axle_count; } set { back_axle_count = value; } }*/
        //-- Положение двери на боковой стенке вагона (вагон грузится от двери к краям). -->
        public int DoorWidth { get; set; } // Ширина двери. -->
        public int DoorHeight { get; set; } //- Высота двери. -->
        public int DoorCenterDistance { get; set; } // Смещение двери относительно центра вагона. -->
        public int Roof { get; set; } // Высота скругленного свода (крыши) вагона в центре. -->
        public Point3D FirstPoint { get; set; } //положение левого нижнего угла
        public int EmptyAvtoTonnage { get; set; }
        public int EmptyTrailerTonnage { get; set; }
        public int TrailerAxisQuantity { get; set; }
        public List<double> AxisMassMeanList { get; set; }

        public const int AvtoAxisQuantity = 2;
        
        public object Clone()
        {
            Vehicle vehicle = new Vehicle
            {
                Name = Name,
                Kind = Kind,
                Length = Length,
                Width = Width,
                Height = Height,
                Tonnage = Tonnage,
                Mass = Mass,
                DoorWidth = DoorWidth,
                DoorHeight = DoorHeight,
                DoorCenterDistance = DoorCenterDistance,
                Roof = Roof,
                EmptyAvtoTonnage=EmptyAvtoTonnage,
                EmptyTrailerTonnage = EmptyTrailerTonnage,
                TrailerAxisQuantity = TrailerAxisQuantity

            };
            return vehicle;
        }

        public Point3D GetMassCenter()
        {
            Point3D point = new Point3D(0, 0, 0);
            double nLength = 0;
            double nWidth = 0;
            double nHeight = 0;
            foreach (RowBlock r in Blocks)
            {
                Point3D pointRow = calculation.CalculateMassCenterRow(r);
                nLength = nLength + r.Mass * pointRow.Y;
                nWidth = nWidth + r.Mass * pointRow.X;
                nHeight = nHeight + r.Mass * pointRow.Z;
            }
            point.Y = nLength / Mass;
            point.X = nWidth / Mass;
            point.Z = nHeight / Mass;
            return point;
        }

        public List<Container> VehicleToContainerList()
        {
            FromTempListToContList fromTempListToContList = new FromTempListToContList();
            List<Container> tempList = new List<Container>();
            fromTempListToContList.ToContainerList(tempList, Blocks);
            tempList.AddRange(SmallBlocks);
            return tempList;
        }
    }
}