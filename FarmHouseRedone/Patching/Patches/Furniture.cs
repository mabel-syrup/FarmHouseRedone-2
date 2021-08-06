using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using FarmHouseRedone.Maps;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Patching.Patches
{
    class Furniture_GetAdditionalFurniturePlacementStatus_Patch
    {
        public static void Postfix(ref int __result, GameLocation location, int x, int y, Farmer who, StardewValley.Objects.Furniture __instance)
        {
            //Wall furniture was marked as valid, check against actual wall structure
            if(__result == 0 && !__instance.isGroundFurniture())
            {
                Point point = new Point(x / 64, y / 64);
                __result = 1;
                foreach (Room room in States.StatesHandler.GetDecorState(location).rooms.Values)
                {
                    if (room.PointWithinWall(point, location.map))
                    {
                        __result = 0;
                        return;
                    }
                    //Logger.Log($"{point} was not in {room.name}");
                }
            }
            if(__result == 3)
            {
                Point point = new Point(x / 64, y / 64);
                for (int x1 = point.X; x1 < point.X + __instance.getTilesWide(); x1++)
                {
                    for(int y1 = point.Y; y1 < point.Y + __instance.getTilesHigh(); y1++)
                    {
                        foreach (Room room in States.StatesHandler.GetDecorState(location).rooms.Values)
                        {
                            if (room.PointWithinWall(new Point(x1, y1), location.map) && (!(__instance is StardewValley.Objects.BedFurniture) || y1 != point.Y))
                                return;
                        }
                    }
                }
                __result = 0;
            }
        }
    }
}
