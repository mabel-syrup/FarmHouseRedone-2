using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using StardewValley;

namespace FarmHouseRedone.ContentPacks
{
    public static class PackHandler
    {
        public static IContentPackHelper packHelper;
        public static Dictionary<string, IContentPack> packs;

        public static List<string> professions = new List<string>
        {
            "rancher",
            "tiller",
            "butcher",
            "shepherd",
            "artisan",
            "agriculturist",
            "fisher",
            "trapper",
            "angler",
            "pirate",
            "baitmaster",
            "mariner",
            "forester",
            "gatherer",
            "lumberjack",
            "tapper",
            "botanist",
            "tracker",
            "miner",
            "blacksmith",
            "burrower",
            "excavator",
            "gemologist",
            "fighter",
            "scout",
            "brute",
            "defender",
            "acrobat",
            "desperado"
        };

        public static string[] opOrder = new string[]
        {
            "IF",
            "*/",
            "+-",
            "<=>=!=~==",
            "&&||ANDOR"
        };

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
            Pack pack;
            if (packs.ContainsKey(id))
            {
                pack = packs[id].ReadJsonFile<Pack>("content.json");
            }
            else
            {
                pack = Loader.loader.Load<Pack>("content.json", ContentSource.ModFolder);
            }
            pack.Init();
            return pack;
        }

        public static bool CompareToRequirements(States.FarmHouseState state, UpgradeModel model)
        {
            List<string> requirements = model.GetRequirements();
            foreach(string requirement in requirements)
            {
                if (!Compare(requirement, state, model))
                    return false;
            }
            return true;
        }

