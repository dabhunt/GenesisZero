using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
/**
 * Justin Couch
 * This script moves the camera to keep the list of objects in view.
 */
public class ContainingCamera : MonoBehaviour
{
    Camera cam;
    public Transform[] ViewObjects; // Objects to keep in view
    public float DefaultDistance = 10f; // Distance when everything is in view
    public float DistanceIncrease = 10f; // Distance to increase as objects go outside of the view
    public float OutsideFactor = 10f; // Multiplier for result of how far objects are outside of the viewport
    public float moveRate = 10f; // How quickly the camera moves

    protected void Awake()
    {
        cam = GetComponent<Camera>();
    }

    protected void FixedUpdate()
    {
        if (ViewObjects.Length == 0) { return; }

        Vector3 avgPos = ViewObjects[0].position;
        float outsideAmount = 0.0f;
        for (int i = 0; i < ViewObjects.Length; i++)
        {
            if (i > 0)
            {
                avgPos += ViewObjects[i].position; // Get average position of objects
            }

            //Calculate how far out of the viewport the object is
            Vector3 viewportPos = cam.WorldToViewportPoint(ViewObjects[i].position);
            outsideAmount += Mathf.Max(Mathf.Max(0.0f, Mathf.Abs(0.5f - viewportPos.x) - 0.5f), Mathf.Max(0.0f, Mathf.Abs(0.5f - viewportPos.y) - 0.5f));
        }

        outsideAmount *= OutsideFactor;
        avgPos /= ViewObjects.Length;

        transform.position = Vector3.Lerp(transform.position, avgPos + Vector3.back * (DefaultDistance + DistanceIncrease * outsideAmount), moveRate * Time.fixedDeltaTime);
    }
}
