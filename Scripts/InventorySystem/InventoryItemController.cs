using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemController : MonoBehaviour
{
    private Items item;
    public GameObject ItemDescriptionUI;

    public void AddItem(Items newItem)
    {
        item = newItem;
    }
    public void ItemDescription()
    {
        ItemDescriptionUI.SetActive(true);
        Debug.Log("Button pressed");

        var ItemNameDescription = ItemDescriptionUI.transform.Find("ItemNameText").GetComponent<TextMeshProUGUI>();
        var ItemDescription = ItemDescriptionUI.transform.Find("ItemDescriptionText").GetComponent<TextMeshProUGUI>();
        var ItemHeal = ItemDescriptionUI.transform.Find("ItemHeal").GetComponent<TextMeshProUGUI>();
        var ItemValueDescription = ItemDescriptionUI.transform.Find("ItemValueText").GetComponent<TextMeshProUGUI>();
        var ItemIconDescription = ItemDescriptionUI.transform.Find("ItemIconDescription").GetComponent<Image>();

        ItemNameDescription.text = item.itemName;
        ItemDescription.text = item.Description.ToString();
        ItemHeal.text = item.ItemHeal.ToString();
        ItemValueDescription.text = item.value.ToString();
        ItemIconDescription.sprite = item.icon;
    }
}