        public static object Token(States.FarmHouseState state, string token, object[] args)
        {
            try
            {
                switch (token)
                {
                    //House stuff
                    case "level":
                        return state.location.upgradeLevel;
                    case "upgrades":
                        if(args.Length >= 1){
                            if(int.TryParse(args[0].ToString(), out int index))
                            {
                                string upgradeID = state.appliedUpgrades[index];
                                return GetPackData(state.packID).GetModel(upgradeID);
                            }
                            return GetPackData(state.packID).GetModel(args[0].ToString());
                        }
                        return state.appliedUpgrades.Count;
                    case "hasupgrade":
                        return state.appliedUpgrades.Contains(args[0]);

                    //Global stuff
                    case "day":
                        return Game1.dayOfMonth;
                    case "dayevent":
                        return Game1.CurrentEvent == null ? "none" : Game1.CurrentEvent.FestivalName;
                    case "dayofweek":
                        return Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth);
                    case "season":
                        return Game1.currentSeason;
                    case "time":
                        return Game1.timeOfDay;
                    case "year":
                        return Game1.year;

                    //Player stuff
                    case "dailyluck":
                        return Game1.player.DailyLuck;
                    case "hasactivequest":
                        if (args.Length < 1)
                            return Game1.player.questLog.Count > 0;
                        else
                            return Game1.player.hasQuest(Convert.ToInt32(args[0]));
                    case "hascaughtfish":
                        if (args.Length < 1)
                            return Game1.player.fishCaught.Count() > 0;
                        return Game1.player.fishCaught.ContainsKey(Convert.ToInt32(args[0]));
                    case "hasflag":
                        return Game1.player.mailReceived.Contains(args[0].ToString());
                    case "hasprofession":
                        if (args.Length < 1)
                            return Game1.player.professions.Count > 0;
                        if(int.TryParse(args[0].ToString(), out int professionIndex))
                            return Game1.player.professions.Contains(Convert.ToInt32(professionIndex));
                        if (professions.Contains(args[0].ToString()))
                            return Game1.player.professions.Contains(professions.IndexOf(args[0].ToString()));
                        return false;
                    case "hasseenevent":
                        return Game1.player.eventsSeen.Contains(Convert.ToInt32(args[0]));
                    case "haswalletitem":
                        switch (args[0].ToString())
                        {
                            case "dwarf":
                            case "dwarvish":
                            case "dwarftranslation":
                            case "dwarvishtranslation":
                            case "dwarftranslationguide":
                            case "dwarvishtranslationguide":
                                return Game1.player.canUnderstandDwarves;
                            case "rustykey":
                                return Game1.player.hasRustyKey;
                            case "clubcard":
                                return Game1.player.hasClubCard;
                            case "keytotown":
                            case "keytown":
                            case "townkey":
                                return Game1.player.HasTownKey;
                            case "specialcharm":
                            case "charm":
                                return Game1.player.hasSpecialCharm;
                            case "skullkey":
                                return Game1.player.hasSkullKey;
                            case "magnifyingglas":
                                return Game1.player.hasMagnifyingGlass;
                            case "darktalisman":
                            case "talisman":
                                return Game1.player.hasDarkTalisman;
                            case "magicink":
                            case "ink":
                                return Game1.player.hasMagicInk;
                            case "bearsknowledge":
                            case "bear":
                                return Game1.player.eventsSeen.Contains(2120303);
                            case "springonionmastery":
                            case "springonion":
                                return Game1.player.eventsSeen.Contains(3910979);
                        }
                        return false;
                    case "ismainplayer":
                        return Game1.player.IsMainPlayer;
                    case "isoutdoors":
                        return Game1.player.currentLocation.IsOutdoors;
                    case "gender":
                        return Game1.player.isMale ? "Male" : "Female";
                    case "playername":
                        return Game1.player.name;
                    case "preferredpet":
                        return Game1.player.catPerson ? "Cat" : "Dog";
                    case "skilllevel":
                        switch (args[0].ToString())
                        {
                            case "combat":
                                return Game1.player.combatLevel;
                            case "farming":
                                return Game1.player.farmingLevel;
                            case "fishing":
                                return Game1.player.fishingLevel;
                            case "foraging":
                                return Game1.player.foragingLevel;
                            case "mining":
                                return Game1.player.miningLevel;
                            case "luck":
                                return Game1.player.luckLevel;
                        }
                        return 0;

                    //Relationships
                    case "hearts":
                        return Game1.player.getFriendshipHeartLevelForNPC(args[0].ToString());
                    case "relationship":
                        Character character = Game1.getCharacterFromName(args[0].ToString());
                        if (character != null)
                        {
                            if (Game1.player.friendshipData.TryGetValue(character.name, out Friendship friendship))
                            {
                                /*if (friendship.IsDivorced())
                                    return "Divorced";
                                if (friendship.IsMarried())
                                    return "Married";
                                if (friendship.IsRoommate())
                                    return "Roommate";
                                if (friendship.IsEngaged())
                                    return "Engaged";
                                if (friendship.IsDating())
                                    return "Dating";*/
                                return friendship.Status.ToString();
                            }
                        }
                        return "Unmet";
                }
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Log($"Not enough arguments supplied for the token \"{token}\"!");
            }
            Logger.Log($"Failed to parse token \"{token}\"!");
            return false;
        }

        public static object ParseToken(string token, States.FarmHouseState state, UpgradeModel model)
        {
            token = token.ToLower();
            Pack pack = GetPackData(state.packID);
            if (pack.Tokens.ContainsKey(token))
            {
                return Evaluate(ParseOperation(pack.Tokens[token], state, model));
            }
            Farmer player = Game1.player;
            string tokenName = token.Split(':')[0];
            List<string> listArgs = new List<string>();
            if (token.Contains(':'))
            {
                if (token.Split(':')[1].Contains(','))
                    listArgs = token.Split(':')[1].Split(',').ToList();
                else
                    listArgs.Add(token.Split(':')[1]);
            }
            return Token(state, tokenName, listArgs.ToArray());
        }

        public static object Operate(object first, string operatorString, object second)
        {
            Logger.Log($"Operation given {first} {operatorString} {second}");
            switch (operatorString)
            {
                case "*":
                    return Convert.ToDouble(first) * Convert.ToDouble(second);
                case "/":
                    return Convert.ToDouble(first) / Convert.ToDouble(second);
                case "+":
                    if (double.TryParse(first.ToString(), out double a) && double.TryParse(second.ToString(), out double b))
                        return a + b;
                    else
                        return first.ToString() + second.ToString();
                case "-":
                    return Convert.ToDouble(first) - Convert.ToDouble(second);
                case "=":
                case "==":
                    if (double.TryParse(first.ToString(), out double c) && double.TryParse(second.ToString(), out double d))
                    {
                        /*if (c % 1 < 0.001 && d % 1 < 0.001)
                            return (int)c == (int)d;
                        else*/
                            return c == d;
                    }
                    else
                        return first.Equals(second);
                case ">":
                    return Convert.ToDouble(first) > Convert.ToDouble(second);
                case "<":
                    return Convert.ToDouble(first) < Convert.ToDouble(second);
                case ">=":
                    if (double.TryParse(first.ToString(), out double e) && double.TryParse(second.ToString(), out double f))
                    {
                        /*if (Math.Abs(e % 1) < 0.001 && Math.Abs(f % 1) < 0.001)
                            return (int)e >= (int)f;
                        else*/
                            return e >= f;
                    }
                    else
                    {
                        Logger.Log($"Cannot compare {first} and {second} with >= because at least one side cannot be resolved into a number!");
                        return false;
                    }
                case "<=":
                    if (double.TryParse(first.ToString(), out double g) && double.TryParse(second.ToString(), out double h))
                    {
                        /*if (Math.Abs(g % 1) < 0.001 && Math.Abs(h % 1) < 0.001)
                            return (int)g <= (int)h;
                        else*/
                            return g <= h;
                    }
                    else
                    {
                        Logger.Log($"Cannot compare {first} and {second} with <= because at least one side cannot be resolved into a number!");
                        return false;
                    }
                case "!=":
                    if (double.TryParse(first.ToString(), out double j) && double.TryParse(second.ToString(), out double k))
                    {
                        /*if (j % 1 < 0.001 && k % 1 < 0.001)
                            return (int)j != (int)k;
                        else*/
                            return j != k;
                    }
                    else
                        return !first.Equals(second);
                case "~=":
                    if (double.TryParse(first.ToString(), out double l) && double.TryParse(second.ToString(), out double m))
                        return (int)l == (int)m;
                    else
                        return false;
                case "&":
                case "&&":
                case "AND":
                    return Convert.ToBoolean(first) && Convert.ToBoolean(second);
                case "|":
                case "||":
                case "OR":
                    return Convert.ToBoolean(first) || Convert.ToBoolean(second);
            }
            Logger.Log("Could not resolve operation");
            return false;
        }

        public static object Evaluate(List<object> parts)
        {
            if(parts.Count == 1)
            {
                return parts[0];
                /*Logger.Log("Final value: " + parts[0].ToString());
                try
                {
                    return Convert.ToBoolean(parts[0]);
                }
                catch (FormatException)
                {
                    Logger.Log($"Could not resolve the final value {parts[0]} to true or false!", LogLevel.Warn);
                    return false;
                }*/
            }
            string logString = "Operating ";
            foreach(object obj in parts)
            {
                logString += obj.ToString() + " ";
            }
            Logger.Log(logString);
            List<object> outObjects = new List<object>();
            int indexToReplaceAt = -1;
            for(int layer = 0; layer < opOrder.Length; layer++)
            {
                if (indexToReplaceAt != -1)
                    break;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i] is string && opOrder[layer].Contains(parts[i] as string))
                    {
                        if (parts[i] as string == "IF")
                            indexToReplaceAt = i;
                        else
                            indexToReplaceAt = i - 1;
                        break;
                    }
                }
            }
            if (indexToReplaceAt == -1)
            {
                return false;
            }

            for(int i = 0; i < parts.Count; i++)
            {
                if (i == indexToReplaceAt)
                {
                    if (parts[i] is string && parts[i] as string == "IF")
                    {
                        if (Convert.ToBoolean(parts[i + 1]))
                            outObjects.Add(parts[i + 2]);
                        else
                            outObjects.Add(parts[i + 3]);
                        i += 3;
                    }
                    else
                    {
                        outObjects.Add(Operate(parts[i], parts[i + 1].ToString(), parts[i + 2]));
                        i += 2;
                    }
                }
                else
                    outObjects.Add(parts[i]);
            }

            if (outObjects.Count == parts.Count)
            {
                Logger.Log("Infinite loop detected while parsing the requirements string!  Stopping...", LogLevel.Warn);
                return false;
            }
            return (Evaluate(outObjects));
        }

        public static List<object> ParseOperation(string operation, States.FarmHouseState state, UpgradeModel model)
        {
            Logger.Log("Parsing\n" + operation);

            MatchCollection brackets = Regex.Matches(operation, @"\(.*\)");
            foreach(Match bracket in brackets)
            {
                string bracketString = bracket.Captures[0].Value;
                string strippedString = bracketString.Substring(1, bracketString.Length - 2);
                object evaluatedBrackets = Evaluate(ParseOperation(strippedString, state, model));
                operation = operation.Replace(bracketString, evaluatedBrackets.ToString());
            }

            MatchCollection tokens = Regex.Matches(operation, @"{{[^ ]*}}(?=( |$))");

            Logger.Log($"Found {tokens.Count} tokens");
            //MatchCollection operators = Regex.Matches(requirement, @"[+\-*/]|[<=>]+");

            Dictionary<string, object> parsedTokens = new Dictionary<string, object>();
            foreach (Match match in tokens)
            {
                string token = match.Captures[0].Value;
                string strippedToken = token.TrimStart('{').TrimEnd('}').ToLower();
                parsedTokens[token] = ParseToken(strippedToken, state, model);
                Logger.Log($"Evaluated {token} as {parsedTokens[token]}");
            }
            List<string> partsAsStrings = operation.Split(' ').ToList();
            List<object> parts = new List<object>();
            foreach (string part in partsAsStrings)
            {
                if (parsedTokens.ContainsKey(part))
                    parts.Add(parsedTokens[part]);
                else
                    parts.Add(part);
            }
            return parts;
        }

        public static bool Compare(string requirement, States.FarmHouseState state, UpgradeModel model)
        {
            object result = Evaluate(ParseOperation(requirement, state, model));
            if (result is bool requirementsMet)
            {
                Logger.Log($"Requirements are {(requirementsMet ? "" : "not")} met");
                return requirementsMet;
            }
            return false;

            /*string parsed = requirement;
            foreach(Match match in tokens)
            {
                string token = match.Captures[0].Value;
                string strippedToken = token.TrimStart('{').TrimEnd('}').ToLower();
                parsed = parsed.Replace(token, availableProperties[strippedToken].ToString());
            }
            foreach(Match match in operators)
            {
                string op = match.Captures[0].Value;
                string parsedOperator = "";
                switch (op)
                {
                    case ">":
                        parsedOperator = "is greater than";
                        break;
                    case "<":
                        parsedOperator = "is less than";
                        break;
                    case ">=":
                        parsedOperator = "is greater than or equal to";
                        break;
                    case "<=":
                        parsedOperator = "is less than or equal to";
                        break;
                    case "=":
                        parsedOperator = "is equal to";
                        break;
                    case "+":
                        parsedOperator = "plus";
                        break;
                    case "-":
                        parsedOperator = "minus";
                        break;
                    case "/":
                        parsedOperator = "divided by";
                        break;
                    case "*":
                        parsedOperator = "times";
                        break;
                }
                parsed = parsed.Replace(op, parsedOperator);
            }
            Logger.Log("Parsed as\n" + parsed);
            return true;*/
        }
    }
}
