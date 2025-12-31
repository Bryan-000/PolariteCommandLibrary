namespace Polarite.CommandLibrary;

using BepInEx;
using HarmonyLib;
using Polarite.CommandLibrary.Command;

[BepInPlugin("Bryan_-000-.PolariteCommandLibrary", "PolariteCommandLibrary", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public void Awake()
    {
        CompiledComment("Commands.Load(); is a method that registers a bunch of commands manually with CommandHandler.RegisterCommand()");
        Commands.Load();
        CompiledComment("While CommandHandler.ApplyAllCommands(); is like Harmony.PatchAll(); but for [PolariteCommand(\"uwu\", \"rawr\")]'s, which does CommandHandler.RegisterCommand() for you :3");
        CommandHandler.ApplyAllCommands();

        new Harmony("rraaaawrrrr :3").PatchAll();
    }

    static void CompiledComment(string comment) {}
}