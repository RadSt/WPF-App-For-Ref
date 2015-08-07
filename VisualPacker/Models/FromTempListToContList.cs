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
            GetValue(blocks, tempList);
        }

        public List<Container> ToContainerList<T>(List<T> blocks)
        {
            var tempList = new List<Container>();
            GetValue(blocks, tempList);
            return tempList;
        }

        private void GetValue<T>(List<T> blocks, List<Container> tempList)
        {
            foreach (Object data in blocks)
            {
                if (data is VerticalBlock)
                {
                    var v = (VerticalBlock) data;
                    v.ToContainerList(tempList);
                }
                else if (data is RowBlock)
                {
                    var r = (RowBlock) data;
                    ToContainerList(tempList, r.Blocks);
                }
                else if (data is HorizontalBlock)
                {
                    var c = (HorizontalBlock) data;
                    c.ToContainerList(tempList);
                }
                else if (data is Container)
                {
                    var c = (Container) data;
                    c.ToContainerList(tempList);
                }
                else
                {
                    MessageBox.Show(
                        "В процедуру выгрузки контейнеров класса VerticalBlock передан неверный тип данных:" +
                        data.GetType());
                }
            }
        }
    }
}