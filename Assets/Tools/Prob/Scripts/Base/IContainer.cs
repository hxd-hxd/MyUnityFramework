using System.Collections;
using System.Collections.Generic;

namespace Prob
{
    /// <summary>
    /// 容器接口
    /// </summary>
    /// <typeparam name="TInclude">包含类型</typeparam>
    /// <typeparam name="TProbBranch">实现的分支类型</typeparam>
    /// <typeparam name="TProbItem">实现的概率类型</typeparam>
    public interface IContainer<TInclude, TProbBranch, TProbItem> : 
        IContainerVariable
        //, IContainer
        where TProbBranch : IBranch<TInclude, TProbBranch, TProbItem>
        where TProbItem : IProb
    {

        /// <summary>
        /// 此概率所包含的直属列表
        /// </summary>
        List<TProbItem> Items { get; set; }
        /// <summary>
        /// 此概率所包含的分支概率列表
        /// </summary>
        List<TProbBranch> Sons { get; set; }

        /// <summary>
        /// 添加一个分支
        /// <para>ps：不可添加同名分支</para>
        /// </summary>
        bool AddSon(TProbBranch son);
        /// <summary>
        /// 添加直属
        /// </summary>
        bool AddItem(TProbItem item);

        /// <summary>
        /// 随机获取一个直属概率项
        /// </summary>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        TProbItem GetRandmoDirectlyProbItem(bool ignoreEnable = false);
        /// <summary>
        /// 随机获取一个直属分支
        /// </summary>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        TProbBranch GetRandmoDirectlySon(bool ignoreEnable = false);
        /// <summary>
        /// 随机获取一个直属分支的直属概率项
        /// </summary>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        TProbItem GetRandmoDirectlySonProbItem(bool ignoreEnable = false);
        /// <summary>
        /// 随机获取一个分支，在所有下级分支中
        /// </summary>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        TProbBranch GetRandmoSon(bool ignoreEnable = false);

        /// <summary>
        /// 查找一个符合条件的 概率项
        /// <para>ps：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        TProbItem FindProbItem(TInclude include);
        /// <summary>
        /// 查找一个符合条件的 节点
        /// <para>ps：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        TProbBranch FindNode(System.Func<TProbBranch, bool> condition);

        /// <summary>
        /// 获取对应直属真实概率
        /// <para>ps：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        /// <param name="include"></param>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        float GetItemsRealProb(TInclude include, bool ignoreEnable = false);

        /// <summary>
        /// 清空此生成器
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// 容器接口
    /// </summary>
    public interface IContainer : IContainerVariable
    {

        /// <summary>
        /// 此概率所包含的直属列表
        /// </summary>
        List<IProb> Items { get; set; }
        /// <summary>
        /// 此概率所包含的分支概率列表
        /// </summary>
        List<IBranch> Sons { get; set; }

        /// <summary>
        /// 添加一个分支
        /// <para>ps：不可添加同名分支</para>
        /// </summary>
        bool AddSon(IBranch son);
        /// <summary>
        /// 添加直属
        /// </summary>
        bool AddItem(IProb item);

        /// <summary>
        /// 随机获取一个直属概率项
        /// </summary>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        IProb GetRandmoDirectlyProbItem(bool ignoreEnable = false);
        /// <summary>
        /// 随机获取一个直属分支
        /// </summary>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        IBranch GetRandmoDirectlySon(bool ignoreEnable = false);
        /// <summary>
        /// 随机获取一个直属分支的直属概率项
        /// </summary>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        IProb GetRandmoDirectlySonProbItem(bool ignoreEnable = false);
        /// <summary>
        /// 随机获取一个分支，在所有下级分支中
        /// </summary>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        IBranch GetRandmoSon(bool ignoreEnable = false);

        /// <summary>
        /// 查找一个符合条件的 概率项
        /// <para>ps：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        IProb FindProbItem(object include);
        /// <summary>
        /// 查找一个符合条件的 节点
        /// <para>ps：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        IBranch FindNode(System.Func<IBranch, bool> condition);

        /// <summary>
        /// 获取对应直属真实概率
        /// <para>ps：除非你确定其所有分支都没有相同项，否则不同分支有相同的，只会找到第一个</para>
        /// </summary>
        /// <param name="include"></param>
        /// <param name="ignoreEnable">忽视启用的影响</param>
        /// <returns></returns>
        float GetItemsRealProb(object include, bool ignoreEnable = false);

        /// <summary>
        /// 清空此生成器
        /// </summary>
        void Clear();
    }
}
