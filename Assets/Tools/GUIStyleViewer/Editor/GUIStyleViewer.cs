using UnityEngine;
using UnityEditor;

public class GUIStyleViewer : EditorWindow
{
    private Vector2 scrollVector2 = Vector2.zero;
    private string search = "";
    private int currentNum;// 当前数量

    [MenuItem("Tools/Window/GUIStyle查看器")]
    public static void InitWindow()
    {
        EditorWindow.GetWindow(typeof(GUIStyleViewer));
    }

    void OnGUI()
    {
        //Debug.Log(rootVisualElement.layout);
        currentNum = 0;

        GUILayout.BeginHorizontal("HelpBox");
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("搜索", GUILayout.MaxWidth(36));
            search = EditorGUILayout.TextField("", search, "SearchTextField", GUILayout.MaxWidth(400));

            foreach (GUIStyle style in GUI.skin.customStyles)
            {
                if (style.name.ToLower().Contains(search.ToLower()))
                {
                    currentNum++;
                }
            }
            EditorGUILayout.LabelField(string.Format("{0}/{1}", currentNum, GUI.skin.customStyles.Length), GUILayout.MaxWidth(50));
            //GUILayout.Label("取消", "SearchCancelButtonEmpty");
            if (!string.IsNullOrEmpty(search))
                if (GUILayout.Button("", "SearchCancelButton"))
                {
                    search = "";
                }
        }
        GUILayout.EndHorizontal();

        int num = 0;
        scrollVector2 = GUILayout.BeginScrollView(scrollVector2);
        foreach (GUIStyle style in GUI.skin.customStyles)
        {
            if (style.name.ToLower().Contains(search.ToLower()))
            {
                num++;
                DrawStyleItem(num, style);
            }
        }
        GUILayout.EndScrollView();
    }

    void DrawStyleItem(int serialNumber, GUIStyle style)
    {
        GUILayout.BeginHorizontal("box");
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField(serialNumber.ToString(), GUILayout.MaxWidth(30));
            GUILayout.Space(10);
            EditorGUILayout.SelectableLabel(style.name);
            GUILayout.FlexibleSpace();
            EditorGUILayout.SelectableLabel(style.name, style);
            GUILayout.Space(40);
            EditorGUILayout.SelectableLabel("", style, GUILayout.Height(40), GUILayout.Width(40));
            GUILayout.Space(175);
            //GUILayout.FlexibleSpace();
            //GUILayout.Space(style.fixedWidth);
            if (GUILayout.Button("复制GUIStyle名字"))
            {
                TextEditor textEditor = new TextEditor();
                textEditor.text = style.name;
                textEditor.OnFocus();
                textEditor.Copy();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
}