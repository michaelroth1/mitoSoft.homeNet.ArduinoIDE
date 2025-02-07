namespace mitoSoft.homeNet.ArduinoIDE.Extensions
{
    internal static class StringExtensions
    {
        public static string GetArduinoIPFormat(this string value)
        {
            return value.Replace(" ", "")
                        .Replace(".", ",")
                        .Replace(",", ", ");
        }
    }
}