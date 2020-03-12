using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitOnDestroy : MonoBehaviour
{
    public List<GameObject> Emits;
    private bool quitting;
    private Restart restartScript;
    // Start is called before the first frame update
    void Start()
    {
        quitting = false;
        GameObject temp = GameObject.FindWithTag("StateManager");
        restartScript = temp.GetComponent<Restart>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        //if the scene is being restarted or the player quits
        if (restartScript == null || restartScript.ExitingScene() || quitting)
        {
            return;
        }

        if (Emits.Count > 0)
        {
            foreach (GameObject g in Emits)
            {
                if (g)
                {
                    try
                    {
                        GameObject emit = (GameObject)Instantiate(g, transform.position, Quaternion.identity);
                    }
                    catch { }

                }
            }
        }
    }
}
