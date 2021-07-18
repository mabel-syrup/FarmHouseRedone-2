using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using xTile.Tiles;
using xTile.ObjectModel;
using StardewValley;
using StardewValley.Locations;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Maps
{
    public class MapSection
    {
        public Map map;
        public string ID;
        public int anchorX;
        public int anchorY;

        public MapSection(string ID, Map map)
        {
            this.ID = ID;
            this.map = map;
            Init();
        }

        public MapSection(string ID, string mapPath)
        {
            Loader.loader.Load<Map>(mapPath, StardewModdingAPI.ContentSource.GameContent);
        }

        public void Init()
        {
            map.Properties.TryGetValue("Anchor", out PropertyValue anchor);
            if (anchor != null)
            {
                string cleanedAnchor = Strings.Cleanup(anchor.ToString());
                try
                {
                    string[] anchorCoords = cleanedAnchor.Split(' ');
                    anchorX = Convert.ToInt32(anchorCoords[0]);
                    anchorY = Convert.ToInt32(anchorCoords[1]);
                    return;
                }
                catch (Exception)
                {
                }
            }
            anchorX = 0;
            anchorY = 0;
        }

        public void Paste(ref Vector2 offset, Map destMap, int pasteX, int pasteY)
        {
            Dictionary<TileSheet, TileSheet> equivalentSheets = MapUtilities.GetEquivalentSheets(destMap, map, true);
            destMap.LoadTileSheets(Game1.mapDisplayDevice);
            //This gets us the paste bounds as relative to the existing map.  A negative x would mean to the left of 0.
            Rectangle pasteBounds = GetPasteBounds(pasteX, pasteY);

            //If the map section's bounds extend past the bounds in the left or up directions, we need to adjust the whole map.
            //The values are inverted such that this is now a value of how much to adjust by.
            Vector2 adjustment = new Vector2(-Math.Min(0, pasteBounds.X), -Math.Min(0, pasteBounds.Y));

            //Resize the map such that there is enough room to paste the map section.  
            MapUtilities.ResizeMapAtLeast(destMap, pasteBounds.Width, pasteBounds.Height, (int)adjustment.X, (int)adjustment.Y);

            //Get the actual sizes of the two maps.  The host map has been resized already.
            Vector2 mapSize = MapUtilities.GetMapSize(map);
            Vector2 hostMapSize = MapUtilities.GetMapSize(destMap);

            for (int x = 0; x < mapSize.X && pasteX + x + adjustment.X - anchorX < hostMapSize.X; x++)
            {
                int destX = pasteX + x + (int)adjustment.X - anchorX;
                for (int y = 0; y < mapSize.Y && pasteY + y + adjustment.Y - anchorY < hostMapSize.Y; y++)
                {
                    int destY = pasteY + y + (int)adjustment.Y - anchorY;
                    MapUtilities.PasteTile(destMap, map, x, y, destX, destY, equivalentSheets, 2);
                }
            }
            MapUtilities.OffsetProperties(map, pasteX - anchorX, pasteY - anchorY);
            foreach (KeyValuePair<string, PropertyValue> pair in map.Properties)
            {
                if (pair.Key == "Anchor")
                    continue;
                MapUtilities.MergeProperties(destMap, pair.Key, pair.Value.ToString());
            }
            offset += adjustment;
        }

        public void Paste(States.FarmHouseState state, int pasteX, int pasteY)
        {
            /*FarmHouse destination = state.location;
            Dictionary<TileSheet, TileSheet> equivalentSheets = MapUtilities.GetEquivalentSheets(destination, map, true);
            destination.map.LoadTileSheets(Game1.mapDisplayDevice);
            Rectangle pasteBounds = GetPasteBounds(pasteX, pasteY);
            Vector2 thisOffset = new Vector2(Math.Min(pasteBounds.X, 0), Math.Min(pasteBounds.Y, 0));
            MapUtilities.ResizeMapAtLeast(destination.map, pasteBounds.Width, pasteBounds.Height, (int)thisOffset.X, (int)thisOffset.Y);
            Vector2 totalOffset = state.offset - thisOffset;
            thisOffset = new Vector2(pasteBounds.X, pasteBounds.Y);
            Vector2 mapSize = MapUtilities.GetMapSize(map);
            Vector2 hostMapSize = MapUtilities.GetMapSize(destination.map);*/

            FarmHouse destination = state.location;
            Dictionary<TileSheet, TileSheet> equivalentSheets = MapUtilities.GetEquivalentSheets(destination.map, map, true);
            destination.map.LoadTileSheets(Game1.mapDisplayDevice);
            //This gets us the paste bounds as relative to the existing map.  A negative x would mean to the left of 0.
            Rectangle pasteBounds = GetPasteBounds(pasteX, pasteY);

            //If the map section's bounds extend past the bounds in the left or up directions, we need to adjust the whole map.
            //The values are inverted such that this is now a value of how much to adjust by.
            Vector2 adjustment = new Vector2(-Math.Min(0, pasteBounds.X), -Math.Min(0, pasteBounds.Y));

            //Resize the map such that there is enough room to paste the map section.  
            MapUtilities.ResizeMapAtLeast(destination.map, pasteBounds.Width, pasteBounds.Height, (int)adjustment.X, (int)adjustment.Y);

            //Get the actual sizes of the two maps.  The host map has been resized already.
            Vector2 mapSize = MapUtilities.GetMapSize(map);
            Vector2 hostMapSize = MapUtilities.GetMapSize(destination.map);

            for (int x = 0; x < mapSize.X && pasteX + x + adjustment.X - anchorX < hostMapSize.X; x++)
            {
                int destX = pasteX + x + (int)adjustment.X - anchorX;
                for (int y = 0; y < mapSize.Y && pasteY + y + adjustment.Y - anchorY < hostMapSize.Y; y++)
                {
                    int destY = pasteY + y + (int)adjustment.Y - anchorY;
                    MapUtilities.PasteTile(destination.map, map, x, y, destX, destY, equivalentSheets, 3);
                }
            }
            MapUtilities.OffsetProperties(map, pasteX - anchorX, pasteY - anchorY);
            foreach (KeyValuePair<string, PropertyValue> pair in map.Properties)
            {
                if (pair.Key == "Anchor")
                    continue;
                MapUtilities.MergeProperties(destination.map, pair.Key, pair.Value.ToString());
            }
            state.Offset(adjustment);
        }

        public Rectangle GetMapFootprint()
        {
            return new Rectangle(0, 0, map.Layers[0].LayerWidth, map.Layers[0].LayerHeight);
        }

        public Rectangle GetPasteBounds(int xPosition, int yPosition)
        {
            Rectangle footprint = GetMapFootprint();
            footprint.X = xPosition - anchorX;
            footprint.Y = yPosition - anchorY;
            return new Rectangle(footprint.X, footprint.Y, footprint.Right, footprint.Bottom);
        }


    }
}
