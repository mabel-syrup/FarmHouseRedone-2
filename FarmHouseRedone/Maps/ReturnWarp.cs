using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmHouseRedone.Maps
{
    public class ReturnWarp
    {
        //The map this return warp handles returns from
        public string mapName;
        //The position the player will land when warped
        public Vector2 location;
        //The specific destination a warp must specify to use this return warp, such as (3, 11) for returns from the Farm.
        public Vector2 destination;

        public ReturnWarp(string mapName, Vector2 location, Vector2 destination)
        {
            this.mapName = mapName;
            this.location = location;
            this.destination = destination;
        }

        public ReturnWarp(string mapName, Vector2 location)
        {
            this.mapName = mapName;
            this.location = location;
            this.destination = new Vector2(-1, -1);
        }

        public bool IsGlobal()
        {
            return destination.X == -1 && destination.Y == -1;
        }

        public bool UseReturn()
        {
            Farmer player = Game1.player;
            return player.currentLocation != null && player.currentLocation.name != null && player.currentLocation.name.Equals(mapName) && (destination.X == -1 && destination.Y == -1) || (Game1.xLocationAfterWarp == destination.X && Game1.yLocationAfterWarp == destination.Y);
        }
    }
}
