using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // 击中的物体
    public Transform hitObj;
    // 射线终点或者手柄
    public Transform oriObj;

    private Vector3 curDragOffset;
    private Quaternion curDragRottaion = new Quaternion(0,0,0,0);

    void Start()
    {
        // ori 往 hit方向
        curDragOffset = Quaternion.Inverse(hitObj.rotation) * (hitObj.position - oriObj.position);
        // curDragOffset = hitObj.rotation * (oriObj.position - hitObj.position);

        // 计算hit相对于ori的旋转角度
        curDragRottaion = Quaternion.Inverse(oriObj.rotation) * hitObj.rotation;

        // 保持原来的本地坐标轴，做旋转
        hitObj.rotation = hitObj.rotation * curDragRottaion;
    }

    private void Update()
    {
        // oriObj.Translate(curDragOffset * Time.deltaTime);


    }
}
