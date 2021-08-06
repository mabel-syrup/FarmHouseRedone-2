using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using FarmHouseRedone.Patching.Patches;
using FarmHouseRedone.States;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

namespace FarmHouseRedone
{
    class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            //Set up globally-accessible helpers.
            Logger.monitor = Monitor;
            StatesHandler.config = helper.ReadConfig<Config>();
            Logger.debugMode = StatesHandler.config.debug;
            Loader.modPath = helper.DirectoryPath;
            Loader.loader = helper.Content;
            Loader.spriteSheet = Loader.loader.Load<Texture2D>("assets/sprites.png", ContentSource.ModFolder);
            ObjectHandling.ObjectIDHelper.Init();
            Loader.data = helper.Data;
            UI.Translation.translation = helper.Translation;
            Patcher.reflector = helper.Reflection;
            ContentPacks.PackHandler.Init(helper.ContentPacks);
            UI.HouseMenu.Init();

            Harmony harmony = Patcher.harmony;
            //Walls
            harmony.Patch(
                original: helper.Reflection.GetMethod(new DecoratableLocation(), "doSetVisibleWallpaper").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_doSetVisibleWallpaper_Patch), nameof(DecoratableLocation_doSetVisibleWallpaper_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.setWallpaper)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_setWallpaper_Patch), nameof(DecoratableLocation_setWallpaper_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.setWallpapers)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_setWallpapers_Patch), nameof(DecoratableLocation_setWallpapers_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getWalls)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_getWalls_Patch), nameof(FarmHouse_getWalls_Patch.Prefix)))
            );
            /*
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.getWalls)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_getWalls_Patch), nameof(DecoratableLocation_getWalls_Patch.Postfix)))
            );*/
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getWalls)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_getWalls_Patch), nameof(FarmHouse_getWalls_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.getWallForRoomAt)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_getWallForRoom_Patch), nameof(DecoratableLocation_getWallForRoom_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.isTileOnWall)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_isTileOnWall_Patch), nameof(DecoratableLocation_isTileOnWall_Patch.Prefix)))
            );
            //Floors
            harmony.Patch(
                original: helper.Reflection.GetMethod(new DecoratableLocation(), "doSetVisibleFloor").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_doSetVisibleFloor_Patch), nameof(DecoratableLocation_doSetVisibleFloor_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.setFloor)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_setFloor_Patch), nameof(DecoratableLocation_setFloor_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.setFloors)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_setFloors_Patch), nameof(DecoratableLocation_setFloors_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getFloors)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_getFloors_Patch), nameof(FarmHouse_getFloors_Patch.Prefix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.getFloors)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_getFloors_Patch), nameof(DecoratableLocation_getFloors_Patch.Postfix)))
            );
            harmony.Patch(
                original: AccessTools.Method(typeof(DecoratableLocation), nameof(DecoratableLocation.getFloorAt)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(DecoratableLocation_getFloorAt_Patch), nameof(DecoratableLocation_getFloorAt_Patch.Prefix)))
            );

            harmony.Patch(
                original: helper.Reflection.GetMethod(new GameLocation(), "houseUpgradeOffer").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(GameLocation_houseUpgradeOffer_Patch), nameof(GameLocation_houseUpgradeOffer_Patch.Prefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Objects.Wallpaper), nameof(StardewValley.Objects.Wallpaper.placementAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Wallpaper_placementAction_Patch), nameof(Wallpaper_placementAction_Patch.Prefix)))
            ); 
            harmony.Patch(
                 original: AccessTools.Method(typeof(StardewValley.Objects.Furniture), nameof(StardewValley.Objects.Furniture.GetAdditionalFurniturePlacementStatus)),
                 postfix: new HarmonyMethod(AccessTools.Method(typeof(Furniture_GetAdditionalFurniturePlacementStatus_Patch), nameof(Furniture_GetAdditionalFurniturePlacementStatus_Patch.Postfix)))
            );
            /*harmony.Patch(
                 original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getEntryLocation)),
                 prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_getEntryLocation_Patch), nameof(FarmHouse_getEntryLocation_Patch.Prefix)))
            );*/

            /*harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.warpFarmer), new Type[]
                {
                    typeof(LocationRequest),
                    typeof(int),
                    typeof(int),
                    typeof(int)
                }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(Game1_warpFarmer_Patch), nameof(Game1_warpFarmer_Patch.Prefix)))
            );*/
            /*Patcher.Patch(
                Patcher.Method(new DecoratableLocation(), "doSetVisibleFloor"),
                prefix: Patcher.Method(typeof(DecoratableLocation_doSetVisibleFloor_Patch), "Prefix")
            );*/

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
            helper.Events.Display.MenuChanged += Display_MenuChanged;
            helper.Events.GameLoop.SaveCreated += GameLoop_SaveCreated;
        }

        private void GameLoop_SaveCreated(object sender, SaveCreatedEventArgs e)
        {
            
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            Delegates.MenuChanged(e.NewMenu);
        }

        private void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            foreach (FarmHouseState state in StatesHandler.houseStates.Values)
            {
                state.DayEnding();
            }
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            foreach(FarmHouseState state in StatesHandler.houseStates.Values)
            {
                state.DayStarted();
            }
            if (Game1.activeClickableMenu == null)
                Delegates.MenuChanged(null);
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.H)
            {
                Game1.activeClickableMenu = new UI.UpgradesMenu(StatesHandler.GetHouseState(Utility.getHomeOfFarmer(Game1.player)));
                //Patcher.reflector.GetMethod(Game1.currentLocation, "houseUpgradeOffer").Invoke();
                //StardewValley.HouseRenovation.ShowRenovationMenu();
            }
        }

        private void DummyRecipient(string pack)
        {
            Logger.Log("Chose pack " + pack);
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            Patcher.harmony.Patch(
                original: Patcher.reflector.GetMethod(new FarmHouse(), "resetLocalState").MethodInfo,
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmHouse_resetLocalState_Patch), nameof(FarmHouse_resetLocalState_Patch.Prefix)))
            );
            States.StatesHandler.Init();
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            ObjectHandling.ObjectIDHelper.Init();
        }
    }
}
