using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Prob
{
    public class TestProb : MonoBehaviour
    {
        public ProbDeviceDistributed<int> pddInt = new ProbDeviceDistributed<int>();

        public ProbDeviceDistributed pdd = new ProbDeviceDistributed();

        [TextArea(0, 10)]
        public string path;

        void Start()
        {
            pddInt.AddItem(10, 24);
            pddInt.AddItem(10, 2445);
            pddInt.AddItem(10, 2432);
            pddInt.AddItem(10, 543);
            pddInt.AddItem(10, 3);

            pdd.ContainerName = "概率生成器";

            ProbBranch ssr = new ProbBranch("SSR", 10);
            ssr.AddItem(new ProbItem(20, "蓝"));
            ssr.AddItem(new ProbItem(100, "红") { Enable = true });
            ssr.AddItem(new ProbItem(20, "黄"));
            ssr.AddItem(new ProbItem(20, "绿"));
            ssr.AddItem(new ProbItem(20, "能量"));
            ssr.AddItem(new ProbItem(20, "任意"));
            pdd.AddSon(ssr);

            ProbBranch ssrUp = new ProbBranch("SSR+", 100);
            ssrUp.AddItem(new ProbItem(20, "蓝<境>"));
            ssrUp.AddItem(new ProbItem(100, "红<境>") { Enable = true });
            ssrUp.AddItem(new ProbItem(20, "黄<境>"));
            ssrUp.AddItem(new ProbItem(20, "绿<境>"));
            ssrUp.AddItem(new ProbItem(20, "能量<境>"));
            ssrUp.AddItem(new ProbItem(20, "任意<境>"));
            ssr.AddSon(ssrUp);

            ProbBranch sp = new ProbBranch("SP", 40);
            sp.AddItem(new ProbItem(20, "蓝Sp"));
            sp.AddItem(new ProbItem(100, "红Sp") { Enable = true });
            sp.AddItem(new ProbItem(20, "黄Sp"));
            sp.AddItem(new ProbItem(20, "绿Sp"));
            sp.AddItem(new ProbItem(20, "能量Sp"));
            sp.AddItem(new ProbItem(20, "任意Sp"));
            pdd.AddSon(sp);

            ProbBranch n = pdd.AddSon("N", 50);
            n.AddItem(10, "锅");
            n.AddItem(15, "碗");
            n.AddItem(12, "瓢");
            n.AddItem(18, "盆");
            n.AddItem(45, "桌");

            var pi = pdd.FindProbItem("任意<境>");
            if (pi != null)
            {
                //Debug.Log();
            }

            path = Application.dataPath + "/" + pdd.GetType().FullName + ".json";
        }

        public void SaveJson()
        {
            Debug.Log(path);
            Debug.Log(Path.GetFileName(path));
            PJsonTools.WriteText(path, pdd);
        }

        public void ReadJson()
        {
            Debug.Log(Path.GetFileName(path));
            pdd = PJsonTools.ReadText<ProbDeviceDistributed>(path);
            pdd.OnReadAfter();
        }

        public void Clear()
        {
            pdd.Clear();
        }
    }
}