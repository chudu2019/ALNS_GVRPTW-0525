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
    public class Request
    {
        public int ID { get; set; }
        public double xCoord { get; set; }
        public double yCoord { get; set; }
        public double earlyTime { get; set; }
        public double lastTime { get; set; }
        public int cusneed { get; set; }

    }
}
