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
        public int _int = 6;
        public int _int_json;

        [Header("直接列表")]
        public List<int> _list_int = new List<int>()
        {
            0, 1, 2, 3
        };
        public List<int> _list_int_json;

        [Header("普通字典")]
        public Dictionary<int, string> _dict_int_string = new Dictionary<int, string>()
        {
            { 0, "你" },
            { 1, "我" },
            { 2, "他" },
        };
        public Dictionary<int, string> _dict_int_string_json;

        [Header("可序列化字典")]
        public DictionarySerializable<int, string> _dictSl_intString = new DictionarySerializable<int, string>()
        {
            { 0, "你" },
            { 1, "我" },
            { 2, "他" },
        };
        public DictionarySerializable<int, string> _dictSl_intString_json;

        [Header("普通自定义类型")]
        public TestJson _testJson = new TestJson()
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
        };
        public TestJson _testJson_json;

        [Header("泛型自定义类型")]
        public TestJson<int> _testJson_int = new TestJson<int>() { value = 6 };
        public TestJson<int> _testJson_int_json;

        public TestJson<List<int>> _testJson_listInt = new TestJson<List<int>>()
        {
            value = new List<int>()
            {
                1, 2, 3, 4
            }
        };
        public TestJson<List<int>> _testJson_listInt_json;

        public TestJson<LogType> _testJson_enum = new TestJson<LogType>() { value = LogType.Assert };
        public TestJson<LogType> _testJson_enum_json;

        public TestJson<List<LogType>> _testJson_listEnum = new TestJson<List<LogType>>()
        {
            value = new List<LogType>()
            {
                LogType.Log, LogType.Log
            }
        };
        public TestJson<List<LogType>> _testJson_listEnum_json;

        [Header("泛型自定义类型 Unity 对象引用")]
        public TestJson<Object> _testJson_GO = new TestJson<Object>();
        public TestJson<Object> _testJson_GO_json;

        void Start()
        {
            var intJson = JsonUtility.ToJson(_int);
            Debug.Log("数字直接转换");
            Debug.Log(intJson);
            JsonUtility.FromJsonOverwrite(intJson, _int_json);

            var listIntJson = JsonUtility.ToJson(_list_int);
            Debug.Log("列表直接转换");
            Debug.Log(listIntJson);
            JsonUtility.FromJsonOverwrite(listIntJson, _list_int_json);

            var dictIntStringJson = JsonUtility.ToJson(_dict_int_string);
            Debug.Log("字典直接转换");
            Debug.Log(listIntJson);
            _dict_int_string_json = new Dictionary<int, string>();
            JsonUtility.FromJsonOverwrite(dictIntStringJson, _dict_int_string_json);

            var _dictSl_intStringJson = JsonUtility.ToJson(_dictSl_intString);
            Debug.Log("可序列化字典转换");
            Debug.Log(_dictSl_intStringJson);
            _dict_int_string_json = new Dictionary<int, string>();
            JsonUtility.FromJsonOverwrite(_dictSl_intStringJson, _dictSl_intString_json);

            var testJsonJson = JsonUtility.ToJson(_testJson);
            Debug.Log("自定义类直接转换");
            Debug.Log(testJsonJson);
            JsonUtility.FromJsonOverwrite(testJsonJson, _testJson_json);

            var testJson_intJson = JsonUtility.ToJson(_testJson_int);
            Debug.Log("自定义泛型类 int 直接转换");
            Debug.Log(testJson_intJson);
            JsonUtility.FromJsonOverwrite(testJson_intJson, _testJson_int_json);

            var _testJson_listIntJson = JsonUtility.ToJson(_testJson_listInt);
            Debug.Log("自定义泛型类 List<int> 直接转换");
            Debug.Log(_testJson_listIntJson);
            JsonUtility.FromJsonOverwrite(_testJson_listIntJson, _testJson_listInt_json);

            var testJson_enumJson = JsonUtility.ToJson(_testJson_enum);
            Debug.Log("自定义泛型类 enum 直接转换");
            Debug.Log(testJson_enumJson);
            JsonUtility.FromJsonOverwrite(testJson_enumJson, _testJson_enum_json);

            var _testJson_listEnumJson = JsonUtility.ToJson(_testJson_listEnum);
            Debug.Log("自定义泛型类 List<enum> 直接转换");
            Debug.Log(_testJson_listEnumJson);
            JsonUtility.FromJsonOverwrite(_testJson_listEnumJson, _testJson_listEnum_json);

            var _testJson_GOJson = JsonUtility.ToJson(_testJson_GO);
            Debug.Log("自定义泛型类 UnityEngine.Object 直接转换");
            Debug.Log(_testJson_GOJson);
            JsonUtility.FromJsonOverwrite(_testJson_GOJson, _testJson_GO_json);

            // 结论
            // UnityJson 不支持字典
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
    }
}