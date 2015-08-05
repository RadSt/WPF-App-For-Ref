using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;

namespace VisualPacker.Models
{
    public class RowBlock
    {
        public RowBlock()
        {
            Height = 0;
            MinHeight = 0;
            Length = 0;
            MaxLength = 0;
            Width = 0;
            Mass = 0;
            RealVolume = 0;
            Included = true;
            Order = 0;
            Priority = 0;
            Count = 0;
        }

        public int Height { get; set; }
        public int MinHeight { get; set; }
        public int Length { get; set; }
        public int MaxLength { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public double Mass { get; set; }
        public double RealVolume { get; set; }
        public bool Included { get; set; }
        public int Order { get; set; }
        public int Priority { get; set; }
        public Point3D FirstPoint { get; set; } //положение левого нижнего угла
        public List<VerticalBlock> Blocks = new List<VerticalBlock>();
        public int Count { get; set; }

        public double Fullness
        {
            get { return Math.Round((RealVolume/1000)/((MaxLength/10)*(Width/10)*(Height/10)), 2); }
        }

        public double FullnessWidth
        {
            get
            {
                double vol = ((Length/10)*(Width/10)*(MinHeight/10));
                return Math.Round(vol/((MaxLength/10)*(Width/10)*(MinHeight/10)), 2);
            }
        }

        public bool ContainsVerticalBlock(VerticalBlock vBlock, List<VerticalBlock> blocks)
        {
            foreach (VerticalBlock verticalBlock in Blocks)
            {
                if (vBlock.Blocks[0].Name == verticalBlock.Blocks[0].Name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Add(VerticalBlock verticalBlock, int mLength)
        {
            MaxLength = mLength;
            if (((Length + verticalBlock.Length) <= MaxLength))
            {
                Blocks.Add(verticalBlock);
                Length = Length + verticalBlock.Length;

                Width = Math.Max(Width, verticalBlock.Width);
                Height = Math.Max(Height, verticalBlock.Height);
                MinHeight = MinHeight == 0 ? verticalBlock.Height : Math.Max(MinHeight, verticalBlock.Height);
                Mass = Mass + verticalBlock.Mass;
                RealVolume = RealVolume + verticalBlock.RealVolume;
                Count = Count + verticalBlock.Count;
                Order = verticalBlock.Order;
                return true;
            }
            if (verticalBlock.Length <= Width & (Length + verticalBlock.Width) <= MaxLength)
            {
                verticalBlock.RotateH();
                Blocks.Add(verticalBlock);
                Length = Length + verticalBlock.Length;

                Width = Math.Max(Width, verticalBlock.Width);
                Height = Math.Max(Height, verticalBlock.Height);
                Mass = Mass + verticalBlock.Mass;
                RealVolume = RealVolume + verticalBlock.RealVolume;
                Count = Count + verticalBlock.Count;
                Order = verticalBlock.Order;
                return true;
            }
            return false;
        }

        public void SetFirstPointForVerticalBlock(Point3D point)
        {
            FirstPoint = point;
            Point3D tempPoint = point;
            foreach (VerticalBlock v in Blocks)
            {
                v.SetFirstPointVerticalBlock(tempPoint);
                tempPoint.Y = tempPoint.Y + v.Length;
            }
        }
    }
}