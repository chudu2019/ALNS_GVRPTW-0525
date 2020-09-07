# ALNS_GVRPTW-0525
项目简介
=====
编辑器 VS2019
------------
语言 C#
------------
项目简介
--------
本文提供的改进型自适应大邻域搜索算法中，使用的算法有，贪婪构造启发式算法+自适应大邻域搜索算法+模拟退火的接受规则+基于蚁群算法信息素挥发机制的自适应更新规则+基于时间窗约束的插入优化规则。<br>
解决的问题是GVRPTW,VRP问题的变种，你可简单的理解为，求解使得车辆的总耗油量最小的路径规划方案。<br>

算法效果
-------
本算法在Solomon算例中表现优异。在100节点规模的GVRPTW中,本算法在10s内求得的解与分支定价求得的精确解进行比较，误差控制在5%以内。<br>
本算法稍作改动，亦可用作VRPTW问题的求解方案。本人水平有限，如有错误与不足，还望各位道友多多指正批评，本人的邮箱：1257524054@qq.com。觉得好的话，就给颗星吧，在此谢过了。<br>
