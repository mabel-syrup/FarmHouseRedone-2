using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using FarmHouseRedone.States;
using FarmHouseRedone.ContentPacks;
using xTile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace FarmHouseRedone.UI
{
    public class HousePreviewMenu : IClickableMenu
    {
        Map map;
        Background bg;
        UpgradeModel model;
        FarmHouseState state;

        public HousePreviewMenu(FarmHouseState state, UpgradeModel model)
        {
            this.model = model;
            this.state = state;
            FarmHouseState cloneState = new FarmHouseState(state);
            cloneState.Reset();
            if (model.IsBase())
            {
                cloneState.location.upgradeLevel = model.GetBase();
                cloneState.Reset();
            }
            else
            {
                cloneState.ApplyUpgrade(model);
            }
            map = cloneState.location.map;

            bg = new Background(Color.Black, false);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (model.IsBase())
            {
                state.location.upgradeLevel = model.GetBase();
            }
            else
            {
                state.appliedUpgrades.Add(model.ID);
            }
            state.mapPath = state.GetBaseMapPath();
            state.UpdateFromMapPath();
            state.Reset();
            this.cleanupBeforeExit();
            Game1.activeClickableMenu.exitThisMenu();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            bg.draw(b);
            if (map == null || Game1.mapDisplayDevice == null)
                return;
            int layerZoom = 1;
            for(int i = 1; i < 10; i++)
            {
                if (map.Layers[0].LayerWidth * 16 * i <= Game1.viewport.Width && map.Layers[0].LayerHeight * 16 * i <= Game1.viewport.Height)
                {
                    layerZoom = i;
                }
            }
            foreach (xTile.Layers.Layer layer in map.Layers)
            {
                layer.Draw(Game1.mapDisplayDevice, new xTile.Dimensions.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new xTile.Dimensions.Location(Game1.viewport.Width / 2 - layer.LayerWidth * 8 * layerZoom, Game1.viewport.Height / 2 - layer.LayerHeight * 8 * layerZoom), false, layerZoom);
            }
        }
    }
}
