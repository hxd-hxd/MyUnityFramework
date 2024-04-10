// -------------------------
// 创建日期：2024/4/10 14:43:18
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.LogSystem;

namespace Framework.Test
{
    public class TestMinMax : MonoBehaviour
    {
        //[TextArea(3, 10)]
        public MinMax<string> _mmStr;
        public MinMax<bool> _mmBool;
        public MinMax<LogType> _mmEnum;
        [Range(0, 1)]
        public MinMax<float> _mmFloat;
        public MinMax<float> _mmFloat1;
        public MinMax<LogInfo> _mmLogInfo;
        public MinMax<Test> _mmTest;
        public MinMax<int> _mmInt;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        [Serializable]
        public struct Test
        {
            public string name; public int value;
            public LogType log;
        }
    }
}