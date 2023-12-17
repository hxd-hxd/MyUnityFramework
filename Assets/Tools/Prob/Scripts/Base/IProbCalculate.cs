
namespace Prob
{
    /// <summary>
    /// 概率计算接口
    /// </summary>
    public interface IProbCalculate
    {
        /// <summary>
        /// 是否在计算时包含所有概率项
        /// </summary>
        bool AllProb { get; set; }
        /// <summary>
        /// 真实概率（返回百分比值<para>例：概率是 20%，即返回 20</para>）
        /// <para>ps：计算公式：真实概率 = 上级真实概率 * 本级百分比转换概率</para>
        /// </summary>
        /// <returns>百分比值</returns>
        float RealProb();
    }
}