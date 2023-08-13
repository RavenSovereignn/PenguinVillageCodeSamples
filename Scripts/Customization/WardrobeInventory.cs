using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public enum WardrobeItemType {All = 0, Hat = 1, Shirts = 2, Glasses }

public enum CustomizationGift { None = 0, PinkTopHat = 1, QuestHat = 2, Crown = 3, Coral = 4, Fish = 5, Barrel = 6, FishBowTie = 7, Bucket = 8,
Tie = 9, Scarf = 10, PirateHat = 11, Mohawk = 12, Goggles = 13, Glasses = 14, Flower = 15, ShellBra = 16, BlueBow = 17, BlackHat = 18,
HeartGlasses = 19, BaseBallHat = 20, WizardHat = 21, IndieHat = 22
}

public class WardrobeInventory : MonoBehaviour
{
    [Header("Wardrobe")]
    [NonReorderable]
    public List<WardrobeItem> AllWardrobeItems;

    [HideInInspector]
    public List<WardrobeItem> AvailableItems;

    [HideInInspector]
    public List<WardrobeItem> playersWardrobe;

    private List<CustomisationItem> currentlyEquippedItems;

    public Transform playersCustomisationParent;

    [Header("Customisation Points")]
    public Transform HatRootParent;
    public Transform ShirtRootParent;
    public Transform GlassesRootParent;

    //the duplicate penguin in the wardrobe
    public Transform DecoyPenguinHatRootParent;
    public Transform DecoyPenguinShirtRootParent;
    public Transform DecoyPenguinGlassesRootParent;

    [Header("UI References")]
    public Transform UIContentTransform;
    public GameObject WardrobeItemTilePrefab;


