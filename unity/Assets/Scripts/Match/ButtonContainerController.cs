﻿using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ButtonContainerController : MonoBehaviour 
{
    public MenuItemController[] menuItems;

    private MenuItemController selectedMenuItem;

    public void EachMenuItem(UnityAction<MenuItemController> a)
    {
        menuItems.ToList().ForEach(m => a(m));
    }

    public void EachMenuItemSet(UnityAction<MenuItemController> a)
    {
        EachMenuItem(m => m.SetCallback(() => a(m)));
    }

    public void SetButtons(bool b)
    {
        EachMenuItem(m => m.SetActive(b));
    }

    public void SetSelected(MenuItemController menuItem)
    {
        if (selectedMenuItem != null) selectedMenuItem.Activate();
        selectedMenuItem = menuItem;
    }

    public void ClearSelected()
    {
        selectedMenuItem = null;
    }

    public void ClearSprites()
    {
        EachMenuItem(m => m.ClearSprite());
    }

    public MenuItemController Get(int index)
    {
        return menuItems[index];
    }

    public MenuItemController GetByName(string name)
    {
        return menuItems.ToList().Find(m => m.name.Equals(name));
    }
}
