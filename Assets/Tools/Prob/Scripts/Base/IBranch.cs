
namespace Prob
{
    /// <summary>
    /// 分支接口
    /// </summary>
    /// <typeparam name="TInclude">包含类型</typeparam>
    /// <typeparam name="TProbBranch">实现的分支类型</typeparam>
    /// <typeparam name="TProbItem">实现的概率类型</typeparam>
    public interface IBranch<TInclude, TProbBranch, TProbItem> : 
        IProb
        , IContainer<TInclude, TProbBranch, TProbItem> 
        where TProbBranch : IBranch<TInclude, TProbBranch, TProbItem> 
        where TProbItem : IProb
    {
        /// <summary>
        /// 所属容器
        /// </summary>
        IContainer<TInclude, TProbBranch, TProbItem> OwnerContainer { get; set; }
    }

    /// <summary>
    /// 分支接口
    /// </summary>
    public interface IBranch
    {
        /// <summary>
        /// 所属容器
        /// </summary>
        IContainer OwnerContainer { get; set; }
    }
}