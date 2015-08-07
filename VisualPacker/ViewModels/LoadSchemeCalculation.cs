using System.Collections.Generic;
using VisualPacker.Models;

namespace VisualPacker.ViewModels
{
    public class LoadSchemeCalculation
    {
        public List<int> DistinctOrdersInRow(List<RowBlock> rBlocks)
        {
            List<int> orderList = new List<int>();

            foreach (RowBlock r in rBlocks)
            {
                if (!orderList.Contains(r.Order))
                {
                    orderList.Add(r.Order);
                }
            }
            return orderList;
        }
    }
}