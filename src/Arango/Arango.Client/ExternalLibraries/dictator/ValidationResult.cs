using System.Collections.Generic;

namespace Arango.Client.ExternalLibraries.dictator
{
    public class ValidationResult
    {
        public bool IsValid 
        {
            get
            {
                return (Violations.Count == 0);
            }
        }
        
        public List<Rule> Violations { get; private set; }
        
        public ValidationResult()
        {
            Violations = new List<Rule>();
        }
        
        public void AddViolation(Rule rule)
        {
            var violatedRule = new Rule();
            violatedRule.FieldPath = rule.FieldPath;
            violatedRule.Constraint = rule.Constraint;
            violatedRule.Parameters = rule.Parameters;
            violatedRule.Message = rule.Message;
            violatedRule.IsViolated = true;
            
            Violations.Add(violatedRule);
        }
        
        public void AddViolations(List<Rule> rules)
        {
            foreach (var rule in rules)
            {
                AddViolation(rule);
            }
        }
    }
}
