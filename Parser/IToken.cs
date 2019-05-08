using Interpreter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
    public interface IToken
    {
        object Token { get; set; }
    }

    public class ParsedExpression : IToken
    {
        public object Token { get; set; }

        public IExpression expression;
    }

    class Operator : IToken
    {
        public object Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Operator(char Symbol)
        {
            switch(Symbol)
            {
                case '+': @operator = OperatorType.Plus; break;
                case '-': @operator = OperatorType.Plus; break;
                case '*': @operator = OperatorType.Prod; break;
                case '/': @operator = OperatorType.Div; break;
                case '=': @operator = OperatorType.Equals; break;
            }
        }

        public OperatorType @operator;
    }

    class ClosingBracket : IToken
    {
        public object Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    class OpeningBracket : IToken
    {
        public object Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    abstract class WordToken : IToken
    {
        public static WordToken NewWord(string word)
        {
            if(ReservedWord.IsReserved(word))
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

        public object Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    class NumericConstant : IToken
    {
        public NumericConstant(string number)
        {
            Value = int.Parse(number);
        }

        public int Value;

        public object Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    class ReservedWord : WordToken
    {
        public ReservedWord(string word)
        {
            _word = word;
        }

        public static bool IsReserved(string Word)
        {
            switch(Word)
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

    class CustomWord : WordToken
    {
        public CustomWord(string word)
        {
            this._word = word;
        }
    }

    class Separator : IToken
    {
        public object Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
