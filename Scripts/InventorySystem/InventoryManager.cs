using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using static Items;
//using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Items> ItemsList = new List<Items>();
    
    public Transform ItemContent;
    public GameObject InventoryItem;
    public TextMeshProUGUI InvCountUI;
    public GameObject ItemDecriptionUI;

    public TMP_InputField itemnameField;
    public TMP_InputField itemcountField;

 

    public InventoryItemController[] InventoryItems;


    //scripts can get notified when an item is picked up. saves checking every second
    public static UnityAction ItemPickedUp;

    public PickupPopUp PopUpManager;


    //maps id to fish type
    public static Dictionary<int, FishType> IdFishMap = new Dictionary<int, FishType>() {
        { 1, FishType.Salmon },{ 2, FishType.Cod },{ 3, FishType.Snapper } };

    private void Awake()
    {
        Instance = this;
        
    }
    public void Add(Items items, int count)
    {
        bool itemAlreadyInInventory = false;
        foreach(Items invItem in ItemsList)
        {
            if (invItem.id == items.id)
            {
                invItem.amount += count;
                //Debug.Log("amount" + invItem.amount)
                //Debug.Log("Weight" + ItemCount);
                itemAlreadyInInventory = true;
            }
        }
        if(!itemAlreadyInInventory)
        {
            ItemsList.Add(items);
            items.amount += count;
        }

        PopUpInfo popUpInfo = new PopUpInfo(items.id, count);

        PopUpManager.AddPopUp(popUpInfo);

        ItemPickedUp?.Invoke();
    }
   
    public void AddItems(int id, int count)
    {
        bool itemAlreadyInInventory = false;
        foreach (Items invItem in ItemsList) {
            if (invItem.id == id) {
                invItem.amount += count;
                itemAlreadyInInventory = true;
            }
        }
        if (!itemAlreadyInInventory) {

            List<Items> allItems = FindObjectOfType<Inventoryreset>().itemList;

            foreach (var aitem in allItems) {
                if (aitem.id == id) {
                    Items items = Instantiate(aitem);
                    ItemsList.Add(items);
                    items.amount += count;
                }

            }      
        }

        PopUpInfo popUpInfo = new PopUpInfo(id, count);

        PopUpManager.AddPopUp(popUpInfo);

        ItemPickedUp?.Invoke();
    }


    public void RemoveFish(FishType fishType, int count)
    {
        //find the item id for the fish 
        int itemId = IdFishMap.FirstOrDefault(x => x.Value == fishType).Key;
        RemoveItems(itemId, count);
    }

    public void RemoveItems(int itemId, int count)
    {
        //references to the items in inventory
        Items items = null;

        //find item in inventory
        foreach (Items invItem in ItemsList) {
            if (invItem.id == itemId) {
                items = invItem;
            }
        }

        //cant find item or doesn't have enough amount
        if(items == null || items.amount < count) {
            Debug.LogError("Invalid item removal call");
            return;
        }

        items.amount -= count;

        if (items.amount <= 0) {
            ItemsList.Remove(items);
        }
    }

    public void ListItems()
    {
        //clean content before open
       foreach(Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }
  

        //add items into inventory
        foreach (var items in ItemsList)
        {
            GameObject obj = Instantiate(InventoryItem,ItemContent);
            var ItemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var ItemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var ItemAmount = obj.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
            var ItemValue = obj.transform.Find("Value").GetComponent<TextMeshProUGUI>();


            ItemName.text = items.itemName;
            ItemIcon.sprite = items.icon;
            ItemAmount.text = items.amount.ToString();
            ItemValue.text = items.value.ToString();


        }
        
    }

   
}
