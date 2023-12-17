using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prob
{
    /// <summary>
    /// 分布式概率生成器
    /// <para>
    /// 可以这么理解原理：ProbDeviceDistributed 生成器类似磁盘，ProbBranch 分支类似文件夹，ProbItem 类似各种文件
    /// </para>
    /// <para>就是随机在文件夹里找一个文件，根据你设定的 probValue 大小，其被随机到的概率也不同</para>
    /// <para>
    /// <see cref="IProb.ProbValue"/> 概率值可以是任何大于零的数，
    /// 但是推荐自己提前计算好值，然后填入，以免出现概率问题而难以追踪
    /// </para>
    /// </summary>
    public abstract class BaseProbDeviceDistributed<TInclude, TProbBranch, TProbItem> : 
        BaseContainer<TInclude, TProbBranch, TProbItem>
        where TProbBranch : BaseProbBranch<TInclude, TProbBranch, TProbItem>
        where TProbItem : BaseProbItem<TInclude, TProbBranch, TProbItem>
    {
        
    }
}
