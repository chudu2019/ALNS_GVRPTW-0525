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
    public class RequesetWithBlacklist//根据插入规则3，提出的黑名单机制
    {
        public int member;//待插入节点
        public List<int> blacklist = new List<int>();//待插入节点对应的不能插入的路径名单；
    }
}
