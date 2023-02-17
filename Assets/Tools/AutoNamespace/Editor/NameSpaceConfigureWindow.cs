using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Threading;

namespace AutoNamespace
{
    // 配置命名空间
    public class NameSpaceConfigureWindow : EditorWindow
    {
        [MenuItem("Tools/Window/AutoNamespace")]
        static void Open()
        {
            NameSpaceConfigureWindow my = GetWindow<NameSpaceConfigureWindow>("自动添加命名空间配置");
            my.minSize = new Vector2(450, 580);
        }

        SerializedObject _NamespaceConfig;
        SerializedProperty _NamespaceList;
        [SerializeField]
        List<string> namespaceList = new List<string>();

        [SerializeField]
        NamespaceConfig nc;

        int explainMaxWidth = 140;

        static bool pathFoldoutHeaderGroup = true;
        static float pathFadeGroupOver = 1;
        static float pathFadeGroup = 1;
        float currentVelocity = 0;

        private void OnEnable()
        {
            nc = Read(Constant.ConfigurePath);
            if (nc != null && nc.NamespaceList != null)
            {
                namespaceList = nc.NamespaceList;
                nc.NamespaceList = null;
            }
            _NamespaceConfig = new SerializedObject(this);
            _NamespaceList = _NamespaceConfig.FindProperty("namespaceList");

        }

        //private void Update()
        //{
        //    //Debug.Log("Update");

        //    if (!Mathf.Approximately(pathFadeGroup, pathFadeGroupOver))
        //    {
        //        pathFadeGroup = Mathf.SmoothDamp(pathFadeGroup, pathFadeGroupOver, ref currentVelocity, 5f, 1);
        //    }
        //}

