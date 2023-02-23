// -------------------------
// 创建日期：2022/11/7 16:24:44
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 处理一些通用的数学计算
    /// </summary>
    public static class MathfUtility
    {
        /// <summary>
        /// 判断向量大小是否在指定区域
        /// </summary>
        /// <param name="v"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool MagnitudeAmong(Vector3 v, float min, float max)
        {
            float magnitude = Mathf.Abs(v.magnitude);
            return magnitude >= min && magnitude < max;
        }
    }
}