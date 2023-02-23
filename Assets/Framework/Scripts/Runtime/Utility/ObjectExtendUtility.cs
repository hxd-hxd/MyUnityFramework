// -------------------------
// 创建日期：2022/11/16 17:10:51
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public static class ObjectExtendUtility
    {
        /// <summary>
        /// 返回自身的克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T CloneSelf<T>(this T t)
        {
            T tOut = Activator.CreateInstance<T>();

            return CloneSelfToTarget(t, tOut);

            //var tInType = t.GetType();

            //foreach (var itemOut in tOut.GetType().GetProperties())
            //{
            //    var itemIn = tInType.GetProperty(itemOut.Name); ;
            //    if (itemIn != null)
            //    {
            //        itemOut.SetValue(tOut, itemIn.GetValue(t));
            //    }
            //}
            //return tOut;
        }

        /// <summary>
        /// 克隆目标到自身
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="tOut"></param>
        /// <param name="tIn"></param>
        /// <returns></returns>
        public static TOut CloneTargetToSelf<TIn, TOut>(this TOut tOut, TIn tIn)
        {
            return CloneSelfToTarget(tIn, tOut);

            //var tInType = tIn.GetType();
            //foreach (var itemOut in tOut.GetType().GetProperties())
            //{
            //    var itemIn = tInType.GetProperty(itemOut.Name); ;
            //    if (itemIn != null)
            //    {
            //        itemOut.SetValue(tOut, itemIn.GetValue(tIn));
            //    }
            //}
            //return tOut;
        }
        /// <summary>
        /// 克隆自身到目标
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="tOut"></param>
        /// <param name="tIn"></param>
        /// <returns></returns>
        public static TOut CloneSelfToTarget<TIn, TOut>(this TIn tIn, TOut tOut)
        {
            var tInType = tIn.GetType();

            // 克隆属性
            foreach (var itemOut in tOut.GetType().GetProperties())
            {
                var itemIn = tInType.GetProperty(itemOut.Name); ;
                if (itemIn != null)
                {
                    itemOut.SetValue(tOut, itemIn.GetValue(tIn));
                }
            }

            // 克隆字段
            foreach (var itemOut in tOut.GetType().GetFields())
            {
                var itemIn = tInType.GetField(itemOut.Name); ;
                if (itemIn != null)
                {
                    itemOut.SetValue(tOut, itemIn.GetValue(tIn));
                }
            }

            return tOut;
        }
    }
}