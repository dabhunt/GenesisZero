using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public Dialogue dialogue;

    //public Text nameText;
    public Text dialogueText;
    public GameObject canv;

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
        sentences = new Queue<string>();
        durations = new Queue<string>();
    }

    public void TriggerDialogue(string filePath)
    {
        /*
         * read each line and store each entity separated by a ';' or '\n'
         * first label = text
         * second label = duration of text
         * third label = voice clip if available
         * assumptions: first two labels always exist, third label may not exist,
         * */

        string path = "Assets/Resources/Dialogue/";
        path += filePath;
        StreamReader reader = new StreamReader(path);

        int count = 0;
        string line;
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
            dialogue.sentences.Enqueue(output[0]);
            dialogue.durations.Enqueue(output[1]);
            count++;
            if (output.Length > 2)
                AudioManager.instance.PlaySound(output[2]);
                //Debug.Log(output[2]);
        }

        reader.Close();
        instance.StartDialogue(dialogue);
    }

    public void StartDialogue (Dialogue dialogue)
    {
        //nameText.text = dialogue.name;

        sentences.Clear();
        durations.Clear();

        while(dialogue.sentences.Count > 0) 
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

        string sentence = sentences.Dequeue();
        string duration = durations.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, duration));
        
    }

    IEnumerator TypeSentence(string sentence, string dur)
    {
        dialogueText.text = "";
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
        canv.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
