using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using FarmHouseRedone.States;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Patching.Patches
{
    class Wallpaper_placementAction_Patch
    {
        public static bool Prefix(ref bool __result, GameLocation location, int x, int y, Farmer who, StardewValley.Objects.Wallpaper __instance)
        {
            if (who == null)
                who = Game1.player;
            if (StatesHandler.decoratableStates.ContainsKey(location))
            {
                Point point = new Point(x / 64, y / 64);
                Logger.Log("Used wallpaper at (" + point.X + ", " + point.Y + ")");
                DecoratableState state = StatesHandler.GetDecorState(who.currentLocation);
                if (__instance.isFloor)
                {
                    foreach (Maps.Room room in state.rooms.Values)
                    {
                        if (room.PointWithinFloor(point))
                        {
                            room.SetFloor(__instance.parentSheetIndex, location.map);
                            location.playSound("coin");
                            __result = true;
                            return false;
                        }
                    }
                }
                else
                {
                    foreach(Maps.Room room in state.rooms.Values)
                    {
                        if (room.PointWithinWall(point, location.map))
                        {
                            room.SetWall(__instance.parentSheetIndex, location.map);
                            location.playSound("coin");
                            __result = true;
                            return false;
                        }
                    }
                }
            }
            else
            {
                Logger.Log($"{location} has no DecoratableState!");
            }
            __result = false;
            return false;
        }
    }

    class Wallpaper_canBePlacedHere_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            return true;
        }
    }
}
