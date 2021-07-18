using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FarmHouseRedone.UI
{
    public class ConfirmButton
    {
        private Action action;

        public bool heldDown = false;
        public bool disabled = true;

        public Rectangle bounds;

        public Color disabledTint = new Color(200, 200, 200);

        public ConfirmButton(Action action, int x, int y)
        {
            this.action = action;
            this.bounds = new Rectangle(x, y, 128, 128);
        }

        public void ReceiveLeftClick(int x, int y)
        {
            if (!this.bounds.Contains(x, y) || this.action == null || disabled)
                return;
            this.action();
        }

        public void Draw(SpriteBatch b)
        {
            //128 256 64 64
            b.Draw(Game1.mouseCursors, new Vector2(bounds.X, bounds.Y), new Rectangle(128, 256, 64, 64), disabled ? disabledTint : Color.White);
        }
    }
}
