using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using BepInEx;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    public class TerminalCommands
    {

        private static string response = "";
        private static bool _clearPreviousText = true;

        public static List<MCommand> mCommands = new List<MCommand>()
        {
            new MCommand()
            {
                command = "MHELP",
                shortinfo = "Provides help information for commands.",
                info = "MHELP [Command]\n   Command - displays help information on that command.",
                execute = new Action<string[]>((arguments) =>
                {
                    if(arguments.Length == 0)
                    {
                        string text = "To display extra information about a particular command use MHELP [Command]\n\n";
                        
                        foreach(MCommand mCommand in mCommands)
                        {
                            text += $"{mCommand.command.PadRight(10)}{mCommand.shortinfo}\n\n";
                        }
                        Respond(text);
                    } else
                    {
                        bool found = false;
                        foreach(MCommand mCommand in mCommands)
                        {
                            if(mCommand.command != arguments[0].ToUpper()) continue;
                            Respond($"{mCommand.shortinfo}\n\n{mCommand.info}");
                            found = true;
                            break;
                        }
                        if(!found)
                        {
                            Respond($"{arguments[0]} is not a command.");
                        }
                    }
                })
            },
            new MCommand()
            {
                command = "MEVENT",
                shortinfo = "Forces a mEvent to occur for next day.",
                info = "MEVENT [mEvent]\n    mEvent - the name of said event.\n\nMEVENT [mEvent 1] [mEvent 2] ... [mEvent n]\n    this can take multiple events.",
                execute = new Action<string[]>((arguments) =>
                {
                    if(arguments.Length == 0)
                    {
                        Respond("MEVENT command is missing argument(s).");
                        return;
                    }

                    string text = "";
                    foreach(string arg in arguments)
                    {
                        bool found = false;
                        foreach(MEvent e in EventManager.events)
                        {
                            if(arg.ToUpper() == e.Name().ToUpper())
                            {
                                found = true;
                                text += $"{e.Name()} will now be forced to occur.\n";
                                EventManager.forcedEvents.Add(e);
                                break;
                            }
                        }
                        if(!found)
                        {
                            text += $"Failed to find {arg} in events.\n";
                        }
                    }
                    text += $"\nCurrent forced events = [{Helper.StringsToList(EventManager.forcedEvents.Select(n => n.Name()).ToList(), ",")}]";

                    Respond(text);
                })
            },
            new MCommand()
            {
                command = "MCLEAR",
                shortinfo = "Clears the forced event list.",
                info = "MCLEAR\n   Clears the forced event list from MEVENT.",
                execute = new Action<string[]>((arguments) =>
                {
                    EventManager.forcedEvents.Clear();
                    Respond($"Cleared forced event list\n\nCurrent forced events = [{Helper.StringsToList(EventManager.forcedEvents.Select(n => n.Name()).ToList(), ",")}]");
                })
            },           
            new MCommand()
            {
                command = "MEVENTS",
                shortinfo = "Displays all events.",
                info = "MEVENTS\n   This will display all events, excluding disabled ones and modded ones where said mod isn't present.\n\nMEVENTS [Name]\n   Displays properties of said event.",
                execute = new Action<string[]>((arguments) =>
                {
                    if(arguments.Length == 0)
                    {
                        string text = "To display extra information about a particular event, use\nMEVENTS [Name]\n\n";
                        text += "[Very Good] events:\n";
                        text += Helper.StringsToList(EventManager.allVeryGood.Select(n => n.Name()).ToList(), ", ");
                        text += "\n\n[Good] events:\n";
                        text += Helper.StringsToList(EventManager.allGood.Select(n => n.Name()).ToList(), ", ");
                        text += "\n\n[Neutral] events:\n";
                        text += Helper.StringsToList(EventManager.allNeutral.Select(n => n.Name()).ToList(), ", ");
                        text += "\n\n[Bad] events:\n";
                        text += Helper.StringsToList(EventManager.allBad.Select(n => n.Name()).ToList(), ", ");
                        text += "\n\n[VeryBad] events:\n";
                        text += Helper.StringsToList(EventManager.allVeryBad.Select(n => n.Name()).ToList(), ", ");
                        text += "\n\n[Remove] events:\n";
                        text += Helper.StringsToList(EventManager.allRemove.Select(n => n.Name()).ToList(), ", ");

                        Respond(text);
                    } else
                    {
                        bool found = false;
                        foreach(MEvent mEvent in EventManager.events)
                        {
                            if(mEvent.Name().ToUpper() == arguments[0].ToUpper())
                            {
                                found = true;

                                string text = $"[{mEvent.Name()}]:\n\n";
                                text += $"[ColorHex]: {mEvent.ColorHex}\n[Weight]: {mEvent.Weight}\n[Type]: {mEvent.Type}\n\n";
                                text += $"[Descriptions]: {Helper.StringsToList(mEvent.Descriptions, "|")}\n\n";
                                text += $"[EventsToRemove]: {Helper.StringsToList(mEvent.EventsToRemove, ", ")}\n\n";
                                text += $"[EventsToSpawnWith]: {Helper.StringsToList(mEvent.EventsToSpawnWith, ", ")}\n\n";
                                text += $"[ScaleList]: \n";
                                foreach(KeyValuePair<MEvent.ScaleType, MEvent.Scale> scaleType in mEvent.ScaleList)
                                {
                                    text += $"{ScaleTypePadded(scaleType.Key)} {Helper.GetStringFromScale(scaleType.Value)}\n";
                                }
                                text += "\n[MonsterEvents]:\n";
                                foreach(MEvent.MonsterEvent monsterEvent in mEvent.monsterEvents)
                                {
                                    text += $"\n{monsterEvent.enemy.name}\n";
                                    text += $"{ScaleTypePadded(MEvent.ScaleType.InsideEnemyRarity)}{Helper.GetStringFromScale(monsterEvent.insideSpawnRarity)}\n";
                                    text += $"{ScaleTypePadded(MEvent.ScaleType.OutsideEnemyRarity)}{Helper.GetStringFromScale(monsterEvent.outsideSpawnRarity)}\n";
                                    text += $"{ScaleTypePadded(MEvent.ScaleType.MinInsideEnemy)}{Helper.GetStringFromScale(monsterEvent.minInside)}\n";
                                    text += $"{ScaleTypePadded(MEvent.ScaleType.MaxInsideEnemy)}{Helper.GetStringFromScale(monsterEvent.maxInside)}\n";
                                    text += $"{ScaleTypePadded(MEvent.ScaleType.MinOutsideEnemy)}{Helper.GetStringFromScale(monsterEvent.minOutside)}\n";
                                    text += $"{ScaleTypePadded(MEvent.ScaleType.MaxOutsideEnemy)}{Helper.GetStringFromScale(monsterEvent.maxOutside)}\n";
                                }
                                text += "\n[TransmutationEvent]:\n";
                                foreach(SpawnableItemWithRarity item in mEvent.scrapTransmutationEvent.items)
                                {
                                    text += $"[Item]: {item.spawnableItem.name.PadRight(18)} [Rarity]: {item.rarity}\n";
                                }
                                Respond(text);
                                break;
                            }
                        }
                        if(!found)
                        {
                            Respond($"Failed to find event {arguments[0]} in events list.");
                        }                             
                    }
                })
            },
            new MCommand()
            {
                command = "MPAY",
                shortinfo = "Adds or subtracts credits.",
                info = "MPAY [Amount]\n   Amount is the quantity of credits added or subtracted.",
                execute = new Action<string[]>((arguments) =>
                {
                    if(arguments.Length == 0)
                    {
                        Respond("MPAY command is missing argument(s).");
                        return;
                    }

                    if(int.TryParse(arguments[0], out int amount))
                    {
                        Manager.PayCredits(amount);
                        Respond($"Added {arguments[0]} credits.");
                    } else
                    {
                        Respond($"{arguments[0]} is not an acceptable value, must be int32.");
                    }
                })
            },
            new MCommand()
            {
                command = "MENEMIES",
                shortinfo = "Displays all enemies.",
                info = "MENEMIES\n   Will display the names of every enemy grabbed by this mod.",
                execute = new Action<string[]>((arguments) =>
                {
                    int i = 0;
                    string text = "";
                    foreach(string enemyName in Assets.EnemyList.Keys)
                    {
                        i++;
                        if(i % 2 == 0)
                        {
                            text += $" {enemyName}\n";
                        } else
                        {
                            text += enemyName.PadRight(23);
                        }
                    }
                    Respond(text);
                })
            },
            new MCommand()
            {
                command = "MITEMS",
                shortinfo = "Displays all items.",
                info = "MITEMS\n   Will display the names of every item grabbed by this mod.",
                execute = new Action<string[]>((arguments) =>
                {
                    int i = 0;
                    string text = "";
                    foreach(string itemName in Assets.ItemList.Keys)
                    {
                        i++;
                        if(i % 2 == 0)
                        {
                            text += $" {itemName}\n";
                        } else
                        {
                            text += itemName.PadRight(23);
                        }
                    }
                    Respond(text);
                })
            }
        };

        private static string ScaleTypePadded(MEvent.ScaleType type) => $"[{type}]:".PadRight(23);

        public static void Respond(string text, bool clearPreviousText = true)
        {
            response = text;
            _clearPreviousText = clearPreviousText;
        }

        public class MCommand
        {
            public string command;

            public string shortinfo;
            public string info;

            public Action<string[]> execute;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Terminal), "ParsePlayerSentence")]
        private static void OnParsePlayerSentence(ref Terminal __instance, ref TerminalNode __result)
        {
            if (!NetworkManager.Singleton.IsServer) return;

            string[] text = __instance.screenText.text[^__instance.textAdded..].Split(" ");
            if (text.Length == 0) return;

            string command = text[0];
            string[] arguments = text[1..];

            foreach(MCommand mCommand in mCommands)
            {
                if (mCommand.command.ToLower() != command) continue;

                if (!Configuration.enableTerminalCommands.Value)
                {
                    Respond("You need to have [Enable terminal Commands?] set to true located inside Difficulty_Settings.cfg to use " + command);
                    break;
                }

                mCommand.execute(arguments);
            }

            if (response.IsNullOrWhiteSpace()) return;

            __result = ScriptableObject.CreateInstance<TerminalNode>();
            __result.displayText = response + "\n\n";
            __result.clearPreviousText = _clearPreviousText;

            response = "";
        }
    }
}
