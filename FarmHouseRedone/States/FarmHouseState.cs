using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StardewValley;
using StardewValley.Locations;
using FarmHouseRedone.Maps;
using xTile;
using StardewModdingAPI;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.States
{
    //A Farmhouse state stores info for any farmhouse or cabin in the world.  These essentially act as an extension to the FarmHouse object, and store things FHR cares about.
    public class FarmHouseState
    {
        //These values help FHR understand things about the townInterior tilesheet.  They are static and shared across all FarmHouseStates.
        public static List<int> townInteriorWalls = new List<int>
        {
            33,
            34,
            35,
            38,
            39,
            40,
            44,
            45,
            46,
            50,
            51,
            52,
            53,
            54,
            55,
            65,
            66,
            67,
            70,
            71,
            72,
            76,
            77,
            78,
            79,
            80,
            81,
            82,
            83,
            84,
            85,
            86,
            87,
            97,
            98,
            99,
            102,
            103,
            104,
            105,
            106,
            107,
            108,
            109,
            110,
            111,
            112,
            113,
            114,
            115,
            116,
            117,
            118,
            119,
            127,
            129,
            131,
            134,
            136,
            140,
            142,
            146,
            148,
            154,
            155,
            169,
            170,
            171,
            172,
            173,
            174,
            201,
            202,
            203,
            204,
            205,
            206,
            218,
            219,
            233,
            234,
            235,
            236,
            237,
            238,
            254,
            265,
            266,
            267,
            268,
            269,
            270,
            286,
            297,
            298,
            299,
            300,
            301,
            302,
            303,
            304,
            305,
            318,
            329,
            330,
            361,
            591,
            592,
            593,
            1137,
            1138,
            1217,
            1218,
            1225,
            1226,
            1402,
            1403,
            1404,
            1405,
            1406,
            1407,
            1434,
            1435,
            1436,
            1437,
            1438,
            1439,
            1466,
            1467,
            1468,
            1469,
            1470,
            1471,
            1498,
            1501,
            1638,
            1639,
            1640,
            1641,
            1670,
            1671,
            1672,
            1673,
            1702,
            1703,
            1704,
            1705
        };
        public static List<int> townInteriorFloors = new List<int>
        {
            4,
            5,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            31,
            57,
            58,
            59,
            60,
            61,
            62,
            63,
            69,
            89,
            90,
            91,
            92,
            93,
            94,
            121,
            122,
            123,
            124,
            125,
            126,
            135,
            150,
            151,
            156,
            157,
            158,
            159,
            198,
            199,
            200,
            228,
            230,
            231,
            232,
            248,
            249,
            250,
            251,
            252,
            253,
            255,
            260,
            263,
            264,
            280,
            281,
            282,
            283,
            284,
            285,
            287,
            289,
            292,
            315,
            316,
            317,
            328,
            362,
            442,
            445,
            446,
            447,
            477,
            478,
            479,
            507,
            508,
            509,
            510,
            511,
            539,
            540,
            541,
            542,
            543,
            571,
            572,
            573,
            574,
            575,
            603,
            604,
            605,
            606,
            607,
            635,
            636,
            637,
            638,
            639,
            670,
            671,
            702,
            703,
            798,
            799,
            830,
            831,
            866,
            928,
            994,
            995,
            997,
            998,
            999,
            1000,
            1001,
            1002,
            1112,
            1113,
            1114,
            1144,
            1145,
            1146,
            1149,
            1150,
            1151,
            1169,
            1170,
            1171,
            1172,
            1175,
            1176,
            1177,
            1181,
            1182,
            1183,
            1207,
            1208,
            1209,
            1214,
            1215,
            1239,
            1240,
            1241,
            1246,
            1247,
            1271,
            1272,
            1273,
            1278,
            1279,
            1299,
            1300,
            1301,
            1302,
            1303,
            1304,
            1305,
            1308,
            1309,
            1310,
            1311,
            1331,
            1332,
            1333,
            1334,
            1336,
            1337,
            1342,
            1343,
            1499,
            1500,
            1502,
            1503,
            1508,
            1509,
            1510,
            1511,
            1512,
            1513,
            1530,
            1531,
            1532,
            1533,
            1534,
            1535,
            1540,
            1541,
            1542,
            1543,
            1544,
            1545,
            1560,
            1561,
            1562,
            1563,
            1564,
            1565,
            1566,
            1567,
            1572,
            1573,
            1574,
            1575,
            1576,
            1577,
            1592,
            1593,
            1594,
            1595,
            1596,
            1597,
            1598,
            1599,
            1604,
            1605,
            1606,
            1607,
            1608,
            1609,
            1625,
            1626,
            1627,
            1628,
            1629,
            1630,
            1631,
            1657,
            1658,
            1659,
            1660,
            1661,
            1662,
            1663,
            1664,
            1665,
            1666,
            1689,
            1690,
            1691,
            1692,
            1693,
            1694,
            1695,
            1696,
            1697,
            1698,
            1710,
            1714,
            1721,
            1722,
            1723,
            1724,
            1725,
            1726,
            1727,
            1753,
            1754,
            1755,
            1756,
            1757,
            1758,
            1759,
            1788,
            1789,
            1790,
            1791,
            1822,
            1845,
            1846,
            1847,
            1848,
            1849,
            1850,
            1851,
            1852,
            1853,
            1854,
            1887,
            1915,
            1916,
            1917,
            1945,
            1946,
            1947,
            1948,
            1949,
            2075,
            2076,
            2078,
            2079,
            2083,
            2084,
            2085,
            2106,
            2107,
            2108,
            2109,
            2110,
            2112,
            2113,
            2114,
            2116,
            2117,
            2118,
            2119,
            2122,
            2127,
            2128,
            2130,
            2131,
            2132,
            2133,
            2138,
            2139,
            2140,
            2144,
            2145,
            2146,
            2148,
            2149,
            2150,
            2151,
            2154,
            2155,
            2161,
            2162,
            2164,
            2166,
            2171,
            2172
        };
        public static List<int> townInteriorWallFurniture = new List<int>
        {
            177,
            180,
            207,
            212,
            213,
            214,
            239,
            240,
            243,
            244,
            246,
            271,
            272,
            275,
            276,
            279,
            306,
            308,
            338,
            341,
            342,
            343,
            344,
            371,
            372,
            373,
            375,
            407,
            411,
            532,
            533,
            564,
            565,
            674,
            826,
            992,
            1032,
            1033,
            1034,
            1065,
            1066,
            1097,
            1098,
            1344
        };
        public static List<int> townInteriorFloorFurniture = new List<int>
        {
            216, 217, 313, 314, 331, 334, 335, 336, 337, 345, 346, 347, 348, 349, 350, 368, 369, 377, 178, 379, 380, 381, 382, 383, 400, 401, 408, 409, 412, 413, 415, 431, 433, 434, 440, 441, 444, 472, 473, 474, 476, 482, 483, 486, 487, 491, 502, 503, 504, 505, 506, 518, 519, 523, 534, 535, 536, 537, 538, 544, 545, 546, 551, 552, 556, 557, 558, 577, 578, 583, 584, 588, 589, 590, 608, 609, 610, 611, 612, 613, 614, 623, 624, 625, 641, 642, 643, 644, 645, 646, 651, 655, 656, 657, 658, 659, 660, 663, 672, 673, 675, 679, 680, 687, 688, 689, 690, 694, 695, 691, 692, 704, 711, 712, 722, 723, 724, 726, 727, 736, 743, 768, 800, 813, 814, 815, 832, 833, 834, 860, 864, 875, 876, 886, 892, 897, 865, 1029, 1062, 1063, 1064, 1094, 1095, 1096, 1108, 1109, 1111, 1143, 1179, 1141, 1180, 1191, 1127, 1128, 1136, 1168, 1179, 1200, 1322, 1327, 1328, 1335, 1348, 1354, 1359, 1360, 1367, 1387, 1388, 1459, 1491, 1525, 1589, 1621, 1837, 1912, 1940, 1944, 1974, 1982, 1983, 1950, 1951, 2115, 2136, 2137, 2141, 2147, 2167, 2168, 2169, 2173
        };
        public static List<int> townInteriorWindows = new List<int>
        {
            225,
            256,
            257,
            288,
            405,
            406,
            469,
            501,
            1219,
            1220,
            1221,
            1222,
            1224,
            1252,
            1253,
            1254,
            1285
        };

        public string mapPath;
        public FarmHouse location;
        public string packID;

        public int daysUntilUpgrade;
        public string upgradeID;

        public Vector2 offset;
        public Point entry;

        public List<ReturnWarp> returnWarps;

        public List<string> appliedUpgrades;

        public string currentBase;

        public FarmHouseState(FarmHouse house)
        {
            offset = Vector2.Zero;
            appliedUpgrades = new List<string>();
            location = house;
            entry = location.getEntryLocation();
            SetUpWarps();
            packID = "";
            upgradeID = "";
            currentBase = "";
            daysUntilUpgrade = 0;
            Load();
            mapPath = GetBaseMapPath();
            Reset();
        }

        public void DayStarted()
        {
            Load();
            if (daysUntilUpgrade > 0)
                daysUntilUpgrade--;
            if(daysUntilUpgrade == 0 && upgradeID != "")
                AddUpgrade(upgradeID);
            mapPath = GetBaseMapPath();
            Reset();
        }

        public void DayEnding()
        {
            Save();
        }

        public void Reset()
        {
            Reposition(-offset);
            offset = Vector2.Zero;
            entry = location.GetMapPropertyPosition("Entry", location.getEntryLocation().X, location.getEntryLocation().Y);
            UpdateFromMapPath();
            SetUpWarps();
            StatesHandler.GetDecorState(location).Reset();
        }

        public void PurgeUpgradedGroups()
        {
            Dictionary<string, string> grouped = new Dictionary<string, string>();
            List<string> ungrouped = new List<string>();
            foreach(ContentPacks.UpgradeModel model in ContentPacks.PackHandler.GetPackData(packID).Upgrades)
            {
                if (appliedUpgrades.Contains(model.ID))
                {
                    if (model.GetGroup() != "")
                        grouped[model.GetGroup()] = model.ID;
                    else
                        ungrouped.Add(model.ID);
                }
            }
            List<string> purgedList = new List<string>();
            foreach(string ID in appliedUpgrades)
            {
                if (grouped.ContainsValue(ID) || ungrouped.Contains(ID))
                    purgedList.Add(ID);
            }
            Logger.Log("AppliedUpgrades before purge: [" + Strings.ListToString(appliedUpgrades) + "]");
            appliedUpgrades = purgedList;
            Logger.Log("AppliedUpgrades after purge: [" + Strings.ListToString(appliedUpgrades) + "]");
        }

        public void UpdateMap()
        {
            //IContentPack pack = ContentPacks.PackHandler.GetPack(packID);
            foreach (ContentPacks.UpgradeModel model in ContentPacks.PackHandler.GetPackData(packID).Upgrades)
            {
                if (appliedUpgrades.Contains(model.ID) && !model.IsBase())
                {
                    ApplyUpgrade(model);
                }
            }
            location.updateMap();
            entry = location.GetMapPropertyPosition("Entry", location.getEntryLocation().X, location.getEntryLocation().Y);
            SetUpWarps();
            StatesHandler.GetDecorState(location).Reset();
            UnvoidAll();
        }

        public void SetCurrentUpgrade(string ID, int days)
        {
            if (StatesHandler.config.immediately_apply_upgrades)
            {
                AddUpgrade(ID, true);
                return;
            }
            upgradeID = ID;
            daysUntilUpgrade = days;
        }

        public void AddUpgrade(string ID, bool applyImmediately = false)
        {
            RecursiveAdd(ID);
            if (applyImmediately)
            {
                mapPath = GetBaseMapPath();
                Reset();
            }
        }

        private void RecursiveAdd(string ID)
        {
            ContentPacks.UpgradeModel model = ContentPacks.PackHandler.GetPackData(packID).GetModel(ID);
            appliedUpgrades.Add(model.ID);
            foreach (string withID in model.GetWith())
            {
                if (appliedUpgrades.Contains(withID))
                    continue;
                RecursiveAdd(withID);
            }
        }

        public void ApplyUpgrade(ContentPacks.UpgradeModel model)
        {
            IContentPack pack = ContentPacks.PackHandler.GetPack(packID);
            Logger.Log("Pasting upgrade " + model.ID + " at " + model.Position);
            foreach(ContentPacks.SectionModel section in model.GetSections())
            {
                Map upgradeMap = pack.LoadAsset<Map>(section.Map);
                MapSection upgrade = new MapSection(model.ID, upgradeMap);
                Vector2 position = model.ConvertPosition(offset, location.map, section.Position);
                upgrade.Paste(this, (int)position.X, (int)position.Y);
            }
        }

        public bool HasUpgradeGroup(string groupName)
        {
            foreach(ContentPacks.UpgradeModel model in ContentPacks.PackHandler.GetPackData(packID).Upgrades)
            {
                if (appliedUpgrades.Contains(model.ID) && model.GetGroup().Equals(groupName))
                    return true;
            }
            return false;
        }

        public string GetBaseMapPath()
        {
            PurgeUpgradedGroups();
            try
            {
                if(packID != "" && packID != "FHRVanillaInternalOnly")
                {
                    ContentPacks.Pack pack = ContentPacks.PackHandler.GetPackData(packID);
                    foreach(string ID in appliedUpgrades)
                    {
                        ContentPacks.UpgradeModel model = ContentPacks.PackHandler.GetPackData(packID).GetModel(ID);
                        if (model.IsBase())
                        {
                            Logger.Log("Applying " + model.ID + " as base.");
                            currentBase = model.ID;
                            location.upgradeLevel = model.GetBase();
                            return model.GetMap();
                        }
                    }
                    foreach (ContentPacks.UpgradeModel model in ContentPacks.PackHandler.GetPackData(packID).Upgrades)
                    {
                        if (appliedUpgrades.Contains(model.ID) && model.IsBase())
                        {
                            if (model.GetBase() >= location.upgradeLevel)
                            {
                                Logger.Log("No base found among applied upgrades.  Applying " + model.ID + " as base.");
                                currentBase = model.ID;
                                location.upgradeLevel = model.GetBase();
                                return model.GetMap();
                            }
                        }
                    }
                    if(location.mapPath != null)
                        return location.mapPath;
                    return "Maps\\FarmHouse";
                }
                else
                {
                    if (location.mapPath != null)
                        return location.mapPath;
                    return "Maps\\FarmHouse";
                }
            }
            catch (KeyNotFoundException)
            {
                Logger.Log("No base map path found!");
                if(location.mapPath != null)
                    return location.mapPath;
                return "Maps\\FarmHouse";
            }
        }

        public void UpdateFromMapPath()
        {
            IContentPack pack = ContentPacks.PackHandler.GetPack(packID);
            if (pack != null && ContentPacks.PackHandler.GetPackData(packID).GetModel(currentBase) != null)
                location.map = pack.LoadAsset<Map>(mapPath);
            else
                location.map = Loader.loader.Load<Map>(mapPath, StardewModdingAPI.ContentSource.GameContent);
            location.updateSeasonalTileSheets();
            location.map.LoadTileSheets(StardewValley.Game1.mapDisplayDevice);
            location.updateMap();
            location.updateWarps();
            UpdateMap();
        }

        public void Offset(Vector2 by)
        {
            this.offset += by;
            MapUtilities.OffsetProperties(location.map, (int)by.X, (int)by.Y);
            location.updateWarps();
            Reposition(by);
        }

        public void Reposition(Vector2 by)
        {
            Logger.Log($"Repositioning contents by ({by.X}, {by.Y})");
            location.overlayObjects.Clear();
            location.shiftObjects((int)by.X, (int)by.Y);
            foreach(StardewValley.Character c in location.characters)
            {
                c.Position += by * 64f;
            }
            foreach(StardewValley.Farmer player in StardewValley.Game1.getAllFarmers())
            {
                if (player.currentLocation == location)
                    player.Position += by * 64f;
            }
        }

        public void UnvoidAll()
        {
            foreach(StardewValley.Objects.Furniture furniture in location.furniture)
            {
                bool canBePlaced = IsFurnitureSpotValid(furniture);
                if (!canBePlaced)
                    Unvoid(furniture);
            }
        }

        internal bool IsFurnitureSpotValid(StardewValley.Objects.Furniture furniture)
        {
            if (furniture.furniture_type == StardewValley.Objects.Furniture.rug)
            {
                Logger.Log(furniture.name + " was rug, looking for placement location...");
                for (int x = furniture.boundingBox.X; x < furniture.boundingBox.Right; x++)
                {
                    for (int y = furniture.boundingBox.Y; y < furniture.boundingBox.Bottom; y++)
                    {
                        if (!IsTilePassableForRugs(new xTile.Dimensions.Location(x, y), Game1.viewport))
                        {
                            Logger.Log("Tile (" + x + ", " + y + ") was not passable for rugs!");
                            return false;
                        }
                    }
                }
                Logger.Log("Rug can be placed here.");
                return true;
            }
            else
            {
                Logger.Log(furniture.name + " was not rug, was " + furniture.furniture_type);
            }
            Vector2 realLocation = furniture.tileLocation;
            furniture.tileLocation.Value = Vector2.Zero;
            ReclaculateFurniture(furniture);
            bool isValid = furniture.canBePlacedHere(location, realLocation);
            bool isPartiallyStuck = !IsCompletelyClear(furniture, realLocation);
            bool isVoid = IsTileVoid(realLocation);
            Logger.Log("Spot valid? " + isValid.ToString() + " Partially Stuck? " + isPartiallyStuck.ToString() + " Void? " + isVoid.ToString());
            furniture.tileLocation.Value = realLocation;
            ReclaculateFurniture(furniture);
            return isValid && !isPartiallyStuck && !isVoid;
        }

        internal void ReclaculateFurniture(StardewValley.Objects.Furniture furniture)
        {
            furniture.boundingBox.X = (int)furniture.tileLocation.Value.X * 64;
            furniture.boundingBox.Y = (int)furniture.tileLocation.Value.Y * 64;
            furniture.updateDrawPosition();
        }

        internal bool IsCompletelyClear(StardewValley.Objects.Furniture furniture, Vector2 point)
        {

            for (int x1 = (int)point.X; x1 < point.X + furniture.getTilesWide(); ++x1)
            {
                for (int y1 = (int)point.Y; y1 < point.Y + furniture.getTilesHigh(); ++y1)
                {
                    if (location.doesTileHaveProperty(x1, y1, "NoFurniture", "Back") != null)
                    {
                        return false;
                    }
                    if (location.getTileIndexAt(x1, y1, "Buildings") != -1)
                        return false;
                }
            }
            return true;
        }

        internal bool IsTileVoid(Vector2 tileLocation)
        {
            Logger.Log("Checking if " + tileLocation.ToString() + " is in the void...");
            //This spot is not even on the map, so it's definitely the void
            if (!location.isTileOnMap(tileLocation))
            {
                Logger.Log("Tile is off the map.");
                return true;
            }

            Map map = location.map;
            //There's a tile here
            if (map.GetLayer("Back").Tiles[(int)tileLocation.X, (int)tileLocation.Y] != null)
            {
                Logger.Log("Tile exists...");
                int tileIndex = map.GetLayer("Back").Tiles[(int)tileLocation.X, (int)tileLocation.Y].TileIndex;
                //The void tile is on index 0 on both sheets, so we can exit out as soon as we see it's not 0
                if (tileIndex != 0)
                {
                    Logger.Log("Tile of index " + tileIndex + " was not 0, so not void.");
                    return false;
                }
                //Get the image source for the tilesheet.  This allows people to name the sheets anything they want
                string sheetSource = map.GetLayer("Back").Tiles[(int)tileLocation.X, (int)tileLocation.Y].TileSheet.ImageSource;
                //The void tiles are found in townInterior and farmhouse_tiles
                Logger.Log("Tile was on townInterior? " + sheetSource.Contains("townInterior").ToString() + " Tile was on farmhouse_tiles? " + sheetSource.Contains("farmhouse_tiles").ToString());
                return (sheetSource.Contains("townInterior") || sheetSource.Contains("farmhouse_tiles"));
            }
            else
            {
                Logger.Log("Tile was null.");
                return true;
            }
        }

        internal bool IsTilePassableForRugs(xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport)
        {
            xTile.ObjectModel.PropertyValue propertyValue = (xTile.ObjectModel.PropertyValue)null;
            xTile.Tiles.Tile tile1 = location.map.GetLayer("Back").PickTile(tileLocation, viewport.Size);
            if (tile1 != null)
                tile1.TileIndexProperties.TryGetValue("Passable", out propertyValue);
            xTile.Tiles.Tile tile2 = location.map.GetLayer("Buildings").PickTile(tileLocation, viewport.Size);
            if (propertyValue == null && tile2 == null)
                return tile1 != null;
            return false;
        }

        public void Unvoid(StardewValley.Objects.Furniture furniture)
        {
            DecoratableState state = StatesHandler.GetDecorState(location);
            Room preferredRoom = null;
            Point tileLocation = new Point((int)furniture.TileLocation.X, (int)furniture.TileLocation.Y);
            foreach (Room room in state.rooms.Values)
            {
                if (room.PointWithinRoom(tileLocation))
                {
                    if (preferredRoom == null || room.GetLinearDistanceToCenter(tileLocation) <= preferredRoom.GetLinearDistanceToCenter(tileLocation))
                        preferredRoom = room;
                }
            }
            if (preferredRoom == null)
                Logger.Log($"{furniture.name} has no preferred room.");
            else
                Logger.Log($"{furniture.name} prefers to be placed in the \"{preferredRoom.name}\"");
        }

        public void SetUpWarps()
        {
            returnWarps = new List<ReturnWarp>();
            if (!location.map.Properties.ContainsKey("Return"))
            {
                Logger.Log("Base map does not contain a definition for Return");
                return;
            }
            string[] warpString = Strings.Cleanup(location.map.Properties["Return"].ToString()).Split(' ');

            try
            {
                for (int index = 0; index < warpString.Length;)
                {
                    string mapName = warpString[index];
                    int x = Convert.ToInt32(warpString[index + 1]);
                    int y = Convert.ToInt32(warpString[index + 2]);
                    if(index + 4 < warpString.Length && int.TryParse(warpString[index+3], out int destX) && int.TryParse(warpString[index+4], out int destY))
                    {
                        returnWarps.Add(new ReturnWarp(mapName, new Vector2(x, y), new Vector2(destX, destY)));
                        Logger.Log($"Added new return warp from {mapName} at ({x}, {y}) when the warp points to ({destX}, {destY})");
                        index += 5;
                    }
                    else
                    {
                        returnWarps.Add(new ReturnWarp(mapName, new Vector2(x, y)));
                        Logger.Log($"Added new return warp from {mapName} at ({x}, {y})");
                        index += 3;
                    }
                }
            }
            catch (FormatException)
            {
                Logger.Log("Couldn't parse Return property!  Given \"" + Strings.Cleanup(location.map.Properties["Return"]) + "\".  Make sure each Return Warp is in the format \"Name x y\" or \"Name x y destX destY\"", LogLevel.Warn);
            }
        }

        public Vector2 GetWarpDestination(StardewValley.GameLocation fromLocation)
        {
            if(fromLocation == StardewValley.Game1.getFarm() && location.map.Properties.ContainsKey("Entry"))
            {
                Logger.Log("Player entered farmhouse from farm.");
                if (StardewValley.Game1.xLocationAfterWarp == 3 && StardewValley.Game1.yLocationAfterWarp == 11)
                    return new Vector2(entry.X, entry.Y);
            }
            foreach(ReturnWarp warp in returnWarps)
            {
                //Search through specific warps first, that way maps can override a global return if needed.
                if (warp.IsGlobal())
                    continue;
                if (warp.UseReturn())
                    return warp.location;
            }
            foreach(ReturnWarp warp in returnWarps)
            {
                if (warp.UseReturn())
                    return warp.location;
            }
            return new Vector2(-1, -1);
        }

        public void Load()
        {
            IO.FarmHouseModel model = Loader.data.ReadSaveData<IO.FarmHouseModel>("FH-" + location.name + location.uniqueName ?? "");
            if (model == null)
            {
                Logger.Log("No saved data for " + location.name + location.uniqueName ?? "");
                Logger.Log("Prompting for pack choice...");
                if (Delegates.onMenuClosed.Contains(PromptForPack))
                    return;
                Delegates.onMenuClosed.Add(PromptForPack);
            }
            else
            {
                this.packID = model.PackID ?? "";
                appliedUpgrades = model.AppliedUpgrades ?? new List<string>();
                daysUntilUpgrade = model.DaysUntilUpgrade;
                upgradeID = model.UpgradeID ?? "";
                offset = model.Offset != null ? new Vector2(model.Offset[0],model.Offset[1]) : Vector2.Zero;
                //Reposition(offset);
                Logger.Log(Environment.StackTrace);
                Logger.Log("Loaded pack FHR Pack: " + packID);
            }
        }

        public void PromptForPack()
        {
            StardewValley.Game1.activeClickableMenu = new UI.HouseMenu(GetPackChoice);
        }

        public void RepositionForBase(ContentPacks.UpgradeModel model)
        {
            string[] positionValues = model.GetPosition().Split(' ');
            try
            {
                Vector2 position = Vector2.Zero;
                position.X = Convert.ToInt32(positionValues[0]) + offset.X;
                position.Y = Convert.ToInt32(positionValues[1]) + offset.Y;
                Reposition(position);
            }
            catch (FormatException)
            {
                Logger.Log($"Failed to parse base position!  Given {model.GetPosition()}", LogLevel.Warn);
            }
        }

        public void GetPackChoice(string chosen)
        {
            packID = chosen;
            Logger.Log("Chose " + packID);

            foreach (ContentPacks.UpgradeModel model in ContentPacks.PackHandler.GetPackData(packID).Upgrades)
            {
                if (model.GetBase() == 0)
                {
                    appliedUpgrades.Add(model.ID);
                    RepositionForBase(model);
                }
            }

            mapPath = GetBaseMapPath();
            UpdateFromMapPath();
            Save();
        }

        public void Save()
        {

            IO.FarmHouseModel model = new IO.FarmHouseModel
            {
                PackID = packID,
                AppliedUpgrades = appliedUpgrades,
                DaysUntilUpgrade = daysUntilUpgrade,
                UpgradeID = upgradeID,
                Offset = new int[] { (int)offset.X, (int)offset.Y }
            };
            Loader.data.WriteSaveData<IO.FarmHouseModel>("FH-" + location.name + location.uniqueName ?? "", model);
        }
    }
}
