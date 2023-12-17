using System.Collections;
using System.Collections.Generic;

namespace Prob
{
    public static class PExpandList
    {
        /// <summary>
        /// 合并目标列表里的不重复元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="target"></param>
        public static void PMerge<T>(this List<T> value, List<T> target)
        {
            if (value == null || target == null) return;

            for (int i = 0; i < target.Count; i++)
            {
                if (!value.Contains(target[i]))
                {
                    value.Add(target[i]);
                }
            }
        }

        /// <summary>
        /// 返回一个新的对自身的拷贝列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<T> PCopySelf<T>(this List<T> value)
        {
            if (value == null) return null;
            List<T> copy = new List<T>();

            for (int i = 0; i < value.Count; i++)
            {
                copy.Add(value[i]);
            }

            return copy;
        }

        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="condition">排序条件</param>
        public static void PBubbleSort<T>(this List<T> value, System.Func<T, T, bool> condition)
        {
            if (value == null || condition == null) return;

            for (int i = 0; i < value.Count - 1; i++)
            {
                for (int j = 0; j < value.Count - i - 1; j++)
                {
                    if (condition(value[j], value[j + 1]))
                    {
                        T t = value[j];
                        value[j] = value[j + 1];
                        value[j + 1] = t;
                    }
                }
            }
        }
        /// <summary>
        /// 冒泡排序一次
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="condition">排序条件</param>
        public static void PBubbleOneSort<T>(this List<T> value, System.Func<T, T, bool> condition)
        {
            if (value == null || condition == null) return;

            for (int j = 0; j < value.Count - 1; j++)
            {
                if (condition(value[j], value[j + 1]))
                {
                    T t = value[j];
                    value[j] = value[j + 1];
                    value[j + 1] = t;
                }
            }
        }

        /// <summary>
        /// 添加时检查是否包含，已包含则不添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="t"></param>
        public static bool PAddTryContains<T>(this List<T> value, T t)
        {
            if (value.Contains(t)) return false;
            value.Add(t);

            return true;
        }
    }

    /// <summary>
    /// 扩展 Dictionary
    /// </summary>
    public static class PExpandDictionary
    {
        /// <summary>
        /// 获取值列表
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<V> PGetValueList<K, V>(this Dictionary<K, V> value)
        {
            List<V> vs = new List<V>();
            if (value == null) return vs;

            foreach (var item in value)
            {
                vs.Add(item.Value);
            }

            return vs;
        }

        /// <summary>
        /// 获取键列表
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<K> PGetKeyList<K, V>(this Dictionary<K, V> value)
        {
            List<K> ks = new List<K>();
            if (value == null) return ks;

            foreach (var item in value)
            {
                ks.Add(item.Key);
            }

            return ks;
        }
    }
}