using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AutoNamespace
{
    public static class NotNameSpaceMenu
    {

        [MenuItem(Constant.Menu.MenuNotNamespacePath + "/Static", false, Constant.RootMenuPriority)]
        private static void NamespaceMenu_Static()
        {
            AutoNamespaceUtility.CreateCSharpScriptAssetBy("", AutoNamespaceUtility.StaticScriptTemplatePath_NotNamespace, "NewStaticScript");
        }
        // ���������ڿ��Ʋ˵���ť������
        [MenuItem(Constant.Menu.MenuNotNamespacePath + "/Static", true, Constant.RootMenuPriority)]
        private static bool NamespaceMenuEnable_Static()
        {
            return AutoNamespaceUtility.Enable();
        }


        [MenuItem(Constant.Menu.MenuNotNamespacePath + "/Script", false, Constant.RootMenuPriority)]
        private static void NamespaceMenu_Script()
        {
            AutoNamespaceUtility.CreateCSharpScriptAssetBy("", AutoNamespaceUtility.ScriptTemplatePath_NotNamespace, "NewBehaviourScript");
        }
        // ���������ڿ��Ʋ˵���ť������
        [MenuItem(Constant.Menu.MenuNotNamespacePath + "/Script", true, Constant.RootMenuPriority)]
        private static bool NamespaceMenuEnable_Script()
        {
            return AutoNamespaceUtility.Enable();
        }


        [MenuItem(Constant.Menu.MenuNotNamespacePath + "/Editor", false, Constant.RootMenuPriority)]
        private static void NamespaceMenu_EditorScript()
        {
            AutoNamespaceUtility.CreateCSharpScriptAssetBy("", AutoNamespaceUtility.EditorScriptTemplatePath_NotNamespace, "NewEditorScript");
        }
        // ���������ڿ��Ʋ˵���ť������
        [MenuItem(Constant.Menu.MenuNotNamespacePath + "/Editor", true, Constant.RootMenuPriority)]
        private static bool NamespaceMenuEnable_EditorScript()
        {
            return AutoNamespaceUtility.Enable();
        }


        [MenuItem(Constant.Menu.MenuNotNamespacePath + "/EditorWindow", false, Constant.RootMenuPriority)]
        private static void NamespaceMenu_EditorWindowScript()
        {
            AutoNamespaceUtility.CreateCSharpScriptAssetBy("", AutoNamespaceUtility.EditorWindowScriptTemplatePath_NotNamespace, "NewEditorWindowScript");
        }
        // ���������ڿ��Ʋ˵���ť������
        [MenuItem(Constant.Menu.MenuNotNamespacePath + "/EditorWindow", true, Constant.RootMenuPriority)]
        private static bool NamespaceMenuEnable_EditorWindowScript()
        {
            return AutoNamespaceUtility.Enable();
        }

    }
}