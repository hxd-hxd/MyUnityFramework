using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestDelegate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Action<int, string> action = Test1;
        UnityAction<int, string> unityAction = Test1;

        Delegate d_a = action;
        Delegate d_ua = unityAction;


        Debug.Log($"Action == UnityAction ：{action.Equals(unityAction)}");

        //Action<int, string> ua_a = (Action<int, string>)(Delegate)unityAction;
        //Action<int, string> d_ua_a = (Action<int, string>)d_ua;
        //UnityAction<int, string> a_ua = (UnityAction<int, string>)d_a;

        if (d_ua is Action<int, string> d_ua_a)
            d_ua_a?.Invoke(1, "UnityAction 转 Action");
        if (d_a is UnityAction<int, string> d_a_ua)
            d_a_ua?.Invoke(1, "Action 转 UnityAction");

        new Action<int, string>(unityAction)(2, "UnityAction 转 Action");
        new UnityAction<int, string>(action)(2, "Action 转 UnityAction");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Test1(int num, string name)
    {
        Debug.Log($"{num}，{name}");
    }
}
