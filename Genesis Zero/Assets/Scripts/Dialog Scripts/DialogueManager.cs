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
    private Dictionary<string, int> dialoguePlayed; 
    private int[] interactions;
    private int currentType = -1;
    private bool canSkip = true;
    private bool deQueueOnFinish = false;
    private BUGE buge;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        dialoguePlayed = new Dictionary<string, int>();
        charIcons = new Queue<string>();
        sentences = new Queue<string>();
        durations = new Queue<string>();
        //DontDestroyOnLoad(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        interactions = new int[3];
        instance.EndDialogue();
        //instance.TriggerDialogue("StartDialogue");
        buge = GameObject.FindWithTag("BUG-E").GetComponent<BUGE>();
        //StateManager.instance.PauseGame();
    }
    //takes the name of the txt file, and a bool of if it should dequeue bug-e waypoints upon completion
    public void TriggerDialogue(string name, bool pauseGame, bool deQueue)
    {
        /*
        * read each line and store each entity separated by a ';' or '\n'
        * first label = character saying the text
        * second label = text
        * third label = duration of text (-1 means it will wait until player input to get to the next sentence)
        * (optional) fourth label = voice clip if available
        * assumptions: first 3 labels always exist, fourth label may not exist,
        * */
        if (StateManager.instance.IsPaused() == true || pauseGame == true)
            Cursor.visible = true;
        StopAllCoroutines(); //makes sure no other dialogue will mess with new dialogue
        deQueueOnFinish = deQueue;
        TextAsset ta = (TextAsset)Resources.Load("Dialogue/" + name);
        if (ta == null)
            return;
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
        if (dialoguePlayed.ContainsKey(name))
            dialoguePlayed[name] = dialoguePlayed[name] + 1;
        else
            dialoguePlayed.Add(name, 1);

        StartDialogue(dialogue);
    }
    public void TriggerDialogue(string name, bool pauseGame)
    {
        TriggerDialogue(name, pauseGame, false);
    }
    public void TriggerDialogue(string name)
    {
        TriggerDialogue(name, false, false);
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
        string charIcon = charIcons.Dequeue();
        string sentence = sentences.Dequeue();
        string duration = durations.Dequeue();
        canSkip = false;
        StartCoroutine(CanSkip(.8f));
        StartCoroutine(TypeSentence(charIcon, sentence, duration));
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
    IEnumerator CanSkip(float delay)
    {
        yield return StartCoroutine(WaitForRealSeconds(delay));
        canSkip = true;
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
        //if the type of interaction needs an unpause
        if (currentType == -1)
        {
            StateManager.instance.UnpauseGame();

        }
        Cursor.visible = false;
        if (deQueueOnFinish)
            buge.DequeueWayPoint();
        InvokeAfterDialogue(currentType);
        StopAllCoroutines();
        parent.SetActive(false);
        
    }
    public void InvokeAfterDialogue(int type)
    {
        if (type == -1)
            return;
        switch (type) 
        {
            case 0: //Merchant interaction trigger
                InteractInterface.instance.ClosestInteractable().GetComponent<Merchant>().AfterDialogue();
                break;
            case 1: //GodHead interaction trigger
                InteractInterface.instance.ClosestInteractable().GetComponent<GodHead>().AfterDialogue();
                break;
            case 2: //Snakeboss interaction trigger
                break;
            default:
                break;
        }
        //reset currentType to a null type
        currentType = -1;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (canSkip)
                DisplayNextSentence();
        }
    }
    //returns the amount of times the player has interacted with a type of game object
    //0 is merchant, 1 is fallen god, 2 is boss.
    public int GetInteractAmount(int type)
    {
        return interactions[type];
    }
    //returns the amount of times that dialoguename has played
    public int GetDialoguePlayedAmount(string name) 
    {
        int newin;
        if (!dialoguePlayed.TryGetValue(name, out newin))
            return 0;
        return dialoguePlayed[name];
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
    //prevent player from pressing F to skip
    public void SetSkip(bool boo)
    {
        canSkip = boo;
    }
}
