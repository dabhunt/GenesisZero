using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractPopup : MonoBehaviour
{
    /*
     * this script will create a popup UI window telling you to interact, if you leave the active distance, it destroys the popup
     */
    public float checksPerSecond = 4f;
    public float activeDistance = 1.5f;
    public float scaleSize = .5f;
    public float YOffset = 1.5f;
    public string TitleText = "";
    public string PopText = "Press [F] to interact";
    private Canvas canvasRef;
    public bool interactable = true;
    public bool visible = false;
    private Player player;
    private GameObject popup;
    private Camera camRef;
    private Color titleColor;
    //private GameInputActions inputActions;
    void Start()
    {
        checksPerSecond = 1 / checksPerSecond;
        GameObject temp = GameObject.FindGameObjectWithTag("Player");
        player = temp.GetComponent<Player>();
        canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
        //inputActions = GameInputManager.instance.GetInputActions();
        camRef = Camera.main;
        InvokeRepeating("DistanceCheck", 0, checksPerSecond);
        //if there is a skillpickup attached to this, set the title to the name and rarity color
        if (GetComponent<SkillPickup>() != null)
        {
            SkillObject skill = GetComponent<SkillPickup>().skill;
            titleColor = player.GetSkillManager().GetColor(skill);
            TitleText = skill.name;
        }
    }
    private void DistanceCheck()
    {
        if (player == null)
            return;
        if (Vector2.Distance(player.transform.position, transform.position) <= activeDistance)
        {
            if (!visible && interactable)
            {
                visible = true;
                InitializeUI();
            }
        }
        else
        {
            visible = false;
            if (popup != null)
                Destroy(popup);
        }

    }
    private void InitializeUI()
    {
        Vector2 screenPos = CalcScreenPos();
        popup = (GameObject)GameObject.Instantiate(Resources.Load("UI/InteractPopup"), screenPos, Quaternion.identity);
        SetScreenPos(screenPos);
        SetTitle(TitleText);
        SetText(PopText);
    }
    private Vector2 CalcScreenPos()
    {
        Vector2 headScreenPos;
        headScreenPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + YOffset, 0));
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRef.transform as RectTransform, headScreenPos, canvasRef.worldCamera, out pos);
        return headScreenPos;
    }

    private void SetScreenPos(Vector2 screenPos)
    {
        //popup.transform.parent = canvasRef.transform;
        popup.transform.SetParent(canvasRef.transform);
        popup.transform.localPosition = new Vector3(popup.transform.localPosition.x, popup.transform.localPosition.y, .1f);
        popup.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        popup.transform.localRotation = Quaternion.Euler(0, 0, 0);
        popup.GetComponent<RectTransform>().anchoredPosition = screenPos;
    }
    private void Update()
    {
        if (interactable && popup != null)
        {
            Vector2 screenPos = camRef.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + YOffset, 0));
            //popup.transform.localPosition = screenPos;
            popup.transform.position = screenPos;
            //SetScreenPos(screenPos);
        }
        else
        {
            visible = false;
            if (popup != null)
                Destroy(popup);
        }
    }
    public void DestroyPopUp()
    {
        if (popup != null)
            Destroy(popup);
    }
    public void SetText(string newtext)
    {
        if (popup != null)
        {
            popup.transform.Find("InteractText").gameObject.GetComponent<Text>().text = newtext;
            PopText = newtext;
        }
    }

    public void SetTitle(string newtitle)
    {
        if (popup != null)
        {
            //if there is a valid newtitle string, do this
            if (newtitle != null && newtitle != "")
            {
                popup.transform.Find("TitleText").gameObject.SetActive(true);
                popup.transform.Find("TitleText").gameObject.GetComponent<Text>().text = newtitle;
                popup.transform.Find("TitleText").gameObject.GetComponent<Text>().color = titleColor;
                popup.transform.Find("BG").localScale = new Vector2(1, 1.7f);
                TitleText = newtitle;
            }
            else 
            {
                popup.transform.Find("BG").localScale = new Vector2(1, 1f);
                TitleText = "";
                popup.transform.Find("TitleText").gameObject.SetActive(false);
            }
        }       
    }
    public void SetTitleAndText(string newtext, string newtitle)
    {
        SetText(newtext);
        SetTitle(newtitle);
    }
    private void OnDestroy()
    {
        Destroy(popup);
    }
    public void SetInteractable(bool caninteract)
    {
        interactable = caninteract;
    }
}
