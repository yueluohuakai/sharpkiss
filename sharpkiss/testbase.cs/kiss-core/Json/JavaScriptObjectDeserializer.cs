﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Kiss.Json
{
    internal class JavaScriptObjectDeserializer
    {
        // Fields
        private int _depthLimit;
        internal JavaScriptString _s;
        private JavaScriptSerializer _serializer;
        private const string DateTimePrefix = "\"\\/Date(";
        private const int DateTimePrefixLength = 8;

        // Methods
        private JavaScriptObjectDeserializer(string input, int depthLimit, JavaScriptSerializer serializer)
        {
            this._s = new JavaScriptString(input);
            this._depthLimit = depthLimit;
            this._serializer = serializer;
        }

        private void AppendCharToBuilder(char? c, StringBuilder sb)
        {
            if (((c == '"') || (c == '\'')) || (c == '/'))
            {
                sb.Append(c);
            }
            else if (c == 'b')
            {
                sb.Append('\b');
            }
            else if (c == 'f')
            {
                sb.Append('\f');
            }
            else if (c == 'n')
            {
                sb.Append('\n');
            }
            else if (c == 'r')
            {
                sb.Append('\r');
            }
            else if (c == 't')
            {
                sb.Append('\t');
            }
            else
            {
                if (c != 'u')
                {
                    throw new ArgumentException(this._s.GetDebugString(""));
                }
                sb.Append((char)int.Parse(this._s.MoveNext(4), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
            }
        }

        internal static object BasicDeserialize(string input, int depthLimit, JavaScriptSerializer serializer)
        {
            JavaScriptObjectDeserializer deserializer = new JavaScriptObjectDeserializer(input, depthLimit, serializer);
            object obj2 = deserializer.DeserializeInternal(0);
            char? nextNonEmptyChar = deserializer._s.GetNextNonEmptyChar();
            int? nullable3 = nextNonEmptyChar.HasValue ? new int?(nextNonEmptyChar.GetValueOrDefault()) : null;
            if (nullable3.HasValue)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "", new object[] { deserializer._s.ToString() }));
            }
            return obj2;
        }

        private char CheckQuoteChar(char? c)
        {
            if (c == '\'')
            {
                return c.Value;
            }
            if (c != '"')
            {
                throw new ArgumentException(this._s.GetDebugString(""));
            }
            return '"';
        }

        private IDictionary<string, object> DeserializeDictionary(int depth)
        {
            IDictionary<string, object> dictionary = null;
            char? nextNonEmptyChar;
            char? nullable8;
            char? nullable11;
            if (this._s.MoveNext() != '{')
            {
                throw new ArgumentException(this._s.GetDebugString(""));
            }
        Label_018D:
            nullable8 = nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            int? nullable10 = nullable8.HasValue ? new int?(nullable8.GetValueOrDefault()) : null;
            if (nullable10.HasValue)
            {
                this._s.MovePrev();
                if (nextNonEmptyChar == ':')
                {
                    throw new ArgumentException(this._s.GetDebugString(""));
                }
                string str = null;
                if (nextNonEmptyChar != '}')
                {
                    str = this.DeserializeMemberName();
                    if (string.IsNullOrEmpty(str))
                    {
                        throw new ArgumentException(this._s.GetDebugString(""));
                    }
                    if (this._s.GetNextNonEmptyChar() != ':')
                    {
                        throw new ArgumentException(this._s.GetDebugString(""));
                    }
                }
                if (dictionary == null)
                {
                    dictionary = new DynamicDictionary();
                    if (string.IsNullOrEmpty(str))
                    {
                        nextNonEmptyChar = this._s.GetNextNonEmptyChar();
                        goto Label_01CB;
                    }
                }
                object obj2 = this.DeserializeInternal(depth);
                dictionary[str] = obj2;
                nextNonEmptyChar = this._s.GetNextNonEmptyChar();
                if (nextNonEmptyChar != '}')
                {
                    if (nextNonEmptyChar != ',')
                    {
                        throw new ArgumentException(this._s.GetDebugString(""));
                    }
                    goto Label_018D;
                }
            }
        Label_01CB:
            nullable11 = nextNonEmptyChar;
            if ((nullable11.GetValueOrDefault() != '}') || !nullable11.HasValue)
            {
                throw new ArgumentException(this._s.GetDebugString(""));
            }
            return dictionary;
        }

        private dynamic DeserializeInternal(int depth)
        {
            if (++depth > this._depthLimit)
            {
                throw new ArgumentException(this._s.GetDebugString(""));
            }
            char? nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            char? nullable2 = nextNonEmptyChar;
            int? nullable4 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault()) : null;
            if (!nullable4.HasValue)
            {
                return null;
            }
            this._s.MovePrev();
            if (this.IsNextElementDateTime())
            {
                return this.DeserializeStringIntoDateTime();
            }
            if (IsNextElementObject(nextNonEmptyChar))
            {
                IDictionary<string, object> o = this.DeserializeDictionary(depth);
                if (o.ContainsKey("__type"))
                {
                    return ObjectConverter.ConvertObjectToType(o, null, this._serializer);
                }
                return o;
            }
            if (IsNextElementArray(nextNonEmptyChar))
            {
                return this.DeserializeList(depth);
            }
            if (IsNextElementString(nextNonEmptyChar))
            {
                return this.DeserializeString();
            }
            return this.DeserializePrimitiveObject();
        }

        private IList DeserializeList(int depth)
        {
            char? nextNonEmptyChar;
            char? nullable5;
            IList list = new ArrayList();
            if (this._s.MoveNext() != '[')
            {
                throw new ArgumentException(this._s.GetDebugString("JSON_InvalidArrayStart"));
            }
            bool flag = false;
        Label_00C4:
            nullable5 = nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            int? nullable7 = nullable5.HasValue ? new int?(nullable5.GetValueOrDefault()) : null;
            if (nullable7.HasValue && (nextNonEmptyChar != ']'))
            {
                this._s.MovePrev();
                object obj2 = this.DeserializeInternal(depth);
                list.Add(obj2);
                flag = false;
                nextNonEmptyChar = this._s.GetNextNonEmptyChar();
                if (nextNonEmptyChar != ']')
                {
                    flag = true;
                    if (nextNonEmptyChar != ',')
                    {
                        throw new ArgumentException(this._s.GetDebugString("JSON_InvalidArrayExpectComma"));
                    }
                    goto Label_00C4;
                }
            }
            if (flag)
            {
                throw new ArgumentException(this._s.GetDebugString("JSON_InvalidArrayExtraComma"));
            }
            if (nextNonEmptyChar != ']')
            {
                throw new ArgumentException(this._s.GetDebugString("JSON_InvalidArrayEnd"));
            }
            return list;
        }

        private string DeserializeMemberName()
        {
            char? nextNonEmptyChar = this._s.GetNextNonEmptyChar();
            char? nullable2 = nextNonEmptyChar;
            int? nullable4 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault()) : null;
            if (!nullable4.HasValue)
            {
                return null;
            }
            this._s.MovePrev();
            if (IsNextElementString(nextNonEmptyChar))
            {
                return this.DeserializeString();
            }
            return this.DeserializePrimitiveToken();
        }

        private object DeserializePrimitiveObject()
        {
            double num4;
            string s = this.DeserializePrimitiveToken();
            if (s.Equals("null"))
            {
                return null;
            }
            if (s.Equals("true"))
            {
                return true;
            }
            if (s.Equals("false"))
            {
                return false;
            }
            bool flag = s.IndexOf('.') >= 0;
            if (s.LastIndexOf("e", StringComparison.OrdinalIgnoreCase) < 0)
            {
                decimal num3;
                if (!flag)
                {
                    int num;
                    long num2;
                    if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
                    {
                        return num;
                    }
                    if (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
                    {
                        return num2;
                    }
                }
                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out num3))
                {
                    return num3;
                }
            }
            if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out num4))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "JSON_IllegalPrimitive", new object[] { s }));
            }
            return num4;
        }

        private string DeserializePrimitiveToken()
        {
            char? nullable2;
            StringBuilder builder = new StringBuilder();
            char? nullable = null;
        Label_0066:
            nullable2 = nullable = this._s.MoveNext();
            int? nullable4 = nullable2.HasValue ? new int?(nullable2.GetValueOrDefault()) : null;
            if (nullable4.HasValue)
            {
                if ((char.IsLetterOrDigit(nullable.Value) || (nullable.Value == '.')) || (((nullable.Value == '-') || (nullable.Value == '_')) || (nullable.Value == '+')))
                {
                    builder.Append(nullable);
                }
                else
                {
                    this._s.MovePrev();
                    goto Label_00A2;
                }
                goto Label_0066;
            }
        Label_00A2:
            return builder.ToString();
        }

        private string DeserializeString()
        {
            StringBuilder sb = new StringBuilder();
            bool flag = false;
            char? c = this._s.MoveNext();
            char ch = this.CheckQuoteChar(c);
            while (true)
            {
                char? nullable4 = c = this._s.MoveNext();
                int? nullable6 = nullable4.HasValue ? new int?(nullable4.GetValueOrDefault()) : null;
                if (!nullable6.HasValue)
                {
                    throw new ArgumentException(this._s.GetDebugString("JSON_UnterminatedString"));
                }
                if (c == '\\')
                {
                    if (flag)
                    {
                        sb.Append('\\');
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                else if (flag)
                {
                    this.AppendCharToBuilder(c, sb);
                    flag = false;
                }
                else
                {
                    char? nullable3 = c;
                    int num = ch;
                    if ((nullable3.GetValueOrDefault() == num) && nullable3.HasValue)
                    {
                        return sb.ToString();
                    }
                    sb.Append(c);
                }
            }
        }

        private object DeserializeStringIntoDateTime()
        {
            long num;
            Match match = Regex.Match(this._s.ToString(), "^\"\\\\/Date\\((?<ticks>-?[0-9]+)(?:[a-zA-Z]|(?:\\+|-)[0-9]{4})?\\)\\\\/\"");
            if (long.TryParse(match.Groups["ticks"].Value, out num))
            {
                this._s.MoveNext(match.Length);
                return new DateTime((num * 0x2710L) + JavaScriptSerializer.DatetimeMinTimeTicks);
            }
            return this.DeserializeString();
        }

        private static bool IsNextElementArray(char? c)
        {
            return (c == '[');
        }

        private bool IsNextElementDateTime()
        {
            string a = this._s.MoveNext(8);
            if (a != null)
            {
                this._s.MovePrev(8);
                return string.Equals(a, "\"\\/Date(", StringComparison.Ordinal);
            }
            return false;
        }

        private static bool IsNextElementObject(char? c)
        {
            return (c == '{');
        }

        private static bool IsNextElementString(char? c)
        {
            return ((c == '"') || (c == '\''));
        }
    }

    public class DynamicDictionary : DynamicObject, IDictionary<string, object>
    {
        IDictionary<string, object> dictionary;

        public DynamicDictionary()
        {
            dictionary = new Dictionary<string, object>();
        }

        public DynamicDictionary(IDictionary<string, object> data)
        {
            dictionary = data;
        }

        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();

            return dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            dictionary[binder.Name.ToLower()] = value;

            return true;
        }

        public object this[string key]
        {
            get { return dictionary[key]; }
            set { dictionary[key] = value; }
        }

        public void Add(string key, object value)
        {
            dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return dictionary.Keys; }
        }

        public bool Remove(string key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return dictionary.Values; }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            dictionary.Add(item);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return dictionary.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return dictionary.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
    }
}
