using Interpreter.Value;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Parser.Tests
{
    [TestClass]
    public class StatementRecognitionTests
    {
        [TestMethod]
        public void TestIfValid()
        {

            var tokens = new List<IToken> {
                new ReservedWord("if"),
                new CustomWord("ahoj"),
                new OperatorToken('?'),
                new NumericConstant("1"),
                new ReservedWord("then"),
                new BracketContent(){ StatementList = new List<IList<IToken>>{ new List<IToken> { 
                new ReservedWord("return"),
                new NumericConstant("3")
                 } } },
                new ReservedWord("else"),
                new BracketContent(){ StatementList = new List<IList<IToken>>{ new List<IToken> {
                new CustomWord("ahoj"),
                new OperatorToken('='),
                new NumericConstant("1")
                 } } }
            };
            var statement = Parser.MakeStatement(tokens);

            Assert.AreEqual(statement.GetType(), typeof(Interpreter.ConditionalStatement));
        }

        [TestMethod]
        public void TestWhileValid()
        {

            var tokens = new List<IToken> {
                new ReservedWord("while"),
                new CustomWord("ahoj"),
                new OperatorToken('?'),
                new NumericConstant("1"),
                new ReservedWord("do"),
                new BracketContent(){ StatementList = new List<IList<IToken>>{ new List<IToken>{
                new ReservedWord("return"),
                new NumericConstant("3") } } },
            };
            var statement = Parser.MakeStatement(tokens);

            Assert.AreEqual(statement.GetType(), typeof(Interpreter.WhileLoop));
        }

        [TestMethod]
        public void TestReturn()
        {
            var tokens = new List<IToken> {
                new ReservedWord("return"),
                new NumericConstant("3")
            };

            Assert.AreEqual(Parser.MakeStatement(tokens).GetType(), typeof(Interpreter.ReturnStatement));
        }

        [TestMethod]
        public void TestCallValid()
        {
            var tokens = new List<IToken> {
                new CustomWord("foo"),
                new ArgVector(){ List = new List<IList<IToken>>{ new List<IToken>{
                new CustomWord("x") } } }
            };

            Assert.AreEqual(Parser.MakeStatement(tokens).GetType(), typeof(Interpreter.CallStatement));
        }

        [TestMethod]
        public void TestCallInvalid()
        {
            var tokens = new List<IToken> {
                new CustomWord("foo"),
                new ArgVector(),
                new CustomWord("foo")
            };

            Assert.ThrowsException<SyntaxError>(() => Parser.MakeStatement(tokens));
        }

        [TestMethod]
        public void TestFunctionValid()
        {
            var tokens = new List<IToken> {
                new CustomWord("foo"),
                new OperatorToken('@'),
                new ArgVector(){ List = new List<IList<IToken>>{ new List<IToken>{
                new CustomWord("x") } } },
                new BracketContent(){ StatementList = new List<IList<IToken>>{ new List<IToken>{
                new ReservedWord("return"),
                new CustomWord("x") } } }
            };

            Assert.AreEqual(Parser.MakeStatement(tokens).GetType(), typeof(Interpreter.FunctionDefinition));
        }

        [TestMethod]
        public void TestFunctionInvalid()
        {
            var tokens = new List<IToken> {
                new CustomWord("foo"),
                new OperatorToken('@'),
                new ArgVector(){ List = new List<IList<IToken>>{ new List<IToken>{
                new CustomWord("x") } } },
                new BracketContent(){ StatementList = new List<IList<IToken>>{ new List<IToken>{
                new ReservedWord("return"),
                new CustomWord("x") } } },
                new BracketContent(){ StatementList = new List<IList<IToken>>{ new List<IToken>{
                new ReservedWord("return"),
                new CustomWord("x") } } }
            };

            Assert.ThrowsException<SyntaxError>(() => Parser.MakeStatement(tokens));
        }

        [TestMethod]
        public void TestWhileInValid()
        {

            var tokens = new List<IToken> {
                new ReservedWord("while"),
                new ReservedWord("if"),
                new OperatorToken('?'),
                new NumericConstant("1"),
                new ReservedWord("do"),
                new OpeningBracket(),
                new ReservedWord("return"),
                new NumericConstant("3"),
                new StatementTerminator(),
                new ClosingBracket(),
                new StatementTerminator()
            };
            Assert.ThrowsException<SyntaxError>(() => Parser.MakeStatement(tokens));
        }

        [TestMethod]
        public void TestIfInvalid()
        {

            var tokens = new List<IToken> {
                new ReservedWord("if"),
                new ReservedWord("while"),
                new OperatorToken('?'),
                new NumericConstant("1"),
                new ReservedWord("do"),
                new OpeningBracket(),
                new ReservedWord("return"),
                new NumericConstant("3"),
                new StatementTerminator(),
                new ClosingBracket(),
                new StatementTerminator()
            };
            Assert.ThrowsException<SyntaxError>(() => Parser.MakeStatement(tokens));
        }
    }
}
