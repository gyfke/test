using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Algebra.Computations.Models
{
    public abstract class Enumeration<TValue> : IComparable where TValue : IComparable
    {
        public string Name { get; private set; }

        public TValue Value { get; private set; }

        protected Enumeration(TValue value, string name) 
        {
            Value = value; 
            Name = name; 
        }

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration<TValue>
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | 
                                            BindingFlags.Static | 
                                            BindingFlags.DeclaredOnly); 

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public static T FromValue<T>(TValue value) where T : Enumeration<TValue>
        {
            var matchingItem = parse<T, TValue>(value, "value", item => item.Value.Equals(value));
            return matchingItem;
        }

        public override bool Equals(object obj) 
        {
            var otherValue = obj as Enumeration<TValue>; 

            if (otherValue == null) 
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public int CompareTo(object other) => Value.CompareTo(((Enumeration<TValue>)other).Value); 

        private static T parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration<TValue>
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
            {
                var message = string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(T));
                throw new ApplicationException(message);
            }

            return matchingItem;
        }
    }
}
