namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

internal class ReflectionHelper
{
    public static Dictionary<string, string> GetAllProperties(object src)
    {
        var dict = new Dictionary<string, string>();

        src.GetType().GetProperties().ToList().ForEach(p =>
        {
            dict.Add(p.Name, p.GetValue(src)?.ToString() ?? "");
        });

        return dict;
    }

    public static object GetPropValue(object src, string propName)
    {
        return src.GetType().GetProperty(propName)?.GetValue(src, null) ?? "";
    }
}