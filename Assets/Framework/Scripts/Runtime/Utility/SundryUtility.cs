using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ʵ�ú�����
/// </summary>
public static class SundryUtility
{
    /// <summary>
    /// ƽ��������ɫ
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="currentVelocity"></param>
    /// <param name="smoothTime"></param>
    /// <returns></returns>
    public static Color SmoothDampColor(Color current, Color target, ref Color currentVelocity, float smoothTime)
    {
        Color c = default;
        c.r = Mathf.SmoothDamp(current.r, target.r, ref currentVelocity.r, smoothTime);
        c.g = Mathf.SmoothDamp(current.g, target.g, ref currentVelocity.g, smoothTime);
        c.b = Mathf.SmoothDamp(current.b, target.b, ref currentVelocity.b, smoothTime);
        c.a = Mathf.SmoothDamp(current.a, target.a, ref currentVelocity.a, smoothTime);
        return c;
    }

    /// <summary>
    /// ��ɫ�Ƿ����
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool ColorApproximately(Color a, Color b)
    {
        return Approximately(a.r, b.r) && Approximately(a.g, b.g) && Approximately(a.b, b.b);
    }

    /// <summary>
    /// ��ֵ�Ƿ����
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Approximately(float a, float b)
    {
        //return Mathf.Approximately(a, b);
        return Mathf.Abs(a - b) <= 0.01f;
    }
}
