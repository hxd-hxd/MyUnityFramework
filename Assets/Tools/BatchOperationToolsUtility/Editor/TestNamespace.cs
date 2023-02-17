//// -------------------------
//// 创建日期：2021/8/13 17:15:58
//// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;


namespace BatchOperationToolsUtility
{
    // 添加命名空间测试用例
    public class TestNamespace
    {
        [MenuItem("测试命名空间/添加")]
        static void TestMenu()
        {
            string code =
@"
// -------------------------
// 创建日期：2021/8/13 17:15:58
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Text;

/*
 *

注释666

//加个皮 */

/// <summary>
/// uishdia
/// </summary>

public class Test
{
    
}
";

            string code1 =
@"

// -------------------------
// 创建日期：2022/8/31 16:42:58
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public static class NewStaticScript
{

}

";

            Debug.Log(BOTUtility.NamespaceAdd("Test", code));
            //Debug.Log(BOTUtility.NamespaceAdd("Test", code1));
        }

        //        [MenuItem("Assets/查看文件类型")]
        //        public static void TestAsset()
        //        {
        //            TestAsset(Selection.activeObject);
        //        }
        //        public static void GetAllAsset(DefaultAsset obj)
        //        {
        //            if (obj)
        //            {
        //                string path = AssetDatabase.GetAssetPath(obj);
        //                string[] dirs = Directory.GetDirectories(path.GetFullPath());
        //                for (int i = 0; i < dirs.Length; i++)
        //                {
        //                    Debug.Log("文件夹：" + dirs[i]);

        //                    GetAllAsset(AssetDatabase.LoadAssetAtPath(dirs[i].CutOutAssetPath(), typeof(DefaultAsset)) as DefaultAsset);
        //                }
        //                string[] files = Directory.GetFiles(path.GetFullPath()).ExcludeType("meta");
        //                for (int i = 0; i < files.Length; i++)
        //                {
        //                    Debug.Log("文件：" + files[i]);
        //                }

        //                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        //                for (int i = 0; i < objs.Length; i++)
        //                {
        //                    TestAsset(objs[i]);
        //                }

        //            }
        //        }
        //        public static void TestAsset(UnityEngine.Object obj)
        //        {
        //            if (!obj)
        //            {
        //                Debug.Log("Null");
        //                return;
        //            }

        //            if (obj is DefaultAsset)
        //            {
        //                Debug.Log(string.Format("文件夹：{0}\t{1}", obj.GetType().FullName, AssetDatabase.GetAssetPath(obj)));
        //                GetAllAsset(obj as DefaultAsset);
        //            }
        //            else
        //            {
        //                Debug.Log(string.Format("文件：{0}\t{1}", obj.GetType().FullName, AssetDatabase.GetAssetPath(obj)));
        //            }
        //        }

        //        [MenuItem("TestMenu/拆分转换float")]
        //        public static void TestStrigSpilt()
        //        {
        //            string str = "0.4 :0.83     :1.2:1.55,3.4";
        //            string str1 = "";

        //            List<List<float>> vs = str1.ToFloatTwoList(',', ':');

        //            Debug.Log(vs.Count);
        //        }

        //        [MenuItem("TestMenu/list 容量")]
        //        public static void TestList()
        //        {
        //            List<float> vs = new List<float>(2);
        //            Debug.Log(vs.Count);
        //            for (int i = 0; i < vs.Count; i++)
        //            {
        //                Debug.Log(vs[i]);
        //            }
        //        }
        //        [MenuItem("TestMenu/list 排序")]
        //        public static void TestListSort()
        //        {
        //            List<float> vs = new List<float>() { 5, 3, 666, 520, 7.8f };

        //            // 倒序
        //            vs.Sort((v1, v2) => { return (int)(v2 - v1); });
        //            vs.LogAll();

        //            // 正序
        //            vs.Sort();
        //            vs.LogAll();

        //        }

        //        [MenuItem("TestMenu/TestStrigBuilder")]
        //        public static void TestStrigBuilder()
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            sb.Append("1314");
        //            sb.Append("2333");
        //            Debug.Log(sb.Length);
        //        }
        //        [MenuItem("TestMenu/using")]
        //        public static void Testusing()
        //        {
        //            Debug.Log(string.IsNullOrWhiteSpace("\t\r\n"));

        //            string us =
        //@"using System;

        //namespace BatchOperationToolsUtility
        //{
        //    public class Test
        //    {


        //    }
        //}";

        //            Debug.Log(us);

        //            //us = us.RemoveEmpty();
        //            //us = us.RemoveSpace();
        //            us = us.Remove("\n");

        //            string[] strs = us.Split("using");

        //            Debug.Log(strs.Length);
        //            Debug.Log(strs[0] == "");
        //            Debug.Log(strs[0] == null);

        //            for (int i = 0; i < strs.Length; i++)
        //            {
        //                Debug.Log(strs[i]);
        //            }
        //        }
        //        [MenuItem("TestMenu/StringSplit")]
        //        public static void TestStringSplit()
        //        {
        //            Debug.Log(string.IsNullOrWhiteSpace("\t\r\n"));

