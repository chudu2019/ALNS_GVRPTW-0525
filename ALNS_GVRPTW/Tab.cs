using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* 
  @Author：Created by 张平 from 东北大学
  @Email:1257524054@qq.com
  @Date：Created in 2020.0504
 */
namespace ALNS_GVRPTW
{
    class Tab//辅助计算类
    {
        private double valueChange;
        private List<int> index = new List<int>();//删除点的位置，第一位表示在solution，第二个代表route中的位置

        public double ValueChange { get => valueChange; set => valueChange = value; }
        public List<int> Index { get => index; set => index = value; }
    }
}
