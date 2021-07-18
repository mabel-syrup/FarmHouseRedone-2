using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmHouseRedone
{
    public static class Delegates
    {
        public static List<OnMenusClosedBehavior> onMenuClosed = new List<OnMenusClosedBehavior>();

        public static void MenuChanged(StardewValley.Menus.IClickableMenu newMenu)
        {
            if (newMenu == null && onMenuClosed.Count > 0)
            {
                onMenuClosed[0].Invoke();
                onMenuClosed.RemoveAt(0);
            }
        }

        public delegate void OnMenusClosedBehavior();
    }
}
