namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.ProgramParser.Extensions;

internal static class StringExtensions
{
    public static string TruncateText(this string inputText, string keyword)
    {
        // Suche nach dem Schlüsselwort "!include"
        int index = inputText.IndexOf(keyword);

        // Wenn das Schlüsselwort gefunden wurde, schneide den Text ab
        if (index != -1)
        {
            return inputText.Substring(0, index).Trim();
        }

        // Wenn das Schlüsselwort nicht gefunden wurde, gib den gesamten Text zurück
        return inputText;
    }

    public static string ReplaceWithWhiteSpaces(this string value, int whiteSpaces, string oldValue, string newValue)
    {
        var indenting = "";
        for (int i = 0; i < whiteSpaces; i++)
        {
            indenting += " ";
        }

        var @new = indenting + newValue.Replace("\n", $"\n{indenting}");

        return value.Replace(oldValue, @new);
    }

    public static string CleanEmptyRows(this string value)
    {
        string[] lines = value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var cleaned = new List<string>();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                cleaned.Add(string.Empty);
            }
            else
            {
                cleaned.Add(line);
            }
        }

        return string.Join('\n', cleaned.ToArray());
    }


    public static string RemoveDoubleEmptyRows(this string value)
    {
        string[] lines = value.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        var cleaned = new List<string>();
        bool lastWasEmpty = false;

        foreach (string line in lines)
        {
            bool isEmpty = string.IsNullOrWhiteSpace(line);

            lastWasEmpty = lastWasEmpty && isEmpty;

            if (!lastWasEmpty)
            {
                cleaned.Add(line);
            }

            lastWasEmpty = isEmpty;
        }

        return string.Join('\n', cleaned.ToArray());
    }
}