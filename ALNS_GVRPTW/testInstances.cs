using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
/* 
  @Author：Created by 张平 from 东北大学
  @Email:1257524054@qq.com
  @Date：Created in 2020.0504
*/
namespace ALNS_GVRPTW
{
    class TestInstances//运行算例，验证算法
    {
        double[] Depots = new double[2];
        private double ServiceTime=1;
        private double Speed= 1/42.0;
        private double MS =3.6 * 1/42.0;
        private double A1= 1.078088108;
        private double A2= 0.072253994;
        private double Capacity=70;
        private double Car_weight=920;
        private double Coef=2.0;//碳排放系数
        private int Length=101;
        private int Q=10;//删除节点数
        private int Iterations=20000;
        private static List<double> exactSolution = new List<double>() {141.34, 262.45, 427.86 };//分支定价算法在Solomon算例r101下，得出的问题规模为25,50,100的精确解
        private double e = exactSolution[2];
        public void instances(string path, string SolomonInstancesName)
        {
            #region  数据的录入
            string strconn =path + SolomonInstancesName + ".xlsx;Extended Properties='Excel 12.0;HDR=no;IMEX=1'";
            for (int f = 42; f < 43; f++) //开始测试算例-r101
            {
                OleDbConnection Conn = new OleDbConnection(strconn); //读取算例文件名
                Conn.Open();
                OleDbDataAdapter MyCommand1 = new OleDbDataAdapter("SELECT * FROM [DATA$]", strconn);
                DataTable TripData = new DataTable();
                try
                {
                    MyCommand1.Fill(TripData);
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception(ex.Message);
                }
                Conn.Close();
                string fileName = TripData.Rows[f][0].ToString();

                //读取顾客数据
                string strConn = path + fileName + ".xlsx;Extended Properties='Excel 12.0;HDR=no;IMEX=1'";
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                OleDbDataAdapter myCommand1 = new OleDbDataAdapter("SELECT * FROM [sheet1$]", strConn);
                DataTable tripData = new DataTable();
                try
                {
                    myCommand1.Fill(tripData);
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception(ex.Message);
                }
                conn.Close();

                List<Request> CustomerSet = new List<Request>();
                double EndTime =  Convert.ToDouble(tripData.Rows[0][4].ToString());
                Depots[0] = Convert.ToDouble(tripData.Rows[0][1].ToString());
                Depots[1] = Convert.ToDouble(tripData.Rows[0][2].ToString());
                for (int j = 0; j < Length; j++)
                {
                    int mj = j;
                    Request tempCust = new Request();
                    tempCust.ID = mj;
                    tempCust.xCoord = Convert.ToDouble(tripData.Rows[mj][1].ToString());
                    tempCust.yCoord = Convert.ToDouble(tripData.Rows[mj][2].ToString());
                    tempCust.earlyTime = Convert.ToDouble(tripData.Rows[mj][3].ToString()) * (24.0 / EndTime);
                    tempCust.lastTime = Convert.ToDouble(tripData.Rows[mj][4].ToString()) * (24.0 / EndTime);
                    tempCust.cusneed = Convert.ToInt16(tripData.Rows[mj][5].ToString());
                    CustomerSet.Add(tempCust);
                }
                List<List<double>>Cij = CustomerDistance(CustomerSet);//顾客点距离矩阵
                List<List<double>>Tij = TimeMatrix(CustomerSet, Cij);//顾客点行驶时间矩阵
                double max1 = 0;
                double min1 = double.MaxValue; 
                for (int i = 0; i < Cij.Count; i++)
                {
                    for (int j = 0; j < Cij[0].Count; j++)
                    {
                        if (max1 < Cij[i][j])
                        {
                            max1 = Cij[i][j];
                        }
                        if (min1> Cij[i][j])
                        {
                            min1 = Cij[i][j];
                        }
                    }
                }
                double max2 = 0;
                double min2 = double.MaxValue;
                for (int i = 0; i < CustomerSet.Count; i++)
                {
                    if (max2 < CustomerSet[i].cusneed)
                    {
                        max2 = CustomerSet[i].cusneed;
                    }
                    if (min2 > CustomerSet[i].cusneed)
                    {
                        min2 = CustomerSet[i].cusneed;
                    }
                }
                #endregion
                double testResult = 0;//计算平均值
                double totalTime = 0;//计算平均耗时
                int numberOfExperiments =10;//本算例实验次数
                Stopwatch time = new Stopwatch();
                time.Start(); //开始计时
                for (int t = 0; t < numberOfExperiments; t++)
                {
                    //算法开始
                    List<Route> solution = new GreedyInitialSolution(Cij, Tij, Capacity, ServiceTime, CustomerSet, A1, A2, Car_weight, MS).GetSolution(); //初始解
                    List<Route> BestSolution = new ALNS(Cij, Tij,Capacity, ServiceTime, CustomerSet, Coef,A1, A2, Car_weight, MS, min2, max2 - min2, min1, max1 - min1, Iterations).GetSolution(Q,solution);               
                    if (!CheckSolution(BestSolution, CustomerSet, Cij, Tij).Equals(true))//检查解正误，错误中断报警
                    {
                        BeepUp.Beep(500, 700); 
                        Console.ReadLine();
                    }
                    ////输出最终解
                    //    Console.WriteLine("最终解");
                    //for (int i = 0; i < BestSolution.Count; i++)
                    //{
                    //    Console.WriteLine();
                    //    Console.Write("第{0}条route成员为：", i);
                    //    for (int j = 0; j < BestSolution[i].route.Count; j++)
                    //    {
                    //        Console.Write(BestSolution[i].route[j] + "\t");
                    //    }
                    //    Console.WriteLine();
                    //}
                    //Console.WriteLine();
                    testResult += SolutionCost(BestSolution);
                }
                time.Stop();
                //输出本次实验的均值
                Console.WriteLine("解合法！{0}:共实验{1}次，平均耗时{2}s，平均解：{3},平均误差：{4}%", fileName, numberOfExperiments, Math.Round(time.ElapsedMilliseconds /1000.0 /numberOfExperiments, 2), Math.Round(testResult /numberOfExperiments, 2), Math.Round((Math.Round(testResult /numberOfExperiments, 2) - e) * 100 / e, 2));
        }
    }
        public static double SolutionCost(List<Route> queue)
        {
            double cost = 0;
            int i;
            for (i = 0; i < queue.Count; i++)
            {
                cost += queue[i].RouteCost;
            }
            return cost;
        }
        //检验求解的正确性
        public bool CheckSolution(List<Route> solution, List<Request> CustomerSet, List<List<double>> Cij, List<List<double>> Tij)//
        {
            bool suceess = true;
            int number = 0;
            for (int i = 0; i < solution.Count; i++)
            {
                number += (solution[i].IRoute.Count - 1);
            }
            for (int i = 0; i < solution.Count; i++)
            {
                solution[i].RouteCost = 0;
                solution[i].RouteCapacity = 0;
                solution[i].VisitTime.RemoveRange(1, solution[i].IRoute.Count - 1);
                double td;//行驶时间
                int j;
                for (j = 1; j < solution[i].IRoute.Count; j++)
                {
                    double Ta;
                    solution[i].RouteCost += (A1 + A2 * (Car_weight + solution[i].RouteCapacity) / 1000) * Cij[solution[i].IRoute[j - 1]][solution[i].IRoute[j]] * MS;//碳排放
                    if (j == 1)
                    {
                        Ta = Tij[0][solution[i].IRoute[j]];//车场没有服务时间
                    }
                    else
                    {
                        Ta = solution[i].VisitTime[j - 1] + ServiceTime + Tij[solution[i].IRoute[j - 1]][solution[i].IRoute[j]];
                    }
                    double Tmin = CustomerSet[solution[i].IRoute[j]].earlyTime;
                    double Tmax = CustomerSet[solution[i].IRoute[j]].lastTime;
                    if (Ta <= Tmax)
                    {
                        if (Ta <= Tmin)
                        {
                            solution[i].VisitTime.Add(Tmin);
                        }
                        else
                        {
                            solution[i].VisitTime.Add(Ta);
                        }
                        solution[i].RouteCapacity += CustomerSet[solution[i].IRoute[j]].cusneed;
                    }
                    else
                    {
                        suceess = false;
                        Console.WriteLine("第{0}route第{1}节点违反时间窗约束！", i, j);
                        break;
                    }
                }
                if (suceess && solution[i].VisitTime[j - 1] + ServiceTime + Tij[solution[i].IRoute[j - 1]][0] > CustomerSet[0].lastTime)
                {
                    suceess = false;
                    Console.WriteLine("第{0}route的{1}节点违法车场时间窗约束！,超出：{2}", i, solution[i].IRoute[j - 1], CustomerSet[0].lastTime - solution[i].VisitTime[j - 1] - ServiceTime - Tij[solution[i].IRoute[j - 1]][0]);
                }
                if (solution[i].RouteCapacity > Capacity)
                {
                    suceess = false;
                    Console.WriteLine("第{0}route违法载重约束！", i);
                }
                else
                    solution[i].RouteCost += (A1 + A2 * (Car_weight + solution[i].RouteCapacity) / 1000) * Cij[solution[i].IRoute[j - 1]][0] * MS;//碳排放
            }
            double c = 0;
            int L = Length - 1;
            if (number != L)
            {
                List<int> node = new List<int>();
                for (int i = 0; i < L; i++)
                {
                    node.Add(i + 1);
                }
                suceess = false;
                Console.WriteLine("节点数目错误！为：{0}", number);
                for (int t = 0; t < node.Count; t++)
                {
                    bool stop = false;
                    int a = 0;
                    int count = 0;
                    for (int i = 0; i < solution.Count; i++)
                    {

                        for (int j = 1; j < solution[i].IRoute.Count; j++)
                        {
                            if (solution[i].IRoute[j] == node[t])
                            {
                                a = i;
                                count++;
                            }
                        }
                    }
                    if (count < 1)
                    {
                        suceess = false;
                        Console.WriteLine("节点{0}缺少", node[t]);
                    }
                    else if (count > 1)
                    {
                        Console.WriteLine("节点{0}重复,位置在{1}", node[t], a);
                    }
                }
            }
            else if (suceess)
            {
                Console.Write("可行解，其碳排放为：");
                for (int i = 0; i < solution.Count; i++)
                {
                    c += solution[i].RouteCost;
                }
                Console.WriteLine(c);
            }
            return suceess;
        }
        public List<List<double>> CustomerDistance(List<Request> CustomerSet)
        {
            List<List<double>> Matrix = new List<List<double>>();
          
                for (int i = 0; i < CustomerSet.Count; i++)
                {
                    Matrix.Add(new List<double>());
                }
                CustomerSet[0].xCoord = Depots[0];
                CustomerSet[0].yCoord = Depots[1];
                double distance;
                int len = CustomerSet.Count;
                for (int i = 0; i < len; i++)
                {
                    for (int j = 0; j < len; j++)
                    {
                        distance = Math.Sqrt(Math.Pow(CustomerSet[i].xCoord - CustomerSet[j].xCoord, 2) + Math.Pow(CustomerSet[i].yCoord - CustomerSet[j].yCoord, 2)) * Coef; 
                        Matrix[i].Add(Math.Round(distance, 4));
                    }
                }

            return Matrix;
        }
        public List<List<double>>TimeMatrix(List<Request> CustomerSet, List<List<double>>Cij)
        {
            double time;
            List<List<double>>Matrix = new List<List<double>>();
                for (int i = 0; i < CustomerSet.Count; i++)
                {
                    Matrix.Add(new List<double>());
                }
                int len = CustomerSet.Count;
                for (int i = 0; i < len; i++)
                {
                    for (int j = 0; j < len; j++)
                    {
                        time = Cij[i][j] * Speed;
                        Matrix[i].Add(Math.Round(time, 4));
                    }
                }
            return Matrix;
        }
        // 声明
        public class BeepUp
        {
            /// <param name="iFrequency">声音频率（从37Hz到32767Hz）。在windows95中忽略</param>
            /// <param name="iDuration">声音的持续时间，以毫秒为单位。</param>
            [DllImport("Kernel32.dll")] //引入命名空间 using System.Runtime.InteropServices;
            public static extern bool Beep(int frequency, int duration);
        }
       
    }
}

