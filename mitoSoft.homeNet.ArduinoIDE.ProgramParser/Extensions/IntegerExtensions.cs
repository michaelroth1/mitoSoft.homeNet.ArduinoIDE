namespace mitoSoft.homeNet.ArduinoIDE.ProgramParser.Extensions;

public static class IntegerExtensions
{
    public static List<int> FindDuplicates(this List<int> numbers)
    {
        var seen = new HashSet<int>();  // Speichert eindeutige Werte
        var duplicates = new HashSet<int>(); // Speichert nur doppelte Werte

        foreach (var num in numbers)
        {
            if (!seen.Add(num)) // Falls num bereits in "seen" ist, ist es ein Duplikat
                duplicates.Add(num);
        }

        return new List<int>(duplicates); // Rückgabe als Liste
    }
}