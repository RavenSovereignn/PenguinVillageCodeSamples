using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject player;
    public PlayerManager playerManager;
    public QuestManager questManager;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject controlScheme;
    public bool isPausedMenu;
    [SerializeField] private GameObject volumeSettingUI;
    [SerializeField] private GameObject youWinUI;
    public GameObject youDiedUI;
    public AudioManager audioManager;
    public GameObject playerUI;
    public DialogueManager dialogueManager;
    //public PlayerHealth youDiedBool;
    private bool youdiedboolean;
    private bool youwin = false;

    [SerializeField] private GameObject InventoryMenuUI;
    [SerializeField] private GameObject QuestIconUI;
    public InventoryManager inventoryManager;
    [SerializeField]
    public bool isPausedInventory;
    public Penguin3DController penguin3DController;
    public bool playerIsInUI = false;
    public bool tutorialActive = false;


    public Transform Player;
    public Transform spawnPoint;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
       
        //youdiedboolean = youDiedBool.youdiedbool;
        if (Input.GetKeyDown(KeyCode.Escape) && isPausedInventory == false && playerIsInUI == false && dialogueManager.talking == false)
        {
            isPausedMenu = !isPausedMenu;
            //Debug.Log("Case 1");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            isPausedInventory = !isPausedInventory;
           // Debug.Log("Case 2");
        }
        if (Input.GetKeyDown(KeyCode.Escape) && isPausedInventory == true)
        {
            DeactivateInventoryMenu();
            
           // Debug.Log("Case 1");
        }
        if (Input.GetKeyDown(KeyCode.Escape) && playerIsInUI == true)
        {
            DeactivateMenu();
            DeactivateQuest();

             Debug.Log("Case 2");
        }
        if (isPausedInventory && isPausedMenu == false)
        {
            
            ActivateInventoryMenu();
            
            //Debug.Log("Case 3");
        }
        if(!isPausedInventory && isPausedMenu == false)
        {
            
            DeactivateInventoryMenu();
            //Debug.Log("Case 4");
        }
        if (isPausedMenu && !youwin && isPausedInventory == false)
        {
            ActivateMenu();
            //Debug.Log("Case 5");
        }
        if(!isPausedMenu && isPausedInventory ==false)
        {
            DeactivateMenu();
            //Debug.Log("Case 6");
        }
        if(InventoryMenuUI.activeSelf == true)
        {
            audioManager.Play("BackpackOpen");
            //Debug.Log("case open");
        }
        if (InventoryMenuUI.activeSelf == false && tutorialActive == false)
        {
            audioManager.Play("BackpackClose");
            //Debug.Log("case closed");
        }
    }
    
    public void DiablePlayerHUD()
    {
        playerUI.SetActive(false);

    }
    public void EnablePlayerHUD()
    {
        playerUI.SetActive(true);

    }

    void ActivateInventoryMenu()
    {
       
        InventoryMenuUI.SetActive(true);
        //playerUI.SetActive(false);
        QuestIconUI.SetActive(false);
        inventoryManager.ListItems();
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }
    public void DeactivateQuest()
    {
        questManager.DisableUI();
    }
    public void DeactivateInventoryMenu()
    {
        
        Time.timeScale = 1;
        InventoryMenuUI.SetActive(false);
        QuestIconUI.SetActive(true);
        //playerUI.SetActive(true);
        if (!playerIsInUI) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }

        isPausedInventory = false;
        
    }
    public void YouWinScreen()
    {
        youwin = true;
        isPausedMenu = true;
        Time.timeScale = 0;
        youWinUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ActivateMenu ()
    {
        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void youDiedScreen(bool state)
    {
        Time.timeScale = (state) ? 0 : 1;
        youDiedUI.SetActive(state);
        Cursor.lockState = (state) ? CursorLockMode.None : CursorLockMode.Confined;
        Cursor.visible = state;
    }

    public void DeactivateMenu ()
    {
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
       
        youWinUI.SetActive(false);

        if (!playerIsInUI) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        //when in tutorial stop time
        if (tutorialActive)
        {
            Time.timeScale = 0;
        }

        isPausedMenu = false;
        youwin = false;
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Restart()
    {
        SceneManager.LoadScene(1);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Controls()
    {
        controlScheme.SetActive(true);
        pauseMenuUI.SetActive(false);
    }
    public void ControlsBack()
    {
        pauseMenuUI.SetActive(true);
        controlScheme.SetActive(false);
    }
    public void Options()
    {
        pauseMenuUI.SetActive(false);
        volumeSettingUI.SetActive(true);
    }
    public void OptionsBack()
    {
        volumeSettingUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
    //player options(later one)
    public void Recall()
    {
        Player.position = spawnPoint.position;
        penguin3DController.gameObject.SetActive(true);
        penguin3DController.swimming = false;
        penguin3DController.walking = true;
        penguin3DController.rb.useGravity = true;
        penguin3DController.rb.isKinematic = false;
        penguin3DController.MovementRestricted = false;
        dialogueManager.EndDialogue();
        playerManager.ResetToPlayerCamera();
    }
}
