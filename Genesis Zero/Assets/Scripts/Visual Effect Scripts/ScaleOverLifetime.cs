using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOverLifetime : MonoBehaviour
{
    public float StartScale;
    public float EndScale;
    public float TimeToScale;
    private float CurrentTimeToScale;
    private float diff;
    // Start is called before the first frame update
    void Start()
    {
        CurrentTimeToScale = 0;
        diff = EndScale - StartScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(CurrentTimeToScale < TimeToScale)
        {
            float scale = diff * (CurrentTimeToScale / TimeToScale);
            gameObject.transform.localScale = new Vector3(StartScale + scale, StartScale + scale, StartScale + scale);
            CurrentTimeToScale += Time.fixedDeltaTime;
        }
        
    }
}
