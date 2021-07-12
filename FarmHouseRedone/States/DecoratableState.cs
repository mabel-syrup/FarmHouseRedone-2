using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using xTile;
using xTile.ObjectModel;
using Newtonsoft.Json.Linq;
using FarmHouseRedone.Maps;

namespace FarmHouseRedone.States
{
    public class DecoratableState
    {

        internal Dictionary<string, Room> rooms;
        internal readonly GameLocation location;

        public DecoratableState(GameLocation location)
        {
            this.location = location;
            Logger.Log("Created DecoratableState for " + location.Name);
            rooms = new Dictionary<string, Room>();
            Reset();
        }

        public void Reset()
        {
            rooms.Clear();
            if (location is FarmHouse && !location.map.Properties.ContainsKey("Walls") && !location.map.Properties.ContainsKey("Floors"))
            {
                LoadVanillaProperties();
            }
            List<Room> mapRooms = GetRoomsFromMap(location.map);
            foreach(Room room in mapRooms)
            {
                rooms[room.name] = room;
            }
            Load();
        }

        public static List<Room> GetRoomsFromMap(Map map)
        {
            List<Room> mapRooms = new List<Room>();
            map.Properties.TryGetValue("Walls", out PropertyValue walls);
            if(walls != null)
            {
                foreach(string[] set in ReadPropertiesIntoRoomData(walls))
                {
                    string name = set[4];
                    Room newRoom = null;
                    foreach(Room room in mapRooms)
                    {
                        if (room.name == name)
                            newRoom = room;
                    }
                    if (newRoom == null)
                        newRoom = new Room(name);
                    newRoom.walls.Add(new Wall(new Rectangle(
                        Convert.ToInt32(set[0]),
                        Convert.ToInt32(set[1]),
                        Convert.ToInt32(set[2]),
                        Convert.ToInt32(set[3]))
                    ));
                    if(!mapRooms.Contains(newRoom))
                        mapRooms.Add(newRoom);
                }
            }
            map.Properties.TryGetValue("Floors", out PropertyValue floors);
            if (floors != null)
            {
                foreach (string[] set in ReadPropertiesIntoRoomData(floors))
                {
                    string name = set[4];
                    Room newRoom = null;
                    foreach (Room room in mapRooms)
                    {
                        if (room.name == name)
                            newRoom = room;
                    }
                    if (newRoom == null)
                        newRoom = new Room(name);
                    newRoom.floors.Add(new Floor(new Rectangle(
                        Convert.ToInt32(set[0]),
                        Convert.ToInt32(set[1]),
                        Convert.ToInt32(set[2]),
                        Convert.ToInt32(set[3]))
                    ));
                    if (!mapRooms.Contains(newRoom))
                        mapRooms.Add(newRoom);
                }
            }
            Logger.Log("Found and built " + mapRooms.Count + " rooms:");
            foreach(Room room in mapRooms)
            {
                Logger.Log(room.ToString());
            }
            return mapRooms;
        }

        internal static List<string[]> ReadPropertiesIntoRoomData(string propertyString)
        {
            List<string[]> outRooms = new List<string[]>();
            propertyString = Strings.Cleanup(propertyString);
            if (propertyString == "" || propertyString == " ")
                return outRooms;
            string[] property = propertyString.Split(' ');
            for (int i = 0; i < property.Length; i += 5)
            {
                try
                {
                    //Test fit into a rectangle so we can catch format errors here.  Incorrect and incomplete definitions will be skipped and warned about to the developer.
                    Rectangle newRect = new Rectangle(Convert.ToInt32(property[i]), Convert.ToInt32(property[i + 1]), Convert.ToInt32(property[i + 2]), Convert.ToInt32(property[i + 3]));
                    string roomName = property[i + 4];
                    outRooms.Add(new string[] { property[i], property[i+1], property[i+2], property[i+3], property[i+4] });
                }
                catch (IndexOutOfRangeException)
                {
                    string wallStringAttempt = "";
                    while (i < property.Length)
                    {
                        wallStringAttempt += property[i] + " ";
                        i++;
                    }
                    Logger.Log(string.Format("Incomplete definition!  Walls and Floors must be defined as [x y width height room].  There were insufficient values for the room {0}", wallStringAttempt), StardewModdingAPI.LogLevel.Warn);
                }
                catch (FormatException)
                {
                    string wallStringAttempt = "";
                    for (int j = 0; j < 5; j++)
                    {
                        wallStringAttempt += property[i + j] + " ";
                    }
                    Logger.Log(string.Format("Invalid definition!  Walls and Floors must be defined as [x y width height room].  The invalid wall definition was {0}\nThe invalid wall was skipped.", wallStringAttempt), StardewModdingAPI.LogLevel.Warn);
                }
            }
            return outRooms;
        }

