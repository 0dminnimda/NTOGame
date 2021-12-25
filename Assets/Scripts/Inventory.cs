using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string name;
    public GameObject prefub;
    public Sprite image;
    public int amount;
}

public class UIInventoryItem : MonoBehaviour
{
    
    
    void Awake()
    {
        
    }
    
    public void UpdateUI(InventoryItem item)
    {
        
    }
}

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private InventoryItem[] items;
    
    [SerializeField]
    private UIInventoryItem[] uiItem;
    
    
}
