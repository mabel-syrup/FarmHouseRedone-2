using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FarmHouseRedone.UI
{
    public class PackSlot
    {
        public bool isSelected;
        public IContentPack pack;
        public Rectangle bounds;
        Texture2D packIcon;

        public PackSlot(IContentPack pack, int x, int y)
        {
            bounds = new Rectangle(x, y, 900, 150);
            this.pack = pack;
            if (pack.HasFile("icon.png"))
                packIcon = pack.LoadAsset<Texture2D>("icon.png");
            else
                packIcon = Loader.loader.Load<Texture2D>("assets/Pack_Icon.png", ContentSource.ModFolder);
        }

        public bool receiveLeftClick(int x, int y)
        {
            if (bounds.Contains(x, y))
                return true;
            return false;
        }

        public void draw(SpriteBatch b)
        {
            drawSlotBackground(b);
            drawPackName(b);
            drawIconBackground(b);
            drawPackIcon(b);
        }

        protected void drawPackName(SpriteBatch b)
        {
            Utility.drawTextWithShadow(b, pack.Manifest.Name, Game1.dialogueFont, new Vector2(bounds.X + 128 + 36, bounds.Y + 16), Game1.textColor);
            string descriptionRaw = pack.Manifest.Description;
            string[] descriptionWords = descriptionRaw.Split(' ');
            string[] lines = new string[] { "", "" };
            int line = 0;
            foreach (string word in descriptionWords)
            {
                if (lines[line].Length + word.Length <= 56)
                {
                    lines[line] += word + " ";
                }
                else if (line == 0)
                {
                    line++;
                    lines[line] += word + " ";
                }
                else
                {
                    lines[line] += "...";
                    break;
                }
            }
            Utility.drawTextWithShadow(b, lines[0], Game1.smallFont, new Vector2(bounds.X + 128 + 36, bounds.Y + 60), Game1.textColor);
            Utility.drawTextWithShadow(b, lines[1], Game1.smallFont, new Vector2(bounds.X + 128 + 36, bounds.Y + 84), Game1.textColor);
            Utility.drawTextWithShadow(b, pack.Manifest.Author, Game1.smallFont, new Vector2(bounds.Right - Game1.smallFont.MeasureString(pack.Manifest.Author).X - 8, bounds.Bottom - 36), Game1.textColor);
            Utility.drawTextWithShadow(b, pack.Manifest.Version.ToString(), Game1.smallFont, new Vector2(bounds.Right - Game1.smallFont.MeasureString(pack.Manifest.Version.ToString()).X - 8, bounds.Y + 4), Game1.textColor * 0.4f);
        }

        protected void drawPackIcon(SpriteBatch b)
        {
            b.Draw(packIcon, new Vector2(bounds.X + 2 + 9, bounds.Y + 2 + 9), Color.White);
        }

        protected void drawSlotBackground(SpriteBatch b)
            => IClickableMenu.drawTextureBox(
                b, 
                Game1.mouseCursors,
                new Rectangle(384, 396, 15, 15),
                this.bounds.X,
                this.bounds.Y,
                this.bounds.Width,
                this.bounds.Height,
                this.isSelected ? Color.Wheat : Color.White,
                2f,
                false
        );

        protected void drawIconBackground(SpriteBatch b)
            => IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 373, 18, 18),
                this.bounds.X + 2,
                this.bounds.Y + 2,
                128 + 18,
                128 + 18,
                Color.White,
                2f,
                false
        );
    }
}
