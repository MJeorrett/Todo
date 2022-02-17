using System.Text.RegularExpressions;

namespace Todo.Domain.Extensions;

public static class EnumExtensions
{
    public static string GetUserFriendlyName<T>(this T target)
        where T : Enum
    {
        var words = Regex.Split(target.ToString(), @"(?<!^)(?=[A-Z])");
        var result = string.Join(" ", words).ToLower();

        if (result.Length == 1) return result.ToUpper();

        return char.ToUpper(result[0]) + result[1..];
    }
}
