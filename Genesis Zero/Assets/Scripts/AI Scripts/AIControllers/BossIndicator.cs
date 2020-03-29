using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BossIndicator : MonoBehaviour
{
    public Sprite Square;
    public Sprite Circle;

    private Vector2 size;
    private Vector2 origin;

    public Vector2 targetsize;
    public float angle;

    private float scalex, scaley;

    private SpriteRenderer image;

    public Vector2 offset;
    private Vector2 lastoffset;
    public bool Centered;

    private bool Initialized;

    private void Awake()
    {
        image = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        lastoffset = offset;
        if (Initialized == false)
        {
            origin = image.transform.position;
            Initialized = true;
        }
        size = image.sprite.rect.size;
        scalex = targetsize.x / size.x;
        scaley = targetsize.y / size.y;
        image.transform.localScale = new Vector2(scalex * image.sprite.pixelsPerUnit, scaley * image.sprite.pixelsPerUnit);
        image.transform.localEulerAngles = new Vector3(0, 0, angle);

        float distance = targetsize.x / 2;
        float offsetx = Mathf.Cos(angle) * distance;
        float offsety = Mathf.Sin(angle) * distance;
        if (Centered)
        {
            offsetx = 0;
            offsety = 0;
        }
        image.transform.position = new Vector2(origin.x + offsetx + offset.x, origin.y + offsety + offset.y);

    }

    public void SetOrigin(Vector2 pos)
    {
        origin = pos;
        size = image.sprite.rect.size;
        scalex = targetsize.x / size.x;
        scaley = targetsize.y / size.y;
        image.transform.localScale = new Vector2(scalex * image.sprite.pixelsPerUnit, scaley * image.sprite.pixelsPerUnit);
        image.transform.localEulerAngles = new Vector3(0, 0, angle);

        float distance = targetsize.x / 2;
        float offsetx = Mathf.Cos(angle) * distance;
        float offsety = Mathf.Sin(angle) * distance;
        if (Centered)
        {
            offsetx = 0;
            offsety = 0;
        }
        image.transform.position = new Vector2(origin.x + offsetx + offset.x, origin.y + offsety + offset.y);
    }

    // Update is called once per frame
    void Update()
    {
        size = image.sprite.rect.size;
        scalex = targetsize.x / size.x;
        scaley = targetsize.y / size.y;
        image.transform.localScale = new Vector2(scalex * image.sprite.pixelsPerUnit, scaley * image.sprite.pixelsPerUnit);
        image.transform.localEulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI);

        float distance = targetsize.x / 2;
        float offsetx = Mathf.Cos(angle) * distance;
        float offsety = Mathf.Sin(angle) * distance;
        if (Centered)
        {
            offsetx = 0;
            offsety = 0;
        }
        image.transform.position = new Vector2(origin.x + offsetx + offset.x, origin.y + offsety + offset.y);
    }

    public void SetSize(Vector2 vector)
    {
        targetsize = vector;
        SetOrigin(origin);
    }

    public void SetAngle(Vector2 dir)
    {
        angle = Mathf.Atan2(dir.y, dir.x);
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    public void SetOffset(Vector2 offs)
    {
        offset = offs;
    }

    public void SetCentered(bool cent)
    {
        Centered = cent;
    }

    public void SetCircle()
    {
        image.sprite = Circle;
    }

    public void SetSquare()
    {
        image.sprite = Square;
    }

    public void SetIndicator(Vector2 position, Vector2 size, Vector2 dir, Color color, bool centered)
    {
        SetOrigin(position);
        SetSize(size);
        SetAngle(dir);
        SetColor(color);
        SetCentered(centered);
        Initialized = true;
    }
}
