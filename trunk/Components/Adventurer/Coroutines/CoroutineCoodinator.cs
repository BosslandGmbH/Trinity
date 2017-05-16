using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Zeta.Common;

namespace Trinity.Components.Adventurer.Coroutines
{
    public static class CoroutineCoodinator
    {
        private static ICoroutine _current;

        public static ICoroutine Current
        {
            get
            {
                return _current;
            }
            set
            {
                if (!Equals(_current, value))
                {
                    if (History.Any(i => Equals(i, value)))
                    {
                        History.Prune(i => Equals(i, value));
                    }
                    else
                    {
                        Core.Logger.Debug(LogCategory.TraceCoroutines,
                            Current == null
                                ? $"Coroutine: {value?.GetType().Name}"
                                : $"Coroutine: {_current?.GetType().Name} >> {value?.GetType().Name}");

                        History.Push(value);                        
                    }
                    _current = value;
                }
            }
        }


        private static bool Equals(ICoroutine a, ICoroutine b)
        {
            return a?.Id == b?.Id || a?.GetType().Name == b?.GetType().Name;
        }

        public static DropOutStack<ICoroutine> History { get; } = new DropOutStack<ICoroutine>(20);

        public static string Trace()
        {
            return History.Aggregate("Coroutine StackTrace", (s, c) => ">" + s + c + Environment.NewLine);
        }
    }

    public class DropOutStack<T> : IEnumerable<T>
    {
        private T[] _items;
        private int _top;
        private int _capacity;

        public DropOutStack(int capacity)
        {
            _capacity = capacity;
            _items = new T[capacity];
        }

        public void Push(T item)
        {
            _items[_top] = item;
            _top = (_top + 1) % _items.Length;
        }

        public void Prune(Func<T, bool> selector)
        {
            var items = _items.ToList();
            var newItems = new T[_capacity];
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                newItems[i] = item;
                if (selector(item))
                {
                    _items = newItems.ToArray();
                    _top = i + 1;
                    break;
                }
            }
        }

        public T Peek(int depth = 0) => (_top - 1 - depth) > 0 && (_top - 1 - depth) < _capacity ? _items[_top - 1 - depth] : default(T);

        public T Pop()
        {
            _top = (_items.Length + _top - 1) % _items.Length;
            return _items[_top];
        }

        public IEnumerator<T> GetEnumerator() => _items.ToList().Where(i => i != null).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}




//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using Trinity.Framework;
//using Trinity.Framework.Helpers;
//using Zeta.Common;

//namespace Trinity.Components.Adventurer.Coroutines
//{
//    public static class CoroutineCoodinator
//    {
//        private static ICoroutine _current;

//        public static ICoroutine Current
//        {
//            get
//            {
//                return _current;
//            }
//            set
//            {
//                if (!Equals(_current, value))
//                {
//                    if (Equals(History.Peek(1), value))
//                    {
//                        //Core.Logger.Debug(LogCategory.TraceCoroutines, $"Coroutine return to parent: {_current?.GetType().Name} << {value?.GetType().Name}");
//                        History.Pop();
//                    }
//                    else
//                    {
//                        Core.Logger.Debug(LogCategory.TraceCoroutines,
//                            Current != null
//                                ? $"Coroutine: {value?.GetType().Name}"
//                                : $"Coroutine: {_current?.GetType().Name} >> {value?.GetType().Name}");

//                        History.Push(value);
//                    }
//                    _current = value;                    
//                }
//            }
//        }

//        private static bool Equals(ICoroutine a, ICoroutine b)
//        {
//            return a?.Id == b?.Id || a?.GetType().Name == b?.GetType().Name;
//        }

//        public static DropOutStack<ICoroutine> History { get; } = new DropOutStack<ICoroutine>(20);

//        public static string Trace()
//        {
//            return History.Aggregate("Coroutine StackTrace", (s, c) => ">" + s + c + Environment.NewLine);
//        }
//    }

//    public class DropOutStack<T> : IEnumerable<T>
//    {
//        private T[] _items;
//        private int _top;
//        private int _capacity;

//        public DropOutStack(int capacity)
//        {
//            _capacity = capacity;
//            _items = new T[capacity];
//        }

//        public void Push(T item)
//        {
//            _items[_top] = item;
//            _top = (_top + 1) % _items.Length;
//        }

//        public void Prune(Func<T, bool> selector)
//        {
//            var items = _items.ToList();
//            var newItems = new List<T>();
//            for (var index = 0; index < items.Count; index++)
//            {
//                var item = items[index];
//                if (selector(item))
//                    break;

//                newItems[index] = item;
//            }
//            _top = newItems.Count - 1;
//            _items = newItems.ToArray();
//            _capacity = newItems.Count;
//        }

//        public T Peek(int depth = 0) => _top - depth > 0 && _top - depth < _capacity
//            ? _items[_top - depth]
//            : _items[_top];

//        public T Pop()
//        {
//            _top = (_items.Length + _top - 1) % _items.Length;
//            return _items[_top];
//        }

//        public IEnumerator<T> GetEnumerator()
//        {
//            var items = _items.ToList();
//            items.Reverse();
//            return items.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//    }
//}



