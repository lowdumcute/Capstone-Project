using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1.7f, -4f);
    private Quaternion rotation;

    private float x;
    private float y;
    [SerializeField] private float xSpeed = 5f;
    [SerializeField] private float ySpeed = 5f;

    [SerializeField] private float xMinRotation = -360f;
    [SerializeField] private float xMaxRotation = 360f;
    [SerializeField] private float yMinRotation = 10f;
    [SerializeField] private float yMaxRotation = 80f;

    [Header("External Additive Effects (set by AttackCameraEffects)")]
    [HideInInspector] public Vector3 externalPositionOffset = Vector3.zero; 
    [HideInInspector] public float externalTilt = 0f; 
    [HideInInspector] public float externalFOV = 0f; 

    // internal
    private Camera cam;
    private float baseFOV = 60f;

    void Start()
    {
        Vector3 angles = this.transform.eulerAngles;
        x = angles.x;
        y = angles.y;

        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        if (cam != null)
        {
            baseFOV = cam.fieldOfView;
        }
    }

   

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        CameraMove();

    
        rotation = Quaternion.Euler(y, x, 0);

  
        Quaternion tiltQuat = Quaternion.Euler(0, 0, externalTilt);
        Quaternion finalRotation = rotation * tiltQuat;

        Vector3 distanceVector = offset;
        Vector3 position = finalRotation * distanceVector + target.position;

    
        position += externalPositionOffset;

        transform.rotation = finalRotation;
        transform.position = position;

 
        if (cam != null)
        {
            float targetFOV = Mathf.Max(1f, baseFOV + externalFOV);
         
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.unscaledDeltaTime * 20f);
        }

      
    }

    public void CameraMove()
    {
        x += Input.GetAxis("Mouse X") * xSpeed;
        y += Input.GetAxis("Mouse Y") * ySpeed;

        x = ClampAngle(x, xMinRotation, xMaxRotation);
        y = ClampAngle(y, yMinRotation, yMaxRotation);
    }

    public float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
