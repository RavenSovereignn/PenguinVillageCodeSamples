using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//used for other scripts to lookup items 

public class ItemSystem : MonoBehaviour
{
    public Inventoryreset invReset;

    [HideInInspector]
    public List<SystemItem> Items;

    public static Dictionary<int, FishType> IdToFish = new Dictionary<int, FishType>() {
        { 1, FishType.Salmon },{ 2, FishType.Cod },{ 3, FishType.Snapper } };

    public static Dictionary<FishType, int> FishToId;

    public Dictionary<V, K> Reverse<K, V>(IDictionary<K, V> dict)
    {
        return dict.ToDictionary(x => x.Value, x => x.Key);
    }

    private void Awake()
    {
        Items = new List<SystemItem>();

        //items as scriptable objects
        List<Items> itemsSO = invReset.itemList;

        foreach (var i in itemsSO) {
            SystemItem sysItem = new SystemItem();
            sysItem.id = i.id;
            sysItem.Name = i.itemName;
            sysItem.Icon = i.icon;
            Items.Add(sysItem);
        }

        FishToId = Reverse<int, FishType>(IdToFish);

    }


    public Sprite GetIcon(int id)
    {
        return Items.Find(x => x.id == id).Icon;
    }
    public string GetName(int id)
    {
        return Items.Find(x => x.id == id).Name;
    }


}

[System.Serializable]
public class SystemItem {
    public int id;
    public string Name;

    public Sprite Icon;
}

public enum SystemItemType { Fish, Collectible, Misc}