using System.Collections;
using System.Collections.Generic;

namespace Prob
{
    /// <summary>
    /// 概率类实用函数集
    /// </summary>
    public static class ProbUtility
    {

        /// <summary>
        /// 随机一个概率
        /// </summary>
        /// <param name="probs"></param>
        /// <param name="ignoreEnable">忽视启用的影响（false 不忽视）</param>
        /// <returns></returns>
        public static T RandmoProb<T>(this List<T> probs, bool ignoreEnable = false) where T : IProb
        {
            if (probs == null) return default(T);

            List<T> cTypes = probs.GetEnableProb(ignoreEnable);
            SortProbs(cTypes);

            if (cTypes.Count <= 0) return default(T);

            // 计算概率
            float pvSumUp = 0;// 总概率
            for (int i = 0; i < cTypes.Count; i++)
            {
                pvSumUp += cTypes[i].ProbValue;
            }
            float randmoPV = RandmoFloat100();// 随机一个概率值
            for (int i = 0; i < cTypes.Count - 1; i++)
            {
                // 计算概率等级
                float pv = 0;
                for (int j = i + 1; j < cTypes.Count; j++)
                {
                    float formatPV = cTypes[j].ProbValue / pvSumUp * 100;// 格式化成百分比
                    pv += formatPV;
                }

                if (randmoPV >= pv) return cTypes[i];
            }

            return cTypes[cTypes.Count - 1];
        }
        /// <summary>
        /// 获取列表中可用的部分
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="probs"></param>
        /// <param name="ignoreEnable">忽视启用的影响（false 不忽视）</param>
        /// <returns></returns>
        public static List<T> GetEnableProb<T>(this List<T> probs, bool ignoreEnable = false) where T : IProb
        {
            List<T> cTypes = null;
            if (ignoreEnable)
            {
                cTypes = probs.FindAll((p) =>
                {
                    return p.ProbValue > 0;
                });
            }
            else
            {
                cTypes = probs.FindAll((p) =>
                {
                    return p.Enable && p.ProbValue > 0;
                });
            }

            return cTypes;
        }
        /// <summary>
        /// 获取此列表的总概率值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="probs"></param>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <param name="ignoreMinus">忽视负值的影响（如非特殊需要，请保持默认）</param>
        /// <returns></returns>
        public static float GetSumProbValue<T>(this List<T> probs, bool ignoreEnable = false, bool ignoreMinus = false) where T : IProb
        {
            float pv = 0;
            for (int i = 0; i < probs.Count; i++)
            {
                if (!ignoreEnable)
                {
                    if (!probs[i].Enable) continue;
                    else
                    {
                        if (!ignoreMinus && probs[i].ProbValue <= 0) continue;
                    }
                }
                else
                {
                    if (!ignoreMinus && probs[i].ProbValue <= 0)
                    {
                        continue;
                    }
                }

                pv += probs[i].ProbValue;
            }

            return pv;
        }

        /// <summary>
        /// 完整排序
        /// </summary>
        public static void SortProbs<T>(this List<T> probs) where T : IProb
        {
            probs.PBubbleSort((v1, v2) => { return v1.ProbValue > v2.ProbValue; });
        }
        /// <summary>
        /// 排序一次
        /// </summary>
        public static void SortOneProbs<T>(this List<T> probs) where T : IProb
        {
            probs.PBubbleOneSort((v1, v2) => { return v1.ProbValue > v2.ProbValue; });
        }

        /// <summary>
        /// 获取一个 大于0 小于等于100 的单精度浮点数
        /// </summary>
        /// <returns></returns>
        public static float RandmoFloat100()
        {
            return RandmoFloat(100 + float.Epsilon);
        }
        /// <summary>
         /// 获取一个 大于0 的单精度浮点数
         /// </summary>
         /// <returns></returns>
        public static float RandmoFloat(float max)
        {
            return UnityEngine.Random.Range(0 + float.Epsilon, max);
        }

        /// <summary>
        /// 将目标概率转换成百分比的形式
        /// <para>ps：不应为负数</para>
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float ProbFormat100(float sum, float target)
        {
            return target / sum * 100;
        }

        /// <summary>
        /// 转换成百分比的形式
        /// <para>ps：不应为负数</para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="cardinality">目标基数</param>
        /// <returns></returns>
        public static float FormatProb100(this float self, float cardinality)
        {
            return self / cardinality * 100;
        }
        /// <summary>
        /// 转换成归一化的形式（计算比率）
        /// <para>ps：不应为负数</para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="cardinality">目标基数</param>
        /// <returns></returns>
        public static float FormatProb1(this float self, float cardinality)
        {
            return self / cardinality;
        }
    }
}