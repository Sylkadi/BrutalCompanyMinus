using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(ItemDropship))]
    internal class ShipmentFees
    {
        private static int due = 0;

        [HarmonyPrefix]
        [HarmonyPatch("LandShipOnServer")]
        static void OnShipment()
        {
            if(due > 0 && Manager.currentTerminal.groupCredits > 0) // Try to pay off due first
            {
                HUDManager.Instance.AddTextToChatOnServer("<color=#FF0000>Due payment.</color>");

                int owed = due;
                if(Manager.currentTerminal.groupCredits - due < 0)
                {
                    due = due - Manager.currentTerminal.groupCredits;
                    owed = Manager.currentTerminal.groupCredits;
                }
                Manager.PayCredits(-owed);

                HUDManager.Instance.AddTextToChatOnServer(string.Format("<color=#FF0000>New Due balance:</color> <color=#800000>{0}</color>", due));
            }

            if(Manager.ShipmentFees)
            {
                MEvent shipmentEvent = MEvent.GetEvent(nameof(Events.ShipmentFees));
                float min = shipmentEvent.Getf(MEvent.ScaleType.MinCash);
                float max = shipmentEvent.Getf(MEvent.ScaleType.MaxCash);

                int feeIncured = 0;
                foreach(int i in Manager.currentTerminal.orderedItemsFromTerminal)
                {
                    Item item = Manager.currentTerminal.buyableItemsList[i];
                    feeIncured += (int)(item.creditsWorth * UnityEngine.Random.Range(min, max));
                }
                if(Manager.currentTerminal.groupCredits - feeIncured < 0) // Game dosen't like it when group credits go below 0
                {
                    due += feeIncured - Manager.currentTerminal.groupCredits;
                    feeIncured = Manager.currentTerminal.groupCredits;

                    HUDManager.Instance.AddTextToChatOnServer(string.Format("<color=#FF0000>Since you lack credits to pay the full fee, you will be due </color><color=#800000>{0}</color><color=#FF0000> on the next shipment.</color>", due));
                }
                Manager.PayCredits(-feeIncured);
            }
        }
    }
}
