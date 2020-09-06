using System;
using System.Collections;
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
    public class Route
    {
        private int iD;//路径编号
        private int idDepot;//车场号
        private List<int> route = new List<int>();//route的第一位为0
        private List<double> visitTime = new List<double>();//记录访问每个节点的时刻
        private List<double> waitTime = new List<double>();//每个节点的等待时间
        private double routeCost;//碳排放
        private double routeCapacity;//容量
        private double value;//用于记录插入后的routeCost的变动，同时也被用于储存regret值

        public int ID { get => iD; set => iD = value; }
        public int IdDepot { get => idDepot; set => idDepot = value; }
        public List<int> IRoute { get => route; set => route = value; }
        public List<double> VisitTime { get => visitTime; set => visitTime = value; }
        public List<double> WaitTime { get => waitTime; set => waitTime = value; }
        public double RouteCost { get => routeCost; set => routeCost = value; }
        public double RouteCapacity { get => routeCapacity; set => routeCapacity = value; }
        public double Value { get => value; set => this.value = value; }
    }
}
