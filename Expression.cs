
static List<string> GetVariables(string expression)
{
    List<string> variables = new List<string>();
    MatchCollection matches = Regex.Matches(expression, @"[\p{L}]+");

    foreach (Match match in matches)
    {
        string variable = match.Value;
        if (!variables.Contains(variable))
        {
            variables.Add(variable);
        }
    }

    return variables;
}


static List<string> GetOperatorsV2(string expression)
{
    List<string> operators = new List<string>();
    MatchCollection matches = Regex.Matches(expression, @"\*\*|\+|\-|\*|\/|\%|\!|\@|\#|\$|\&|\^");

    foreach (Match match in matches)
    {
        operators.Add(match.Value);
    }

    return operators.Distinct().ToList();
}