        private void LoadVanillaProperties()
        {
            Logger.Log("Loading provided vanilla walls and floors...");
            try
            {
                FarmHouse house = location as FarmHouse;
                JObject defaults = Loader.loader.Load<JObject>("assets/defaults/vanilla map walls and floors.json", StardewModdingAPI.ContentSource.ModFolder);
                if (defaults == null)
                    Logger.Log("Error reading the file!");
                JObject levels = defaults["upgrade levels"] as JObject;
                JObject level = levels[house.upgradeLevel.ToString()] as JObject;
                if(level.ContainsKey("Walls"))
                    Maps.MapUtilities.MergeProperties(house.map, "Walls", level["Walls"].ToString(), insertAt: 0, autoSpace: true);
                if(level.ContainsKey("Floors"))
                    Maps.MapUtilities.MergeProperties(house.map, "Floors", level["Floors"].ToString(), insertAt: 0, autoSpace: true);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message + "\n" + e.StackTrace, StardewModdingAPI.LogLevel.Error);
            }
        }

        public void Load()
        {
            IO.DecoratableModel model = Loader.data.ReadSaveData<IO.DecoratableModel>(location.name + location.uniqueName ?? "");
            if(model == null)
                Logger.Log("No saved data for " + location.name + location.uniqueName ?? "");
            else
            {
                foreach(string roomName in model.rooms.Keys)
                {
                    try
                    {
                        Logger.Log("Loading save data for room \"" + roomName + "\"...");
                        if (rooms.ContainsKey(roomName))
                        {
                            rooms[roomName].SetWall(model.rooms[roomName][0], location.map);
                            rooms[roomName].SetFloor(model.rooms[roomName][1], location.map);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Log("There was a problem loading the save data!\n" + e.Message + "\n" + e.StackTrace);
                    }
                }
                
            }
        }

        public void Save()
        {

            IO.DecoratableModel model = new IO.DecoratableModel();
            foreach(string room in rooms.Keys)
            {
                model.rooms.Add(room, new int[2] { rooms[room].wallIndex, rooms[room].floorIndex });
            }
            Loader.data.WriteSaveData<IO.DecoratableModel>(location.name + location.uniqueName ?? "", model);
        }

        /*/// <summary>
        /// This re-generates the room definitions from the map.  This should be performed only when those may have changed.
        /// </summary>
        public void UpdateRoomDefs()
        {
            //Here we test to see if the base map - that is, the map we started with before adding in any Map Sections, had a Walls def or a Floors def.  If not, we assume it's a vanilla map.
            Tuple<string, string> baseMapDefs = GetWallsAndFloorsFromMap(Loader.loader.Load<Map>(location.mapPath, StardewModdingAPI.ContentSource.GameContent));
            if(baseMapDefs.Item1 == "" && baseMapDefs.Item2 == "")
            {
                //Vanilla maps do have walls and floors, so we will use a provided list of their walls and floors.
                if(location is FarmHouse)
                    LoadDefaults(location as FarmHouse);
            }
            //This gets the walls and floors properties as strings from this location's map.  Appended Map Sections should be able to affect this.
            Tuple<string, string> mapDefs = GetWallsAndFloorsFromMap(location.map);
            Logger.Log(string.Format("Walls: {0}\nFloors: {1}", mapDefs.Item1, mapDefs.Item2));
            ReadPropertiesIntoFacades(mapDefs.Item1, mapDefs.Item2);
            Logger.Log("The following rooms have been created:");
            foreach(int roomID in walls.Keys)
            {
                Logger.Log(walls[roomID].ToString());
            }

            LoadStoredWallpaper();
        }

        public List<Rectangle> GetWalls()
        {
            List<Rectangle> outRects = new List<Rectangle>();
            foreach (int roomID in walls.Keys)
            {
                foreach(Rectangle rect in walls[roomID].facades)
                {
                    outRects.Add(rect);
                }
            }
            return outRects;
        }

        public void DoSetVisibleWallpaper(int whichRoom, int which)
        {
            location.updateMap();
            int index = which % 16 + which / 16 * 48;
            Logger.Log(string.Format("Chosen index {0}.", index));
            if(whichRoom == -1)
            {
                foreach(int roomID in walls.Keys)
                {
                    foreach(Rectangle rect in walls[roomID].facades)
                    {
                        for (int x = rect.X; x < rect.Right; x++)
                        {
                            for (int y = rect.Y; y < rect.Bottom; y++)
                            {
                                Maps.MapUtilities.SetWallMapTileIndexForAnyLayer(location.map, x, y, index);
                            }
                        }
                    }
                }
            }
            else
            {
                if (whichRoom >= walls.Count)
                    return;
                Logger.Log("Attempting to find room via roomID " + whichRoom + ":");
                foreach(int roomID in walls.Keys)
                {
                    Logger.Log(roomID.ToString());
                }
                Maps.Room room = walls[whichRoom];
                Logger.Log(string.Format("Applying to {0} wall rectangles in \"{1}\"", room.facades.Count, room.name));

                foreach(Rectangle rect in room.facades)
                {
                    for (int x = rect.X; x < rect.Right; x++)
                    {
                        for (int y = rect.Y; y < rect.Bottom; y++)
                        {
                            Maps.MapUtilities.SetWallMapTileIndexForAnyLayer(location.map, x, y, index);
                        }
                    }
                }
            }
        }

        public int GetRoomIDForWall(Rectangle wall)
        {
            foreach(int roomID in walls.Keys)
            {
                if (walls[roomID].facades.Contains(wall))
                    return roomID;
            }
            Logger.Log("Missing wall definition!");
            return -1;
        }

        /// <summary>
        /// Gets and stores the indices of wallpapers by finding them in the map itself.  Used for setting missing wallpaper values.  Can be used to update the wallpaper list as well.
        /// </summary>
        public void GetWallpaperIndices()
        {
            foreach(int roomID in walls.Keys)
            {

            }
        }

        public void LoadStoredWallpaper()
        {
            Logger.Log(string.Format("{0} has {1} wallpaper definitions.  Applying to defined rooms...", location.Name, location.wallPaper.Count));
            for(int wallpaper = 0; wallpaper < location.wallPaper.Count && wallpaper < walls.Count; wallpaper++)
            {
                if(walls.ContainsKey(wallpaper))
                    walls[wallpaper].index = location.wallPaper[wallpaper];
            }
            Logger.Log(string.Format("After applying {0} wallpapers:", location.wallPaper.Count));
            foreach (int roomID in walls.Keys)
            {
                Logger.Log(walls[roomID].ToString());
            }
        }

        internal void ReadPropertiesIntoFacades(string wallString, string floors)
        {
            wallString = Strings.Cleanup(wallString);
            if (wallString == "")
                return;
            string[] walls = wallString.Split(' ');
            for(int i = 0; i < walls.Length; i += 5)
            {
                try
                {
                    Rectangle newRect = new Rectangle(Convert.ToInt32(walls[i]), Convert.ToInt32(walls[i + 1]), Convert.ToInt32(walls[i + 2]), Convert.ToInt32(walls[i + 3]));
                    string roomName = walls[i + 4];
                    int thisRoomID = -1;
                    foreach(int roomID in this.walls.Keys)
                    {
                        if(this.walls[roomID].name == roomName)
                        {
                            thisRoomID = roomID;
                            break;
                        }
                    }
                    if(thisRoomID == -1)
                    {
                        thisRoomID = GenerateID();
                    }
                    if (!this.walls.ContainsKey(thisRoomID))
                    {
                        this.walls.Add(thisRoomID, new Maps.Room(roomName, thisRoomID));
                    }
                    this.walls[thisRoomID].facades.Add(newRect);
                }
                catch (IndexOutOfRangeException)
                {
                    string wallStringAttempt = "";
                    while(i < walls.Length)
                    {
                        wallStringAttempt += walls[i] + " ";
                        i++;
                    }
                    Logger.Log(string.Format("Incomplete wall definition!  Walls must be defined as [x y width height room].  There were insufficient values for the room {0}", wallStringAttempt), StardewModdingAPI.LogLevel.Warn);
                }
                catch (FormatException)
                {
                    string wallStringAttempt = "";
                    for(int j = 0; j < 5; j++)
                    {
                        wallStringAttempt += walls[i + j] + " ";
                    }
                    Logger.Log(string.Format("Invalid wall definition!  Walls must be defined as [x y width height room].  The invalid wall definition was {0}\nThe invalid wall was skipped.", wallStringAttempt), StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        private int GenerateID(bool walls = true)
        {
            int id = 0;
            while (true)
            {
                if (!this.walls.ContainsKey(id))
                    return id;
                id++;
            }
        }

        internal Tuple<string,string> GetWallsAndFloorsFromMap(Map map)
        {
            string WallsData = "";
            string FloorsData = "";

            map.Properties.TryGetValue("Walls", out PropertyValue definedWalls);
            if (definedWalls != null)
                WallsData = Strings.Cleanup(definedWalls.ToString());
            map.Properties.TryGetValue("Floors", out PropertyValue definedFloors);
            if (definedFloors != null)
                FloorsData = Strings.Cleanup(definedFloors.ToString());
            return new Tuple<string, string>(WallsData, FloorsData);
        }

        internal void LoadDefaults(FarmHouse house)
        {
            try
            {
                Logger.Log("Loading provided vanilla walls and floors...");
                string outStream = "";
                JObject defaults = Loader.loader.Load<JObject>("assets/defaults/vanilla map walls and floors.json", StardewModdingAPI.ContentSource.ModFolder);
                if (defaults == null)
                    Logger.Log("Error reading the file!");
                JObject levels = defaults["upgrade levels"] as JObject;
                JArray rects = levels[house.upgradeLevel.ToString()] as JArray;
                Logger.Log("Found " + rects.Count + " rects for upgrade level " + house.upgradeLevel.ToString());
                foreach(JToken def in rects.Children())
                {
                    JArray facade = def as JArray;
                    foreach(JToken token in facade)
                    {
                        outStream += token.ToString() + " ";
                    }
                }
                Maps.MapUtilities.MergeProperties(house.map, "Walls", outStream, insertAt: 0, autoSpace: true);
            }
            catch(Exception e)
            {
                Logger.Log(e.Message + "\n" + e.StackTrace, StardewModdingAPI.LogLevel.Error);
            }
        }*/
    }
}
