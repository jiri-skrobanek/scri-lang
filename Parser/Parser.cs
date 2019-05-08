using System;
using System.Collections.Generic;
using Interpreter;
using System.Text;

namespace Parser
{
    public static class Parser
    {
        public static Block ParseCode(String Code)
        {
            GetTokens(Code);

        }

        public static IList<IToken> GetTokens(String Code)
        {
            List<IToken> tokens = new List<IToken>();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Code.Length; i++)
            {
                switch (Code[i])
                {
                    case ' ':
                    case '\n':
                        new_token(); break;
                    case '(':
                        new_token(); tokens.Add(new OpeningBracket()); break;
                    case ')':
                        new_token(); tokens.Add(new ClosingBracket()); break;
                    case '+':
                    case '-':
                    case '/':
                    case '*':
                    case '=':
                        new_token(); tokens.Add(new Operator(Code[i])); break;
                    case ',':
                        new_token(); tokens.Add(new Separator()); break;
                    case char c: if (sb.Length == 0 && c >= 0 && c <= 9) read_numeric(ref i); else sb.Append(c); break;
                }
            }

            new_token();

            return tokens;

            void new_token()
            {
                if (sb.Length > 0)
                {
                    tokens.Add(WordToken.NewWord(sb.ToString())); sb.Clear();
                }
            }

            void read_numeric(ref int index)
            {
                while (index < Code.Length && Code[index] >= 0 && Code[index] <= 9)
                {
                    sb.Append(Code[index++]);
                }
                tokens.Add(new NumericConstant(sb.ToString()));
                sb.Clear();
                index--;
            }
        }

        private static IExpression Parse(List<IToken> tokens)
        {
            if (tokens.Count == 0) throw new Exception("Void expression");

            // Remove function calls and nested parentheses:
            List<IToken> simplified = new List<IToken>();
            int bracket_index = 0;
            int start_position = 0;
            bool will_be_function_call = false;
            for (int i = 0; i < tokens.Count; i++)
            {
                will_be_function_call = tokens[i].GetType() == typeof(CustomWord) && bracket_index == 0;
                if (tokens[i].GetType() == typeof(OpeningBracket))
                {
                    if (bracket_index == 0)
                    {
                        start_position = i;
                        if (will_be_function_call) { simplified.RemoveAt(simplified.Count - 1); }
                    }
                    bracket_index++;
                }
                else if (tokens[i].GetType() == typeof(ClosingBracket))
                {
                    bracket_index--;
                    if (bracket_index == 0)
                    {
                        if (will_be_function_call)
                        {
                            simplified.Add(ParseFunctionCall(tokens.GetRange(start_position - 1, i - start_position + 2)));
                        }
                        else
                        {
                            simplified.Add(new ParsedExpression { expression = Parse(tokens.GetRange(start_position + 1, i - start_position - 1)) });
                        }
                    }
                    else if (bracket_index < 0)
                    {
                        throw new Exception("Unbalanced brackets");
                    }
                }
                else if (bracket_index == 0)
                {
                    simplified.Add(tokens[i]);
                }
            }

            return SplitByOperator(simplified);

            // Keep finding the highest priority operator:
            IExpression SplitByOperator(List<IToken> exp)
            {
                if (exp.Count == 0) throw new Exception("Void expression");
                else if (exp.Count == 1)
                {
                    if (exp[0].GetType() == typeof(ParsedExpression))
                    {
                        return (exp[0] as ParsedExpression).expression;
                    }
                    else if (exp[0].GetType() == typeof(CustomWord))
                    {
                        return new VariableExpression { variableName = (exp[0] as CustomWord).Word };
                    }
                    else if (exp[0].GetType() == typeof(NumericConstant))
                    {
                        return new ConstantExpression { value = new IntegralValue((exp[0] as NumericConstant).Value) };
                    }
                    else
                    {
                        throw new Exception("Invalid expression");
                    }
                }
                // Unary operator:
                else if (exp.Count == 2)
                {
                    throw new NotImplementedException("Unary operator");
                }
                int highest_priority_index = 0;
                int highest_priority = 0;
                for (int i = 0; i < exp.Count; i++)
                {
                    if (exp[i].GetType() == typeof(OperatorType))
                    {
                        var oper = (exp[i] as Operator).@operator;
                        if (OperatorPriorities.HasGreaterPriority(highest_priority, oper))
                        {
                            highest_priority = (int)oper;
                            highest_priority_index = i;
                        }
                    }
                }
                // Binary operator:
                var left = SplitByOperator(exp.GetRange(0, highest_priority_index - 1));
                var right = SplitByOperator(exp.GetRange(highest_priority_index + 1, exp.Count - highest_priority_index));
                var op = tokens[highest_priority_index];
                return new Interpreter.OperatorEvaluation() { left_arg = left, right_arg = right, @operator = (op as Operator).@operator };
            }
        }

        /// <summary>
        /// Parses a function call
        /// </summary>
        /// <param name="list">Tokens must be with name and parentheses, i.d. in the form ["foo", "(", "a", ",", "b", ")"].</param>
        /// <returns></returns>
        private static IToken ParseFunctionCall(List<IToken> list)
        {
            List<IExpression> arguments = new List<IExpression>();
            // Isolated arguments:
            int arg_begin = 2;
            int bracket_index = 0;
            for (int i = 3; i < list.Count - 1; i++)
            {
                if (bracket_index == 0 && list[i].GetType() == typeof(Separator))
                {
                    arg_begin = i + 1;
                    arguments.Add(Parse(list.GetRange(arg_begin, i - arg_begin)));
                }
                else if (list[i].GetType() == typeof(OpeningBracket))
                {
                    bracket_index++;
                }
                else if (list[i].GetType() == typeof(ClosingBracket))
                {
                    bracket_index--;
                }
            }
            if(arg_begin < list.Count - 1) arguments.Add(Parse(list.GetRange(arg_begin, list.Count - 1 - arg_begin)));
            var expr = new FunctionCall { functionName = (list[0] as CustomWord).Word, args = arguments };
            return new ParsedExpression { expression = expr };
        }
    }
}
