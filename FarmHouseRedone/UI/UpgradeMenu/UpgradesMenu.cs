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
            for(int i = 11; i >= 0; i--)
            {
                ContentPacks.UpgradeModel model = null;
                if (i < ContentPacks.PackHandler.GetPackData(state.packID).Upgrades.Length)
                    model = ContentPacks.PackHandler.GetPackData(state.packID).Upgrades[i];
                else
                {
                    model = new ContentPacks.UpgradeModel();
                    model.Name = adjectives[Game1.random.Next(adjectives.Count - 1)] + " " + nouns[Game1.random.Next(nouns.Count - 1)];
                }
                items.Add(new UpgradeMenuItem(state.packID, model, (Game1.viewport.Width/2 - 600) + (i % 2) * 600, bounds.Y + yMargin/2 + ((11 - i)/2) * 100, i >=10, SelectionMade));
            }

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
            this.cleanupBeforeExit();
            Game1.activeClickableMenu = new HousePreviewMenu(state, model);
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
            b.Draw(Loader.spriteSheet, new Rectangle(bounds.X + 16, -64 - 64, 1200, 192), new Rectangle(154, 68, 8, 24), Color.Wheat);
            //b.Draw(Loader.spriteSheet, new Rectangle(bounds.Right + 16, -64, 64, 128), new Rectangle(158, 68, 8, 16), Color.Wheat);
            Utility.drawTextWithShadow(b, "Blueprints", Game1.dialogueFont, new Vector2(bounds.X + 600 - Game1.dialogueFont.MeasureString("Blueprints").X * 0.75f, bounds.Bottom + 168) + animationController, Color.Red, shadowIntensity: 0, scale: 1.5f);
            //base.draw(b);
            this.drawMouse(b);
        }
    }
}
