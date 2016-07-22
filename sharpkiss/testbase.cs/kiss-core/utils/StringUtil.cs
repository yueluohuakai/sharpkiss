using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Kiss.Utils
{
    /// <summary>
    /// Miscellaneous <see cref="System.String"/> utility methods.
    /// </summary>
    public class StringUtil
    {
        #region Fields

        /// <summary>
        /// An empty array of <see cref="System.String"/> instances.
        /// </summary>
        public static readonly string[] EmptyStrings = new string[] { };

        /// <summary>
        /// The string that signals the start of an Ant-style expression.
        /// </summary>
        private const string AntExpressionPrefix = "${";

        /// <summary>
        /// The string that signals the end of an Ant-style expression.
        /// </summary>
        private const string AntExpressionSuffix = "}";

        public const string Backslash = @"\";

        public const string Comma = ",";

        public const string Equal = "=";

        public const string Semicolon = ";";

        #endregion

        #region Methods

        public static string htmlencode(object str)
        {
            if (str == null) return string.Empty;

            return HttpUtility.HtmlEncode(str);
        }

        public static string urlencode(object str)
        {
            if (str == null) return string.Empty;

            return HttpUtility.UrlEncode(str.ToString());
        }

        public static string htmldecode(object str)
        {
            if (str == null) return string.Empty;

            return HttpUtility.HtmlDecode(str.ToString());
        }

        public static string urldecode(object str)
        {
            if (str == null) return string.Empty;

            return HttpUtility.UrlDecode(str.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static NameValueCollection CommaDelimitedEquation2NVCollection(string equations)
        {
            return DelimitedEquation2NVCollection(Comma, equations, true);
        }

        public static NameValueCollection DelimitedEquation2NVCollection(string delimiter, string equations)
        {
            return DelimitedEquation2NVCollection(delimiter, equations, true);
        }

        public static NameValueCollection DelimitedEquation2NVCollection(string delimiter, string equations, bool urlDecode)
        {
            string[] s = Split(equations, delimiter, true, true);

            NameValueCollection equationCol = new NameValueCollection(s.Length);

            foreach (string equation in s)
            {
                string[] expression = Equation2StringArray(equation);
                if (expression.Length != 2)
                    continue;

                equationCol.Add(expression[0], urlDecode ? ServerUtil.UrlDecode(expression[1]) : expression[1]);
            }

            return equationCol;
        }

        public static string ToQueryString(NameValueCollection nv)
        {
            return ToQueryString(nv, "&");
        }

        /// <summary>
        /// convert namevaluecollection to query string
        /// </summary>
        /// <param name="nv"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string ToQueryString(NameValueCollection nv, string delimiter)
        {
            StringBuilder sb = new StringBuilder(nv.Count);

            for (int i = 0; i < nv.Count; i++)
            {
                if (i > 0)
                    sb.Append(delimiter);

                string key = nv.GetKey(i);
                string[] values = nv.GetValues(i);

                for (int j = 0; j < values.Length; j++)
                {
                    if (j > 0)
                        sb.Append(Comma);

                    sb.AppendFormat("{0}={1}", key, values[j]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="equation"></param>
        /// <returns></returns>
        public static string[] Equation2StringArray(string equation)
        {
            string[] s = Split(equation, Equal, false, false);

            if (s.Length == 0)
                return new string[] { };

            string s2 = s.Length > 1 ? s[1] : string.Empty;

            return new string[] { s[0], s2 };
        }

        /// <summary>
        /// Tokenize the given <see cref="System.String"/> into a
        /// <see cref="System.String"/> array.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If <paramref name="s"/> is <see langword="null"/>, returns an empty
        /// <see cref="System.String"/> array.
        /// </p>
        /// <p>
        /// If <paramref name="delimiters"/> is <see langword="null"/> or the empty
        /// <see cref="System.String"/>, returns a <see cref="System.String"/> array with one
        /// element: <paramref name="s"/> itself.
        /// </p>
        /// </remarks>
        /// <param name="s">The <see cref="System.String"/> to tokenize.</param>
        /// <param name="delimiters">
        /// The delimiter characters, assembled as a <see cref="System.String"/>.
        /// </param>
        /// <param name="trimTokens">
        /// Trim the tokens via <see cref="System.String.Trim()"/>.
        /// </param>
        /// <param name="ignoreEmptyTokens">
        /// Omit empty tokens from the result array.</param>
        /// <returns>An array of the tokens.</returns>
        public static string[] Split(string s, string delimiters, bool trimTokens, bool ignoreEmptyTokens)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new string[0];
            }
            if (string.IsNullOrEmpty(delimiters))
            {
                return new string[] { s };
            }
            string[] tmp = s.Split(delimiters.ToCharArray());
            // short circuit if String.Split default behavior is ok
            if (!trimTokens && !ignoreEmptyTokens)
            {
                return tmp;
            }
            else
            {
                ArrayList tokens = new ArrayList(tmp.Length);
                for (int i = 0; i < tmp.Length; ++i)
                {
                    string token = (trimTokens ? tmp[i].Trim() : tmp[i]);
                    if (!(ignoreEmptyTokens && token.Length == 0))
                    {
                        tokens.Add(token);
                    }
                }
                return (string[])tokens.ToArray(typeof(string));
            }
        }

        /// <summary>
        /// Convert a CSV list into an array of <see cref="System.String"/>s.
        /// </summary>
        /// <param name="s">A CSV list.</param>
        /// <returns>
        /// An array of <see cref="System.String"/>s, or the empty array
        /// if <paramref name="s"/> is <see langword="null"/>.
        /// </returns>
        public static string[] CommaDelimitedListToStringArray(string s)
        {
            return DelimitedListToStringArray(s, Comma);
        }

        /// <summary>
        /// Take a <see cref="System.String"/> which is a delimited list
        /// and convert it to a <see cref="System.String"/> array.
        /// </summary>
        /// <remarks>
        /// <p>
        /// If the supplied <paramref name="delimiter"/> is a
        /// <cref lang="null"/> or zero-length string, then a single element
        /// <see cref="System.String"/> array composed of the supplied
        /// <paramref name="input"/> <see cref="System.String"/> will be 
        /// eturned. If the supplied <paramref name="input"/>
        /// <see cref="System.String"/> is <cref lang="null"/>, then an empty,
        /// zero-length <see cref="System.String"/> array will be returned.
        /// </p>
        /// </remarks>
        /// <param name="input">
        /// The <see cref="System.String"/> to be parsed.
        /// </param>
        /// <param name="delimiter">
        /// The delimeter (this will not be returned). Note that only the first
        /// character of the supplied <paramref name="delimiter"/> is used.
        /// </param>
        /// <returns>
        /// An array of the tokens in the list.
        /// </returns>
        public static string[] DelimitedListToStringArray(string input, string delimiter)
        {
            if (input == null)
            {
                return new string[0];
            }
            if (!HasText(delimiter))
            {
                return new string[] { input };
            }
            return Split(input, delimiter, true, true);
        }

        /// <summary>
        /// Convenience method to return an
        /// <see cref="System.Collections.ICollection"/> as a delimited
        /// (e.g. CSV) <see cref="System.String"/>.
        /// </summary>
        /// <returns>The delimited string representation.</returns>
        public static string CollectionToDelimitedString(ICollection c, string delimiter, string surround)
        {
            if (c == null)
            {
                return "null";
            }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (object obj in c)
            {
                string str = obj.ToString();

                str = Surround(surround, str);

                if (i++ > 0)
                {
                    sb.Append(delimiter);
                }
                sb.Append(str);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Convenience method to return an
        /// <see cref="System.Collections.ICollection"/> as a CSV
        /// <see cref="System.String"/>.
        /// </summary>
        /// <returns>The delimited string representation.</returns>
        public static string CollectionToCommaDelimitedString(ICollection collection, string surround)
        {
            return CollectionToDelimitedString(collection, Comma, surround);
        }

        public static string CollectionToCommaDelimitedString(ICollection collection)
        {
            return CollectionToDelimitedString(collection, Comma, string.Empty);
        }

        /// <summary>
        /// Convenience method to return an array as a CSV
        /// <see cref="System.String"/>.
        /// </summary>
        /// <param name="source">
        /// The array to parse. Elements may be of any type (
        /// <see cref="System.Object.ToString"/> will be called on each
        /// element).
        /// </param>
        public static string ArrayToCommaDelimitedString(object[] source)
        {
            return ArrayToDelimitedString(source, Comma, string.Empty);
        }

        public static string ArrayToCommaDelimitedString(object[] source, string surround)
        {
            return ArrayToDelimitedString(source, Comma, surround);
        }

        /// <summary>
        /// Convenience method to return a <see cref="System.String"/>
        /// array as a delimited (e.g. CSV) <see cref="System.String"/>.
        /// </summary>
        public static string ArrayToDelimitedString(object[] source, string delimiter, string surround)
        {
            if (source == null)
            {
                return "null";
            }
            else
            {
                return CollectionToDelimitedString(source, delimiter, surround);
            }
        }

        /// <summary>
        /// Checks if a <see cref="System.String"/> has text.
        /// </summary>
        /// <remarks>
        /// <p>
        /// More specifically, returns <see langword="true"/> if the string is
        /// not <see langword="null"/>, it's <see cref="string.Length"/> is >
        /// zero <c>(0)</c>, and it has at least one non-whitespace character.
        /// </p>
        /// </remarks>
        /// <param name="target">
        /// The string to check, may be <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="target"/> is not
        /// <see langword="null"/>,
        /// <see cref="string.Length"/> > zero <c>(0)</c>, and does not consist
        /// solely of whitespace.
        /// </returns>
        /// <example>
        /// <code language="C#">
        /// StringUtils.HasText(null) = false
        /// StringUtils.HasText("") = false
        /// StringUtils.HasText(" ") = false
        /// StringUtils.HasText("12345") = true
        /// StringUtils.HasText(" 12345 ") = true
        /// </code>
        /// </example>
        public static bool HasText(object target)
        {
            return (target != null && target.ToString().Trim().Length > 0);
        }

        /// <summary>
        /// Checks if a <see cref="System.String"/> is <see langword="null"/>
        /// or an empty string.
        /// </summary>
        /// <remarks>
        /// <p>
        /// More specifically, returns <see langword="false"/> if the string is
        /// <see langword="null"/>, it's <see cref="string.Length"/> is equal
        /// to zero <c>(0)</c>, or it is composed entirely of whitespace
        /// characters.
        /// </p>
        /// </remarks>
        /// <param name="target">
        /// The string to check, may (obviously) be <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the <paramref name="target"/> is
        /// <see langword="null"/>, has a length equal to zero <c>(0)</c>, or
        /// is composed entirely of whitespace characters.
        /// </returns>
        /// <example>
        /// <code language="C#">
        /// StringUtils.IsNullOrEmpty(null) = true
        /// StringUtils.IsNullOrEmpty("") = true
        /// StringUtils.IsNullOrEmpty(" ") = true
        /// StringUtils.IsNullOrEmpty("12345") = false
        /// StringUtils.IsNullOrEmpty(" 12345 ") = false
        /// </code>
        /// </example>
        public static bool IsNullOrEmpty(string target)
        {
            return !HasText(target);
        }

        public static bool IsGuid(string target)
        {
            return HasText(target) && target.Length == 36;
        }

        /// <summary>
        /// Strips first and last character off the string.
        /// </summary>
        /// <param name="text">The string to strip.</param>
        /// <returns>The stripped string.</returns>
        public static string StripFirstAndLastCharacter(string text)
        {
            if (text != null
                && text.Length > 2)
            {
                return text.Substring(1, text.Length - 2);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Returns a list of Ant-style expressions from the specified text.
        /// </summary>
        /// <param name="text">The text to inspect.</param>
        /// <returns>
        /// A list of expressions that exist in the specified text.
        /// </returns>
        /// <exception cref="System.FormatException">
        /// If any of the expressions in the supplied <paramref name="text"/>
        /// is empty (<c>${}</c>).
        /// </exception>
        public static IList GetAntExpressions(string text)
        {
            IList expressions = new ArrayList();
            if (HasText(text))
            {
                int start = text.IndexOf(AntExpressionPrefix);
                while (start >= 0)
                {
                    int end = text.IndexOf(AntExpressionSuffix, start + 2);
                    if (end == -1)
                    {
                        // terminator character not found, so let's quit...
                        start = -1;
                    }
                    else
                    {
                        string exp = text.Substring(start + 2, end - start - 2);
                        if (IsNullOrEmpty(exp))
                        {
                            throw new FormatException(
                                string.Format("Empty {0}{1} value found in text : '{2}'.",
                                              AntExpressionPrefix,
                                              AntExpressionSuffix,
                                              text));
                        }
                        if (expressions.IndexOf(exp) < 0)
                        {
                            expressions.Add(exp);
                        }
                        start = text.IndexOf(AntExpressionPrefix, end);
                    }
                }
            }
            return expressions;
        }

        /// <summary>
        /// Replaces Ant-style expression placeholder with expression value.
        /// </summary>
        /// <remarks>
        /// <p>
        /// 
        /// </p>
        /// </remarks>
        /// <param name="text">The string to set the value in.</param>
        /// <param name="expression">The name of the expression to set.</param>
        /// <param name="expValue">The expression value.</param>
        /// <returns>
        /// A new string with the expression value set; the
        /// <see cref="String.Empty"/> value if the supplied
        /// <paramref name="text"/> is <see langword="null"/>, has a length
        /// equal to zero <c>(0)</c>, or is composed entirely of whitespace
        /// characters.
        /// </returns>
        public static string SetAntExpression(string text, string expression, object expValue)
        {
            if (IsNullOrEmpty(text))
            {
                return String.Empty;
            }
            if (expValue == null)
            {
                expValue = String.Empty;
            }
            return text.Replace(
                Surround(AntExpressionPrefix, expression, AntExpressionSuffix), expValue.ToString());
        }

        public static string FormatAntExpression(string text)
        {
            return Surround(AntExpressionPrefix, text, AntExpressionSuffix);
        }

        /// <summary>
        /// Surrounds (prepends and appends) the string value of the supplied
        /// <paramref name="fix"/> to the supplied <paramref name="target"/>.
        /// </summary>
        /// <remarks>
        /// <p>
        /// The return value of this method call is always guaranteed to be non
        /// <see langword="null"/>. If every value passed as a parameter to this method is
        /// <see langword="null"/>, the <see cref="System.String.Empty"/> string will be returned.
        /// </p>
        /// </remarks>
        /// <param name="fix">
        /// The pre<b>fix</b> and suf<b>fix</b> that respectively will be prepended and
        /// appended to the target <paramref name="target"/>. If this value
        /// is not a <see cref="System.String"/> value, it's attendant
        /// <see cref="System.Object.ToString()"/> value will be used.
        /// </param>
        /// <param name="target">
        /// The target that is to be surrounded. If this value is not a
        /// <see cref="System.String"/> value, it's attendant
        /// <see cref="System.Object.ToString()"/> value will be used.
        /// </param>
        /// <returns>The surrounded string.</returns>
        public static string Surround(object fix, object target)
        {
            return Surround(fix, target, fix);
        }

        /// <summary>
        /// Surrounds (prepends and appends) the string values of the supplied
        /// <paramref name="prefix"/> and <paramref name="suffix"/> to the supplied
        /// <paramref name="target"/>.
        /// </summary>
        /// <remarks>
        /// <p>
        /// The return value of this method call is always guaranteed to be non
        /// <see langword="null"/>. If every value passed as a parameter to this method is
        /// <see langword="null"/>, the <see cref="System.String.Empty"/> string will be returned.
        /// </p>
        /// </remarks>
        /// <param name="prefix">
        /// The value that will be prepended to the <paramref name="target"/>. If this value
        /// is not a <see cref="System.String"/> value, it's attendant
        /// <see cref="System.Object.ToString()"/> value will be used.
        /// </param>
        /// <param name="target">
        /// The target that is to be surrounded. If this value is not a
        /// <see cref="System.String"/> value, it's attendant
        /// <see cref="System.Object.ToString()"/> value will be used.
        /// </param>
        /// <param name="suffix">
        /// The value that will be appended to the <paramref name="target"/>. If this value
        /// is not a <see cref="System.String"/> value, it's attendant
        /// <see cref="System.Object.ToString()"/> value will be used.
        /// </param>
        /// <returns>The surrounded string.</returns>
        public static string Surround(object prefix, object target, object suffix)
        {
            return string.Format(
                CultureInfo.InvariantCulture, "{0}{1}{2}", prefix, target, suffix);
        }

        /// <summary>
        /// Converts escaped characters (for example "\t") within a string
        /// to their real character.
        /// </summary>
        /// <param name="inputString">The string to convert.</param>
        /// <returns>The converted string.</returns>
        public static string ConvertEscapedCharacters(string inputString)
        {
            StringBuilder sb = new StringBuilder(inputString.Length);
            for (int i = 0; i < inputString.Length; i++)
            {
                if (inputString[i].Equals('\\'))
                {
                    i++;
                    if (inputString[i].Equals('t'))
                    {
                        sb.Append('\t');
                    }
                    else if (inputString[i].Equals('r'))
                    {
                        sb.Append('\r');
                    }
                    else if (inputString[i].Equals('n'))
                    {
                        sb.Append('\n');
                    }
                    else if (inputString[i].Equals('\\'))
                    {
                        sb.Append('\\');
                    }
                    else
                    {
                        sb.Append("\\" + inputString[i]);
                    }
                }
                else
                {
                    sb.Append(inputString[i]);
                }
            }
            return sb.ToString();
        }

        public static bool ToBoolean(object o)
        {
            return ToBoolean(o, false);
        }

        public static bool ToBoolean(object o, bool defaultValue)
        {
            if (o == null)
                return defaultValue;

            if (IsNullOrEmpty(o.ToString()))
                return defaultValue;

            if (string.Equals(o.ToString(), "1") || string.Equals(o.ToString(), "true", StringComparison.InvariantCultureIgnoreCase))
                return true;

            if (string.Equals(o.ToString(), "0") || string.Equals(o.ToString(), "false", StringComparison.InvariantCultureIgnoreCase))
                return false;

            return defaultValue;
        }

        public static string Boolean2String(bool b)
        {
            return b ? "1" : "0";
        }

        /// <summary>
        /// convert boolean to js bool string
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToJsBoolean(bool b)
        {
            return b.ToString().ToLower();
        }

        public static bool InvariantCultureIgnoreCaseEquals(string source, string target)
        {
            return StringComparer.InvariantCultureIgnoreCase.Equals(source, target);
        }

        /// <summary>
        /// convert a string to base64 string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64(string str)
        {
            if (IsNullOrEmpty(str))
                return string.Empty;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// convert a base64 string to a normal string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FromBase64(string str)
        {
            if (IsNullOrEmpty(str))
                return string.Empty;

            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        #endregion

        public static string templatestring(Dictionary<string, object> datas, string template)
        {
            try
            {
                ITemplateEngine te = ServiceLocator.Instance.Resolve<ITemplateEngine>();

                using (StringWriter sw = new StringWriter())
                {
                    te.Process(datas, string.Empty, sw, template);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger<StringUtil>().Error(ex.Message);

                return string.Empty;
            }
        }

        public static string Int2String(int i)
        {
            if (i == 0)
                return string.Empty;

            return i.ToString();
        }

        public static string Guid2UrlSafeString(string guid)
        {
            return guid.Replace("-", string.Empty);
        }

        public static string UrlSafeString2Guid(string urlsafeString)
        {
            if (StringUtil.HasText(urlsafeString) && urlsafeString.Length == 32)
                return urlsafeString.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-");

            return string.Empty;
        }

        public static string GetSafeQuery(string strSrc)
        {
            if (HasText(strSrc))
                return Regex.Replace(strSrc.Trim(), @"[;|\/|\(|\\|\)|\[|\]|\}|\{|%|@|\*|!|\']", string.Empty, RegexOptions.IgnoreCase);
            return string.Empty;
        }

        public static string GetSafeCacheKey(string key)
        {
            return Regex.Replace(key, @"(\s*)", string.Empty, RegexOptions.IgnoreCase);
        }

        public static string GetMatchedCharacters(string key)
        {
            return key + @"[-|;|\s|\w|,|\/|\(|\\|\)|\[|\]|\}|\{|%|@|\*|!|\']*";
        }

        public delegate string Format<S>(S s);
        public delegate T Format<S, T>(S s);

        public static string[] ToStringArray<S>(S[] source, Format<S> format)
        {
            return ToArray<S, string>(source,
                delegate(S s) { return format(s); });
        }

        public static int[] ToIntArray(string[] source)
        {
            return ToArray<string, int>(source, delegate(string str)
            {
                int i;
                if (Int32.TryParse(str, out i))
                    return i;
                return default(int);
            });
        }

        public static T[] ToArray<S, T>(S[] source, Format<S, T> format) where T : IEquatable<T>
        {
            if (source == null || format == null)
                return new T[] { };

            List<T> list = new List<T>(source.Length);
            foreach (S s in source)
            {
                T t = format(s);
                list.Add(format(s));
            }

            return list.ToArray();
        }

        public static string Trim(string param, int length)
        {
            return Trim(param, length, "...");
        }

        /// <summary>
        /// 按字节长度截取字符串(支持截取带HTML代码样式的字符串)
        /// </summary>
        /// <param name=”param”>将要截取的字符串参数</param>
        /// <param name=”length”>截取的字节长度</param>
        /// <param name=”end”>字符串末尾补上的字符串</param>
        /// <returns>返回截取后的字符串</returns>
        public static string Trim(string param, int length, string end)
        {
            string Pattern = null;
            MatchCollection m = null;
            StringBuilder result = new StringBuilder();
            int n = 0;
            char temp;
            bool isCode = false; //是不是HTML代码
            bool isHTML = false; //是不是HTML特殊字符,如&nbsp;
            char[] pchar = param.ToCharArray();
            for (int i = 0; i < pchar.Length; i++)
            {
                temp = pchar[i];
                if (temp == '<')
                {
                    isCode = true;
                }
                else if (temp == '&')
                {
                    isHTML = true;
                }
                else if (temp == '>' && isCode)
                {
                    n = n - 1;
                    isCode = false;
                }
                else if (temp == ';' && isHTML)
                {
                    isHTML = false;
                }

                if (!isCode && !isHTML)
                {
                    n = n + 1;
                    //UNICODE码字符占两个字节
                    if (Encoding.Default.GetBytes(temp + "").Length > 1)
                    {
                        n = n + 1;
                    }
                }

                result.Append(temp);
                if (n >= length)
                {
                    if (i < pchar.Length - 1)
                        result.Append(end);

                    break;
                }
            }

            //取出截取字符串中的HTML标记
            string temp_result = Regex.Replace(result.ToString(), "(>)[^<>]*(<?)", "$1$2");
            //去掉不需要结束标记的HTML标记
            temp_result = Regex.Replace(temp_result, @"</?(AREA|BASE|BASEFONT|BODY|BR|COL|COLGROUP|DD|DT|FRAME|HEAD|HR|HTML|IMG|INPUT|ISINDEX|LI|LINK|META|OPTION|P|PARAM|TBODY|TD|TFOOT|TH|THEAD|TR|area|base|basefont|body|br|col|colgroup|dd|dt|frame|head|hr|html|img|input|isindex|li|link|meta|option|p|param|tbody|td|tfoot|th|thead|tr)[^<>]*/?>", "");
            //去掉成对的HTML标记
            temp_result = Regex.Replace(temp_result, @"<([a-zA-Z]+)[^<>]*>(.*?)</\1>", "$2");
            //用正则表达式取出标记
            Pattern = ("<([a-zA-Z]+)[^<>]*>");
            m = Regex.Matches(temp_result, Pattern);
            ArrayList endHTML = new ArrayList();
            foreach (Match mt in m)
            {
                endHTML.Add(mt.Result("$1"));
            }
            //补全不成对的HTML标记
            for (int i = endHTML.Count - 1; i >= 0; i--)
            {
                result.Append("</");
                result.Append(endHTML[i]);
                result.Append(">");
            }
            return result.ToString();
        }

        public static string TrimHtml(string html)
        {
            if (!HasText(html))
                return string.Empty;
            Regex regex1 = new Regex(@"<script[\s\S]+</script *>", RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(@" href *= *[\s\S]*script *:", RegexOptions.IgnoreCase);
            //Regex regex3 = new Regex(@" no[\s\S]*=", RegexOptions.IgnoreCase);
            Regex regex4 = new Regex(@"<iframe[\s\S]+</iframe *>", RegexOptions.IgnoreCase);
            Regex regex5 = new Regex(@"<frameset[\s\S]+</frameset *>", RegexOptions.IgnoreCase);
            Regex regex6 = new Regex(@"\<img[^\>]+\>", RegexOptions.IgnoreCase);
            Regex regex7 = new Regex(@"</p>", RegexOptions.IgnoreCase);
            Regex regex8 = new Regex(@"<p>", RegexOptions.IgnoreCase);
            Regex regex9 = new Regex(@"<[^>]*>", RegexOptions.IgnoreCase);
            html = regex1.Replace(html, string.Empty); //过滤<script></script>标记
            html = regex2.Replace(html, string.Empty); //过滤href=javascript: (<A>) 属性
            //html = regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件
            html = regex4.Replace(html, string.Empty); //过滤iframe
            html = regex5.Replace(html, string.Empty); //过滤frameset
            html = regex6.Replace(html, string.Empty); //过滤frameset
            html = regex7.Replace(html, string.Empty); //过滤frameset
            html = regex8.Replace(html, string.Empty); //过滤frameset
            html = regex9.Replace(html, string.Empty);
            html = html.Replace(" ", string.Empty);
            html = html.Replace("&nbsp;", string.Empty);
            html = html.Replace("</strong>", string.Empty);
            html = html.Replace("<strong>", string.Empty);

            return html;
        }

        /// <summary>
        /// will remove in version 4.7
        /// </summary>
        /// <param name="url1"></param>
        /// <param name="url2"></param>
        /// <returns></returns>
        public static string CombinUrl(string url1, string url2)
        {
            return CombinUrl(url1, url2, string.Empty);
        }

        public static string CombinUrl(string url1, string url2, params string[] urls)
        {
            StringBuilder sb = new StringBuilder();

            List<string> list = new List<string>();

            list.Add(url1);
            list.Add(url2);

            list.AddRange(urls);

            list.RemoveAll((i) => { return string.IsNullOrEmpty(i); });

            for (int i = 0; i < list.Count; i++)
            {
                if (i == 0)
                    list[i] = list[i].TrimEnd('/');
                else if (i == list.Count - 1)
                    list[i] = list[i].TrimStart('/');
                else
                    list[i] = list[i].TrimStart('/').TrimEnd('/');
            }

            return list.Join("/");
        }

        public static string GetExtension(string str)
        {
            if (IsNullOrEmpty(str))
                return string.Empty;

            int index = str.LastIndexOf('.');
            if (index == -1)
                return string.Empty;

            return str.Substring(index);
        }

        public static int ToInt(object str, int defaultValue)
        {
            if (str == null)
                return defaultValue;
            int v;
            if (!int.TryParse(str.ToString(), out v))
                v = defaultValue;
            return v;
        }

        public static int ToInt(object str) { return ToInt(str, 0); }

        public static string FormatDate(object obj)
        {
            if (obj == null || obj is DBNull)
                return string.Empty;

            DateTime now = DateTime.Now;
            DateTime datetime = ToDateTime(obj, DateTime.MinValue);

            TimeSpan ts = now - datetime;

            if (datetime.Year == now.Year)
            {
                if (ts.Days < 1 && now.Day == datetime.Day)
                {
                    if (ts.Hours < 1)
                    {
                        if (ts.Minutes < 1)
                        {
                            if (ts.Seconds == 0)
                            {
                                return "刚刚";
                            }
                            return ts.Seconds.ToString() + "秒前";
                        }
                        return ts.Minutes.ToString() + "分钟前";
                    }
                    return "今天 " + datetime.ToString("HH:mm");
                }

                return string.Format("{0}月{1}日 {2}:{3}", datetime.Month.ToString(), datetime.Day.ToString(), datetime.Hour.ToString("#00"), datetime.Minute.ToString("#00"));
            }

            return string.Format("{4}-{0}-{1} {2}:{3}", datetime.Month.ToString(), datetime.Day.ToString(), datetime.Hour.ToString("#00"), datetime.Minute.ToString("#00"), datetime.Year.ToString());
        }

        public static decimal ToDecimal(object str, decimal defaultValue)
        {
            if (str == null) return defaultValue;
            decimal v;
            if (!decimal.TryParse(str.ToString(), out v))
                v = defaultValue;
            return v;
        }

        public static decimal ToDecimal(object str) { return ToDecimal(str, 0); }

        public static DateTime ToDateTime(object str, DateTime defaultValue)
        {
            if (str == null)
                return defaultValue;

            DateTime v;
            if (!DateTime.TryParse(str.ToString(), out v))
                v = defaultValue;

            return v;
        }

        public static DateTime ToDateTime(object str) { return ToDateTime(str, DateTime.Now); }

        public static string ToJson(object obj)
        {
            return new Kiss.Json.JavaScriptSerializer().Serialize(obj);
        }

        public static double Similarity(string str1, string str2)
        {
            int[,] d;     // 矩阵 
            int n = str1.Length;
            int m = str2.Length;
            int i;     // 遍历str1的 
            int j;     // 遍历str2的 
            char ch1;     // str1的 
            char ch2;     // str2的 
            int temp;     // 记录相同字符,在某个矩阵位置值的增量,不是0就是1 
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            d = new int[n + 1, m + 1];
            for (i = 0; i <= n; i++)
            {     // 初始化第一列 
                d[i, 0] = i;
            }
            for (j = 0; j <= m; j++)
            {     // 初始化第一行 
                d[0, j] = j;
            }
            for (i = 1; i <= n; i++)
            {     // 遍历str1 
                ch1 = str1[i - 1];
                // 去匹配str2 
                for (j = 1; j <= m; j++)
                {
                    ch2 = str2[j - 1];
                    if (ch1 == ch2)
                    {
                        temp = 0;
                    }
                    else
                    {
                        temp = 1;
                    }
                    // 左边+1,上边+1, 左上角+temp取最小 
                    d[i, j] = Math.Min(d[i - 1, j] + 1, Math.Min(d[i, j - 1] + 1, d[i - 1, j - 1] + temp));
                }
            }

            return 1 - (double)d[n, m] / Math.Max(str1.Length, str2.Length);
        }

        public static string UniqueId()
        {
            byte[] destinationArray = Guid.NewGuid().ToByteArray();
            DateTime time = new DateTime(0x76c, 1, 1);
            DateTime now = DateTime.Now;
            TimeSpan span = new TimeSpan(now.Ticks - time.Ticks);
            TimeSpan timeOfDay = now.TimeOfDay;
            byte[] bytes = BitConverter.GetBytes(span.Days);
            byte[] array = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));
            Array.Reverse(bytes);
            Array.Reverse(array);
            Array.Copy(bytes, bytes.Length - 2, destinationArray, destinationArray.Length - 6, 2);
            Array.Copy(array, array.Length - 4, destinationArray, destinationArray.Length - 4, 4);

            Array.Reverse(destinationArray);

            Guid guid = new Guid(destinationArray);

            string enc = Convert.ToBase64String(guid.ToByteArray());
            enc = enc.Replace("/", "-");
            enc = enc.Replace("+", "-");

            return enc.Substring(0, 22).ToLower();
        }

        /// <summary> 
        /// Encodes non-US-ASCII characters in a string. 
        /// </summary> 
        /// <param name="s"></param> 
        /// <returns></returns> 
        public static string ToHexString(string s)
        {
            char[] chars = s.ToCharArray();
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < chars.Length; index++)
            {
                bool needToEncode = NeedToEncode(chars[index]);
                if (needToEncode)
                {
                    string encodedString = ToHexString(chars[index]);
                    builder.Append(encodedString);
                }
                else
                {
                    builder.Append(chars[index]);
                }
            }
            return builder.ToString();
        }
        /// <summary> 
        /// Determines if the character needs to be encoded. 
        /// </summary> 
        /// <param name="chr"></param> 
        /// <returns></returns> 
        private static bool NeedToEncode(char chr)
        {
            string reservedChars = "$-_.+!*'(),@=&";
            if (chr > 127)
                return true;
            if (char.IsLetterOrDigit(chr) || reservedChars.IndexOf(chr) >= 0)
                return false;
            return true;
        }

        /// <summary> 
        /// Encodes a non-US-ASCII character. 
        /// </summary> 
        /// <param name="chr"></param> 
        /// <returns></returns> 
        private static string ToHexString(char chr)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(chr.ToString());
            StringBuilder builder = new StringBuilder();
            for (int index = 0; index < encodedBytes.Length; index++)
            {
                builder.AppendFormat("%{0}", Convert.ToString(encodedBytes[index], 16));
            }
            return builder.ToString();
        }
    }
}
