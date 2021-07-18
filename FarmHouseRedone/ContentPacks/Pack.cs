using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarmHouseRedone.States;

namespace FarmHouseRedone.ContentPacks
{
    public class Pack
    {
        public static UpgradeModel[] defaults = Loader.loader.Load<UpgradeModel[]>("assets/defaults/vanilla upgrades.json", StardewModdingAPI.ContentSource.ModFolder);

        public string Format { get; set; }
        public UpgradeModel[] Upgrades { get; set; } = new UpgradeModel[0];

        public Dictionary<string, string> Tokens = new Dictionary<string, string>();

        public void Init()
        {
            List<UpgradeModel> upgradeModels = Upgrades.ToList();
            for (int i = 0; i < 3; i++)
            {
                if(GetBaseFor(i) == null)
                {
                    upgradeModels.Add(defaults[i]);
                }
            }
            Upgrades = upgradeModels.ToArray();
        }

        public List<UpgradeModel> GetAvailableModels(FarmHouseState state)
        {
            List<UpgradeModel> outModels = new List<UpgradeModel>();
            foreach(UpgradeModel model in Upgrades)
            {
                if (state.appliedUpgrades.Contains(model.ID) || !PackHandler.CompareToRequirements(state, model))
                    continue;
                outModels.Add(model);
            }
            return outModels;
        }

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
