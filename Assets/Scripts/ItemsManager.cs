using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using static ItemController;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;
    
    private Dictionary <string, int> _items = new Dictionary<string, int>();

    public Dictionary<string, int> Items { get => _items; set => _items = value; }

    private Dictionary <string, int> _itemsToUnlockExtras = new Dictionary<string, int>();

    public Dictionary<string, int> ItemsToUnlockExtras { get => _itemsToUnlockExtras; set => _itemsToUnlockExtras = value; }

    private GameObject player;
    private PlayerExtrasTracker playerExtrasTracker;

    public class SaveDataItems
    {
        public int hearts;
        public int spinningCoins;
        public int shiningCoins;

        public SaveDataItems()
        {
        }

        public SaveDataItems(int hearts, int spinningCoins, int shiningCoins)
        {
            this.hearts = hearts;
            this.spinningCoins = spinningCoins;
            this.shiningCoins = shiningCoins;
        }
    }

    private void Awake()
    {
        instance = this;

        _items.Add("Heart", 0);        
        _items.Add("SpinningCoin", 0);
        _items.Add("ShiningCoin", 0);

        _itemsToUnlockExtras.Add("Heart", 5);        
        _itemsToUnlockExtras.Add("SpinningCoin", 6);
        _itemsToUnlockExtras.Add("ShiningCoin", 10);
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        playerExtrasTracker = player.GetComponent<PlayerExtrasTracker>();
    }

    public void SetTracker()
    {       
        if (_items["Heart"] >= _itemsToUnlockExtras["Heart"]) playerExtrasTracker.CanDoubleJump = true;
        if (_items["SpinningCoin"] >= _itemsToUnlockExtras["SpinningCoin"]) playerExtrasTracker.CanDash = true;
        if (_items["ShiningCoin"] >= _itemsToUnlockExtras["ShiningCoin"])
        {
            playerExtrasTracker.CanEnterBallMode = true;
            playerExtrasTracker.CanDropBombs = true;
        }
    }

    public JObject Serialize()
    {
        SaveDataItems saveDataItems = new SaveDataItems(_items[ItemType.Heart.ToString()], _items[ItemType.SpinningCoin.ToString()],
                                                        _items[ItemType.ShiningCoin.ToString()]);
        string jsonString = JsonUtility.ToJson(saveDataItems);
        JObject returnObject = JObject.Parse(jsonString);
        return returnObject;
    }

    public void DeSerialize(string jsonString)
    {
        SaveDataItems saveDataItems = JsonUtility.FromJson<SaveDataItems>(jsonString);
        _items[ItemType.Heart.ToString()] = saveDataItems.hearts;
        _items[ItemType.SpinningCoin.ToString()] = saveDataItems.spinningCoins;
        _items[ItemType.ShiningCoin.ToString()] = saveDataItems.shiningCoins;
        Start();
        SetTracker();
    }
}