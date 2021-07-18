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
        internal static bool Prefix(ref List<Microsoft.Xna.Framework.Rectangle> __result, FarmHouse __instance)
        {
            List<Rectangle> rects = new List<Rectangle>();
            States.DecoratableState state = States.StatesHandler.GetDecorState(__instance);
            foreach(Maps.Room room in state.rooms.Values)
            {
                foreach(Maps.Wall wall in room.walls)
                {
                    rects.Add(wall.region);
                }
            }
            __result = rects;
            return false;
        }
    }

    class FarmHouse_getFloors_Patch
    {
        internal static bool Prefix(ref List<Microsoft.Xna.Framework.Rectangle> __result, FarmHouse __instance)
        {
            return true;
        }
    }

    class FarmHouse_setMapForUpgradeLevel_Patch
    {
        internal static bool Prefix(int level, FarmHouse __instance)
        {
            return false;
        }
    }
}
