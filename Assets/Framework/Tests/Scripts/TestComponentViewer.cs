// -------------------------
// 创建日期：2024/4/19 13:53:00
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using UObject = UnityEngine.Object;

namespace Framework.Test
{
    public class TestComponentViewer : MonoBehaviour
    {
        #region 字段

        /// 基元类型
        //public int _int;
        //public uint _uint;
        //public long _long;
        //public ulong _ulong;
        //public float _float;
        //public double _double;
        //public bool _bool;
        //[TextArea] public string _string = $"你好吗？\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n人类";
        //public char _char;
        //public TestType _testType;
        //public TestTypeFlags _testTypeFlags;

        /// unity 类型
        //[ColorUsageAttribute(false)]
        //public Color _color = new Color(1, 123 / 255f, 56 / 255f, 139 / 255f);
        //[ColorUsageAttribute(false)]
        //public Color32 _color32 = new Color(88 / 255f, 164 / 255f, 209 / 255f, 36 / 255f);

        //public Vector2 _Vector2;
        //public Vector2Int _Vector2Int;
        //public Vector3 _Vector3;
        //public Vector3Int _Vector3Int;
        //public Vector4 _Vector4;

        //public Bounds _Bounds;
        //public BoundsInt _BoundsInt;

        //Rect _Rect;
        //public Rect _Rect_public;
        //public RectInt _RectInt;

        ////[SerializeField]
        //AnimationCurve _animationCurve;
        //AnimationCurve _animationCurve1;
        //public AnimationCurve _animationCurve_public;

        //public Gradient _gradient;
        //Gradient _gradient1;

        //// 继承 unity 类型，会被当做普通类型处理
        //public GradientInherit _GradientInherit;
        //public AnimationCurveInherit _AnimationCurveInherit;

        //public UObject _uObject;
        //public Transform _Transform;
        //public GameObject _GameObject;
        //public TextAsset _TextAsset;
        //public ScriptableObject _ScriptableObject;

        //public Hash128 _Hash128;
        //public Scene _Scene;
        //public LayerMask _LayerMask = new LayerMask() { value = -1 };
        //public RectOffset _RectOffset;
        //public GUIStyle _GUIStyle;

        /* TODO：Matrix4x4：报错 Assertion failed on expression: 'ValidTRS()' */
        //public Matrix4x4 _matrix4X4;


        /// 暂不支持的 unity 类型
        //public UnityEvent _UnityEvent;
        //public UnityEvent<int> _UnityEventInt;
        //public UnityEvent<int, string> _UnityEventIntString;
        //public List<UnityEvent> _UnityEvents;

        // 用于 UnityEvent 事件绑定
        public void UnityEvent_func(bool b)
        {

        }

        /// 其他类型
        //public object _object;
        /// 声明类型和实例类型
        // 接口
        //public IList _objsIList = new List<int>() { 1, 2 };
        //public IList _objArrayIList = new int[] { 1, 2 };
        // 值类型
        //public ValueType _valueTypeInt = 1;
        //public ValueType _valueTypeVector3 = Vector3.one;
        //public ValueType _valueTypeHideFlags = HideFlags.None;
        // 枚举
        //public Enum _enumTestType = TestType.None;
        //public Enum _enumTestTypeFlags = TestTypeFlags.None;
        //public Enum _enumHideFlags = HideFlags.None;

        /// 基础容器 List、一维数组 unity 支持直接序列化
        //public List<int> _ints;
        //[TextArea] public List<string> _strings = new List<string>() { "你好，\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n世\r\n盖" };
        //public int[] _intArray;
        /// unity 不支持，但工具支持
        //int[] _intArray_private;
        //public List<object> _objects = new List<object>() { 1, new Info(), Vector3.zero, new List<object>() { "子列表\r\n第二行", Color.red, default(UObject) } };// 元素支持继承的类型
        //public List<List<int>> _intsList = new List<List<int>>() { new List<int>() { 1 }, new List<int>() { 2 } };
        //public List<int[]> _intsArray = new List<int[]>();
        //public Array _Array;
        //public Array _Array1 = new string[] { "你好，世盖" };
        //public List<Matrix4x4> _matrix4X4s;
        /// 以下复杂容器暂不支持
        //public Queue _intQueue;
        //public Queue _intQueue1 = new Queue();// 在构造函数中初始化
        //public Queue<int> _intQueueInt;
        //public Queue<int> _intQueueInt1;// 在构造函数中初始化
        //public Stack _intStack;
        //public Stack _intStack1;        // 在构造函数中初始化
        //public Stack<int> _intStackInt;
        //public Stack<int> _intStackInt1;// 在构造函数中初始化
        //public Dictionary<int, int> _dicIntInt;

