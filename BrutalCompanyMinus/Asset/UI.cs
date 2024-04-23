using BrutalCompanyMinus.Minus;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using HarmonyLib;
using GameNetcodeStuff;
using System.Globalization;
using UnityEngine.InputSystem.Controls;
using Unity.Netcode;
using static System.Net.Mime.MediaTypeNames;
using System.Collections;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    internal class UI : MonoBehaviour
    {
        public static UI Instance { get; private set; }
        public static GameObject eventUIObject { get; set; }

        public GameObject panelBackground, upArrowPanel, downArrowPanel;
        public TextMeshProUGUI panelText, letter, upArrow, downArrow;
        public Scrollbar panelScrollBar;

        public string key = "K";

        public KeyControl keyControl, upKeyControl, downKeyControl;

        public bool showCaseEvents = false;

        public float showCaseEventTime = 45.0f;
        public float curretShowCaseEventTime = 0.0f;

        public bool keyPressEnabledTyping = true, keyPressEnabledTerminal = true, keyPressEnabledSettings = true;

        public Keyboard keyboard;

        public static bool canClearText = true;

        public void Start()
        {
            Instance = this;

            showCaseEventTime = Configuration.UITime.Value;
            Net.Instance.textUI.OnValueChanged += (previous, current) => panelText.text = current.ToString(); // For Text update

            Component[] components = UI.eventUIObject.GetComponentsInChildren<Component>(true);
            foreach (Component comp in components)
            {
                try
                {
                    switch (comp.name)
                    {
                        case "EventPanel":
                            if (panelBackground == null) panelBackground = comp.gameObject;
                            break;
                        case "EventText":
                            if (panelText == null) panelText = comp.GetComponent<TextMeshProUGUI>();
                            break;
                        case "Letter":
                            if (letter == null)
                            {
                                letter = comp.GetComponent<TextMeshProUGUI>();
                                key = Configuration.UIKey.Value.ToUpper();
                                letter.text = key;
                            }
                            break;
                        case "LetterPanel":
                            if (!Configuration.ShowUILetterBox.Value || !Configuration.EnableUI.Value) comp.gameObject.SetActive(false);
                            break;
                        case "Scrollbar":
                            if (panelScrollBar == null) panelScrollBar = comp.GetComponent<Scrollbar>();
                            break;
                        case "UpArrowPannel":
                            if(upArrowPanel == null) upArrowPanel = comp.gameObject;
                            break;
                        case "DownArrowPanel":
                            if(downArrowPanel == null) downArrowPanel = comp.gameObject;
                            break;
                        case "UpArrow":
                            if (upArrow == null) upArrow = comp.GetComponent<TextMeshProUGUI>();
                            break;
                        case "DownArrow":
                            if (downArrow == null) downArrow = comp.GetComponent<TextMeshProUGUI>();
                            break;
                    }
                } catch
                {
                    Log.LogError("Failed to capture EventUI component/s.");
                }
            }

            keyboard = Keyboard.current;
            if (keyboard != null && Configuration.EnableUI.Value)
            {
                keyControl = keyboard.FindKeyOnCurrentKeyboardLayout(key);

                downKeyControl = keyboard.downArrowKey;
                upKeyControl = keyboard.upArrowKey;

                keyboard.onTextInput += OnKeyboardInput;
            }

            panelText.text = Net.Instance.GetSyncedTextServerRpc().ToString();
        }

        void Update()
        {
            if (showCaseEvents)
            {
                curretShowCaseEventTime -= Time.deltaTime; // Decrement timer
                if (curretShowCaseEventTime <= showCaseEventTime * 0.6f) panelScrollBar.value -= (1 / (showCaseEventTime * 0.8f)) * Time.deltaTime * 2.0f;
                // End showcase events
                if (curretShowCaseEventTime < 0.0f)
                {
                    panelScrollBar.value = 1.0f; // Reset to top
                    showCaseEvents = false;
                    TogglePanel(false);
                }
            }

            if (panelBackground.activeSelf && downKeyControl != null && upKeyControl != null)
            {
                if(downKeyControl.isPressed)
                {
                    showCaseEvents = false;
                    downArrow.color = new Color(0.0f, 1.0f, 0.0f);
                    panelScrollBar.value -= Time.deltaTime * Configuration.scrollSpeed.Value;
                } else
                {
                    downArrow.color = new Color(0.0f, 0.6f, 0.0f);
                }

                if (upKeyControl.isPressed)
                {
                    showCaseEvents = false;
                    upArrow.color = new Color(0.0f, 1.0f, 0.0f);
                    panelScrollBar.value += Time.deltaTime * Configuration.scrollSpeed.Value;
                } else
                {
                    upArrow.color = new Color(0.0f, 0.6f, 0.0f);
                }
            }
        }

        public static void SpawnObject()
        {
            if (eventUIObject != null) return;

            eventUIObject = (GameObject)Assets.bundle.LoadAsset("EventGUI");
            eventUIObject.AddComponent<UI>();

            eventUIObject = Instantiate(eventUIObject, Vector3.zero, Quaternion.identity);
        }

        public static void GenerateText(List<MEvent> events)
        {
            // Generate Text
            string text = "<br>Events:<br>";
            foreach (string eventDescription in EventManager.currentEventDescriptions)
            {
                text += $"-{eventDescription}<br>";
            }

            // Extra properties
            if (Configuration.ShowExtraProperties.Value)
            {
                float ScrapValueMultiplier = RoundManager.Instance.scrapValueMultiplier * Manager.scrapValueMultiplier;
                if (Configuration.NormaliseScrapValueDisplay.Value) ScrapValueMultiplier *= 2.5f;

                text += GetDifficultyText();

                text += "<br><br>Other:";
                
                text += 
                    $"<br> -Scrap Value: x{ScrapValueMultiplier:F2}" +
                    $"<br> -Scrap Amount: x{(RoundManager.Instance.scrapAmountMultiplier * Manager.scrapAmountMultiplier):F2}" +
                    $"<br> -Factory Size: x{RoundManager.Instance.currentLevel.factorySizeMultiplier:F2}" +
                    $"<br> -Spawn Chance: x{Manager.spawnChanceMultiplier:F2}" +
                    $"<br> -Spawn Cap: x{Manager.spawncapMultipler:F2}" +
                    $"<br> -Bonus enemy hp: {plusMinus(Manager.bonusEnemyHp)}";
            }

            Net.Instance.textUI.Value = new FixedString4096Bytes(text);
            if (Configuration.PopUpUI.Value && Configuration.EnableUI.Value) Net.Instance.ShowCaseEventsClientRpc();
        }

        private static string plusMinus(float value)
        {
            string s = value.ToString();
            if (value >= 0) s = "+" + s;
            return s;
        }

        private static string plusMinusExclusive(float value) => (value < 0) ? "" : "+";

        [ServerRpc(RequireOwnership = false)]
        private static void ClearTextServerRpc()
        {
            try
            {
                ClearText();
            } catch
            {

            }
        }

        public static void ClearText()
        {
            if(Configuration.DisplayExtraPropertiesAfterShipLeaves.Value)
            {
                string text = "";

                Manager.ComputeDifficultyValues();
                if (!Configuration.useCustomWeights.Value)
                {
                    EventManager.UpdateAllEventWeights();
                    text += 
                        $"<br>EventType Chances:" +
                        $"<br> -<color=#800000>VeryBad</color>:  {Helper.GetPercentage(EventManager.eventTypeRarities[0])}" +
                        $"<br> -<color=#FF0000>Bad</color>:      {Helper.GetPercentage(EventManager.eventTypeRarities[1])}" +
                        $"<br> -<color=#FFFFFF>Neutral</color>:  {Helper.GetPercentage(EventManager.eventTypeRarities[2])}" +
                        $"<br> -<color=#008000>Good</color>:     {Helper.GetPercentage(EventManager.eventTypeRarities[3])}" +
                        $"<br> -<color=#00FF00>VeryGood</color>: {Helper.GetPercentage(EventManager.eventTypeRarities[4])}" +
                        $"<br> -<color=#008000>Remove</color>:   {Helper.GetPercentage(EventManager.eventTypeRarities[5])}<br>";
                }

                text += GetDifficultyText();

                Net.Instance.textUI.Value = new FixedString4096Bytes(text);
            } else
            {
                Net.Instance.textUI.Value = new FixedString4096Bytes(" ");
            }
        }

        private static string GetDifficultyText()
        {
            string text =
                $"<br>Difficulty: <color=#{Helper.GetDifficultyColorHex(Manager.difficulty, Configuration.difficultyMaxCap.Value)}>{Helper.GetDifficultyText(Manager.difficulty)}</color>" +
                $"<br> -Difficulty: <color=#{Helper.GetDifficultyColorHex(Manager.difficulty, Configuration.difficultyMaxCap.Value)}>{Manager.difficulty:F1}</color>";

            if (Configuration.scaleByDaysPassed.Value) text += $"<br> -Day: <color=#{Helper.GetDifficultyColorHex(Manager.daysDifficulty, Configuration.daysPassedDifficultyCap.Value)}>{plusMinusExclusive(Manager.daysDifficulty)}{Manager.daysDifficulty:F1}</color>";
            if (Configuration.scaleByScrapInShip.Value) text += $"<br> -Ship Scrap: <color=#{Helper.GetDifficultyColorHex(Manager.scrapInShipDifficulty, Configuration.scrapInShipDifficultyCap.Value)}>{plusMinusExclusive(Manager.scrapInShipDifficulty)}{Manager.scrapInShipDifficulty:F1}</color>";
            if (Configuration.scaleByMoonGrade.Value) text += $"<br> -Moon risk: <color=#{Helper.GetDifficultyColorHex(Manager.moonGradeDifficulty, Configuration.gradeAdditives["S+++"])}>{plusMinusExclusive(Manager.moonGradeDifficulty)}{Manager.moonGradeDifficulty:F1}</color>";

            return text;
        }

        public void OnKeyboardInput(char input)
        {
            bool pressed = false;
            if(keyControl != null)
            {
                pressed = keyControl.isPressed;
            } else
            {
                pressed = (input.ToString().ToUpper() == key.ToUpper());
            }
            if (pressed && keyPressEnabledTyping && keyPressEnabledTerminal && keyPressEnabledSettings)
            {
                bool newState = !panelBackground.activeSelf;

                if (!newState && showCaseEvents)
                {
                    showCaseEvents = false;
                    panelScrollBar.value = 1.0f; // Reset to top
                }

                TogglePanel(newState);
            }
        }

        public void UnsubscribeFromKeyboardEvent()
        {
            if (Configuration.EnableUI.Value) keyboard.onTextInput -= OnKeyboardInput;
        }

        public void TogglePanel(bool state)
        {
            if(Configuration.DisplayExtraPropertiesAfterShipLeaves.Value && Net.Instance.textUI.Value.IsEmpty) ClearTextServerRpc();

            panelBackground.SetActive(state);
            upArrowPanel.SetActive(state);
            downArrowPanel.SetActive(state);
            letter.color = new Color(0, state ? 1.0f : 0.6f, 0);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.SetDiscordStatusDetails))]
        private static void OnChangeLevel(ref StartOfRound __instance)
        {
            if (!NetworkManager.Singleton.IsServer || !canClearText) return;
            try
            {
                ClearText();
            } catch
            {
                __instance.StartCoroutine(ClearAfterDelay());
            }
        }

        private static IEnumerator ClearAfterDelay()
        {
            yield return new WaitForSeconds(0.2f);
            try
            {
                ClearText();
            } catch
            {

            }
        }

        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
        private static void OnShipLeave()
        {
            canClearText = true;
            if (!RoundManager.Instance.IsHost) return;

            if (!Configuration.DisplayUIAfterShipLeaves.Value)
            {
                ClearText();
                return;
            }

            if(Configuration.EnableUI.Value) GenerateText(EventManager.currentEvents);

            if (Configuration.showEventsInChat.Value)
            {
                HUDManager.Instance.AddTextToChatOnServer("<color=#FFFFFF>Events:</color>");
                foreach (string eventDescription in EventManager.currentEventDescriptions)
                {
                    HUDManager.Instance.AddTextToChatOnServer(eventDescription);
                }
            }
        }

        // Disable inputs when in terminal, in settings or is typing.

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Terminal), "Update")]
        private static void OnTerminalUpdate(ref bool ___terminalInUse)
        {
            try
            {
                Instance.keyPressEnabledTerminal = !___terminalInUse;
            } catch { }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        public static void OnPlayerControllerBUpdate(ref QuickMenuManager ___quickMenuManager)
        {
            try
            {
                Instance.keyPressEnabledSettings = !___quickMenuManager.isMenuOpen;
            } catch { }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HUDManager), "Update")]
        public static void OnUpdate(ref PlayerControllerB ___localPlayer)
        {
            try
            {
                Instance.keyPressEnabledTyping = !___localPlayer.isTypingChat;
            } catch { }
        }
    }
}
