//Copyright © 2015 kt@katbyte.me
using System;
using System.Windows.Forms;



namespace katbyte.winforms {

    /// <summary>
    /// Container class for MenuItem Extensions
    /// </summary>
    public static class Extend_MenuItemCollection {


        /// <summary>
        /// creates a new menu item, adds it to the collection, and returns it
        /// </summary>
        public static MenuItem New (this Menu.MenuItemCollection collection, string caption, EventHandler onClick) {
            var mi = new MenuItem(caption, onClick);
            collection.Add(mi);
            return mi;
        }

    }
}