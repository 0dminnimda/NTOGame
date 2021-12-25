using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Inventory inventory;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(inventory.GetItem(inventory.selected).prefub, transform.position, Quaternion.identity);
        }
    }
}
