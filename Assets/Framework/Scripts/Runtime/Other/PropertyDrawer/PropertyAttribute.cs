using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// �������� <see cref="RangeAttribute"/>��֧���趨 <see cref="MinMax{T}"/> T Ϊ <see cref="float"/> ��<see cref="int"/> ����ֵ���͵Ĵ�С��Χ
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