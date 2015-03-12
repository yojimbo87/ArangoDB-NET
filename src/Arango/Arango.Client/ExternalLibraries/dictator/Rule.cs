using System.Collections.Generic;

namespace Arango.Client
{
    public class Rule
    {
        string _message = null;
        
        public string FieldPath { get; set; }
        public Constraint Constraint { get; set; }
        public List<object> Parameters { get; set; }
        public bool IsViolated { get; set; }
        
        public string Message
        { 
            get
            {
                if (_message == null)
                {
                    return string.Format("Field '{0}' violated '{1}' constraint rule.", FieldPath, Constraint);
                }
                
                return _message;
            }
            set
            {
                _message = value;
            }
        }
        
        public Rule()
        {
            Parameters = new List<object>();
        }
    }
}
