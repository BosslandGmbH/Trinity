using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

// Compacted version of https://csharpeval.codeplex.com/ 
// Since we aren't allowed to include .dll files in plugins.
// Includes custom fix for CompiledExpression<T> compile error

namespace Trinity.Framework.Helpers
{
    public class CompiledExpression<T> : ExpressionCompiler
    {
        private Action _compiledAction;
        private Func<T> _compiledMethod;

        public CompiledExpression() { Parser = new Parser { TypeRegistry = TypeRegistry }; }

        public CompiledExpression(string expression) { Parser = new Parser(expression) { TypeRegistry = TypeRegistry }; }

        public object Global { set { Parser.Global = value; } }

        public Func<T> Compile(bool isCall = false)
        {
            if (Expression == null) Expression = WrapExpression<T>(BuildTree());
            return Expression.Lambda<Func<T>>(Expression).Compile();
        }

        public Action CompileCall()
        {
            if (Expression == null) Expression = BuildTree(null, true);
            return Expression.Lambda<Action>(Expression).Compile();
        }

        public Action<U> ScopeCompileCall<U>()
        {
            var scopeParam = Expression.Parameter(typeof(U), "scope");
            if (Expression == null) Expression = BuildTree(scopeParam, true);
            return Expression.Lambda<Action<U>>(Expression, scopeParam).Compile();
        }

        public Func<object, T> ScopeCompile()
        {
            var scopeParam = Expression.Parameter(typeof(object), "scope");
            if (Expression == null) Expression = WrapExpression(BuildTree(scopeParam));
            return Expression.Lambda<Func<dynamic, T>>(Expression, scopeParam).Compile();
        }

        public Func<U, T> ScopeCompile<U>()
        {
            var scopeParam = Expression.Parameter(typeof(U), "scope");
            if (Expression == null) Expression = WrapExpression(BuildTree(scopeParam));
            return Expression.Lambda<Func<U, T>>(Expression, scopeParam).Compile();
        }

        protected override void ClearCompiledMethod()
        {
            _compiledMethod = null;
            _compiledAction = null;
        }

        public T Eval()
        {
            if (_compiledMethod == null) _compiledMethod = Compile();
            return (T)Convert.ChangeType(_compiledMethod(), typeof(T));
        }

        public void Call()
        {
            if (_compiledAction == null) _compiledAction = CompileCall();
            _compiledAction();
        }
    }

    public class CompiledExpression : ExpressionCompiler
    {
        private Action _compiledAction;
        private Func<object> _compiledMethod;

        public CompiledExpression()
        {
            Parser = new Parser {TypeRegistry = TypeRegistry};
        }

        public CompiledExpression(string expression)
        {
            Parser = new Parser(expression) {TypeRegistry = TypeRegistry};
        }

        public object Global { set { Parser.Global = value; } }

        public Func<object> Compile()
        {
            if (Expression == null) Expression = WrapExpression(BuildTree());
            return Expression.Lambda<Func<object>>(Expression).Compile();
        }

        public Action CompileCall()
        {
            if (Expression == null) Expression = BuildTree(null, true);
            return Expression.Lambda<Action>(Expression).Compile();
        }

        public Func<object, object> ScopeCompile()
        {
            var scopeParam = Expression.Parameter(typeof(object), "scope");
            if (Expression == null) Expression = WrapExpression(BuildTree(scopeParam));
            return Expression.Lambda<Func<dynamic, object>>(Expression, scopeParam).Compile();
        }

        public Action<object> ScopeCompileCall()
        {
            var scopeParam = Expression.Parameter(typeof(object), "scope");
            if (Expression == null) Expression = BuildTree(scopeParam, true);
            return Expression.Lambda<Action<dynamic>>(Expression, scopeParam).Compile();
        }

        public Action<U> ScopeCompileCall<U>()
        {
            var scopeParam = Expression.Parameter(typeof(U), "scope");
            if (Expression == null) Expression = BuildTree(scopeParam);
            return Expression.Lambda<Action<U>>(Expression, scopeParam).Compile();
        }

        public Func<U, object> ScopeCompile<U>()
        {
            var scopeParam = Expression.Parameter(typeof(U), "scope");
            if (Expression == null) Expression = WrapExpression(BuildTree(scopeParam));
            return Expression.Lambda<Func<U, object>>(Expression, scopeParam).Compile();
        }

        protected override void ClearCompiledMethod()
        {
            _compiledMethod = null;
            _compiledAction = null;
        }

        public object Eval()
        {
            if (_compiledMethod == null) _compiledMethod = Compile();
            return _compiledMethod();
        }

        public void Call()
        {
            if (_compiledAction == null) _compiledAction = CompileCall();
            _compiledAction();
        }
    }

    public abstract class ExpressionCompiler
    {
        protected Expression Expression;
        protected Parser Parser;
        protected string Pstr = null;
        protected TypeRegistry TypeRegistry = new TypeRegistry();

        public string StringToParse
        {
            get { return Parser.StringToParse; }
            set
            {
                Parser.StringToParse = value;
                Expression = null;
                ClearCompiledMethod();
            }
        }

        public void RegisterDefaultTypes() { TypeRegistry.RegisterDefaultTypes(); }

        public void RegisterType(string key, object type) { TypeRegistry.Add(key, type); }

        protected Expression BuildTree(Expression scopeParam = null, bool isCall = false) { return Parser.BuildTree(scopeParam, isCall); }

        protected abstract void ClearCompiledMethod();

        protected void Parse() { Parser.Parse(); }

        public void RegisterNamespace(string p) { }

        public void RegisterAssembly(Assembly assembly) { }

        protected Expression WrapExpression(Expression source, bool castToObject = true)
        {
            if (source.Type != typeof(void) && castToObject) { return Expression.Convert(source, typeof(object)); }
            return Expression.Block(source, Expression.Constant(null));
        }

        protected Expression WrapExpression<T>(Expression source)
        {
            if (source.Type != typeof(void)) { return Expression.Convert(source, typeof(T)); }
            return Expression.Block(source, Expression.Constant(null));
        }
    }

    public class HelperMethods
    {
        public static bool IsANumber(string str, int ptr) { return (!IsNumeric(str, ptr - 1) & (str[ptr] == '-') & IsNumeric(str, ptr + 1)) || IsNumeric(str, ptr); }

        public static bool IsNumeric(string str, int ptr)
        {
            if ((ptr >= 0) & (ptr < str.Length)) return (str[ptr] >= '0' & str[ptr] <= '9');
            return true;
        }

        public static bool IsHexStart(string str, int ptr)
        {
            if ((ptr >= 0) & (ptr + 2 < str.Length)) return (str[ptr] == '-' & str[ptr + 1] == '0' & str[ptr + 2] == 'x');
            if ((ptr >= 0) & (ptr + 1 < str.Length)) return (str[ptr] == '0' & str[ptr + 1] == 'x');
            return false;
        }

