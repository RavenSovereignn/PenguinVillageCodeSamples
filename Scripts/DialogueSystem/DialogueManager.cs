using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    public GameObject dialogueUI;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image charIcon;
    public DonaldHouseManager donaldHouseManager;

    public bool talking;

    public bool dialogueActive;
    public float dialogueStartTime;

    private List<DialogueTrigger> dialogueTriggers;
    private Dictionary<string,Color> colorToName = new Dictionary<string, Color>();
    private PlayerManager playerManager;

    private Dialogue cachedDialogue;
    private int cachedDialogueIndex = 0;
    private Sprite[] cachedIcons;
    private string[] cachedNames;
    private string[] cachedColor;

    private DialogueTrigger.DialogueFinishedCallBack finsishedCallBack;
    private IEnumerator SentenceTypying;
    private string currentSentence;
    private bool currentlyTyping = false;


    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        colorToName.Add("Susan", new Color32(131, 255, 154, 255));
        colorToName.Add("Daisy", new Color32(255, 131, 192, 255));
        colorToName.Add("King", new Color32(255, 254, 131, 255));
        colorToName.Add("Mary", new Color32(175, 131, 255, 255));
        colorToName.Add("Frederick", new Color32(255, 146, 131, 255));
        colorToName.Add("Donald", new Color32(131, 255, 224, 255));
        colorToName.Add("Charles", new Color32(242, 131, 255, 255));
        colorToName.Add("Jasmine", new Color32(131, 186, 255, 255));
        colorToName.Add("Roger", new Color32(255, 207, 131, 255));
        dialogueTriggers = new List<DialogueTrigger>(FindObjectsOfType<DialogueTrigger>());
        playerManager = FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && dialogueActive)
        {
            DisplayNextSentence2(firstTime: false);
            /*string sentence = sentences.Dequeue();
            if(dialogueText.text == sentence)
            {
                
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = sentence;
            }*/
            
        }
    }
    public void StartDialogue (Dialogue dialogue)
    {
        talking = true;
        cachedDialogue = dialogue;

        dialogueStartTime = Time.time;
        dialogueActive = true;
        playerManager.inDialogue = true;

        //set dialogue UI
        dialogueUI.SetActive(true);
        nameText.text = dialogue.charName[0];
        charIcon.sprite = dialogue.Icons[0];

        //add dialogue to queue
        sentences.Clear();

        for (int i = dialogue.latestDialogueIndex; i < dialogue.sentences.Length; i++) {
            sentences.Enqueue(dialogue.sentences[i]);
        }

        //foreach (string sentence in dialogue.sentences)
        //{
        //    sentences.Enqueue(sentence);
        //}

        //write dialogue 
        DisplayNextSentence(firstTime: true);
    }

    public void StartDialogue2(string[] characterName, Sprite[] characterIcons, List<string> dialogueSenteces, DialogueTrigger.DialogueFinishedCallBack callBack)
    {
        talking = true;
        finsishedCallBack = callBack;
        dialogueActive = true;
        playerManager.inDialogue = true;
        dialogueStartTime = Time.time;

        //set dialogue UI
        dialogueUI.SetActive(true);
        nameText.text = characterName[0];
        charIcon.sprite = characterIcons[0];
        cachedIcons = characterIcons;
        cachedNames = characterName;

        //add dialogue to queue
        sentences.Clear();

        foreach (string sentence in dialogueSenteces) {
            sentences.Enqueue(sentence);
        }

        //write dialogue 
        DisplayNextSentence2(firstTime: true);
    }

    public void DisplayNextSentence(bool firstTime)
    {
        if(!firstTime && Time.time - dialogueStartTime < 0.1) {
            return;
        }
        if(sentences.Count == 0)
        {
            EndDialogue();
            //FindObjectOfType<KingsQuests>().readyToStartQuest = true;
            FindObjectOfType<KingsQuests>().startQuest = true;

            SetFinishedDialogues();

            return;
        }

        string sentence = sentences.Dequeue();


        //doesn't use this but leaving it, in case it breaks something
        if(sentence.Contains("<Quest>") ) {
            EndDialogue();
            FindObjectOfType<KingsQuests>().readyToStartQuest = true;
            FindObjectOfType<KingsQuests>().startQuest = true;
            playerManager.inQuestScreen = true;

            return;
        }


        dialogueText.text = sentence;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        cachedDialogueIndex++;
    }

    public void DisplayNextSentence2(bool firstTime)
    {
        if (!firstTime && Time.time - dialogueStartTime < 0.1) {
            return;
        }
        
        //currently typing so finish the sentence
        if (currentlyTyping) {
            StopCoroutine(SentenceTypying);
            dialogueText.text = currentSentence;
            currentlyTyping = false;
            return;
        }

        if (sentences.Count == 0) {
            EndDialogue();
            finsishedCallBack.Invoke(DialogueReturnState.Finished);
            return;
        }

        string sentence = sentences.Dequeue();
        currentSentence = sentence;

        if (sentence.Contains("<IMG>")) {
            //find the index of the icon
            string iconString = "" + sentence[5];
            int iconIndex = int.Parse(iconString);
            charIcon.sprite = cachedIcons[iconIndex - 1];
            
            //load the next sentence in
            if (sentences.Count == 0) {
                EndDialogue();
                finsishedCallBack.Invoke(DialogueReturnState.Finished);
                return;
            }
            sentence = sentences.Dequeue();
            currentSentence = sentence;
        }
        if (sentence.Contains("<NAME>"))
        {
            //find the index of the name
            string nameString = "" + sentence[6];
            int nameIndex = int.Parse(nameString);
            nameText.text = cachedNames[nameIndex - 1];
            ChangeNameColour();
            //load the next sentence in
            if (sentences.Count == 0)
            {
                EndDialogue();
                finsishedCallBack.Invoke(DialogueReturnState.Finished);
                return;
            }
            sentence = sentences.Dequeue();
            currentSentence = sentence;
            
        }
        if (sentence.Contains("<DonaldUI>"))
        {
            donaldHouseManager.ActiveUI();
            if (sentences.Count == 0)
            {
                EndDialogue();
                finsishedCallBack.Invoke(DialogueReturnState.Finished);
                return;
            }
            sentence = sentences.Dequeue();
            currentSentence = sentence;
        }
        //start typing next sentence
        SentenceTypying = TypeSentence(sentence);
        StartCoroutine(SentenceTypying);
    }


    IEnumerator TypeSentence (string sentence)
    {
        currentlyTyping = true;
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text+= letter;
            yield return new WaitForSeconds(0.03f);
        }
        currentlyTyping = false;
    }

    public void ChangeNameColour()
    {
        nameText.color = colorToName[nameText.text];
    }

    public void EndDialogue()
    {
        talking = false;
        dialogueUI.SetActive(false);
        dialogueActive = false;
        playerManager.inDialogue = false;

        if(cachedDialogue != null) {
            cachedDialogue.latestDialogueIndex = cachedDialogueIndex;
        }
    }

    private void SetFinishedDialogues()
    {
        foreach (var trigger in dialogueTriggers){
            if (trigger.talking) {
                trigger.talkedAlready = true;
            }
        }

    }
}
