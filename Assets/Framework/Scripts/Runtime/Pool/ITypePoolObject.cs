// -------------------------
// 创建日期：2023/10/19 1:41:25
// -------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 类型池对象接口
    /// <para>一般用于，在返回对象池时，执行必要的清理操作等。推荐显式实现。</para>
    /// </summary>
    public interface ITypePoolObject
    {
        // TODO：增加一个是否在池子里的标识
        //bool inPool { get; set; }

        /// <summary>
        /// 清除
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 类型池对象初始化接口
    /// <para>一般用于，在对象池取出时，执行必要的初始操作等，像 构造函数 和 Start、Awake 等生命周期一样。推荐显式实现。</para>
    /// </summary>
    public interface ITypePoolObjectInit
    {
        /// <summary>
        /// 初始
        /// </summary>
        void Init();
    }

}