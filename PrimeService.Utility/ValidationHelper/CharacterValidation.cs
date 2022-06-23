namespace PrimeService.Utility.ValidationHelper;

/// <summary>
/// Character Validations
/// </summary>
public sealed class CharacterValidation
{
    public static IEnumerable<string> Max50Characters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 50 < ch?.Length)
            yield return "Max 50 characters";
    }
    public static IEnumerable<string> Max21Characters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 21 < ch?.Length)
            yield return "Max 21 characters";
    }
    public static IEnumerable<string> Max12Characters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 12 < ch?.Length)
            yield return "Max 12 characters";
    }
    public static IEnumerable<string> Max10Characters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 10 < ch?.Length)
            yield return "Max 10 characters";
    }
    public static IEnumerable<string> Max15Characters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 15 < ch?.Length)
            yield return "Max 10 characters";
    }
}