using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Parser.Tests
{
    [TestClass]
    public class TokenizationTests
    {
        [TestMethod]
        public void TestAssignment()
        {
            var assignment = "v = 3 + 5;";
            var tokens = Parser.GetTokens(assignment);
            Assert.AreEqual(tokens.Count, 6);
            Assert.AreEqual(tokens[0].GetType(), typeof(CustomWord));
            Assert.AreEqual(tokens[1].GetType(), typeof(OperatorToken));
            Assert.AreEqual(tokens[2].GetType(), typeof(NumericConstant));
            Assert.AreEqual(tokens[3].GetType(), typeof(OperatorToken));
            Assert.AreEqual(tokens[4].GetType(), typeof(NumericConstant));
            Assert.AreEqual(tokens[5].GetType(), typeof(StatementTerminator));
        }

        [TestMethod]
        public void TestIfStatement()
        {
            var assignment = "if ahoj ? 1 then (return 3;) else (ahoj = 1;);";
            var tokens = Parser.GetTokens(assignment);
            Assert.AreEqual(tokens.Count, 18);
            // if
            Assert.AreEqual(tokens[0].GetType(), typeof(ReservedWord));
            // ahoj
            Assert.AreEqual(tokens[1].GetType(), typeof(CustomWord));
            // ?
            Assert.AreEqual(tokens[2].GetType(), typeof(OperatorToken));
            // 1
            Assert.AreEqual(tokens[3].GetType(), typeof(NumericConstant));
            // then
            Assert.AreEqual(tokens[4].GetType(), typeof(ReservedWord));
            // (
            Assert.AreEqual(tokens[5].GetType(), typeof(OpeningBracket));
            // return
            Assert.AreEqual(tokens[6].GetType(), typeof(ReservedWord));
            // 3
            Assert.AreEqual(tokens[7].GetType(), typeof(NumericConstant));
            // ;
            Assert.AreEqual(tokens[8].GetType(), typeof(StatementTerminator));
            // )
            Assert.AreEqual(tokens[9].GetType(), typeof(ClosingBracket));
            // else
            Assert.AreEqual(tokens[10].GetType(), typeof(ReservedWord));
            // (
            Assert.AreEqual(tokens[11].GetType(), typeof(OpeningBracket));
            // ahoj
            Assert.AreEqual(tokens[12].GetType(), typeof(CustomWord));
            // =
            Assert.AreEqual(tokens[13].GetType(), typeof(OperatorToken));
            // 1
            Assert.AreEqual(tokens[14].GetType(), typeof(NumericConstant));
            // ;
            Assert.AreEqual(tokens[15].GetType(), typeof(StatementTerminator));
            // )
            Assert.AreEqual(tokens[16].GetType(), typeof(ClosingBracket));
            // ;
            Assert.AreEqual(tokens[17].GetType(), typeof(StatementTerminator));
        }
    }
}
