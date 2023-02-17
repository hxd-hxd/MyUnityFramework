// -------------------------
// 创建日期：2021/7/30 19:47:40
// -------------------------

using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using static BatchOperationToolsUtility.BOTConstant;
using static BatchOperationToolsUtility.BOTUtility;
using static BatchOperationToolsUtility.BOTUtility.BMANUtility;
using static BatchOperationToolsUtility.BOTGlobalVariable;

namespace BatchOperationToolsUtility
{
    // 批量修改或者添加命名空间
    public class BatchModifyOrAddNamespaceWindow : EditorWindow
    {
        [MenuItem("Tools/Window/BatchModifyOrAddNamespace")]
        static void Menu()
        {
            BatchModifyOrAddNamespaceWindow my = GetWindow<BatchModifyOrAddNamespaceWindow>("批量修改或者添加命名空间");
            my.minSize = new Vector2(530, 580);
        }
        [MenuItem("Tools/Window/BatchModifyOrAddNamespace", true)]
        static bool MenuEnable()
        {
            return Enable;
        }

        const float spaceOne = 16;
        GUILayoutOption maxWidth25 = GUILayout.MaxWidth(25);
        GUILayoutOption maxWidth50 = GUILayout.MaxWidth(50);
        GUILayoutOption maxWidth100 = GUILayout.MaxWidth(100);
        GUILayoutOption maxWidth125 = GUILayout.MaxWidth(125);
        GUILayoutOption maxWidth150 = GUILayout.MaxWidth(150);

        GUILayoutOption maxHeight50 = GUILayout.MinHeight(50);

        Vector2 sv;

        static List<OperationItem> namespaceFileList = BOTGlobalVariable.namespaceFileList;// 要操作的项列表

