using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRecursive : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;
    void Start()
    {
        if (material != null)
            ReplaceMaterialRecursive(this.transform, material);
    }
    public void ReplaceMaterialRecursive(Transform t, Material mat)
    {
            foreach (Transform child in t)
            {
                if (child.gameObject.GetComponent<MeshRenderer>() != null)
                {
                    child.gameObject.GetComponent<MeshRenderer>().material = mat;
                }
                Renderer r = child.gameObject.GetComponent<Renderer>();
                if (child.childCount > 0)
                {
                    ReplaceMaterialRecursive(child, mat);
                }
            }
        }
}
