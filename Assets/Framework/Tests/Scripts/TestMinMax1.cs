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
    public class TestMinMax1 : MonoBehaviour
    {
        [MinMaxRange(0, 1)]
        public Test1 _test1;
        [MinMaxRange(0, 1)]
        public Test _test;
        [MinMaxRange(0, 1)]
        public MinMax<Test> _mmTest;
        [MinMaxRange(0, 1)]
        public MinMax<string> _mmString;

        [Header("List MinMax<T>")]
        [MinMaxRange(0, 1)]
        public List<MinMax<float>> _mmListFloat;
        [MinMaxRange(0, 1)]
        public List<MinMax<string>> _mmListString;
        [MinMaxRange(0, 1)]
        public List<MinMax<Test>> _mmListTest;

        [Header("List")]
        [MinMaxRange(0, 1)]
        public List<float> _listFloat;
        [MinMaxRange(0, 1)]
        public List<string> _listString;
        [MinMaxRange(0, 1)]
        public List<Test> _listTest;
        [MinMaxRange(0, 1)]
        public List<Test1> _listTest1;

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
        [Serializable]
        public struct Test1
        {
            public string name;
        }
    }
}