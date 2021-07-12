using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmHouseRedone.ContentPacks
{
    public class UpgradeModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Base { get; set; }
        public string Map { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }
        public Dictionary<string,int> Cost { get; set; }
        public List<SectionModel> Sections { get; set; }
        public string Requirements { get; set; }

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
            return Sections[index].Position;
        }

        public string GetDescription()
        {
            if (Description == null)
                return "";
            return Description;
        }

        public string GetName()
        {
            if (Name == null)
            {
                if (IsBase())
                    return "Level " + GetBase() + " House";
            }
            return Name;
        }

        public int GetPrice()
        {
            if (Cost == null || !Cost.ContainsKey("money"))
                return 0;
            return Cost["money"];
        }

        public Dictionary<StardewValley.Object, int> GetMaterials()
        {
            Dictionary<StardewValley.Object, int> materials = new Dictionary<StardewValley.Object, int>();
            if (Cost == null)
                return materials;
            foreach(string material in Cost.Keys)
            {
                if (material == "money")
                    continue;
                StardewValley.Object matObject = new StardewValley.Object(ObjectHandling.ObjectIDHelper.GetID(material), 1);
                materials[matObject] = Cost[material];
            }
            return materials;
        }

        public bool IsBase()
        {
            return Base != null;
        }

        public int GetBase()
        {
            if (!IsBase())
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
    }

    public class SectionModel
    {
        public string Map { get; set; }
        public string Position { get; set; }
    }
}
