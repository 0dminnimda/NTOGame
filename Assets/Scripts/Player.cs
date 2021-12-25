using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;
    private InventoryItem item;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            item = inventory.GetItem(inventory.selected);
            if (item.amount > 0)
            {
                
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit))
                {
                    var o = Instantiate(item.prefub, hit.point + Vector3.up * 0.1f, Quaternion.identity);
                    item.amount -= 1;
                    inventory.SetItem(inventory.selected, item);
                }
                else
                {
                    throw new System.Exception(
                        "Nowhere to place main character : "); 
                }
                
            }
        }
    }
}
