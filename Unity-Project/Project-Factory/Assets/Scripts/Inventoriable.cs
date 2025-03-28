﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface Inventoriable
{
    Sprite Image { get; }
    void OnPickUp();
    void OnDrop();
}

public class InventoryEventArgs : EventArgs
{
    public InventoryItem Item;

    public InventoryEventArgs(InventoryItem item)
    {
        Item = item;
    }

    public void InventoryEventsArgs(InventoryItem item)
    {
        Item = item;
    }
}