using System;
using System.Collections.Generic;

namespace Prob
{
    /// <summary>
    /// 分支
    /// </summary>
    [System.Serializable]
    public  class ProbBranch : BaseProbBranch<string, ProbBranch, ProbItem>
    {
        public ProbBranch() : base()
        {
        }

        public ProbBranch(float probValue) : base(probValue)
        {
        }

        public ProbBranch(IContainer<string, ProbBranch, ProbItem> ownerContainer) : base(ownerContainer)
        {
        }

        public ProbBranch(string containerName, float probValue) : base(containerName, probValue)
        {
        }

        public ProbBranch(string containerName, float probValue, List<ProbItem> types, List<ProbBranch> sonProbs) : base(containerName, probValue, types, sonProbs)
        {
        }
    }
    
    /// <summary>
    /// 分支
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public  class ProbBranch<T> : BaseProbBranch<T, ProbBranch<T>, ProbItem<T>>
    {
        public ProbBranch() : base()
        {
        }

        public ProbBranch(float probValue) : base(probValue)
        {
        }

        public ProbBranch(IContainer<T, ProbBranch<T>, ProbItem<T>> ownerContainer) : base(ownerContainer)
        {
        }

        public ProbBranch(string containerName, float probValue) : base(containerName, probValue)
        {
        }

        public ProbBranch(string containerName, float probValue, List<ProbItem<T>> types, List<ProbBranch<T>> sonProbs) : base(containerName, probValue, types, sonProbs)
        {
        }
    }

}