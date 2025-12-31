namespace Polarite.CommandLibrary.Command;

using Polarite.Multiplayer;
using System.Collections.Generic;
using System.Linq;

public class Commands
{
    static Dictionary<char, char[]> validLevels = new()
    {
        {'0', ['1', '2', '3', '4', '5', 's', 'e']},
        {'1', ['1', '2', '3', '4', 's', 'e']},
        {'2', ['1', '2', '3', '4', 's']},
        {'3', ['1', '2']},
        {'4', ['1', '2', '3', '4', 's']},
        {'5', ['1', '2', '3', '4', 's']},
        {'6', ['1', '2']},
        {'7', ['1', '2', '3', '4', 's']},
        {'p', ['1', '2']},
    };

    public static void Load()
    {
        CommandHandler.RegisterCommand("/Help", "Shows every single command and their description.", input =>
        {
            CommandHandler.ReceiveMessage("<size=24><color=#bbb>-- Commands --</color></size>");
            foreach (Command cmd in CommandHandler.commands.Values)
            {
                if (!cmd.showOnHelp) continue;
                CommandHandler.ReceiveMessage($"<color=#bbb>• {cmd.name}, {cmd.description}</color>");
            }
        }, false);

        CommandHandler.RegisterCommand("/Level", "Loads whichever ULTRAKILL level by name.", input =>
        {
            if (!NetworkManager.Instance.AmIHost())
            {
                CommandHandler.ReceiveMessage("<color=red>ONLY HOST CAN ENTER OTHER LEVELS.</color>");
                return;
            }

            if (input == "sandbox" || input == "uk_construct")
            {
                SceneHelper.LoadScene("uk_construct");
                CommandHandler.ReceiveMessage("Loading the Sandbox...");
                return;
            }

            if (input == "museum")
            {
                SceneHelper.LoadScene("CreditsMuseum2");
                CommandHandler.ReceiveMessage("Loading the Museum...");
                return;
            }

            if ((input[0] == '0' && input[2] == '0') || input == "tutorial")
            {
                SceneHelper.LoadScene("Tutorial");
                CommandHandler.ReceiveMessage("Loading the Tutorial...");
                return;
            }

            string levelname = input[(input.Length >= 6 ? 6 : 0)..];
            
            if (levelname.Length != 3 || !validLevels.TryGetValue(levelname[0], out char[] validPostfix) || !validPostfix.Contains(levelname[2]))
            {
                CommandHandler.ReceiveMessage("<color=red>Invalid Level Name.</color>");
                return;
            }

            string level = "Level " + (levelname[0] + "-" + levelname[2]).ToUpper();
            SceneHelper.LoadScene(level);
            CommandHandler.ReceiveMessage($"Loading {level}...");
        });

        #region test/showcase commands

        CommandHandler.RegisterCommand("/Echo", "Input test", input => 
            CommandHandler.ReceiveMessage("Echo: "+input), false);

        CommandHandler.RegisterToggleableCommand("/Toggle", "Toggle test", active => 
            CommandHandler.ReceiveMessage("Toggle: "+active.ToString()), false);

        #endregion
    }

    #region attribute test/showcase commands

    [PolariteCommand("/AttributeTest", "Attribute test", false)]
    public static void AttributeTest() =>
        CommandHandler.ReceiveMessage("Attribute Executed!");

    [PolariteCommand("/AttributeEcho", "Attribute echo test", false)]
    public static void AttributeEcho(string input) =>
        CommandHandler.ReceiveMessage("Attribute Echo: "+input);

    [PolariteCommand("/AttributeToggle", "Attribute toggle test (having a bool parameter makes it know its a toggleable command)", false)]
    public static void AttributeEcho(bool active) =>
        CommandHandler.ReceiveMessage("Attribute Toggle: "+active.ToString());

    #endregion
}