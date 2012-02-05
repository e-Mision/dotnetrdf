﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF.Query.Expressions;
using VDS.RDF.Nodes;
using VDS.RDF.Query.Expressions.Primary;

namespace VDS.RDF.Query.Aggregates.Leviathan
{
    /// <summary>
    /// Class representing MEDIAN Aggregate Functions
    /// </summary>
    public class MedianAggregate 
        : BaseAggregate
    {
        private String _varname;

        /// <summary>
        /// Creates a new MEDIAN Aggregate
        /// </summary>
        /// <param name="expr">Variable Expression</param>
        public MedianAggregate(VariableTerm expr)
            : this(expr, false) { }

        /// <summary>
        /// Creates a new MEDIAN Aggregate
        /// </summary>
        /// <param name="expr">Expression</param>
        public MedianAggregate(ISparqlExpression expr)
            : this(expr, false) { }

        /// <summary>
        /// Creates a new MEDIAN Aggregate
        /// </summary>
        /// <param name="expr">Variable Expression</param>
        /// <param name="distinct">Whether a DISTINCT modifier applies</param>
        public MedianAggregate(VariableTerm expr, bool distinct)
            : base(expr, distinct)
        {
            this._varname = expr.ToString().Substring(1);
        }

        /// <summary>
        /// Creates a new MEDIAN Aggregate
        /// </summary>
        /// <param name="expr">Expression</param>
        /// <param name="distinct">Whether a DISTINCT modifer applies</param>
        public MedianAggregate(ISparqlExpression expr, bool distinct)
            : base(expr, distinct) { }

        /// <summary>
        /// Applies the Median Aggregate function to the results
        /// </summary>
        /// <param name="context">Evaluation Context</param>
        /// <param name="bindingIDs">Binding IDs over which the Aggregate applies</param>
        /// <returns></returns>
        public override IValuedNode Apply(SparqlEvaluationContext context, IEnumerable<int> bindingIDs)
        {
            if (this._varname != null)
            {
                //Ensured the MEDIANed variable is in the Variables of the Results
                if (!context.Binder.Variables.Contains(this._varname))
                {
                    throw new RdfQueryException("Cannot use the Variable " + this._expr.ToString() + " in a MEDIAN Aggregate since the Variable does not occur in a Graph Pattern");
                }
            }

            List<IValuedNode> values = new List<IValuedNode>();
            HashSet<IValuedNode> distinctValues = new HashSet<IValuedNode>();
            bool nullSeen = false;
            foreach (int id in bindingIDs)
            {
                try
                {
                    IValuedNode temp = this._expr.Evaluate(context, id);
                    if (this._distinct)
                    {
                        if (temp != null)
                        {
                            if (distinctValues.Contains(temp))
                            {
                                continue;
                            }
                            else
                            {
                                distinctValues.Add(temp);
                            }
                        }
                        else if (!nullSeen)
                        {
                            nullSeen = false;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    values.Add(temp);
                }
                catch
                {
                    //Ignore errors
                }
            }

            if (values.Count == 0) return null;

            //Find the middle value and return
            values.Sort();
            int skip = values.Count / 2;
            return values.Skip(skip).First();
        }

        /// <summary>
        /// Gets the String representation of the Aggregate
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            output.Append('<');
            output.Append(LeviathanFunctionFactory.LeviathanFunctionsNamespace);
            output.Append(LeviathanFunctionFactory.Median);
            output.Append(">(");
            if (this._distinct) output.Append("DISTINCT ");
            output.Append(this._expr.ToString());
            output.Append(')');
            return output.ToString();
        }

        /// <summary>
        /// Gets the Functor of the Aggregate
        /// </summary>
        public override string Functor
        {
            get
            {
                return LeviathanFunctionFactory.LeviathanFunctionsNamespace + LeviathanFunctionFactory.Median;
            }
        }
    }
}
