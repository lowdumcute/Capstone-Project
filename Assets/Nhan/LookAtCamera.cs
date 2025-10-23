using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Camera mainCam;



    // Update is called once per frame
    private void Start()
    {
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCam != null)
            transform.LookAt(transform.position + mainCam.transform.forward);
    }
}
