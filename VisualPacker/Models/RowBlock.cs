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

        public bool ContainsVerticalBlock(VerticalBlock vBlock)
        {
            foreach (VerticalBlock v in Blocks)
            {
                if (vBlock.Blocks[0].Name == v.Blocks[0].Name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Add(VerticalBlock c, int mLength)
        {
            MaxLength = mLength;
            if (((Length + c.Length) <= MaxLength))
            {
                Blocks.Add(c);
                Length = Length + c.Length;

                Width = Math.Max(Width, c.Width);
                Height = Math.Max(Height, c.Height);
                MinHeight = MinHeight == 0 ? c.Height : Math.Max(MinHeight, c.Height);
                Mass = Mass + c.Mass;
                RealVolume = RealVolume + c.RealVolume;
                Count = Count + c.Count;
                Order = c.Order;
                return true;
            }
            if (c.Length <= Width & (Length + c.Width) <= MaxLength)
            {
                c.RotateH();
                Blocks.Add(c);
                Length = Length + c.Length;

                Width = Math.Max(Width, c.Width);
                Height = Math.Max(Height, c.Height);
                Mass = Mass + c.Mass;
                RealVolume = RealVolume + c.RealVolume;
                Count = Count + c.Count;
                Order = c.Order;
                return true;
            }
            return false;
        }

        public void SetFirstPoint(Point3D point)
        {
            FirstPoint = point;
            Point3D tempPoint = point;
            foreach (VerticalBlock v in Blocks)
            {
                v.SetFirstPoint(tempPoint);
                tempPoint.Y = tempPoint.Y + v.Length;
            }
        }

        public void ToContainerList(List<Container> tempList)
        {
            foreach (Object data in Blocks)
            {
                if (data is VerticalBlock)
                {
                    VerticalBlock c = (VerticalBlock) data;
                    c.ToContainerList(tempList);
                }
                else if (data is RowBlock)
                {
                    RowBlock c = (RowBlock) data;
                    c.ToContainerList(tempList);
                }
                else if (data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock) data;
                    c.ToContainerList(tempList);
                }
                else if (data is Container)
                {
                    Container c = (Container) data;
                    c.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show("В процедуру выгрузки контейнеров класса RowBlock передан неверный тип данных:" +
                                    data.GetType());
                }
            }
        }
    }
}