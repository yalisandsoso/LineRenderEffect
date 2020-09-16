using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    [SerializeField]
    private VrpnTracker wandTracker;
    [SerializeField]
    private VrpnTracker headTracker;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        string headPose = string.Format("(X={0:F5}, Y={1:F5}, Z={2:F5})", headTracker.transform.position.x, headTracker.transform.position.y, headTracker.transform.position.z);
        string headRot = string.Format("(X={0:F5}, Y={1:F5}, Z={2:F5})", headTracker.transform.eulerAngles.x, headTracker.transform.eulerAngles.y, headTracker.transform.eulerAngles.z);
        string wandPose = string.Format("(X={0:F5}, Y={1:F5}, Z={2:F5})", wandTracker.transform.position.x, wandTracker.transform.position.y, wandTracker.transform.position.z);
        string wandRot = string.Format("(X={0:F5}, Y={1:F5}, Z={2:F5})", wandTracker.transform.eulerAngles.x, wandTracker.transform.eulerAngles.y, wandTracker.transform.eulerAngles.z);

        text.text = "HeadPose: " + headPose + "\n";
        text.text += "HeadRot: " + headRot + "\n";
        text.text += "WandPose: " + wandPose + "\n";
        text.text += "WandRot: " + wandRot + "\n";
    }
}
