// -------------------------
// 创建日期：2024/4/10 14:43:18
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.LogSystem;
using UnityEngine.Rendering;

namespace Framework.Test
{
    public class TestMinMax : MonoBehaviour
    {
        [MinMaxRange(0, 1)]
        public Color _color = Color.white;
        [MinMaxRange(0, 1)]
        public MinMax<Color> _mmColor;
        [MinMaxRange(0, 1)]
        public Vector2 _vector2;
        [MinMaxRange(0, 1)]
        public MinMax<Vector2> _mmVector2;
        [MinMaxRange(0, 1)]
        public Vector3 _vector3;
        [MinMaxRange(0, 1)]
        public MinMax<Vector3> _mmVector3;
        [MinMaxRange(0, 1)]
        public Vector4 _vector4;
        [MinMaxRange(0, 1)]
        public MinMax<Vector4> _mmVector4;

        [Header("MinMaxRangeAttribute 应用于 其他类型")]
        [MinMaxRange(0, 1)]
        public float _float;
        [MinMaxRange(0, 1)]
        public LogType _logType;
        [MinMaxRange(0, 1)]
        public LogInfo _logInfo;
        [MinMaxRange(0, 1)]
        public Test _test;
        [MinMaxRange(0, 1)]
        public string _string;

        [Header("MinMaxRangeAttribute 应用于 MinMax<T>")]
        //[TextArea(3, 10)]
        [MinMaxRange(0, 1)]
        public MinMax<string> _mmString;
        [MinMaxRange(0, 1)]
        public MinMax<bool> _mmBool;
        [MinMaxRange(0, 1)]
        public MinMax<LogType> _mmEnum;
        [MinMaxRange(0, 1)]
        public MinMax<uint> _mmUint;
        [MinMaxRange(0, 1)]
        public MinMax<long> _mmLong;
        [MinMaxRange(0, 1)]
        public MinMax<ulong> _mmUlong;
        [MinMaxRange(0, 1)]
        public MinMax<double> _mmDouble;
        [MinMaxRange(0, 1)]
        public MinMax<float> _mmFloat;
        public MinMax<float> _mmFloat1;
        [MinMaxRange(0, 1)]
        public MinMax<Test> _mmTest;
        [MinMaxRange(0, 1)]
        public MinMax<LogInfo> _mmLogInfo;
        public MinMax<int> _mmInt;

        // Start is called before the first frame update
        void Start()
        {
            //SplashScreen
        }

        [Serializable]
        public struct Test
        {
            public string name; public int value;
            public LogType log;
        }
    }
}