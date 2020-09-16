/*   raycast 3D draw
    Nolo 
 
 */
using UnityEngine;

public class SimplePointer : MonoBehaviour
{
    [Header("射线样式")]
    public Color pointerHitColor = new Color(0f, 0.5f, 0f, 1f);
    public Color pointerMissColor = new Color(0.8f, 0f, 0f, 1f);
    public Material pointerMaterial;
    public float pointerLength = 10f;
    public float HangingLength = 0.3f;
    // 射线粗细
    public float pointerThickness = 0.002f;  

    [Header("射线不可达层级")]
    public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;

    [Header("射线尾部")]
    public GameObject customPointerCursor;
    public float pointerTipScale = 0.01f;

    // 射线物体
    private GameObject curHitObject = null;
    private GameObject pointerHolder;
    private GameObject pointer;
    private GameObject pointerTip;

    [HideInInspector]
    public GameObject hoveringElement;

    // 与射线具体位置有关
    private float pointerContactDistance = 0f;
    private RaycastHit pointerContactRaycastHit = new RaycastHit();
    private Transform pointerContactTarget = null;
    private Vector3 destinationPosition;

    private bool startDraging = false;

    // 计算射线拖拽
    private GameObject curDragObject;
    private float curDragDistance = 0.0f;
    private Vector3 curDragOffset = Vector3.zero;
    private Quaternion curDragRotation = Quaternion.identity;

    /// <summary>
    /// 初始化射线
    /// </summary>
    private void InitPointer()
    {
        //pointerHolder
        pointerHolder = new GameObject("PointerHolder");
        pointerHolder.transform.parent = transform;
        pointerHolder.transform.localPosition = Vector3.zero;

        //pointer
        pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pointer.transform.name = string.Format("Pointer");
        pointer.transform.parent = pointerHolder.transform;
        pointer.GetComponent<BoxCollider>().isTrigger = true;
        pointer.AddComponent<Rigidbody>().isKinematic = true;
        pointer.layer = LayerMask.NameToLayer("Ignore Raycast");
        var pointerRenderer = pointer.GetComponent<MeshRenderer>();
        pointerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        pointerRenderer.receiveShadows = false;
        if (pointerMaterial != null)
        {
            pointerRenderer.material = pointerMaterial;
        }
        
        //pointerTip
        if (customPointerCursor)
        {
            pointerTip = Instantiate(customPointerCursor);
        }
        else
        {
            pointerTip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pointerTip.transform.localScale = new Vector3(pointerTipScale, pointerTipScale, pointerTipScale);

            var pointerTipRenderer = pointerTip.GetComponent<MeshRenderer>();
            pointerTipRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            pointerTipRenderer.receiveShadows = false;
            if (pointerMaterial != null)
            {
                pointerTipRenderer.material = pointerMaterial;
            }
        }
        pointerTip.transform.name = string.Format("PointerTip");
        pointerTip.transform.parent = pointerHolder.transform;
        //pointerTip.GetComponent<Collider>().isTrigger = true;
        //pointerTip.AddComponent<Rigidbody>().isKinematic = true;
        pointerTip.layer = LayerMask.NameToLayer("Ignore Raycast");
        PointerActivate(false);
    }

    /// <summary>
    /// 设置pointer、pinterTip是否可见
    /// </summary>
    /// <param name="state"></param>
    private void PointerActivate(bool state)
    {
        pointer.SetActive(state);
        pointerTip.SetActive(state);
    }

    private void Start()
    {
        InitPointer();
    }

    void Update()
    {

        PointerActivate(true);
        // 创建射线
        Ray pointerRaycast = new Ray(transform.position, transform.forward);
        RaycastHit pointerCollidedWith;
        // rayHit: true or false
        var rayHit = Physics.Raycast(pointerRaycast, out pointerCollidedWith, pointerLength, ~layersToIgnore);
        var pointerBeamLength = GetPointerBeamLength(rayHit, pointerCollidedWith);
        // 根据射线的长度和宽度画出射线
        SetPointerTransform(pointerBeamLength, pointerThickness);

        if (Input.GetMouseButtonUp(0))
        {
            //// 扳机松开
            //// 停止拖动
            if (curDragObject != null)
            {
                curDragObject = null;
            }

            startDraging = false;
        }

        if (startDraging)
        {
            if (curDragObject != null)
            {
                //// 更新拖动的物体位置和角度
                UpdateDrag(curDragObject);
            }
        }
    }

