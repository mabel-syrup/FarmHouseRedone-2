using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using FarmHouseRedone.Maps;
using xTile;
using StardewModdingAPI;
using StardewValley;

namespace FarmHouseRedone.ContentPacks
{
    public class UpgradeModel
    {
        public string ID { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public string Base { get; set; }
        public string Map { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public Dictionary<string,string> Cost { get; set; }
        public string Days { get; set; }
        public List<SectionModel> Sections { get; set; }
        public string Requirements { get; set; }
        public string Entry { get; set; }
        public List<string> With { get; set; }

        public string GetMap(int index = 0)
        {
            if (Map != null)
                return Map;
            if (Sections == null || index >= Sections.Count || Sections[index].Map == null)
                return "";
            return Sections[index].Map;
        }

        public string GetPosition(int index = 0)
        {
            if (Position != null)
                return Position;
            if(Sections.Count > index)
                return Sections[index].Position;
            return "0 0";
        }

        public string GetEntry()
        {
            if (Entry == null)
                return "";
            return Entry;
        }

        public string GetDescription(States.FarmHouseState state = null)
        {
            if (Description == null)
                return "";
            if (Description.Contains('{') || Description.Contains('}') && state != null)
            {
                return PackHandler.Evaluate(PackHandler.ParseOperation(Description, state, this)).ToString();
            }
            return Description;
        }

        public string GetName(States.FarmHouseState state = null)
        {
            if (Name == null)
            {
                if (IsBase())
                    return "Level " + GetBase() + " House";
            }
            if((Name.Contains('{') || Name.Contains('}')) && state != null)
            {
                return PackHandler.Evaluate(PackHandler.ParseOperation(Name, state, this)).ToString();
            }
            return Name;
        }

        public string GetGroup()
        {
            if (Group == null)
            {
                if (IsBase())
                    return "Base";
                return "";
            }
            return Group;
        }

        public List<string> GetWith()
        {
            if (With == null)
                return new List<string>();
            return With;
        }

        public List<SectionModel> GetSections()
        {
            if (Sections != null)
                return Sections;
            return new List<SectionModel> { new SectionModel { Map = Map, Position = Position } };
        }

        public int GetPrice()
        {
            if (Cost == null || !Cost.ContainsKey("Money"))
                return 0;
            if(int.TryParse(Cost["Money"], out int price))
                return price;
            if (double.TryParse(PackHandler.Evaluate(PackHandler.ParseOperation(Cost["Money"], States.StatesHandler.GetHouseState(Utility.getHomeOfFarmer(Game1.player)), this)).ToString(), out double price1))
                return (int)price1;
            return 0;
        }

        public string GetRequirements()
        {
            if (Requirements == null)
                return "";
            return Requirements;
        }

        public Dictionary<StardewValley.Object, int> GetMaterials()
        {
            Dictionary<StardewValley.Object, int> materials = new Dictionary<StardewValley.Object, int>();
            if (Cost == null)
                return materials;
            foreach(string material in Cost.Keys)
            {
                if (material == "Money")
                    continue;
                StardewValley.Object matObject = new StardewValley.Object(Convert.ToInt32(material), 1);
                //StardewValley.Object matObject = new StardewValley.Object(ObjectHandling.ObjectIDHelper.GetID(material), 1);
                if (int.TryParse(Cost[material], out int qty))
                    materials[matObject] = qty;
                if (double.TryParse(PackHandler.Evaluate(PackHandler.ParseOperation(Cost[material], States.StatesHandler.GetHouseState(Utility.getHomeOfFarmer(Game1.player)), this)).ToString(), out double qty1))
                    materials[matObject] = (int)qty1;

            }
            return materials;
        }

        public bool IsBase()
        {
            return Base != null;
        }

        public int GetBase()
        {
            if (Base == null)
                return -1;
            try 
            {
                return Convert.ToInt32(Base);
            }
            catch (FormatException)
            {
                Logger.Log($"The value for \"Base\" in {GetName()} was not in the correct format!  Base needs to represent a numeric value.  Given {Base}");
            }
            return -1;
        }

        public Vector2 ConvertPosition(Vector2 offset, Map map, string sectionPos)
        {
            string[] positionValues = sectionPos.Split(' ');
            try
            {
                Vector2 mapBounds = MapUtilities.GetMapSize(map);
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
                Logger.Log($"Couldn't parse the position value for upgrade \"{this.ID}\"!  " +
                    $"Positions must be given as\n\tx y\nwith a coordinate prefaced with \"<\" to denote it is measured from the left or bottom.", LogLevel.Warn);
                return new Vector2(-1000, -1000);
            }
        }

        public int GetDays()
        {
            if (Days == null)
                return 3;
            try
            {
                return Convert.ToInt32(Days);
            }
            catch (FormatException)
            {
                Logger.Log($"The value for \"Base\" in {GetName()} was not in the correct format!  Base needs to represent a numeric value.  Given {Base}");
                return 3;
            }
        }

        public override string ToString()
        {
            return ID;
        }
    }

    public class SectionModel
    {
        public string Map { get; set; }
        public string Position { get; set; }
    }
}
