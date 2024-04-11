using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 功能类似 <see cref="RangeAttribute"/>，支持设定 <see cref="MinMax{T}"/> T 为 <see cref="float"/> 、<see cref="int"/> 等数值类型的大小范围
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;

        // Attribute used to make a float or int variable in a script be restricted to a specific range.
        public MinMaxRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}