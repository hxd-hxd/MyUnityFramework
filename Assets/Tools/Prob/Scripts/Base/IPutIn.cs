
namespace Prob
{
    /// <summary>
    /// 容器可容物接口
    /// </summary>
    /// <typeparam name="TInclude">包含类型</typeparam>
    /// <typeparam name="TProbBranch">实现的分支类型</typeparam>
    /// <typeparam name="TProbItem">实现的概率类型</typeparam>
    public interface IPutIn
    {
        /// <summary>
        /// 所属容器
        /// </summary>
        IContainer OwnerContainer { get; set; }
    }
}