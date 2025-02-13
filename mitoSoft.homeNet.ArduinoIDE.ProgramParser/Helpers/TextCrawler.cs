using System.Text.RegularExpressions;

namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;

public class TextCrawler
{
    private readonly List<string> _lines;

    public TextCrawler(string text)
    {
        _lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public List<string> ParseHomeNetControllers()
    {
        int homeNetIndex = -1;
        // Finde den 'homeNet:'-Block (z.B. erste Zeile, die mit "homeNet:" beginnt)
        for (int i = 0; i < _lines.Count; i++)
        {
            if (_lines[i].TrimStart().StartsWith("homeNet:"))
            {
                homeNetIndex = i;
                break;
            }
        }
        if (homeNetIndex == -1)
        {
            return [];
        }

        int controllerIndex = -1;
        int controllerIndent = -1;
        // Suche innerhalb des homeNet-Blocks nach "controller:"
        for (int i = homeNetIndex + 1; i < _lines.Count; i++)
        {
            string trimmed = _lines[i].TrimStart();
            if (trimmed.StartsWith("controller:"))
            {
                controllerIndex = i;
                controllerIndent = _lines[i].Length - trimmed.Length;
                break;
            }
            // Falls wir einen neuen Abschnitt auf gleicher Ebene erreichen, beenden
            if (!string.IsNullOrWhiteSpace(_lines[i]) &&
                (_lines[i].Length - _lines[i].TrimStart().Length) <= (_lines[homeNetIndex].Length - _lines[homeNetIndex].TrimStart().Length))
            {
                break;
            }
        }
        if (controllerIndex == -1)
        {
            return [];
        }

        // Ab dem controller-Block werden nur Zeilen verarbeitet, die stärker eingerückt sind.
        List<string> controllerNames = [];
        // Ab der nächsten Zeile nach "controller:":
        for (int i = controllerIndex + 1; i < _lines.Count; i++)
        {
            string line = _lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            int currentIndent = line.Length - line.TrimStart().Length;
            // Beenden, wenn die Einrückung zurückgeht
            if (currentIndent <= controllerIndent && !line.TrimStart().StartsWith("-"))
                break;

            // Nur Zeilen, die mit "- name:" beginnen, sollen verarbeitet werden
            string trimmedLine = line.TrimStart();
            if (trimmedLine.StartsWith("- name:"))
            {
                // Mit Regex wird der in Anführungszeichen stehende Name extrahiert.
                Regex regex = new Regex(@"- name:\s*""([^""]+)""");
                Match match = regex.Match(trimmedLine);
                if (match.Success)
                {
                    controllerNames.Add(match.Groups[1].Value);
                }
            }
        }

        return controllerNames;
    }
}