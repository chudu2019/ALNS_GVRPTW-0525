using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
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
    class Program
    {
        static void Main(string[] args)
        {
            //测试算法
                string path = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source =C:\\Users\\张平\\Desktop\\ALNS_GVRPTW-0525\\Solomon\\";
                string SolomonInstancesName = "results of Solomon example";
                new TestInstances().instances(path, SolomonInstancesName); 
                Console.ReadLine();
        }
    }
}
