// ORIGNALLY FROM https://gist.githubusercontent.com/ftvs/5822103/raw/ce32e02f11545fede46e60079c993091c5f52a0f/CameraShake.cs

using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.
    public Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    public void ShakeCamera(float s = 1)
    {
        originalPos = camTransform.localPosition;
        shakeDuration = s;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount * (shakeDuration/1f);

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }
}