using System.Text;

namespace Doroti.Intl;

internal static class IntlDateInputPattern
{
    internal static string Canonicalize(string pattern)
    {
        var result = new StringBuilder(pattern.Length + 6);
        var fields = new HashSet<char>();
        char quote = '\0';
        for (var i = 0; i < pattern.Length; i++)
        {
            var token = pattern[i];
            if (token == '\\' && i + 1 < pattern.Length)
            {
                result.Append(token).Append(pattern[++i]);
            }
            else if (quote != '\0')
            {
                result.Append(token);
                if (quote == token) quote = '\0';
            }
            else if (token is '\'' or '"')
            {
                quote = token;
                result.Append(token);
            }
            else if (token is 'd' or 'M' or 'y')
            {
                var start = i;
                while (i + 1 < pattern.Length && pattern[i + 1] == token) i++;
                if (!fields.Add(token) || (token == 'd' && i - start >= 2)) return "yyyy-MM-dd";
                result.Append(token, token == 'y' ? 4 : 2);
            }
            else if (token is 'g' or '%')
            {
                return "yyyy-MM-dd";
            }
            else
            {
                result.Append(token);
            }
        }
        return quote == '\0' && fields.Count == 3 ? result.ToString() : "yyyy-MM-dd";
    }
}
