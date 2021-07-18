using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarmHouseRedone.ContentPacks;
using xTile;

namespace FarmHouseRedone.UI
{
    public class UpgradeMenuItem
    {
        UpgradeModel upgrade;
        public Rectangle bounds;
        Map map;
        Dictionary<StardewValley.Object, int> materials;
        int price;
        bool bottomRow;
        internal int yOffset;
        private double lerpTime;
        private double selectedTime;
        private const double lerpDuration = 0.2f;
        private const double selectedDuration = 0.4f;
        private const int maxOffset = -100;

        public bool isSelected = false;
        public bool didHover = false;

        SelectionMadeBehavior b;

        public Vector2 animationController;

        public UpgradeMenuItem(string packID, UpgradeModel upgrade, int x, int y, bool bottomRow, SelectionMadeBehavior b)
        {
            this.upgrade = upgrade;
            animationController = Vector2.Zero;
            this.bounds = new Rectangle(x, y, 600, 200);
            this.bottomRow = bottomRow;
            lerpTime = 0;
            yOffset = 0;
            if (upgrade.GetMap() == "")
                map = null;
            else
            {
                map = PackHandler.GetPack(packID).LoadAsset<Map>(upgrade.GetMap());
                map.LoadTileSheets(Game1.mapDisplayDevice);
            }
            materials = upgrade.GetMaterials();
            price = upgrade.GetPrice();
            this.b = b;
        }

        public bool Click(int x, int y)
        {
            if (InBounds(x, y))
            {
                Game1.playSound("shwip");
                isSelected = true;
                return true;
            }
            return false;
        }

        public void Update(GameTime time)
        {
            if (didHover || isSelected)
            {
                lerpTime += time.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                if (lerpTime > 0)
                    lerpTime -= time.ElapsedGameTime.TotalSeconds;
                if (lerpTime < 0)
                    lerpTime = 0;
            }
            if (isSelected)
            {
                selectedTime += time.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                selectedTime = 0;
            }
            lerpTime = Math.Min(lerpTime, lerpDuration);
            selectedTime = Math.Min(selectedTime, selectedDuration);
            yOffset = (int)Utility.Lerp(0, maxOffset, (float)(lerpTime / lerpDuration));
            yOffset += (int)Utility.Lerp(0, -25, (float)(selectedTime / selectedDuration));
            if (yOffset <= -125)
            {
                b.Invoke(upgrade);
                isSelected = false;
            }
        }

        public delegate void SelectionMadeBehavior(UpgradeModel model);

        public bool Hover(int x, int y)
        {
            didHover = false;
            if (InBounds(x, y))
            {
                didHover = true;
            }
            return didHover;
        }

        public bool InBounds(int x, int y)
        {
            return (new Rectangle(bounds.X + (int)animationController.X, bounds.Y + (int)animationController.Y + 40 + yOffset, bounds.Width, bounds.Height).Contains(x, y) || 
                new Rectangle(bounds.X + 150 - 24 + (int)animationController.X, bounds.Y - 20 + yOffset + (int)animationController.Y, (int)Game1.dialogueFont.MeasureString(upgrade.GetName()).X + 24, 60).Contains(x, y));
        }

        public void Draw(SpriteBatch b)
        {
            /*IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 396, 15, 15),
                this.bounds.X,
                this.bounds.Y,
                this.bounds.Width,
                this.bounds.Height,
                Color.White,
                2f,
                false
            );*/
            DrawBluePrint(b);
            DrawText(b);
            DrawCosts(b);
            DrawPreview(b);
        }

