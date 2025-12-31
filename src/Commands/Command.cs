namespace Polarite.CommandLibrary.Command;

using System;

public class Command(string name, string description, Action<string> cmd, bool showOnHelp = true)
{
    public string name = name;
    public string description = description;
    public bool showOnHelp = showOnHelp;
    public Action<string> cmd = cmd;
}