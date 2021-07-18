using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

namespace FarmHouseRedone.UI
{
    public class UpgradesMenu : IClickableMenu
    {
        List<UpgradeMenuItem> items;
        public List<string> nouns;
        public List<string> adjectives;
        Background bg;
        public Vector2 animationController;
        States.FarmHouseState state;

        public ContentPacks.UpgradeModel selectedModel;

        public bool isOpeningAnimation;

        public Rectangle bounds;

        public Animation openingAnimation;

        public UpgradesMenu(States.FarmHouseState state)
        {
            int xMargin = 32;
            int yMargin = 32;
            this.state = state;
            isOpeningAnimation = true;
            animationController = Vector2.Zero;
            bg = new Background(Color.Black, false);
            bounds = new Rectangle(Game1.viewport.Width/2 - (600 + xMargin/2), 150 - yMargin/2, 1200 + xMargin, 600 + yMargin);
            items = new List<UpgradeMenuItem>();
            Dictionary<string, List<string>> names = Loader.loader.Load<Dictionary<string, List<string>>>("assets/defaults/names.json", ContentSource.ModFolder);
            nouns = names["Noun"];
            adjectives = names["Adjective"];
            ContentPacks.Pack pack = ContentPacks.PackHandler.GetPackData(state.packID);

            List<ContentPacks.UpgradeModel> models = ContentPacks.PackHandler.GetPackData(state.packID).GetAvailableModels(state);

            for (int i = 11; i >= 0; i--)
            {
                ContentPacks.UpgradeModel model = null;
                if (i < models.Count)
                    model = models[i];
                else
                {
                    model = new ContentPacks.UpgradeModel
                    {
                        Name = adjectives[Game1.random.Next(adjectives.Count - 1)] + " " + nouns[Game1.random.Next(nouns.Count - 1)]
                    };
                }
                items.Add(new UpgradeMenuItem(state.packID, model, (Game1.viewport.Width/2 - 600) + (i % 2) * 600, bounds.Y + yMargin/2 + ((11 - i)/2) * 100, i >=10, SelectionMade));
            }

            selectedModel = null;

            openingAnimation = new Animation(new List<KeyFrame>
            {
                new KeyFrame(0.5f, new Vector2(0, -bounds.Bottom)),
                new KeyFrame(2.5f, new Vector2(0, 0))
            }); 
            DelayedAction.playSoundAfterDelay("openChest", 500, pitch: -1);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (isOpeningAnimation)
            {
                openingAnimation.Update(time);
                animationController.Y = openingAnimation.animationController.Y;

                foreach (UpgradeMenuItem item in items)
                {
                    item.animationController.Y = openingAnimation.animationController.Y;
                }

                if (openingAnimation.isFinished)
                {
                    openingAnimation.isFinished = false;
                    isOpeningAnimation = false;
                }
            }
            else
            {
                foreach (UpgradeMenuItem item in items)
                {
                    item.Update(time);
                }
            }
        }

        public void SelectionMade(ContentPacks.UpgradeModel model)
        {
            Logger.Log("Selected " + model.GetName());
            selectedModel = model;
            string costString = "";
            Dictionary<StardewValley.Object, int> materials = model.GetMaterials();
            List<StardewValley.Object> orderedKeyList = materials.Keys.ToList();
            for (int i = 0; i < materials.Count; i++)
            {
                costString += materials[orderedKeyList[i]] + " " + orderedKeyList[i].name + (i == materials.Count - 2 ? ", and " : (i == materials.Count - 1 ? "." : " "));
            }

            Game1.currentLocation.createQuestionDialogue(
                model.GetDescription() + $"  This will take {model.GetDays()} days to complete, plus {model.GetPrice().ToString() + "g"} and {costString}  Are you interested?",
                Game1.currentLocation.createYesNoResponses(),
                ConfirmSelection,
                Game1.getCharacterFromName("Robin"));
        }

