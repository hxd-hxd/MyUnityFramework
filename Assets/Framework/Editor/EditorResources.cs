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
    /// <para>
    /// 功能和 <see cref="Resources"/> 类似，但只能加载包含在 Editor 文件夹中的下级文件夹 Resources 里的资源，
    /// </para>
    /// <para>
    /// 例如："Editor/Resources/Config/text.txt"、"Editor/Test/Resources/Config/text.txt"，都可以加载到 "Config/text.txt"
    /// </para>
    /// </summary>
    public class EditorResources
    {
        /// <summary>
        /// 加载资源
        /// <para>资源路径与 <see cref="Resources"/> 格式一样，需要完整名称（text.txt）</para>
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
        /// 加载资源
        /// <para>资源路径与 <see cref="Resources"/> 格式一样，需要完整名称（text.txt）</para>
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Object Load(string resName, Type type)
        {
            Object result = null;

            //result = Load(Application.dataPath, resName, type);

            if (GetFullPath(resName, out string resFileUnityPath))
            {
                result = AssetDatabase.LoadAssetAtPath(resFileUnityPath, type);
                //result = AssetDatabase.LoadMainAssetAtPath(resFileUnityPath);
                //result = EditorGUIUtility.Load(resFileUnityPath);
                //result = PrefabUtility.LoadPrefabContents(resFileUnityPath);

                // 以下是已经实例化到场景中
                //result = PrefabUtility.InstantiateAttachedAsset(result);
            }

            return result;
        }

        // 递归的查找资源
        [Obsolete]
        static Object _Load(string dir, string resName, Type type)
        {
            Object result = null;

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
                            result = AssetDatabase.LoadAssetAtPath(resFileUnityPath, type);
                            //result = AssetDatabase.LoadMainAssetAtPath(resFileUnityPath);
                            //result = EditorGUIUtility.Load(resFileUnityPath);
                            //result = PrefabUtility.LoadPrefabContents(resFileUnityPath);

                            // 以下是已经实例化到场景中
                            //result = PrefabUtility.InstantiateAttachedAsset(result);
                            break;
                        }
                        else
                        {
                            result = _Load(ldir, resName, type);
                        }
                    }
                }
                else
                {
                    result = _Load(ldir, resName, type);
                }

                if (result != null) break;
            }

            return result;
        }

        /// <summary>
        /// 获取 Unity 资产文件的全路径
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool GetFullPath(string resName, out string path)
        {
            List<string> paths = null;
            bool result = _GetFullPath(resName, false, ref paths);
            path = null;
            if (result)
            {
                path = paths[0];
            }

            TypePool.root.Return(paths);
            return result;
            //return _GetFullPath(Application.dataPath, resName, out path);
        }
        [Obsolete]
        static bool _GetFullPath(string dir, string resName, out string path)
        {
            bool result = false;
            path = null;

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
                                /* TODO：是否处理嵌套型文件
                                    例如：
                                    有以下两个文件路径
                                        1、Editor/Resources/Config/text.txt
                                        2、Editor/Resources/Resources/Config/text.txt
                                    要找到文件 Config/text.txt，
                                    其中 2 是否被算作资源，
                                    还是在顶级 Editor/Resources/ 就被截断，
                                    即 
                                        Config/text.txt
                                        Resources/Config/text.txt
                                    是两个资源。

                                如果不处理嵌套型文件，就取消以下代码的注释
                                */

                                //result = GetFullPath(ledir, resName, out path);
                                continue;
                            }
                            // 获取 Unity Assets 文件夹的路径
                            path = resFileFullPath.Replace(Application.dataPath, "Assets");
                            result = true;
                        }
                        else
                        {
                            result = _GetFullPath(ledir, resName, out path);
                        }

                        if (result) break;
                    }
                }
                else
                {
                    result = _GetFullPath(ldir, resName, out path);
                }

                if (result) break;
            }

            return result;
        }

        /// <summary>
        /// 获取 Unity 资产文件的全路径，将所有找到的路径存储在列表中
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static bool GetFullPath(string resName, ref List<string> paths) => _GetFullPath(resName, true, ref paths);

        /// <summary>
        /// 获取指定 Unity 资产文件的全路径，将所有找到的路径存储在列表中
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="findAll">true：找到所有资源，在不同目录下的相同资源  false：找到第一个</param>
        /// <param name="paths"></param>
        /// <returns>是否找到</returns>
        static bool _GetFullPath(string resName, bool findAll, ref List<string> paths)
        {
            //paths ??= new List<string>(1);
            paths ??= TypePool.root.GetList<string>();
            return _GetFullPath(Application.dataPath, resName, findAll, ref paths);
        }
        // 递归查找
        static bool _GetFullPath(string dir, string resName, bool findAll, ref List<string> paths)
        {
            bool result = false;
            if (paths != null) paths.Clear();

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
                                /* TODO：是否处理嵌套型文件
                                    例如：
                                    有以下两个文件路径
                                        1、Editor/Resources/Config/text.txt
                                        2、Editor/Resources/Resources/Config/text.txt
                                    要找到文件 Config/text.txt，
                                    其中 2 是否被算作资源，
                                    还是在顶级 Editor/Resources/ 就被截断，
                                    即 
                                        Config/text.txt
                                        Resources/Config/text.txt
                                    是两个资源。

                                如果不处理嵌套型文件，就取消以下代码的注释
                                */

                                //result = _GetFullPath(ledir, resName, ref paths);
                                continue;
                            }
                            // 获取 Unity Assets 文件夹的路径
                            string resPath = resFileFullPath.Replace(Application.dataPath, "Assets");
                            paths.Add(resPath);
                            result = true;
                        }
                        else
                        {
                            result = _GetFullPath(ledir, resName, findAll, ref paths);
                        }

                        if (result && !findAll) break;
                    }
                }
                else
                {
                    result = _GetFullPath(ldir, resName, findAll, ref paths);
                }

                if (result && !findAll) break;
            }

            return result;
        }
    }
}