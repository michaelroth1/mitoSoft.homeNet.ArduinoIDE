namespace mitoSoft.homeNet.ArduinoIDE.Contracts;

public interface ISaveable
{
    string? Save(string? filePath);

    string? SaveAs(string? filePath);
}