using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Test
{
    public class TestUnityJson : MonoBehaviour
    {
        [Header("直接基元类型")]
        public TestSerialize<int> _int = new TestSerialize<int>(6, "数字直接");

        [Header("直接结构体类型")]
        public TestSerialize<Vector3> _v3 = new TestSerialize<Vector3>(Vector3.one, "结构体 Vector3 直接");

        public TestSerialize<Quaternion> _quaternion = new TestSerialize<Quaternion>(new Quaternion(1, 1, 1, 1), "结构体 Quaternion 直接");

        public TestSerialize<Color> _color = new TestSerialize<Color>(Color.red, "结构体 Color 直接");

        [Header("直接 Unity 对象类型")]
        public TestSerialize<Object> _uObj = new TestSerialize<Object>(null, "Unity 对象直接");

        [Header("直接列表")]
        public TestSerialize<List<int>> _list_int = new TestSerialize<List<int>>(new List<int>()
        {
            0, 1, 2, 3
        }, "列表直接");

        [Header("直接普通字典")]
        public TestSerialize<Dictionary<int, string>> _dict_int_string = new TestSerialize<Dictionary<int, string>>(new Dictionary<int, string>()
        {
            { 0, "你" },
            { 1, "我" },
            { 2, "他" },
        }, "普通字典直接");

        [Header("直接可序列化字典")]
        public TestSerialize<DictionarySerializable<int, string>> _dictSl_intString = new TestSerialize<DictionarySerializable<int, string>>(new DictionarySerializable<int, string>()
        {
            { 0, "你" },
            { 1, "我" },
            { 2, "他" },
        }, "可序列化字典直接");

        [Header("普通自定义类型")]
        public TestSerialize<TestJson> _testJson = new TestSerialize<TestJson>(new TestJson()
        {
            _list_int = new List<int>()
            {
                0, 1, 2, 3
            },
            _dict_int_string = new Dictionary<int, string>()
            {
                { 0, "你" },
                { 1, "我" },
                { 2, "他" },
            },
        }, "普通自定义类型");

        [Header("泛型自定义类型")]
        public TestSerialize<TestJson<int>> _testJson_int = new TestSerialize<TestJson<int>>(new TestJson<int>() { value = 6 }, "泛型自定义类型 int ");

        public TestSerialize<TestJson<Dictionary<int, int>>> _testJson_dictIntInt = new TestSerialize<TestJson<Dictionary<int, int>>>(new TestJson<Dictionary<int, int>>()
        {
            value = new Dictionary<int, int>() { { 1, 1 } }
        }, "泛型自定义类型 字典 ");

        public TestSerialize<TestJson<List<int>>> _testJson_listInt = new TestSerialize<TestJson<List<int>>>(new TestJson<List<int>>()
        {
            value = new List<int>()
            {
                1, 2, 3, 4
            }
        }, "泛型自定义类型 列表 ");

        public TestSerialize<TestJson<LogType>> _testJson_enum = new TestSerialize<TestJson<LogType>>(new TestJson<LogType>() { value = LogType.Assert }, "泛型自定义类型 枚举 ");

        public TestSerialize<TestJson<List<LogType>>> _testJson_listEnum = new TestSerialize<TestJson<List<LogType>>>(new TestJson<List<LogType>>()
        {
            value = new List<LogType>()
            {
                LogType.Log, LogType.Log
            }
        }, "泛型自定义类型 列表枚举 ");

        [Header("泛型自定义类型 Unity 对象")]
        public TestSerialize<TestJson<Object>> _testJson_uObj = new TestSerialize<TestJson<Object>>(new TestJson<Object>(), "泛型自定义类型 Unity 对象 ");

        [Header("泛型自定义类型 Unity 资产")]
        public TestSerialize<TestJson<Material>> _testJson_uAsset = new TestSerialize<TestJson<Material>>(new TestJson<Material>(), "泛型自定义类型 Unity 资产 ");

        void Start()
        {
            _int.Do();

            _v3.Do();

            _quaternion.Do();

            _color.Do();

            //// 这里会报错，引擎类型必须包装
            //// ArgumentException: JsonUtility.ToJson does not support engine types.
            //_uObj.Do();

            _list_int.Do();

            //// 这里会报错，覆写对象必须有实例
            //// 对于 Unity 可序列化的对象，Unity 会负责创建实例
            //// ArgumentNullException: Value cannot be null.
            //_dict_int_string.Do();

            _dictSl_intString.Do();

            _testJson.Do();

            _testJson_int.Do();

            _testJson_dictIntInt.Do();

            _testJson_listInt.Do();

            _testJson_enum.Do();

            _testJson_listEnum.Do();

            _testJson_uObj.Do();

            _testJson_uAsset.Do();

            // 结论
            // UnityJson 只支持数组、列表这些简单容器
            // 不支持容器类直接转换，必须包装起来
        }

        [Serializable]
        public class TestJson
        {
            public List<int> _list_int;
            public Dictionary<int, string> _dict_int_string;
        }

        [Serializable]
        public class TestJson<T>
        {
            public T value;
        }

        // 测试 json 序列化
        [Serializable]
        public class TestSerialize<T>
        {
            public T _obj;
            // 由 json 转换回来的对象
            public T _obj_json;
            public T _obj_jsonOverwriteObj;// 覆写对象必须有实例

            public string _tips;

            public TestSerialize(T obj, string tips)
            {
                _obj = obj;
                _tips = tips;
            }

            public void Do()
            {
                var _objJson = JsonUtility.ToJson(_obj);
                Debug.Log($"{_tips} 转换\r\n{_objJson}");
                _obj_json = JsonUtility.FromJson<T>(_objJson);
                JsonUtility.FromJsonOverwrite(_objJson, _obj_jsonOverwriteObj);
            }
        }
    }
}