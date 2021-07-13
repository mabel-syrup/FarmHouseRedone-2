using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using FarmHouseRedone.UI;

namespace FarmHouseRedone.Patching.Patches
{
    class GameLocation_houseUpgradeOffer_Patch
    {
        public static bool Prefix()
        {
            Game1.drawDialogue(Game1.getCharacterFromName("Robin"), Translation.Translate("houseUpgradeDialogue"));
            Game1.afterDialogues += (Game1.afterFadeFunction)(() =>
            {
                Game1.activeClickableMenu = new UpgradesMenu(States.StatesHandler.GetHouseState(Utility.getHomeOfFarmer(Game1.player)));
            }
            );
            
            return false;
        }
    }
}
