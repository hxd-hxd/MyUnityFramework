
namespace Prob
{
    /// <summary>
    /// 概率类接口
    /// </summary>
    public interface IProb : IProbCalculate
        //, IPutIn
    {
        /// <summary>
        /// 启用
        /// </summary>
        bool Enable { get; set; }
        /// <summary>
        /// 概率值
        /// </summary>
        float ProbValue { get; set; }

        ///// <summary>
        ///// 容器属性
        ///// </summary>
        //IContainerVariable ContainerVariable { get; set; }

        /// <summary>
        /// 获取完整节点
        /// </summary>
        string CompleteNode();
    }
}