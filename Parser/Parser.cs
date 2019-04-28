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
                switch(Code[i])
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
                while(index < Code.Length && Code[index] >= 0 && Code[index] <= 9)
                {
                    sb.Append(Code[index++]);
                }
                tokens.Add(new NumericConstant(sb.ToString()));
                sb.Clear();
                index--;
            }
        }

        public static IExpression ParseExpression(IList<IToken> Expression)
        {
            if(Expression.Count == 0)
            {
                throw new Exception();
            }
            int highest_priority_index = 0;
            int highest_priority = 0;
            for (int i = 0; i < Expression.Count; i++)
            {
                if (Expression[i].GetType() == typeof(OperatorToken))
                {
                    var op = (Expression[i] as Operator).@operator;
                    if (OperatorPriorities.HasGreaterPriority(highest_priority, op))
                    {
                        highest_priority = (int)op;
                        highest_priority_index = i;
                    }
                }
                else if (Expression[i].GetType() == typeof(CustomWord))
                {
                    if (Expression.Count > i + 1 && Expression[i+1].GetType() == typeof(OpeningBracket))
                    {
                        var call = new FunctionCall();
                        call.functionName = (Expression[i] as CustomWord).Word;
                        while (Expression[i].GetType() != typeof(ClosingBracket))
                        {
                            var argument = new List<IToken>();
                            while (Expression[i].GetType() != typeof(Separator) || Expression[i].GetType() != typeof(ClosingBracket))
                            {
                                argument.Add(Expression[i]);
                                i++;
                            }
                            call.args.Add(ParseExpression(argument));
                        }
                    }
                }
            }
        }
    }
}