        //public Hashtable _Hashtable;
        //public HashSet<int> _HashSetInt;

        /// 自定义 结构体
        //public InfoStruct _InfoStruct;
        //public InfoStructNonSerializable _InfoStructNonSerializable;
        //public AStruct _AStruct;
        //private AStruct _AStruct_private;
        //public List<AStruct> _AStructs;
        //private List<AStruct> _AStructs_private;

        /// 自定义 类
        //public Info _info;
        //Info _info_private;
        //public List<Info> _infos;
        //// 未添加 [Serializable]
        //public InfoNonSerializable _InfoNonSerializable;
        //public TypePool _typePool;

        // 索引器
        /* 
            报错：
                获取属性 Item 值时异常：System.Reflection.TargetParameterCountException: Number of parameters specified does not match the expected number.

            一个类可以有多个索引器，所有索引器都会有一个默认属性“Item”，由编译器自动创建。
            需要为索引器传索引参数，否则获取其值会导致以上错误。
        */
        //public Indexer _Indexer;

        //// 没有无参构造
        //public InfoNonCtor _infoNonCtor;

        //// 嵌套
        //public Dog _Dog;

        /*循环依赖
        unity 最多处理 10 层循环，算上自身 11 层
        */
        //[Header("单链 循环")]
        //public A _A;
        //private A _A_private;// 测试 创建实例
        //private A_NonSerializable _A_NonSerializable_private;// 测试 创建实例
        //public B _B;
        //[Header("长链 循环")]
        //public ALong _ALong;
        [Header("长链 列表循环")]
        public AListLong _AListLong;
        //[Header("自循环")]
        //public ANested _ANested;
        //[Header("继承式自循环")]
        //public BNested _BNested;
        //public BNestedInherit _BNestedInherit;

        //// 测试成员访问权限
        //public InfoMemberVisit _InfoMemberVisit;

        //// 空类
        //public InfoEmpty _InfoEmpty;

        //// 继承 Info
        //public InfoWorld _InfoWorld;

        //// 继承 List
        //public SubList _subList;
        //public SubList<int> _subListInt;

        //// 访问权限
        //public int _intP;

        //public readonly int _intR = 10;
        //public const int _intC = 10;

        #endregion

        #region 属性

        //public Scene locationScene => gameObject.scene;

        //public Color color { get => _color; }
        //private Color color_Private { get => _color; }
        //public Color32 color32 { get => _color32; set => _color32 = value; }
        //protected Color32 color32_Protected { get => _color32; set => _color32 = value; }

        //// 测试访问权限
        //public int intP_getOnly { get; }
        //public int intP_setOnly { set => _intP = value; }
        //protected int intP_setOnlyProtected { set => _intP = value; }
        //private int intP_setOnlyPrivate { set => _intP = value; }
        //// 以下属性的字段由编译器生成
        //public int intP { get; set; } = 10;
        //public int intP_setProtected { get; protected set; }
        //public int intP_setPrivate { get; private set; }
        //public int intP_setInternal { get; internal set; }
        //public int intP_setProtectedInternal { get; protected internal set; }
        //public int intP_setProtectedPrivate { get; protected private set; }

        #endregion

        public TestComponentViewer()
        {
            //_intQueue1 ??= new Queue();
            //_intQueue1.Enqueue(1);
            //_intQueue1.Enqueue(Color.red);

            //_intQueueInt1 ??= new Queue<int>();
            //_intQueueInt1.Enqueue(1);

            //_intStack1 ??= new Stack();
            //_intStack1.Push(1);

            //_intStackInt1 ??= new Stack<int>();
            //_intStackInt1.Push(1);
        }

