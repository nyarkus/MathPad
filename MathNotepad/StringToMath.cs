using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathNotepad;

public class StringToMath
{
#nullable disable
    public static decimal ParseExpression(string input, Dictionary<string, decimal> variables)
    {
        Stack<decimal> stack = new Stack<decimal>();
        Stack<Operator> operatorStack = new Stack<Operator>();
        int i = 0;
        bool OperatorBefore = false;    
        while (i < input.Length)
        {
            if (input[i] == '(')
            {
                int endIndex = FindMatchingParenthesis(input, i);
                decimal subResult = ParseExpression(input.Substring(i + 1, endIndex - i - 1), variables);
                stack.Push(subResult);
                i = endIndex;
                
            }
            else if (char.IsDigit(input[i]) || (input[i] == '-' && (OperatorBefore || stack.Count == 0)))
            {
                int endIndex = i;
                if (input[i] == '-')
                    endIndex++;
                while (endIndex < input.Length && (char.IsDigit(input[endIndex]) || input[endIndex] == '.' || input[endIndex] == ','))
                {
                    endIndex++;
                }
                string s = input.Substring(i, endIndex - i);
                decimal value = decimal.Parse(s.Replace(",", "."), NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
                stack.Push(value);
                i = endIndex;
                OperatorBefore = false;
            }
            else if (char.IsLetter(input[i]))
            {
                int endIndex = i;
                while (endIndex < input.Length && char.IsLetterOrDigit(input[endIndex]))
                {
                    endIndex++;
                }
                string varName = input.Substring(i, endIndex - i);
                if (variables.TryGetValue(varName, out decimal varValue))
                {
                    stack.Push(varValue);
                }
                else
                {
                    throw new Exception($"Variable '{varName}' not found");
                }
                i = endIndex;
                OperatorBefore = false;
            }
            else if (Operators.Contains(input[i]))
            {
                Operator @operator = input[i] switch
                {
                    '+' => Operator.Pluse,
                    '-' => Operator.Minus,
                    '*' => Operator.Multiply,
                    '/' => Operator.Devide,
                    '\\' => Operator.Devide,
                    '^' => Operator.Degree,
                    _ => throw new NotSupportedException()
                };

                while (operatorStack.Count > 0 && operatorStack.Peek() != Operator.Pluse && GetPrecedence(@operator) <= GetPrecedence(operatorStack.Peek()))
                {
                    ApplyOperator(stack, operatorStack.Pop());
                }

                operatorStack.Push(@operator);
                i++;

                OperatorBefore = true;
            }
            else
            {
                i++;
            }
        }

        while (operatorStack.Count > 0)
        {
            ApplyOperator(stack, operatorStack.Pop());
        }

        return stack.Pop();
    }

    static int GetPrecedence(Operator @operator)
    {
        return @operator switch
        {
            Operator.Pluse => 1,
            Operator.Minus => 1,
            Operator.Multiply => 2,
            Operator.Devide => 2,
            Operator.Degree => 3,
            _ => throw new NotSupportedException()
        };
    }

    static void ApplyOperator(Stack<decimal> stack, Operator @operator)
    {
        if (stack.Count < 2)
            throw new InvalidOperationException("Not enough operands on the stack to apply an operator.");

        decimal b = stack.Pop();
        decimal a = stack.Pop();

        decimal result = @operator switch
        {
            Operator.Pluse => a + b,
            Operator.Minus => a - b,
            Operator.Multiply => a * b,
            Operator.Devide => a / b,
            Operator.Degree => Pow(a, b),
            _ => throw new NotSupportedException()
        };

        stack.Push(result);
    }
    static decimal Pow(decimal x, decimal y)
    {
        return (decimal)Math.Pow((double)x, (double)y);
    }
    static int FindMatchingParenthesis(string input, int startIndex)
    {
        int count = 1;
        for (int i = startIndex + 1; i < input.Length; i++)
        {
            if (input[i] == '(')
            {
                count++;
            }
            else if (input[i] == ')')
            {
                count--;
                if (count == 0)
                {
                    return i;
                }
            }
        }
        throw new Exception("Unmatched parenthesis");
    }
    public static bool IsExpression(string input, Dictionary<string, decimal> variables)
    {
        try
        {
            ParseExpression(input, variables);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public static char[] Operators = ['+', '-', '/', '\\', '*', '^'];
}
public struct StringToMathResult
{
    public KeyValuePair<string, decimal>? Variable;
    public decimal? Result;
}
