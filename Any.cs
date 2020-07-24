using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace System
{
    /// <summary>
    /// Cho một thứ gì đó bất kỳ, một cấu trúc tự do
    /// </summary>
    public class Any : DynamicObject
    {
        public enum Types
        {
            Object,
            Value,
            Array
        }
        #region contrustor
        public Any()
        {

        }
        public Any(object Object)
        {


            From(Object);

        }
        #endregion

        #region Instance
        /// <summary>
        /// true: Any này biểu diễn cho 1 giá trị
        /// </summary>
        private Types Type = Types.Object;
        private object _Value = null;
        private bool _Empty = true;
        private List<Any> _Arr = null;
        /// <summary>
        /// of Any
        /// </summary>
        /// <returns></returns>
        public object GetAbsValue()
        {
            object v = _Value;
            while (v is Any)
            {
                v = (v as Any).GetAbsValue();
            }


            return v;
        }
        /// <summary>
        /// ofAny
        /// </summary>
        /// <param name="Object"></param>
        public void From(object Object)
        {
            if (Object is IList)
            {
                this.Type = Types.Array;
                var l = Object as IList;
                _Arr = new List<Any>();
                for (var i = 0; i < l.Count; i++)
                {
                    _Arr.Add(new Any(l[i]));
                }
            }
            else
            {
                this._Value = Object;
                this.Type = Types.Value;
                _Empty = false;
            }
        }
        /// <summary>
        /// ofAny List`Any
        /// </summary>
        /// <returns></returns>
        public List<Any> GetArray()
        {
            return _Arr;
        }
        /// <summary>
        /// ofAny
        /// </summary>
        /// <param name="any"></param>
        public void Push(Any any)
        {
            if (_Arr == null) _Arr = new List<Any>();
            _Empty = false;
            _Arr.Add(any);
        }

        public bool IsEmpty()
        {
            return _Empty;
        }

        // The inner dictionary.
        Dictionary<string, object> dictionary
            = new Dictionary<string, object>();

        // This property returns the number of elements
        // in the inner dictionary.
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        // If you try to get a value of a property
        // not defined in the class, this method is called.
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();



            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            var rs = dictionary.TryGetValue(name, out result);

            if (rs == false && result == null)
            {
                result = new Any();
                dictionary.Add(name, result);
                _Empty = false;
                return true;
            }

            return rs;
        }

        private Any ConvertToAny(object input)
        {
            if (input is Any)
            {
                return input as Any;
            }

            return new Any(input);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            dictionary[binder.Name.ToLower()] = ConvertToAny(value);
            Type = Types.Object;
            _Empty = false;
            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }

        public Any this[string key]
        {
            get
            {

                object a = null;
                if (dictionary != null)
                {
                    dictionary.TryGetValue(key, out a);
                }
                var b = a == null ? null : a as Any;
                return b ?? new Any();
            }
            set
            {
                dictionary[key] = ConvertToAny(value);
            }
        }

        public Any this[int index]
        {
            get
            {
                if (Type == Types.Array && _Arr != null && _Arr.Count > index)
                {
                    return _Arr[index];
                }
                else
                {
                    return new Any();
                }
            }
            set
            {
                if (Type == Types.Array && _Arr != null && _Arr.Count > index)
                {
                    _Arr[index] = new Any(value); ;
                }
                else
                {

                }
            }
        }


        public

        #endregion

        #region Static

        static object ExportObject(object v)
        {

            if (v is IList)
            {
                var l = v as IList;
                var n = new object[l.Count];

                for (var i = 0; i < l.Count; i++)
                {
                    n[i] = ExportObject(l[i]);
                }
                return n;

            }
            else if (v is Any)
            {
                var a = v as Any;

                switch (a.Type)
                {

                    case Types.Array:
                        var l = a.GetArray();
                        var arr = l == null ? new object[0] : from x in l
                                                              select ExportObject(x);
                        return arr;
                    case Types.Value:
                        return a.GetAbsValue();

                    case Types.Object:
                    default:

                        var d = new Dictionary<string, object>();
                        foreach (var k in a.dictionary)
                        {
                            d.Add(k.Key, ExportObject(k.Value));
                        }
                        return d;

                }

            }
            else
            {
                return v;
            }
        }
        public static object Export(object any)
        {

            return ExportObject(any);


        }
        public static dynamic New()
        {
            dynamic d = new Any();
            return d;
        }

        public static Any FromJsonToken(JToken token)
        {
            Any d = new Any();
            object v = null;
            switch (token.Type)
            {
                case JTokenType.None:
                    v = null;
                    break;
                //
                case JTokenType.Object:
                    foreach (var c in token.Children())
                    {
                        var n = c.Path.Split('.').LastOrDefault();
                        d[n] = FromJsonToken(c);
                    }
                    return d;

                //Array
                case JTokenType.Array:
                    d.Type = Types.Array;
                    foreach (var jc in token.Children())
                    {
                        d.Push(FromJsonToken(jc));
                    }
                    return d;


                case JTokenType.Constructor:
                    v = null;
                    break;
                case JTokenType.Property:
                    JProperty jp = token as JProperty;

                    return FromJsonToken(jp.Value);
                case JTokenType.Comment:
                    v = null;
                    break;
                case JTokenType.Integer:
                    v = token.Value<int>();
                    break;
                case JTokenType.Float:
                    v = token.Value<double>();
                    break;
                case JTokenType.String:
                    v = token.Value<string>();
                    break;
                case JTokenType.Boolean:
                    v = token.Value<bool>();
                    break;
                case JTokenType.Null:
                    v = null;
                    break;
                case JTokenType.Undefined:
                    v = "undefined";
                    break;

                case JTokenType.Date:
                    v = token.Value<DateTime>();
                    break;
                case JTokenType.Raw:
                case JTokenType.Bytes:
                    break;
                case JTokenType.Guid:
                    v = token.Value<Guid>();
                    break;
                case JTokenType.Uri:
                    v = token.Value<Uri>();
                    break;
                case JTokenType.TimeSpan:
                    v = token.Value<TimeSpan>();
                    break;
                default:
                    return d;
            }

            d.From(v);
            return d;
        }

        public static Any FromJsonString(string json)
        {

            JToken jt = JToken.Parse(json);
            return FromJsonToken(jt);
        }

        public static Any FromType(Type type)
        {
            return New();
        }

        public static List<Any> Select(Any any, string selector, Func<Any, int, int> onEach)
        {
            List<Any> lst = new List<Any>();
            var segs = selector.Split('.');

            List<Any> temps = new List<Any>();

            Func<Any, string, int, List<Any>> func = (Any a, string name, int _index) =>
              {
                  Any a1 = null;
                  if (_index > -1)
                  {
                      if (a.Type == Types.Array)
                      {
                          a1 = a[name];

                      }
                  }
                  else
                  {
                      a1 = a[name];

                  }
                  if (a1 != null && !a1.IsEmpty())
                  {
                      if (a1.Type == Types.Array)
                      {
                          return a1.GetArray();
                      }
                      else
                          return new List<Any>() { a1 };
                  }
                  return new List<Any>() { };

              };

            for (var i = 0; i < segs.Length; i++)
            {
                var ns = segs[i].Split('[', ']');
                var name = ns[0];
                var index = -1;
                if (ns.Length > 1)
                {
                    index = ParseInteger(ns[1]);
                    if (index.ToString() != ns[1]) index = -1;
                }

                if (i == 0)
                {

                    temps.AddRange(func(any, name, index));


                }
                else if (i == segs.Length - 1)
                {
                    foreach (var x in temps)
                    {
                        lst.AddRange(func(x, name, index));
                    }
                }
                else
                {
                    var _temps = new List<Any>();

                    foreach (var x in temps)
                    {
                        _temps.AddRange(func(x, name, index));
                    }
                    if (_temps.Count == 0) break;


                    temps = _temps;
                }
            }
            if (onEach != null)
            {
                for (var i = 0; i < lst.Count; i++)
                {
                    onEach(lst[i], i);
                }
            }

            return lst;
        }
        #endregion

        #region Private Static
        private static int ParseInteger(object input)
        {
            try
            {
                if (input == null) return 0;
                int a = 0;
                int.TryParse(input.ToString(), out a);
                return a;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region Operator
        public static implicit operator double(Any a)
        {
            try
            {
                var v = a.GetAbsValue();
                double c = 0;
                double.TryParse(v ==null ? "0": v.ToString(), out c);
                return c;
            }
            catch
            {
                return 0;
            }
        }
        public static explicit operator Any(double b)
        {
            return new Any(b);
        }
        //public static implicit operator string(Any a)
        //{
        //    try
        //    {
        //        var v = a.GetAbsValue();
        //        return v == null ? null : v.ToString();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        //public static explicit operator Any(string b)
        //{
        //    return new Any(b);
        //}


        #endregion

        public override string ToString()
        {
            return _Value ==null ? null: _Value.ToString();
        }
    }
}
