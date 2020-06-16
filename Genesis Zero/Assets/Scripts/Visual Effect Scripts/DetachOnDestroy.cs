using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachOnDestroy : MonoBehaviour
{

    public GameObject Detachment;
    public bool detachImmediate = false;
    public float DestroyAfterXTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        if (detachImmediate)
            Detachment.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        Detachment.transform.parent = null;
        Destroy(Detachment, DestroyAfterXTime);
    }
}
