using System.Collections;
using UnityEngine;

public class AttackCameraEffects : MonoBehaviour
{
    [Header("References")]
    public CameraController cameraController; // assign in inspector (camera controller)
    private Camera cam;

    [Header("FOV")]
    public float normalFOV = 60f;
    public float attackFOV = 45f;
    public float fovBlendDuration = 0.15f;

    [Header("Tilt")]
    public float attackTilt = 8f;
    public float tiltBlendSpeed = 12f;

    [Header("Shake")]
    public float shakeDuration = 0.12f;
    public float shakeStrength = 0.12f; // world units
    private float shakeTimer = 0f;

    [Header("Slow Motion")]
    public bool useSlowMotion = true;
    public float slowmoScale = 0.15f;
    public float slowmoDuration = 0.10f;

    // internal states
    private bool isAttackCam = false;
    private float currentTilt = 0f;
    private float targetTilt = 0f;

    // FOV animation coroutine handle
    private Coroutine fovCoroutine;

    void Start()
    {
        if (cameraController == null)
        {
            
            cameraController = GetComponent<CameraController>();
        }
        cam = Camera.main;
        if (cam == null) cam = GetComponent<Camera>();
        if (cam != null)
        {
            normalFOV = cam.fieldOfView;
        }
    }

    void LateUpdate()
    {
       
        targetTilt = isAttackCam ? attackTilt : 0f;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.unscaledDeltaTime * tiltBlendSpeed);
        if (cameraController != null)
        {
           // cameraController.externalTilt = currentTilt;
        }

        //// handle shake
        //if (shakeTimer > 0f)
        //{
        //    shakeTimer -= Time.unscaledDeltaTime;
        //    Vector3 shake = Random.insideUnitSphere * shakeStrength;
        
        //    if (cameraController != null)
        //      //  cameraController.externalPositionOffset = shake;
        //}
        //else
        //{
        //    if (cameraController != null)
        //       // cameraController.externalPositionOffset = Vector3.zero;
        //}

        
    }


    public void OnAttackCamStart()
    {
        isAttackCam = true;

        
        if (fovCoroutine != null) StopCoroutine(fovCoroutine);
        fovCoroutine = StartCoroutine(LerpFOV(cam != null ? cam.fieldOfView : normalFOV, attackFOV, fovBlendDuration));
    }

    public void OnAttackCamImpact()
    {

        shakeTimer = shakeDuration;

        if (useSlowMotion) StartCoroutine(DoSlowMotion(slowmoScale, slowmoDuration));
    }

    public void OnAttackCamEnd()
    {
        isAttackCam = false;

        // revert FOV to normal
        if (fovCoroutine != null) StopCoroutine(fovCoroutine);
        fovCoroutine = StartCoroutine(LerpFOV(cam != null ? cam.fieldOfView : attackFOV, normalFOV, fovBlendDuration * 1.3f));
    }

    private IEnumerator LerpFOV(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float val = Mathf.Lerp(from, to, t / duration);
            if (cam != null)
            {
                cam.fieldOfView = val;
            }
            yield return null;
        }
        if (cam != null) cam.fieldOfView = to;
    }

    private IEnumerator DoSlowMotion(float scale, float duration)
    {
        float prev = Time.timeScale;
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = prev;
        Time.fixedDeltaTime = 0.02f;
    }
}
