using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 测试本地坐标的转换
[ExecuteAlways]
public class TestPosition : MonoBehaviour
{
    public Transform target;
    public Transform target_x;// 只同步z
    public Vector3 pos;
    public TransformType _transformType;

    void Start()
    {

    }

    void Update()
    {
        switch (_transformType)
        {
            case TransformType.Direction:
                if (target != null)
                {
                    target.position = transform.TransformDirection(pos);
                }

                if (target_x != null)
                {

                }
                break;
            case TransformType.Point:
                if (target != null)
                {
                    target.position = transform.TransformPoint(pos);
                }

                if (target_x != null)
                {
                    // 只想影响 x 的相对坐标，可以得到正确结果，只想影响另外两个轴也只同理
                    // 注意：这种计算大约需要20次迭代，才能得到近似的（小数点后6位）正确结果，因为每次计算都会除以 2，也就是当前总距离的一半，相当于线性过度

                    var oldPos = target_x.position;
                    // 转换本地坐标，需复用 y 和 z
                    var lPos = transform.InverseTransformPoint(oldPos);
                    // 计算复用转换后的新世界坐标
                    var newPos = transform.TransformPoint(new Vector3(pos.x, lPos.y, lPos.z));
                    oldPos.x = newPos.x;// 只改 x
                    target_x.position = oldPos;
                }
                break;
            default:
                break;
        }
    }

    public enum TransformType
    {
        Direction, Point
    }
}
