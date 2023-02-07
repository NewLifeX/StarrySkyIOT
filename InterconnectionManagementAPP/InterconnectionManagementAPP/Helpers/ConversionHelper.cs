using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace InterconnectionManagementAPP.Helpers
{
    public static class ConversionHelper
    {
        /// <summary>
        /// 高效的对象深拷贝
        /// </summary>
        /// <typeparam name="TSourc"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <returns></returns>
        public static Action<TSourc, TTarget> CopyClass<TSourc, TTarget>()
        {
            System.Linq.Expressions.ParameterExpression pe_Sourc = System.Linq.Expressions.Expression.Parameter(typeof(TSourc), "p");
            System.Linq.Expressions.ParameterExpression pe_Target = System.Linq.Expressions.Expression.Parameter(typeof(TSourc), "t");
            List<System.Linq.Expressions.BinaryExpression> binaryExpressions = new List<System.Linq.Expressions.BinaryExpression>();
            foreach (var item in typeof(TSourc).GetProperties())
            {
                if (!item.CanWrite) continue;
                System.Linq.Expressions.MemberExpression property_Sourc = System.Linq.Expressions.Expression.Property(pe_Sourc, typeof(TSourc).GetProperty(item.Name));
                System.Linq.Expressions.MemberExpression property_Target = System.Linq.Expressions.Expression.Property(pe_Target, typeof(TSourc).GetProperty(item.Name));
                binaryExpressions.Add(System.Linq.Expressions.Expression.MakeBinary(ExpressionType.Assign, property_Target, property_Sourc));
            }
            System.Linq.Expressions.Expression<Action<TSourc, TTarget>> lambda = System.Linq.Expressions.Expression.Lambda<Action<TSourc, TTarget>>(Expression.Block(binaryExpressions), new System.Linq.Expressions.ParameterExpression[] { pe_Sourc, pe_Target });
            return lambda.Compile();
        }
    }
}