        public static bool IsHex(string str, int ptr)
        {
            if ((ptr >= 0) & (ptr < str.Length)) return (str[ptr] >= '0' & str[ptr] <= '9') | (str[ptr] >= 'A' & str[ptr] <= 'F') | (str[ptr] >= 'a' & str[ptr] <= 'f');
            return true;
        }

        public static bool IsAlpha(char chr) { return (chr >= 'A' & chr <= 'Z') || (chr >= 'a' & chr <= 'z'); }

        internal static bool IsWhiteSpace(string str, int ptr) { return (str[ptr] == ' ' || str[ptr] == '\t'); }
    }

    internal class OpToken : Token
    {
        public OpToken()
        {
            IsOperator = true;
            ArgCount = 0;
        }
    }

    internal class Token
    {
        public object Value { get; set; }
        public bool IsIdent { get; set; }
        public bool IsOperator { get; set; }
        public bool IsType { get; set; }
        public Type Type { get; set; }
        public int ArgCount { get; set; }
        public int Ptr { get; set; }
        public bool IsCast { get; set; }
        public bool IsScope { get; set; }
        public bool IsFunction { get; set; }
        public bool IsCall { get; set; }
    }

    internal class MemberToken : OpToken
    {
        public MemberToken() { Value = "."; }

        public string Name { get; set; }
    }

    internal class BinaryOperator : Operator<Func<Expression, Expression, Expression>>
    {
        public BinaryOperator(string value, int precedence, bool leftassoc, Func<Expression, Expression, Expression> func, ExpressionType expressionType) : base(value, precedence, leftassoc, func)
        {
            Arguments = 2;
            ExpressionType = expressionType;
        }
    }

    internal class TernaryOperator : Operator<Func<Expression, Expression, Expression, Expression>>
    {
        public TernaryOperator(string value, int precedence, bool leftassoc, Func<Expression, Expression, Expression, Expression> func) : base(value, precedence, leftassoc, func) { Arguments = 3; }
    }

    internal class IndexOperator : Operator<Func<Expression, Expression, Expression>>
    {
        public IndexOperator(string value, int precedence, bool leftassoc, Func<Expression, Expression, Expression> func) : base(value, precedence, leftassoc, func) { }
    }

    internal interface IOperator
    {
        string Value { get; set; }
        int Precedence { get; set; }
        int Arguments { get; set; }
        bool LeftAssoc { get; set; }
        ExpressionType ExpressionType { get; set; }
    }

    internal class Operators : Operator<Func<bool, bool, Expression, string, List<Expression>, Expression>>
    {
        public Operators(string value, int precedence, bool leftassoc, Func<bool, bool, Expression, string, List<Expression>, Expression> func) : base(value, precedence, leftassoc, func) { }
    }

    internal class TernarySeparatorOperator : Operator<Func<Expression, Expression>>
    {
        public TernarySeparatorOperator(string value, int precedence, bool leftassoc, Func<Expression, Expression> func) : base(value, precedence, leftassoc, func) { }
    }

    internal abstract class Operator<T> : IOperator
    {
        protected Operator(string value, int precedence, bool leftassoc, T func)
        {
            Value = value;
            Precedence = precedence;
            LeftAssoc = leftassoc;
            Func = func;
        }

        protected Operator(string value, int precedence, bool leftassoc, T func, ExpressionType expressionType)
        {
            Value = value;
            Precedence = precedence;
            LeftAssoc = leftassoc;
            Func = func;
            ExpressionType = expressionType;
        }

        public T Func { get; set; }
        public string Value { get; set; }
        public int Precedence { get; set; }
        public int Arguments { get; set; }
        public bool LeftAssoc { get; set; }
        public ExpressionType ExpressionType { get; set; }

        public virtual T GetFunc() { return Func; }
    }

    internal class OperatorCollection : Dictionary<string, IOperator>
    {
        private readonly List<char> _firstlookup = new List<char>();

        public new void Add(string key, IOperator op)
        {
            _firstlookup.Add(key[0]);
            base.Add(key, op);
        }

        public bool ContainsFirstKey(char key) { return _firstlookup.Contains(key); }

        public bool IsOperator(string c)
        {
            var i = 0;
            return IsOperator(c, ref i) != null;
        }

        public string IsOperator(string str, ref int p)
        {
            string op = null;

            if (ContainsFirstKey(str[p]))
            {
                string pop;
                if (str.Substring(p).Length > 1)
                {
                    pop = str.Substring(p, 2);
                    if (ContainsKey(pop))
                    {
                        p++;
                        op = pop;
                    }
                }
                if (op == null)
                {
                    pop = str.Substring(p, 1);
                    if (ContainsKey(pop)) op = pop;
                }
            }
            return op;
        }
    }

    internal class OperatorCustomExpressions
    {
        private static readonly Type StringType = typeof(string);
        private static readonly MethodInfo ToStringMethodInfo = typeof(Convert).GetMethod("ToString", new[] { typeof(CultureInfo) });

        public static Expression MemberAccess(bool isFunction, bool isCall, Expression le, string membername, List<Expression> args)
        {
            var argTypes = args.Select(x => x.Type);

            Expression instance = null;
            Type type = null;

            var isDynamic = false;
            var isRuntimeType = false;

            if (le.Type.Name == "RuntimeType")
            {
                isRuntimeType = true;
                type = ((Type)((ConstantExpression)le).Value);
            }
            else
            {
                type = le.Type;
                instance = le;
                isDynamic = type.IsDynamic();
            }

            if (isFunction)
            {
                if (isDynamic)
                {
                    var expArgs = new List<Expression> { instance };

                    expArgs.AddRange(args);

                    if (isCall)
                    {
                        var binderMC = Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, membername, null, type, expArgs.Select(x => CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)));

                        return Expression.Dynamic(binderMC, typeof(void), expArgs);
                    }

                    var binderM = Binder.InvokeMember(CSharpBinderFlags.None, membername, null, type, expArgs.Select(x => CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)));

                    return Expression.Dynamic(binderM, typeof(object), expArgs);
                }
                var mis = MethodResolution.GetApplicableMembers(type, membername, args);
                var methodInfo = (MethodInfo)mis[0];

                if (methodInfo != null)
                {
                    var parameterInfos = methodInfo.GetParameters();

                    foreach (var parameterInfo in parameterInfos)
                    {
                        var index = parameterInfo.Position;

                        args[index] = TypeConversion.Convert(args[index], parameterInfo.ParameterType);
                    }

                    return Expression.Call(instance, methodInfo, args.ToArray());
                }

                var match = MethodResolution.GetExactMatch(type, instance, membername, args) ?? MethodResolution.GetParamsMatch(type, instance, membername, args);