        //            string us = " class A";

        //            string[] strs = us.Split("using");

        //            Debug.Log(strs.Length);
        //            for (int i = 0; i < strs.Length; i++)
        //            {
        //                Debug.Log(strs[i]);
        //            }
        //        }
        //        [MenuItem("TestMenu/注释")]
        //        public static void TestZhuShi()
        //        {
        //            string us =
        //@"using System;

        //namespace BatchOperationToolsUtility
        //{

        //    /* 测试类 */ public class Test   /* 
        //                         * 测试类 */
        //    {


        //    }
        //}";

        //            /* 测试类 */
        //            Debug.Log(us); /* 测试类 */

        //            //us = us.RemoveEmpty();
        //            //us = us.RemoveSpace();
        //            us = us.Remove("\n");

        //            string[] strs = us.Split("using");

        //            Debug.Log(strs.Length);
        //            Debug.Log(strs[0] == "");
        //            Debug.Log(strs[0] == null);

        //            for (int i = 0; i < strs.Length; i++)
        //            {
        //                Debug.Log(strs[i]);
        //            }
        //        }

        //        [MenuItem("TestMenu/修改命名空间")]
        //        public static void TestModifyNamespace()
        //        {
        //            string us =
        //@"using System;

        // /* namespac */ // 就是皮    
        //namespace /* 测试类 */ BOTU
        //{

        //    /* 测试类 */ public class Test   /* 
        //                         * 测试类 */
        //    {


        //    }
        //}";

        //            String n = " namespace BatchOperationToolsUtility  /* 测试6 */   { // 666 }";
        //            String cla = " public class BBC  /* 测试类6 */   { // 666 }";
        //            String func = " public void BatchOperationToolsUtility  /* 测试6 */   { // 666 }";


        //            // 检查命名空间定义
        //            Debug.Log(BOTUtility.IsHaveNamespace(us));
        //            Debug.Log(BOTUtility.IsHaveNamespace(n));
        //            Debug.Log(BOTUtility.IsHaveNamespace(cla));
        //            Debug.Log(BOTUtility.IsHaveNamespace(func));

        //            // 获取命名空间名称
        //            Debug.Log(BOTUtility.GetNamespaceValue(us));
        //            Debug.Log(BOTUtility.GetNamespaceValue(n));
        //            Debug.Log(BOTUtility.GetNamespaceValue(cla));
        //            Debug.Log(BOTUtility.GetNamespaceValue(func));

        //            // 修改命名空间名称
        //            Debug.Log(BOTUtility.NamespaceModify(us, "Test"));
        //            Debug.Log(string.Format("修改前：{0}\r\n修改后：{1}", n, BOTUtility.NamespaceModify(n, "Test")));
        //            Debug.Log(BOTUtility.NamespaceModify(cla, "520"));
        //            Debug.Log(BOTUtility.NamespaceModify(func, "66666"));
        //        }
        //        [MenuItem("TestMenu/添加命名空间")]
        //        public static void TestAddNamespace()
        //        {
        //            string us =
        //@"using System;

        // /* namespac */ // 就是皮    
        //namespace /* 测试类 */ BOTU
        //{

        //    /* 测试类 */ public class Test   /* 
        //                         * 测试类 */
        //    {


        //    }
        //}";

        //            String n = " namespace BatchOperationToolsUtility  /* 测试6 */   { // 666 }";
        //            String cla = " public class BBC  /* 测试类6 */   { // 666 }";
        //            String func = " public void BatchOperationToolsUtility  /* 测试6 */   { // 666 }";


        //            // 检查命名空间定义
        //            Debug.Log(BOTUtility.IsHaveNamespace(us));
        //            Debug.Log(BOTUtility.IsHaveNamespace(n));
        //            Debug.Log(BOTUtility.IsHaveNamespace(cla));
        //            Debug.Log(BOTUtility.IsHaveNamespace(func));

        //            // 获取命名空间名称
        //            Debug.Log(BOTUtility.GetNamespaceValue(us));
        //            Debug.Log(BOTUtility.GetNamespaceValue(n));
        //            Debug.Log(BOTUtility.GetNamespaceValue(cla));
        //            Debug.Log(BOTUtility.GetNamespaceValue(func));

        //            // 修改命名空间名称
        //            Debug.Log(BOTUtility.NamespaceAdd("Test", us.Split("\r\n")));
        //            Debug.Log(string.Format("修改前：{0}\r\n修改后：{1}", n, BOTUtility.NamespaceAdd("Test", n)));
        //            Debug.Log(BOTUtility.NamespaceAdd("520", cla));
        //            Debug.Log(BOTUtility.NamespaceAdd("66666", func));
        //        }

        //        public void /* 测试类 */ Ttt

        //            (int jdkk
        //            )
        //        {

        //        }
    }
}