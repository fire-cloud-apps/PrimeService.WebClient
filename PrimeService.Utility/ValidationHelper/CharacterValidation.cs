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
    public static IEnumerable<string> Max12Characters(string ch)
    {
        if (!string.IsNullOrEmpty(ch) && 12 < ch?.Length)
            yield return "Max 12 characters";
    }
}