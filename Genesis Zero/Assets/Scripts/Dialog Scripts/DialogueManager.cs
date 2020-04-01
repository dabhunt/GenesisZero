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
    public Image  charImage;
    public GameObject parent;
    private Sprite[] iconArray;
    private Queue<string> charIcons;
    private Queue<string> sentences;
    private Queue<string> durations;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        charIcons = new Queue<string>();
        sentences = new Queue<string>();
        durations = new Queue<string>();
        instance.EndDialogue();
        instance.TriggerDialogue("TestDialog.txt");
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

        string path = "Assets/Resources/Dialogue/";
        path += filePath;
        StreamReader reader = new StreamReader(path);

        int count = 0;
        string line;
        dialogue.charIcons = new Queue<string>();
        dialogue.sentences = new Queue<string>();
        dialogue.durations = new Queue<string>();

        /*string text = reader.ReadToEnd();
        string[] lines = text.Split(';','\n');
        foreach(string s in lines)
        {
            if (IsNumeric(s))
            {
                Debug.Log(s + " is a valid number");
            }
            else if(s.Contains("_"))
            {
                Debug.Log(s + " is an audio label");
            }
            else
            {
                Debug.Log(s);
            }

        }*/

        while ((line = reader.ReadLine()) != null)
        {
            string[] output = line.Split(';');
            dialogue.charIcons.Enqueue(output[0]);
            dialogue.sentences.Enqueue(output[1]);
            dialogue.durations.Enqueue(output[2]);
            count++;
            if (output.Length > 3)
                AudioManager.instance.PlaySound(output[3]);
                //Debug.Log(output[2]);
        }
        reader.Close();
        instance.StartDialogue(dialogue);
    }

    public void StartDialogue (Dialogue dialogue)
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
        //Sprite spr = null;
        //print("spr.name: " + spr.name);
        if (spr != null)
        {
            charImage.sprite = spr;
        }
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        if (float.Parse(dur) != -1)
        {
            yield return new WaitForSeconds(float.Parse(dur));
            DisplayNextSentence();
        }
    }

    void EndDialogue()
    {
        parent.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            DisplayNextSentence();
        }
    }
}
