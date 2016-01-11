using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arango.Client
{
    public abstract class AQL
    {
        private readonly Dictionary<string, Procedure> _procedures = new Dictionary<string, Procedure>();
        private readonly StringBuilder _queryCache = new StringBuilder();
        
        public Procedure Queries
        {
            get
            {
                return new Procedure(this);
            }
        }
        
        public void FOR(string item)
        {
            _queryCache.Append(item + "\n");
        }
        
        public string Call(string procedureName)
        {
            _procedures[procedureName].Body();
            
            var query = _queryCache.ToString();
            
            _queryCache.Clear();
            
            return query;
        }
        
        internal void RegisterProcedure(Procedure procedure)
        {
            _procedures.Add(procedure.Name, procedure);
        }
    }
    
    public class Procedure
    {
        private readonly AQL _parentContainer;
        
        internal string Name { get; set; }
        internal Action Body { get; set; }
        
        public Action this[string name]
        {
            set
            {
                Name = name;
                Body = value;
                
                _parentContainer.RegisterProcedure(this);
            }
        }
        
        public Procedure(AQL parentContainer)
        {
            _parentContainer = parentContainer;
        }
    }
}
