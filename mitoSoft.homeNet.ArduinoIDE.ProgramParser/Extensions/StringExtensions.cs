using System.Net;
using System.Text.RegularExpressions;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;

internal static class StringExtensions
{
    public static bool IsValidMacAddress(this string value)
    {
        if (value.Replace(" ", "") == "0x00,0x00,0x00,0x00,0x00,0x00")
        {
            return false;
        }
        else
        {
            string pattern = @"^(0x[0-9A-Fa-f]{2}, ){5}0x[0-9A-Fa-f]{2}$";
            return Regex.IsMatch(value, pattern);
        }
    }

    public static bool IsValidIPAddress(this string value)
    {
        if (value.Replace(" ", "") == "0.0.0.0")
        {
            return false;
        }
        else
        {
            return IPAddress.TryParse(value, out _);
        }
    }

    public static string TruncateText(this string inputText, string keyword)
    {
        // Suche nach dem Schlüsselwort "!include"
        int index = inputText.IndexOf(keyword);

        // Wenn das Schlüsselwort gefunden wurde, schneide den Text ab
        if (index != -1)
        {
            return inputText[..index].Trim();
        }

        // Wenn das Schlüsselwort nicht gefunden wurde, gib den gesamten Text zurück
        return inputText;
    }

    public static string Replace(this string value, string oldValue, string newValue, int indenting)
    {
        var whiteSpaces = "";
        for (int i = 0; i < indenting; i++)
        {
            whiteSpaces += " ";
        }

        var @new = whiteSpaces + newValue.Replace("\n", $"\n{whiteSpaces}");

        return value.Replace(oldValue, @new);
    }

    public static string CleanEmptyRows(this string value)
    {
        string[] lines = value.Split(["\r\n", "\n"], StringSplitOptions.None);
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

    public static string CleanKeyWord(this string value, string keyWord)
    {
        string[] lines = value.Split(["\r\n", "\n"], StringSplitOptions.None);
        var cleaned = new List<string>();

        foreach (string line in lines)
        {
            if (line.Trim() != keyWord) //line is not the keyword
            {
                cleaned.Add(line);
            }
        }

        var full = string.Join('\n', cleaned.ToArray());
        //full = full.Replace(keyWord, "");
        return full;
    }


    public static string RemoveDoubleEmptyRows(this string value)
    {
        string[] lines = value.Split(["\r\n", "\n"], StringSplitOptions.None);
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

    public static string GetArduinoIPFormat(this string value)
    {
        return value.Replace(".", ",")
                    .GetArduinoSignaturFormat();
    }

    public static string GetArduinoSignaturFormat(this string value)
    {
        return value.Replace(" ", "")
                    .Replace(",", ", ");
    }

    public static bool DontStartsWith(this string text, string value)
    {
        return !text.StartsWith(value);
    }
}