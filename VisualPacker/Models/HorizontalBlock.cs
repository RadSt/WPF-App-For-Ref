using System;
using System.Collections.Generic;
using System.Windows;

namespace VisualPacker.Models
{
    class HorizontalBlock : Container
    {
        public List<Container> blocks = new List<Container>();
        public int rowCount = 1;
        public double realVolume = 0;
        public bool NotEmpty()
        {
            if (Count > 0) { 
                return true; }
            else { 
                return false; }
        }
        public HorizontalBlock()
        {
            this.Kind = "HorizontalBlock";
            this.Length = 0;
            this.Width = 0;
            this.Height = 0;
            this.Color = "Blue";
            this.Only4Bottom = 0;
            this.Count = 0;
            this.Name = "горизонтальный блок";
            this.Mass = 0;
            this.Price = 0;
            this.PressLength = 0; //Допустимое давление на яшик, если его длина ориентирована вертикально. 
            this.PressWidth = 0;
            this.PressHeight = 0;
            this.FragilityLength = 1;
            this.FragilityWidth = 1;
            this.FragilityHeight = 1;
            this.Freezable = 0;
            this.Level = 0;

        }
        public bool Add(Container c)
        {
            int CurrentWidth = 0;
            foreach (Container b in blocks)
            { CurrentWidth = CurrentWidth + b.Width; }
            if (CurrentWidth + c.Width < Width * rowCount)
            {
                blocks.Add(c);
                this.Height = Math.Max(this.Height, c.Height);
                this.Mass = this.Mass + c.Mass;
                Count = Count + c.Count;
                if (c.Height == this.Height)
                { this.PressHeight = this.PressHeight + c.PressHeight; }

                this.Order = c.Order;
                realVolume = realVolume + c.Volume;
                return true;
            }
            else
            { return false; }


        }
        new public void ToContainerList(List<Container> tempList)
        {
            foreach (Object Data in blocks)
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
