using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeTutorial : MonoBehaviour
{
    public PlayerManager playerManager;
    public TutorialManager tutorialManager;
    public WardrobeInventory wardrobeInventory;

    private bool tutorialStared = false;
    private bool startTutorialNext = false;

    public static bool inTutorial = false;

    void Update()
    {
        if (!tutorialStared && startTutorialNext && playerManager.inDialogue == false) {
            FindObjectOfType<PickupPopUp>().AddPopUp(wardrobeInventory.GetWardrobeItem(CustomizationGift.IndieHat));
            tutorialManager.WardrobeTutorialStart();
            tutorialStared = true;
            inTutorial = true;
        }

    }

    public void RecievedAccessoryItem(CustomizationGift accessory)
    {
        if (!tutorialStared) {
            if (accessory == CustomizationGift.IndieHat) {
                startTutorialNext = true;
            }
        }
    }

}
