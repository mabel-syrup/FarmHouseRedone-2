using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Locations;

namespace FarmHouseRedone.States
{
    public static class StatesHandler
    {
        //Holds a state for each house.  Access a house's state via States.StatesHandler.houseStates[house]
        public static Dictionary<FarmHouse, FarmHouseState> houseStates;
        public static Dictionary<GameLocation, DecoratableState> decoratableStates;
        public static bool initialized = false;

        //This method is intended to be run only on startup.  FHR does not store data within the core of Stardew Valley, allowing it to be installed and uninstalled freely.
        //Each time it is run, it is essentially run for the first time.
        public static void Init()
        {
            initialized = true;
            houseStates = new Dictionary<FarmHouse, FarmHouseState>();
            decoratableStates = new Dictionary<GameLocation, DecoratableState>();
            foreach (GameLocation location in Game1.locations) {
                if (location is FarmHouse)
                    houseStates[location as FarmHouse] = new FarmHouseState(location as FarmHouse);
                if (location.map.Properties.ContainsKey("Walls") || location.map.Properties.ContainsKey("Floors"))
                    decoratableStates[location] = new DecoratableState(location);
            }
        }

        /// <summary>Returns the FarmHouseState of a given <c>FarmHouse</c>.  Will generate one if none exists.</summary>
        public static FarmHouseState GetHouseState(FarmHouse house)
        {
            if (!initialized)
                Init();
            //If a house got added to the game after the save is loaded, it won't have a state.  Here we quickly intercept that and give it a state once it is requested.
            if (!houseStates.ContainsKey(house))
            {
                Logger.Log("No house state found for " + house.name + "!  (" + house.uniqueName + ")");
                houseStates[house] = new FarmHouseState(house);
            }
            return houseStates[house];
        }

        public static DecoratableState GetDecorState(GameLocation location)
        {
            if (!initialized)
                Init();
            if (!decoratableStates.ContainsKey(location))
            {
                Logger.Log(string.Format("No decoratable state found for {0}! ({1})", location.Name ?? "unnamed location", location.uniqueName));
                decoratableStates[location] = new DecoratableState(location);
            }
            return decoratableStates[location];
        }
    }
}
