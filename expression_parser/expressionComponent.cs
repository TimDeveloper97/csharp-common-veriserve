public class ExpressionComponent
{
    public string? PreviousResultId1 { get; set; }
    public string? PreviousResultId2 { get; set; }

    public string? Variable1 { get; set; }
    public string? Variable2 { get; set; }
    public string Operator { get; set; }

    public ExpressionComponent(string variable1, string variable2, string operatorSymbol)
    {
        Variable1 = variable1;
        Variable2 = variable2;
        Operator = operatorSymbol;
    }

    public override string ToString()
    {
        return $"{{ variable1 = {Variable1}, variable2 = {Variable2}, operator = {Operator}, PreviousResultId1 = {PreviousResultId1}, PreviousResultId2 = {PreviousResultId2} }}";
    }
}

// Expression tree nodes
public abstract class ExprNode
{
}

// Leaf node storing an operand (e.g. A, B, etc.)
public class OperandNode : ExprNode
{
    public string Value { get; }

    public OperandNode(string value)
    {
        Value = value;
    }
}

// Binary operator node
public class OperatorNode : ExprNode
{
    public string Operator { get; }
    public ExprNode Left { get; }
    public ExprNode Right { get; }

    public OperatorNode(string op, ExprNode left, ExprNode right)
    {
        Operator = op;
        Left = left;
        Right = right;
    }
}

// Parser implementing recursive descent parsing with operator precedence.
public class Parser
{
    private readonly string _text;
    private int _pos;
    private char CurrentChar => _pos < _text.Length ? _text[_pos] : '\0';

    public Parser(string text)
    {
        _text = text;
        _pos = 0;
    }

    // Entry point to parse an expression string.
    public ExprNode ParseExpression()
    {
        var node = ParseAddSub();
        SkipWhiteSpace();
        if (_pos < _text.Length)
            throw new Exception("Unexpected characters at end of expression.");
        return node;
    }

    // Parse addition and subtraction
    private ExprNode ParseAddSub()
    {
        var left = ParseMulDiv();

        while (true)
        {
            SkipWhiteSpace();
            //if (CurrentChar == '+' || CurrentChar == '-')
            if (CurrentChar == '+')
            {
                char op = CurrentChar;
                _pos++;
                var right = ParseMulDiv();
                left = new OperatorNode(op.ToString(), left, right);
            }
            else
                break;
        }
        return left;
    }

    // Parse multiplication and division
    private ExprNode ParseMulDiv()
    {
        var left = ParseFactor();

        while (true)
        {
            SkipWhiteSpace();
            //if (CurrentChar == '*' || CurrentChar == '/')
            if (CurrentChar == '*')
            {
                char op = CurrentChar;
                _pos++;
                var right = ParseFactor();
                left = new OperatorNode(op.ToString(), left, right);
            }
            else
                break;
        }
        return left;
    }

    // Parse a factor: a variable (possibly multi-letter and containing spaces) or a parenthesized expression
    private ExprNode ParseFactor()
    {
        SkipWhiteSpace();
        if (CurrentChar == '(')
        {
            _pos++; // skip '('
            var node = ParseAddSub();
            SkipWhiteSpace();
            if (CurrentChar != ')')
                throw new Exception("Expected ')'");
            _pos++; // skip ')'
            return node;
        }
        else
        {
            return ParseOperand();
        }
    }

    // Updated ParseOperand: continues reading until an operator or parenthesis is encountered.
    // This allows multi-letter operands that may include spaces (like "E F").
    private ExprNode ParseOperand()
    {
        int start = _pos;
        // Read until we hit an operator or a parenthesis.
        while (_pos < _text.Length)
        {
            char ch = _text[_pos];
            //if (ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '(' || ch == ')')
            if (ch == '+' || ch == '*' || ch == '(' || ch == ')')
            {
                break;
            }
            _pos++;
        }
        string token = _text.Substring(start, _pos - start).Trim();
        if (string.IsNullOrEmpty(token))
            throw new Exception("Expected operand");
        return new OperandNode(token);
    }

    private void SkipWhiteSpace()
    {
        while (_pos < _text.Length && char.IsWhiteSpace(_text[_pos]))
            _pos++;
    }
}

// Evaluator traverses the expression tree in postorder and builds the list of components.
public class Evaluator
{
    private int _idCounter = 0;
    public Dictionary<string, ExpressionComponent> Components { get; } = new();

    // Evaluate returns the "result" label of the node.
    public (bool, string) Evaluate(ExprNode node)
    {
        if (node is OperandNode operand)
        {
            return (false, operand.Value);
        }

        else if (node is OperatorNode opNode)
        {
            var (isOperandNodeL, leftStr) = Evaluate(opNode.Left);
            var (isOperandNodeR, rightStr) = Evaluate(opNode.Right);

            string id = "Id" + _idCounter++;
            var ec = new ExpressionComponent(null, null, opNode.Operator);
            ec.Operator = opNode.Operator;
            if (!isOperandNodeL)
            {
                ec.Variable1 = leftStr;
            }
            else
            {
                ec.PreviousResultId1 = leftStr;
            }
            if (!isOperandNodeR)
            {
                ec.Variable2 = rightStr;
            }
            else
            {
                ec.PreviousResultId2 = rightStr;
            }

            Components.Add(id, ec);

            return (true, id);
        }

        throw new Exception("Unknown node type.");
    }
}

// Sample
var expressions = new List<string>
            {
                "(A + B) * (C + D)",
                "A + B * C",
                "A + (A + B) * (C + D / E F)",
            };

foreach (var expr in expressions)
{
    Console.WriteLine("Input: " + expr);
    Parser parser = new Parser(expr);
    ExprNode root = parser.ParseExpression();
    Evaluator evaluator = new Evaluator();
    evaluator.Evaluate(root);

    Console.WriteLine("Output:");
    int i = 0;
    foreach (var item in evaluator.Components)
    {
        Console.WriteLine($"[{i}]: {item.Value}");
        i++;
    }

    Console.WriteLine(new string('-', 40));
}