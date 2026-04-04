namespace mitoSoft.homeNet.ArduinoIDE;

public interface ISaveable
{
    string? Save(string? filePath);

    string? SaveAs(string? filePath);
}