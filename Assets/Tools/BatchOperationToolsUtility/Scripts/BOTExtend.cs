using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;


namespace BatchOperationToolsUtility
{

    public static class BOTExtend
    {

        #region 扩展

        /// <summary>
        /// 获取完整目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPath(this string path)
        {
            return Path.GetFullPath(path);
        }
        /// <summary>
        /// 是否是目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectory(this string path)
        {
            return Directory.Exists(Path.GetFullPath(path));
        }
        /// <summary>
        /// 排除指定的文件类型
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string[] ExcludeType(this string[] paths, string fileType)
        {
            List<string> vs = new List<string>();
            for (int i = 0; i < paths.Length; i++)
            {
                string e = Path.GetExtension(paths[i]);
                if (e != fileType)
                {
                    vs.Add(paths[i]);
                }
            }

            return vs.ToArray();
        }
        /// <summary>
        /// 查找指定的文件类型
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static string[] IncludeType(this string[] paths, string fileType)
        {
            List<string> vs = new List<string>();
            for (int i = 0; i < paths.Length; i++)
            {
                string e = Path.GetExtension(paths[i]);
                if (e == fileType)
                {
                    vs.Add(paths[i]);
                }
            }

            return vs.ToArray();
        }
        /// <summary>
        /// 截取为 asset 资产路径格式
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CutOutAssetPath(this string path)
        {
            string[] strs = path.Split("Assets");
            return "Assets" + strs[strs.Length - 1];
        }

        /// <summary>
        /// 移除无效字符串
        /// <para>ps：" "、"\t"、"\n"、"\r"等</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveEmpty(this string value)
        {
            value = value.Remove("\t", "\n", "\r", " ");
            return value;
        }
        /// <summary>
        /// 移除字符串空格
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveSpace(this string value)
        {
            return value.Replace(" ", null);
        }
        /// <summary>
        /// 移除字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Remove(this string value, params string[] rs)
        {
            for (int i = 0; i < rs.Length; i++)
            {
                value = value.Replace(rs[i], null);
            }
            return value;
        }

        /// <summary>
        /// 按字符串拆分
        /// </summary>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] Split(this string value, string split)
        {
            string[] strs = Regex.Split(value, split);
            return strs;
        }

        /// <summary>
        /// 是否包含其中至少一个字符串
        /// </summary>
        /// <param name="contains"></param>
        /// <returns></returns>
        public static bool ContainsOne(this string value, params string[] contains)
        {
            for (int i = 0; i < contains.Length; i++)
            {
                if (value.Contains(contains[i]))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 在给定的字符列表中，查找包含的字符串
        /// </summary>
        /// <param name="contains"></param>
        /// <returns></returns>
        public static string[] ContainsArray(this string value, params string[] contains)
        {
            List<string> vs = new List<string>();
            for (int i = 0; i < contains.Length; i++)
            {
                if (value.Contains(contains[i]))
                {
                    vs.Add(contains[i]);
                }
            }
            return vs.ToArray();
        }
        /// <summary>
        /// 包含字符串的个数
        /// </summary>
        /// <param name="contains"></param>
        /// <returns></returns>
        public static int ContainsNum(this string value, params string[] contains)
        {
            int sum = 0;
            for (int i = 0; i < contains.Length; i++)
            {
                if (value.Contains(contains[i]))
                {
                    sum++;
                }
            }
            return sum;
        }
        /// <summary>
        /// 是否包含其中全部字符串
        /// </summary>
        /// <param name="contains"></param>
        /// <returns></returns>
        public static bool ContainsAll(this string value, params string[] contains)
        {
            for (int i = 0; i < contains.Length; i++)
            {
                if (!value.Contains(contains[i]))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 字符串拆分转换为二维嵌套的 float 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<List<float>> ToFloatTwoList(this string value, char parseSign1, char parseSign2)
        {
            return value.Split(parseSign1).ToFloatTwoList(parseSign2);
        }
        /// <summary>
        /// 字符串数组拆分转换为二维嵌套的 float 列表
        /// </summary>
        /// <param name="vs"></param>
        /// <param name="parseSign">拆分解析符</param>
        /// <returns></returns>
        public static List<List<float>> ToFloatTwoList(this string[] vs, char parseSign)
        {
            List<List<float>> valuess = new List<List<float>>();

            for (int i = 0; i < vs.Length; i++)
            {
                List<float> ints = vs[i].ToFloatList(parseSign);
                valuess.Add(ints);
            }

            return valuess;
        }
        /// <summary>
        /// 字符串拆分转换为 float 列表
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parseSign">拆分解析符</param>
        /// <returns></returns>
        public static List<float> ToFloatList(this string value, char parseSign)
        {
            return value.Split(parseSign).ToFloatList();
        }
        /// <summary>
        /// 字符串数组转为 float 列表
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public static List<float> ToFloatList(this string[] vs)
        {
            List<float> values = new List<float>();
            for (int i = 0; i < vs.Length; i++)
            {
                values.Add(vs[i].ToFloat());
            }
            return values;
        }
        /// <summary>
        /// 字符串数组转为 float 数组
        /// </summary>
        /// <param name="vs"></param>
        /// <returns></returns>
        public static float[] ToFloatArray(this string[] vs)
        {
            float[] values = new float[vs.Length];
            for (int i = 0; i < vs.Length; i++)
            {
                values[i] = vs[i].ToFloat();
            }
            return values;
        }

        /// <summary>
        /// 字符串转为 int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this string value)
        {
            value = value.RemoveEmpty();
            return value == "" ? 0 : int.Parse(value);
        }
        /// <summary>
        /// 字符串转为 float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToFloat(this string value)
        {
            value = value.RemoveEmpty();
            return value == "" ? 0 : float.Parse(value);
        }

        /// <summary>
        /// 打印所有列表元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        public static void LogAll<T>(this List<T> values)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                sb.Append(values[i].ToString()).Append("\t");
            }
            Debug.Log(sb.ToString());
        }

        #endregion

    }
}