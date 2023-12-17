using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Prob
{
    /// <summary>
    /// 容器基类
    /// </summary>
    public abstract class BaseContainer<TInclude, TProbBranch, TProbItem> :
        IContainer<TInclude, TProbBranch, TProbItem>
        where TProbBranch : BaseProbBranch<TInclude, TProbBranch, TProbItem>
        where TProbItem : BaseProbItem<TInclude, TProbBranch, TProbItem>
    {
        [SerializeField]
        protected string containerName = "Node";

        [Header("ReadOnly")]
        [SerializeField]
        protected List<TProbItem> items = new List<TProbItem>();
        [SerializeField]
        protected List<TProbBranch> sons = new List<TProbBranch>();

        public List<TProbItem> Items { get => items; set => items = value; }
        public List<TProbBranch> Sons { get => sons; set => sons = value; }
        public string ContainerName { get => containerName; set => containerName = value; }

        public bool AddSon(TProbBranch son)
        {
            // 不可添加同名分支
            if (Sons.Find((tpb) => { return tpb.ContainerName == son.ContainerName; }) != null)
                return false;

            bool add = Sons.PAddTryContains(son);
            if (add)
            {
                son.OwnerContainer = this;
                //son.ContainerVariable = this;
            }
            return add;
        }
        /// <summary>
        /// 添加分支
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="provValue"></param>
        /// <returns></returns>
        public TProbBranch AddSon(string containerName, float provValue)
        {
            TProbBranch son = Activator.CreateInstance<TProbBranch>();
            son.ContainerName = containerName;
            son.ProbValue = provValue;
            son = AddSon(son) ? son : null;
            return son;
        }
        #region 
        ///// <summary>
        ///// 添加分支
        ///// </summary>
        ///// <typeparam name="TBranch"></typeparam>
        ///// <param name="containerName"></param>
        ///// <param name="provValue"></param>
        ///// <returns></returns>
        //public TBranch AddSon<TBranch>(string containerName, float provValue)
        //    where TBranch : TProbBranch, new()
        //{
        //    TBranch son = new TBranch();
        //    son.ContainerName = containerName;
        //    son.ProbValue = provValue;
        //    son = AddSon(son) ? son : null;
        //    return son;
        //} 
        #endregion
        public bool AddItem(TProbItem type)
        {
            bool add = Items.PAddTryContains(type);
            if (add)
            {
                type.OwnerContainer = this;
                //type.ContainerVariable = this;
            }
            return add;
        }
        /// <summary>
        /// 添加概率项
        /// </summary>
        /// <param name="probValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TProbItem AddItem(float provValue, TInclude value)
        {
            TProbItem item = Activator.CreateInstance<TProbItem>();
            item.TValue = value;
            item.ProbValue = provValue;
            item = AddItem(item) ? item : null;
            return item;
        }
        #region 
        ///// <summary>
        ///// 添加概率项
        ///// </summary>
        ///// <typeparam name="TItem"></typeparam>
        ///// <param name="provValue"></param>
        ///// <param name="value"></param>
        ///// <returns></returns>
        //public TItem AddItems<TItem>(float provValue, TInclude value)
        //    where TItem : TProbItem, new()
        //{
        //    TItem item = new TItem();
        //    item.TValue = value;
        //    item.ProbValue = provValue;
        //    item = AddItems(item) ? item : null;
        //    return item;
        //} 
        #endregion


        public TProbItem GetRandmoDirectlyProbItem(bool ignoreEnable = false)
        {
            TProbItem p = Items.RandmoProb(ignoreEnable);
            return p;
        }

        public TProbBranch GetRandmoDirectlySon(bool ignoreEnable = false)
        {
            return Sons.RandmoProb(ignoreEnable);
        }

        public TProbItem GetRandmoDirectlySonProbItem(bool ignoreEnable = false)
        {
            TProbBranch p = GetRandmoDirectlySon(ignoreEnable);
            return p?.GetRandmoDirectlyProbItem(ignoreEnable);
        }

        public TProbBranch GetRandmoSon(bool ignoreEnable = false)
        {
            var b = GetRandmoDirectlySon(ignoreEnable);
            if (b != null)
            {
                bool next = Random.Range(0, 2) == 0 ? false : true;
                if(next) b = b.GetRandmoSon(ignoreEnable);
            }
            return b;
            //return GetRandmoDirectlySon(ignoreEnable)?.GetRandmoSon(ignoreEnable);
        }

        /// <summary>
        /// 随机获取分支中概率项的结果
        /// </summary>
        /// <param name="ignoreEnable"></param>
        /// <returns></returns>
        public virtual TInclude GetSonInclude(bool ignoreEnable = false)
        {
            TProbItem p = GetRandmoDirectlySonProbItem(ignoreEnable);
            return p == null ? default : p.TValue;
        }

        public float GetItemsRealProb(TInclude include, bool ignoreEnable = false)
        {
            TProbItem probItem = FindProbItem(include);
            return probItem == null ? 0 : probItem.RealProb();
        }

        /// <summary>
        /// 如要读取保存后的数据，请在读取完之后调用此函数
        /// </summary>
        public virtual void OnReadAfter()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] != null)
                {
                    Items[i].OwnerContainer = this;
                    //Items[i].ContainerVariable = this;
                }
            }

            for (int i = 0; i < Sons.Count; i++)
            {
                if (Sons[i] != null)
                {
                    Sons[i].OwnerContainer = this;
                    //Sons[i].ContainerVariable = this;
                    Sons[i].OnReadAfter();
                }
            }
        }

        public TProbItem FindProbItem(TInclude include)
        {
            TProbItem p = Items.Find((tpi) => { return tpi.TValue.Equals(include); });

            if (p == null)
                for (int i = 0; i < Sons.Count; i++)
                {
                    p = Sons[i].FindProbItem(include);
                    if (p != null) break;
                }

            return p;
        }
        public TProbBranch FindNode(Func<TProbBranch, bool> condition)
        {
            TProbBranch branch = Sons.Find((b) => { return (bool)(condition?.Invoke(b)); });
            for (int i = 0; i < Sons.Count; i++)
            {
                branch = Sons[i].FindNode(condition);
                if (branch != null) break;
            }

            return branch;
        }
        /// <summary>
        /// 查找节点
        /// <para>path - 相对路径，格式：Node/a/b/c，其中 Node、a、b 等都是 <see cref="ContainerName"/></para>
        /// </summary>
        /// <param name="path">相对路径，格式：Node/a/b/c，其中 Node、a、b 等都是 <see cref="ContainerName"/></param>
        /// <returns></returns>
        public virtual TProbBranch FindNode(string path)
        {
            string[] strs = path.Split('/');
            TProbBranch branch = this as TProbBranch;

            int start = strs[0] == ContainerName ? 1 : 0;// 排除自己

            for (int i = 0; i < strs.Length; i++)
            {
                string name = strs[i];
                branch = branch.FindNode((TProbBranch item) =>
                {
                    return item.ContainerName == name;
                });
            }
            return branch;
        }

        public void Clear()
        {
            Items.Clear();
            Sons.Clear();
        }
    }
}
