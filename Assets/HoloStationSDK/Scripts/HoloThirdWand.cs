using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloThirdWand : VrpnTracker
{
    [SerializeField]
    protected string headName = "";

    // Update is called once per frame
    protected override void Update()
    {
        if (trackPosition)
        {
            Vector3 wandPose = trackerSetting.GetPosition(objectName, channel);
            Vector3 headPose = trackerSetting.GetPosition(headName, channel);

            if ( (headPose.x != -505 || headPose.y != -505 || headPose.z != -505)
             &&  (wandPose.x != -505 || wandPose.y != -505 || wandPose.z != -505) )
            {
                transform.localPosition = wandPose - headPose;
            }
        }

        if (trackRotation)
        {
            Quaternion rotate = trackerSetting.GetRotation(objectName, channel);
            if (rotate.x != -505 || rotate.y != -505 || rotate.z != -505 || rotate.w != -505)
            {
                transform.localRotation = rotate;
            }
        }
    }
}
