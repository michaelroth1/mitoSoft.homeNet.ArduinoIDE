namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;

public static class ListExtensions
{
    public static IList<string> FindDuplicates(this List<string> list)
    {
        var seen = new HashSet<string>();  // Speichert eindeutige Werte
        var duplicates = new HashSet<string>(); // Speichert nur doppelte Werte

        foreach (var item in list)
        {
            if (!seen.Add(item)) // Falls num bereits in "seen" ist, ist es ein Duplikat
            {
                duplicates.Add(item);
            }
        }

        return new List<string>(duplicates); // Rückgabe als Liste
    }
}