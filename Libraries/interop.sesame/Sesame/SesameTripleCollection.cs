﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotSesame = org.openrdf.model;

namespace VDS.RDF.Interop.Sesame
{
    public class SesameTripleCollection : BaseTripleCollection
    {
        private dotSesame.Graph _g;
        private SesameMapping _mapping;

        public SesameTripleCollection(dotSesame.Graph g, SesameMapping mapping)
        {
            this._g = g;
            this._mapping = mapping;
        }

        protected override void Add(Triple t)
        {
            this._g.add(SesameConverter.ToSesame(t, this._mapping));
        }

        public override bool Contains(Triple t)
        {
            return this._g.contains(SesameConverter.ToSesame(t, this._mapping));
        }

        public override int Count
        {
            get 
            {
                return this._g.size();
            }
        }

        protected override void Delete(Triple t)
        {
            this._g.remove(SesameConverter.ToSesame(t, this._mapping));
        }

        public override Triple this[Triple t]
        {
            get 
            {
                if (this.Contains(t))
                {
                    return t;
                }
                else
                {
                    throw new KeyNotFoundException("The given Triple does not exist in this Triple Collection");
                }
            }
        }

        public override IEnumerable<INode> ObjectNodes
        {
            get 
            {
                JavaIteratorWrapper<dotSesame.Statement> stmtIter = new JavaIteratorWrapper<org.openrdf.model.Statement>(this._g.iterator());
                return (from s in stmtIter
                        select SesameConverter.FromSesameValue(s.getObject(), this._mapping)).Distinct();
            }
        }

        public override IEnumerable<INode> PredicateNodes
        {
            get 
            {
                JavaIteratorWrapper<dotSesame.Statement> stmtIter = new JavaIteratorWrapper<org.openrdf.model.Statement>(this._g.iterator());
                return (from s in stmtIter
                        select SesameConverter.FromSesameUri(s.getPredicate(), this._mapping)).Distinct();
            }
        }

        public override IEnumerable<INode> SubjectNodes
        {
            get 
            {
                JavaIteratorWrapper<dotSesame.Statement> stmtIter = new JavaIteratorWrapper<org.openrdf.model.Statement>(this._g.iterator());
                return (from s in stmtIter
                        select SesameConverter.FromSesameResource(s.getSubject(), this._mapping)).Distinct(); 
            }
        }

        public override void Dispose()
        {
            //Do Nothing
        }

        public override IEnumerator<Triple> GetEnumerator()
        {
            JavaIteratorWrapper<dotSesame.Statement> stmtIter = new JavaIteratorWrapper<org.openrdf.model.Statement>(this._g.iterator());
            return stmtIter.Select(s => SesameConverter.FromSesame(s, this._mapping)).GetEnumerator();
        }

        //TODO: Override the WithX() methods to use the match() method of the underlying Graph
    }
}
