using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidanceSystem : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject buge;
    private GameObject player;
    public float triggerDistance = 5;
    public float showArrowDuration = 6;
    private Vector2 lastLocation;
    private float totalDistance;
    private GameObject teleporter;
    void Start()
    {
        InvokeRepeating("CheckDist",10,10);
        player = GameObject.FindWithTag("Player");
        buge = GameObject.FindWithTag("BUG-E");
        teleporter = GameObject.FindWithTag("Teleporter");

    }
    public void CheckDist()
    {
        if (player == null)
            return;
        totalDistance += Vector3.Distance(player.transform.position, lastLocation);
        if (totalDistance > triggerDistance)
        {
            ShowGuidance();
            totalDistance = 0;
        }
        lastLocation = player.transform.position;

    }
    public void ShowGuidance()
    {
        print("showing guidance");
        buge.transform.Find("Arrow").gameObject.SetActive(true);
        GetComponent<BUGE>().LookAt(teleporter,showArrowDuration);
        DialogueManager.instance.TriggerDialogue("BUG-E_Arrow");
    }
}
