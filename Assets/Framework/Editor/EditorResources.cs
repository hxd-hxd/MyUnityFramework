// -------------------------
// 创建日期：2024/3/19 10:46:29
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;

namespace Framework.Editor
{
    /// <summary>
    /// 加载编辑器资源文件夹中的资源
    /// <para>和 <see cref="Resources"/> 功能类似，但只能加载包含在 Editor 文件夹中的 Resources 子文件夹里的资源</para>
    /// </summary>
    public class EditorResources
    {
        // 查找 Editor 文件夹的子文件夹 Resources
        /// <summary>
        /// 加载资源，需要完整名称（text.txt）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resName"></param>
        /// <returns></returns>
        public static T Load<T>(string resName) where T : Object
        {
            Object obj = Load(resName, typeof(T));
            return obj as T;
        }
        /// <summary>
        /// 加载资源，需要完整名称（text.txt）
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Object Load(string resName, Type type)
        {
            return Load(Application.dataPath, resName, type);
        }

        // 递归的查找资源
        static Object Load(string dir, string resName, Type type)
        {
            Object obj = null;

            // 查找 Editor 文件夹
            var dirs = Directory.GetDirectories(dir);
            foreach (var ldir in dirs)
            {
                string dirName = Path.GetFileName(ldir);
                if (dirName == "Editor")
                {
                    // 查找 Resources 文件夹
                    var edirs = Directory.GetDirectories(ldir);
                    foreach (var ledir in edirs)
                    {
                        string edirName = Path.GetFileName(ledir);
                        if (edirName == "Resources")
                        {
                            string resFileFullPath = Path.Combine(ledir, resName);// 资源文件的完整路径
                            // 查看指定资源文件是否存在
                            if (!File.Exists(resFileFullPath))
                            {
                                continue;
                            }
                            // 获取 Unity Assets 文件夹的路径
                            string resFileUnityPath = resFileFullPath.Replace(Application.dataPath, "Assets");
                            obj = AssetDatabase.LoadAssetAtPath(resFileUnityPath, type);
                            break;
                        }
                        else
                        {
                            obj = Load(ldir, resName, type);
                        }
                    }
                }
                else
                {
                    obj = Load(ldir, resName, type);
                }

                if (obj != null) break;
            }

            return obj;
        }
    }
}