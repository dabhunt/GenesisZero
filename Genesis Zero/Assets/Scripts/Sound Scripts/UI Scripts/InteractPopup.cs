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
    public string PopText = "Press [F] to interact";
    private Canvas canvasRef;
    private bool interactable = true;
    private Player player;
    private GameObject popup;
    //private GameInputActions inputActions;
    void Start()
    {
        checksPerSecond = 1 / checksPerSecond;
        GameObject temp = GameObject.FindGameObjectWithTag("Player");
        player = temp.GetComponent<Player>();
        canvasRef = GameObject.FindGameObjectWithTag("CanvasUI").GetComponent<Canvas>();
        //inputActions = GameInputManager.instance.GetInputActions();
        InvokeRepeating("DistanceCheck", 0, checksPerSecond);
    }
    private void DistanceCheck()
    {
        if (player == null)
            return;
        if (Vector3.Distance(player.transform.position, transform.position) <= activeDistance)
        {
            if (!interactable)
            {
                interactable = true;
                InitializeUI();
            }
        }
        else
        {
            interactable = false;
            if (popup != null)
                Destroy(popup);
        }

    }
    private void InitializeUI()
    {
        Vector2 screenPos = CalcScreenPos();
        popup = (GameObject)GameObject.Instantiate(Resources.Load("UI/InteractPopup"), screenPos, Quaternion.identity);
        popup.GetComponentInChildren<Text>().text = PopText;
        SetScreenPos(screenPos);
    }
    private Vector2 CalcScreenPos()
    {
        Vector2 pos;
        Vector2 headScreenPos;
        headScreenPos = canvasRef.worldCamera.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 1.5f, 0));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRef.transform as RectTransform, headScreenPos, canvasRef.worldCamera, out pos);
        return pos;
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
            Vector2 screenPos = CalcScreenPos();
            popup.transform.localPosition = screenPos;
            SetScreenPos(screenPos);
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
            popup.GetComponentInChildren<Text>().text = newtext;
            PopText = newtext;
        }       
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
