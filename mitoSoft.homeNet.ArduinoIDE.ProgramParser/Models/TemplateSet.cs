namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

internal class TemplateSet
{
    public string? Declaration { get; set; } 

    public string? Setup { get; set; }

    public string? MainTemplate { get; set; }

    public string? Loop { get; set; }

    public void ReplaceInAll(string oldValue, string newValue)
    {
        Declaration = Declaration?.Replace(oldValue, newValue);
        Setup = Setup?.Replace(oldValue, newValue);
        MainTemplate = MainTemplate?.Replace(oldValue, newValue);
        Loop = Loop?.Replace(oldValue, newValue);
    }

    public void EnableFeature(string prefix)
    {
        ReplaceInAll(prefix, "");
    }
}