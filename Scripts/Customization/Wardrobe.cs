using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Wardrobe : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager;
    private WardrobeInventory wardrobeInventory;
    public PauseMenu pauseMenuManager;
    public GameObject CanvasObj;

    [Header("UI References")]
    public GameObject WardrobeUI;
    public TextMeshProUGUI AccessoryLocationText;

    //raycasting to canvas
    GraphicRaycaster UIRaycaster;
    PointerEventData CursorEventData;
    EventSystem CanvasEventSystem;

    //picking up items, ui item obj
    private CustomisationItem currentUIItem;
    //holds all info for this item
    private WardrobeItem currentWardrobeItem;

    private Transform cachedItemParent;

    private List<WardrobeItem> EquippedItems;

    //flags
    private bool WardrobeEnabled = false;

    float prevSellTime = 0.0f;

    void Start()
    {
        UIRaycaster = CanvasObj.GetComponent<GraphicRaycaster>();
        CanvasEventSystem = CanvasObj.GetComponent<EventSystem>();
        wardrobeInventory = GetComponent<WardrobeInventory>();
        EquippedItems = new List<WardrobeItem>();
    }

    void Update()
    {
        if (WardrobeEnabled) {

            if(currentUIItem == null && GetItemDescriptionAtCursor(out AccessoryDescription itemInfo)) {
                AccessoryLocationText.transform.parent.gameObject.SetActive(true);
                AccessoryLocationText.text = (itemInfo.playerOwns)? itemInfo.description : itemInfo.location;
            }
            else {
                AccessoryLocationText.transform.parent.gameObject.SetActive(false);
                AccessoryLocationText.text = "not";
            }

            //if clicking on currently equipped item 
            if (Input.GetMouseButtonDown(0) && GetItemAtCursor(out CustomisationItem equippedItem)
                && CheckCustomisationZone(out CustomisationZone zone) ) {
                if(zone.CurrentItem == null ) { return; }

                UnEquipTempClothing(equippedItem.Item);

                currentUIItem = zone.CurrentItem;
                cachedItemParent = wardrobeInventory.UIContentTransform;
                currentUIItem.gameObject.GetComponent<RectTransform>().SetParent(WardrobeUI.transform);

                currentUIItem.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                currentUIItem.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

                zone.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;

                currentWardrobeItem = wardrobeInventory.GetWardrobeItem(equippedItem.Item);
            }

            //on click find the ui item obj from inventory
            else if (Input.GetMouseButtonDown(0) && GetItemAtCursor(out CustomisationItem item)) {
                currentUIItem = item;
                cachedItemParent = item.transform.parent;
                currentUIItem.gameObject.GetComponent<RectTransform>().SetParent(WardrobeUI.transform);
                currentUIItem.gameObject.GetComponent<Image>().color = new Color(1,1,1,0);
                currentUIItem.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

                currentWardrobeItem = wardrobeInventory.GetWardrobeItem(item.Item);
            }

            //if holding left click move the ui item obj to the cursor's position
            if (Input.GetMouseButton(0) && currentUIItem != null) {
                currentUIItem.gameObject.GetComponent<RectTransform>().position = Input.mousePosition;
            }

            //let go out mouse, will check if the item fits the penguin or just puts back into inv
            if (Input.GetMouseButtonUp(0) && Time.time - prevSellTime >= 0.2f && currentUIItem != null) {

                //check if over one of customisation zone, IE hats, shirts etc
                bool overCustomiseZone = CheckCustomisationZone(out CustomisationZone customisationZone);

                //if not over a customisezone or it doesn't fit put back into inv
                if (!overCustomiseZone || customisationZone.ZoneType != currentWardrobeItem.mType) {
                    //reset item back into inv
                    currentUIItem.gameObject.GetComponent<RectTransform>().SetParent(cachedItemParent);
                    currentUIItem.gameObject.GetComponent<RectTransform>().SetAsFirstSibling();
                    currentUIItem.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    currentUIItem.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;

                    //reset vars
                    currentUIItem = null;
                    currentWardrobeItem = null;
                    return;
                }

                //replace exsiting customisation item
                if(customisationZone.CurrentItem != null) {
                    CustomisationItem oldItem = customisationZone.CurrentItem;
                    oldItem.gameObject.GetComponent<RectTransform>().SetParent(cachedItemParent);
                    oldItem.gameObject.GetComponent<RectTransform>().SetAsFirstSibling();
                    currentUIItem.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    oldItem.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                }

                currentUIItem.gameObject.GetComponent<RectTransform>().SetParent(customisationZone.transform);
                currentUIItem.gameObject.GetComponent<RectTransform>().position = customisationZone.GetComponent<RectTransform>().position + new Vector3(0, 0, 0);
                
                currentUIItem.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
                customisationZone.gameObject.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

                customisationZone.CurrentItem = currentUIItem;

                EquipTempClothing(currentWardrobeItem.mItem);

                currentUIItem = null;
                currentWardrobeItem = null;
            }
        }       
    }

    //equips clothes to the wardrobe penguin and keeps list to give the player afterwards
    private void EquipTempClothing(CustomizationGift _item)
    {
        wardrobeInventory.EquipDisplayitem(_item);

        WardrobeItem wItem =  wardrobeInventory.GetWardrobeItem(_item);

        //add item to the currently equipped list, remove items of the same type
        EquippedItems.RemoveAll(x => x.mType == wItem.mType);
        EquippedItems.Add(wItem);
    }

    private void UnEquipTempClothing(CustomizationGift _item)
    {
        wardrobeInventory.UnEquipItem(_item);
        WardrobeItem wItem = wardrobeInventory.GetWardrobeItem(_item);
        EquippedItems.RemoveAll(x => x.mType == wItem.mType);
    }

    //returns the item at the cursor's position
    private bool GetItemAtCursor(out CustomisationItem item)
    {
        foreach (RaycastResult result in GetUIAtCursor()) {
            if (result.gameObject.TryGetComponent<CustomisationItem>(out item)) {
                return true;
            }
        }

        item = null;
        return false;
    }

    //returns the item description at the cursor's position
    private bool GetItemDescriptionAtCursor(out AccessoryDescription description)
    {
        foreach (RaycastResult result in GetUIAtCursor()) {
            if (result.gameObject.TryGetComponent<AccessoryDescription>(out description)) {
                return true;
            }
        }

        description = null;
        return false;
    }

    //checks if the cursor is above the sell zone
    private bool CheckCustomisationZone(out CustomisationZone customisationZone)
    {
        foreach (RaycastResult result in GetUIAtCursor()) {
            if (result.gameObject.TryGetComponent<CustomisationZone>(out customisationZone)) {
                return true;
            }
        }

        customisationZone = null;
        return false;
    }

    //returns all ui components at the cursors pos
    private List<RaycastResult> GetUIAtCursor()
    {
        CursorEventData = new PointerEventData(CanvasEventSystem);
        CursorEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        UIRaycaster.Raycast(CursorEventData, results);

        return results;
    }

    public void SetupWardrobe()
    {
        //equip items onto display penguin
        foreach (var equippedItem in EquippedItems) {
            wardrobeInventory.EquipDisplayitem(equippedItem.mItem);
        }

        WardrobeEnabled = true;

        WardrobeUI.SetActive(true);
        pauseMenuManager.playerIsInUI = true;
        pauseMenuManager.DiablePlayerHUD();
        wardrobeInventory.ListPlayersWardrobe(WardrobeItemType.All);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void StopWardrobe()
    {
        WardrobeEnabled = false;

        WardrobeUI.SetActive(false);
        pauseMenuManager.playerIsInUI = false;
        pauseMenuManager.EnablePlayerHUD();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //equip the temp item to the player
        foreach (var equippedItem in EquippedItems) {
            wardrobeInventory.EquipItem(equippedItem.mItem);
        }
    }

    public void CustomisationButtonClick(int cutomisationType)
    {
        Debug.Log((WardrobeItemType)cutomisationType);
        wardrobeInventory.ListPlayersWardrobe((WardrobeItemType)cutomisationType);

    }
}
