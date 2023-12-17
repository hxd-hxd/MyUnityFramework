using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prob
{
    /// <summary>
    /// 概率生成器
    /// <para>
    /// 可以这么理解原理：ProbDeviceDistributed 生成器类似磁盘，ProbBranch 分支类似文件夹，ProbItem 类似各种文件
    /// </para>
    /// <para>就是随机在文件夹里找一个文件，根据你设定的 probValue 大小，其被随机到的概率也不同</para>
    /// <para>
    /// <see cref="IProb.ProbValue"/> 概率值可以是任何大于零的数，
    /// 但是推荐自己提前计算好值，然后填入，以免出现概率问题而难以追踪
    /// </para>
    /// </summary>
    [System.Serializable]
    public class ProbDeviceDistributed : BaseProbDeviceDistributed<string, ProbBranch, ProbItem>
    {
        static ProbDeviceDistributed<bool> pdd = new ProbDeviceDistributed<bool>();

        static ProbDeviceDistributed()
        {
            pdd.AddItem(1, true);
            pdd.AddItem(1, false);
        }

        /// <summary>
        /// 随机 <see cref="bool"/>，true 和 false 概率一样
        /// </summary>
        /// <returns></returns>
        public static bool RandomBool()
        {
            return Random.Range(0, 2) == 1;
        }
        /// <summary>
        /// 随机 <see cref="bool"/>
        /// </summary>
        /// <param name="truePV">true 的概率值</param>
        /// <param name="falsePV">false 的概率值</param>
        /// <returns></returns>
        public static bool RandomBool(float truePV, float falsePV)
        {
            pdd.FindProbItem(true).ProbValue = truePV;
            pdd.FindProbItem(false).ProbValue = falsePV;
            return pdd.GetRandmoDirectlyProbItem().TValue;
        }
    }

    /// <summary>
    /// 概率生成器
    /// <para>
    /// 可以这么理解原理：ProbDeviceDistributed 生成器类似磁盘，ProbBranch 分支类似文件夹，ProbItem 类似各种文件
    /// </para>
    /// <para>就是随机在文件夹里找一个文件，根据你设定的 probValue 大小，其被随机到的概率也不同</para>
    /// <para>
    /// <see cref="IProb.ProbValue"/> 概率值可以是任何大于零的数，
    /// 但是推荐自己提前计算好值，然后填入，以免出现概率问题而难以追踪
    /// </para>
    /// </summary>
    [System.Serializable]
    public class ProbDeviceDistributed<T> : BaseProbDeviceDistributed<T, ProbBranch<T>, ProbItem<T>>
    {


    }
}
