using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Menus;
using FarmHouseRedone.Maps;
using FarmHouseRedone.States;
using FarmHouseRedone.ContentPacks;
using xTile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Input;

namespace FarmHouseRedone.UI
{
    public class HousePreviewMenu : IClickableMenu
    {
        GameLocation location;
        Background bg;
        UpgradeModel model;
        FarmHouseState state;

        GameLocation oldLocation;

        Vector2 offset;

        public HousePreviewMenu(FarmHouseState state, UpgradeModel model)
        {
            this.model = model;
            this.state = state;

            oldLocation = Game1.currentLocation;

            Game1.locations.Add(new GameLocation(state.location.mapPath, "FakeFHRLocation"));
            Logger.Log("Created FakeFHRLocation: " + state.location.mapPath);
            location = Game1.getLocationFromName("FakeFHRLocation");
            //DecoratableState decoratableState = new DecoratableState(location);

            /*if(model.IsBase())
                location.map = ContentPacks.PackHandler.GetPack(state.packID).LoadAsset<Map>(model.GetMap());
            else
                location.map = PackHandler.GetPack(state.packID).LoadAsset<Map>(PackHandler.GetPackData(state.packID).GetBaseFor(state.location.upgradeLevel).GetMap());
            foreach (ContentPacks.UpgradeModel upgrade in ContentPacks.PackHandler.GetPackData(state.packID).Upgrades)
            {
                if (state.appliedUpgrades.Contains(upgrade.ID) && !upgrade.IsBase())
                {
                    ApplyUpgrade(model);
                }
            }
            ApplyUpgrade();

            Dictionary<string, Room> rooms = StatesHandler.GetDecorState(state.location).rooms;
            foreach (string roomName in rooms.Keys)
            {
                decoratableState.rooms[roomName] = new Room(roomName);
                decoratableState.rooms[roomName].SetWall(rooms[roomName].wallIndex, decoratableState.location.map);
                decoratableState.rooms[roomName].SetFloor(rooms[roomName].floorIndex, decoratableState.location.map);
            }*/
            /*Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = location;
            Game1.player.viewingLocation.Value = "FakeFHRLocation-" + state.location.name;
            Game1.currentLocation.resetForPlayerEntry();
            Game1.globalFadeToClear();

            Game1.displayHUD = false;
            Game1.viewport.Location = new xTile.Dimensions.Location(3136, 320);
            Game1.panScreen(0, 0);
            Game1.displayFarmer = false;*/

            this.exitFunction = OnClose;
        }

        private void ApplyUpgrade(UpgradeModel upgradeModel = null)
        {
            if (upgradeModel == null)
                upgradeModel = model;
            IContentPack pack = ContentPacks.PackHandler.GetPack(state.packID);
            Logger.Log("Pasting upgrade " + upgradeModel.ID + " at " + upgradeModel.Position);
            Map upgradeMap = pack.LoadAsset<Map>(upgradeModel.GetMap());
            MapSection upgrade = new MapSection(upgradeModel.ID, upgradeMap);
            Vector2 position = upgradeModel.ConvertPosition(offset, location.map);
            upgrade.Paste(ref offset, location.map, (int)position.X, (int)position.Y);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            state.SetCurrentUpgrade(model.ID, 3);
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && this.readyToClose() && Game1.locationRequest == null)
            {
                exitThisMenu();
            }
            if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
                Game1.panScreen(0, 4);
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
                Game1.panScreen(4, 0);
            else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
            {
                Game1.panScreen(0, -4);
            }
            else
            {
                if (!Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
                    return;
                Game1.panScreen(-4, 0);
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (Game1.IsFading())
                return;
            int num1 = Game1.getOldMouseX(false) + Game1.viewport.X;
            int num2 = Game1.getOldMouseY(false) + Game1.viewport.Y;
            if (num1 - Game1.viewport.X < 64)
                Game1.panScreen(-8, 0);
            else if (num1 - (Game1.viewport.X + Game1.viewport.Width) >= (int)sbyte.MinValue)
                Game1.panScreen(8, 0);
            if (num2 - Game1.viewport.Y < 64)
                Game1.panScreen(0, -8);
            else if (num2 - (Game1.viewport.Y + Game1.viewport.Height) >= -64)
                Game1.panScreen(0, 8);
            foreach (Keys pressedKey in Game1.oldKBState.GetPressedKeys())
                this.receiveKeyPress(pressedKey);
            if (Game1.IsMultiplayer)
                return;
        }

        public void OnClose()
        {
            Logger.Log("Current location: " + Game1.currentLocation.Name);
            Game1.currentLocation.resetForPlayerEntry();
            /*Game1.player.viewingLocation.Value = (string)null;
            Game1.displayHUD = true;
            Game1.viewportFreeze = false;
            Game1.viewport.Location = new xTile.Dimensions.Location(320, 1536);
            Game1.displayFarmer = true;
            Game1.warpFarmer(oldLocation.name, Game1.player.getTileX(), Game1.player.getTileY(), (int)Game1.player.facingDirection);
            Game1.player.forceCanMove();*/
            this.cleanupBeforeExit();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            /*bg.draw(b);
            if (location.map == null || Game1.mapDisplayDevice == null)
                return;
            int layerZoom = 1;
            for(int i = 1; i < 10; i++)
            {
                if (location.map.Layers[0].LayerWidth * 16 * i <= Game1.viewport.Width && location.map.Layers[0].LayerHeight * 16 * i <= Game1.viewport.Height)
                {
                    layerZoom = i;
                }
            }
            foreach (xTile.Layers.Layer layer in location.map.Layers)
            {
                layer.Draw(Game1.mapDisplayDevice, new xTile.Dimensions.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), new xTile.Dimensions.Location(Game1.viewport.Width / 2 - layer.LayerWidth * 8 * layerZoom, Game1.viewport.Height / 2 - layer.LayerHeight * 8 * layerZoom), false, layerZoom);
            }*/
            drawMouse(b);
        }
    }
}
