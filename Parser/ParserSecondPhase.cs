using Interpreter;
using Interpreter.Value;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    public static partial class Parser
    {
        private static Block MakeBlock(BracketContent content)
        {
            var statements = from x in content.StatementList select MakeStatement(x);
            return new Block { statements = statements.ToArray() };
        }

        private static Statement MakeStatement(IList<IToken> tokens)
        {
            if (tokens.Count == 1)
            {
                // This statement must be break, continue, return
                {
                    if (tokens[0] is ReservedWord rw)
                    {
                        return rw.Word switch
                        {
                            "break" => new BreakStatement(),
                            "continue" => new ContinueStatement(),
                            "return" => new ReturnStatement { Expression = new ConstantExpression { value = new None() } },
                            _ => throw new SyntaxError("Invalid statement")
                        };
                    }
                    else
                    {
                        throw new SyntaxError("Invalid statement");
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
                        return new ReturnStatement { Expression = MakeExpression(tokens) };
                    case "print":
                        tokens.RemoveAt(0);
                        return new PrintStatement(MakeExpression(tokens));
                    default:
                        throw new SyntaxError("Invalid statement");
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
                    throw new SyntaxError("Invalid statement");
                }
            }
            else if (tokens[1] is ArgVector && tokens[0] is CustomWord)
            {
                return MakeCallStatement(tokens);
            }
            else
            {
                throw new SyntaxError("Unrecognized statement patterns");
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
                throw new SyntaxError("Void expression");
            }
            if (tokens.Count == 1)
            {
                if (tokens[0] is CustomWord cw)
                {
                    return new VariableExpression { variableName = cw.Word };
                }
                else if (tokens[0] is ArgVector av)
                {
                    if (av.List.Count != 1)
                    {
                        throw new SyntaxError("Invalid expression");
                    }
                    return MakeExpression(av.List.First());
                }
                else if (tokens[0] is NumericConstant nc)
                {
                    return new ConstantExpression { value = new IntegralValue(nc.Value) };
                }
                else if (tokens[0] is CharacterConstant cc)
                {
                    return new ConstantExpression { value = new CharValue(cc.Token) };
                }
                else if (tokens[0] is ReservedWord rw)
                {
                    if (rw.Word == "none")
                    {
                        return new ConstantExpression { value = new None() };
                    }
                    else
                    {
                        throw new SyntaxError("Invalid expression");
                    }
                }
                else
                {
                    throw new SyntaxError("Invalid expression");
                }
            }
            else
            {
                return split_by_operator(tokens, (int)OperatorType.Equals);
            }

            static IExpression split_by_operator(IList<IToken> exp, int priority)
            {
                if (priority <= 0)
                {
                    return function_calls(exp);
                }

                var arg = new List<IToken>();
                int i = 0;
                for (; i < exp.Count; i++)
                {
                    if (exp[i] is OperatorToken ot)
                    {
                        if (priority <= (int)ot.@operator)
                        {
                            break;
                        }

                    }
                    arg.Add(exp[i]);
                }
                if (arg.Count == exp.Count)
                {
                    return split_by_operator(exp, priority - 100);
                }
                IExpression left_expression = i == 0 ? null : split_by_operator(arg, priority - 100);
                OperatorType type = (exp[i++] as OperatorToken).@operator;
                arg = new List<IToken>();
                for (; i < exp.Count; i++)
                {
                    if (exp[i] is OperatorToken ot)
                    {
                        if (priority <= (int)ot.@operator)
                        {
                            if(arg.Count == 0)
                            {
                                throw new SyntaxError("Invalid expression");
                            }
                            var r_expr = split_by_operator(arg, priority - 100);
                            left_expression = new OperatorEvaluation { left_arg = left_expression, right_arg = r_expr, @operator = type };
                            arg = new List<IToken>();
                            type = (exp[i] as OperatorToken).@operator;
                            continue;
                        }
                    }
                    arg.Add(exp[i]);
                }
                if (arg.Count == 0)
                {
                    throw new SyntaxError("Invalid expression");
                }
                return new OperatorEvaluation
                {
                    left_arg = left_expression,
                    right_arg = split_by_operator(arg, priority - 100),
                    @operator = type
                };

            }

            static IExpression function_calls(IList<IToken> list)
            {
                var fn = MakeExpression(list.Take(1).ToList());
                if (list.Count > 1)
                {
                    for(int i = 1; i < list.Count; i++)
                    {
                        if(list[i] is ArgVector av)
                        {
                            fn = new FunctionCall { function = fn, args = MakeExpressionFromVector(av.List) }; 
                        }
                        else
                        {
                            throw new SyntaxError("Invalid expression");
                        }
                    }
                }
                return fn;
            }
        }

        private static Interpreter.FunctionDefinition MakeFunctionDefinition(IList<IToken> tokens)
        {

            if (tokens[0] is CustomWord cw && tokens[2] is ArgVector av && tokens[3] is BracketContent bc)
            {
                var fc = new FunctionCall
                {
                    function = new VariableExpression { variableName = cw.Word },
                    args = MakeExpressionFromVector(av.List)
                };
                return new FunctionDefinition { Name = cw.Word, Args = extract_args(av), Body = MakeBlock(bc) };
            }
            else
            {
                throw new SyntaxError("Invalid function definition");
            }

            static List<string> extract_args(ArgVector vector)
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
                        throw new SyntaxError("Invalid argument naming in function definition");
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
                throw new SyntaxError("Invalid assignment");
            }
        }

        private static CallStatement MakeCallStatement(IList<IToken> tokens)
        {
            if (tokens[0] is CustomWord cw && tokens[1] is ArgVector av)
            {
                var fc = new FunctionCall
                {
                    function = new VariableExpression { variableName = cw.Word },
                    args = MakeExpressionFromVector(av.List)
                };
                return new CallStatement { Call = fc };
            }
            else
            {
                throw new SyntaxError("Invalid function call");
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
                    throw new SyntaxError("No then in conditional statement");
                }
            }
            else
            {
                throw new SyntaxError("No then in conditional statement");
            }
            if (tokens.Count > condition.Count + 2 && tokens[condition.Count + 2] is BracketContent bc)
            {
                satisfied = MakeBlock(bc);
            }
            else
            {
                throw new SyntaxError("No code to execute in conditional statement when condition succeeds");
            }
            if (tokens.Count > condition.Count + 3)
            {
                if (tokens[condition.Count + 3] is ReservedWord rw2)
                {
                    if (rw2.Word != "else")
                    {
                        throw new SyntaxError("Else or end of statement was expected");
                    }
                    else if (tokens.Count == condition.Count + 5 && tokens[condition.Count + 4] is BracketContent bc2)
                    {
                        failed = MakeBlock(bc2);
                    }
                    else
                    {
                        throw new SyntaxError("No code to execute in conditional statement when condition fails");
                    }
                }
                else
                {
                    throw new SyntaxError("Else or end of statement was expected");
                }
            }
            else
            {
                failed = null;
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
                    throw new SyntaxError("No do in loop statement");
                }
            }
            else
            {
                throw new SyntaxError("No do in conditional statement");
            }
            if (tokens.Count > condition.Count + 2 && tokens[condition.Count + 2] is BracketContent bc)
            {
                satisfied = MakeBlock(bc);
            }
            else
            {
                throw new SyntaxError("No code to execute in loop statement when condition succeeds");
            }
            return new WhileLoop { block = satisfied, condition = expression };
        }
    }
}