        #region 类

        // 测试索引器类
        [Serializable]
        public class Indexer
        {
            public string this[long index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return "1";
                        case 1: return "2";
                        case 2: return "3";
                        default:
                            return "-1";
                    }
                }
            }

            public object this[long index1, string index2]
            {
                get
                {
                    return null;
                }
            }

            //public int Item => 0;
        }

        [Serializable]
        public struct AStruct
        {
            public string name;
            public int num;
            public BStruct _BStruct;
        }
        [Serializable]
        public struct BStruct
        {
            public string name;
            public int num;

            public string text => $"{name} ({num})";
        }

        [Serializable]
        public struct InfoStruct
        {
            public string name;
            public int num;
        }
        public struct InfoStructNonSerializable
        {
            public string name;
            public int num;
        }

        [Serializable]
        public class GradientInherit : Gradient
        {
            public string name;
        }
        [Serializable]
        public class AnimationCurveInherit : AnimationCurve
        {
            public string name;
        }

        #region 继承 List
        [Serializable]
        public class SubList : List<string>
        {
            public string name;
            public int num;
            //Array
        }
        [Serializable]
        public class SubList<T> : List<T>
        {
            public string name;
            public int num;
        }
        #endregion

        #region 循环继承，会导致编译错误
        //[Serializable]
        //public class A1 : A2
        //{

        //}
        //[Serializable]
        //public class A2 : A3
        //{

        //}
        //[Serializable]
        //public class A3 : A1
        //{

        //} 
        #endregion

        // 自循环
        [Serializable]
        public class ANested
        {
            public string name;
            public ANested a;
        }

        // 继承式自循环
        [Serializable]
        public class BNested
        {
            public string name;
            public BNestedInherit _BNestedInherit;// 父类中有子类
        }
        [Serializable]
        public class BNestedInherit : BNested
        {

        }

        #region 单链循环嵌套
        [Serializable]
        public class A
        {
            public string name;
            public List<int> list;
            public B b;
        }
        public class A_NonSerializable // 没有 序列化特性
        {
            public string name;
            public B b;
        }
        [Serializable]
        public class B
        {
            public string name;
            public A a;
        }
        #endregion

        #region 长链循环嵌套

        [Serializable]
        public class AListLong
        {
            public string name;
            public List<AListLong> list = new List<AListLong>() { /*new AListLong() */};
            public BLong b;
        }

        [Serializable]
        public class ALong
        {
            public string name;
            public List<int> list;
            public BLong b;
        }
        [Serializable]
        public class BLong
        {
            public string name;
            public CLong c;
        }
        [Serializable]
        public class CLong
        {
            public string name;
            public DLong d;
        }
        [Serializable]
        public class DLong
        {
            public string name;
            public ELong e;
        }
        [Serializable]
        public class ELong
        {
            public string name;
            public ALong a;
        }
        #endregion

        [Serializable]
        public class Dog
        {
            public string tag;
            public Info info;
        }
        [Serializable]
        public class Info
        {
            public string name;
            public int num;

            public override string ToString()
            {
                return $"name：{name}";
            }
        }

        // 没有无参构造
        [Serializable]
        public class InfoNonCtor
        {
            public string name;
            public int num;

            public InfoNonCtor(string name)
            {
                this.name = name;
            }
        }
        // 测试成员访问权限
        [Serializable]
        public class InfoMemberVisit
        {
            public int _public;
            private int _private;
            protected int _protected;
            internal int _internal;
            protected internal int _protected_internal;
            private protected int _private_protected;
        }
        [Serializable]
        private class InfoPrivate
        {
            public string name;
            public int num;
        }

        // 空
        [Serializable]
        public class InfoEmpty
        {

        }

        // 单继承
        [Serializable]
        public class InfoWorld : Info
        {
            public Vector3 pos;
        }

        public class InfoNonSerializable
        {
            public string name;
            public int num;
        }

        public enum TestType
        {
            None,
            A,
            B,
            C,
        }
        [Flags]
        public enum TestTypeFlags
        {
            None = 0,
            A = 1,
            B = 2,
            C = 4,
            D = 8,
            All = 15,
        }
        #endregion
    }
}