namespace Polarite.CommandLibrary.Command;

using HarmonyLib;
using plog;
using Polarite.Multiplayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

public class CommandHandler
{
    public static PrefsManager Prefs => PrefsManager.Instance;

    public static Dictionary<string, Command> commands = [];

    public static Logger Log = new("Polarite CommandHandler");

    public static void ReceiveMessage(string message, string spokenMessage = null, bool tts = false) =>
        ChatUI.Instance.OnSubmitMessage(message, false, spokenMessage ?? message, tts: tts);

    public static bool TryExecuteCommand(string msg)
    {
        int space = msg.IndexOf(' ');
        string command = msg.ToLower()[..(space == -1 ? msg.Length : space)];
        if (!commands.TryGetValue(command, out Command cmd)) return false;

        string input = space == -1 ? string.Empty : msg.ToLower()[(space+1)..];

        try {
            cmd.cmd(input);
        }
        catch (Exception ex) {
            Log.Exception($"EXCEPTION WHILE EXCUTING COMMAND {input}... EXCEPTION: {ex.Message}");
        }

        return true;
    }

    public static void RegisterCommand(string name, string description, Action<string> cmd, bool showOnHelp = true) =>
        commands.Add(name.ToLower(), new(name, description, cmd, showOnHelp));

    public static void RegisterToggleableCommand(string name, string description, Action<bool> togglecmd, bool showOnHelp = true) =>
        commands.Add(name.ToLower(), new(name, description, input => HandleToggleableCommand(name, input, togglecmd), showOnHelp));

    public static void HandleToggleableCommand(string name, string input, Action<bool> togglecmd)
    {
        string key = "Polarite.CommandLibrary."+name;

        bool pref = Prefs.GetBool(key, false);
        bool active = input.Length == 0 ? !pref : input == "on" || (input != "off" && !pref);

        Prefs.SetBool(key, active);
        togglecmd(active);
    }

    public static void ApplyAllCommands()
    {
        Assembly assembly = new StackTrace().GetFrame(1).GetMethod().ReflectedType.Assembly; // yes this is stolen from harmony
        AccessTools.GetTypesFromAssembly(assembly).Do(type =>
        {
            IEnumerable<MethodInfo> methods = type.GetMethods(AccessTools.all).Where(m => m.GetCustomAttribute<PolariteCommand>() != null);

            foreach (MethodInfo method in methods)
            {
                PolariteCommand pCmd = method.GetCustomAttribute<PolariteCommand>();
                ParameterInfo[] Params = method.GetParameters();

                if (Params.Length == 0) RegisterCommand(pCmd.Name, pCmd.Description, _ => method.Invoke(null, []), pCmd.ShowOnHelp);

                else if (Params[0].ParameterType == typeof(string)) RegisterCommand(pCmd.Name, pCmd.Description, input => method.Invoke(null, [input]), pCmd.ShowOnHelp);

                else if (Params[0].ParameterType == typeof(bool)) RegisterToggleableCommand(pCmd.Name, pCmd.Description, active => method.Invoke(null, [active]), pCmd.ShowOnHelp);
            }
        });
    }
}