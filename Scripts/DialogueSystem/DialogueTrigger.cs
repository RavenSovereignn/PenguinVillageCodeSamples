using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//state of dialogue sent back to dialogue trigger
public enum DialogueReturnState { Finished, Cancelled }

//state of quest when closing the ui 
public enum QuestReturnState { Completed, Accepted, Declined }

public class DialogueTrigger : MonoBehaviour
{
    public delegate void DialogueFinishedCallBack(DialogueReturnState finishedState);

    [Header("Dialogue settings")]
    public Dialogue dialogue;
    public bool repeatable = false;

    [Header("Small Talk")]
    [NonReorderable, TextArea(3, 10)] public List<string> HouseBuiltTalks;
    [HideInInspector] public bool shouldThankForHouse = false;

    [NonReorderable, TextArea(3, 10)] public List<string> Level1SmallTalks;
    [NonReorderable, TextArea(3, 10)] public List<string> Level2SmallTalks;
    [NonReorderable, TextArea(3, 10)] public List<string> Level3SmallTalks;

    private List<bool> smallTalksCompletion = new List<bool> { false, false, false };

    [Header("UI")]
    public GameObject DialogueStartUI;
    public string DialogueStartText;
    public DialogueManager dialogueManager;
    public PauseMenu pauseMenu;

    [Header("References")]
    public Cinemachine.CinemachineVirtualCamera dialogueCamera;
    public GameObject questmark;
    public GameObject questmarkAccepted;
    public GameObject smallTalkMark;
    public EndGameCinematic endGameScript;

    public QuestManager questManager;
    private PlayerManager playerManager;
    private IQuestGiver questScript;

    private bool playerInRange;
    public bool talkedAlready = false;
    public bool talking = false;
    private bool playerOnQuest = false;
    private bool showQuestNext = false;
    private bool lastDialogue = false;
    private bool breakDialogueOnEnd = false;
    private bool enableQuestOnEnd = false;
    private bool smallTalking = false;
    private bool ignoreNextFrame = false;
    private bool finishGameOnEnd = false;

    public bool isitDonald;
    [HideInInspector]
    public DonaldHouseManager donaldHouseManager;
    public PickupChick pickupChick;
    [HideInInspector]public int dialogueTracker = 0;
    private int currentDialogueLength = 0;

    private Action cachedCamSwitchCallBack;

    private EndGameCinematic ending;

    //public bool debug =false;

    private void Start()
    {
        pickupChick = FindObjectOfType<PickupChick>();
        donaldHouseManager = FindObjectOfType<DonaldHouseManager>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        playerManager = FindObjectOfType<PlayerManager>();
        questScript = GetComponent<IQuestGiver>();
        ending = FindObjectOfType<EndGameCinematic>();

        SetupMarks();

        ProgressManager.LevelPogressed += SetupMarks;
    }

    private void OnDestroy()
    {
        ProgressManager.LevelPogressed -= SetupMarks;
    }

    private void Update()
    {
        //if(debug)Debug.Log($"in range:{playerInRange}, {!dialogueManager.dialogueActive}, {!playerOnQuest}, {!showQuestNext}");
        if (Input.GetKeyDown(KeyCode.T)) { shouldThankForHouse = true; }

        if(InputAccept() && playerInRange && !dialogueManager.dialogueActive && !playerOnQuest && !showQuestNext
            && !ignoreNextFrame && donaldHouseManager.DonaldUIActive == false && !WardrobeTutorial.inTutorial
            && !ending.cinematicActive)
        {
            //Debug.Log("Dialogue Start Triggered");
            //TriggerDialogue();
            TriggerDialogue2(dialogueTracker);
        }
        if(talking == true && playerInRange==false|| Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I))
        {
            StopTalking();   
        }
        if (questManager.UIAtive) {
            DialogueStartUI.SetActive(false);
        }

        if (ignoreNextFrame) { ignoreNextFrame = false; }
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