    /// <summary>
    /// 画射线
    /// </summary>
    /// <param name="setLength"></param>
    /// <param name="setThicknes"></param>
    private void SetPointerTransform(float setLength, float setThicknes)
    {
        //if the additional decimal isn't added then the beam position glitches
        var beamPosition = setLength / (2 + 0.00001f);

        // 射线
        pointer.transform.localScale = new Vector3(setThicknes, setThicknes, setLength);
        pointer.transform.localPosition = new Vector3(0f, 0f, beamPosition);
        // 射线尾部
        pointerTip.transform.localPosition = new Vector3(0f, 0f, setLength - (pointerTip.transform.localScale.z / 2));
        pointerHolder.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// 获取射线长度
    /// </summary>
    /// <param name="hasRayHit"></param>
    /// <param name="collidedWith"></param>
    /// <returns></returns>
    private float GetPointerBeamLength(bool hasRayHit, RaycastHit collidedWith)
    {
        var actualLength = pointerLength;

        //reset if beam not hitting or hitting new collider
        if (!hasRayHit || (pointerContactRaycastHit.collider && pointerContactRaycastHit.collider != collidedWith.collider))
        {
            if (pointerContactRaycastHit.collider != null)
            {
                // 射线移出上次击中的物体
                PointerOut(pointerContactRaycastHit.collider.gameObject);
            }
        }

        //adjust beam length if something is blocking it
        if (hasRayHit && 
            pointerContactDistance < pointerLength)
        {
            /// 击中物体
            /// 将射线长度调整到射线到物体的距离
            actualLength = pointerContactDistance;
        }
        else {
            /// 没有击中任何物体
            /// 悬空
            actualLength = HangingLength;
        }

        //check if beam has hit a new target
        if (hasRayHit)
        {
            pointerContactDistance = collidedWith.distance;
            pointerContactTarget = collidedWith.transform;
            // 保存当前射线碰撞的信息
            pointerContactRaycastHit = collidedWith;
            destinationPosition = pointerTip.transform.position;
            UpdatePointerMaterial(pointerHitColor);

            // 射线击中物体
            PointerIn(collidedWith.collider.gameObject, actualLength);
        }

        return actualLength;
    }

    /// <summary>
    /// 更新射线材质
    /// </summary>
    /// <param name="color"></param>
    private void UpdatePointerMaterial(Color color)
    {
        var pointerRenderer = pointer.GetComponent<Renderer>();
        pointerRenderer.material.color = color;

        var pointerTipRenderer = pointerTip.GetComponent<Renderer>();
        if (pointerTipRenderer != null)
        {
            pointerTipRenderer.material.color = color;
        }
        else
        {
            Renderer[] tipRendererList = pointerTip.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in tipRendererList)
            {
                renderer.material.color = color;
            }
        }
    }

    public GameObject GetHitObject()
    {
        return curHitObject;
    }

    /// <summary>
    /// 射线移出
    /// </summary>
    /// <param name="hitObj"></param>
    public void PointerOut(GameObject hitObj)
    {
        curHitObject = null;

        if (curDragObject == null)
        {
            //// 取消物体高亮
        }
    }

    /// <summary>
    /// 射线输入
    /// </summary>
    /// <param name="hitObj"></param>
    /// <param name="hitDistance"></param>
    public void PointerIn(GameObject hitObj, float hitDistance)
    {
        curHitObject = hitObj;

        if (curDragObject == null)
        {
            //// 扣下扳机开始拖动
            if (Input.GetMouseButton(0))
            {
                startDraging = true;
                /// 拖动的物体
                curDragObject = hitObj;
                BeginDrag(curDragObject, hitDistance);
            }
        }   
    }

    /// <summary>
    /// 计算物体拖动的位置和角度偏差
    /// </summary>
    /// <param name="hitObject"></param>
    /// <param name="hitDistance"></param>
    private void BeginDrag(GameObject hitObject, float hitDistance)
    {
        // 计算射线的终点位置
        // Vector3.forward == (0,0,1)
        // hitDistance: saclar，代表射线到达前方的长度
        // rotation * (Vector3.forward * hitDistance) : 计算当前旋转角度下，向前移动hitDistance所需要做的移动向量
        Vector3 rayEndPose = transform.position + (transform.rotation * (Vector3.forward * hitDistance));

        //  计算射线终点向hitObject位置移动方向向量
        curDragOffset = Quaternion.Inverse(hitObject.transform.rotation) * (hitObject.transform.position - rayEndPose);

        //  手柄的旋转四元数  transform.rotation
        //  击中物体的旋转四元数  hitObject.transform.rotation
        // 计算物体相对于手柄的旋转角度
        curDragRotation = Quaternion.Inverse(transform.rotation) * hitObject.transform.rotation;
        curDragDistance = hitDistance;
    }

    /// <summary>
    /// 更新物体的位置和角度
    /// </summary>
    /// <param name="hitObject"></param>
    private void UpdateDrag(GameObject hitObject)
    {
        Vector3 rayEndPose = transform.position + (transform.rotation * (Vector3.forward * curDragDistance));

        // 实质计算
        // (现在手柄旋转角度 * 开始手柄逆时针旋转角度) * 击中物体的原始四元数  
        // 算出现在的旋转角度 = 手柄旋转角度
        Quaternion objectRotation = transform.rotation * curDragRotation;
        hitObject.transform.rotation = objectRotation;
        // 射线击中的位置到击中的"物体"的位置始终保持一致
        // hitObject.transorm.position - rayEndPose ==  hitObject.transform.rotation * Quaternion.Inverse(hitObject.transform.rotation) * (hitObject.transform.position - rayEndPose);
        hitObject.transform.position = rayEndPose + objectRotation * curDragOffset;
    }
}