using Interpreter;
using System;
using System.Collections.Generic;

namespace Parser
{
    public interface IToken
    {
    }

    public class ParsedExpression : IToken
    {
        public IExpression expression;
    }

    internal class OperatorToken : IToken
    {
        public OperatorToken(char Symbol)
        {
            switch (Symbol)
            {
                case '+': @operator = OperatorType.Plus; break;
                case '-': @operator = OperatorType.Minus; break;
                case '*': @operator = OperatorType.Prod; break;
                case '/': @operator = OperatorType.Div; break;
                case '=': @operator = OperatorType.Assign; break;
                case '<': @operator = OperatorType.Lesser; break;
                case '>': @operator = OperatorType.Greater; break;
                case '@': @operator = OperatorType.FDef; break;
                case '?': @operator = OperatorType.Equals; break;
                case '!': @operator = OperatorType.NEQ; break;
            }
        }

        public OperatorType @operator;
    }

    internal class ClosingBracket : IToken
    {

    }

    internal class OpeningBracket : IToken
    {

    }

    internal abstract class WordToken : IToken
    {
        public static WordToken NewWord(string word)
        {
            if (ReservedWord.IsReserved(word))
            {
                return new ReservedWord(word);
            }
            else
            {
                return new CustomWord(word);
            }
        }

        protected string _word;
        public string Word
        { get; }
    }

    internal class NumericConstant : IToken
    {
        public NumericConstant(string number)
        {
            Value = int.Parse(number);
        }

        public int Value;
    }

    internal class CharacterConstant : IToken
    {

        public CharacterConstant(char c)
        {
            if (c <= 255)
            {
                Token = c;
            }
            else
            {
                throw new Exception("Invalid character value.");
            }
        }

        public char Token { get; set; }
    }

    internal class ReservedWord : WordToken
    {
        public ReservedWord(string word)
        {
            _word = word;
        }

        public static bool IsReserved(string Word)
        {
            switch (Word)
            {
                case "if":
                case "else":
                case "while":
                    return true;
                default:
                    return false;
            }
        }
    }

    internal class CustomWord : WordToken
    {
        public CustomWord(string word)
        {
            this._word = word;
        }
    }

    internal class Separator : IToken
    {
    }

    internal class StatementTerminator : IToken
    {
    }

    /// <summary>
    /// Contains the code inside parentheses split to statements.
    /// </summary>
    internal class BracketContent : IToken
    {
        public IList<IList<IToken>> StatementList { get; set; } = new List<IList<IToken>>();
    }

    /// <summary>
    /// Contains a list of argument names or expressions separated by commas.
    /// </summary>
    internal class ArgVector : IToken
    {
        public IList<IList<IToken>> List { get; set; } = new List<IList<IToken>>();
    }
}
