using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Framework
{
	/// <summary>
	/// 对象方法扩充
	/// </summary>
	public static class ObjectExtend
	{
	    /// <summary>
	    /// 克隆指定类数据到新组件上
	    /// </summary>
	    /// <typeparam name="TOut">指定要克隆的组件</typeparam>
	    /// <param name="tIn">原对象</param>
	    /// <param name="target">新组件要附加的目标对象</param>
	    /// <param name="removeOld">是否移除原对象</param>
	    public static TOut CloneComponent<TOut>(UnityEngine.Object tIn, GameObject target, bool removeOld = false) where TOut : Component
	    {
	        // 构建临时对象，必须加这步
	        GameObject sbcGO = new GameObject("[temporary]");
	        TOut tout = sbcGO.AddComponent<TOut>();
	
	        Copy(tIn, tout);// 拷贝到临时
	
	        if (removeOld)  // 移除原对象（如果原对象是组件，新组件和原组件不能共存时，则一定要移除）
	            Object.DestroyImmediate(tIn);

			TOut newTOut = target.AddComponent<TOut>();
	        Copy(tout, newTOut);// 从临时拷贝到目标
	
	        Object.DestroyImmediate(sbcGO);// 销毁临时
			return newTOut;
	    }
		
	    /// <summary>
	    /// 拷贝数据到指定类型
	    /// </summary>
	    /// <typeparam name="TOut"></typeparam>
	    /// <param name="tIn"></param>
	    /// <returns>指定类型新实例</returns>
	    public static TOut CopyToNew<TOut>(object tIn)
	    {
	        TOut tOut = Activator.CreateInstance<TOut>();
	        Copy(tIn, tOut);
	        return tOut;
	    }
	
	    public static void Copy(object tIn, object tOut)
	    {
	        var tInType = tIn.GetType();
	        foreach (var itemOut in tOut.GetType().GetFields())
	        {
	            var itemIn = tInType.GetField(itemOut.Name); ;
	            if (itemIn != null)
	            {
	                itemOut.SetValue(tOut, itemIn.GetValue(tIn));
	            }
	        }
	    }
	
	
	    #region 扩展方法
	
	
	
	    #endregion
	}
}