using System;

namespace Trinity.Framework.Helpers
{
    public class StaticRouter<T>
    {
        public T Value => _expr();
        private readonly Func<T> _expr;

        public StaticRouter(Type parentType)
        {
            _expr = ReflectionHelper.GetStaticPropertyAccessor<T>(parentType);
        }

        public StaticRouter(Type parentType, string memberName)
        {
            _expr = ReflectionHelper.GetStaticAccessor<T>(parentType, memberName);
        }

        public StaticRouter(Type parentType, int index)
        {
            _expr = ReflectionHelper.GetPrivateStaticFieldAccessor<T>(parentType, index);
        }

        
    }
}

