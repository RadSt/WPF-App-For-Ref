using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace VisualPacker.Models
{
    public class FromTempListToContList
    {
        public void ToContainerList<T>(List<Container> tempList, List<T> blocks)
        {
            foreach (Object data in blocks)
            {
                if (data is VerticalBlock)
                {
                    VerticalBlock c = (VerticalBlock)data;
                    c.ToContainerList(tempList);
                }
                else if (data is RowBlock)
                {
                    RowBlock c = (RowBlock)data;
                    ToContainerList(tempList, c.Blocks);
                }
                else if (data is HorizontalBlock)
                {
                    HorizontalBlock c = (HorizontalBlock)data;
                    c.ToContainerList(tempList);
                }
                else if (data is Container)
                {
                    Container c = (Container)data;
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