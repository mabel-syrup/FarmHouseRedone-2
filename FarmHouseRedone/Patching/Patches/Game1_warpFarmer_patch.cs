using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;
using StardewModdingAPI;

namespace FarmHouseRedone.Patching.Patches
{
    /*class Game1_warpFarmer_Patch
    {
        internal static bool Prefix(LocationRequest locationRequest, ref int tileX, ref int tileY, int facingDirectionAfterWarp)
        {
            *//*if(Game1.currentLocation.NameOrUniqueName == "FarmHouse" && tileX == Game1.getFarm().mainFarmhouseEntry.Value.X && tileY == Game1.getFarm().mainFarmhouseEntry.Value.Y)
            {

            }*//*
            if(locationRequest.Location is FarmHouse)
            {
                Logger.Log("Warp location was farmhouse...");
                if (tileX == 3 && tileY == 11)
                {
                    Logger.Log("Warp location was default entrance...");
                    States.FarmHouseState state = States.StatesHandler.GetHouseState(locationRequest.Location as FarmHouse);
                    tileX = state.entry.X;
                    tileY = state.entry.Y;
                    Logger.Log($"Set warp location to ({tileX} {tileY})");


                    IReflectedMethod warpFarmer = Patcher.reflector.GetMethod(typeof(Game1), "performWarpFarmer");

                    warpFarmer.Invoke(locationRequest, tileX, tileY, facingDirectionAfterWarp);
                    return false;
                }
            }
            return true;
        }
    }*/
}
