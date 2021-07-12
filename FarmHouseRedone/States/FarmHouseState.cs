using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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

        public Vector2 offset;

        public List<string> appliedUpgrades;

        public FarmHouseState(FarmHouseState clone)
        {
            offset = clone.offset;
            location = new FarmHouse("Maps\\FarmHouse", "Dummy");
            location.upgradeLevel = clone.location.upgradeLevel;
            packID = clone.packID;
            mapPath = GetBaseMapPath();
            Reset();
        }

        public FarmHouseState(FarmHouse house)
        {
            offset = Vector2.Zero;
            location = house;
            packID = "";
            Load();
            mapPath = GetBaseMapPath();
            Reset();
        }

        public void Reset()
        {
            offset = Vector2.Zero;
            appliedUpgrades = new List<string>();
            UpdateFromMapPath();
        }

        public void UpdateMap()
        {
            IContentPack pack = ContentPacks.PackHandler.GetPack(packID);
            foreach (ContentPacks.UpgradeModel model in ContentPacks.PackHandler.GetPackData(packID).Upgrades)
            {
                if (appliedUpgrades.Contains(model.ID) && !model.IsBase())
                {
                    ApplyUpgrade(model);
                }
            }
            location.updateMap();
            //StatesHandler.GetDecorState(location).Reset();
        }

        public void ApplyUpgrade(ContentPacks.UpgradeModel model)
        {
            IContentPack pack = ContentPacks.PackHandler.GetPack(packID);
            Logger.Log("Pasting upgrade " + model.ID + " at " + model.Position);
            Map upgradeMap = pack.LoadAsset<Map>(model.GetMap());
            MapSection upgrade = new MapSection(model.ID, upgradeMap);
            Vector2 position = ConvertPosition(model);
            upgrade.Paste(this, (int)position.X, (int)position.Y);
        }

        private Vector2 ConvertPosition(ContentPacks.UpgradeModel model)
        {
            string[] positionValues = model.GetPosition().Split(' ');
            try
            {
                Vector2 mapBounds = MapUtilities.GetMapSize(location.map);
                Vector2 position = Vector2.Zero;
                if (positionValues[0].StartsWith("<"))
                    position.X = mapBounds.X - Convert.ToInt32(positionValues[0].Substring(1));
                else
                    position.X = Convert.ToInt32(positionValues[0]) + offset.X;

                if (positionValues[1].StartsWith("<"))
                    position.Y = mapBounds.Y - Convert.ToInt32(positionValues[1].Substring(1));
                else
                    position.Y = Convert.ToInt32(positionValues[1]) + offset.Y;
                return position;
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Log($"Couldn't parse the position value for upgrade \"{model.ID}\"!  " +
                    $"Positions must be given as\n\tx y\nwith a coordinate prefaced with \"<\" to denote it is measured from the left or bottom.", LogLevel.Warn);
                return new Vector2(-1000,-1000);
            }
        }

        public string GetBaseMapPath()
        {
            try
            {
                if(packID != "" && packID != "FHRVanillaInternalOnly")
                {
                    ContentPacks.Pack pack = ContentPacks.PackHandler.GetPackData(packID);
                    ContentPacks.UpgradeModel model = pack.GetBaseFor(location.upgradeLevel);
                    if(model != null)
                        return model.GetMap();
                    else if(location.mapPath != null)
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
            if (pack != null && ContentPacks.PackHandler.GetPackData(packID).GetBaseFor(location.upgradeLevel) != null)
                location.map = pack.LoadAsset<Map>(mapPath);
            else
                location.map = Loader.loader.Load<Map>(mapPath, StardewModdingAPI.ContentSource.GameContent);
            location.updateSeasonalTileSheets();
            location.map.LoadTileSheets(StardewValley.Game1.mapDisplayDevice);
            UpdateMap();
        }

        public void Offset(Vector2 by)
        {
            this.offset += by;
            MapUtilities.OffsetProperties(location.map, (int)by.X, (int)by.Y);
        }

        public void Load()
        {
            IO.FarmHouseModel model = Loader.data.ReadSaveData<IO.FarmHouseModel>("FH-" + location.name + location.uniqueName ?? "");
            if (model == null)
            {
                Logger.Log("No saved data for " + location.name + location.uniqueName ?? "");
                Logger.Log("Prompting for pack choice...");
                StardewValley.Game1.activeClickableMenu = new UI.HouseMenu(GetPackChoice);
            }
            else
            {
                this.packID = model.PackID;
                Logger.Log("Loaded pack FHR Pack: " + packID);
            }
        }

        public void GetPackChoice(string chosen)
        {
            packID = chosen;
            Logger.Log("Chose " + packID);
            mapPath = GetBaseMapPath();
            UpdateFromMapPath();
            Save();
        }

        public void Save()
        {

            IO.FarmHouseModel model = new IO.FarmHouseModel();
            model.PackID = packID;
            Loader.data.WriteSaveData<IO.FarmHouseModel>("FH-" + location.name + location.uniqueName ?? "", model);
        }
    }
}
