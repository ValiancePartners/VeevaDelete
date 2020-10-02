using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VeevaDelete
{
    public static class PropertyHelper
    {
        public static void AssignIfChanged<TObject, TProperty>(TObject owner, string propertyName, TProperty value)
        {
            Func<TObject, TProperty> getter = GetSetHelper<TObject,TProperty>.GetPropGetter(propertyName);
            Action<TObject, TProperty> setter = GetSetHelper<TObject,TProperty>.GetPropSetter(propertyName);
            TProperty oldValue = getter(owner);
            if (!value.Equals(oldValue))
            {
                setter(owner, value);
            }
        }

        public static class GetSetHelper<TObject, TProperty>
        {
            // returns property getter
            public static Func<TObject, TProperty> GetPropGetter(string propertyName)
            {
                ParameterExpression paramExpression = Expression.Parameter(typeof(TObject), "value");

                Expression propertyGetterExpression = Expression.Property(paramExpression, propertyName);

                Func<TObject, TProperty> result =
                    Expression.Lambda<Func<TObject, TProperty>>(propertyGetterExpression, paramExpression).Compile();

                return result;
            }

            // returns property setter:
            public static Action<TObject, TProperty> GetPropSetter(string propertyName)
            {
                ParameterExpression paramExpression = Expression.Parameter(typeof(TObject));

                ParameterExpression paramExpression2 = Expression.Parameter(typeof(TProperty), propertyName);

                MemberExpression propertyGetterExpression = Expression.Property(paramExpression, propertyName);

                Action<TObject, TProperty> result = Expression.Lambda<Action<TObject, TProperty>>
                (
                    Expression.Assign(propertyGetterExpression, paramExpression2), paramExpression, paramExpression2
                ).Compile();

                return result;
            }
        }
    }
}
