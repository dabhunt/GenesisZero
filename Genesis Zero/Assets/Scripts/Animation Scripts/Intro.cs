using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    public float durationPerCard;
    public float fadeDuration;
    // Start is called before the first frame update
    private Queue<GameObject> cardPrefabs;
    void Start()
    {
        //GameObject[] objs = Resources.LoadAll<GameObject>("Intro");
        //for (int i = 0; i < objs.Length; i++)
        //{
        //    cardPrefabs.Enqueue(objs[i]);
        //}
    }
    public void NextCard()
    {
        
    }
}
