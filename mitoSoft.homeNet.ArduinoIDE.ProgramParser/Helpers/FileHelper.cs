using System.Reflection;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

public class FileHelper
{
    /// <summary>
    /// Read contents of an embedded resource file
    /// </summary>
    public static string ReadResourceFile(string filename)
    {
        var thisAssembly = Assembly.GetExecutingAssembly();

        using var stream = thisAssembly.GetManifestResourceStream(filename);

        using var reader = new StreamReader(stream!);

        return reader.ReadToEnd();
    }
}