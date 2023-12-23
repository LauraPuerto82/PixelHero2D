using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
    public static ItemsManager instance;
    
    private Dictionary <string, int> _items = new Dictionary<string, int>();

    public Dictionary<string, int> Items { get => _items; set => _items = value; }

    private Dictionary <string, int> _itemsToUnlockExtras = new Dictionary<string, int>();

    public Dictionary<string, int> ItemsToUnlockExtras { get => _itemsToUnlockExtras; set => _itemsToUnlockExtras = value; }

    private GameObject player;
    private PlayerExtrasTracker playerExtrasTracker;

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
}
