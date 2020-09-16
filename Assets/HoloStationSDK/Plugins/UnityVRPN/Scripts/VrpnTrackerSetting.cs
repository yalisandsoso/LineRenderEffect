using UnityEngine;
using System.Runtime.InteropServices;

public class VrpnTrackerSetting : MonoBehaviour
{
    [DllImport("unityVrpn")]
    private static extern double vrpnAnalogExtern(string address, int channel, int frameCount);

    [DllImport("unityVrpn")]
    private static extern bool vrpnButtonExtern(string address, int channel, int frameCount);

    [DllImport("unityVrpn")]
    private static extern double vrpnTrackerExtern(string address, int channel, int component, int frameCount);

    [SerializeField]
    private string hostname = "localhost";

    public Vector3 GetPosition(string tracker, int channel)
    {
        string address = tracker + "@" + hostname;

        return new Vector3(
           (float)vrpnTrackerExtern(address, channel, 0, Time.frameCount),
           (float)vrpnTrackerExtern(address, channel, 1, Time.frameCount),
           (float)vrpnTrackerExtern(address, channel, 2, Time.frameCount));
    }

    public Quaternion GetRotation(string tracker, int channel)
    {
        string address = tracker + "@" + hostname;

        return new Quaternion(
            (float)vrpnTrackerExtern(address, channel, 3, Time.frameCount),
            (float)vrpnTrackerExtern(address, channel, 4, Time.frameCount),
            (float)vrpnTrackerExtern(address, channel, 5, Time.frameCount),
            (float)vrpnTrackerExtern(address, channel, 6, Time.frameCount));
    }

    public bool GetButton(string tracker, int channel)
    {
        string address = tracker + "@" + hostname;

        return vrpnButtonExtern(address, channel, Time.frameCount);
    }
}
