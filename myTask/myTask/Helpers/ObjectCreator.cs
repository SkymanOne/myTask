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

        private static CreateObjectInternal<T> GetActivator<T>(Type objectType) where T : class
        {
            //form a new expression which call parameterless constructor of the specified type
            NewExpression newExpression = Expression.New(objectType);
            LambdaExpression lambdaExpression = Expression.Lambda<CreateObjectInternal<T>>(newExpression);
            //compile lambda into an executable function
            return (CreateObjectInternal<T>) lambdaExpression.Compile();
        }

        //need to complete tests to estimate the performance
        /// <summary>
        /// Create an object of specified type which is then casted to generic type
        /// </summary>
        /// <param name="type"></param> 
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObject<T>(Type type) where T : class
        {
            //assign the generic function to the delegate
            CreateObjectInternal<T> creator = GetActivator<T>(type);
            T instance = creator();
            return instance;
        }
        
        public static T CreateObject<T>() where T : class
        {
            CreateObjectInternal<T> creator = GetActivator<T>(typeof(T));
            T instance = creator();
            return instance;
        }
    }
}