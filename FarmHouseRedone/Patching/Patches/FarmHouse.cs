using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmHouseRedone.Patching.Patches
{
    class FarmHouse_getWalls_Patch
    {
        internal static bool Prefix(ref List<Microsoft.Xna.Framework.Rectangle> __result, GameLocation __instance)
        {
            if (!(__instance is DecoratableLocation))
                return true;
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

    class FarmHouse_getEntryLocation_Patch
    {
        internal static bool Prefix(ref Point __result, FarmHouse __instance)
        {
            Logger.Log("Patched getEntryLocation()!");
            __result = new Point(0, 0);
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

    class FarmHouse_resetLocalState_Patch
    {
        internal static bool Prefix(FarmHouse __instance)
        {
            Logger.Log("Resetting local farmhouse state...");
            States.FarmHouseState state = States.StatesHandler.GetHouseState(__instance);
            state.Reset();

            Vector2 warpLocation = state.GetWarpDestination(Game1.player.currentLocation);

            if (warpLocation.X != -1 && warpLocation.Y != -1)
            {
                Game1.player.Position = new Vector2(warpLocation.X, warpLocation.Y) * 64f;
                Game1.xLocationAfterWarp = (int)warpLocation.X;
                Game1.yLocationAfterWarp = (int)warpLocation.Y;
                Logger.Log($"Warped player to ({Game1.player.Position.X}, {Game1.player.Position.Y}), because of Return Warp ({warpLocation.X}, {warpLocation.Y})");
                Game1.player.currentLocation = __instance;
            }
            return true;
        }

    }
}