    void Start()
    {
        currentlyEquippedItems = new List<CustomisationItem>();

        AvailableItems = new List<WardrobeItem>();

        for (int i = 8; i < AllWardrobeItems.Count; i++) {
            WardrobeItem wItem = AllWardrobeItems[i];
            AvailableItems.Add(wItem.Clone() as WardrobeItem);
        }
        AvailableItems.Add(AllWardrobeItems[0] as WardrobeItem);

        //playersWardrobe.Add(GetWardrobeItem(CustomizationGift.Barrel));
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Equals)) {
            Cheat_AddDansCustomisation();
        }
        
    }

    public void AddWardrobeItemToPlayer(CustomizationGift customisationItem)
    {
        playersWardrobe.Add(GetWardrobeItem(customisationItem));
    }

    public void ListPlayersWardrobe(WardrobeItemType customisationType)
    {
        //clean content before open
        foreach (Transform item in UIContentTransform) {
            Destroy(item.gameObject);
        }

        //set-up display list, shows owned items first
        List<WardrobeItem> orderedDisplayList = new List<WardrobeItem>();
        List<WardrobeItem> unownedItems = new List<WardrobeItem>();

        for (int i = 0; i < AvailableItems.Count; i++) {
            WardrobeItem wItem = AvailableItems[i];

            //if owned add to display 
            if (!playersWardrobe.Any(pItem => pItem.ID == wItem.ID)) { unownedItems.Add(wItem); }
            else { orderedDisplayList.Add(wItem); }

        }
        //add the unowned to the ordered display list, so they're at the bottom of the list
        orderedDisplayList.AddRange(unownedItems);

        //loop over display list
        foreach (var item in orderedDisplayList) {

            bool playerWearingItem = currentlyEquippedItems.Any(pItem => pItem.Item == item.mItem);

            //if specified item type, only show those
            if ((customisationType != WardrobeItemType.All && item.mType != customisationType) || playerWearingItem) {
                continue;
            }

            //create the item ui tile
            GameObject obj = Instantiate(WardrobeItemTilePrefab, UIContentTransform);
            obj.GetComponentsInChildren<Image>()[1].sprite = item.TileIcon;
            obj.GetComponentInChildren<TextMeshProUGUI>().text = item.TileName;

            //setup item description information
            AccessoryDescription description = obj.AddComponent<AccessoryDescription>();
            description.location = item.ItemLocation;
            description.description = item.ItemDescription;
            description.playerOwns = true;

            //if the player doesn't own it fade out and make it un-draggable
            if (!playersWardrobe.Any(pItem => pItem.ID == item.ID)) {
                Destroy(obj.GetComponent<CustomisationItem>());

                description.playerOwns = false;

                obj.GetComponentsInChildren<Image>()[1].color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
                obj.GetComponentInParent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
                obj.GetComponentInParent<Image>().GetComponentInChildren<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f, 1.0f); ;
            }
            else {
                obj.GetComponent<CustomisationItem>().Item = item.mItem;
            }

            
        }
    }

    public void EquipItem(CustomizationGift item)
    {
        Transform equipPoint;

        switch (GetWardrobeItem(item).mType) {
            case WardrobeItemType.Hat:
                equipPoint = HatRootParent;
                break;
            case WardrobeItemType.Shirts:
                equipPoint = ShirtRootParent;
                break;
            case WardrobeItemType.Glasses:
                equipPoint = GlassesRootParent;
                break;
            default:
                equipPoint = ShirtRootParent;
                break;
        }

        ActivateWardrobeItem(item, equipPoint);
    }

    public void EquipDisplayitem(CustomizationGift item)
    {
        Transform equipPoint;

        switch (GetWardrobeItem(item).mType) {
            case WardrobeItemType.Hat:
                equipPoint = DecoyPenguinHatRootParent;
                break;
            case WardrobeItemType.Shirts:
                equipPoint = DecoyPenguinShirtRootParent;
                break;
            case WardrobeItemType.Glasses:
                equipPoint = DecoyPenguinGlassesRootParent;
                break;
            default:
                equipPoint = DecoyPenguinShirtRootParent;
                break;
        }

        ActivateWardrobeItem(item, equipPoint);
    }

    public void UnEquipItem(CustomizationGift item)
    {
        WardrobeItem wardItem = GetWardrobeItem(item);

        CustomisationItem equipped = null;
        foreach (CustomisationItem equippedItem in currentlyEquippedItems) {
            if (equippedItem.ItemType == wardItem.mType) {
                equipped = equippedItem;
            }
        }

        //remove equipped item and destroy it in scene
        if (equipped != null) {
            currentlyEquippedItems.Remove(equipped);
            Destroy(equipped.gameObject);
        }
    }

    public void ActivateWardrobeItem(CustomizationGift item, Transform penguinTransform)
    {
        //ward item holds all detail needed to spawn and position item
        WardrobeItem wardItem = GetWardrobeItem(item);

        //check if there is already an item with the same type equipped
        CustomisationItem equipped = null;
        foreach (CustomisationItem equippedItem in currentlyEquippedItems) {
            if(equippedItem.ItemType == wardItem.mType) {
                equipped = equippedItem;
            }
        }

        //remove equipped item and destroy it in scene
        if(equipped != null) {
            currentlyEquippedItems.Remove(equipped);
            Destroy(equipped.gameObject);
        }

        //cust item is the actual in scene customisation item
        GameObject custItem = Instantiate(wardItem.mPrefab, penguinTransform);
        CustomisationItem itemIdentifier = custItem.AddComponent<CustomisationItem>();
        itemIdentifier.Item = item;

        //add item to equipped list
        itemIdentifier.ItemType = wardItem.mType;
        currentlyEquippedItems.Add(itemIdentifier);

        //set items pos, rot and scale
        custItem.transform.localPosition = wardItem.mPos;
        custItem.transform.localRotation = Quaternion.Euler(wardItem.mRot);
        custItem.transform.localScale = wardItem.mScale;
    }

    public WardrobeItem GetWardrobeItem(CustomizationGift targetItem)
    {
        foreach (WardrobeItem item in AllWardrobeItems) {
            if(item.ID == (int)targetItem) {
                return (WardrobeItem)item.Clone();
            }
        }
        return null;
    }

    public void AddAccessoryToInventory(CustomizationGift accessory)
    {
        playersWardrobe.Add(GetWardrobeItem(accessory));
        FindObjectOfType<PickupPopUp>().AddPopUp(GetWardrobeItem(accessory));

        FindObjectOfType<WardrobeTutorial>().RecievedAccessoryItem(accessory);
    }

    private void Cheat_AddDansCustomisation()
    {
        gameObject.SetActive(true);

        EquipItem(CustomizationGift.IndieHat);
        EquipItem(CustomizationGift.Glasses);
        EquipItem(CustomizationGift.BlueBow);
    }

}

//contians all info for use in scene and other sorting info
[System.Serializable]
public class WardrobeItem : ICloneable{

    WardrobeItem() {}

    //clone item
    public object Clone()
    {
        WardrobeItem itemClone = (WardrobeItem) this.MemberwiseClone();
        return itemClone;
    }

    public int ID { get { return (int)mItem; } }

    [Header("ItemDetails")]
    public GameObject mPrefab;
    public CustomizationGift mItem;
    public WardrobeItemType mType;
    public int mCost;
    public bool availableInMarket = false;

    [TextArea(3,3)]public string ItemLocation;
    [TextArea(3,3)]public string ItemDescription;

    //where on the penguin the item should go
    [Header("Positioning")]
    public Vector3 mPos;
    public Vector3 mRot;
    public Vector3 mScale;

    [Header("UI")]
    public string TileName;
    public Sprite TileIcon;

    
}
