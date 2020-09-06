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
    public class TWJudgement//插入规则，祖传代码！勿动！
    {
        public List<Request> CustomerSet = new List<Request>();
        public double Capacity;
        public double serviceTime;
        public List<List<double>>Cij = new List<List<double>>(); //距离矩阵
        public List<List<double>> Tij = new List<List<double>>();//时间矩阵
        public TWJudgement(List<List<double>> _Cij, List<List<double>> _Tij, double capacity, double ServiceTime, List<Request> customerSet)//构造函数
        {
            this.Capacity = capacity;
            this.CustomerSet = customerSet;
            this.serviceTime = ServiceTime;
            this.Cij = _Cij;
            this.Tij = _Tij;
        }
        //用于判断待插入节点的时间窗上的合法性，a为插入节点，c为插入点，b为插入点的前一位
        public bool JudgingFeasibilityOfInsertedNode(ref double Time, ref double visittime, ref double waittime, ref bool stop, ref int Case, int a, int b, int c, double d, Route route)//祖传代码！勿动！
        {
            bool f = true;
            double t, T;
            if (c == 1)//插入位置位于0点之后
            {
                t = route.VisitTime[b] + Tij[route.IRoute[b]][a]; //d为与前一个节点的距离
                T = Tij[a][route.IRoute[c]];
            }
            else
            {
                t = route.VisitTime[b] + serviceTime + Tij[route.IRoute[b]][a]; //d为与前一个节点的距离
                T = Tij[a][route.IRoute[c]];
            }
            if (t > CustomerSet[a].lastTime)//违反插入点时间窗，说明c点之后也无合法插入位置,立即停止试探
            {
                stop = true;
            }
            else//进一步考察其插入的合法性
            {
                if (t <= CustomerSet[a].earlyTime)//提前到
                {
                    visittime = CustomerSet[a].earlyTime;
                    waittime = CustomerSet[a].earlyTime - t;
                    f = FurtherJudgment(ref Time, ref Case, visittime, waittime, T, a, b, c, route);
                }
                else
                {
                    visittime = t;
                    waittime = 0;
                    f = FurtherJudgment(ref Time, ref Case, visittime, waittime, T, a, b, c, route);
                }
            }
            return f;
        }
        public bool FurtherJudgment(ref double Time, ref int Case, double VisitTime, double waittime, double T, int a, int b, int c, Route route)//祖传代码！勿动！
        {
            bool f = true;
            double tt = VisitTime + serviceTime + T;
            if (tt <= CustomerSet[route.IRoute[c]].lastTime)
            {
                if (tt <= CustomerSet[route.IRoute[c]].earlyTime)//延迟后，还是早到，说明后面的节点的状态不变，为可行解
                {
                    Case = 1;//
                    Time = tt - route.VisitTime[c] + route.WaitTime[c];//等价于新旧到达时间之差
                }
                else
                {
                    Case = 2;
                    bool STOP = true;
                    Time = tt - route.VisitTime[c] + route.WaitTime[c];//Time值为后移的节点新的到达时间减去原来的到达时间
                    double tv = 0;
                    int i;
                    for (i = c; i < route.IRoute.Count; i++)
                    {
                        tv = route.VisitTime[i] - route.WaitTime[i] + Time;//访问时间减等待时间等于实际达到时间
                        if (tv <= CustomerSet[route.IRoute[i]].earlyTime)//提前到，后面不用考察了，都是合法解
                        {
                            STOP = false;
                            break;
                        }
                        else if (tv > CustomerSet[route.IRoute[i]].lastTime)//不可行
                        {
                            f = false;
                            break;
                        }
                    }
                    if (STOP && f && tv + serviceTime + Tij[route.IRoute[i - 1]][0] > CustomerSet[0].lastTime)//回不到车场，也为非法解,如果最后一个节点可以回去，那么说明前面的都可以回去
                    {
                        f = false;
                    }
                }
            }
            else//不可省略
            {
                f = false;
            }
            return f;
        }
    }
}
