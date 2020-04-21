using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Kenny Doan
 * Skill Display is a script that displays the Skills the player has in a UI. Each element is displayed
 * and can be moused over for it's description.
 */
public class SkillDisplay : MonoBehaviour
{
    public Vector2 StartPoint;  // Where the first skill starts
    public Vector2 Seperation;  // the x,y seperation of each skill
    public int MaxColumns = 10;
    private Player Player;
    public SkillManager SkillManager;
    private List<GameObject> skilldisplay;
    public GameObject UIElement;
    public GameObject PopupElement;
    [Space]
    public float AbilityScale = .7f;
    public Vector2 PopupPosition;
    public bool ModsInMiddle = true;
    private Canvas canvasRef;

    public int skillnumber;
    // Start is called before the first frame update
    void Start()
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        SkillManager = Player.GetSkillManager();
        skillnumber = SkillManager.GetAmount();
        skilldisplay = new List<GameObject>();
        canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        //SkillManager = Player.GetSkillManager();
        if (skillnumber != SkillManager.GetAmount() || SkillManager.GetUpdated() == true)
        {
            skillnumber = SkillManager.GetAmount();
            SkillManager.SetUpdated(false);
            if (SkillManager.GetAddedSkill() != null)
            {
                SkillObject s = SkillManager.GetAddedSkill();
                PopupElement.GetComponent<SkillPopupDisplay>().SetPopup(s.Icon, s.name, s.SimpleDescription, SkillManager.GetColor(s));
                PopupElement.GetComponent<SkillPopupDisplay>().Popup();
                SkillManager.SetAddedSkill(null);
            }
            UpdateDisplay();
        }
    }

    /**
     * Updates the display of the skills.
     */
    public void UpdateDisplay()
    {
        List<SkillObject> skills = SkillManager.GetSkillObjects();
        foreach (GameObject sk in skilldisplay)
        {
            Destroy(sk.gameObject);
        }
        skilldisplay.Clear();
        int modnum = 0;
        for (int i = 0; i < skills.Count; i++)
        {
            int col = modnum % MaxColumns;
            int row = modnum / MaxColumns;

            if (skills[i].IsAbility == false)
            {
                float scaleFactor = canvasRef.scaleFactor;
                if (ModsInMiddle)
                    StartPoint.x = -355f;
                GameObject instance = (GameObject)Instantiate(UIElement, (Vector3)transform.position + (Vector3)StartPoint *scaleFactor + new Vector3(Seperation.x * col * scaleFactor, Seperation.y * row * scaleFactor), Quaternion.identity);
                instance.transform.SetParent(transform);
                //RectTransform myRect = instance.GetComponent<RectTransform>();
                Vector2 finalPosition = new Vector2(instance.transform.position.x, instance.transform.position.y);
                //myRect.anchoredPosition = finalPosition;
                //instance.GetComponent<RectTransform>().anchoredPosition = (Vector3)transform.position + (Vector3)StartPoint + new Vector3(Seperation.x * col, Seperation.y * row);
                instance.transform.localScale = new Vector3(1, 1, 1);
                instance.transform.localRotation = Quaternion.Euler(0, 0, 0);
                instance.GetComponent<SkillUIElement>().SetIcon(skills[i].Icon);
                instance.GetComponent<SkillUIElement>().SetColor(SkillManager.GetColor(skills[i]));
                instance.GetComponent<SkillUIElement>().SetStack(SkillManager.GetSkillStack(skills[i].name));
                instance.GetComponent<SkillUIElement>().SetSkill(skills[i]);
                instance.GetComponent<SimpleTooltip>().infoLeft = skills[i].Description;
                skilldisplay.Add(instance);
            }

            if (skills[i].IsAbility && SkillManager.GetAbility1() && skills[i].name == SkillManager.GetAbility1().name)
            {
                GameObject abil1 = (GameObject)Instantiate(UIElement, (Vector3)transform.position, Quaternion.identity);
                GameObject parent = canvasRef.transform.Find("AbilityPanel").Find("CooldownOverlay").Find("Ability1CD").gameObject;
                abil1.transform.SetParent(parent.transform);
                abil1.transform.localPosition = new Vector3(0, 0, 0);
                abil1.transform.localRotation = Quaternion.Euler(0, 0, 0);
                abil1.transform.localScale = new Vector3(AbilityScale, AbilityScale, AbilityScale);
                abil1.GetComponent<SkillUIElement>().SetIcon(skills[i].Icon);
                abil1.GetComponent<SkillUIElement>().SetColor(SkillManager.GetColor(skills[i]));
                abil1.GetComponent<SimpleTooltip>().infoLeft = skills[i].Description;
                skilldisplay.Add(abil1);
            }
            else if (skills[i].IsAbility && SkillManager.GetAbility2() && skills[i].name == SkillManager.GetAbility2().name)
            {
                GameObject abil2 = (GameObject)Instantiate(UIElement, (Vector3)transform.position, Quaternion.identity);
                GameObject parent = canvasRef.transform.Find("AbilityPanel").Find("CooldownOverlay").Find("Ability2CD").gameObject;
                abil2.transform.SetParent(parent.transform);
                abil2.transform.localPosition = new Vector3(0, 0, 0);
                abil2.transform.localRotation = Quaternion.Euler(0, 0, 0);
                abil2.transform.localScale = new Vector3(AbilityScale, AbilityScale, AbilityScale);
                abil2.GetComponent<SkillUIElement>().SetIcon(skills[i].Icon);
                abil2.GetComponent<SkillUIElement>().SetColor(SkillManager.GetColor(skills[i]));
                abil2.GetComponent<SimpleTooltip>().infoLeft = skills[i].Description;
                skilldisplay.Add(abil2);
            }

            if (!skills[i].IsAbility)
            {
                modnum++;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position + (Vector3)StartPoint, 25f);
        Gizmos.DrawWireSphere(transform.position + (Vector3)StartPoint + (Vector3)Seperation, 25f);
        Gizmos.DrawWireSphere(transform.position + (Vector3)StartPoint + ((Vector3)Seperation * 2), 25f);

        Gizmos.color = Color.yellow;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)PopupPosition, new Vector3(3, 1, 0));
    }
}
