using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public Dialogue dialogue;

    //public Text nameText;
    public TextMeshProUGUI dialogueText;
    public Image charImage;
    public GameObject parent;
    private Sprite[] iconArray;
    private Queue<string> charIcons;
    private Queue<string> sentences;
    private Queue<string> durations;
    private int[] interactions;
    private int currentType = -1;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        //DontDestroyOnLoad(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        charIcons = new Queue<string>();
        sentences = new Queue<string>();
        durations = new Queue<string>();
        interactions = new int[3];
        instance.EndDialogue();
        instance.TriggerDialogue("StartDialogue");
        StateManager.instance.PauseGame();
    }

    public void TriggerDialogue(string filePath)
    {
        /*
         * read each line and store each entity separated by a ';' or '\n'
         * first label = character saying the text
         * second label = text
         * third label = duration of text (-1 means it will wait until player input to get to the next sentence)
         * (optional) fourth label = voice clip if available
         * assumptions: first 3 labels always exist, fourth label may not exist,
         * */
        TextAsset ta = (TextAsset)Resources.Load("Dialogue/"+filePath);
        int count = 0;
        dialogue.charIcons = new Queue<string>();
        dialogue.sentences = new Queue<string>();
        dialogue.durations = new Queue<string>();
        var arrayString = ta.text.Split('\n');
        foreach (var line in arrayString)
        {
            string[] output = line.Split(';');
            dialogue.charIcons.Enqueue(output[0]);
            dialogue.sentences.Enqueue(output[1]);
            dialogue.durations.Enqueue(output[2]);
            count++;
            if (output.Length > 3)
                AudioManager.instance.PlaySound(output[3]);
        }
        StartDialogue(dialogue);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        //nameText.text = dialogue.name;
        parent.SetActive(true);
        charIcons.Clear();
        sentences.Clear();
        durations.Clear();
        while (dialogue.charIcons.Count > 0)
        {
            charIcons.Enqueue(dialogue.charIcons.Dequeue());
        }
        while (dialogue.sentences.Count > 0)
        {
            sentences.Enqueue(dialogue.sentences.Dequeue());
        }
        while (dialogue.durations.Count > 0)
        {
            durations.Enqueue(dialogue.durations.Dequeue());
        }
        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string charIcon = charIcons.Dequeue();
        string sentence = sentences.Dequeue();
        string duration = durations.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(charIcon, sentence, duration));
    }

    IEnumerator TypeSentence(string charIcon, string sentence, string dur)
    {
        dialogueText.text = "";
        //names in this folder HAVE to match the .txt file names of characters in order to load
        charIcon.ToUpper();
        Sprite spr = Resources.Load<Sprite>("Dialogue/CharacterIcons/" + charIcon);
        if (spr != null)
        {
            charImage.sprite = spr;
        }
        if (spr != null)
        {
            charImage.sprite = spr;
        }
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return StartCoroutine(WaitForRealSeconds(.03f));
        }
        if (float.Parse(dur) != -1)
        {
            yield return StartCoroutine(WaitForRealSeconds(float.Parse(dur)));
            DisplayNextSentence();
        }
    }
      public static IEnumerator WaitForRealSeconds( float delay )
     {
         float start = Time.realtimeSinceStartup;
         while( Time.realtimeSinceStartup < start + delay)
         {
             yield return null;
         }
     }
    public void SkipAll()
    {
        //possibly add a check to see if the dialogue is unskippable
        EndDialogue();
    }
    private void EndDialogue()
    {
        InvokeAfterDialogue(currentType);
        StateManager.instance.UnpauseGame();
        parent.SetActive(false);
    }
    public void InvokeAfterDialogue(int type)
    {
        if (type == -1)
            return;
        switch (type) 
        {
            case 0: //Merchant interaction trigger
                GetComponent<InteractInterface>().ClosestInteractable().GetComponent<Merchant>().AfterDialogue();
                break;
            case 1: //GodHead interaction trigger
                GetComponent<InteractInterface>().ClosestInteractable().GetComponent<GodHead>().AfterDialogue();
                break;
            case 2: //Snakeboss interaction trigger
                break;
            default:
                break;
        }
        //reset currentType
        currentType = -1;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            DisplayNextSentence();
        }
    }
    //returns the amount of times the player has interacted with a type of game object
    //0 is merchant, 1 is fallen god, 2 is boss.
    public int GetInteractAmount(int type)
    {
        return interactions[type];
    }
    //change currentType so that invokeAfterDialogue plays the right dialogue
    public void SetInteractionAfterDialogue(int type)
    {
        currentType = type;
    }
    //adds 1 to interactions of that type of game object
    public void IncrementInteract(int type)
    {
        interactions[type] ++;
    }
}
