using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoNamespace
{
    [System.Serializable]
    public class NamespaceConfig
    {
        /// <summary>
        /// 允许配置的动态路径
        /// <para>ps：修改时的格式限定为："/你要修改的目录"</para>
        /// </summary>
        public string DynamicPath;

        /// <summary>
        /// 自动创建菜单脚本的目录
        /// </summary>
        public string AutoCreateDirectory;
        /// <summary>
        /// 脚本模板路径
        /// </summary>
        public string ScriptTemplatePath;
        /// <summary>
        /// 静态脚本模板路径
        /// </summary>
        public string StaticScriptTemplatePath;
        /// <summary>
        /// 编辑器脚本模板路径
        /// </summary>
        public string EditorScriptTemplatePath;
        /// <summary>
        /// 编辑器窗口脚本模板路径
        /// </summary>
        public string EditorWindowScriptTemplatePath;
        /// <summary>
        /// 脚本模板路径
        /// </summary>
        public string ScriptTemplatePath_NotNamespace;
        /// <summary>
        /// 静态脚本模板路径
        /// </summary>
        public string StaticScriptTemplatePath_NotNamespace;
        /// <summary>
        /// 编辑器脚本模板路径
        /// </summary>
        public string EditorScriptTemplatePath_NotNamespace;
        /// <summary>
        /// 编辑器窗口脚本模板路径
        /// </summary>
        public string EditorWindowScriptTemplatePath_NotNamespace;
        /// <summary>
        /// 菜单脚本模板路径
        /// </summary>
        public string MenuScriptTemplatePath;
        /// <summary>
        /// 菜单选项模板路径
        /// </summary>
        public string MenuOptionTemplatePath;

        public bool enable;
        public List<string> NamespaceList;
    }
}