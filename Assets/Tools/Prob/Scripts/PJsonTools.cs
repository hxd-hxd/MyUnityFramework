using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Prob
{
    /// <summary>
    /// Json 工具
    /// </summary>
    public class PJsonTools
    {
        public static T FromJson<T>(string jsonText) where T : class
        {
            T t = JsonUtility.FromJson<T>(jsonText);
            return t;
        }
        public static string ToJson(object data, bool prettyPrint = true)
        {
            string jsonText = JsonUtility.ToJson(data, prettyPrint);
            return jsonText;
        }

        /// <summary>
        /// 写入 Json 二进制
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void WriteBinary(string path, object data)
        {
            if (!File.Exists(path))
            {
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                File.Create(path).Close();
            }
            string text = JsonUtility.ToJson(data, true);
            byte[] vs = StringToByte(text);

            File.WriteAllBytes(path, vs);
        }

        /// <summary>
        /// 读取 Json 文本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T ReadText<T>(string path) where T : class
        {
            if (!File.Exists(path))
            {
                return null;
            }
            string data = File.ReadAllText(path);
            T t = JsonUtility.FromJson<T>(data);
            return t;
        }

        /// <summary>
        /// 写入 Json 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void WriteText(string path, object data)
        {
            bool fileExists = File.Exists(path);

            if (!fileExists)
            {
                if (Directory.Exists(path))
                {
                    path = path + "/" + data.GetType().FullName + ".json";
                    File.Create(path).Close();
                }
                else
                {
                    string directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    File.Create(path).Close();
                }
            }
            string text = JsonUtility.ToJson(data, true);
            if (File.Exists(path))
            {
                File.WriteAllText(path, text);
            }
        }

        public static byte[] StringToByte(string data)
        {
            byte[] vs = Encoding.Unicode.GetBytes(data);
            return vs;
        }
        public static string ByteToString(byte[] data)
        {
            string vs = Encoding.Unicode.GetString(data);
            return vs;
        }

        /// <summary>
        /// Json 对象序列化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void JsonSerialize(string path, object data)
        {
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, data);
            }
        }
        /// <summary>
        /// Json 对象反序列化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static T JsonDeserialize<T>(string path) where T : class
        {
            T data = null;
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = (T)formatter.Deserialize(stream);
            }

            return data;
        }
    }
}