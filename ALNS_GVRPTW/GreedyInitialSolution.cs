using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* 
   @Author：张平-东北大学
   @Email:1257524054@qq.com
   @Date：Created in 2020.0504
  */
namespace ALNS_GVRPTW
{
    public class GreedyInitialSolution : TWJudgement 
    {
        public double A1;
        public double A2;
        public double Car_weight;
        public double MS;
        public GreedyInitialSolution(List<List<double>> _Cij, List<List<double>> _Tij, double capacity, double ServiceTime, List<Request> customerSet,double _a1, double _a2, double _Car_weight, double _ms) : base(_Cij, _Tij, capacity, ServiceTime, customerSet)
        {
            A1 = _a1;
            A2 = _a2;
            Car_weight = _Car_weight;
            MS = _ms;
        }
        public List<Route> GetSolution()//重点
        {
            List<Route> _solution = new List<Route>();
            List<int> requests = new List<int>();
            for (int i = 1; i < CustomerSet.Count; i++)
            {
                requests.Add(i);
            }
            int n = 0;
            while (requests.Count != 0)
            {
                Route Route = new Route();
                Route.IRoute.Add(0);//每个route从0开始
                Route.ID = n;
                Route.VisitTime.Add(0);
                Route.WaitTime.Add(0);
                Route.VisitTime.Add(VisitTime(0, requests[0]));
                Route.WaitTime.Add(WaitTime(0, requests[0]));
                Route.RouteCapacity = CustomerSet[requests[0]].cusneed;
                Route.RouteCost = (A1 + A2 * (Car_weight) / 1000) * Cij[0][requests[0]] * MS + (A1 + A2 * (Car_weight + CustomerSet[requests[0]].cusneed) / 1000) * Cij[0][requests[0]] * MS;
                Route.IRoute.Add(requests[0]);
                requests.Remove(requests[0]);
                while (true)
                {
                    List<Route> TwoRegret = new List<Route>();//用于储存每个待插入点的遗憾值
                    for (int t = 0; t < requests.Count; t++)
                    {
                        if (Route.RouteCapacity + CustomerSet[requests[t]].cusneed <= Capacity) //如果满足容量限制则继续进行，可以优化
                        {
                            List<Route> FeasibleRoute = new List<Route>();
                            bool stop = false;
                            double Time = 0;
                            double visittime = 0;
                            double waittime = 0;
                            int Case = 0;
                            int j;
                            for (j = 1; j < Route.IRoute.Count; j++)//route是从0开始的
                            {
                                Route route = new Route();
                                for (int i1 = 0; i1 < Route.IRoute.Count; i1++)
                                {
                                    route.IRoute.Add(Route.IRoute[i1]);
                                    route.VisitTime.Add(Route.VisitTime[i1]);
                                    route.WaitTime.Add(Route.WaitTime[i1]);
                                }
                                route.ID = t;
                                route.RouteCapacity = Route.RouteCapacity;
                                route.RouteCost = Route.RouteCost;
                                //为避免不必要的计算，先判断插入的合法性
                                double d = Cij[Route.IRoute[j - 1]][requests[t]];
                                bool f = JudgingFeasibilityOfInsertedNode(ref Time, ref visittime, ref waittime, ref stop, ref Case, requests[t], j - 1, j, d, route);
                                if (!f)//此位置插入违背时间窗，
                                {
                                    continue;
                                }
                                else if (stop)//之后route中不存在合法插入位置，立即停止
                                {
                                    break;
                                }
                                else
                                {
                                    InsertionRouteChange(requests[t], j, visittime, waittime, Time, Case, ref route);
                                    FeasibleRoute.Add(route);//此处有问题
                                }
                            }
                            if (!stop)//插入到末尾试试,
                            {
                                double Tv = Route.VisitTime[j - 1] + serviceTime +  Tij[Route.IRoute[j - 1]][requests[t]];
                                if (Tv <= CustomerSet[requests[t]].lastTime&& CustomerSet[requests[t]].earlyTime + serviceTime + Tij[requests[t]][0] <= CustomerSet[0].lastTime)//可以插入到末尾且满足车场时间窗
                                {
                                    Route route = new Route();
                                    for (int i1 = 0; i1 < Route.IRoute.Count; i1++)
                                    {
                                        route.IRoute.Add(Route.IRoute[i1]);
                                        route.VisitTime.Add(Route.VisitTime[i1]);
                                        route.WaitTime.Add(Route.WaitTime[i1]);
                                    }
                                    route.ID = t;
                                    route.RouteCapacity = Route.RouteCapacity;
                                    route.RouteCost = Route.RouteCost;
                                    if (Tv <= CustomerSet[requests[t]].earlyTime)//tb为上一个节点的发车时间,碳排放的改版
                                    {
                                            route.VisitTime.Add(CustomerSet[requests[t]].earlyTime);
                                            route.WaitTime.Add(CustomerSet[requests[t]].earlyTime - Tv);
                                            route.RouteCapacity += CustomerSet[requests[t]].cusneed;
                                            double L, l;
                                            int k = requests[t];
                                            L = route.RouteCapacity;
                                            l = CustomerSet[k].cusneed;
                                            double cost = (A1 + A2 * (Car_weight + L + l) / 1000) * Cij[k][0] * MS;
                                            double increase = cost - (A1 + A2 * (Car_weight + L) / 1000) * (Cij[route.IRoute.Last()][0] - Cij[route.IRoute.Last()][k]) * MS;
                                            route.Value = increase;
                                            route.RouteCost += increase;
                                            route.IRoute.Add(requests[t]);
                                            FeasibleRoute.Add(route);
                                    }
                                    else
                                    {
                                            route.VisitTime.Add(Tv);
                                            route.WaitTime.Add(0);
                                            route.RouteCapacity += CustomerSet[requests[t]].cusneed;
                                            double L, l;
                                            int k = requests[t];
                                            L = route.RouteCapacity;
                                            l = CustomerSet[k].cusneed;
                                            double cost = (A1 + A2 * (Car_weight + L + l) / 1000) * Cij[k][0] * MS;
                                            double increase = cost - (A1 + A2 * (Car_weight + L) / 1000) * (Cij[route.IRoute.Last()][0] - Cij[route.IRoute.Last()][k]) * MS;
                                            route.Value = increase;
                                            route.RouteCost += increase;
                                            route.IRoute.Add(requests[t]);
                                            FeasibleRoute.Add(route);
                                    }
                                }
                            }
                            if (FeasibleRoute.Count != 0)//求单个route中最佳的插入位置
                            {
                                double min = double.MaxValue;
                                int m = 0;
                                for (int i1 = 0; i1 < FeasibleRoute.Count; i1++)//记录requests[t]在Route.route中最好的位置
                                {
                                    if (FeasibleRoute[i1].Value < min)
                                    {
                                        m = i1;
                                    }
                                }
                                TwoRegret.Add(FeasibleRoute[m]);
                            }
                        }
                    }
                    if (TwoRegret.Count != 0)
                    {
                        double min = double.MaxValue;
                        int r = 0;
                        int r1 = 0;
                        for (int i = 0; i < TwoRegret.Count; i++)
                        {
                            if (TwoRegret[i].Value < min)
                            {
                                min = TwoRegret[i].Value;
                                r = i;
                                r1 = TwoRegret[i].ID ;
                            }
                        }
                        Route.IRoute.Clear();
                        Route.VisitTime.Clear();
                        Route.WaitTime.Clear();
                        for (int i = 0; i < TwoRegret[r].IRoute.Count; i++)
                        {
                            Route.IRoute.Add(TwoRegret[r].IRoute[i]);
                            Route.VisitTime.Add(TwoRegret[r].VisitTime[i]);
                            Route.WaitTime.Add(TwoRegret[r].WaitTime[i]);
                        }
                        Route.RouteCapacity = TwoRegret[r].RouteCapacity;
                        Route.RouteCost = TwoRegret[r].RouteCost;
                        //将此点从 requests中移除
                        requests.RemoveAt(r1);
                    }
                    else//无法继续插入
                    {
                        _solution.Add(Route);
                        n++;
                        break;
                    }
                }
            }
            return _solution;
        }
        public void InsertionRouteChange(int a, int c, double VisitTime, double waittime, double Time, double Case, ref Route queue)
        {
            double oldTw;
            double Loade = CustomerSet[a].cusneed;
            if (Case == 1)//插入点后一个节点的访问时间不变，所以之后的访问时间与等待时间都不变
            {
                oldTw = queue.WaitTime[c];
                queue.WaitTime[c] = oldTw - Time;//等待时间缩短了
                queue.VisitTime.Insert(c, VisitTime);
                queue.WaitTime.Insert(c, waittime);
                RouteAddCostChange(ref queue, c, a, Loade);
                queue.RouteCapacity += CustomerSet[a].cusneed;
            }
            else
            {
                for (int i = c; i < queue.IRoute.Count; i++)
                {
                    double tv = queue.VisitTime[i] - queue.WaitTime[i] + Time;
                    if (tv < CustomerSet[queue.IRoute[i]].earlyTime)//提前到,也就是之后的节点的VisitTime、WaitTime不变
                    {
                        oldTw = queue.WaitTime[i];
                        queue.WaitTime[i] = oldTw - Time;//等待时间减少了
                        break;
                    }
                    else
                    {
                        queue.VisitTime[i] = tv;
                        queue.WaitTime[i] = 0;
                    }
                }
                //再插入插入节点的访问时间与等待时间
                queue.VisitTime.Insert(c, VisitTime);
                queue.WaitTime.Insert(c, waittime);
                RouteAddCostChange(ref queue, c, a, Loade);
                queue.RouteCapacity += CustomerSet[a].cusneed;
            }
        }
        public void RouteAddCostChange(ref Route _route, int a, int b, double load)//可以优化
        {
            double Load = 0;
            for (int i = 0; i < a; i++)//a-1节点时的载重
            {
                Load += CustomerSet[_route.IRoute[i]].cusneed;
            }
            double cost = (A1 + A2 * (Car_weight + Load + load) / 1000) * Cij[b][_route.IRoute[a]] * MS;
            double increase = cost - (A1 + A2 * (Car_weight + Load) / 1000) * (Cij[_route.IRoute[a - 1]][_route.IRoute[a]] - Cij[_route.IRoute[a - 1]][b]) * MS;
            for (int i = a; i < _route.IRoute.Count - 1; i++)//计算新的目标值
            {
                increase += (A2 * load / 1000) * Cij[_route.IRoute[i]][_route.IRoute[i + 1]] * MS;
            }
            increase += (A2 * load / 1000) * Cij[_route.IRoute.Last()][0] * MS;
            _route.RouteCost += increase;
            _route.Value = increase;
            _route.IRoute.Insert(a, b);
        }
        public double VisitTime(int a, int b)
        {
            double VisitTime = 0;
            double T = 0;
            T = Tij[a][b];
            if (T <= CustomerSet[b].earlyTime)//tb为上一个节点的发车时间
            {
                VisitTime = CustomerSet[b].earlyTime;
            }
            else
            {
                VisitTime = T;
            }
            return VisitTime;
        }
        public double WaitTime(int a, int b)
        {
            double waittTime = 0;
            double T = 0;
            T = Tij[a][b];
            if (T <= CustomerSet[b].earlyTime)
            {
                waittTime = CustomerSet[b].earlyTime - T;
            }
            else
            {
                waittTime = 0;
            }
            return waittTime;
        }
    }
}
