using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleMan : MonoBehaviour
{
    public BoxCollider2D bounds;
    public GameObject BossRoom;
    public GameObject Boss;
    public Vector3 maxoffset;
    private Vector2 currentoffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = Boss.transform.position - (BossRoom.transform.position + (Vector3)BossRoom.GetComponent<BoxCollider2D>().offset);
        Vector2 ratio = (offset / bounds.size) * 2;
        float xoff = Boss.GetComponent<BossAI>().lookingatcamera == true ? 0 : Boss.GetComponent<BossAI>().lookDir.normalized.x * -maxoffset.x;
        Vector2 localoffset = new Vector2(xoff, (maxoffset.y / 2 * -ratio.y) + Boss.GetComponent<BossAI>().lookDir.normalized.y * -maxoffset.y/2);    // local offset
        currentoffset = Vector2.Lerp(currentoffset, localoffset, Time.deltaTime * 2);
        transform.position = Boss.transform.position + (Vector3)currentoffset + new Vector3(0, 0, Boss.transform.position.z + maxoffset.z);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(BossRoom.transform.position + (Vector3)BossRoom.GetComponent<BoxCollider2D>().offset, bounds.size);
    }
}