        private void OnGUI()
        {
            sv = EditorGUILayout.BeginScrollView(sv);
            {
                EditorGUI.BeginDisabledGroup(!Enable);

                // 配置
                GUILayout.Space(10);
                EditorGUILayout.LabelField("配置", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(spaceOne);
                        EditorGUILayout.LabelField("命名空间名称", maxWidth100);
                        namespaceValue = EditorGUILayout.TextField(namespaceValue);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(spaceOne);
                        EditorGUILayout.LabelField("支持修改的文件", maxWidth100);
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < BOTConstant.FileExtensionList.Count; i++)
                        {
                            sb.Append(BOTConstant.FileExtensionList[i]).Append("  ");
                        }

                        EditorGUILayout.BeginHorizontal("box");
                        {
                            EditorGUILayout.LabelField(sb.ToString());
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(spaceOne);
                        EditorGUILayout.LabelField("修改目标文件的命名空间", maxWidth150);
                        modify = EditorGUILayout.Toggle(modify);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(spaceOne);
                        EditorGUILayout.LabelField("为目标文件添加命名空间", maxWidth150);
                        add = EditorGUILayout.Toggle(add);
                    }
                    EditorGUILayout.EndHorizontal();

                    // 执行
                    bool noCanExecution = string.IsNullOrWhiteSpace(namespaceValue) || !modify && !add || namespaceFileList.Count <= 0 || namespaceFileList.Find((value) => value.file != BOTConstant.DefaultTextAsset) == null;// 不可执行的条件
                    EditorGUI.BeginDisabledGroup(noCanExecution);
                    {
                        if (GUILayout.Button("执行", GUILayout.MinHeight(35)))
                        {
                            Execution();
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                    MessageType mt = MessageType.Info;
                    if (noCanExecution)
                        mt = MessageType.Warning;
                    EditorGUILayout.HelpBox("执行修改前：\r\n   请添加要修改的命名空间（确保合法性）\r\n   选择修改条件\r\n   确保修改列表中有有效的修改文件", mt);
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(spaceOne / 2);

                // 选择
                EditorGUILayout.LabelField("选择", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(spaceOne);
                        EditorGUILayout.LabelField("要修改的文件列表", maxWidth125);
                        EditorGUILayout.LabelField(namespaceFileList.Count.ToString());

                        float mw = 40;
                        BtnPlusMinus(mw, () =>
                        {
                            if (namespaceFileList.Count > 0) namespaceFileList.RemoveAt(namespaceFileList.Count - 1);
                        }, () =>
                        {
                            namespaceFileList.Add(new OperationItem());
                        });
                        if (GUILayout.Button("ClearALL", "toolbarbuttonLeft", GUILayout.MaxWidth(mw * 3), GUILayout.MinWidth(mw * 2)))
                        {
                            namespaceFileList.Clear();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    // 拖入区
                    Rect rectDrag = EditorGUILayout.BeginVertical("box");
                    {
                        // SelectionRect
                        // sv_iconselector_back
                        // Tab onlyOne
                        // WarningOverlay
                        EditorGUILayout.LabelField("", "拖入想要修改的文件或文件夹到此区域以添加", "Tab onlyOne", maxHeight50);
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical("box");
                    {
                        if (namespaceFileList.Count <= 0)
                        {
                            EditorGUILayout.LabelField("", "未选择任何文件", "LODRendererAddButton");
                        }
                        else
                        {
                            for (int i = 0; i < namespaceFileList.Count; i++)
                            {
                                int index = i;
                                FileOperationItem(namespaceFileList[index], index);
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();


                    // 拖入区文件检测放到最后，用于解决：ArgumentException: Getting control 1's position in a group with only 1 controls when doing DragExited Aborting
                    DragAddFile(rectDrag);

                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndScrollView();
        }

        // 一个文件操作项GUI
        private void FileOperationItem(OperationItem item, int index)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(spaceOne * 2);

                EditorGUILayout.LabelField(item == null || !item.file || string.IsNullOrEmpty(item.file.name) || item.file == BOTConstant.DefaultTextAsset ? "E " + index : index + "  " + item.file.name, maxWidth150);

                UnityEngine.Object obj = EditorGUILayout.ObjectField(item.file, item.file.GetType(), false);
                if (obj == null)
                {
                    item.file = BOTConstant.DefaultTextAsset;
                    obj = item.file;
                }

                AddOperationItem(obj, item);

                //Debug.LogFormat("{0}\r\n{1}", item.file.name, item.pathFull);

                BtnMinus(25, () =>
                {
                    namespaceFileList.RemoveAt(index);
                });
            }
            EditorGUILayout.EndHorizontal();
        }
        // 加减按钮
        private void BtnPlusMinus(float width, System.Action Minus, System.Action Plus)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(width * 2));
            {
                // "OL Minus" "OL Plus"
                // "ShurikenMinus" "ShurikenPlus"
                BtnMinus(width, () =>
                {
                    Minus?.Invoke();
                });
                BtnPlus(width, () =>
                {
                    Plus?.Invoke();
                });
            }
            EditorGUILayout.EndHorizontal();
        }
        private void BtnPlus(float width, System.Action Plus)
        {
            // "OL Minus" "OL Plus"
            // "ShurikenMinus" "ShurikenPlus"
            if (GUILayout.Button("", "OL Plus", GUILayout.MaxWidth(width), GUILayout.MinWidth(width)))
            {
                Plus?.Invoke();
            }
        }
        private void BtnMinus(float width, System.Action Minus)
        {
            // "OL Minus" "OL Plus"
            // "ShurikenMinus" "ShurikenPlus"
            if (GUILayout.Button("", "OL Minus", GUILayout.MaxWidth(width), GUILayout.MinWidth(width)))
            {
                Minus?.Invoke();
            }
        }
        // 空间
        private void SpaceOne(float size = 1)
        {
            GUILayout.Space(spaceOne * size);
        }

        // 拖拽添加文件
        private void DragAddFile(Rect rect)
        {
            if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited) && rect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;// 改变鼠标图标

                if (Event.current.type == EventType.DragExited)
                {
                    string[] paths = DragAndDrop.paths;
                    UnityEngine.Object[] objs = DragAndDrop.objectReferences;

                    AddAllAsset(objs);
                }
            }
        }

        // 为操作项添加文件
        private void AddOperationItems(params UnityEngine.Object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                Object obj = objs[i];

                OperationItem item = AddOperationItem(obj);
                if (item != null)
                {
                    namespaceFileList.Add(item);
                }
            }
        }
        // 为操作项添加文件
        private OperationItem AddOperationItem(UnityEngine.Object obj)
        {
            OperationItem item = new OperationItem();

            if (AddOperationItem(obj, item))
            {
                return item;
            }
            return null;
        }
        // 为操作项添加文件
        private bool AddOperationItem(UnityEngine.Object obj, OperationItem item)
        {
            if (obj == item.file) return false;

            if (obj)
            {
                TextAsset file = (TextAsset)obj;
                string pathAsset = AssetDatabase.GetAssetPath(obj);
                string pathFull = AssetDatabase.Contains(obj) ? Path.GetFullPath(AssetDatabase.GetAssetPath(obj)) : "";

                bool noSelf = false;
                noSelf = namespaceFileList.Find((value) => value.file == file && value.file.name != BOTConstant.DefaultName) == null;
                if (obj && IsPerfect(obj) && noSelf)
                {
                    item.file = file;
                    item.pathAsset = pathAsset;
                    item.pathFull = pathFull;

                    return true;
                }
            }

            return false;
        }

        private void AddAllAsset(params Object[] objs)
        {
            for (int i = 0; i < objs.Length; i++)
            {
                AddAsset(objs[i]);
            }
        }
        private void AddAsset(Object obj)
        {
            if (!obj) return;

            DefaultAsset da = obj as DefaultAsset;

            if (da != null)
            {
                AddAllAsset(da);
            }
            else
            {
                AddOperationItems(obj);
            }
        }
        // 加载文件夹下的所有资产
        private void AddAllAsset(DefaultAsset obj)
        {
            if (obj)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                // 获取子目录
                string[] dirs = Directory.GetDirectories(path.GetFullPath());
                for (int i = 0; i < dirs.Length; i++)
                {
                    AddAllAsset(AssetDatabase.LoadAssetAtPath(dirs[i].CutOutAssetPath(), typeof(DefaultAsset)) as DefaultAsset);
                }
                // 获取该目录下的文件
                string[] files = Directory.GetFiles(path.GetFullPath()).ExcludeType("meta");
                for (int i = 0; i < files.Length; i++)
                {
                    AddAsset(AssetDatabase.LoadAssetAtPath(files[i].CutOutAssetPath(), typeof(Object)));
                }
            }
        }

        // 执行
        private void Execution()
        {
            BOTUtility.Execution(namespaceFileList);

            AssetDatabase.Refresh();
        }

    }
}