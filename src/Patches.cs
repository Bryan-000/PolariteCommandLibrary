namespace Polarite.CommandLibrary;

using HarmonyLib;
using Polarite.CommandLibrary.Command;
using Polarite.Multiplayer;

[HarmonyPatch]
public class Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatUI), "OnSubmitMessage")]
    public static bool SendMessagePatch(bool network, string realMsg)
    {
        if (network && CommandHandler.TryExecuteCommand(realMsg))
        {
            ChatUI.Instance.inputField.text = "";
            ChatUI.Instance.inputField.ActivateInputField();

            return false;
        }

        return true;
    }
}