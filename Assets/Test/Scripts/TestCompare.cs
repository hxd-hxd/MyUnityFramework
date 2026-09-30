using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Test
{
    public class TestCompare : MonoBehaviour
    {
        void Start()
        {
            List<int> ints = new List<int>
            {
                1, 9, 574, 44, 89, 2, 66
            };

            var ints1 = new List<int>(ints);
            ints1.Sort((x, y) =>
            {
                if (x > y) return 1;
                else if (x < y) return -1;
                return 0;
            });

            var ints2 = new List<int>(ints);
            ints2.Sort((x, y) =>
            {
                return x.CompareTo(y);
            });

            Debug.Log($"比较 3 和 4：{3.CompareTo(4)}");
            Debug.Log($"自己比较：{Log(ints1)}");
            Debug.Log($"int 自带比较：{Log(ints2)}");
        }

        private string Log<T>(List<T> vs)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vs.Count; i++)
            {
                sb.Append(vs[i]);
                if (i < vs.Count - 1) sb.Append("，");
            }
            return sb.ToString();
        }
    }
}