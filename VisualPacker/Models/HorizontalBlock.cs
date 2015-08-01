using System;
using System.Collections.Generic;
using System.Windows;

namespace VisualPacker.Models
{
    class HorizontalBlock : Container
    {
        public List<Container> Blocks = new List<Container>();
        public int RowCount = 1;
        public double RealVolume = 0;
        public bool NotEmpty()
        {
            if (Count > 0) { 
                return true; }
            return false;
        }

        public HorizontalBlock()
        {
            Kind = "HorizontalBlock";
            Length = 0;
            Width = 0;
            Height = 0;
            Color = "Blue";
            Only4Bottom = 0;
            Count = 0;
            Name = "горизонтальный блок";
            Mass = 0;
            Price = 0;
            PressLength = 0; //Допустимое давление на яшик, если его длина ориентирована вертикально. 
            PressWidth = 0;
            PressHeight = 0;
            FragilityLength = 1;
            FragilityWidth = 1;
            FragilityHeight = 1;
            Freezable = 0;
            Level = 0;

        }
        public bool Add(Container c)
        {
            int CurrentWidth = 0;
            foreach (Container b in Blocks)
            { CurrentWidth = CurrentWidth + b.Width; }
            if (CurrentWidth + c.Width < Width * RowCount)
            {
                Blocks.Add(c);
                Height = Math.Max(Height, c.Height);
                Mass = Mass + c.Mass;
                Count = Count + c.Count;
                if (c.Height == Height)
                { PressHeight = PressHeight + c.PressHeight; }

                Order = c.Order;
                RealVolume = RealVolume + c.Volume;
                return true;
            }
            return false;
        }
        new public void ToContainerList(List<Container> tempList)
        {
            foreach (Object Data in Blocks)
            {
                if (Data is VerticalBlock)
                {
                    VerticalBlock v = (VerticalBlock)Data;
                    v.ToContainerList(tempList);
                }
                else if (Data is RowBlock)
                {
                    RowBlock r = (RowBlock)Data;
                    r.ToContainerList(tempList);
                }
                else if (Data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock)Data;
                    c.ToContainerList(tempList);
                }
                else if (Data is Container)
                {
                    Container c = (Container)Data;
                    c.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show("В процедуру выгрузки контейнеров класса HorizontalBlock  передан неверный тип данных:" + Data.GetType());
                }

            }
        }
    }
}
