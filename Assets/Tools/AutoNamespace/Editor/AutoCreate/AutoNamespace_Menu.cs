// -------------------------
// 由工具自动创建，请勿手动修改
// 创建日期：2022/9/5 16:16:55
// -------------------------

using UnityEngine;
using UnityEditor;

namespace AutoNamespace
{
    public static class AutoNamespace_Menu
    {
        

        [MenuItem(Constant.Menu.MenuAutoNamespacePath + "/AutoNamespace", false, 81)]
        private static void NamespaceMenu_AutoNamespace()
        {
            AutoNamespaceUtility.CreateCSharpScriptAsset("AutoNamespace");
        }
        // 本方法用于控制菜单按钮 AutoNamespace 的启用
        [MenuItem(Constant.Menu.MenuAutoNamespacePath + "/AutoNamespace", true, 81)]
        private static bool NamespaceMenuEnable_AutoNamespace()
        {
            return AutoNamespaceUtility.Enable();
        }
        

        [MenuItem(Constant.Menu.MenuAutoNamespacePath + "/TestMenu", false, 1000)]
        private static void NamespaceMenu_TestMenu()
        {
            AutoNamespaceUtility.CreateCSharpScriptAsset("TestMenu");
        }
        // 本方法用于控制菜单按钮 TestMenu 的启用
        [MenuItem(Constant.Menu.MenuAutoNamespacePath + "/TestMenu", true, 1000)]
        private static bool NamespaceMenuEnable_TestMenu()
        {
            return AutoNamespaceUtility.Enable();
        }
        

        [MenuItem(Constant.Menu.MenuAutoNamespacePath + "/Framework", false, 1000)]
        private static void NamespaceMenu_Framework()
        {
            AutoNamespaceUtility.CreateCSharpScriptAsset("Framework");
        }
        // 本方法用于控制菜单按钮 Framework 的启用
        [MenuItem(Constant.Menu.MenuAutoNamespacePath + "/Framework", true, 1000)]
        private static bool NamespaceMenuEnable_Framework()
        {
            return AutoNamespaceUtility.Enable();
        }
        
    }
}