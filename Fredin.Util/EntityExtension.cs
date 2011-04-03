using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Fredin.Util
{
	public static class EntityExtension
	{
		public static bool TryDetach(this ObjectContext context, object entity)
		{
			bool detached = false;
			ObjectStateEntry entry;
			if (entity != null && context.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
			{
				context.Detach(entity);
				detached = true;
			}
			return detached;
		}

		public static bool TryDetach(this ObjectContext context, IEnumerable<object> entities)
		{
			bool detached = true;
			foreach (object e in entities)
			{
				detached &= context.TryDetach(e);
			}
			return detached;
		}

		public static bool TryAttach(this ObjectContext context, IEntityWithKey entity)
		{
			bool attached = false;
			ObjectStateEntry entry;
			if (entity != null && !context.ObjectStateManager.TryGetObjectStateEntry(entity, out entry))
			{
				context.Attach(entity);
				attached = true;
			}
			return attached;
		}

		public static bool TryAttach(this ObjectContext context, IEnumerable<IEntityWithKey> entities)
		{
			bool attached = true;
			foreach (IEntityWithKey e in entities)
			{
				attached &= context.TryAttach(e);
			}
			return attached;
		}

		public static ObjectQuery<T> Include<T>(this ObjectQuery<T> mainQuery, Expression<Func<T, object>> subSelector)
		{
			return mainQuery.Include(FuncToString(subSelector.Body));
		}

		private static string FuncToString(Expression selector)
		{
			switch (selector.NodeType)
			{

				case ExpressionType.MemberAccess:
					return ((selector as MemberExpression).Member as System.Reflection.PropertyInfo).Name;

				case ExpressionType.Call:
					var method = selector as MethodCallExpression;
					return FuncToString(method.Arguments[0]) + "." + FuncToString(method.Arguments[1]);

				case ExpressionType.Quote:
					return FuncToString(((selector as UnaryExpression).Operand as LambdaExpression).Body);

			}

			throw new InvalidOperationException();
		}

		public static K Include<T, K>(this EntityCollection<T> mainQuery, Expression<Func<T, object>> subSelector)
			where T : EntityObject, IEntityWithRelationships
			where K : class
		{
			return null;
		}

		public static K Include<T, K>(this T mainQuery, Expression<Func<T, object>> subSelector)
			where T : EntityObject
			where K : class
		{
			return null;
		}

		private static Expression<Func<TElement, bool>> GetWhereInExpression<TElement, TValue>(Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
		{
			ParameterExpression p = propertySelector.Parameters.Single();
			if (!values.Any())
				return e => false;

			var equals = values.Select(value => (Expression)Expression.Equal(propertySelector.Body, Expression.Constant(value, typeof(TValue))));
			var body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));

			return Expression.Lambda<Func<TElement, bool>>(body, p);
		}

		/// <summary> 
		/// Return the element that the specified property's value is contained in the specifiec values 
		/// </summary> 
		/// <typeparam name="TElement">The type of the element.</typeparam> 
		/// <typeparam name="TValue">The type of the values.</typeparam> 
		/// <param name="source">The source.</param> 
		/// <param name="propertySelector">The property to be tested.</param> 
		/// <param name="values">The accepted values of the property.</param> 
		/// <returns>The accepted elements.</returns> 
		public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, params TValue[] values)
		{
			return source.Where(GetWhereInExpression(propertySelector, values));
		}

		/// <summary> 
		/// Return the element that the specified property's value is contained in the specifiec values 
		/// </summary> 
		/// <typeparam name="TElement">The type of the element.</typeparam> 
		/// <typeparam name="TValue">The type of the values.</typeparam> 
		/// <param name="source">The source.</param> 
		/// <param name="propertySelector">The property to be tested.</param> 
		/// <param name="values">The accepted values of the property.</param> 
		/// <returns>The accepted elements.</returns> 
		public static IQueryable<TElement> WhereIn<TElement, TValue>(this IQueryable<TElement> source, Expression<Func<TElement, TValue>> propertySelector, IEnumerable<TValue> values)
		{
			return source.Where(GetWhereInExpression(propertySelector, values));
		}
	}
}
