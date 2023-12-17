using UnityEngine;
using System;

namespace Prob
{
    /// <summary>
    /// 概率类
    /// </summary>
    public abstract class BaseProbItem<TInclude, TProbBranch, TProbItem> : IProb
        where TProbBranch : BaseProbBranch<TInclude, TProbBranch, TProbItem>
        where TProbItem : BaseProbItem<TInclude, TProbBranch, TProbItem>
    {
        [SerializeField]
        private TInclude tValue;
        [SerializeField]
        private float probValue = -1;

        [SerializeField]
        private bool enable = true;
        [SerializeField]
        private bool allProb;

        //[SerializeField]
        //private IContainerVariable containerVariable;
        [SerializeField]
        private IContainer<TInclude, TProbBranch, TProbItem> ownerContainer;

        public TInclude TValue { get => tValue; set => tValue = value; }
        public float ProbValue { get => probValue; set => probValue = value; }
        public bool Enable { get => enable; set => enable = value; }
        public bool AllProb { get => allProb; set => allProb = value; }

        public IContainer<TInclude, TProbBranch, TProbItem> OwnerContainer { get => ownerContainer; set => ownerContainer = value; }
        //public IContainerVariable ContainerVariable { get => containerVariable; set => containerVariable = value; }
        /// <summary>
        /// 根节点
        /// </summary>
        public IContainer<TInclude, TProbBranch, TProbItem> Root
        {
            get
            {
                Func<TProbBranch, IContainer<TInclude, TProbBranch, TProbItem>> f = null;
                f = (branch) =>
                {
                    var parent = branch?.OwnerContainer;
                    if (parent != null && parent is TProbBranch parentBranch)
                    {
                        parent = f(parentBranch);
                    }
                    return parent;
                };
                return f(OwnerContainer as TProbBranch);
            }
        }

        public BaseProbItem()
        {
            probValue = -1;
            enable = true;
            tValue = default(TInclude);
        }
        public BaseProbItem(float probValue)
        {
            enable = true;
            tValue = default(TInclude);
            this.probValue = probValue;
        }
        public BaseProbItem(float probValue, TInclude t)
        {
            enable = true;
            this.probValue = probValue;
            this.tValue = t;
        }

        public float RealProb()
        {
            if (OwnerContainer == null) return ProbValue;

            return RealProb(AllProb);
        }

        /// <summary>
        /// 真实概率（返回百分比值<para>例：概率是 20%，即返回 20</para>）
        /// <para>ps：计算公式：真实概率 = 上级真实概率 * 本级百分比转换概率</para>
        /// </summary>
        /// <returns>百分比值</returns>
        public float RealProb(bool AllProb)
        {
            if (OwnerContainer == null) return ProbValue;

            float prob = 0;
            // 所属者是否分支
            IBranch<TInclude, TProbBranch, TProbItem> branch = OwnerContainer as IBranch<TInclude, TProbBranch, TProbItem>;
            if (branch != null)
            {
                // 获取所属分支地真实概率
                prob = branch.RealProb();

                // 计算百分比概率
                float branchSumProb = branch.Items.GetSumProbValue();// 上级分支的总概率
                if (AllProb)
                    branchSumProb += branch.Sons.GetSumProbValue();// 是否包含所有概率项
                float prob100 = ProbValue.FormatProb1(branchSumProb);

                // 真实概率
                prob = prob * prob100;
            }
            else
            {
                // 这里直接计算
                // 计算百分比概率
                float branchSumProb = OwnerContainer.Items.GetSumProbValue();// 上级分支的总概率
                if (AllProb)
                    branchSumProb += branch.Sons.GetSumProbValue();// 是否包含所有概率项

                prob = ProbValue.FormatProb100(branchSumProb);// 真实概率
            }

            return prob;
        }

        public string CompleteNode()
        {
            string cn = "";
            if (OwnerContainer != null)
            {
                IBranch<TInclude, TProbBranch, TProbItem> branch = OwnerContainer as IBranch<TInclude, TProbBranch, TProbItem>;
                if (branch != null)
                {
                    cn = branch.CompleteNode() + "/" + OwnerContainer.ContainerName;
                }
                else
                {
                    cn = OwnerContainer.ContainerName;
                }
            }

            return cn;
        }
    }
}