using JetBrains.Annotations;
using JYW.Game.EventPlay;
using UnityEngine;

public class InventoryCanvas : MonoBehaviour
{
    [SerializeField] private GameObject inventoryGrid;
    [SerializeField] private GameObject fuelItem;
    [SerializeField] private GameObject keyItem;
    [SerializeField] private GameObject remoteItem;

    
    private void Start()
    {
        EventPlayManager.Instance.AddAction<string>(gameObject, UpdateInventory);
    }

    private void UpdateInventory(string item)
    {
        if(item == "Fuel") Instantiate(fuelItem, inventoryGrid.transform);
        else if (item == "Key") Instantiate(keyItem, inventoryGrid.transform);
        else if (item == "Remote") Instantiate(remoteItem, inventoryGrid.transform);
    }
}
