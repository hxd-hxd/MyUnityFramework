using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prob
{
    /// <summary>
    /// 分支
    /// </summary>
    /// <typeparam name="TInclude">包含类型</typeparam>
    /// <typeparam name="TProbBranch">实现的分支类型</typeparam>
    /// <typeparam name="TProbItem">实现的概率类型</typeparam>
    public abstract class BaseProbBranch<TInclude, TProbBranch, TProbItem> :
        BaseContainer<TInclude, TProbBranch, TProbItem>,
        IBranch<TInclude, TProbBranch, TProbItem>
        where TProbBranch : BaseProbBranch<TInclude, TProbBranch, TProbItem>
        where TProbItem : BaseProbItem<TInclude, TProbBranch, TProbItem>
    {
        [SerializeField]
        protected float probValue = -1;
        [SerializeField]
        protected bool enable = true;
        [SerializeField]
        protected bool allProb;

        //[SerializeField]
        //private IContainerVariable containerVariable;
        [SerializeField]
        protected IContainer<TInclude, TProbBranch, TProbItem> ownerContainer;

        public float ProbValue { get => probValue; set => probValue = value; }
        public bool Enable
        {
            get
            {
                return enable;
            }
            set => enable = value;
        }
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
                return f(this as TProbBranch);
            }
        }
        public string NodePath
        {
            get
            {
                string path = ContainerName;
                //Action<IContainer<TInclude, TProbBranch, TProbItem>> f = null;
                //f = (container) =>
                //{
                //    path = $"{container.ContainerName}/{path}";
                //    if (container is TProbBranch branch) f(branch.OwnerContainer);
                //};
                //f(OwnerContainer);

                IContainer<TInclude, TProbBranch, TProbItem> container = OwnerContainer;
                while (true)
                {
                    path = $"{container.ContainerName}/{path}";
                    if (container is TProbBranch branch) container = branch.OwnerContainer;
                    else break;
                }
                return path;
            }
        }

        #region 构造
        public BaseProbBranch()
        {
            Init();
        }
        public BaseProbBranch(IContainer<TInclude, TProbBranch, TProbItem> ownerContainer)
        {
            Init();

            this.OwnerContainer = ownerContainer;
        }

        public BaseProbBranch(float probValue)
        {
            Init();

            this.ProbValue = probValue;
        }
        public BaseProbBranch(string ContainerName, float probValue)
        {
            Init();

            this.ContainerName = ContainerName;
            this.ProbValue = probValue;
        }
        public BaseProbBranch(string ContainerName, float probValue, List<TProbItem> types, List<TProbBranch> sonProbs)
        {
            Init();

            this.ContainerName = ContainerName;
            this.ProbValue = probValue;
            this.Items = types;
            this.Sons = sonProbs;
        }

        protected virtual void Init()
        {
            Enable = true;
            ProbValue = -1;
            ContainerName = default(string);
            Items = new List<TProbItem>();
            Sons = new List<TProbBranch>();
        }
        #endregion

        public float RealProb()
        {
            if (ownerContainer == null) return ProbValue;

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
                float branchSumProb = branch.Sons.GetSumProbValue();// 上级分支的总概率
                if (AllProb)
                    branchSumProb += branch.Items.GetSumProbValue();// 是否包含所有概率项

                float prob100 = ProbValue.FormatProb1(branchSumProb);

                // 真实概率
                prob = prob * prob100;
            }
            else
            {
                // 这里直接计算
                // 计算百分比概率
                float branchSumProb = ownerContainer.Sons.GetSumProbValue();// 上级分支的总概率
                if (AllProb)
                    branchSumProb += branch.Items.GetSumProbValue();// 是否包含所有概率项

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