                if (match != null) { return match; }
            }
            else
            {
                if (isDynamic)
                {
                    var binder = Binder.GetMember(CSharpBinderFlags.None, membername, type, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

                    var result = Expression.Dynamic(binder, typeof(object), instance);

                    if (args.Count > 0)
                    {
                        var expArgs = new List<Expression> { result };

                        expArgs.AddRange(args);

                        var indexedBinder = Binder.GetIndex(CSharpBinderFlags.None, type, expArgs.Select(x => CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)));

                        result = Expression.Dynamic(indexedBinder, typeof(object), expArgs);
                    }

                    return result;
                }
                Expression exp = null;

                var propertyInfo = type.GetProperty(membername);
                if (propertyInfo != null) { exp = Expression.Property(instance, propertyInfo); }
                else
                {
                    var fieldInfo = type.GetField(membername);
                    if (fieldInfo != null) { exp = Expression.Field(instance, fieldInfo); }
                }

                if (exp != null)
                {
                    if (args.Count > 0) { return Expression.ArrayAccess(exp, args); }
                    return exp;
                }
            }

            throw new Exception(string.Format("Member not found: {0}.{1}", le.Type.Name, membername));
        }

        private static Expression CallToString(Expression instance) { return Expression.Call(typeof(Convert), "ToString", null, instance, Expression.Constant(CultureInfo.InvariantCulture)); }

        public static Expression Add(Expression le, Expression re)
        {
            if (le.Type == StringType || re.Type == StringType)
            {
                if (le.Type != typeof(string)) le = CallToString(le);
                if (re.Type != typeof(string)) re = CallToString(re);
                return Expression.Add(le, re, StringType.GetMethod("Concat", new[] { le.Type, re.Type }));
            }
            return Expression.Add(le, re);
        }

        public static Expression ArrayAccess(Expression le, Expression re)
        {
            if (le.Type == StringType)
            {
                var mi = StringType.GetMethod("ToCharArray", new Type[] { });
                le = Expression.Call(le, mi);
            }

            return Expression.ArrayAccess(le, re);
        }

        public static Expression TernarySeparator(Expression le) { return le; }
    }

    internal class OpFuncArgs
    {
        public Queue<Token> TempQueue { get; set; }
        public Stack<Expression> ExprStack { get; set; }
        public Token T { get; set; }
        public IOperator Op { get; set; }
        public List<Expression> Args { get; set; }
        public Expression ScopeParam { get; set; }
        public List<string> Types { get; set; }
    }

    internal class OpFuncServiceLocator
    {
        private static readonly OpFuncServiceLocator Instance = new OpFuncServiceLocator();

        private readonly Dictionary<Type, Func<OpFuncArgs, Expression>> _typeActions = new Dictionary<Type, Func<OpFuncArgs, Expression>>();

        private OpFuncServiceLocator()
        {
            _typeActions.Add(typeof(Operators), OpFuncServiceProviders.MethodOperatorFunc);
            _typeActions.Add(typeof(TypeOperator), OpFuncServiceProviders.TypeOperatorFunc);
            _typeActions.Add(typeof(UnaryOperator), OpFuncServiceProviders.UnaryOperatorFunc);
            _typeActions.Add(typeof(BinaryOperator), OpFuncServiceProviders.BinaryOperatorFunc);
            _typeActions.Add(typeof(TernaryOperator), OpFuncServiceProviders.TernaryOperatorFunc);
            _typeActions.Add(typeof(TernarySeparatorOperator), OpFuncServiceProviders.TernarySeparatorOperatorFunc);
        }

        public static Func<OpFuncArgs, Expression> Resolve(Type type) { return Instance.ResolveType(type); }

        private Func<OpFuncArgs, Expression> ResolveType(Type type) { return _typeActions[type]; }
    }

    internal class TypeOperator : Operator<Func<Expression, Type, UnaryExpression>>
    {
        public TypeOperator(string value, int precedence, bool leftassoc, Func<Expression, Type, UnaryExpression> func) : base(value, precedence, leftassoc, func) { Arguments = 1; }
    }

    internal class UnaryOperator : Operator<Func<Expression, UnaryExpression>>
    {
        public UnaryOperator(string value, int precedence, bool leftassoc, Func<Expression, UnaryExpression> func, ExpressionType expressionType) : base(value, precedence, leftassoc, func, expressionType) { Arguments = 1; }
    }

    internal static class TypeExtensions
    {
        public static bool IsDynamic(this Type type) { return type.GetInterfaces().Contains(typeof(IDynamicMetaObjectProvider)) || type == typeof(object); }
    }

    internal class OpFuncServiceProviders
    {
        public static Expression MethodOperatorFunc(OpFuncArgs args)
        {
            var nextToken = ((MemberToken)args.T).Name;
            var le = args.ExprStack.Pop();

            var result = ((Operators)args.Op).Func(args.T.IsFunction, args.T.IsCall, le, nextToken, args.Args);

            return result;
        }

        public static Expression TypeOperatorFunc(OpFuncArgs args)
        {
            var le = args.ExprStack.Pop();
            return ((TypeOperator)args.Op).Func(le, args.T.Type);
        }

        public static Expression UnaryOperatorFunc(OpFuncArgs args)
        {
            var le = args.ExprStack.Pop();
            if (le.Type.IsDynamic()) { return DynamicUnaryOperatorFunc(le, args.Op.ExpressionType); }
            return ((UnaryOperator)args.Op).Func(le);
        }

        public static Expression BinaryOperatorFunc(OpFuncArgs args)
        {
            var re = args.ExprStack.Pop();
            var le = args.ExprStack.Pop();
            if (le.Type.IsDynamic() && re.Type.IsDynamic())
            {
                var expressionType = args.Op.ExpressionType;

                if (expressionType == ExpressionType.OrElse)
                {
                    le = Expression.IsTrue(Expression.Convert(le, typeof(bool)));
                    expressionType = ExpressionType.Or;
                    return Expression.Condition(le, Expression.Constant(true), Expression.Convert(DynamicBinaryOperatorFunc(Expression.Constant(false), re, expressionType), typeof(bool)));
                }

                if (expressionType == ExpressionType.AndAlso)
                {
                    le = Expression.IsFalse(Expression.Convert(le, typeof(bool)));
                    expressionType = ExpressionType.And;
                    return Expression.Condition(le, Expression.Constant(false), Expression.Convert(DynamicBinaryOperatorFunc(Expression.Constant(true), re, expressionType), typeof(bool)));
                }

                return DynamicBinaryOperatorFunc(le, re, expressionType);
            }
            TypeConversion.Convert(ref le, ref re);

            return ((BinaryOperator)args.Op).Func(le, re);
        }

        private static Expression DynamicUnaryOperatorFunc(Expression le, ExpressionType expressionType)
        {
            var expArgs = new List<Expression> { le };

            var binderM = Binder.UnaryOperation(CSharpBinderFlags.None, expressionType, le.Type, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

            return Expression.Dynamic(binderM, typeof(object), expArgs);
        }

        private static Expression DynamicBinaryOperatorFunc(Expression le, Expression re, ExpressionType expressionType)
        {
            var expArgs = new List<Expression> { le, re };

            var binderM = Binder.BinaryOperation(CSharpBinderFlags.None, expressionType, le.Type, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

            return Expression.Dynamic(binderM, typeof(object), expArgs);
        }

        public static Expression TernaryOperatorFunc(OpFuncArgs args)
        {
            var falsy = args.ExprStack.Pop();
            var truthy = args.ExprStack.Pop();
            var condition = args.ExprStack.Pop();
            if (condition.Type != typeof(bool)) { condition = Expression.Convert(condition, typeof(bool)); }
            TypeConversion.Convert(ref falsy, ref truthy);
            return ((TernaryOperator)args.Op).Func(condition, truthy, falsy);
        }

        public static Expression TernarySeparatorOperatorFunc(OpFuncArgs args) { return args.ExprStack.Pop(); }
    }

    internal class MethodResolution
    {
        private static readonly Dictionary<Type, List<Type>> NumConv = new Dictionary<Type, List<Type>> { { typeof(sbyte), new List<Type> { typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } }, { typeof(byte), new List<Type> { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } }, { typeof(short), new List<Type> { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) } }, { typeof(ushort), new List<Type> { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } }, { typeof(int), new List<Type> { typeof(long), typeof(float), typeof(double), typeof(decimal) } }, { typeof(uint), new List<Type> { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } }, { typeof(long), new List<Type> { typeof(float), typeof(double), typeof(decimal) } }, { typeof(ulong), new List<Type> { typeof(float), typeof(double), typeof(decimal) } }, { typeof(char), new List<Type> { typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) } }, { typeof(float), new List<Type> { typeof(double) } } };

        private static readonly Func<MethodInfo, bool> IsVirtual = mi => (mi.Attributes & MethodAttributes.Virtual) != 0;
        private static readonly Func<MethodInfo, bool> HasVTable = mi => (mi.Attributes & MethodAttributes.VtableLayoutMask) != 0;

        private static readonly BindingFlags findFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.OptionalParamBinding | BindingFlags.DeclaredOnly;

        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace) { return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray(); }

        public static Expression GetExactMatch(Type type, Expression instance, string membername, List<Expression> args)
        {
            var argTypes = args.Select(x => x.Type);
            var methodInfo = type.GetMethod(membername, argTypes.ToArray());
            if (methodInfo != null)
            {
                var parameterInfos = methodInfo.GetParameters();
                for (var i = 0; i < parameterInfos.Length; i++) { args[i] = TypeConversion.Convert(args[i], parameterInfos[i].ParameterType); }
                return Expression.Call(instance, methodInfo, args);
            }
            return null;
        }

        public static Expression GetParamsMatch(Type type, Expression instance, string membername, List<Expression> args)
        {
            var methodInfos = type.GetMethods().Where(x => x.Name == membername);
            var matchScore = new List<Tuple<MethodInfo, int>>();

            foreach (var info in methodInfos.OrderByDescending(m => m.GetParameters().Count()))
            {
                var parameterInfos = info.GetParameters();
                var lastParam = parameterInfos.Last();
                var newArgs = args.Take(parameterInfos.Length - 1).ToList();
                var paramArgs = args.Skip(parameterInfos.Length - 1).ToList();

                var i = 0;
                var k = 0;

                foreach (var expression in newArgs)
                {
                    k += TypeConversion.CanConvert(expression.Type, parameterInfos[i].ParameterType);
                    i++;
                }

                if (k > 0) { if (Attribute.IsDefined(lastParam, typeof(ParamArrayAttribute))) { k += paramArgs.Sum(arg => TypeConversion.CanConvert(arg.Type, lastParam.ParameterType.GetElementType())); } }

                matchScore.Add(new Tuple<MethodInfo, int>(info, k));
            }

            var info2 = matchScore.OrderBy(x => x.Item2).FirstOrDefault(x => x.Item2 >= 0);

            if (info2 != null)
            {
                var parameterInfos2 = info2.Item1.GetParameters();
                var lastParam2 = parameterInfos2.Last();
                var newArgs2 = args.Take(parameterInfos2.Length - 1).ToList();
                var paramArgs2 = args.Skip(parameterInfos2.Length - 1).ToList();

                for (var i = 0; i < parameterInfos2.Length - 1; i++) { newArgs2[i] = TypeConversion.Convert(newArgs2[i], parameterInfos2[i].ParameterType); }

                var targetType = lastParam2.ParameterType.GetElementType();

                newArgs2.Add(Expression.NewArrayInit(targetType, paramArgs2.Select(x => TypeConversion.Convert(x, targetType))));
                return Expression.Call(instance, info2.Item1, newArgs2);
            }
            return null;
        }

        public static bool CanConvertType(object value, bool isLiteral, Type from, Type to)
        {
            if (@from.GetHashCode().Equals(to.GetHashCode())) return true;
            if (isLiteral)
            {
                var canConv = false;

                dynamic num = value;
                if (@from == typeof(int))
                {
                    switch (Type.GetTypeCode(to))
                    {
                        case TypeCode.SByte:
                            if (num >= sbyte.MinValue && num <= sbyte.MaxValue) canConv = true;
                            break;
                        case TypeCode.Byte:
                            if (num >= byte.MinValue && num <= byte.MaxValue) canConv = true;
                            break;
                        case TypeCode.Int16:
                            if (num >= short.MinValue && num <= short.MaxValue) canConv = true;
                            break;
                        case TypeCode.UInt16:
                            if (num >= ushort.MinValue && num <= ushort.MaxValue) canConv = true;
                            break;
                        case TypeCode.UInt32:
                            if (num >= uint.MinValue && num <= uint.MaxValue) canConv = true;
                            break;
                        case TypeCode.UInt64:
                            if (num >= 0) canConv = true;
                            break;
                    }
                }
                else if (@from == typeof(long)) { if (to == typeof(ulong)) { if (num >= 0) canConv = true; } }

                if (canConv) return true;
            }

            if (@from == typeof(string))
            {
                if (to == typeof(object)) return true;
                return false;
            }

            if (IsNullableType(to))
            {
                if (IsNullableType(@from))
                {
                    if (value == null) { return true; }
                }

                return CanConvertType(value, isLiteral, Nullable.GetUnderlyingType(@from), Nullable.GetUnderlyingType(to));
            }

            long longTest = -1;
            if (isLiteral && to.IsEnum && long.TryParse(value.ToString(), out longTest)) { if (longTest == 0) return true; }
            if (!@from.IsValueType && !to.IsValueType)
            {
                var irc = ImpRefConv(value, @from, to);
                if (irc.HasValue) return irc.Value;
            }
            try
            {
                double dblTemp;
                decimal decTemp;
                char chrTemp;
                var fromObj = Activator.CreateInstance(@from);

                if (char.TryParse(fromObj.ToString(), out chrTemp) || double.TryParse(fromObj.ToString(), out dblTemp) || decimal.TryParse(fromObj.ToString(), out decTemp))
                {
                    if (NumConv.ContainsKey(@from) && NumConv[@from].Contains(to)) return true;
                    return CrawlThatShit(to.GetHashCode(), @from, new List<int>());
                }
                return CrawlThatShit(to.GetHashCode(), @from, new List<int>());
            }
            catch
            {
                return CrawlThatShit(to.GetHashCode(), @from, new List<int>());
            }
        }

        public static List<MemberInfo> GetApplicableMembers(Type type, string membername, List<Expression> args)
        {
            var results = GetCandidateMembers(type, membername);
            var appMembers = new List<MemberInfo>();
            foreach (var methodInfo in results)
            {
                var isMatch = true;
                var argCount = 0;
                foreach (var pInfo in methodInfo.GetParameters())
                {
                    var haveArg = argCount < args.Count;

                    if (pInfo.IsOut || pInfo.ParameterType.IsByRef)
                    {
                        if (!haveArg) { isMatch = false; }
                        var argTypeStr = args[argCount].Type.FullName;
                        var paramType = methodInfo.GetParameters()[argCount].ParameterType;
                        var paramTypeStr = paramType.ToString().Substring(0, paramType.ToString().Length - 1);

                        if (argTypeStr != paramTypeStr) { isMatch = false; }
                    }
                    else
                    {
                        if (pInfo.IsOptional)
                        {
                            if (haveArg && !CanConvertType(((ConstantExpression)args[argCount]).Value, false, args[argCount].Type, pInfo.ParameterType)) { isMatch = false; }
                        }
                        else if (pInfo.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
                        {
                            for (var j = pInfo.Position; j < args.Count; j++)
                            {
                                if (!CanConvertType(null, false, args[j].Type, pInfo.ParameterType.GetElementType())) { isMatch = false; }
                                argCount++;
                            }
                            break;
                        }
                        else
                        {
                            if (!haveArg || !CanConvertType(null, false, args[argCount].Type, pInfo.ParameterType)) { isMatch = false; }
                        }
                    }

                    if (!isMatch) { break; }
                    argCount++;
                }
                if (isMatch && argCount < args.Count) isMatch = false;
                if (isMatch) appMembers.Add(methodInfo);
            }
            return appMembers;
        }

        public static bool? ImpRefConv(object value, Type from, Type to)
        {
            bool? success = null;
            if (@from == to)
                success = true;
            else if (to == typeof(object))
                success = true;
            else if (value == null)
                success = !to.IsValueType;
            else if (false)
                ;
            else if (@from.IsArray && to.IsArray)
            {
                var sameRank = (@from.GetArrayRank() == to.GetArrayRank());
                var bothRef = (!@from.GetElementType().IsValueType && !to.GetElementType().IsValueType);
                var impConv = ImpRefConv(value, @from.GetElementType(), to.GetElementType());
                success = (sameRank && bothRef && impConv.GetValueOrDefault(false));
            }
            else if (to.IsGenericParameter)
            {
                if (to.GenericParameterAttributes != GenericParameterAttributes.None) { if ((int)(to.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0) {; } }
            }
            else if (@from.IsValueType && !to.IsValueType) { return IsBoxingConversion(@from, to); }
            else if ((@from.IsClass && to.IsClass) || (@from.IsClass && to.IsInterface) || (@from.IsInterface && to.IsInterface))
                success = CrawlThatShit(to.GetHashCode(), @from, new List<int>());
            else if (@from.IsArray && CrawlThatShit(to.GetHashCode(), typeof(Array), new List<int>()))
            {
                return true;
            }
            else if (@from.IsArray && @from.GetArrayRank() == 1 && to.IsGenericType && CrawlThatShit(to.GetHashCode(), typeof(IList<>), new List<int>()))
                success = ImpRefConv(value, @from.GetElementType(), to.GetGenericTypeDefinition());
            return success;
        }

        public static bool CrawlThatShit(int target, Type current, List<int> visitedTypes)
        {
            var curHashCode = current.GetHashCode();
            if (visitedTypes.Contains(curHashCode)) { return false; }
            var found = (curHashCode == target);
            visitedTypes.Add(curHashCode);
            if (!found && current.BaseType != null) { found = CrawlThatShit(target, current.BaseType, visitedTypes); }
            if (!found)
            {
                if (current.GetInterfaces() != null)
                {
                    foreach (var iface in current.GetInterfaces())
                    {
                        if (CrawlThatShit(target, iface, visitedTypes))
                        {
                            found = true;
                            break;
                        }
                    }
                }
            }

            return found;
        }

        public static bool IsNullableType(Type t) { return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>); }

        public static bool IsBoxingConversion(Type from, Type to)
        {
            if (IsNullableType(@from)) { @from = Nullable.GetUnderlyingType(@from); }

            if (to == typeof(ValueType) || to == typeof(object)) { return true; }

            if (CrawlThatShit(to.GetHashCode(), @from, new List<int>())) { return true; }

            if (@from.IsEnum && to == typeof(Enum)) { return true; }
            return false;
        }

        public static List<MethodInfo> GetCandidateMembers(Type type, string membername)
        {
            var results = GetMethodInfos(type, membername);
            while (type != typeof(object))
            {
                type = type.BaseType;
                results.AddRange(GetMethodInfos(type, membername));
            }
            return results;
        }

        public static List<MethodInfo> GetMethodInfos(Type env, string memberName) { return env.GetMethods(findFlags).Where(mi => mi.Name == memberName && (!IsVirtual(mi) || HasVTable(mi))).ToList(); }
    }

    public class Parser
    {
        private readonly Stack<OpToken> _opStack = new Stack<OpToken>();
        private readonly Queue<Token> _tokenQueue = new Queue<Token>();
        private OperatorCollection _operators;
        private string _pstr;
        private int _ptr;

        public Parser()
        {
            Initialize();
            TypeRegistry = new TypeRegistry();
        }

        public Parser(string str)
        {
            Initialize();
            _pstr = str;
        }

        public TypeRegistry TypeRegistry { get; set; }
        public object Global { get; set; }

        public string StringToParse
        {
            get { return _pstr; }
            set
            {
                _pstr = value;
                _tokenQueue.Clear();
                _ptr = 0;
            }
        }

        private void Initialize()
        {
            _operators = new OperatorCollection { { ".", new Operators(".", 12, true, OperatorCustomExpressions.MemberAccess) }, { "!", new UnaryOperator("!", 11, false, Expression.Not, ExpressionType.Not) }, { "*", new BinaryOperator("*", 10, true, Expression.Multiply, ExpressionType.Multiply) }, { "/", new BinaryOperator("/", 10, true, Expression.Divide, ExpressionType.Divide) }, { "%", new BinaryOperator("%", 10, true, Expression.Modulo, ExpressionType.Modulo) }, { "+", new BinaryOperator("+", 9, true, OperatorCustomExpressions.Add, ExpressionType.Add) }, { "-", new BinaryOperator("-", 9, true, Expression.Subtract, ExpressionType.Subtract) }, { "<<", new BinaryOperator("<<", 8, true, Expression.LeftShift, ExpressionType.LeftShift) }, { ">>", new BinaryOperator(">>", 8, true, Expression.RightShift, ExpressionType.RightShift) }, { "<", new BinaryOperator("<", 7, true, Expression.LessThan, ExpressionType.LessThan) }, { ">", new BinaryOperator(">", 7, true, Expression.GreaterThan, ExpressionType.GreaterThan) }, { "<=", new BinaryOperator("<=", 7, true, Expression.LessThanOrEqual, ExpressionType.LessThanOrEqual) }, { ">=", new BinaryOperator(">=", 7, true, Expression.GreaterThanOrEqual, ExpressionType.GreaterThanOrEqual) }, { "==", new BinaryOperator("==", 6, true, Expression.Equal, ExpressionType.Equal) }, { "!=", new BinaryOperator("!=", 6, true, Expression.NotEqual, ExpressionType.NotEqual) }, { "&", new BinaryOperator("&", 5, true, Expression.And, ExpressionType.And) }, { "^", new BinaryOperator("^", 4, true, Expression.ExclusiveOr, ExpressionType.ExclusiveOr) }, { "|", new BinaryOperator("|", 3, true, Expression.Or, ExpressionType.Or) }, { "&&", new BinaryOperator("&&", 2, true, Expression.AndAlso, ExpressionType.AndAlso) }, { "||", new BinaryOperator("||", 1, true, Expression.OrElse, ExpressionType.OrElse) }, { ":", new TernarySeparatorOperator(":", 2, false, OperatorCustomExpressions.TernarySeparator) }, { "=", new BinaryOperator("=", 1, false, Expression.Assign, ExpressionType.Assign) }, { "?", new TernaryOperator("?", 1, false, Expression.Condition) } };
        }

        private bool IsInBounds() { return _ptr < _pstr.Length; }

        public Expression Parse(string expression)
        {
            StringToParse = expression;
            Parse();
            return BuildTree();
        }

        public void Parse(bool isScope = false)
        {
            try
            {
                _tokenQueue.Clear();
                _ptr = 0;

                while (IsInBounds())
                {
                    var op = "";
                    var lastptr = _ptr;
                    if (_pstr[_ptr] != ' ')
                    {
                        if (_pstr[_ptr] == '\'')
                        {
                            var isStringClosed = false;
                            _ptr++;
                            lastptr = _ptr;
                            var tokenbuilder = new StringBuilder();
                            while (IsInBounds())
                            {
                                if (_pstr[_ptr] == '\\')
                                {
                                    tokenbuilder.Append(_pstr.Substring(lastptr, _ptr - lastptr));
                                    var nextchar = _pstr[_ptr + 1];
                                    switch (nextchar)
                                    {
                                        case '\'':
                                        case '\\':
                                            tokenbuilder.Append(nextchar);
                                            break;
                                        default:
                                            throw new Exception("Unrecognized escape sequence");
                                    }
                                    _ptr++;
                                    _ptr++;
                                    lastptr = _ptr;
                                }
                                else if ((_pstr[_ptr] == '\''))
                                {
                                    isStringClosed = true;
                                    break;
                                }
                                else
                                { _ptr++; }
                            }

                            if (!isStringClosed) throw new Exception("Unclosed string literal at " + lastptr);

                            tokenbuilder.Append(_pstr.Substring(lastptr, _ptr - lastptr));
                            var token = tokenbuilder.ToString();
                            _tokenQueue.Enqueue(new Token { Value = token, IsIdent = true, Type = typeof(string) });
                            _ptr++;
                        }
                        else if (_pstr[_ptr] == '#')
                        {
                            var isDateClosed = false;

                            _ptr++;
                            lastptr = _ptr;

                            while (IsInBounds())
                            {
                                _ptr++;
                                if (_pstr[_ptr] == '#')
                                {
                                    isDateClosed = true;
                                    break;
                                }
                            }

                            if (!isDateClosed) throw new Exception("Unclosed date literal at " + lastptr);

                            var token = _pstr.Substring(lastptr, _ptr - lastptr);

                            DateTime dt;
                            if (token == "Now") { dt = DateTime.Now; }
                            else
                            { dt = DateTime.Parse(token); }

                            _tokenQueue.Enqueue(new Token { Value = dt, IsIdent = true, Type = typeof(DateTime) });
                            _ptr++;
                        }
                        else if (_pstr[_ptr] == ',')
                        {
                            var pe = false;

                            while (_opStack.Count > 0)
                            {
                                if ((string)_opStack.Peek().Value == "(")
                                {
                                    var temp = _opStack.Pop();
                                    Token lastToken = _opStack.Peek();
                                    if (lastToken.GetType() == typeof(MemberToken))
                                    {
                                        var lastmember = (MemberToken)lastToken;
                                        if (lastmember != null) lastmember.ArgCount++;
                                    }
                                    _opStack.Push(temp);
                                    pe = true;
                                    break;
                                }
                                var popToken = _opStack.Pop();
                                _tokenQueue.Enqueue(popToken);
                            }

                            if (!pe) { throw new Exception("Parenthesis mismatch"); }

                            _ptr++;
                        }
                        else if (_pstr[_ptr] == '.')
                        {
                            if (_opStack.Count > 0)
                            {
                                var sc = _opStack.Peek();
                                if ((string)sc.Value == ".")
                                {
                                    var popToken = _opStack.Pop();
                                    _tokenQueue.Enqueue(popToken);
                                }
                            }

                            _opStack.Push(new MemberToken());
                            _ptr++;
                        }
                        else if (HelperMethods.IsHexStart(_pstr, _ptr))
                        {
                            var isNeg = false;
                            if (_pstr[_ptr] == '-')
                            {
                                isNeg = true;
                                _ptr++;
                                lastptr = _ptr;
                            }
                            _ptr += 2;
                            while (IsInBounds() && (HelperMethods.IsHex(_pstr, _ptr) || _pstr[_ptr] == 'L')) { _ptr++; }

                            var token = _pstr.Substring(lastptr, _ptr - lastptr);

                            var ntype = typeof(int);
                            object val = null;

                            if (token.EndsWith("L"))
                            {
                                ntype = typeof(long);
                                token = token.Remove(token.Length - 1, 1);
                            }

                            switch (ntype.Name)
                            {
                                case "Int32":
                                    val = isNeg ? -Convert.ToInt32(token, 16) : Convert.ToInt32(token, 16);
                                    break;
                                case "Int64":
                                    val = isNeg ? -Convert.ToInt64(token, 16) : Convert.ToInt64(token, 16);
                                    break;
                            }

                            _tokenQueue.Enqueue(new Token { Value = val, IsIdent = true, Type = ntype });
                        }
                        else if (HelperMethods.IsANumber(_pstr, _ptr))
                        {
                            var isDecimal = false;
                            var suffixLength = 0;
                            while (IsInBounds())
                            {
                                if (_pstr[_ptr] == 'l' || _pstr[_ptr] == 'L' || _pstr[_ptr] == 'u' || _pstr[_ptr] == 'U')
                                {
                                    if (isDecimal) throw new Exception("Expected end of decimal literal");
                                    if (suffixLength == 1)
                                    {
                                        _ptr++;
                                        break;
                                    }
                                    suffixLength++;
                                }
                                else if (_pstr[_ptr] == '.')
                                {
                                    if (isDecimal) break;
                                    isDecimal = true;
                                }
                                else if (_pstr[_ptr] == 'd' || _pstr[_ptr] == 'D' || _pstr[_ptr] == 'f' || _pstr[_ptr] == 'F' || _pstr[_ptr] == 'm' || _pstr[_ptr] == 'M')
                                {
                                    suffixLength++;
                                    _ptr++;
                                    break;
                                }
                                else if (!HelperMethods.IsANumber(_pstr, _ptr)) { break; }
                                _ptr++;
                            }

                            var token = _pstr.Substring(lastptr, _ptr - lastptr);
                            var suffix = "";

                            Type ntype = null;
                            object val = null;

                            if (suffixLength > 0)
                            {
                                suffix = token.Substring(token.Length - suffixLength);
                                token = token.Substring(0, token.Length - suffixLength);

                                switch (suffix.ToLower())
                                {
                                    case "d":
                                        ntype = typeof(double);
                                        val = double.Parse(token, CultureInfo.InvariantCulture);
                                        break;
                                    case "f":
                                        ntype = typeof(float);
                                        val = float.Parse(token, CultureInfo.InvariantCulture);
                                        break;
                                    case "m":
                                        ntype = typeof(decimal);
                                        val = decimal.Parse(token, CultureInfo.InvariantCulture);
                                        break;
                                    case "l":
                                        ntype = typeof(long);
                                        val = long.Parse(token, CultureInfo.InvariantCulture);
                                        break;
                                    case "u":
                                        ntype = typeof(uint);
                                        val = uint.Parse(token, CultureInfo.InvariantCulture);
                                        break;
                                    case "ul":
                                    case "lu":
                                        ntype = typeof(ulong);
                                        val = ulong.Parse(token, CultureInfo.InvariantCulture);
                                        break;
                                }
                            }
                            else
                            {
                                if (isDecimal)
                                {
                                    ntype = typeof(double);
                                    val = double.Parse(token, CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    ntype = typeof(int);
                                    val = int.Parse(token, CultureInfo.InvariantCulture);
                                }
                            }

                            _tokenQueue.Enqueue(new Token { Value = val, IsIdent = true, Type = ntype });
                        }
                        else if (HelperMethods.IsAlpha(_pstr[_ptr]) || (_pstr[_ptr] == '_'))
                        {
                            _ptr++;

                            while (IsInBounds() && (HelperMethods.IsAlpha(_pstr[_ptr]) || (_pstr[_ptr] == '_') || HelperMethods.IsNumeric(_pstr, _ptr))) { _ptr++; }

                            var token = _pstr.Substring(lastptr, _ptr - lastptr);

                            MemberToken mToken = null;

                            if (_opStack.Count > 0)
                            {
                                var opToken = _opStack.Peek();
                                if (opToken.GetType() == typeof(MemberToken)) mToken = (MemberToken)opToken;
                            }

                            if ((mToken != null) && (mToken.Name == null)) { mToken.Name = token; }
                            else if (TypeRegistry.ContainsKey(token))
                            {
                                if (TypeRegistry[token].GetType().Name == "RuntimeType") { _tokenQueue.Enqueue(new Token { Value = ((Type)TypeRegistry[token]).UnderlyingSystemType, IsType = true }); }
                                else
                                { _tokenQueue.Enqueue(new Token { Value = TypeRegistry[token], IsType = true }); }
                            }
                            else
                            {
                                if ((token == "null")) { _tokenQueue.Enqueue(new Token { Value = null, IsIdent = true, Type = typeof(object) }); }
                                else if ((token == "true") || (token == "false")) { _tokenQueue.Enqueue(new Token { Value = bool.Parse(token), IsIdent = true, Type = typeof(bool) }); }
                                else
                                {
                                    if (Global != null) { _tokenQueue.Enqueue(new Token { Value = Global, IsType = true }); }
                                    else
                                    {
                                        if (isScope) { _tokenQueue.Enqueue(new Token { IsScope = true }); }
                                        else
                                        {
                                            throw new Exception(string.Format("Unknown type or identifier '{0}'", token));
                                        }
                                    }

                                    if (_opStack.Count > 0)
                                    {
                                        var sc = _opStack.Peek();
                                        if ((string)sc.Value == ".")
                                        {
                                            var popToken = _opStack.Pop();
                                            _tokenQueue.Enqueue(popToken);
                                        }
                                    }

                                    _opStack.Push(new MemberToken());
                                    _ptr -= token.Length;
                                }
                            }
                        }
                        else if (_pstr[_ptr] == '[')
                        {
                            _opStack.Push(new OpToken { Value = "[", Ptr = _ptr + 1 });
                            _ptr++;
                        }
                        else if (_pstr[_ptr] == ']')
                        {
                            var pe = false;
                            while (_opStack.Count > 0)
                            {
                                var sc = _opStack.Peek();
                                if ((string)sc.Value == "[")
                                {
                                    var temp = _opStack.Pop();
                                    if (_opStack.Count > 0)
                                    {
                                        Token lastToken = _opStack.Peek();
                                        if (lastToken.GetType() == typeof(MemberToken))
                                        {
                                            var lastmember = (MemberToken)lastToken;
                                            if (_pstr.Substring(sc.Ptr, _ptr - sc.Ptr).Trim().Length > 0) lastmember.ArgCount++;
                                        }
                                    }
                                    _opStack.Push(temp);
                                    pe = true;
                                    break;
                                }
                                var popToken = _opStack.Pop();
                                _tokenQueue.Enqueue(popToken);
                            }
                            if (!pe) { throw new Exception("Parenthesis mismatch"); }
                            var lopToken = _opStack.Pop();
                            _ptr++;
                        }
                        else if (_pstr[_ptr] == '(')
                        {
                            var curptr = _ptr;
                            var depth = 0;
                            var containsComma = false;

                            while (IsInBounds())
                            {
                                if (_pstr[_ptr] == '(') depth++;
                                if (_pstr[_ptr] == ')') depth--;
                                if (_pstr[_ptr] == ',') containsComma = true;
                                _ptr++;
                                if (depth == 0) break;
                            }

                            _ptr--;

                            if (depth != 0) throw new Exception("Parenthesis mismatch");

                            var token = _pstr.Substring(lastptr + 1, _ptr - lastptr - 1).Trim();

                            Type t;

                            var isCast = false;

                            if (!containsComma)
                            {
                                if (TypeRegistry.ContainsKey(token))
                                {
                                    _tokenQueue.Enqueue(new Token { Value = "(" + token + ")", IsCast = true, Type = (Type)TypeRegistry[token] });
                                    isCast = true;
                                }
                                else if ((t = Type.GetType(token)) != null)
                                {
                                    _tokenQueue.Enqueue(new Token { Value = "(" + t.Name + ")", IsCast = true, Type = t });
                                    isCast = true;
                                }
                            }

                            if (!isCast)
                            {
                                _opStack.Push(new OpToken { Value = "(", Ptr = curptr + 1 });

                                _ptr = curptr + 1;
                            }
                        }
                        else if (_pstr[_ptr] == ')')
                        {
                            var pe = false;
                            while (_opStack.Count > 0)
                            {
                                var sc = _opStack.Peek();
                                if ((string)sc.Value == "(")
                                {
                                    var temp = _opStack.Pop();
                                    if (_opStack.Count > 0)
                                    {
                                        Token lastToken = _opStack.Peek();
                                        if (lastToken.GetType() == typeof(MemberToken))
                                        {
                                            var lastmember = (MemberToken)lastToken;
                                            if (_pstr.Substring(sc.Ptr, _ptr - sc.Ptr).Trim().Length > 0) lastmember.ArgCount++;
                                        }
                                    }
                                    _opStack.Push(temp);
                                    pe = true;
                                    break;
                                }
                                var popToken = _opStack.Pop();
                                _tokenQueue.Enqueue(popToken);
                            }
                            if (!pe) { throw new Exception("Parenthesis mismatch"); }
                            _opStack.Pop();
                            if (_opStack.Count > 0)
                            {
                                var popToken = _opStack.Peek();
                                if ((string)popToken.Value == ".")
                                {
                                    popToken = _opStack.Pop();
                                    popToken.IsFunction = true;
                                    _tokenQueue.Enqueue(popToken);
                                }
                            }
                            _ptr++;
                        }
                        else if ((op = _operators.IsOperator(_pstr, ref _ptr)) != null)
                        {
                            while (_opStack.Count > 0)
                            {
                                var sc = _opStack.Peek();

                                if (_operators.IsOperator((string)sc.Value) && ((_operators[op].LeftAssoc && (_operators[op].Precedence <= _operators[(string)sc.Value].Precedence)) || (_operators[op].Precedence < _operators[(string)sc.Value].Precedence)))
                                {
                                    var popToken = _opStack.Pop();
                                    _tokenQueue.Enqueue(popToken);
                                }
                                else
                                { break; }
                            }

                            _opStack.Push(new OpToken { Value = op });
                            _ptr++;
                        }
                        else
                        { throw new Exception("Unexpected token '" + _pstr[_ptr] + "'"); }
                    }
                    else
                    { _ptr++; }
                }
                while (_opStack.Count > 0)
                {
                    var sc = _opStack.Peek();
                    if ((string)sc.Value == "(" || (string)sc.Value == ")") { throw new Exception("Paren mismatch"); }

                    sc = _opStack.Pop();
                    _tokenQueue.Enqueue(sc);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Parser error at position {0}: {1}", _ptr, ex.Message), ex);
            }
        }

        public Expression BuildTree(Expression scopeParam = null, bool isCall = false)
        {
            if (_tokenQueue.Count == 0) Parse(scopeParam != null);
            var tempQueue = new Queue<Token>(_tokenQueue);
            var exprStack = new Stack<Expression>();
            var args = new List<Expression>();
            var isCastPending = -1;
            Type typeCast = null;
            while (tempQueue.Count > 0)
            {
                var t = tempQueue.Dequeue();
                t.IsCall = isCall && tempQueue.Count == 0;
                if (isCastPending > -1) isCastPending--;
                if (isCastPending == 0)
                {
                    exprStack.Push(Expression.Convert(exprStack.Pop(), typeCast));
                    isCastPending = -1;
                }
                if (t.IsIdent)
                {
                    exprStack.Push(Expression.Constant(t.Value, t.Type));
                }
                else if (t.IsType) { exprStack.Push(Expression.Constant(t.Value)); }
                else if (t.IsScope)
                {
                    if (scopeParam == null) { throw new Exception(string.Format("Unexpected identifier {0} or scope empty", t.Value)); }
                    exprStack.Push(scopeParam);
                }
                else if (t.IsOperator)
                {
                    Expression result = null;
                    var op = _operators[(string)t.Value];
                    var opfunc = OpFuncServiceLocator.Resolve(op.GetType());
                    for (var i = 0; i < t.ArgCount; i++) { args.Add(exprStack.Pop()); }
                    args.Reverse();
                    result = opfunc(new OpFuncArgs { TempQueue = tempQueue, ExprStack = exprStack, T = t, Op = op, Args = args, ScopeParam = scopeParam, Types = new List<string> { "System.Linq" } });
                    args.Clear();
                    exprStack.Push(result);
                }
                else if (t.IsCast)
                {
                    isCastPending = 2;
                    typeCast = t.Type;
                }
            }
            if (exprStack.Count == 1)
            {
                var pop = exprStack.Pop();
                return pop;
            }
            throw new Exception("Invalid expression");
        }
    }

    internal class TypeConversion
    {
        private static readonly TypeConversion Instance = new TypeConversion();
        private readonly Dictionary<Type, int> _typePrecedence;
        private TypeConversion() { _typePrecedence = new Dictionary<Type, int> { { typeof(object), 0 }, { typeof(bool), 1 }, { typeof(byte), 2 }, { typeof(int), 3 }, { typeof(short), 4 }, { typeof(long), 5 }, { typeof(float), 6 }, { typeof(double), 7 } }; }
        internal static void Convert(ref Expression le, ref Expression re)
        {
            if (Instance._typePrecedence.ContainsKey(le.Type) && Instance._typePrecedence.ContainsKey(re.Type))
            {
                if (Instance._typePrecedence[le.Type] > Instance._typePrecedence[re.Type]) re = Expression.Convert(re, le.Type);
                if (Instance._typePrecedence[le.Type] < Instance._typePrecedence[re.Type]) le = Expression.Convert(le, re.Type);
            }
        }
        internal static Expression Convert(Expression le, Type type)
        {
            if (Instance._typePrecedence.ContainsKey(le.Type) && Instance._typePrecedence.ContainsKey(type)) { if (Instance._typePrecedence[le.Type] < Instance._typePrecedence[type]) return Expression.Convert(le, type); }
            return le;
        }
        internal static int CanConvert(Type from, Type to)
        {
            if (Instance._typePrecedence.ContainsKey(from) && Instance._typePrecedence.ContainsKey(to)) { return Instance._typePrecedence[to] - Instance._typePrecedence[from]; }
            if (@from == to) return 0;
            if (to.IsAssignableFrom(@from)) return 1;
            return -1;
        }
    }
    public class TypeRegistry : Dictionary<string, object>
    {
        public TypeRegistry()
        {
            Add("bool", typeof(bool));
            Add("byte", typeof(byte));
            Add("char", typeof(char));
            Add("int", typeof(int));
            Add("decimal", typeof(decimal));
            Add("double", typeof(double));
            Add("float", typeof(float));
            Add("object", typeof(object));
            Add("string", typeof(string));
        }
        public void RegisterDefaultTypes()
        {
            Add("DateTime", typeof(DateTime));
            Add("Convert", typeof(Convert));
            Add("Math", typeof(Math));
        }
    }
}