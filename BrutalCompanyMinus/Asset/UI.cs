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
using Discord;
using Unity.Netcode;

namespace BrutalCompanyMinus
{
    [HarmonyPatch]
    internal class UI : MonoBehaviour
    {
        public static UI Instance { get; private set; }
        public static GameObject eventUIObject { get; set; }

        public GameObject panelBackground;
        public TextMeshProUGUI panelText, letter;
        public Scrollbar panelScrollBar;

        public string key = "K";

        public KeyControl keyControl;

        public bool showCaseEvents = false;

        public float showCaseEventTime = 45.0f;
        public float curretShowCaseEventTime = 0.0f;

        public bool keyPressEnabledTyping = true, keyPressEnabledTerminal = true, keyPressEnabledSettings = true;

        public Keyboard keyboard;

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

                keyboard.onTextInput += OnKeyboardInput;
            }
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
                    panelBackground.SetActive(false);
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
                text += 
                    $"<br>Difficulty:" +
                    $"<br> -Day: {Manager.daysPassed}" +
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

        public static void ClearText() => Net.Instance.textUI.Value = new FixedString4096Bytes(" ");

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
                bool newState = !UI.Instance.panelBackground.activeSelf;

                if (!newState && UI.Instance.showCaseEvents)
                {
                    UI.Instance.showCaseEvents = false;
                    UI.Instance.panelScrollBar.value = 1.0f; // Reset to top
                }

                UI.Instance.panelBackground.SetActive(newState);
            }
        }

        public void UnsubscribeFromKeyboardEvent()
        {
            if (Configuration.EnableUI.Value) keyboard.onTextInput -= OnKeyboardInput;
        }


        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
        private static void OnShipLeave()
        {
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
