using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Patching.Patches
{
    class DecoratableLocation_doSetVisibleWallpaper_Patch
    {
        internal static bool Prefix(int whichRoom, int which, DecoratableLocation __instance)
        {
            return false;
        }
    }

    class DecoratableLocation_setWallpaper_Patch
    {
        internal static bool Prefix(int which, int whichRoom, bool persist, DecoratableLocation __instance)
        {
            return false;
        }
    }

    class DecoratableLocation_setWallpapers_Patch
    {
        public static bool Prefix(DecoratableLocation __instance)
        {
            return false;
        }
    }

    class DecoratableLocation_getWalls_Patch
    {
        public static void Postfix(ref List<Rectangle> __result, DecoratableLocation __instance)
        {

        }
    }

    class DecoratableLocation_getWallForRoom_Patch
    {
        public static bool Prefix(ref int __result, DecoratableLocation __instance)
        {
            __result = -1;
            return false;
        }
    }

    class DecoratableLocation_getFloorAt_Patch
    {
        public static bool Prefix(ref int __result, DecoratableLocation __instance)
        {
            __result = -1;
            return false;
        }
    }

    class DecoratableLocation_doSetVisibleFloor_Patch
    {
        internal static bool Prefix(int whichRoom, int which, DecoratableLocation __instance)
        {
            return false;
        }
    }

    class DecoratableLocation_setFloor_Patch
    {
        internal static bool Prefix(int which, int whichRoom, bool persist, DecoratableLocation __instance)
        {
            return false;
        }
    }

    class DecoratableLocation_setFloors_Patch
    {
        public static bool Prefix(DecoratableLocation __instance)
        {
            return false;
        }
    }

    class DecoratableLocation_getFloors_Patch
    {
        public static void Postfix(ref List<Rectangle> __result, DecoratableLocation __instance)
        {

        }
    }
}
