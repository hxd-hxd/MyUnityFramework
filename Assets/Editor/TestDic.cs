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
using System.Linq;
using System.Reflection;
using UnityEngine.Rendering;

namespace Test
{
    public class TestDic
    {
        [MenuItem("Test/获取当前渲染管线")]
        public static void Test_currentPipeline()
        {
            Debug.Log(RenderPipelineManager.currentPipeline);
        }

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
            if (obj is Enum vEnum)
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

        [MenuItem("Test/类型继承子类型 Type.IsSubclassOf 判断")]
        static void TestTypeSubclass()
        {
            Debug.Log($"{nameof(ScriptableObject)} 是 {nameof(Object)} 的子类型：{typeof(ScriptableObject).IsSubclassOf(typeof(Object))}");
            Debug.Log($"{nameof(ScriptableObject)} 是 {nameof(Object)} 的子类型：{typeof(Object).IsSubclassOf(typeof(ScriptableObject))}");
            Debug.Log($"{nameof(ScriptableObject)} 是 {nameof(ScriptableObject)} 的子类型：{typeof(ScriptableObject).IsSubclassOf(typeof(ScriptableObject))}");
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

        [MenuItem("Test/UnityObject 转 object 的空值判断")]
        static void TestUnityObjectAsObjectNull()
        {
            Object uObj = new Object();
            //uObj = null;
            Object.DestroyImmediate(uObj);
            Debug.Log($"直接 判空：{uObj}");
            Debug.Log($"直接 判空：{uObj == null}");
            Debug.Log($"as 判空：{uObj as object}");
            Debug.Log($"as 判空：{uObj as object == null}");
            Debug.Log($"强转 判空：{(object)uObj}");
            Debug.Log($"强转 判空：{(object)uObj == null}");
        }

        [MenuItem("Test/列表和数组的索引器查看")]
        static void TestListAndArrayIndexer()
        {
            List<int> ints = new List<int>() { 1, 2, 3 };
            int[] intArray = new int[] { 1, 2, 3 };

            var bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var ints_fs = typeof(List<int>).GetFields(bf);
            var ints_ps = typeof(List<int>).GetProperties(bf);
            foreach (var i in ints_ps)
            {
                var _paraInfos = i.GetIndexParameters();
            }

            var intArray_fs = typeof(int[]).GetFields(bf);
            var intArray_ps = typeof(int[]).GetProperties(bf);
            foreach (var i in intArray_ps)
            {
                var _paraInfos = i.GetIndexParameters();
            }

            var qStr_fs = typeof(Queue<string>).GetFields(bf);
            var qStr_ps = typeof(Queue<string>).GetProperties(bf);
            foreach (var i in qStr_ps)
            {
                var _paraInfos = i.GetIndexParameters();
            }

            var str_fs = typeof(string).GetFields(bf);
            var str_ps = typeof(string).GetProperties(bf);
            foreach (var i in str_ps)
            {
                var _paraInfos = i.GetIndexParameters();
            }

        }

        class A { }
        class B : A { }
    }
}