using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleMan : MonoBehaviour
{
    public BoxCollider2D bounds;
    public GameObject BossRoom;
	public GameObject Target;
    public GameObject Boss;
    public Vector3 maxoffset;
	public GameObject Joint;
    private Vector2 currentoffset;
    private bool pushedback;
    private Vector3 originmaxoffset;

    // Start is called before the first frame update
    void Start()
    {
        originmaxoffset = maxoffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (Boss == null)
            return;
        Vector2 offset = Target.transform.position - (BossRoom.transform.position + (Vector3)BossRoom.GetComponent<BoxCollider2D>().offset);
        Vector2 ratio = (offset / bounds.size) * 2;
        float xoff = 0;
        if (Boss.GetComponent<BossAI>())
        {
            bool looking = Boss.GetComponent<BossAI>().lookingatcamera == true;
            xoff = looking ? 0 : Boss.GetComponent<BossAI>().lookDir.normalized.x * -maxoffset.x;
            if (looking)
            {
                maxoffset.z = Mathf.Lerp(maxoffset.z, originmaxoffset.z + 3, Time.deltaTime * 2);
                pushedback = true;
            }
            else if (!looking)
            {
                maxoffset.z = Mathf.Lerp(maxoffset.z, originmaxoffset.z - 3, Time.deltaTime * 2);
                pushedback = false;
            }
        }
        else
        {
            xoff = (maxoffset.x / 2 * -ratio.x);
        }

        Vector2 localoffset = Vector2.zero;
        if (Boss.GetComponent<BossAI>())
        {
            localoffset = new Vector2(xoff, (maxoffset.y / 2 * -ratio.y) + Boss.GetComponent<BossAI>().lookDir.normalized.y * -maxoffset.y / 2);    // local offset
        }
        else
        {
            localoffset = new Vector2(xoff, (maxoffset.y / 2 * -ratio.y));    // local offset
        }
        currentoffset = Vector2.Lerp(currentoffset, localoffset, Time.deltaTime * 2);
        transform.position = Boss.transform.position + (Vector3)currentoffset + new Vector3(0, 0, maxoffset.z);
    }

	public void UpdateJoint()
	{
		transform.position = Joint.transform.position;
		currentoffset = Vector2.zero;
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(BossRoom.transform.position + (Vector3)BossRoom.GetComponent<BoxCollider2D>().offset, bounds.size);
    }
}
