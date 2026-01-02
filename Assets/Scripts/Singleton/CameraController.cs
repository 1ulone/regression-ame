using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float cameraSpeed = 7.5f;

    private void FixedUpdate()
    {
        Vector3 smoothCam = Vector3.Lerp(transform.position, target.position, cameraSpeed * Time.fixedUnscaledDeltaTime);
        // Vector3 lockedCam = new Vector3(
        //         Mathf.Clamp(smoothCam.x, minCameraThreshold.x, maxCameraThreshold.x),
        //         Mathf.Clamp(smoothCam.y, minCameraThreshold.y, maxCameraThreshold.y), zValue);
        this.transform.position = new Vector3(smoothCam.x, smoothCam.y, -10);
    }
}
