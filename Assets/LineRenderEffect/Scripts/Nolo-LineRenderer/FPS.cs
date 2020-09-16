/*  First Person Controller
    ：模拟射线发射器移动和旋转
 
 */

using UnityEngine;

public class FPS : MonoBehaviour
{
    // 移动速度
    [SerializeField]
    private float speed = 10.0f;
    // 旋转速度
    [SerializeField]
    private float rotationSpeed = 100.0f;

    private float translation = 0;
    private float rotation = 0;

    void Update()
    {
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        transform.Translate(0,0,translation);
        transform.Rotate(0, rotation, 0);
    }
}
