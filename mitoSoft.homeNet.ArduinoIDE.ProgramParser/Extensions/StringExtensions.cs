using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}