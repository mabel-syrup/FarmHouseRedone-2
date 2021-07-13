using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmHouseRedone.ContentPacks
{
    public class Pack
    {
        public string Format { get; set; }
        public UpgradeModel[] Upgrades { get; set; } = new UpgradeModel[0];

        public UpgradeModel GetBaseFor(int level)
        {
            foreach(UpgradeModel model in Upgrades)
            {
                if (model.GetBase() == level)
                    return model;
            }
            return null;
        }

        public UpgradeModel GetModel(string ID)
        {
            foreach (UpgradeModel model in Upgrades)
            {
                if (model.ID == ID)
                    return model;
            }
            return null;
        }
    }
}
