using UnityEngine;

public class UIManager : MonoBehaviour
{
    private ItemsManager itemsManager;
    
    void Awake()
    {
        itemsManager = GameObject.FindObjectOfType<ItemsManager>();
    }

    void OnGUI()
    {        
        int doubleJumpItemsLeft = itemsManager.ItemsToUnlockExtras[ItemType.Heart.ToString()] - 
            itemsManager.Items[ItemType.Heart.ToString()];
        int dashItemsLeft = itemsManager.ItemsToUnlockExtras[ItemType.SpinningCoin.ToString()] - 
            itemsManager.Items[ItemType.SpinningCoin.ToString()];
        int ballModeItemsLeft = itemsManager.ItemsToUnlockExtras[ItemType.ShiningCoin.ToString()] - 
            itemsManager.Items[ItemType.ShiningCoin.ToString()];

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 24;

        GUI.Label(new Rect(10, 60, 350, 50), "Hearts left (Double Jump): " + doubleJumpItemsLeft.ToString(), style);
        GUI.Label(new Rect(10, 90, 350, 50), "Spinning coins left (Dash): " + dashItemsLeft.ToString(), style);
        GUI.Label(new Rect(10, 120, 350, 50), "Shining coins left (Ball mode): " + ballModeItemsLeft.ToString(), style);

        // Reset button               
        if (GUI.Button(new Rect(10, 10, 500, 30), "RESET GAME", style))
        {
            SaveDataGame.instance.ResetGame();
        }
    }    
}