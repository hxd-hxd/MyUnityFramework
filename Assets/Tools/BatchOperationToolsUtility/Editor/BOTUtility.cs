// -------------------------
// 创建日期：2021/8/3 16:52:10
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;
using static BatchOperationToolsUtility.BOTConstant;
using static BatchOperationToolsUtility.BOTGlobalVariable;
using System.Text;

namespace BatchOperationToolsUtility
{
    /// <summary>
    /// 批量操作工具的实用函数集
    /// </summary>
    public static partial class BOTUtility
    {
        /// <summary>
        /// 启用条件
        /// </summary>
        public static bool Enable
        {
            get
            {
                return !EditorApplication.isPlaying;

                //#if UNITY_EDITOR
                //                return !EditorApplication.isPlaying;
                //#else
                //                return false;
                //#endif
            }
        }

        /// <summary>
        /// 批量修改或添加命名空间工具的实用函数集
        /// </summary>
        public static class BMANUtility
        {
            /// <summary>
            /// 根据文本获取一个 操作项实例
            /// </summary>
            /// <param name="ta"></param>
            /// <returns></returns>
            public static OperationItem GetOperationItem(TextAsset ta)
            {
                OperationItem item = null;

                if (IsPerfect(ta))
                {
                    item = new OperationItem()
                    {
                        file = ta
                    };
                }

                return item;
            }

            /// <summary>
            /// 该文本文件是否符合条件
            /// </summary>
            /// <param name="ta"></param>
            /// <returns></returns>
            public static bool IsPerfect(TextAsset obj)
            {
                return BOTConstant.FileExtensionList.Contains(Path.GetExtension(AssetDatabase.GetAssetPath(obj)));
            }
            /// <summary>
            /// 该文本文件是否符合条件
            /// </summary>
            /// <param name="ta"></param>
            /// <returns></returns>
            public static bool IsPerfect(UnityEngine.Object obj)
            {
                return BOTConstant.FileExtensionList.Contains(Path.GetExtension(AssetDatabase.GetAssetPath(obj)));
            }

        }

        public static void LoadAsset(List<OperationItem> namespaceFileList)
        {
            for (int i = 0; i < namespaceFileList.Count; i++)
            {
                var item = namespaceFileList[i];
                item.file = AssetDatabase.LoadAssetAtPath<TextAsset>(item.pathAsset);
            }
        }

        /// <summary>
        /// 对操作项列表执行修改
        /// </summary>
        /// <param name="namespaceFileList"></param>
        public static void Execution(List<OperationItem> namespaceFileList)
        {
            for (int i = 0; i < namespaceFileList.Count; i++)
            {
                if (namespaceFileList[i].file == BOTConstant.DefaultTextAsset || namespaceFileList[i].file == null)
                {
                    continue;// 如果从列表中删除，则不修改
                }
                Execution(namespaceFileList[i].pathFull);
            }
        }
        public static void Execution(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            string newValue = "";

            if (CanExecutionOperationModify(lines))
            {
                newValue = ExecutionOperationModify(lines);
            }
            else if (CanExecutionOperationAdd(lines))
            {
                newValue = ExecutionOperationAdd(lines);
            }

            File.WriteAllText(filePath, newValue, Encoding.UTF8);
        }

