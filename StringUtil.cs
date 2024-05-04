using System.Text;
using System.Text.RegularExpressions;

public class StringUtil
{
    public static string RemoverAcentos(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        input = input.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char c in input)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string RemoverCaracteresEspeciais(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(input, "[^a-zA-Z0-9 ]", string.Empty);
    }

    public static string FormatNomeAluno(string nome)
    {
        nome = RemoverAcentos(nome);
        nome = RemoverCaracteresEspeciais(nome);
        return nome;
    }
}
