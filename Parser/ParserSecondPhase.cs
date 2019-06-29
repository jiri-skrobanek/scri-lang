using Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    public static partial class ParserFirstPhase
    {
        private static Block MakeBlock(BracketContent content)
        {

        }

        private static Statement MakeStatement(IList<IToken> tokens)
        {
            if (tokens.Count == 1)
            {
                // This statement must be break, continue, return
                {
                    if (tokens[0] is ReservedWord rw)
                    {
                        switch (rw.Word)
                        {
                            case "break":
                                return new BreakStatement();
                            case "continue":
                                return new ContinueStatement();
                            case "return":
                                return new ReturnStatement { expression = new ConstantExpression { value = new None() } };
                            default:
                                throw new Exception("Invalid statement");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid statement");
                    }
                }
            }
            else if (tokens[0] is ReservedWord rw)
            {
                switch (rw.Word)
                {
                    case "if":
                        return MakeConditional(tokens);
                    case "while":
                        return MakeLoop(tokens);
                    case "return":
                        tokens.RemoveAt(0);
                        return new ReturnStatement { expression = MakeExpression(tokens) };
                    default:
                        throw new Exception("Invalid statement");
                }
            }
            else if (tokens[1] is OperatorToken op)
            {
                if (op.@operator == OperatorType.FDef)
                {
                    return MakeFunctionDefinition(tokens);
                }
                else if (op.@operator == OperatorType.Assign)
                {
                    return MakeAssignment(tokens);
                }
                else
                {
                    throw new Exception("Invalid statement");
                }
            }
            else
            {
                throw new Exception("Unrecognized statement patterns");
            }
        }

        private static IList<IExpression> MakeExpressionFromVector(IList<IList<IToken>> items)
        {
            return (from item in items select MakeExpression(item)).ToList();
        }

        private static IExpression MakeExpression(IList<IToken> tokens)
        {
            if (tokens.Count == 0)
            {
                throw new Exception("Void expression");
            }

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
                if (exp.Count == 0)
                {
                    throw new Exception("Void expression");
                }
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
                        var oper = (exp[i] as OperatorToken).@operator;
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
                return new Interpreter.OperatorEvaluation() { left_arg = left, right_arg = right, @operator = (op as OperatorToken).@operator };
            }
        }

        private static Interpreter.FunctionDefinition MakeFunctionDefinition(IList<IToken> tokens)
        {

            if (tokens[0] is CustomWord cw && tokens[2] is ArgVector av && tokens[3] is BracketContent bc)
            {
                var fc = new FunctionCall { functionName = cw.Word, args = MakeExpressionFromVector(av.List) };
                return new FunctionDefinition { Name = cw.Word, Args = extract_args(av) };
            }
            else
            {
                throw new Exception("Invalid function definition");
            }

            List<string> extract_args(ArgVector vector)
            {
                var names = new List<string>();
                foreach (var item in vector.List)
                {
                    if (item.Count == 1 && item[0] is CustomWord cw2)
                    {
                        names.Add(cw2.Word);
                    }
                    else
                    {
                        throw new Exception("Invalid argument naming in function definition");
                    }
                }
                return names;
            }
        }

        private static AssignmentStatement MakeAssignment(IList<IToken> tokens)
        {
            if (tokens[0] is CustomWord cw)
            {
                tokens.RemoveAt(0);
                tokens.RemoveAt(0);
                var expr = MakeExpression(tokens);
                return new AssignmentStatement { Expression = expr, Name = cw.Word };
            }
            else
            {
                throw new Exception("Invalid assignment");
            }
        }

        private static CallStatement MakeCallStatement(IList<IToken> tokens)
        {
            if (tokens[0] is CustomWord cw && tokens[1] is ArgVector av)
            {
                var fc = new FunctionCall { functionName = cw.Word, args = MakeExpressionFromVector(av.List) };
                return new CallStatement { Call = fc };
            }
            else
            {
                throw new Exception("Invalid function call");
            }
        }

        private static ConditionalStatement MakeConditional(IList<IToken> tokens)
        {
            var condition = tokens.Skip(1).TakeWhile(x => !(x is ReservedWord)).ToList();
            var expression = MakeExpression(condition);
            Block satisfied, failed;
            if (tokens.Count > condition.Count + 1 && tokens[condition.Count + 1] is ReservedWord rw)
            {
                if (rw.Word != "then")
                {
                    throw new Exception("No then in conditional statement");
                }
            }
            else
            {
                throw new Exception("No then in conditional statement");
            }
            if (tokens.Count > condition.Count + 2 && tokens[condition.Count + 2] is BracketContent bc)
            {
                satisfied = MakeBlock(bc);
            }
            else
            {
                throw new Exception("No code to execute in conditional statement when condition succeeds");
            }
            if (tokens.Count > condition.Count + 3)
            {
                if (tokens[condition.Count + 3] is ReservedWord rw2)
                {
                    if (rw2.Word != "else")
                    {
                        throw new Exception("Else or end of statement was expected");
                    }
                    else if (tokens.Count == condition.Count + 4 && tokens[condition.Count + 4] is BracketContent bc2)
                    {
                        failed = MakeBlock(bc2);
                    }
                    else
                    {
                        throw new Exception("No code to execute in conditional statement when condition fails");
                    }
                }
                else
                {
                    throw new Exception("Else or end of statement was expected");
                }
            }
            else
            {
                failed = new Block();
            }
            return new ConditionalStatement { condition = expression, satisfied = satisfied, unsatisfied = failed };
        }

        private static WhileLoop MakeLoop(IList<IToken> tokens)
        {
            var condition = tokens.Skip(1).TakeWhile(x => !(x is ReservedWord)).ToList();
            var expression = MakeExpression(condition);
            Block satisfied;
            if (tokens.Count > condition.Count + 1 && tokens[condition.Count + 1] is ReservedWord rw)
            {
                if (rw.Word != "do")
                {
                    throw new Exception("No do in loop statement");
                }
            }
            else
            {
                throw new Exception("No do in conditional statement");
            }
            if (tokens.Count > condition.Count + 2 && tokens[condition.Count + 2] is BracketContent bc)
            {
                satisfied = MakeBlock(bc);
            }
            else
            {
                throw new Exception("No code to execute in loop statement when condition succeeds");
            }
            return new WhileLoop { block = satisfied, condition = expression };
        }
    }
}

