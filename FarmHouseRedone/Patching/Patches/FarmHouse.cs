using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Patching.Patches
{
    class FarmHouse_getWalls_Patch
    {
        internal static void Postfix(ref List<Microsoft.Xna.Framework.Rectangle> __result, FarmHouse __instance)
        {
            
        }
    }

    class FarmHouse_getFloors_Patch
    {
        internal static void Postfix(ref List<Microsoft.Xna.Framework.Rectangle> __result, FarmHouse __instance)
        {

        }
    }

    class FarmHouse_setMapForUpgradeLevel_Patch
    {
        internal static bool Prefix(int level, FarmHouse __instance)
        {
            return true;
        }
    }
}
