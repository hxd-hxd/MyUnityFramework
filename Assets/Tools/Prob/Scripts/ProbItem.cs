
namespace Prob
{
    /// <summary>
    /// 概率类
    /// </summary>
    [System.Serializable]
    public class ProbItem : BaseProbItem<string, ProbBranch, ProbItem>
    {
        public ProbItem() : base()
        {
        }

        public ProbItem(float probValue) : base(probValue)
        {
        }

        public ProbItem(float probValue, string t) : base(probValue, t)
        {
        }


    }
    
    /// <summary>
    /// 概率类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class ProbItem<T> : BaseProbItem<T, ProbBranch<T>, ProbItem<T>>
    {
        public ProbItem() : base()
        {
        }

        public ProbItem(float probValue) : base(probValue)
        {
        }

        public ProbItem(float probValue, T t) : base(probValue, t)
        {
        }


    }

}