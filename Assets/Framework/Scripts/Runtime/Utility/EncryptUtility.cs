﻿// -------------------------
// 创建日期：2022/11/11 18:45:15
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

namespace Framework
{
    /// <summary>
    /// 加解密
    /// </summary>
    public static class EncryptUtility
    {
        /// <summary>
        /// 安全密钥
        /// </summary>
        static string secretKey = "1yy2";


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToEncrypt(string str)
        {
            return ToEncrypt(secretKey, str);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string ToEncrypt(string encryptKey, string str)
        {
            //if (string.IsNullOrEmpty(encryptKey) || encryptKey.Length != 4)
            //{
            //    Debug.LogError("");
            //    return str;
            //}

            try
            {
                byte[] P_byte_key = Encoding.Unicode.GetBytes(encryptKey);//将密钥字符串转换为字节序列

                byte[] P_byte_data = Encoding.Unicode.GetBytes(str);//将字符串转换为字节序列

                MemoryStream P_Stream_MS = new MemoryStream();//创建内存流对象
                {
                    using (CryptoStream P_CryptStream_Stream = new CryptoStream(P_Stream_MS, new DESCryptoServiceProvider().CreateEncryptor(P_byte_key, P_byte_key), CryptoStreamMode.Write))
                    {
                        P_CryptStream_Stream.Write(P_byte_data, 0, P_byte_data.Length);//向加密流中写入字节序列

                        P_CryptStream_Stream.FlushFinalBlock();//将数据压入基础流

                        byte[] P_bt_temp = P_Stream_MS.ToArray();//从内存流中获取字节序列

                        return Convert.ToBase64String(P_bt_temp);
                    }
                }
            }

            catch (CryptographicException ce)
            {
                throw new Exception(ce.Message);
            }
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToDecrypt(string str)
        {
            return ToDecrypt(secretKey, str);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptKey"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string ToDecrypt(string encryptKey, string str)
        {
            try
            {
                byte[] P_byte_key = Encoding.Unicode.GetBytes(encryptKey);//将密钥字符串转换为字节序列

                byte[] P_byte_data = Convert.FromBase64String(str);//将加密后的字符串转换为字节序列

                MemoryStream P_Stream_MS = new MemoryStream(P_byte_data);//创建内存流对象并写入数据

                //创建加密流对象
                CryptoStream P_CryptStream_Stream = new CryptoStream(
                    P_Stream_MS,
                    new DESCryptoServiceProvider().CreateDecryptor(P_byte_key, P_byte_key),
                    CryptoStreamMode.Read);

                byte[] P_bt_temp = new byte[200];//创建字节序列对象

                MemoryStream P_MemoryStream_temp = new MemoryStream();//创建内存流对象

                int i = 0;//创建记数器

                while ((i = P_CryptStream_Stream.Read(P_bt_temp, 0, P_bt_temp.Length)) > 0)//使用while循环得到解密数据
                {
                    P_MemoryStream_temp.Write(P_bt_temp, 0, i);//将解密后的数据放入内存流
                }

                return Encoding.Unicode.GetString(P_MemoryStream_temp.ToArray());//方法返回解密后的字符串
            }

            catch (CryptographicException ce)
            {
                throw new Exception(ce.Message);
            }
        }
    }
}