using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TestUnityJson : MonoBehaviour
    {
        public int _int = 6;
        public int _int_json;

        public List<int> _list_int = new List<int>()
        {
            0, 1, 2, 3
        };
        public List<int> _list_int_json;
        public Dictionary<int, string> _dict_int_string = new Dictionary<int, string>()
        {
            { 0, "你" },
            { 1, "我" },
            { 2, "他" },
        };
        public Dictionary<int, string> _dict_int_string_json;

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

            var testJsonJson = JsonUtility.ToJson(_testJson);
            Debug.Log("自定义类直接转换");
            Debug.Log(testJsonJson);
            JsonUtility.FromJsonOverwrite(testJsonJson, _testJson_json);

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
    }
}