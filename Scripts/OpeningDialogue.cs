using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningDialogue : MonoBehaviour {

    [Header("UI References")]
    public GameObject dialogueUI;
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI dialogueText;
    public Image charIcon;

    public Image fade;

    [Header("References")]
    public Dialogue dialogue;
    public Image SkipImage;

    private Queue<string> sentences;

    private Dictionary<string, Color> colorToName = new Dictionary<string, Color>();

    private Sprite[] cachedIcons;
    private string[] cachedNames;
    private string[] cachedColor;

    private float dialogueStartTime = 0;
    private bool dialogueActive = false;

    private IEnumerator SentenceTypying;
    private string currentSentence;
    private bool currentlyTyping = false;

    private void Start()
    {
        colorToName.Add("Susan", new Color32(131, 255, 154, 255));
        colorToName.Add("Daisy", new Color32(255, 131, 192, 255));
        colorToName.Add("King", new Color32(255, 254, 131, 255));
        colorToName.Add("Mary", new Color32(175, 131, 255, 255));
        colorToName.Add("Frederick", new Color32(255, 146, 131, 255));
        colorToName.Add("Donald", new Color32(131, 255, 224, 255));
        colorToName.Add("Charles", new Color32(242, 131, 255, 255));
        colorToName.Add("Jasmine", new Color32(131, 186, 255, 255));
        colorToName.Add("Roger", new Color32(255, 207, 131, 255));

        sentences = new Queue<string>();
    }
    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && dialogueActive) {
            DisplayNextSentence(firstTime: false);
        }
    }

    public void TriggerDialogue(int dialogueStartIndex)
    {
        SkipImage.gameObject.SetActive(false);

        List<string> dialogueTrimmed = new List<string>();
        int i = 0;
        for (i = dialogueStartIndex; i < dialogue.sentences.Length; i++) {
            dialogueTrimmed.Add(dialogue.sentences[i]);
        }

        StartDialogue(dialogue.charName, dialogue.Icons, dialogueTrimmed);
    }


    public void StartDialogue(string[] characterName, Sprite[] characterIcons, List<string> dialogueSenteces)
    {
        dialogueActive = true;
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
        DisplayNextSentence(firstTime: true);
    }

    public void DisplayNextSentence(bool firstTime)
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
                return;
            }
            sentence = sentences.Dequeue();
            currentSentence = sentence;
        }
        if (sentence.Contains("<NAME>")) {
            //find the index of the name
            string nameString = "" + sentence[6];
            int nameIndex = int.Parse(nameString);
            nameText.text = cachedNames[nameIndex - 1];
            ChangeNameColour();
            //load the next sentence in
            if (sentences.Count == 0) {
                EndDialogue();
                return;
            }
            sentence = sentences.Dequeue();
            currentSentence = sentence;
        }
        //start typing next sentence
        SentenceTypying = TypeSentence(sentence);
        StartCoroutine(SentenceTypying);
    }


    IEnumerator TypeSentence(string sentence)
    {
        currentlyTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
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
        dialogueUI.SetActive(false);
        dialogueActive = false;

        StartCoroutine(StartMainGame());
    }

    IEnumerator StartMainGame()
    {
        yield return new WaitForSeconds(0.5f);


        for (int i = 0; i < 100; i++) {
            Color colour = fade.color;
            colour.a += 0.01f;
            fade.color = colour;
            yield return new WaitForSeconds(0.01f);
        }

        SceneManager.LoadScene(1);
    }

}