using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prob
{
    /// <summary>
    /// 容器变量接口
    /// </summary>
    public interface IContainerVariable
    {
        /// <summary>
        /// 容器名称
        /// </summary>
        string ContainerName { get; set; }
    }
}
