﻿using VDS.RDF.Query.Compiler;
using VDS.RDF.Query.Engine.Algebra;
using VDS.RDF.Query.Engine.Bgps;
using VDS.RDF.Query.Engine.Joins.Strategies;

namespace VDS.RDF.Query.Processors
{
    public abstract class MedusaQueryProcessor
        : AlgebraQueryProcessor
    {
        protected MedusaQueryProcessor(IBgpExecutor bgpExecutor)
            : this(new DefaultQueryCompiler(), bgpExecutor, new DefaultJoinStrategySelector()) { }

        protected MedusaQueryProcessor(IBgpExecutor bgpExecutor, IJoinStrategySelector joinStrategySelector)
            : this(new DefaultQueryCompiler(), bgpExecutor, joinStrategySelector) { }

        protected MedusaQueryProcessor(IQueryCompiler defaultQueryCompiler, IBgpExecutor bgpExecutor, IJoinStrategySelector joinStrategySelector)
            : base(defaultQueryCompiler, new MedusaAlgebraExecutor(bgpExecutor, joinStrategySelector)) { }
    }
}
