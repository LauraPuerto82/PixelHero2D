using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.IO;
using UnityEngine;

public class SaveDataGame : MonoBehaviour
{

    public static SaveDataGame instance;

    private string _itemsKey = "items";       
    public string ItemsKey { get => _itemsKey; set => _itemsKey = value; }

    private string _itemsValue;
    public string ItemsValue { get => _itemsValue; set => _itemsValue = value; }

    // Data Persistence
    private ItemController[] itemControllers;
    private PlayerController playerController;
    private ItemsManager itemsManager;

    byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
    byte[] iVector = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

    void Awake()
    {
        instance = this;                

        _itemsValue = PlayerPrefs.GetString(_itemsKey);        
    }

    private void Start()
    {
        itemControllers = GameObject.FindObjectsOfType<ItemController>();        
        playerController = GameObject.FindObjectOfType<PlayerController>();
        itemsManager = GameObject.FindObjectOfType<ItemsManager>();

        if (PlayerPrefs.GetString(_itemsKey, _itemsValue) == "true")
        {
            StartGameWithItems();
        }
        else
        {            
            GameManager.instance.IsGameStarted = true;
        }
    }

    private void StartGameWithItems()
    {                
        ReadDataItemsPicked();
        ReadDataItemsNumber();
        ReadDataPlayer();        

        GameManager.instance.IsGameStarted = true;
    }  

    void ReadDataItemsPicked()
    {
        string filePath = Application.persistentDataPath + "/saveDataItemsPicked.sdg";
        
        byte[] decryptedSaveGame = File.ReadAllBytes(filePath);
        string jsonString = Decrypt(decryptedSaveGame);

        JObject jDataSaved = JObject.Parse(jsonString);
        for (int i = 0; i < itemControllers.Length; i++)
        {
            ItemController itemControllerTemp = itemControllers[i];
            string itemControllerJsonString = jDataSaved[itemControllerTemp.name].ToString();
            itemControllerTemp.DeSerialize(itemControllerJsonString);              
        }        
    }

    void ReadDataItemsNumber()
    {
        string filePath = Application.persistentDataPath + "/saveDataItemsNumber.sdg";
        
        byte[] decryptedSaveGame = File.ReadAllBytes(filePath);
        string jsonString = Decrypt(decryptedSaveGame);

        JObject jDataSaved = JObject.Parse(jsonString);

        string itemManagerJsonString = jDataSaved[itemsManager.name].ToString();
        itemsManager.DeSerialize(itemManagerJsonString);        
    }

    void ReadDataPlayer()
    {
        string filePath = Application.persistentDataPath + "/saveDataPlayer.sdg";
        
        byte[] decryptedSaveGame = File.ReadAllBytes(filePath);
        string jsonString = Decrypt(decryptedSaveGame);

        JObject jDataSaved = JObject.Parse(jsonString);
        
        string playerControllerJsonString = jDataSaved[playerController.name].ToString();
        playerController.DeSerialize(playerControllerJsonString);        
    }
    
    public void ResetGame()
    {
        _itemsValue = "false";        
        PlayerPrefs.SetString(_itemsKey, _itemsValue);
        GameManager.instance.LoadScene(0);        
        GameManager.instance.IsGameStarted = true;
        GameManager.instance.StartGame();
    }

    public void SaveData()
    {
        SaveDataItemsPicked();
        SaveDataItemsNumber();
        SaveDataPlayer();
    }

    void SaveDataItemsPicked()
    {
        JObject jDataSave = new JObject();

        for (int i = 0; i < itemControllers.Length; i++)
        {
            ItemController itemControllerTemp = itemControllers[i];
            bool itemPicked = itemControllerTemp.ItemPicked;
            JObject serializedItemController = itemControllerTemp.Serialize();
            jDataSave.Add(itemControllerTemp.name, serializedItemController);
        }

        string filePath = Application.persistentDataPath + "/saveDataItemsPicked.sdg";
        byte[] encryptedSaveGame = Encrypt(jDataSave.ToString());
        File.WriteAllBytes(filePath, encryptedSaveGame);        
    }

    void SaveDataItemsNumber()
    {
        JObject jDataSave = new JObject();

        ItemsManager itemsManagerTemp = GameObject.FindObjectOfType<ItemsManager>();
        JObject serializedItemManager = itemsManagerTemp.Serialize();
        jDataSave.Add(itemsManagerTemp.name, serializedItemManager);

        string filePath = Application.persistentDataPath + "/saveDataItemsNumber.sdg";        
        byte[] encryptedSaveGame = Encrypt(jDataSave.ToString());
        File.WriteAllBytes(filePath, encryptedSaveGame);
    }

    void SaveDataPlayer()
    {
        JObject jDataSave = new JObject();

        PlayerController playerControllerTemp = GameObject.FindObjectOfType<PlayerController>();
        JObject serializedPlayerController = playerControllerTemp.Serialize();
        jDataSave.Add(playerControllerTemp.name, serializedPlayerController);

        string filePath = Application.persistentDataPath + "/saveDataPlayer.sdg";        
        byte[] encryptedSaveGame = Encrypt(jDataSave.ToString());
        File.WriteAllBytes(filePath, encryptedSaveGame);
    }

    byte[] Encrypt(string plainText)
    {
        AesManaged aesManaged = new AesManaged();
        ICryptoTransform cryptoTransform = aesManaged.CreateEncryptor(key, iVector);
        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
        StreamWriter streamWriter = new StreamWriter(cryptoStream);
        streamWriter.Write(plainText);

        streamWriter.Close();
        cryptoStream.Close();
        memoryStream.Close();

        return memoryStream.ToArray();
    }

    string Decrypt(byte[] encryptedText)
    {
        AesManaged aesManaged = new AesManaged();
        ICryptoTransform decryptTransform = aesManaged.CreateDecryptor(key, iVector);
        MemoryStream memoryStream = new MemoryStream(encryptedText);
        CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptTransform, CryptoStreamMode.Read);
        StreamReader streamReader = new StreamReader(cryptoStream);

        string decryptedPlainText = streamReader.ReadToEnd();
        streamReader.Close();
        cryptoStream.Close();
        memoryStream.Close();

        return decryptedPlainText;
    }
}