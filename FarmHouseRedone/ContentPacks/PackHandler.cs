using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Newtonsoft.Json.Linq;

namespace FarmHouseRedone.ContentPacks
{
    public static class PackHandler
    {
        public static IContentPackHelper packHelper;
        public static Dictionary<string, IContentPack> packs;

        public static void Init(IContentPackHelper helper)
        {
            packHelper = helper;
            packs = new Dictionary<string, IContentPack>();
            GenerateFakePack();
            foreach (IContentPack pack in packHelper.GetOwned())
            {
                Logger.Log($"Reading content pack: {pack.Manifest.Name} {pack.Manifest.Version} from {pack.DirectoryPath}");
                if (!pack.HasFile("content.json"))
                {
                    Logger.Log("Couldn't read FHR mod \"" + pack.Manifest.Name + "\" because it is missing its content.json", LogLevel.Warn);
                }
                packs.Add(pack.Manifest.UniqueID, pack);
            }
        }

        private static void GenerateFakePack()
        {
            IContentPack vanilla = packHelper.CreateTemporary(
                directoryPath: Loader.modPath,
                id: "FHRVanillaInternalOnly",
                name: "Vanilla",
                description: "This will not modify the game files, but FHR will continue to run behind the scenes.",
                author: "ConcernedApe",
                version: new SemanticVersion(1, 5, 4)
            );
            packs.Add(vanilla.Manifest.UniqueID, vanilla);
        }

        public static IContentPack GetPack(string id)
        {
            if (packs.ContainsKey(id))
            {
                return packs[id];
            }
            Logger.Log("Couldn't find pack \"" + id + "\"!", LogLevel.Warn);
            return null;
        }

        public static Pack GetPackData(string id)
        {
            if (packs.ContainsKey(id))
            {
                return packs[id].ReadJsonFile<Pack>("content.json");
            }
            return Loader.loader.Load<Pack>("content.json", ContentSource.ModFolder);
        }
    }
}
