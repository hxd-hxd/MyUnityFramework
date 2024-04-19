// -------------------------
// 创建日期：2023/4/14 17:20:00
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using Object = UnityEngine.Object;

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

        [MenuItem("Test/枚举 Type 判断")]
        static void TestTypeEnum()
        {
            object obj = LogType.Log;
            Debug.Log(typeof(Enum).Name);
            Debug.Log(typeof(LogType).Name);
            Debug.Log(typeof(Enum) == typeof(LogType));
            Debug.Log(LogType.Log is Enum);
            if(obj is Enum vEnum)
            Debug.Log(vEnum);
        }
        [MenuItem("Test/类型转换关键字 is as 判断")]
        static void TestTypeCase()
        {
            GameObject obj = null;
            Debug.Log((obj is Object obj1));
            Debug.Log(obj as Object);
            Debug.Log((Object)obj);
            Debug.Log(1 as object);
            Debug.Log(null as Object);
        }

        [MenuItem("Test/类型继承 is as Type 判断")]
        static void TestTypeCaseAs()
        {
            A a = null;
            B b = null;

            a = new B();
            TestTypeCaseAs(a);
            Debug.Log("------------------------------------");
            a = new A();
            TestTypeCaseAs(a);
            Debug.Log("------------------------------------");
            TestTypeCaseAs(a = b);
            Debug.Log("------------------------------------");
            TestTypeCaseAs<object>(null);
        }
        static void TestTypeCaseAs<T>(T v)
        {
            Debug.Log(v is A);
            Debug.Log(v is B);
            Debug.Log(typeof(T));
            Debug.Log(v?.GetType());
        }

        class A { }
        class B : A { }
    }
}