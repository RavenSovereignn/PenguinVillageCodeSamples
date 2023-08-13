using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class ZOLDMarket : MonoBehaviour
{
    enum ShopState { Buying,Selling, Browsing};

    [Header("References")] 
    public PlayerManager playerManager;
    public InventoryManager inventoryManager;
    public Inventoryreset inventoryreset;
    public PauseMenu pauseMenuManager;
    public GameObject CanvasObj;

    [Header("UI References")]
    public GameObject MarketUI;
    public GameObject SellingItemPanel;
    public TextMeshProUGUI ItemSellAmountText;
    public TMP_InputField ItemSellInputfield;
    public TextMeshProUGUI SellBuyButtonText;

    [Header("Seller")]
    public Transform SellersInventoryUI;
    public List<Items> SellersItems;

    //raycasting to canvas
    GraphicRaycaster UIRaycaster;
    PointerEventData CursorEventData;
    EventSystem CanvasEventSystem;

    //selling items
    private MarketItem currentItem;
    private Transform cachedItemParent;
    private int sellBuyAmount = 0;

    //flags
    private ShopState currentShopState = ShopState.Browsing;
    private bool marketEnabled = false;

    //needed so logic works
    private float prevSellTime = 0.0f;

    void Start()
    {
        UIRaycaster = CanvasObj.GetComponent<GraphicRaycaster>();
        CanvasEventSystem = CanvasObj.GetComponent<EventSystem>();

    }

    void Update()
    {
        if (marketEnabled) {
            //on click find the ui item obj 
            if (Input.GetMouseButtonDown(0) && currentShopState == ShopState.Browsing && GetItemAtCursor(out MarketItem item, ref currentShopState)) {
                currentItem = item;
                cachedItemParent = item.transform.parent;
                currentItem.gameObject.GetComponent<RectTransform>().SetParent(MarketUI.transform);
            }

            //if holding left click move the ui item obj to the cursor's position
            if (Input.GetMouseButton(0) && currentItem != null && currentShopState == ShopState.Browsing) {
                currentItem.gameObject.GetComponent<RectTransform>().position = Input.mousePosition;
            }

            //let go of mouse will move the item back into the inventory
            if (Input.GetMouseButtonUp(0) && Time.time - prevSellTime >= 0.2f && currentShopState == ShopState.Browsing) {
                bool overSellZone = CheckSellZone(out UISellZone sellZone);

                //check the item was over a sellzone, and check its the correct sellzone i.e player inv to market sellzone
                //reset the item position if either of those cases are false
                if (!overSellZone || (sellZone.MarketSellZone && currentItem.owner != MarketItem.ItemOwner.Player)
                    || (!sellZone.MarketSellZone && currentItem.owner != MarketItem.ItemOwner.MarketSeller)) {

                    currentItem.gameObject.GetComponent<RectTransform>().SetParent(cachedItemParent);
                    inventoryManager.ListItems();
                    currentItem = null;
                    return;
                }


                currentShopState = (sellZone.MarketSellZone) ? ShopState.Selling : ShopState.Buying;
                SellBuyButtonText.text = (sellZone.MarketSellZone) ? "Sell" : "Buy";

                SellingItemPanel.SetActive(true);
                ItemSellAmountText.text = "0";
                currentItem.gameObject.GetComponent<RectTransform>().position = SellingItemPanel.GetComponent<RectTransform>().position + new Vector3(0, 10, 0);
            }

        }      
    }

    //returns the item at the cursor's position
    private bool GetItemAtCursor(out MarketItem item, ref ShopState state)
    {
        foreach (RaycastResult result in GetUIAtCursor()) {
            if (result.gameObject.TryGetComponent<MarketItem>(out item)) {
                return true;
            }
        }

        item = null;
        return false;
    }

    //checks if the cursor is above the sell zone
    private bool CheckSellZone(out UISellZone sellZone)
    {
        foreach (RaycastResult result in GetUIAtCursor()) {
            if (result.gameObject.TryGetComponent<UISellZone>(out sellZone)) {
                return true;
            }
        }

        sellZone = null;
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

    private void SetupSeller()
    {
        SellersItems = new List<Items>();

        for (int i = 0; i < 5; i++) {
            Items marketItem = GetItemSO(Random.Range(1, 10));
            int itemAmount = Random.Range(1, 6);

            //see if random item is already in list
            Items itemInlist = SellersItems.Find(i => i.id == marketItem.id);

            //if already in the list add more amount otherwise add to the list
            if (itemInlist != null) {
                itemInlist.amount += itemAmount;
            }
            else {
                marketItem.amount = itemAmount;
                SellersItems.Add(marketItem);
            }
            
        }

        //inventoryManager.ListItems(SellersInventoryUI, SellersItems, true);
    }

    private void RemoveSellersItems(int _itemId, int _amount)
    {
        if(_amount <= 0) { return; }

        //if need to remove item from list
        int removeId = -1;
        foreach(Items sellerItem in SellersItems) {
            if(sellerItem.id == _itemId) {
                sellerItem.amount -= _amount;
                if(sellerItem.amount <= 0) {
                    removeId = SellersItems.IndexOf(sellerItem);
                }
            }
        }
        if(removeId != -1) {
            SellersItems.RemoveAt(removeId);
        }
    }

    public void SetupMarket()
    {
        marketEnabled = true;

        MarketUI.SetActive(true);
        pauseMenuManager.playerIsInUI = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        inventoryManager.ListItems();

        SetupSeller();
    }

    public void StopMarket()
    {
        marketEnabled = false;

        MarketUI.SetActive(false);
        pauseMenuManager.playerIsInUI = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //UI Functions
    public void IncreaseSellAmount()
    {
        Debug.Log(playerManager.PenguinCash / currentItem.itemAmount);
        if(currentShopState == ShopState.Buying) {
            sellBuyAmount = Mathf.Clamp(++sellBuyAmount, 0, Mathf.Min(currentItem.itemAmount, playerManager.PenguinCash / currentItem.itemCost) );
        }
        else {
            sellBuyAmount = Mathf.Clamp(++sellBuyAmount, 0, currentItem.itemAmount);
        }
        ItemSellAmountText.text = sellBuyAmount.ToString();
    }

    public void DecreaseSellAmount()
    {
        sellBuyAmount = Mathf.Clamp(--sellBuyAmount, 0, currentItem.itemAmount);
        ItemSellAmountText.text = sellBuyAmount.ToString();
    }

    //called on input field change value
    public void SetSellAmount()
    {
        //get amount and clamp
        int amount = int.Parse(ItemSellInputfield.text);
        amount = Mathf.Clamp(amount, 0, currentItem.itemAmount);
        ItemSellInputfield.text = amount.ToString();

        //set amount
        sellBuyAmount = amount;
        ItemSellAmountText.text = sellBuyAmount.ToString();
    }

    //sell or buy items
    public void SellBuyItems()
    {
        //buying
        if (currentShopState == ShopState.Buying) {
            //have enough money to buy
            if(sellBuyAmount > 0 && playerManager.PenguinCash >= currentItem.itemCost * sellBuyAmount) {
                //deduct cash and add items to inventory
                playerManager.PenguinCash -= currentItem.itemCost * sellBuyAmount;
                inventoryManager.Add(GetItemSO(currentItem.itemId), sellBuyAmount);
                inventoryManager.ListItems();
                RemoveSellersItems(currentItem.itemId, sellBuyAmount);
            }
            //reset sellers inventory
            currentItem.gameObject.GetComponent<RectTransform>().SetParent(cachedItemParent);
            //inventoryManager.ListItems(SellersInventoryUI, SellersItems, true);
            currentItem = null;
        }
        //sell the items
        else {
            //actually sell item and add cash to player
            playerManager.PenguinCash += sellBuyAmount * currentItem.itemCost;
            inventoryManager.RemoveItems(currentItem.itemId, sellBuyAmount);

            //reset inventory
            currentItem.gameObject.GetComponent<RectTransform>().SetParent(cachedItemParent);
            inventoryManager.ListItems();
            currentItem = null;
        }

        //reset ui
        currentShopState = ShopState.Browsing;
        SellingItemPanel.SetActive(false);
        ItemSellAmountText.text = "0";

        sellBuyAmount = 0;
        prevSellTime = Time.time;
    }

    private Items GetItemSO(int _id)
    {
        foreach(Items item in inventoryreset.itemList) {
            if(item.id == _id) {
                //creates another instance of scriptable object
                Items resultItem = Instantiate(item) as Items;
                return resultItem;
            }
        }
        return null;
    } 
}

