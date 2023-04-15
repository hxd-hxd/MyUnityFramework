// -------------------------
// 创建日期：2023/4/14 17:20:00
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Test
{
    public class TestDic
    {
        [MenuItem("Test/字典用索引添加")]
        static void Dic()
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();

            dic[0] = 666;
            dic[5] = 2333;

            Debug.Log(dic.Count);
            Debug.Log(dic[0]);
            Debug.Log(dic[5]);
        }
    }
}