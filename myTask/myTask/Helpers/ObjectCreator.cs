using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace myTask.Helpers
{
    public static class ObjectCreator
    {
        private delegate T CreateObjectInternal<T>();

        private static CreateObjectInternal<T> GetActivator<T>(ConstructorInfo constructorInfo) where T : class
        {
            ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
            Expression[] args = new Expression[0];

            NewExpression newExpression = Expression.New(constructorInfo, args);
            LambdaExpression lambdaExpression = Expression.Lambda(typeof(CreateObjectInternal<T>), newExpression,
                new List<ParameterExpression>(0));
            return (CreateObjectInternal<T>) lambdaExpression.Compile();
        }

        public static T CreateObject<T>(Type type) where T : class
        {
            CreateObjectInternal<T> creator = GetActivator<T>(type.GetConstructors().First());
            T instance = creator();
            return instance;
        }
        
        public static T CreateObject<T>() where T : class
        {
            CreateObjectInternal<T> creator = GetActivator<T>(typeof(T).GetConstructors().First());
            T instance = creator();
            return instance;
        }
    }
}