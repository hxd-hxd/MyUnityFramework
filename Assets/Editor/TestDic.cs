// -------------------------
// 创建日期：2023/4/14 17:20:00
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text;
using System.Reflection;
using UnityEditor;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Test
{

    class TestMethodInfo
    {
        public int this[string index]
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public string N { get; set; }

        //[Return]
        public /*[Def("")]*/int P()
        {

            return 0;
        }

        static void P1(int i = 666)
        {

        }

        static void P2([Def("233")] int i, string n)
        {

        }

        static void POutRef([Def("2548")] out int i, ref string n)
        {
            i = 0;
        }

        static void PParas([Def("577")] params object[] vs)
        {

        }

        [System.AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = true)]
        sealed class DefAttribute : Attribute
        {
            // See the attribute guidelines at 
            //  http://go.microsoft.com/fwlink/?LinkId=85236
            readonly string v;

            // This is a positional argument
            public DefAttribute(string v)
            {
                this.v = v;
            }

            public string Value
            {
                get { return v; }
            }

            // This is a named argument
            public int NamedInt { get; set; }
        }

        [System.AttributeUsage(AttributeTargets.ReturnValue, Inherited = false, AllowMultiple = true)]
        sealed class ReturnAttribute : Attribute
        {

        }
    }

    public class TestDic
    {
        [MenuItem("Test/反射 获取方法参数")]
        public static void Test_MethodInfo_Parameters()
        {
            var ms = typeof(TestMethodInfo).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            foreach (var m in ms)
            {
                StringBuilder sb = new StringBuilder();
                var paras = m.GetParameters();
                sb.Append($"参数个数：{paras.Length} ，{m.Name}\t（");
                int i = 0;
                foreach (var param in paras)
                {
                    ++i;
                    // out、ref 标记，其中 ref 未直接提供 api
                    if (param.IsOut) sb.Append("out ");
                    else if (param.ParameterType.IsByRef) sb.Append("ref ");
                    // 无法判断 params，可能需要用到 System.Reflection.Metadata 

                    // 类型、名字
                    sb.Append($"{param.ParameterType.Name} {param.Name}");
                    // 默认值
                    if (param.HasDefaultValue) sb.Append($" = {param.DefaultValue}");
                    // 参数分隔符
                    if (i < paras.Length) sb.Append(", ");
                }
                sb.Append("）");
                Debug.Log(sb);
            }
        }

        [MenuItem("Test/可变参方法")]
        public static void Test_DuoCanShu()
        {
            object[] vs1 = { 1, "a" };
            object[] vs2 = { 2 };
            DuoCanShu(vs1);
            DuoCanShu(vs1, vs2);

        }
        static void DuoCanShu(params object[] ps)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("参数个数：").Append(ps.Length);
            sb.AppendLine().Append("参数：");
            for (int i = 0; i < ps.Length; i++)
            {
                sb.AppendLine().Append(i).Append("：\t").Append(ps[i]);
            }
            Debug.Log(sb);
        }

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