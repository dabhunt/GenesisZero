using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BUG-E Way Point")]
public class DialogueInfo: ScriptableObject
{
    public WayPoint waypoint;
    public string dialogueName;
    public bool flysOver;
    public int ID;
    public void SetValues(WayPoint point, string name, bool flys, int id)
    {
        waypoint = point;
        dialogueName = name;
        flysOver = flys;
        ID = id;
    }
}
