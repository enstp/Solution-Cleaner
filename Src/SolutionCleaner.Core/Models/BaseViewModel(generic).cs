using System;
using System.Linq.Expressions;

namespace SolutionCleaner.Core.Models
{
    public class BaseViewModel<T> : BaseViewModel
    {
        protected static BusinessProperty<TProperty> RegisterProperty<TProperty>(Expression<Func<T, TProperty>> projection)
        {
            MemberExpression memberExpression = (MemberExpression)projection.Body;
            return new BusinessProperty<TProperty>(memberExpression, memberExpression.Member.Name);
        }
    }
}
