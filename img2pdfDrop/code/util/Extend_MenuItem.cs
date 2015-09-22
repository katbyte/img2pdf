//Copyright © 2015 kt@katbyte.me
using System.Windows.Forms;



namespace katbyte.winforms {

    /// <summary>
    /// Container class for MenuItem Extensions
    /// </summary>
    public static class Extend_MenuItem {


        /// <summary>
        /// inverts a menuitems check
        /// </summary>
        public static void CheckedMenuItemClicked (this MenuItem mi) {
            mi.Checked = ! mi.Checked;
        }

    }
}
