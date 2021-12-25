using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct InventoryItem
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


    [SerializeField]
    private TextMeshProUGUI[] texts;

    public RectTransform t;
    
    private void Start()
    {
        UpdateUI();
        Select(0);
    }

    void UpdateUI()
    {
        var len = Math.Min(texts.Length, items.Length);
        for (int i = 0; i < len; i++)
        {
            UpdateUIItem(i);
        }
    }

    void UpdateUIItem(int ind)
    {
        texts[ind].text = items[ind].amount.ToString();
    }

    public InventoryItem GetItem(int ind)
    {
        return items[ind];
    }
    
    public void SetItem(int ind, InventoryItem item)
    {
        items[ind] = item;
        UpdateUIItem(ind);
    
    }

    private static Dictionary<KeyCode, int> mapping = new Dictionary<KeyCode, int>()
    {
        {KeyCode.Alpha0, 0},
        {KeyCode.Alpha1, 1},
        {KeyCode.Alpha2, 2},
        {KeyCode.Alpha3, 3},
    };

    private void Update()
    {
        int ind;
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ind = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ind = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ind = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ind = 3;
        else
            return;
        
        Select(ind);
    }

    public int selected;
    
    void Select(int ind)
    {
        selected = ind;
        // t.position = Vector2.MoveTowards();
        // t.SetParent(texts[ind].transform);
        var rt = texts[ind].GetComponentInParent<RectTransform>();
        t.position = rt.position + new Vector3(50 + (-224.5f - (-233.3333f)), 50 + (0 - (-8.333332f)), 0);
        // -233.3333,  -224.5
        // -8.333332,  0
    }
}
