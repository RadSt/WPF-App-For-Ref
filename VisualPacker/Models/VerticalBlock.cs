using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace VisualPacker.Models
{
   public class VerticalBlock: Container
    {
         public List<Container> blocks=new List<Container>();
         public double realVolume;
         public double maxPress = 10000;

     public VerticalBlock()
     {
         Count = 0;
         DirLength = "a";
     }
      public override object Clone() 
      {
          VerticalBlock verticalBlock = new VerticalBlock
          {
              Count = Count,
              Length = Length,
              Width = Width,
              Height = Height,
              Priority = Priority,
              Kind = Kind,
              Name = Name,
              ContainerType = ContainerType,
              Mass = Mass,
              Price = Price,
              DirLength = DirLength,
              DirWidth = DirWidth,
              DirHeight = DirHeight,
              Order = Order,
              Color = Color,
              Only4Bottom = Only4Bottom,
              FirstPoint = new Point3D(FirstPoint.X, FirstPoint.Y, FirstPoint.Z)
          };
          foreach (Object o in this.blocks)
          {
              if (o is VerticalBlock)
              {
                  VerticalBlock vBlock = (VerticalBlock)o;
                  verticalBlock.blocks.Add(vBlock.Clone() as VerticalBlock);
              }
              else if (o is RowBlock)
              {
                  RowBlock rBlock = (RowBlock)o;
                  MessageBox.Show("Ошибка. Клонирование рядов не выполнено");
              }
              else if (o is Container)
              {
                  Container cont = (Container)o;
                  verticalBlock.blocks.Add(cont.Clone() as Container);
              }
          }
          return verticalBlock;
      }
        public bool Add(Container c, int maxHeight)
        {
            if (
                (this.Height + c.Height) <= maxHeight &  (((blocks.Count > 0) & (c.Only4Bottom == 1))) == false & (maxPress>=c.Mass)) 
            {   if (c.DirLength != "a") //если направленность контейнера не подходит, то блок не добавляем
                 {
                     if (this.DirLength == "a") { this.DirLength = c.DirLength;}
                     else if (this.DirLength!=c.DirLength) {return false;} 
                 };
                if (this.Order != c.Order & Count != 0) { 
                    return false; 
                } //в одном блоке не может быть контейнеров из разных заказов
                blocks.Add(c);
                this.Height = this.Height + c.Height;
                this.Length = Math.Max(this.Length, c.Length);
                this.Width = Math.Max(this.Width, c.Width);
                this.Mass = this.Mass + c.Mass;
                this.Price = this.Price + c.Price;
                this.maxPress = Math.Min(this.maxPress-c.Mass, c.PressHeight);
                this.Count=this.Count+c.Count;
                this.Order = c.Order;
                this.realVolume = realVolume + c.Volume;
                this.Priority = Math.Min(this.Priority, c.Priority);
                return true;
            }
            else
            { 
                return false; 
            }

        }
       public List<Container> AddOneContainerFromList(List<Container> tempList,int maxHeight)
        {  
           int maxLength=this.blocks[this.blocks.Count()-1].Length;
           int maxWidth=this.blocks[this.blocks.Count()-1].Width;
           int oldCount=this.blocks.Count();
           List<Container> SameList = tempList.Where(c => c.Length <= maxLength & c.Width <= maxWidth & c.Order == this.Order).OrderBy(s => s.Priority).ThenByDescending(s => s.Length).ThenByDescending(s => s.Price).ToList();
           tempList=tempList.Where(c => c.Length>maxLength |c.Width>maxWidth |c.Order!=this.Order).ToList();
           foreach (Container c in SameList ){
               if (oldCount==this.blocks.Count())  {
                   if (this.Add(c, maxHeight) == false)
                   {
                       tempList.Add(c);}
       
               }
               else { tempList.Add(c); }
           }
           return tempList;
        }
        public bool AreSame(VerticalBlock v)
        {
           
            if (0.8*this.Width <= v.Width & 1.2*this.Width >= v.Width) return true;
            else return false; 

        }
        public bool AreSame01(VerticalBlock v)
        {

            if (0.9 * this.Width <= v.Width & 1.1 * this.Width >= v.Width) return true;
            else return false;

        }
        public bool AreEqual(VerticalBlock v)
        {

            if (this.Width == v.Width & this.Length == v.Length) return true;
            else return false;

        }
        public void SetFirstPoint(Point3D point)
        {
            this.FirstPoint = point;
            Point3D tempPoint = point;
            foreach (Object Data in blocks)
            {
                if (Data is VerticalBlock)
                {
                    VerticalBlock v = (VerticalBlock)Data;
                       v.SetFirstPoint(tempPoint);
                        tempPoint.Z = tempPoint.Z + v.Height;
                }
                else if (Data is RowBlock)
                {
                    RowBlock r = (RowBlock)Data;
                    r.SetFirstPoint(tempPoint);
                    tempPoint.Z = tempPoint.Z + r.Height;
                }
                else if (Data is HorizontalBlock)
                {
                    HorizontalBlock h = (HorizontalBlock)Data;
                    h.FirstPoint=tempPoint;
                    tempPoint.Z = tempPoint.Z + h.Height;
                }
                else if (Data is Container)
                {
                    Container c = (Container)Data;
                    c.FirstPoint = tempPoint;
                    tempPoint.Z = tempPoint.Z + c.Height;
                }
                else
                {
                    MessageBox.Show("В процедуру рисования SetFirstPoint класса VerticalBlock передан неверный тип данных:" + Data.GetType());
                }
            }
        }
        public new void RotateH()
        {
            int temp = this.Length;
            this.Length = this.Width;
            this.Width = temp;
            foreach (Object Data in blocks)
            {
                if (Data is VerticalBlock)
                {
                    VerticalBlock c = (VerticalBlock)Data;
                    c.RotateH();
                }
                else if (Data is RowBlock)
                {
                    RowBlock c = (RowBlock)Data;
                    // ничего не делаем
                }
                else if (Data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock)Data;
                    c.RotateH();
                }
                else if (Data is Container)
                {
                    Container c = (Container)Data;
                    c.RotateH();
                }
                else
                {
                    MessageBox.Show("В процедуру поворота вертикального блока передан неверный тип данных:" + Data.GetType());
                }
               
            
            }

        }

        new public void ToContainerList(List<Container> tempList)
        {
            foreach (Object Data in blocks)
            {
                if (Data is VerticalBlock)
                {
                    VerticalBlock v = (VerticalBlock)Data;
                    if (v.Kind == "VerticalPallet")
                    {
                        tempList.Add(v);
                    }
                    else
                    {
                        v.ToContainerList(tempList);
                    }
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
                    MessageBox.Show("В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" + Data.GetType());
                }
            }
        }
           public void  ToContainerListIncludeVerticalPallet(List<Container> tempList)
        {
           foreach (Object Data in blocks)
           {   
                if (Data is VerticalBlock)
                {   
                   VerticalBlock v=(VerticalBlock)Data;
                       v.ToContainerList(tempList);
                }
                else if (Data is RowBlock)
                {
                   RowBlock r =(RowBlock)Data;
                    r.ToContainerList(tempList); 
                }
                else if (Data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock)Data;
                    c.ToContainerList(tempList);
                }
                else if (Data is Container)
                {
                    Container c=(Container)Data;
                    c.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show("В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" + Data.GetType());
                }
           }
        }


    }
}