        DialogueStartUI.SetActive(false);
        talking = true;
    }

    public void TriggerDialogue2(int dialogueStartIndex)
    {
        playerManager.PlayIdle();
        playerManager.StopPlayerAudio();

        if (shouldThankForHouse) { ThankHouseDialogue(); return; }

        List<string> dialogueTrimmed = new List<string>();
        currentDialogueLength = 0;

        int i = 0;
        for (i = dialogueStartIndex; i < dialogue.sentences.Length; i++) {
            if(dialogue.sentences[i].Contains("<Quest>")) {
                //starts a quest at the end of these sentences
                enableQuestOnEnd = true;
                break;
            }
            if (dialogue.sentences[i].Contains("<Stop>")) {
                //stops the dialogue at the end of these sentences
                breakDialogueOnEnd = true;
                break;
            }
            if (dialogue.sentences[i].Contains("NextChar"))
            {
                break;
            }
            if (dialogue.sentences[i].Contains("<Level>")) {
                //get level required to pass this dialogue
                int levelRequired =  int.Parse("" + dialogue.sentences[i][7]);

                //stop dialogue if current level is too low
                if(levelRequired <= ProgressManager.CurrentLevel) {
                    currentDialogueLength++;
                    continue;
                }
                else {
                    break;
                }
            }
            //trigger end cinematic and credits
            if (dialogue.sentences[i].Contains("<EndGame>")) {
                if(!endGameScript.endingPlayed) {
                    finishGameOnEnd = true;
                }
                else {
                    breakDialogueOnEnd = true;
                }

                break;
            }
            dialogueTrimmed.Add(dialogue.sentences[i]);
        }
        currentDialogueLength += dialogueTrimmed.Count + 1;

        if(i == dialogue.sentences.Length) {
            lastDialogue = true;
            //Debug.Log("End of dialogue");
        }
        if (dialogueTrimmed.Count == 0 && !enableQuestOnEnd && !breakDialogueOnEnd) {
            dialogueTrimmed = (ProgressManager.CurrentLevel == 0) ? Level1SmallTalks : Level2SmallTalks;
            smallTalking = true;
            lastDialogue = false;    
        }


        //switch to dialogue camera, keep callback to reset camera when finsihed dialogue
        cachedCamSwitchCallBack = playerManager.SetActiveCamera(dialogueCamera);
        //dont drown when talking to npcs underwater
        playerManager.ToggleUnderWaterBreathMeter(false);

        FindObjectOfType<DialogueManager>().StartDialogue2(dialogue.charName, dialogue.Icons, dialogueTrimmed, DialogueFinished);

        DialogueStartUI.SetActive(false);
        talking = true;    
    }

    private void ThankHouseDialogue()
    {
        List<string> dialogueTrimmed = new List<string>(HouseBuiltTalks);
        breakDialogueOnEnd = true;

        cachedCamSwitchCallBack = playerManager.SetActiveCamera(dialogueCamera);
        FindObjectOfType<DialogueManager>().StartDialogue2(dialogue.charName, dialogue.Icons, dialogueTrimmed, DialogueFinished);
        DialogueStartUI.SetActive(false);
        talking = true;

        shouldThankForHouse = false;
    }

    //called by dialogue manager after the player finishes this dialogue
    private void DialogueFinished(DialogueReturnState finishedState)
    {
        playerManager.ToggleUnderWaterBreathMeter(true);

        if (finishGameOnEnd) {
            dialogueTracker += currentDialogueLength;
            playerOnQuest = false;
            cachedCamSwitchCallBack.Invoke();
            breakDialogueOnEnd = false;
            ignoreNextFrame = true;

            if (smallTalkMark != null) { smallTalkMark.SetActive(false); }
            if (questmark != null) { questmark.SetActive(false); }

            OtherScriptsHacks();

            endGameScript.BeginEndOfGame();

            finishGameOnEnd = false;
        }
        //stop the dialogue when requested.
        else if (breakDialogueOnEnd) {
            //move dialogue along so next time we speak it will start after the <Stop> command
            dialogueTracker += currentDialogueLength;
            playerOnQuest = false;

            cachedCamSwitchCallBack.Invoke();
            breakDialogueOnEnd = false;
            ignoreNextFrame = true;

            OtherScriptsHacks();

            if(dialogueTracker == dialogue.sentences.Length && repeatable) {
                SetupRepeating();
            }
        }
        //finished entire dialogue list
        else if (lastDialogue) {
            cachedCamSwitchCallBack.Invoke();
            
            //if marked repeatable this will reset needed vars
            if (repeatable) {
                SetupRepeating();
            }
        }
        else if(enableQuestOnEnd) {
            questScript.EnableQuest(QuestFinishedCallback);
            showQuestNext = true;
            enableQuestOnEnd = false;
        }
        //small talking with the npcs, doesn't affect other dialogue 
        else if (smallTalking) {
            Debug.Log("MAde it to the small talking finish");
            cachedCamSwitchCallBack.Invoke();
            ignoreNextFrame = true;
            smallTalking = false;
            smallTalksCompletion[ProgressManager.CurrentLevel] =  true;
            SetupMarks();
        }
        else {
            cachedCamSwitchCallBack.Invoke();
            dialogueTracker += currentDialogueLength - 1;
        }

    }

    private void SetupRepeating()
    {
        //Debug.Log("gets to repeatable");

        talkedAlready = false;
        showQuestNext = false;
        lastDialogue = false;

        dialogueTracker = 0;
        currentDialogueLength = 0;

        SetupMarks();
    }

    private void OtherScriptsHacks()
    {
        if (FindObjectOfType<DonaldHouseManager>().DonaldUIActive) {playerManager.RestricPlayer();}

        FindObjectOfType<MarketTrigger>(true).inDialogue = false;
        FindObjectOfType<MarketTrigger>(true).ignoreOneFrame = true;
    }

    private void QuestFinishedCallback(QuestReturnState state)
    {
        //Debug.Log("Callback");
        
        //switch back to player camera
        cachedCamSwitchCallBack.Invoke();

        if (state == QuestReturnState.Accepted) {
            playerOnQuest = true;
            showQuestNext = false;
            if (questmark != null) { questmark.SetActive(false); }
            if (questmarkAccepted != null) { questmarkAccepted.SetActive(true); }
        }
        if (state == QuestReturnState.Declined) {
            playerOnQuest = false;
            questScript.DisableQuest();
            showQuestNext = false;
        }
        if(state == QuestReturnState.Completed) {
            playerOnQuest = false;
            showQuestNext = false;
            questScript.DisableQuest();

            dialogueTracker += currentDialogueLength;

            if (questmarkAccepted != null) { questmarkAccepted.SetActive(false); }
            SetupMarks();

            FindObjectOfType<MarketTrigger>(true).inDialogue = true;       

            TriggerDialogue2(dialogueTracker);
        }
    }

    public void StopTalking()
    {
        dialogueManager.EndDialogue();
        DialogueStartUI.SetActive(false);
    }

    private bool CheckForMoreQuests(out bool causeOfLevel)
    {
        causeOfLevel = false;
        bool anotherQuest = false;
        for (int i = dialogueTracker; i < dialogue.sentences.Length; i++) {

            if (dialogue.sentences[i].Contains("<Quest>")) {
                anotherQuest = true;
                break;
            }
            if (dialogue.sentences[i].Contains("<Level>")) {
                //get level required to pass this dialogue
                int levelRequired = int.Parse("" + dialogue.sentences[i][7]);

                //stop dialogue if current level is too low
                if (levelRequired <= ProgressManager.CurrentLevel) {
                    continue;
                }
                else {
                    causeOfLevel = true;
                    break;
                }
            }
        }

        return anotherQuest;
    }

    private void SetupMarks()
    {
        bool questAvailable = CheckForMoreQuests(out bool smallTalkCauseLevel);

        if (questmark != null && !questmarkAccepted.activeSelf) { questmark.SetActive(questAvailable);}
        
        //set active if quest is repeatable and not locked by level
        if (repeatable && !smallTalkCauseLevel && !questmarkAccepted.activeSelf) { questmark.SetActive(true); questmarkAccepted.SetActive(false); }

        if (smallTalkMark != null) { smallTalkMark.SetActive(!questAvailable);}

        //disable small talk mark if the quest is repatable and not locked by level
        if(repeatable && !smallTalkCauseLevel) { smallTalkMark.SetActive(false); }

        //if already read the small talk for the current level
        if (ProgressManager.CurrentLevel < 3 && smallTalksCompletion[ProgressManager.CurrentLevel] && smallTalkMark != null) {
            smallTalkMark.SetActive(false);
        }
        //on level 3 check for small talk of level 2
        if(ProgressManager.CurrentLevel == 2 && smallTalksCompletion[1] && smallTalkMark != null) {
            smallTalkMark.SetActive(false);

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            DialogueStartUI.SetActive(true);
            DialogueStartUI.GetComponent<TMPro.TextMeshProUGUI>().text = DialogueStartText;
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
            DialogueStartUI.SetActive(false);
            talking = false;
        }
    }

    //returns true if the player presses accept button or key
    private bool InputAccept()
    {
        if (ProgressManager.InCinemtatic) { return false; }

        if (!talkedAlready ) {
            return Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1");
        }
        return false;
    }

}