        private void DrawBluePrint(SpriteBatch b)
        {
            b.Draw(Loader.spriteSheet, new Vector2(this.bounds.X, this.bounds.Y + 40 + yOffset) + animationController, new Rectangle(0, 15, 150, 50), upgrade.ID != null ? Color.White : Color.BlanchedAlmond, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            if(!bottomRow)
                b.Draw(Loader.spriteSheet, new Vector2(this.bounds.X, this.bounds.Y + 40 + yOffset - 12) + animationController, new Rectangle(0, 65, 150, 3), upgrade.ID != null ? Color.White : Color.BlanchedAlmond, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            //19 * 4
        }

        private void DrawText(SpriteBatch b)
        {
            int nameWidth = (int)Game1.dialogueFont.MeasureString(upgrade.GetName()).X;
            b.Draw(Loader.spriteSheet, new Vector2(this.bounds.X + 150 - 24, this.bounds.Y - 20 + yOffset) + animationController, new Rectangle(upgrade.IsBase() ? 15 : 0, 0, 6, 15), upgrade.ID != null ? Color.White : Color.BlanchedAlmond, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Loader.spriteSheet, new Rectangle(bounds.X + 150 + (int)animationController.X, bounds.Y - 20 + yOffset + (int)animationController.Y, nameWidth, 60), new Rectangle(6 + (upgrade.IsBase() ? 15 : 0), 0, 3, 15), upgrade.ID != null ? Color.White : Color.BlanchedAlmond);
            b.Draw(Loader.spriteSheet, new Vector2(this.bounds.X + 150 + nameWidth, this.bounds.Y - 20 + yOffset) + animationController, new Rectangle(9 + (upgrade.IsBase() ? 15 : 0), 0, 6, 15), upgrade.ID != null ? Color.White : Color.BlanchedAlmond, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            Utility.drawTextWithShadow(b, upgrade.GetName(), Game1.dialogueFont, new Vector2(bounds.X + 150, bounds.Y - 12 + yOffset) + animationController, Game1.textColor * (upgrade.ID == null ? 0.6f : 1f), shadowIntensity: (upgrade.IsBase() ? 0 : 1));
            Utility.drawTextWithShadow(b, Strings.Fit(upgrade.GetDescription(), Game1.smallFont, new Vector2(bounds.Width - 166, 100)), Game1.smallFont, new Vector2(bounds.X + 150, bounds.Y + 65 + yOffset) + animationController, Color.White, shadowIntensity: 0);
        }

        private void DrawCosts(SpriteBatch b)
        {
            if(price > 0)
            {
                int priceWidth = (int)Game1.smallFont.MeasureString(price.ToString() + "g").X;
                Utility.drawTextWithShadow(b, price.ToString() + "g", Game1.smallFont, new Vector2(bounds.Right - (priceWidth + 18), bounds.Bottom - 40 + yOffset) + animationController, Color.White, shadowIntensity: 0);
                b.Draw(Game1.mouseCursors, new Vector2(bounds.Right - (priceWidth + 36), bounds.Bottom - 32 + yOffset) + animationController, new Rectangle(193, 373, 9, 9), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
            }
        }

        //193 373 9 9

        private void DrawPreview(SpriteBatch b)
        {
            if (map == null || Game1.mapDisplayDevice == null)
                return;
            IClickableMenu.drawTextureBox(
                b,
                Loader.spriteSheet,
                new Rectangle(61, 0, 15, 15),
                this.bounds.X + (int)animationController.X,
                this.bounds.Y + yOffset + (int)animationController.Y,
                128 + 12,
                128 + 18,
                Color.White,
                3f,
                false
            );
            //477
            foreach (xTile.Layers.Layer layer in map.Layers)
            {
                layer.Draw(Game1.mapDisplayDevice, new xTile.Dimensions.Rectangle(0, 0, 128, 128), new xTile.Dimensions.Location(bounds.X + 6 + (int)animationController.X, bounds.Y + 6 + yOffset + (int)animationController.Y), false, 1);
            }
            if(bounds.Y + animationController.Y > 50)
                b.Draw(Loader.spriteSheet, new Vector2(this.bounds.X - 16, this.bounds.Y + 80 + yOffset) + animationController, new Rectangle(32, 0, 28, 15), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            else
                b.Draw(Loader.spriteSheet, new Vector2(this.bounds.X - 8, this.bounds.Y + 80 + yOffset) + animationController, new Rectangle(34, 0, 24, 15), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }
    }
}