        // 可否执行修改操作
        public static bool CanExecutionOperationModifyPath(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            return CanExecutionOperationModify(lines);
        }
        // 可否执行修改操作
        public static bool CanExecutionOperationModify(params string[] values)
        {
            if (modify)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (CanExecutionOperationModify(values[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        // 可否执行修改操作
        public static bool CanExecutionOperationModify(string value)
        {
            if (modify)
            {
                return IsNamespaceLine(value);
            }

            return false;
        }

        // 可否执行添加操作
        public static bool CanExecutionOperationAddPath(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            return CanExecutionOperationAdd(lines);
        }
        // 可否执行添加操作
        public static bool CanExecutionOperationAdd(params string[] values)
        {
            if (add)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (CanExecutionOperationAdd(values[i]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        // 可否执行添加操作
        public static bool CanExecutionOperationAdd(string value)
        {
            if (add)
            {
                return !IsNamespaceLine(value);
            }

            return false;
        }

        // 执行修改操作
        public static string ExecutionOperationModify(string[] values)
        {
            return NamespaceModify(namespaceValue, values);
        }
        // 执行添加操作
        public static string ExecutionOperationAdd(string[] values)
        {
            return NamespaceAdd(namespaceValue, values);
        }


        /// <summary>
        /// 是否是定义命名空间的行
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNamespaceLine(string value)
        {
            if (value.Contains(ns))
            {
                string[] strs = value.Split(ns);
                if (strs[0].Contains("/"))
                {
                    string[] vs = strs[0].Split(Environment.NewLine);
                    string last = vs[vs.Length - 1];
                    if (last.Contains("//"))
                    {
                        return false;
                    }

                    if (last.Contains("/*") && !last.Contains("*/"))
                    {
                        return false;
                    }

                    return true;
                }

                return true;
            }

            return false;
        }
        /// <summary>
        /// 是否已有命名空间
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool IsHaveNamespace(params string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (IsNamespaceLine(lines[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否是定义类型的行
        /// <para>ps：class、struct、interface、enum</para>
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsTypeLine(string line)
        {
            if (line.ContainsOne("class", "struct", "interface", "enum"))
            {
                string[] split = null;

                if (line.Contains("//"))
                {
                    line = line.Split("//")[0];
                }
                //else if (line.Contains("/*"))
                //{
                //    line = line.Split("/*")[0];
                //}

                if (line.Contains("class")) split = line.Split("class");
                else if (line.Contains("struct")) split = line.Split("struct");
                else if (line.Contains("interface")) split = line.Split("interface");
                else if (line.Contains("enum")) split = line.Split("enum");


                if (split == null)
                {
                    return false;
                }

                if (split.Length <= 1)
                {
                    return false;
                }

                if (split[0].ContainsOne("//", "/*"))
                {
                    return false;
                }

                return true;
            }

            return false;
        }
        /// <summary>
        /// 是否是定义类的行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsClassLine(string line)
        {
            return CheckLine(line, "class");
        }
        /// <summary>
        /// 是否是定义结构的行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsStructLine(string line)
        {
            return CheckLine(line, "struct");
        }
        /// <summary>
        /// 是否是定义接口的行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsInterfaceLine(string line)
        {
            return CheckLine(line, "interface");
        }
        /// <summary>
        /// 是否是定义枚举的行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsEnumLine(string line)
        {
            return CheckLine(line, "enum");
        }

        /// <summary>
        /// 是否是添加引用的行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsUsingLine(string line)
        {
            return CheckLine(line, "using");
        }

        /// <summary>
        /// 检查有效行
        /// </summary>
        /// <returns></returns>
        public static bool CheckLine(string line, string typeName)
        {
            if (line.Contains(typeName))
            {
                string[] split = line.Split(typeName);

                if (split.Length <= 1) return false;
                if (split[0].ContainsOne("//", "/*"))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否是定义特性的行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsAttributeLine(string line)
        {
            line = line.Remove(" ", "\t", "\r");
            if (line[0] == '[')
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否是注释行
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsCommentsLine(string line)
        {
            line = line.Remove(" ", "\t", "\r");
            if (line[0] == '/' || line[0] == '*')
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否区域注释行
        /// <para>例：</para>
        /// <code>
        /// /* 开始区域注释
        ///  * 这是注释行
        ///    这行也是注释，在一个注释区域内，前方不需要带任何注释表示符： / 或 * 
        ///    在 开始标识符 到 结束标识符 之间的任何东西都将视为注释
        /// // 这也是注释 */ 结束区域注释
        /// </code>
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsAreaCommentsLine(string line)
        {
            line = line.Remove(" ", "\t", "\r");

            if (line.StartsWith("/*") || line[0] == '/' || line[0] == '*')// 开始行
            {
                return true;
            }
            if (IsAreaCommentsEndLine(line))// 结束行
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 是否是区域注释开始行
        /// <para>注意：此方法只判断单行，整体的区域是多行，需自行处理结束</para>
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool IsAreaCommentsStartLine(string line)
        {
            return line.Contains("/*");// 开始行
        }
        /// <summary>
        /// 是否是区域注释结束行
        /// <para>注意：此方法只判断单行，整体的区域是多行，需自行处理开始</para>
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static bool IsAreaCommentsEndLine(string line)
        {
            return line.Contains("*/");// 结束行
        }



        /// <summary>
        /// 获取命名空间名称
        /// </summary>
        /// <param name="line">包含命名空间的行</param>
        /// <returns></returns>
        public static string GetNamespaceValue(string line)
        {
            if (!IsNamespaceLine(line)) return null;

            string nsValue = "";// 修改后的命名空间名称

            string[] lines = line.Split(ns);
            string[] strs = null;

            // 得到命名空间关键字 namespace 两端的值
            string Front = lines[0];
            string queen = lines[1];

            char sign = '/';        // 拆分符
            if (queen.ContainsAll("/", "{"))
            {

                for (int i = 0; i < queen.Length; i++)
                {
                    if (queen[i] == '/')
                    {
                        sign = '/';
                        break;
                    }
                    else if (queen[i] == '{')
                    {
                        sign = '{';
                        break;
                    }
                }

                strs = queen.Split(sign);
            }
            else if (queen.Contains("{"))
            {
                strs = queen.Split("{");

                sign = '{';
            }
            else if (queen.Contains("/"))
            {
                strs = queen.Split("/");

                sign = '/';
            }
            else
            {
                nsValue = queen.RemoveEmpty();
            }

            if (strs != null)
            {
                nsValue = strs[0].RemoveEmpty();
            }

            return nsValue;
        }
        /// <summary>
        /// 修改命名空间
        /// </summary>
        /// <param name="line">包含命名空间的行</param>
        /// <param name="nsValue">要修改成命名空间名称</param>
        /// <returns></returns>
        public static string NamespaceModify(string line, string nsValue)
        {
            string nm = line;
            if (!IsNamespaceLine(line)) return nm;

            string nsValueNew = string.Format(" {0} ", nsValue);// 修改后的命名空间名称

            string[] lines = line.Split(ns);
            string[] strs = null;

            // 得到命名空间关键字 namespace 两端的值
            string Front = lines[0];
            string queen = lines[1];

            string newline = Environment.NewLine;// 换行
            char sign = '/';        // 拆分符
            if (queen.ContainsAll("/", "{"))
            {

                for (int i = 0; i < queen.Length; i++)
                {
                    if (queen[i] == '/')
                    {
                        sign = '/';
                        newline = "";
                        break;
                    }
                    else if (queen[i] == '{')
                    {
                        sign = '{';
                        break;
                    }
                }

                strs = queen.Split(sign);
                nsValueNew += newline;
            }
            else if (queen.Contains("{"))
            {
                strs = queen.Split("{");
                nsValueNew += newline;

                sign = '{';
            }
            else if (queen.Contains("/"))
            {
                strs = queen.Split("/");
                //nsValueNew += newline;

                sign = '/';
            }
            else
            {
                //nsValueNew += newline;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(Front).Append(ns).Append(nsValueNew);

            if (strs != null)
            {
                for (int i = 1; i < strs.Length; i++)
                {
                    sb.Append(sign).Append(strs[i]);
                }
            }
            else
            {
                //sb.Append("\r\n");
            }

            nm = sb.ToString();

            return nm;
        }
        /// <summary>
        /// 修改命名空间
        /// </summary>
        /// <param name="line">包含命名空间的行</param>
        /// <param name="nsValue">要修改成命名空间名称</param>
        /// <returns></returns>
        public static string NamespaceModify(string nsValue, params string[] lines)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = NamespaceModify(lines[i], nsValue);
                sb.Append(lines[i]).Append(Environment.NewLine);
            }

            return sb.ToString();
        }


        /// <summary>
        /// 添加命名空间
        /// </summary>
        /// <param name="values">代码</param>
        /// <param name="nsValue">命名空间值</param>
        /// <returns></returns>
        public static string NamespaceAdd(string nsValue, string codeContext)
        {
            return NamespaceAdd(nsValue, codeContext.Split(Environment.NewLine));
        }
        /// <summary>
        /// 添加命名空间
        /// </summary>
        /// <param name="values">包含完整代码内容的数组</param>
        /// <param name="nsValue">命名空间值</param>
        /// <returns></returns>
        public static string NamespaceAdd(string nsValue, params string[] lines)
        {
            StringBuilder sb = new StringBuilder();


            if (!IsHaveNamespace(lines))
            {
                int start = 0;

                // 检查跳过行
                bool inCommentsArea = false;        // 在注释区域中
                bool startCheckComments = false;    // 开始检查注释
                bool isInterpretComments = false;   // 是否是解释类型的注释
                //bool isSkip = false;    // 是否处于跳过阶段
                int startSkip = 0;      // 记录开始跳过行的索引
                //int addNameLine = 0;    // 添加 命名空间 的行
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        //if (!inCommentsArea)
                        //{
                        //    startSkip = i + 1;
                        //}
                        continue;// 跳过空行
                    }

                    // 检查注释，例：
                    /* 此行开始注释
                     * 
                     * 
                    
                    需要检查类似这样的注释块

                    //测试 */ //此行结束注释
                    if (inCommentsArea)// 处于区域注释
                    {
                        startCheckComments = true;
                        if (IsAreaCommentsEndLine(line))// 检查注释结束
                        {
                            inCommentsArea = false;
                            startCheckComments = false;
                        }
                        continue;
                    }
                    if (IsCommentsLine(line))
                    {
                        if (startCheckComments)
                        {
                            continue;// 已经处于注释则跳过
                        }

                        startCheckComments = true;// 开始注释
                        //isSkip = true;

                        if (IsAreaCommentsLine(line) && IsAreaCommentsStartLine(line))
                            inCommentsArea = true;
                        startSkip = i;// 添加命名空间时跳过行的起始点

                        continue;
                    }
                    else
                    {
                        startCheckComments = false;
                    }

                    if (IsAttributeLine(line))
                    {
                        continue;
                    }

                    if (IsUsingLine(line))
                    {
                        startSkip = i + 1;
                        //isSkip = false;
                        continue;
                    }


                    if (IsTypeLine(line))
                    {
                        //addNameLine = startSkip;
                        break;
                    }

                    // 其他位置条件
                    //startSkip = i;
                }

                string s = "";
                for (int i = start; i < lines.Length; i++)
                {
                    if (startSkip == i)
                    {
                        s = "\t";
                        sb.Append(Environment.NewLine).Append("namespace ").Append(nsValue).Append("\r\n{");
                        sb.Append("\r\n\t").Append(lines[i]);
                        continue;
                    }
                    sb.Append(Environment.NewLine).Append(s).Append(lines[i]);
                }
                sb.Append(Environment.NewLine).Append("}");
            }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    sb.Append(lines[i]).Append(Environment.NewLine);
                }
            }

            return sb.ToString();
        }
    }
}