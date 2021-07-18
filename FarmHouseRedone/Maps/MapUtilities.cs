using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;
using xTile.ObjectModel;
using xTile.Tiles;
using xTile.Layers;
using StardewValley;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Maps
{
    public static class MapUtilities
    {
        public const int PASTE_DESTRUCTIVE = 0;
        public const int PASTE_NONDESTRUCTIVE = 1;
        public const int PASTE_PRESERVE_FLOORS = 2;
        public const int PASTE_REPLACE_FLOORS = 3;

        public static Vector2 GetMapSize(Map map)
        {
            return new Vector2(map.Layers[0].LayerWidth, map.Layers[0].LayerHeight);
        }

        public static void PasteTile(Map map, Map sectionMap, int x, int y, int mapX, int mapY, Dictionary<TileSheet, TileSheet> equivalentSheets, int pasteMode = PASTE_NONDESTRUCTIVE)
        {
            PasteTileInLayer(map, sectionMap, x, y, mapX, mapY, "Back", equivalentSheets, pasteMode);
            PasteTileInLayer(map, sectionMap, x, y, mapX, mapY, "Buildings", equivalentSheets, pasteMode);
            PasteTileInLayer(map, sectionMap, x, y, mapX, mapY, "Front", equivalentSheets, pasteMode);
            PasteTileInLayer(map, sectionMap, x, y, mapX, mapY, "AlwaysFront", equivalentSheets, pasteMode);
        }

        public static void PasteTileInLayer(Map map, Map sectionMap, int x, int y, int mapX, int mapY, string layer, Dictionary<TileSheet, TileSheet> equivalentSheets, int pasteMode)
        {
            if (sectionMap.GetLayer(layer) == null)
                return;
            if (mapX < 0 || mapY < 0 || map.GetLayer(layer).LayerWidth <= mapX || map.GetLayer(layer).LayerHeight <= mapY)
                return;
            if (sectionMap.GetLayer(layer).Tiles[x, y] != null)
            {
                Tile sectionTile = sectionMap.GetLayer(layer).Tiles[x, y];
                if (layer == "Back" && pasteMode == PASTE_PRESERVE_FLOORS && map.GetLayer(layer).Tiles[mapX, mapY] != null)
                {

                }
                else 
                { 
                    if (sectionTile is AnimatedTile)
                    {
                        int framesCount = (sectionTile as AnimatedTile).TileFrames.Length;
                        StaticTile[] frames = new StaticTile[framesCount];
                        for (int i = 0; i < framesCount; i++)
                        {
                            StaticTile frame = (sectionTile as AnimatedTile).TileFrames[i];
                            frames[i] = new StaticTile(map.GetLayer(layer), equivalentSheets[sectionTile.TileSheet], frame.BlendMode, frame.TileIndex);
                        }
                        map.GetLayer(layer).Tiles[mapX, mapY] = new AnimatedTile(map.GetLayer(layer), frames, (sectionTile as AnimatedTile).FrameInterval);
                    }
                    else
                    {
                        map.GetLayer(layer).Tiles[mapX, mapY] = new StaticTile(map.GetLayer(layer), equivalentSheets[sectionTile.TileSheet], sectionTile.BlendMode, sectionTile.TileIndex);
                    }
                }
                if (sectionTile != null && sectionTile.Properties.Keys.Count > 0 && map.GetLayer(layer).Tiles[mapX, mapY] != null)
                {
                    foreach (KeyValuePair<string, PropertyValue> pair in sectionTile.Properties)
                    {
                        map.GetLayer(layer).Tiles[mapX, mapY].Properties[pair.Key] = pair.Value;
                    }
                }
            }
            else if (pasteMode == PASTE_DESTRUCTIVE || (layer != "Back" && pasteMode == PASTE_PRESERVE_FLOORS) || (layer != "Back" && pasteMode == PASTE_REPLACE_FLOORS))
            {
                map.GetLayer(layer).Tiles[mapX, mapY] = null;
            }
        }

        public static void CloneTile(Tile t, Dictionary<TileSheet,TileSheet> equivalentSheets, Layer destLayer, int x, int y)
        {
            if (t is AnimatedTile)
            {
                int framesCount = (t as AnimatedTile).TileFrames.Length;
                StaticTile[] frames = new StaticTile[framesCount];
                for (int i = 0; i < framesCount; i++)
                {
                    StaticTile frame = (t as AnimatedTile).TileFrames[i];
                    frames[i] = new StaticTile(destLayer, equivalentSheets[t.TileSheet], frame.BlendMode, frame.TileIndex);
                }
                destLayer.Tiles[x, y] = new AnimatedTile(destLayer, frames, (t as AnimatedTile).FrameInterval);
            }
            else
            {
                destLayer.Tiles[x, y] = new StaticTile(destLayer, equivalentSheets[t.TileSheet], t.BlendMode, t.TileIndex);
            }
            if (t.Properties.Keys.Count > 0)
            {
                foreach (KeyValuePair<string, PropertyValue> pair in t.Properties)
                {
                    destLayer.Tiles[x, y].Properties[pair.Key] = pair.Value;
                }
            }
        }

        public static Map CloneMap(Map original)
        {
            Map map = new Map();
            Dictionary<TileSheet, TileSheet> clonedSheets = GetEquivalentSheets(map, original, true);
            foreach(Layer layer in original.Layers)
            {
                Layer newLayer = new Layer(layer.Id, map, layer.LayerSize, layer.TileSize);
                map.AddLayer(newLayer);
                for(int x = 0; x < newLayer.LayerWidth; x++)
                {
                    for(int y = 0; y < newLayer.LayerHeight; y++)
                    {
                        if (layer.Tiles[x, y] != null)
                            CloneTile(layer.Tiles[x, y], clonedSheets, newLayer, x, y);
                    }
                }
            }
            return map;
        }

        public static void OffsetProperties(Map map, int xOffset, int yOffset)
        {
            Dictionary<string, string> updatedProperties = new Dictionary<string, string>();
            foreach(KeyValuePair<string,PropertyValue> property in map.Properties)
            {
                string name = property.Key;
                if(name == "Walls" || name == "Floors")
                {
                    string adjusted = "";
                    string[] original = Strings.Cleanup(property.Value.ToString()).Split(' ');
                    for(int i = 0; i < original.Length; i += 5)
                    {
                        try
                        {
                            adjusted += $"{Convert.ToInt32(original[i]) + xOffset} {Convert.ToInt32(original[i + 1]) + yOffset} {original[i + 2]} {original[i + 3]} {original[i + 4]} ";
                        }
                        catch (IndexOutOfRangeException)
                        {
                            string wallStringAttempt = "";
                            while (i < original.Length)
                            {
                                wallStringAttempt += original[i] + " ";
                                i++;
                            }
                            Logger.Log(string.Format("Incomplete definition!  Walls and Floors must be defined as [x y width height room].  There were insufficient values for the room {0}", wallStringAttempt), StardewModdingAPI.LogLevel.Warn);
                        }
                        catch (FormatException)
                        {
                            string wallStringAttempt = "";
                            for (int j = 0; j < 5; j++)
                            {
                                wallStringAttempt += original[i + j] + " ";
                            }
                            Logger.Log(string.Format("Invalid definition!  Walls and Floors must be defined as [x y width height room].  The invalid wall definition was {0}\nThe invalid wall was skipped.", wallStringAttempt), StardewModdingAPI.LogLevel.Warn);
                        }
                    }
                    updatedProperties[name] = Strings.Cleanup(adjusted);
                }
                if(name == "Warp")
                {
                    string adjusted = "";
                    string[] original = Strings.Cleanup(property.Value.ToString()).Split(' ');
                    for(int i = 0; i < original.Length; i += 5)
                    {
                        try
                        {
                            adjusted += $"{Convert.ToInt32(original[i]) + xOffset} {Convert.ToInt32(original[i + 1]) + yOffset} {original[i + 2]} {original[i + 3]} {original[i + 4]} ";
                        }
                        catch (IndexOutOfRangeException)
                        {
                            string warpStringAttempt = "";
                            while (i < original.Length)
                            {
                                warpStringAttempt += original[i] + " ";
                                i++;
                            }
                            Logger.Log(string.Format("Incomplete definition!  Warps must be defined as [x y Destination x y].  There were insufficient values for the warp {0}", warpStringAttempt), StardewModdingAPI.LogLevel.Warn);
                        }
                        catch (FormatException)
                        {
                            string warpStringAttempt = "";
                            for (int j = 0; j < 5; j++)
                            {
                                warpStringAttempt += original[i + j] + " ";
                            }
                            Logger.Log(string.Format("Invalid definition!  Warps must be defined as [x y Destination x y].  There were invalid values for the warp {0}\nThis warp was skipped.", warpStringAttempt), StardewModdingAPI.LogLevel.Warn);
                        }
                    }
                    updatedProperties[name] = Strings.Cleanup(adjusted);
                }
            }
            foreach(string key in updatedProperties.Keys)
            {
                map.Properties[key] = updatedProperties[key];
            }
        }

        /// <summary>
        /// Safely and correctly adds a value to map properties on map.
        /// </summary>
        /// <param name="map">The map to append the value to.</param>
        /// <param name="propertyName">The property's name, whether map has it or not.</param>
        /// <param name="propertyValue">The value to append.</param>
        /// <param name="insertAt">Override the position to append the value at in the property.</param>
        /// <param name="replace">Completely clears the target property and keeps only the provided one.</param>
        /// <param name="autoSpace">Automatically provides a leading and/or trailing space as needed.</param>
        public static void MergeProperties(Map map, string propertyName, string propertyValue, int insertAt = -1, bool replace = false, bool autoSpace = true)
        {
            if (!map.Properties.ContainsKey(propertyName))
                map.Properties.Add(propertyName, "");
            string currentValue = map.Properties[propertyName];

            if(insertAt >= 0 && insertAt <= currentValue.Length)
            {
                if (replace)
                {
                    string subString = currentValue.Substring(0, insertAt); 
                    if (autoSpace && !subString.EndsWith(" "))
                        propertyValue = " " + propertyValue;
                    subString += propertyValue;
                    if (insertAt + propertyValue.Length < currentValue.Length)
                    {
                        if (autoSpace && !subString.EndsWith(" ") && currentValue[insertAt] != ' ')
                            subString += " ";
                        subString += currentValue.Substring(insertAt + propertyValue.Length);
                    }
                    map.Properties[propertyName] = subString;
                    return;
                }
                else
                {
                    string subString = currentValue.Substring(0, insertAt);
                    if (autoSpace && !subString.EndsWith(" "))
                        propertyValue = " " + propertyValue;
                    subString += propertyValue;
                    if (insertAt < currentValue.Length)
                    {
                        if (autoSpace && !subString.EndsWith(" ") && currentValue[insertAt] != ' ')
                            subString += " ";
                        subString += currentValue.Substring(insertAt);
                    }
                    map.Properties[propertyName] = subString;
                    return;
                }
            }
            else
            {
                if (replace)
                {
                    map.Properties[propertyName] = propertyValue;
                    return;
                }
                if (autoSpace && !map.Properties[propertyName].ToString().EndsWith(" "))
                    propertyValue = " " + propertyValue;
                map.Properties[propertyName] += propertyValue;
            }
        }

        public static Dictionary<TileSheet, TileSheet> GetEquivalentSheets(Map map, Map subMap, bool add = false)
        {
            Dictionary<TileSheet, TileSheet> equivalentSheets = new Dictionary<TileSheet, TileSheet>();
            foreach (TileSheet sheet in subMap.TileSheets)
            {
                foreach (TileSheet locSheet in map.TileSheets)
                {
                    if (CleanImageSource(locSheet.ImageSource).Equals(CleanImageSource(sheet.ImageSource)))
                    {
                        Logger.Log("Equivalent sheets: " + sheet.ImageSource + " and " + locSheet.ImageSource);
                        equivalentSheets[sheet] = locSheet;
                        break;
                    }
                    else
                    {
                        //Logger.Log(CleanImageSource(locSheet.ImageSource) + " was not the same as " + CleanImageSource(sheet.ImageSource));
                    }
                }
                if (equivalentSheets.ContainsKey(sheet))
                    continue;
                else
                {
                    if (add)
                    {
                        TileSheet newSheet = new TileSheet(map, sheet.ImageSource, sheet.SheetSize, sheet.TileSize);
                        
                        foreach(KeyValuePair<string,PropertyValue> property in sheet.Properties)
                        {
                            newSheet.Properties[property.Key] = property.Value.ToString();
                        }
                        for(int tileIndex = 0; tileIndex < sheet.TileCount; tileIndex++)
                        {
                            foreach(KeyValuePair<string,PropertyValue> property in sheet.TileIndexProperties[tileIndex])
                            {
                                newSheet.TileIndexProperties[tileIndex][property.Key] = property.Value.ToString();
                            }
                        }
                        
                        map.AddTileSheet(newSheet);
                        Logger.Log("Equivalent sheets: " + sheet.ImageSource + " and " + newSheet.ImageSource);
                        equivalentSheets[sheet] = newSheet;
                    }
                    else
                    {
                        Logger.Log("No equivalent sheet found for " + sheet.ImageSource + "!  Using default (" + map.TileSheets[0].ImageSource + ")", StardewModdingAPI.LogLevel.Warn);
                        equivalentSheets[sheet] = map.TileSheets[0];
                    }
                }
            }
            Logger.Log("Finished sheet equivalence calculation.");
            return equivalentSheets;
        }

        public static string CleanImageSource(string source)
        {
            string[] seasons = { "spring", "summer", "fall", "winter" };
            if (source.Contains("\\"))
            {
                string[] path = source.Split('\\');
                source = path.Last();
            }
            if (source.Contains("/"))
            {
                string[] path = source.Split('/');
                source = path.Last();
            }
            if (source.Contains("_") && seasons.Contains(source.Split('_')[0].ToLower()))
            {
                source = source.Remove(0, (source.Split('_')[0]).Length + 1);
            }
            //Logger.Log("Cleaned as " + source);
            return source;
        }

        public static int GetTileSheet(Map map, string sourceToMatch)
        {
            for (int sheetIndex = 0; sheetIndex < map.TileSheets.Count; sheetIndex++)
            {
                TileSheet sheet = map.TileSheets[sheetIndex];
                if (sheet.ImageSource.Contains(sourceToMatch))
                    return sheetIndex;
            }
            Logger.Log("Couldn't find any tilesheet that matches the source \"" + sourceToMatch + "\"!  Using index 0.", StardewModdingAPI.LogLevel.Warn);
            return 0;
        }

        public static bool IsTileOnSheet(Map map, Tile tile, int tileSheetToMatch)
        {
            return tile != null && tile.TileSheet.Equals((object)map.TileSheets[tileSheetToMatch]);
        }

        public static bool IsTileOnSheet(Map map, string layer, int x, int y, int tileSheetToMatch)
        {
            return map.GetLayer(layer).Tiles[x, y] != null && map.GetLayer(layer).Tiles[x, y].TileSheet.Equals((object)map.TileSheets[tileSheetToMatch]);
        }

        public static bool IsTileOnSheet(Map map, string layer, int x, int y, int tileSheetToMatch, Rectangle region)
        {
            return IsTileOnSheet(map, layer, x, y, tileSheetToMatch) && IsIndexInRect(map.GetLayer(layer).Tiles[x, y].TileIndex, map.TileSheets[tileSheetToMatch], region);
        }

        public static bool IsIndexInRect(int index, TileSheet sheet, Rectangle region)
        {
            int x = index % sheet.SheetSize.Width;
            int y = (index / sheet.SheetSize.Width);

            return region.Contains(x, y);
        }

        /// <summary>
        /// Selectively updates a tile only if it matches a specific tilesheet.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="index"></param>
        /// <param name="layer"></param>
        /// <param name="tileSheetToMatch"></param>
        /// <param name="region"></param>
        public static void SetMapTileIndexIfOnTileSheet(Map map, int x, int y, int index, string layer, int tileSheetToMatch, Rectangle region)
        {
            if (IsTileOnSheet(map, layer, x, y, tileSheetToMatch, region))
            {
                map.GetLayer(layer).Tiles[x, y].TileIndex = index;
            }
        }

        internal static void SetWallMapTileIndexForAnyLayer(Map map, int x, int y, int index)
        {
            SetMapTileIndexIfOnTileSheet(map, x, y, GetWallIndex(map, x, y, "Back", index), "Back", GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
            SetMapTileIndexIfOnTileSheet(map, x, y, GetWallIndex(map, x, y, "Buildings", index), "Buildings", GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
            SetMapTileIndexIfOnTileSheet(map, x, y, GetWallIndex(map, x, y, "Front", index), "Front", GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21));
        }

        public static int GetWallpaperIndex(Map map, int x, int y)
        {
            int wallIndex = GetWallSpriteIndex(map, x, y);
            if (wallIndex == -1)
                return -1;
            int wallPaperX = wallIndex % 16;
            int wallPaperY = wallIndex / 48;
            int wallPaperIndex = (wallPaperY * 16) + wallPaperX;
            Logger.Log("Found wallpaper index of " + wallPaperIndex + " for tilesheet index " + wallIndex + ".");
            return wallPaperIndex;
        }

        public static int GetWallSpriteIndex(Map map, int x, int y)
        {
            int index = -1;
            if (IsTileOnSheet(map, "Back", x, y, GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21)))
            {
                index = map.GetLayer("Back").Tiles[x, y].TileIndex;
            }
            else if (IsTileOnSheet(map, "Buildings", x, y, GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21)))
            {
                index = map.GetLayer("Buildings").Tiles[x, y].TileIndex;
            }
            else if (IsTileOnSheet(map, "Front", x, y, GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 0, 16, 21)))
            {
                index = map.GetLayer("Front").Tiles[x, y].TileIndex;
            }
            return index;
        }

        public static int GetFloorIndex(Map map, int x, int y)
        {
            int floorIndex = GetFloorSpriteIndex(map, x, y);
            if (floorIndex == -1)
                return -1;
            floorIndex -= 336;
            int floorX = (floorIndex / 2) % 8;
            int floorY = floorIndex / 32;
            int floor = (floorY * 8) + floorX;
            Logger.Log("Found floor index of " + floor + " for tilesheet index " + floorIndex + ".");
            return floor;
        }

        public static int GetFloorSpriteIndex(Map map, int x, int y)
        {
            int index = -1;
            if (IsTileOnSheet(map, "Back", x, y, GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 20)))
            {
                index = map.GetLayer("Back").Tiles[x, y].TileIndex;
            }
            return index;
        }

        internal static int GetWallIndex(Map map, int x, int y, string layer, int destinationIndex)
        {
            if (map.GetLayer(layer).Tiles[x, y] == null)
                return -1;
            int currentIndex = map.GetLayer(layer).Tiles[x, y].TileIndex;
            int whichHeight = (currentIndex % 48) / 16;
            return destinationIndex + (whichHeight * 16);
        }

        internal static int GetFloorIndex(Map map, int x, int y, string layer, int destinationIndex)
        {
            if (map.GetLayer(layer).Tiles[x, y] == null)
                return -1;
            int currentIndex = map.GetLayer(layer).Tiles[x, y].TileIndex;

            int horizontalOffset = currentIndex % 2;
            int verticalOffset = ((currentIndex - 336) / 16) % 2;

            return destinationIndex + horizontalOffset + (verticalOffset * 16);

            //TODO: this math is wrong - it's for walls
            //I'm very sleepy right now and can't do math
            //int whichHeight = (currentIndex % 48) / 16;
            //return destinationIndex + (whichHeight * 16);
        }

        internal static void SetFloorMapTileIndexForAnyLayer(Map map, int x, int y, int index)
        {
            SetMapTileIndexIfOnTileSheet(map, x, y, GetFloorIndex(map, x, y, "Back", index), "Back", GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            SetMapTileIndexIfOnTileSheet(map, x, y, GetFloorIndex(map, x, y, "Buildings", index), "Buildings", GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
            SetMapTileIndexIfOnTileSheet(map, x, y, GetFloorIndex(map, x, y, "Front", index), "Front", GetTileSheet(map, "walls_and_floors"), new Rectangle(0, 21, 16, 10));
        }

        internal static void ResizeMapAtLeast(Map map, int width, int height, int xOffset, int yOffset)
        {
            int mapWidth = map.Layers[0].LayerWidth;
            int mapHeight = map.Layers[0].LayerHeight;
            ResizeMap(map, Math.Max(mapWidth, width) + xOffset, Math.Max(mapHeight, height) + yOffset, xOffset, yOffset);
        }

        internal static void ResizeMap(Map map, int width, int height, int xOffset, int yOffset)
        {
            Dictionary<TileSheet, TileSheet> equivalentSheets = GetEquivalentSheets(map, map);
            Logger.Log("Resizing map from (" + GetMapSize(map).X + ", " + GetMapSize(map).Y + "), to (" + width + ", " + height + ") and shifting it by (" + xOffset + ", " + yOffset + ")");
            foreach (Layer layer in map.Layers)
            {
                Layer tempLayer = new Layer(layer.Id + "_temporary", map, new xTile.Dimensions.Size(width, height), layer.TileSize);
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        int sourceX = x - xOffset;
                        int sourceY = y - yOffset;
                        if(sourceX >= 0 && sourceY >= 0 && sourceX < layer.LayerWidth && sourceY < layer.LayerHeight && layer.Tiles[sourceX,sourceY] != null)
                        {
                            CloneTile(layer.Tiles[sourceX, sourceY], equivalentSheets, tempLayer, x, y);
                        }
                    }
                }
                layer.LayerSize = new xTile.Dimensions.Size(width, height);
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        if (tempLayer.Tiles[x, y] == null)
                            layer.Tiles[x, y] = null;
                        else
                            CloneTile(tempLayer.Tiles[x, y], equivalentSheets, layer, x, y);
                    }
                }
            }
        }
    }
}
