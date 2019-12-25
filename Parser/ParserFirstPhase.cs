using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    public static partial class Parser
    {
        /// <summary>
        /// Builds a block of statements from the given string
        /// </summary>
        public static Block ParseCode(String Code)
        {
            var statements = new List<Statement>();
            var tokens = GetTokens(Code);
            foreach (var st in GetStatements(tokens))
            {
                statements.Add(MakeStatement(st));
            }
            return new Block { statements = statements };
        }

        /// <summary>
        /// Splits code to statements
        /// </summary>
        /// <param name="Tokens">The tokens from a code</param>
        /// <returns>Collection of statements -- Lists of ITokens</returns>
        private static IEnumerable<List<IToken>> GetStatements(IList<IToken> Tokens)
        {
            List<List<IToken>> statements = new List<List<IToken>>();
            int i = 0;
            while (i < Tokens.Count)
            {
                statements.Add(read_next_statement(ref i));
            }

            return statements;

            List<IToken> read_next_statement(ref int index)
            {
                var statement = new List<IToken>();
                for (; index < Tokens.Count; index++)
                {
                    if (Tokens[index] is OpeningBracket)
                    {
                        statement.Add(read_next_bracket(ref index));
                    }
                    else if (Tokens[index] is StatementTerminator)
                    {
                        if (statement.Count == 0)
                        {
                            throw new SyntaxError("Empty statement");
                        }

                        index++;
                        break;
                    }
                    else
                    {
                        statement.Add(Tokens[index]);
                    }
                }

                return statement;
            }

            IToken read_next_bracket(ref int index)
            {
                index++;

                var content = new List<IList<IToken>>();
                var current = new List<IToken>();
                for (; index < Tokens.Count; index++)
                {
                    if (Tokens[index] is OpeningBracket)
                    {
                        current.Add(read_next_bracket(ref index));
                    }
                    else if (Tokens[index] is StatementTerminator)
                    {
                        if (current.Count == 0)
                        {
                            throw new SyntaxError("Empty statement");
                        }

                        content.Add(current);
                        current = new List<IToken>();
                    }
                    else if (Tokens[index] is ClosingBracket)
                    {
                        if (content.Count == 0)
                        { return new ArgVector() { List = split_by_commas(current) }; }
                        else if (current.Count == 0)
                        {
                            return new BracketContent { StatementList = content };
                        }
                        else
                        {
                            throw new SyntaxError("Missing statement terminator.");
                        }
                    }
                    else
                    {
                        current.Add(Tokens[index]);
                    }
                }
                throw new SyntaxError("Unbalanced brackets");
            }

            static IList<IList<IToken>> split_by_commas(IList<IToken> tokens)
            {
                var result = new List<IList<IToken>>();
                var current = new List<IToken>();
                for(int j = 0; j < tokens.Count; j++)
                {
                    if(tokens[j] is Separator)
                    {
                        if(current.Count == 0)
                        {
                            throw new SyntaxError("Null expression");
                        }
                        else
                        {
                            result.Add(current);
                            current = new List<IToken>();
                        }
                    }
                    else
                    {
                        current.Add(tokens[j]);
                    }
                }
                if (current.Count == 0 && result.Count > 0)
                {
                    throw new SyntaxError("Null expression");
                }
                else if(current.Count > 0)
                {
                    result.Add(current);
                }

                return result;
            }
        }

        /// <summary>
        /// Processes the code provided and returns a tokenized list.
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static List<IToken> GetTokens(string Code)
        {
            List<IToken> tokens = new List<IToken>();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Code.Length; i++)
            {
                switch (Code[i])
                {
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                        new_token(); break;
                    case '(':
                        new_token(); tokens.Add(new OpeningBracket()); break;
                    case ')':
                        new_token(); tokens.Add(new ClosingBracket()); break;
                    case '+':
                    case '-':
                    case '/':
                    case '*':
                    case '?':
                    case '!':
                    case '<':
                    case '>':
                    case '=':
                    case '&':
                    case '|':
                    case '@':
                        new_token(); tokens.Add(new OperatorToken(Code[i])); break;
                    case '#':
                        new_token(); tokens.Add(new CharacterConstant(Code[++i])); break;
                    case ',':
                        new_token(); tokens.Add(new Separator()); break;
                    case ';':
                        new_token(); tokens.Add(new StatementTerminator()); break;
                    case char c:
                        if (sb.Length == 0 && c >= '0' && c <= '9')
                        {
                            read_numeric(ref i);
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
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
                while (index < Code.Length && Code[index] >= '0' && Code[index] <= '9')
                {
                    sb.Append(Code[index++]);
                }
                tokens.Add(new NumericConstant(sb.ToString()));
                sb.Clear();
                index--;
            }
        }
    }
}
