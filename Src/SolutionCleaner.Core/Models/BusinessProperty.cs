using System.Linq.Expressions;

namespace SolutionCleaner.Core.Models
{
    public class BusinessProperty<TProperty>
    {
        public BusinessProperty(MemberExpression body, string name)
        {
            Name = name;
            Body = body;
        }

        public MemberExpression Body { get; }

        public string Name { get; }
    }
}
