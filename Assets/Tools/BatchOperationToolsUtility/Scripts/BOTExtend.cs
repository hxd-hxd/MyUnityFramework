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

        #region ��չ

        /// <summary>
        /// ��ȡ����Ŀ¼
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFullPath(this string path)
        {
            return Path.GetFullPath(path);
        }
        /// <summary>
        /// �Ƿ���Ŀ¼
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectory(this string path)
        {
            return Directory.Exists(Path.GetFullPath(path));
        }
        /// <summary>
        /// �ų�ָ�����ļ�����
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
        /// ����ָ�����ļ�����
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
        /// ��ȡΪ asset �ʲ�·����ʽ
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CutOutAssetPath(this string path)
        {
            string[] strs = path.Split("Assets");
            return "Assets" + strs[strs.Length - 1];
        }

        /// <summary>
        /// �Ƴ���Ч�ַ���
        /// <para>ps��" "��"\t"��"\n"��"\r"��</para>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveEmpty(this string value)
        {
            value = value.Remove("\t", "\n", "\r", " ");
            return value;
        }
        /// <summary>
        /// �Ƴ��ַ����ո�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveSpace(this string value)
        {
            return value.Replace(" ", null);
        }
        /// <summary>
        /// �Ƴ��ַ���
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
        /// ���ַ������
        /// </summary>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string[] Split(this string value, string split)
        {
            string[] strs = Regex.Split(value, split);
            return strs;
        }

        /// <summary>
        /// �Ƿ������������һ���ַ���
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
        /// �ڸ������ַ��б��У����Ұ������ַ���
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
        /// �����ַ����ĸ���
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
        /// �Ƿ��������ȫ���ַ���
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
        /// �ַ������ת��Ϊ��άǶ�׵� float �б�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<List<float>> ToFloatTwoList(this string value, char parseSign1, char parseSign2)
        {
            return value.Split(parseSign1).ToFloatTwoList(parseSign2);
        }
        /// <summary>
        /// �ַ���������ת��Ϊ��άǶ�׵� float �б�
        /// </summary>
        /// <param name="vs"></param>
        /// <param name="parseSign">��ֽ�����</param>
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
        /// �ַ������ת��Ϊ float �б�
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parseSign">��ֽ�����</param>
        /// <returns></returns>
        public static List<float> ToFloatList(this string value, char parseSign)
        {
            return value.Split(parseSign).ToFloatList();
        }
        /// <summary>
        /// �ַ�������תΪ float �б�
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
        /// �ַ�������תΪ float ����
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
        /// �ַ���תΪ int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this string value)
        {
            value = value.RemoveEmpty();
            return value == "" ? 0 : int.Parse(value);
        }
        /// <summary>
        /// �ַ���תΪ float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ToFloat(this string value)
        {
            value = value.RemoveEmpty();
            return value == "" ? 0 : float.Parse(value);
        }

        /// <summary>
        /// ��ӡ�����б�Ԫ��
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