        public void ConfirmSelection(Farmer who, string which)
        {
            Logger.Log(who.name + " chose " + which);
            if (which.Equals("Yes"))
            {
                bool hasMaterials = true;
                Dictionary<StardewValley.Object, int> materials = selectedModel.GetMaterials();
                foreach(StardewValley.Object material in materials.Keys)
                {
                    if (!Game1.player.hasItemInInventory(material.parentSheetIndex, materials[material]))
                        hasMaterials = false;
                }
                if (who.Money >= selectedModel.GetPrice() && hasMaterials)
                {
                    who.Money -= selectedModel.GetPrice();
                    foreach (StardewValley.Object material in materials.Keys)
                    {
                        who.removeItemsFromInventory(material.parentSheetIndex, materials[material]);
                    }
                    state.SetCurrentUpgrade(selectedModel.ID, selectedModel.GetDays());
                    string confirmDialogue = Translation.Translate("upgradeConfirm", selectedModel.GetDays(), selectedModel.GetDays() == 1 ? Translation.Translate("daySingular") : Translation.Translate("dayPlural"));
                    Game1.drawDialogue(Game1.getCharacterFromName("Robin"), confirmDialogue);
                }
                else
                {
                    if (who.Money < selectedModel.GetPrice())
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:NotEnoughMoney3"));
                    else
                    {
                        string notEnough = Translation.Translate("houseUpgradeNotEnough");
                        Game1.drawDialogue(Game1.getCharacterFromName("Robin"), notEnough);
                    }
                    Game1.afterDialogues += new Game1.afterFadeFunction(ReturnToThisMenu);
                }
            }
            else
            {
                ReturnToThisMenu();
            }
        }

        public void ReturnToThisMenu()
        {
            selectedModel = null;
            foreach (UpgradeMenuItem item in items)
            {
                item.isSelected = false;
            }
            Game1.activeClickableMenu = this;
        }

        public override void performHoverAction(int x, int y)
        {
            for(int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].Hover(x, y))
                    return;
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].isSelected)
                    return;
            }
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].Click(x, y))
                    return;
            }

        }

        public override void draw(SpriteBatch b)
        {
            bg.draw(b); 
            b.Draw(Loader.spriteSheet, new Vector2(bounds.X + 16, 650 - 64 - 56), new Rectangle(0, 68, 150, 75), Color.White, 0f, Vector2.Zero, 8f, SpriteEffects.None, 1f);
            if (!isOpeningAnimation)
                b.Draw(Loader.spriteSheet, new Rectangle(bounds.X + 16, -64 - 64, 1200, 192), new Rectangle(154, 68, 8, 24), Color.Wheat);
            b.Draw(Loader.spriteSheet, new Rectangle(bounds.X - 48 - 64, 0, 128, Game1.viewport.Height), new Rectangle(150, 83, 16, 8), Color.Wheat);
            b.Draw(Loader.spriteSheet, new Rectangle(bounds.X + 16 + 1200, 0, 128, Game1.viewport.Height), new Rectangle(150, 83, 16, 8), Color.Wheat);
            /*IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 373, 18, 18),
                this.bounds.X,
                this.bounds.Y,
                this.bounds.Width,
                this.bounds.Height,
                Color.White,
                4f,
                false
            );*/
            foreach (UpgradeMenuItem item in items)
            {
                item.Draw(b);
            }
            b.Draw(Loader.spriteSheet, new Vector2(bounds.X + 16, bounds.Bottom) + animationController, new Rectangle(0, 68, 150, 75), Color.White, 0f, Vector2.Zero, 8f, SpriteEffects.None, 1f);
            //b.Draw(Loader.spriteSheet, new Rectangle(bounds.X - 48, -64, 64, 128), new Rectangle(150, 68, 8, 16), Color.Wheat);
            //Draw Filing Cabinet Frame
            if(isOpeningAnimation)
                b.Draw(Loader.spriteSheet, new Rectangle(bounds.X + 16, -64 - 64, 1200, 192), new Rectangle(154, 68, 8, 24), Color.Wheat);
            /*b.Draw(Loader.spriteSheet, new Rectangle(bounds.X - 48 - 64, 64, 128, 64), new Rectangle(150, 75, 16, 8), Color.Wheat);
            b.Draw(Loader.spriteSheet, new Rectangle(bounds.X + 16 + 1200, 64, 128, 64), new Rectangle(150, 67, 16, 8), Color.Wheat);*/
            //b.Draw(Loader.spriteSheet, new Rectangle(bounds.Right + 16, -64, 64, 128), new Rectangle(158, 68, 8, 16), Color.Wheat);
            Utility.drawTextWithShadow(b, "Blueprints", Game1.dialogueFont, new Vector2(bounds.X + 600 - Game1.dialogueFont.MeasureString("Blueprints").X * 0.75f, bounds.Bottom + 168) + animationController, Color.Red, shadowIntensity: 0, scale: 1.5f);
            //base.draw(b);
            this.drawMouse(b);
        }
    }
}
