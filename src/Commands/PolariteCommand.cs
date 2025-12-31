using System;

[AttributeUsage(AttributeTargets.Method)]
public class PolariteCommand : Attribute
{
    public string Name { get; }
    public string Description { get; }
    public bool ShowOnHelp {  get; }

    public PolariteCommand(string name, string description, bool showOnHelp = true)
    {
        Name = name;
        Description = description;
        ShowOnHelp = showOnHelp;
    }
}
