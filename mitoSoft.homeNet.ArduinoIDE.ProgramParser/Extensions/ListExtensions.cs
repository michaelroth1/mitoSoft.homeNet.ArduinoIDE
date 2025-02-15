namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;

public static class ListExtensions
{
    public static IList<int> FindDuplicates(this List<int> list)
    {
        var seen = new HashSet<int>();  // Speichert eindeutige Werte
        var duplicates = new HashSet<int>(); // Speichert nur doppelte Werte

        foreach (var item in list)
        {
            if (item > 0 && !seen.Add(item)) // Falls num bereits in "seen" ist, ist es ein Duplikat
            {
                duplicates.Add(item);
            }
        }

        return new List<int>(duplicates); // Rückgabe als Liste
    }
}