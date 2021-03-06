﻿using sly.lexer;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using sly.parser.syntax;
using System.Reflection;
using sly.parser.generator;

namespace sly.parser.syntax
{

    public class Rule<IN> : GrammarNode<IN> where IN : struct
    {

        public bool IsByPassRule { get; set; } = false;

        // visitors for operation rules
        private Dictionary<IN, OperationMetaData<IN>> VisitorMethodsForOperation { get; set; }

        // visitor for classical rules
        private MethodInfo Visitor { get; set; }

        public bool IsExpressionRule { get; set; }

        public string RuleString { get; }

        public string Key
        {

            get
            {
                string k = Clauses
                    .Select(c => c.ToString())
                    .Aggregate<string>((c1, c2) => c1.ToString() + "_" + c2.ToString());
                if (Clauses.Count == 1)
                {
                    k += "_";
                }
                return k;
            }
        }

        public List<IClause<IN>> Clauses { get; set; }
        public List<IN> PossibleLeadingTokens { get; set; }

        public string NonTerminalName { get; set; }

        public bool ContainsSubRule
        {
            get
            {               
                if (Clauses != null && Clauses.Any())
                {
                    foreach(IClause<IN> clause in Clauses)
                    {
                        if (clause is GroupClause<IN>)
                        {
                            return true;
                        }
                        if (clause is ManyClause<IN> many)
                        {
                            return many.Clause is GroupClause<IN>;
                        }
                        if (clause is OptionClause<IN> option)
                        {
                            return option.Clause is GroupClause<IN>;
                        }
                    }
                }
                return false;
            }
        }

        public bool IsSubRule { get; set; }
        



    public OperationMetaData<IN> GetOperation(IN token = default(IN))
        {
            if (IsExpressionRule)
            {
                OperationMetaData<IN> operation = VisitorMethodsForOperation.ContainsKey(token) ? VisitorMethodsForOperation[token] : null;
                return operation;
            }
            return null;
        }

        public MethodInfo GetVisitor(IN token = default(IN))
        {
            MethodInfo visitor = null;
            if (IsExpressionRule)
            {
                OperationMetaData < IN > operation = VisitorMethodsForOperation.ContainsKey(token) ? VisitorMethodsForOperation[token] : null;
                visitor = operation?.VisitorMethod;
            }
            else
            {
                visitor = Visitor;
            }
            return visitor;

        }

        public void SetVisitor(MethodInfo visitor)
        {
            Visitor = visitor;
        }

        public void SetVisitor(OperationMetaData<IN> operation)
        {
            VisitorMethodsForOperation[operation.OperatorToken] = operation;
        }

       


        public Rule()
        {
            Clauses = new List<IClause<IN>>();
            VisitorMethodsForOperation = new Dictionary<IN, OperationMetaData<IN>>();
            Visitor = null;
            IsSubRule = false;
        }

        public bool MayBeEmpty { get
            {
                return Clauses == null
                    || Clauses.Count == 0
                    || (Clauses.Count == 1 && Clauses[0].MayBeEmpty());
            } }

        

    }
}
