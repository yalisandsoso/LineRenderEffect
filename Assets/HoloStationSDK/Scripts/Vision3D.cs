using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision3D : MonoBehaviour
{
    public float screenSize;
    public Vector3 originOffset;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float D = Vector3.Distance(originOffset, transform.parent.position);
        cam.fieldOfView = Mathf.Rad2Deg * Mathf.Atan(screenSize / D);
    }
}
