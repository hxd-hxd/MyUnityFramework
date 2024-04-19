// -------------------------
// 创建日期：2024/4/19 13:53:00
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Framework.Test
{
    public class TestComponentViewer : MonoBehaviour
    {
        //public int _int;
        //public uint _uint;
        //public long _long;
        //public ulong _ulong;
        //public float _float;
        //public double _double;
        //public bool _bool;
        //public string _string;
        //public char _char;
        //public TestType _testType;
        //public TestTypeFlags _testTypeFlags;

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

        //[SerializeField]
        AnimationCurve _animationCurve;
        public AnimationCurve _animationCurve_public;

        public Gradient _gradient;

        public Transform _Transform;
        public GameObject _GameObject;
        public TextAsset _TextAsset;
        public ScriptableObject _ScriptableObject;

        public List<int> _ints;
        public List<List<int>> _intss;
        public int[] _intArray;
        public Queue<int> _intQueue;

        public UnityEvent _UnityEvent;
        public UnityEvent<int> _UnityEventInt;
        public UnityEvent<int,string> _UnityEventIntString;

        public Info _info;

        //public Color color { get => _color; }
        //private Color color_Private { get => _color; }
        //public Color32 color32 { get => _color32; set => _color32 = value; }
        //protected Color32 color32_Protected { get => _color32; set => _color32 = value; }


        [Serializable]
        public class Info
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
        }
    }
}