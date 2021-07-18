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
    class HouseMenu : IClickableMenu
    {
        public List<ClickableComponent> optionSlots = new List<ClickableComponent>();
        private List<PackSlot> options = new List<PackSlot>();

        private static int optionsRows;

        private const int menuWidth = 900 + 16;
        private const int packHeight = 150;

        private int currentScrollIndex = 0;

        private ConfirmButton confirmButton;

        private HouseMenu.ChoiceSelectedBehavior behavior;

        public static void Init()
        {
            optionsRows = 4;
        }

        public HouseMenu(ChoiceSelectedBehavior b) : base(
            Game1.viewport.Width / 2 - (menuWidth + IClickableMenu.borderWidth * 2) / 2,
            Game1.viewport.Height / 2 - (((packHeight / 2) * optionsRows) + 120 + IClickableMenu.borderWidth * 2) / 2,
            menuWidth/2 + IClickableMenu.borderWidth * 2,
            ((packHeight/2) * optionsRows) + 470 + IClickableMenu.borderWidth * 2)
        {
            foreach(IContentPack pack in ContentPacks.PackHandler.packs.Values)
            {
                options.Add(new PackSlot(pack, Game1.viewport.Width / 2 - 450, (Game1.viewport.Height / 2 - (optionsRows * packHeight)/2) + options.Count * packHeight));
            }
            this.behavior = b;
            confirmButton = new ConfirmButton(Confirm, Game1.viewport.Width / 2 + menuWidth / 2 - 64, Game1.viewport.Height / 2 + (packHeight * optionsRows) / 2);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            confirmButton.ReceiveLeftClick(x, y);
            PackSlot selected = null;
            foreach(PackSlot slot in options)
            {
                if (slot.receiveLeftClick(x, y)){
                    Game1.playSound("smallSelect");
                    selected = slot;
                    break;
                }
            }
            foreach(PackSlot slot in options)
            {
                slot.isSelected = false;
            }
            if (selected != null)
            {
                selected.isSelected = true;
                confirmButton.disabled = false;
            }
            else
                confirmButton.disabled = true;
        }

        public void Confirm()
        {
            Game1.playSound("axchop");
            DelayedAction.playSoundAfterDelay("hammer", 250);
            DelayedAction.playSoundAfterDelay("hammer", 500);
            Game1.globalFadeToBlack();
            foreach (PackSlot slot in options)
            {
                if (slot.isSelected)
                    behavior.Invoke(slot.pack.Manifest.UniqueID);
            }
            Game1.fadeClear();
            this.cleanupBeforeExit();
            this.exitThisMenu(false);
        }

        public override void draw(SpriteBatch b)
        {
            DrawTitle(b);
            DrawBackground(b);
            for(int i = 0; i < 4; i++)
            {
                if(i+currentScrollIndex < options.Count)
                {
                    options[i + currentScrollIndex].draw(b);
                }
                else
                {
                    DrawEmptySlot(b, i);
                }
            }
            confirmButton.Draw(b);
            base.draw(b);
            this.drawMouse(b);
        }

        public void DrawTitle(SpriteBatch b)
        {
            string title = Translation.Translate("packSelectionTitle");
            Vector2 size = Game1.dialogueFont.MeasureString(title);
            IClickableMenu.drawTextureBox(b,
                Game1.mouseCursors,
                new Rectangle(293, 360, 24, 24),
                Game1.viewport.Width / 2 - (int)size.X / 2 - 8,
                Game1.viewport.Height / 2 - (packHeight * optionsRows) / 2 - (int)size.Y - 8,
                (int)size.X + 16,
                (int)size.Y + 16,
                Color.White,
                2f,
                false);
            Utility.drawTextWithShadow(b, title, Game1.dialogueFont, new Vector2(Game1.viewport.Width / 2 - (int)size.X / 2, Game1.viewport.Height / 2 - (packHeight * optionsRows) / 2 - (int)size.Y), Game1.textColor);
        }

        protected void DrawBackground(SpriteBatch b)
            => IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 373, 18, 18),
                Game1.viewport.Width / 2 - menuWidth / 2,
                Game1.viewport.Height / 2 - (packHeight * optionsRows) / 2 - 8,
                menuWidth,
                (packHeight * optionsRows) + 16,
                Color.White,
                2f,
                false
        );

        protected void DrawEmptySlot(SpriteBatch b, int slot)
            => IClickableMenu.drawTextureBox(
                b,
                Game1.mouseCursors,
                new Rectangle(384, 396, 15, 15),
                Game1.viewport.Width / 2 - menuWidth / 2 + 8,
                Game1.viewport.Height / 2 - (packHeight * optionsRows) / 2 + (packHeight * slot),
                900,
                150,
                new Color(153, 180, 180),
                2f,
                false
        );

        public delegate void ChoiceSelectedBehavior(string pack);
    }
}
