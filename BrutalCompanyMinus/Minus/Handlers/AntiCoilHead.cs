using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace BrutalCompanyMinus.Minus.Handlers
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(SpringManAI))]
    internal class AntiCoilHead
    {
        [HarmonyTranspiler]
        [HarmonyPatch("Update")]
        static IEnumerable<CodeInstruction> OnUpdateIL(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = new List<CodeInstruction>(instructions);

            int firstindex = -1;
            CodeInstruction firstFlag = Transpilers.EmitDelegate<Func<bool>>(() => Net.Instance.isAntiCoilHead.Value);

            int secondindex = -1;
            CodeInstruction secondFlag = Transpilers.EmitDelegate<Func<bool>>(() => !Net.Instance.isAntiCoilHead.Value);


            for (int i = 0; i < code.Count; i++)
            {
                // Check backwards for first index (did this to fix incompatability with late game upgrades peepers)
                if (code[i].opcode == OpCodes.Ldc_I4_0 && code[i - 1].opcode == OpCodes.Callvirt && code[i - 2].opcode == OpCodes.Ldc_R4 && code[i - 3].opcode == OpCodes.Ldfld && code[i - 4].opcode == OpCodes.Ldarg_0)
                {
                    firstindex = i;
                    firstFlag.labels = code[i].labels;
                }
                if (code[i].opcode == OpCodes.Ldc_I4_1 && code[i + 1].opcode == OpCodes.Stloc_1 && code[i + 2].opcode == OpCodes.Ldloc_2 && code[i + 3].opcode == OpCodes.Ldc_I4_1)
                {
                    secondindex = i;
                    secondFlag.labels = code[i].labels;
                }
            }

            if (firstindex > -1)
            {
                code.RemoveAt(firstindex);
                code.Insert(firstindex, firstFlag);
            }

            if (secondindex > -1)
            {
                code.RemoveAt(secondindex);
                code.Insert(secondindex, secondFlag);
            }

            return code.AsEnumerable();
        }

    }
}
