using System;
using System.Collections.Generic;
using System.Linq;

namespace BookBuilder.Pipeline.Common
{
    internal class Context
    {
        readonly Dictionary<Type, object> _context;
        private const int DefaultCacheSize = 10; 

        public Context(params (Type type, Object obj)[] registrations) : this()
        {
            foreach (var (type, element) in registrations)
            {
                _context[type] = element;
            }
        }

        public Context(params object[] registrations) : this()
        {
            foreach (var element in registrations)
            {
                _context[element.GetType()] = element;
            }
        }

        private Context()
        {
            _context = new Dictionary<Type, object>(DefaultCacheSize);
        }

        public static Context Create()
        {
            return new Context();
        }
        
        private Context(Context copyFrom) : this()
        {
            foreach (var kvp in copyFrom._context)
            {
                _context[kvp.Key] = kvp.Value;
            }
        }
        
        public Context With<T>(T value)
        {
            _context.Add(typeof(T), value);
            return this;
        }
        
        public T Get<T>()
        {
            return (T)_context[typeof(T)];
        }

        public Context CreateCopy()
        {
            return new Context(this);
        }
        
        public Context CreateCopy(params object[] registrations)
        {
            var ctx = new Context(this);
            foreach (var registration in registrations)
            {
                ctx._context[registration.GetType()] = registration;
            }
            return ctx;
        }
    }
}