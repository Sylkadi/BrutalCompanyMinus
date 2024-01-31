using BrutalCompanyMinus.Minus;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using HarmonyLib;
using GameNetcodeStuff;

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

        public bool showCaseEvents = false;

        public float showCaseEventTime = 20.0f;
        public float curretShowCaseEventTime = 0.0f;

        public bool keyPressEnabledTyping = true, keyPressEnabledTerminal = true, keyPressEnabledSettings = true;

        public Keyboard keyboard;

        public void Start()
        {
            Instance = this;

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
            if (keyboard != null && Configuration.EnableUI.Value) keyboard.onTextInput += OnKeyboardInput;
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
            foreach (MEvent e in events) text += string.Format("-<color={0}>{1}</color><br>", e.ColorHex, e.Description);

            // Extra properties
            if (Configuration.ShowExtraProperties.Value)
            {
                float ScrapValueMultiplier = RoundManager.Instance.scrapValueMultiplier;
                if (Configuration.NormaliseScrapValueDisplay.Value) ScrapValueMultiplier *= 2.5f;
                text += string.Format("<br>Map:<br> Scrap:<br>  -Value: x{0}<br>  -Amount: x{1}<br><br> Factory:<br>  -Size: x{2}",
                    ScrapValueMultiplier.ToString("F2"), RoundManager.Instance.scrapAmountMultiplier.ToString("F2"), RoundManager.Instance.currentLevel.factorySizeMultiplier.ToString("F2"));
            }

            Net.Instance.textUI.Value = new FixedString4096Bytes(text);
            if (Configuration.PopUpUI.Value && Configuration.EnableUI.Value) Net.Instance.ShowCaseEventsClientRpc();
        }

        public static void ClearText() => Net.Instance.textUI.Value = new FixedString4096Bytes("<br>No Events...");

        public void OnKeyboardInput(char input)
        {
            if (input.ToString().ToUpper() == UI.Instance.key && keyPressEnabledTyping && keyPressEnabledTerminal && keyPressEnabledSettings)
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