        private void OnGUI()
        {
            // 地址配置信息
            //EditorGUI.BeginChangeCheck();
            //{
            EditorGUILayout.Space(10, false);
            EditorGUILayout.LabelField("地址配置", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            {
                //EditorGUILayout.BeginHorizontal();
                //{
                //    GUILayout.Space(16);
                //    EditorGUILayout.LabelField("动态配置的地址", GUILayout.MaxWidth(explainMaxWidth + 16));
                //    nc.DynamicPath = EditorGUILayout.TextField(nc.DynamicPath);
                //}
                //EditorGUILayout.EndHorizontal();
                //EditorGUILayout.HelpBox("请注意保持本工具文件夹始终直属于 Assets 目录！\r\n否则请配置动态地址，格式限定为：\" / 你要修改的目录\"", MessageType.Warning);
                EditorGUILayout.HelpBox(string.Format("请注意保持本工具文件夹始终直属于 Assets{0} 目录！", nc.DynamicPath), MessageType.Warning);


                pathFoldoutHeaderGroup = EditorGUILayout.BeginFoldoutHeaderGroup(pathFoldoutHeaderGroup, pathFoldoutHeaderGroup ? "收起" : "展开");
                if (pathFoldoutHeaderGroup)
                {
                    AddPathExplain("自动创建脚本的目录", nc.AutoCreateDirectory, true);
                    AddPathExplain("菜单脚本模板路径", nc.MenuScriptTemplatePath, true);
                    AddPathExplain("菜单选项脚本模板路径", nc.MenuOptionTemplatePath, true);
                    AddPathExplain("脚本模板路径", nc.ScriptTemplatePath, true);

                    EditorGUILayout.LabelField("其他", GUILayout.MaxWidth(explainMaxWidth));
                    AddPathExplain("静态脚本模板路径", nc.StaticScriptTemplatePath, true);
                    AddPathExplain("编辑器脚本模板路径", nc.EditorScriptTemplatePath, true);
                    AddPathExplain("编辑器窗口脚本模板路径", nc.EditorWindowScriptTemplatePath, true);
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("不带命名空间", GUILayout.MaxWidth(explainMaxWidth));
                    AddPathExplain("静态脚本模板路径", nc.ScriptTemplatePath_NotNamespace, true);
                    AddPathExplain("静态脚本模板路径", nc.StaticScriptTemplatePath_NotNamespace, true);
                    AddPathExplain("编辑器脚本模板路径", nc.EditorScriptTemplatePath_NotNamespace, true);
                    AddPathExplain("编辑器窗口脚本模板路径", nc.EditorWindowScriptTemplatePath_NotNamespace, true);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();


                //pathFoldoutHeaderGroup = EditorGUILayout.BeginFoldoutHeaderGroup(pathFoldoutHeaderGroup, pathFoldoutHeaderGroup ? "收起" : "展开");
                //{
                //    pathFadeGroupOver = pathFoldoutHeaderGroup ? 1 : 0;
                //}
                //EditorGUILayout.EndFoldoutHeaderGroup();

                //EditorGUILayout.BeginFadeGroup(pathFadeGroup);
                ////if (pathFoldoutHeaderGroup)
                //{
                //    AddPathExplain("自动创建脚本的目录", Application.dataPath + nc.AutoCreateDirectory);
                //    AddPathExplain("脚本模板路径", nc.ScriptTemplatePath);
                //    AddPathExplain("菜单脚本模板路径", nc.MenuScriptTemplatePath);
                //    AddPathExplain("菜单选项脚本模板路径", nc.MenuOptionTemplatePath);
                //}
                //EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndVertical();
            //}
            //if (EditorGUI.EndChangeCheck())
            //{
            //    Save(Constant.ConfigurePath);

            //    AssetDatabase.Refresh();
            //}

            GUILayout.Space(10);

            // 命名空间配置列表
            _NamespaceConfig.Update();// 更新
            EditorGUI.BeginChangeCheck();// 检查是否被修改
            {

                EditorGUILayout.LabelField("命名空间配置", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.Space(25, false);
                        EditorGUILayout.LabelField("启用", GUILayout.MaxWidth(50));
                        nc.enable = EditorGUILayout.Toggle(nc.enable);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUI.BeginDisabledGroup(!nc.enable);// 启用条件
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.Space(5, false);
                                EditorGUILayout.PropertyField(_NamespaceList, true);
                            }
                            EditorGUILayout.EndHorizontal();

                            if (namespaceList.Count <= 0)
                            {
                                EditorGUILayout.HelpBox("未配置任何命名空间！", MessageType.Warning);
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("使用前请仔细检查命名空间格式，以确保没有非法字符！", MessageType.Info);
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();

            }
            if (EditorGUI.EndChangeCheck())//结束检查是否有修改
            {
                //提交修改
                if (CheckNamespaceConfigList(_NamespaceList))
                {

                }
                else
                {
                    //Debug.Log(_NamespaceList.GetArrayElementAtIndex(0).stringValue);
                    _NamespaceConfig.ApplyModifiedProperties();
                }

                Save(Constant.ConfigurePath);

                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("创建菜单选项", GUILayout.Height(32)))
            {
                Debug.LogFormat("创建时间：{0}", DateTime.Now.ToString());

                CreateMenu(namespaceList);

                AssetDatabase.Refresh();
            }
        }

        // 检查命名空间配置列表是否允许修改
        public bool CheckNamespaceConfigList(SerializedProperty namespaceConfigList)
        {
            // 以下是不允许修改的情况
            int count = namespaceConfigList.arraySize;
            //bool exist = false;
            //for (int i = 0; i < count; i++)
            //{
            //    if (i != count - 1)
            //        // 不允许存在相同的
            //        if (namespaceList.Contains(namespaceConfigList.GetArrayElementAtIndex(count - 1).stringValue))
            //        {
            //            exist = true;
            //            break;
            //        }
            //}

            return count < 1 || namespaceConfigList.GetArrayElementAtIndex(0).stringValue != Constant.DefaultNamespace;
        }

        // 添加用于创建对应命名空间C#脚本的菜单项
        private void CreateMenu(List<string> namespaceList)
        {
            //string menuScript = File.ReadAllText(Constant.MenuScriptTemplatePath);
            //string menuOption = File.ReadAllText(Constant.MenuOptionTemplatePath);
            TextAsset menuScriptText = AssetDatabase.LoadAssetAtPath<TextAsset>(CorrectPath(nc.MenuScriptTemplatePath));
            TextAsset menuOptionText = AssetDatabase.LoadAssetAtPath<TextAsset>(CorrectPath(nc.MenuOptionTemplatePath));
            string menuScript = menuScriptText.text;
            string menuOption = menuOptionText.text;

            menuScript = menuScript.Replace(Constant.CreateTime, DateTime.Now.ToString());
            menuScript = menuScript.Replace(Constant.Namespace, Constant.DefaultNamespace);
            menuScript = menuScript.Replace(Constant.ScriptName, Constant.DefaultMenuScriptName);

            //StringBuilder menuText = new StringBuilder(menuScript);
            StringBuilder menuText = new StringBuilder();

            List<string> menuAdd = new List<string>();
            int numSpace = 0;

            // 创建菜单选项
            for (int i = 0; i < namespaceList.Count; i++)
            {
                string strMenuOption = menuOption;
                string MenuName = namespaceList[i];
                string MenuFuncName = namespaceList[i];
                int MenuPriority = Constant.DefaultMenuPriority;

                MenuFuncName = MenuFuncName.Replace('.', '_');

                // 分割
                if (string.IsNullOrWhiteSpace(MenuName))
                {
                    numSpace++;
                    MenuName = "";
                    MenuFuncName = "_Space_" + numSpace.ToString();
                }
                else
                {
                    // 检查是否有重复的菜单项
                    if (menuAdd.Contains(MenuName))
                    {
                        continue;
                    }
                    menuAdd.Add(MenuName);
                }

                // 设置默认命名空间的优先级
                if (MenuName == Constant.DefaultNamespace)
                {
                    MenuPriority = Constant.RootMenuPriority;
                }

                //System.Text.RegularExpressions.Regex.Replace(str, Constant.MenuName, namespaceList[i]);
                strMenuOption = strMenuOption.Replace(Constant.MenuName, MenuName);         // 要创建的菜单名
                strMenuOption = strMenuOption.Replace(Constant.MenuFuncName, MenuFuncName); // 要创建菜单的方法名
                strMenuOption = strMenuOption.Replace(Constant.MenuPriority, MenuPriority.ToString()); // 要创建菜单的优先级

                menuText.Append("\r\n\n").Append(strMenuOption);
            }

            //menuText.Append("\r\n\t}\r\n}");
            //Debug.LogFormat("创建结果：\r\n{0}", menuText.ToString());
            //AutoNamespaceUtility.WriteOrCreateFile(Application.dataPath + CorrectPath(nc.AutoCreateDirectory) + "Menu.cs", menuText.ToString());

            menuScript = menuScript.Replace(Constant.AddCode, menuText.ToString());

            Debug.LogFormat("创建结果：\r\n{0}", menuScript);

            AutoNamespaceUtility.WriteOrCreateFile(Application.dataPath + CorrectPath(nc.AutoCreateDirectory) + Constant.DefaultMenuScriptName + ".cs", menuScript);
        }

        // 添加一个路径说明
        private void AddPathExplain(string explain, string content, bool navigationToOptions = false)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(32);
                EditorGUILayout.LabelField(explain, GUILayout.MaxWidth(explainMaxWidth));

                content = CorrectPath(content);
                content = GetUnityAssetPath(content);

                    EditorGUILayout.LabelField(content);
                if (navigationToOptions)
                {
                    if (GUILayout.Button("选中", GUILayout.MaxWidth(100)))
                    { 
                        //string fileD = Path.GetDirectoryName(content);
                        //string fileName = Path.GetFileName(content);
                        //string fileE = Path.GetExtension(content);
                        //string p = fileD + "\\" + fileName.Replace(fileE, null);
                        //UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(p);
                        UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(content);
                        Selection.activeObject = obj;
                    }
                    GUILayout.Space(16);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // 正确的路径
        private string CorrectPath(string content)
        {
            return string.Format(content, nc.DynamicPath);
        }

        // 获取 unity 资产路径
        private string GetUnityAssetPath(string path)
        {
            string[] pathStrs = Regex.Split(path, "Assets");
            string assetPath = "Assets" + pathStrs[pathStrs.Length - 1];
            while (true)
            {
                if (assetPath.EndsWith("/"))
                {
                    assetPath = assetPath.Remove(assetPath.Length - 1);
                    continue;
                }

                if (assetPath.EndsWith("\\"))
                {
                    assetPath = assetPath.Remove(assetPath.Length - 2);
                    continue;
                }

                break;
            }
            return assetPath;
        }

        //[MenuItem("Test/测试路径处理")]
        //static void Test()
        //{
        //    string path = "AAA/name/";
        //    Debug.Log(path.Remove(path.Length - 1));
        //}

        private NamespaceConfig Read(string path)
        {
            return AutoNamespaceUtility.Read(path);
        }
        private void Save(string path)
        {
            nc.NamespaceList = namespaceList;
            AutoNamespaceUtility.Save(nc, path);
        }

    }
}