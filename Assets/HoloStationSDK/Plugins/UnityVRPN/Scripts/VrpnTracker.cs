using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrpnTracker : MonoBehaviour
{
    [SerializeField]
    protected VrpnTrackerSetting trackerSetting;
    [SerializeField]
    protected string objectName = "";

    [SerializeField]
    protected bool trackPosition = true;
    [SerializeField]
    protected bool trackRotation = true;

    protected int channel = 0;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (trackPosition)
        {
            Vector3 position = trackerSetting.GetPosition(objectName, channel);
            if (position.x != -505 || position.y != -505 || position.z != -505)
            {
                transform.position = position;
            }
        }

        if (trackRotation)
        {
            Quaternion rotate = trackerSetting.GetRotation(objectName, channel);
            if (rotate.x != -505 || rotate.y != -505 || rotate.z != -505 || rotate.w != -505)
            {
                transform.rotation = rotate;
            }
        }
    }
}
