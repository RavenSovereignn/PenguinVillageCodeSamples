using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{

    public bool resetBubble;
    public bool resetTree;
    public bool resetLog;
    public bool resetSeaweed;
    [SerializeField] private GameObject[] Bubbles;
    [SerializeField] private GameObject[] Tree;
    [SerializeField] private GameObject[] TreeLog;
    [SerializeField] private GameObject[] Seaweed;

    private void Start()
    {
        resetBubble = true;
        resetTree = true;
        resetSeaweed = true;
        Seaweed = GameObject.FindGameObjectsWithTag("Seaweed");
        Bubbles = GameObject.FindGameObjectsWithTag("Bubble");
        Tree = GameObject.FindGameObjectsWithTag("Wood");
        TreeLog = GameObject.FindGameObjectsWithTag("WoodLog");
    }
    // Update is called once per frame
    void Update()
    {
        if (resetBubble)
        {
            resetBubble = false;
            StartCoroutine(TurnOnBubbles());
            //Debug.Log("Reseting bubbles");
        }
        if (resetTree)
        {
            resetTree = false;
            StartCoroutine(TurnOnTrees());

        }
        if (resetLog)
        {
            resetLog = false;
            StartCoroutine(TurnOnLogs());
        }
        if (resetSeaweed)
        {
            resetSeaweed = false;
            StartCoroutine(TurnOnSeaweed());
        }
    }
    public IEnumerator TurnOnBubbles()
    {

        
        for(int i = 0; i < Bubbles.Length; i++)
        {
            Bubbles[i].SetActive(true);
        }
        yield return new WaitForSeconds(90f);
        resetBubble = true;
    }
    public IEnumerator TurnOnTrees()
    {
        for (int i = 0; i < Tree.Length; i++)
        {
            Tree[i].GetComponent<WoodPickup>().WoodPickedUp = false;
        }
        yield return new WaitForSeconds(120f);
        resetTree = true;
    }
    public IEnumerator TurnOnLogs()
    {
        for (int i = 0; i < TreeLog.Length; i++)
        {
            TreeLog[i].SetActive(true);
        }
        yield return new WaitForSeconds(120f);
        resetTree = true;
    }
    public IEnumerator TurnOnSeaweed()
    {
        for (int i = 0; i < Seaweed.Length; i++)
        {
            Seaweed[i].SetActive(true);
        }
        yield return new WaitForSeconds(120f);
         resetSeaweed = true;
    }
}
