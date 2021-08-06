using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using FarmHouseRedone.States;
using xTile;
using xTile.Layers;
using Microsoft.Xna.Framework;

namespace FarmHouseRedone.Maps
{
    public class Room
    {
        public string name;
        public List<Wall> walls;
        public int wallIndex;
        public List<Floor> floors;
        public int floorIndex;

        public Room(string name)
        {
            this.name = name;
            walls = new List<Wall>();
            wallIndex = 0;
            floors = new List<Floor>();
            floorIndex = 0;
        }

        public void Init(Map map)
        {
            bool wallFound = false;
            foreach (Wall wall in walls)
            {
                if (wallFound)
                    break;
                for (int x = wall.region.X; x < wall.region.Right; x++)
                {
                    if (wallFound)
                        break;
                    for (int y = wall.region.Y; y < wall.region.Bottom; y++)
                    {
                        if (wallFound)
                            break;
                        int index = MapUtilities.GetWallpaperIndex(map, x, y);
                        if (index != -1)
                        {
                            wallIndex = index;
                            wallFound = true;
                        }
                    }
                }
            }
            bool floorFound = false;
            foreach (Floor floor in floors)
            {
                if (floorFound)
                    break;
                for (int x = floor.region.X; x < floor.region.Right; x++)
                {
                    if (floorFound)
                        break;
                    for (int y = floor.region.Y; y < floor.region.Bottom; y++)
                    {
                        if (floorFound)
                            break;
                        int index = MapUtilities.GetFloorIndex(map, x, y);
                        if (index != -1)
                        {
                            floorIndex = index;
                            floorFound = true;
                        }
                    }
                }
            }
        }

        public void Apply(Map map)
        {
            foreach(Facade wall in walls)
            {
                wall.Apply(map, wallIndex);
            }
            foreach (Facade floor in floors)
            {
                floor.Apply(map, floorIndex);
            }
        }

        public bool PointWithinWall(Point p, Map map)
        {
            foreach(Facade wall in walls)
            {
                if (wall.region.Contains(p))
                {
                    foreach(Layer layer in map.Layers)
                    {
                        if (layer.Tiles[p.X, p.Y] != null && layer.Tiles[p.X, p.Y].TileSheet.ImageSource.Contains("walls_and_floors") && layer.Tiles[p.X,p.Y].TileIndex < 336)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool PointWithinFloor(Point p)
        {
            foreach (Facade floor in floors)
            {
                if (floor.region.Contains(p))
                    return true;
            }
            return false;
        }

        public bool IsEmpty()
        {
            return walls.Count == 0 && floors.Count == 0;
        }

        public bool PointWithinRoom(Point p)
        {
            if (IsEmpty())
                return false;
            return GetRoomBounds().Contains(p);
        }

        public float GetLinearDistanceToCenter(Point p)
        {
            if (IsEmpty())
                return float.PositiveInfinity;
            Point center = GetRoomBounds().Center;
            return (float)Math.Sqrt(Math.Pow(p.X - center.X, 2) + Math.Pow(p.Y - center.Y, 2));
        }

        public Rectangle GetRoomBounds()
        {
            Rectangle roomBounds = walls.Count > 0 ? walls[0].region : floors[0].region;
            foreach(Wall wall in walls)
            {
                roomBounds.X = Math.Min(roomBounds.X, wall.region.X);
                roomBounds.Y = Math.Min(roomBounds.Y, wall.region.Y);
                roomBounds.Width = Math.Max(roomBounds.Right, wall.region.Right) - roomBounds.X;
                roomBounds.Height = Math.Max(roomBounds.Bottom, wall.region.Bottom) - roomBounds.Y;
            }
            Logger.Log($"{name}'s bounds: {roomBounds}");
            return roomBounds;
        }

        public void SetWall(int index, Map map)
        {
            wallIndex = index;
            Apply(map);
            Save();
        }

        public void SetFloor(int index, Map map)
        {
            floorIndex = index;
            Apply(map);
            Save();
        }

        private void Save()
        {
            foreach(DecoratableState state in StatesHandler.decoratableStates.Values)
            {
                if (state.rooms.ContainsKey(name))
                    state.Save();
            }
        }

        public override string ToString()
        {
            string outString = string.Format("Room \"{0}\": {1} walls at index {2}, {3} floors at index {4}", name, walls.Count, wallIndex, floors.Count, floorIndex);
            outString += "\nWalls: ";
            foreach(Facade wall in walls)
            {
                outString += wall.ToString() + " ";
            }
            outString += "\nFloors: ";
            foreach(Facade floor in floors)
            {
                outString += floor.ToString() + " ";
            }
            return outString;
        }
    }
}
