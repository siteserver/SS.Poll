﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SS.Poll.Core.Utils
{
    public static class StringUtils
    {
        public static class Constants
        {
            public const string ReturnAndNewline = "\r\n";//回车换行
            public const string Html5Empty = @"<html><head><meta charset=""utf-8""></head><body></body></html>";

            public const string Ellipsis = "...";

            public const int PageSize = 25;//后台分页数
            public const string HideElementStyle = "display:none";
            public const string ShowElementStyle = "display:";

            public const string TitleImageAppendix = "t_";
            public const string SmallImageAppendix = "s_";
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        public static bool IsMobile(string val)
        {
            return Regex.IsMatch(val, @"^1[3456789]\d{9}$", RegexOptions.IgnoreCase);
        }

        public static bool IsEmail(string val)
        {
            return Regex.IsMatch(val, @"^\w+([-_+.]\w+)*@\w+([-_.]\w+)*\.\w+([-_.]\w+)*$", RegexOptions.IgnoreCase);
        }

        public static bool IsIpAddress(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static bool IsNumber(string val)
        {
            const string formatNumber = "^[0-9]+$";
            return Regex.IsMatch(val, formatNumber);
        }

        public static bool IsDateTime(string val)
        {
            const string formatDate = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";
            const string formatDateTime = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$";

            return Regex.IsMatch(val, formatDate) || Regex.IsMatch(val, formatDateTime);
        }

        public static bool In(string strCollection, int inInt)
        {
            return In(strCollection, inInt.ToString());
        }

        public static bool In(string strCollection, string inStr)
        {
            if (string.IsNullOrEmpty(strCollection)) return false;
            return strCollection == inStr || strCollection.StartsWith(inStr + ",") || strCollection.EndsWith("," + inStr) || strCollection.IndexOf("," + inStr + ",", StringComparison.Ordinal) != -1;
        }

        public static bool Contains(string text, string inner)
        {
            return text?.IndexOf(inner, StringComparison.Ordinal) >= 0;
        }

        public static bool ContainsIgnoreCase(string text, string inner)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(inner)) return false;
            return text.ToLower().IndexOf(inner.ToLower(), StringComparison.Ordinal) >= 0;
        }

        public static bool ContainsIgnoreCase(List<string> list, string target)
        {
            if (list == null || list.Count == 0) return false;

            return list.Any(element => EqualsIgnoreCase(element, target));
        }

        public static string Trim(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.Trim();
        }

        public static string TrimAndToLower(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.ToLower().Trim();
        }

        public static string Remove(string text, int startIndex)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            if (startIndex >= text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            return text.Substring(0, startIndex);
        }


        public static string RemoveNewline(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;
            var retVal = new StringBuilder();
            inputString = inputString.Trim();
            for (var i = 0; i < inputString.Length; i++)
            {
                switch (i)
                {
                    case '\n':
                        break;
                    case '\r':
                        break;
                    default:
                        retVal.Append(i);
                        break;
                }
            }
            return retVal.ToString();
        }

        public static string Guid()
        {
            return System.Guid.NewGuid().ToString();
        }

        public static string GetShortGuid()
        {
            long i = 1;
            foreach (var b in System.Guid.NewGuid().ToByteArray())
            {
                i *= b + 1;
            }
            return $"{i - DateTime.Now.Ticks:x}";
        }

        public static string GetShortGuid(bool isUppercase)
        {
            long i = 1;
            foreach (var b in System.Guid.NewGuid().ToByteArray())
            {
                i *= b + 1;
            }
            string retval = $"{i - DateTime.Now.Ticks:x}";
            return isUppercase ? retval.ToUpper() : retval.ToLower();
        }

        public static string GetBoolText(bool type)
        {
            return type ? "是" : "否";
        }

        public static bool EqualsIgnoreCase(string a, string b)
        {
            if (a == b) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;

            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsIgnoreNull(string a, string b)
        {
            return string.IsNullOrEmpty(a) ? string.IsNullOrEmpty(b) : string.Equals(a, b);
        }

        public static bool StartsWithIgnoreCase(string text, string startString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(startString)) return false;
            return text.Trim().ToLower().StartsWith(startString.Trim().ToLower()) || string.Equals(text.Trim(), startString.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(string text, string endString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(endString)) return false;
            return text.Trim().ToLower().EndsWith(endString.Trim().ToLower());
        }

        public static bool StartsWith(string text, string startString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(startString)) return false;
            return text.StartsWith(startString);
        }

        public static bool EndsWith(string text, string endString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(endString)) return false;
            return text.EndsWith(endString);
        }

        public static bool StringEndsWith(string s, char c)
        {
            var num1 = s.Length;
            if (num1 != 0)
            {
                return s[num1 - 1] == c;
            }
            return false;
        }

        public static bool StringStartsWith(string s, char c)
        {
            if (s.Length != 0)
            {
                return s[0] == c;
            }
            return false;
        }

        public static string InsertBefore(string[] insertBeforeArray, string content, string insertContent)
        {
            if (content == null) return string.Empty;
            foreach (var insertBefore in insertBeforeArray)
            {
                if (content.IndexOf(insertBefore, StringComparison.Ordinal) != -1)
                {
                    return InsertBefore(insertBefore, content, insertContent);
                }
            }
            return content;
        }

        public static string InsertBefore(string insertBefore, string content, string insertContent)
        {
            var retval = content;
            if (insertBefore != null && content != null)
            {
                var startIndex = content.IndexOf(insertBefore, StringComparison.Ordinal);
                if (startIndex != -1)
                {
                    retval = content.Substring(0, startIndex) + insertContent + insertBefore + content.Substring(startIndex + insertBefore.Length);
                }
            }
            return retval;
        }

        public static bool InsertBefore(string[] insertBeforeArray, StringBuilder contentBuilder, string insertContent)
        {
            if (contentBuilder == null) return false;
            foreach (var insertBefore in insertBeforeArray)
            {
                if (contentBuilder.ToString().IndexOf(insertBefore, StringComparison.Ordinal) != -1)
                {
                    InsertBefore(insertBefore, contentBuilder, insertContent);
                    return true;
                }
            }
            return false;
        }

        public static void InsertBefore(string insertBefore, StringBuilder contentBuilder, string insertContent)
        {
            if (string.IsNullOrEmpty(insertBefore) || contentBuilder == null) return;
            var startIndex = contentBuilder.ToString().IndexOf(insertBefore, StringComparison.Ordinal);
            if (startIndex != -1)
            {
                contentBuilder.Insert(startIndex, insertContent);
            }
        }

        public static void InsertBeforeOrAppend(string[] insertBeforeArray, StringBuilder contentBuilder, string insertContent)
        {
            if (!InsertBefore(insertBeforeArray, contentBuilder, insertContent))
            {
                contentBuilder.Append(insertContent);
            }
        }

        public static void InsertAfterOrAppend(string[] insertAfterArray, StringBuilder contentBuilder, string insertContent)
        {
            if (!InsertAfter(insertAfterArray, contentBuilder, insertContent))
            {
                contentBuilder.Append(insertContent);
            }
        }

        public static bool InsertAfter(string[] insertAfterArray, StringBuilder contentBuilder, string insertContent)
        {
            if (contentBuilder != null)
            {
                foreach (var insertAfter in insertAfterArray)
                {
                    if (contentBuilder.ToString().IndexOf(insertAfter, StringComparison.Ordinal) != -1)
                    {
                        InsertAfter(insertAfter, contentBuilder, insertContent);
                        return true;
                    }
                }
            }
            return false;
        }

        public static void InsertAfter(string insertAfter, StringBuilder contentBuilder, string insertContent)
        {
            if (string.IsNullOrEmpty(insertAfter) || contentBuilder == null) return;
            var startIndex = contentBuilder.ToString().IndexOf(insertAfter, StringComparison.Ordinal);
            if (startIndex == -1) return;
            if (startIndex != -1)
            {
                contentBuilder.Insert(startIndex + insertAfter.Length, insertContent);
            }
        }

        public static string HtmlDecode(string inputString)
        {
            return HttpUtility.HtmlDecode(inputString);
        }

        public static string HtmlEncode(string inputString)
        {
            return HttpUtility.HtmlEncode(inputString);
        }

        public static string ToXmlContent(string inputString)
        {
            var contentBuilder = new StringBuilder(inputString);
            contentBuilder.Replace("<![CDATA[", string.Empty);
            contentBuilder.Replace("]]>", string.Empty);
            contentBuilder.Insert(0, "<![CDATA[");
            contentBuilder.Append("]]>");
            return contentBuilder.ToString();
        }
        public static string ReplaceIgnoreCase(string original, string pattern, string replacement)
        {
            if (original == null) return string.Empty;
            if (replacement == null) replacement = string.Empty;
            var count = 0;
            var position0 = 0;
            int position1;
            var upperString = original.ToUpper();
            var upperPattern = pattern.ToUpper();
            var inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            var chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern, position0, StringComparison.Ordinal)) != -1)
            {
                for (var i = position0; i < position1; ++i) chars[count++] = original[i];
                foreach (var t in replacement)
                {
                    chars[count++] = t;
                }
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (var i = position0; i < original.Length; ++i) chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        
        public static void ReplaceHrefOrSrc(StringBuilder builder, string replace, string to)
        {
            builder.Replace("href=\"" + replace, "href=\"" + to);
            builder.Replace("href='" + replace, "href='" + to);
            builder.Replace("href=" + replace, "href=" + to);
            builder.Replace("href=&quot;" + replace, "href=&quot;" + to);
            builder.Replace("src=\"" + replace, "src=\"" + to);
            builder.Replace("src='" + replace, "src='" + to);
            builder.Replace("src=" + replace, "src=" + to);
            builder.Replace("src=&quot;" + replace, "src=&quot;" + to);
        }

        public static string ReplaceFirst(string replace, string input, string to)
        {
            var pos = input.IndexOf(replace, StringComparison.Ordinal);
            if (pos > 0)
            {
                //取位置前部分+替换字符串+位置（加上查找字符长度）后部分
                return input.Substring(0, pos) + to + input.Substring(pos + replace.Length);
            }
            if (pos == 0)
            {
                return to + input.Substring(replace.Length);
            }
            return input;
        }

        public static string ReplaceSpecified(string replace, string input, string to, int specified)
        {
            if (specified <= 1)
            {
                return ReplaceFirst(replace, input, to);
            }
            var pos = 0;
            for (var i = 1; i <= specified; i++)
            {
                pos = input.IndexOf(replace, pos + 1, StringComparison.Ordinal);
            }

            if (pos > 0)
            {
                //取位置前部分+替换字符串+位置（加上查找字符长度）后部分
                return input.Substring(0, pos) + to + input.Substring(pos + replace.Length);
            }
            if (pos == 0)
            {
                return to + input.Substring(replace.Length);
            }
            return input;
        }

        public static string ReplaceAfterIndex(string replace, string input, string to, int index)
        {
            index = input.IndexOf(replace, index + 1, StringComparison.Ordinal);
            if (index > 0)
            {
                //取位置前部分+替换字符串+位置（加上查找字符长度）后部分
                return input.Substring(0, index) + to + input.Substring(index + replace.Length);
            }
            if (index == 0)
            {
                return to + input.Substring(replace.Length);
            }
            return input;
        }

        public static string ReplaceStartsWith(string input, string replace, string to)
        {
            var retval = input;
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(replace) && input.StartsWith(replace))
            {
                retval = to + input.Substring(replace.Length);
            }
            return retval;
        }

        public static string ReplaceStartsWithIgnoreCase(string input, string replace, string to)
        {
            var retval = input;
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(replace) && input.ToLower().StartsWith(replace.ToLower()))
            {
                retval = to + input.Substring(replace.Length);
            }
            return retval;
        }

        public static string ReplaceEndsWith(string input, string replace, string to)
        {
            var retval = input;
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(replace) && input.EndsWith(replace))
            {
                retval = input.Substring(0, input.LastIndexOf(replace, StringComparison.Ordinal)) + to;
            }
            return retval;
        }

        public static string ReplaceNewlineToBr(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;
            var retVal = new StringBuilder();
            inputString = inputString.Trim();
            foreach (var t in inputString)
            {
                switch (t)
                {
                    case '\n':
                        retVal.Append("<br />");
                        break;
                    case '\r':
                        break;
                    default:
                        retVal.Append(t);
                        break;
                }
            }
            return retVal.ToString();
        }
        

        /// <summary>
        /// 将回车换行符替换为Tab符
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string ReplaceNewlineToTab(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;
            var retVal = new StringBuilder();
            inputString = inputString.Trim();
            foreach (var t in inputString)
            {
                switch (t)
                {
                    case '\r':
                        retVal.Append("\t");
                        break;
                    case '\n':
                        break;
                    default:
                        retVal.Append(t);
                        break;
                }
            }
            return retVal.ToString();
        }

        public static string ReplaceNewline(string inputString, string replacement)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;
            var retVal = new StringBuilder();
            inputString = inputString.Trim();
            foreach (var t in inputString)
            {
                switch (t)
                {
                    case '\n':
                        retVal.Append(replacement);
                        break;
                    case '\r':
                        break;
                    default:
                        retVal.Append(t);
                        break;
                }
            }
            return retVal.ToString();
        }

        public static string CutLengthText(string inputString, int length)
        {
            var retval = inputString;
            if (string.IsNullOrEmpty(retval)) return retval;
            retval = retval.Substring(0, length);
            if (retval.Length != inputString.Length)
            {
                retval += Constants.Ellipsis;
            }
            return retval;
        }

        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex = startIndex - length;
                    }
                }

                if (startIndex > str.Length)
                {
                    return string.Empty;
                }
            }
            else
            {
                if (length < 0)
                {
                    return string.Empty;
                }
                if (length + startIndex > 0)
                {
                    length = length + startIndex;
                    startIndex = 0;
                }
                else
                {
                    return string.Empty;
                }
            }

            if (str.Length - startIndex < length)
            {
                length = str.Length - startIndex;
            }

            return str.Substring(startIndex, length);
        }

        
        
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitStringIgnoreCase(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.ToLower().IndexOf(strSplit.ToLower(), StringComparison.Ordinal) < 0)
                {
                    return new[] { strContent };
                }

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            return new string[] { };
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitStringIgnoreCase(string strContent, string strSplit, int count)
        {
            var result = new string[count];
            var splited = SplitStringIgnoreCase(strContent, strSplit);

            for (var i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }
        

        public static int GetByteCount(string content)
        {
            return string.IsNullOrEmpty(content) ? 0 : Encoding.GetEncoding("gb2312").GetByteCount(content);
        }


        /// <summary>
        /// 得到innerText在content中的数目
        /// </summary>
        /// <param name="innerText"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static int GetCount(string innerText, string content)
        {
            if (innerText == null || content == null)
            {
                return 0;
            }
            var count = 0;
            for (var index = content.IndexOf(innerText, StringComparison.Ordinal); index != -1; index = content.IndexOf(innerText, index + innerText.Length, StringComparison.Ordinal))
            {
                count++;
            }
            return count;
        }

        public static int GetStartCount(char startChar, string content)
        {
            if (content == null)
            {
                return 0;
            }
            var count = 0;

            foreach (var theChar in content)
            {
                if (theChar == startChar)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        public static int GetStartCount(string startString, string content)
        {
            if (content == null)
            {
                return 0;
            }
            var count = 0;

            while (true)
            {
                if (content.StartsWith(startString))
                {
                    count++;
                    content = content.Remove(0, startString.Length);
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        public static string GetFirstOfStringCollection(string collection)
        {
            return GetFirstOfStringCollection(collection, ',');
        }

        public static string GetFirstOfStringCollection(string collection, char separator)
        {
            if (!string.IsNullOrEmpty(collection))
            {
                var index = collection.IndexOf(separator);
                return index == -1 ? collection : collection.Substring(0, index);
            }
            return string.Empty;
        }


        private static int _randomSeq;
        public static int GetRandomInt(int minValue, int maxValue)
        {
            var ro = new Random(unchecked((int)DateTime.Now.Ticks));
            var retval = ro.Next(minValue, maxValue);
            retval += _randomSeq++;
            if (retval >= maxValue)
            {
                _randomSeq = 0;
                retval = minValue;
            }
            return retval;
        }

        public static string ValueToUrl(string value)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                //替换url中的换行符，update by sessionliang at 20151211
                retval = value.Replace("=", "_equals_").Replace("&", "_and_").Replace("?", "_question_").Replace("'", "_quote_").Replace("+", "_add_").Replace("\r", "").Replace("\n", "");
            }
            return retval;
        }

        public static string ValueToUrl(string value, bool replaceSlash)
        {

            var retval = string.Empty;
            if (replaceSlash)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    retval = value.Replace("=", "_equals_").Replace("&", "_and_").Replace("?", "_question_").Replace("'", "_quote_").Replace("+", "_add_").Replace("/", "_slash_");
                }
            }
            else
            {
                retval = ValueToUrl(value);
            }
            return retval;
        }

        public static string ValueFromUrl(string value)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                retval = value.Replace("_equals_", "=").Replace("_and_", "&").Replace("_question_", "?").Replace("_quote_", "'").Replace("_add_", "+");
            }
            return retval;
        }

        public static string ValueFromUrl(string value, bool replaceSlash)
        {
            var retval = string.Empty;
            if (replaceSlash)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    retval = value.Replace("_equals_", "=").Replace("_and_", "&").Replace("_question_", "?").Replace("_quote_", "'").Replace("_add_", "+").Replace("_slash_", "/");
                }
            }
            else
            {
                retval = ValueFromUrl(value);
            }
            return retval;
        }

        public static string ToJsString(string value)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                retval = value.Replace("'", @"\'").Replace("\r", "\\r").Replace("\n", "\\n");
            }
            return retval;
        }

        public static int GetDotNetVersion()
        {
            return Environment.Version.Major;
        }

        public static string IntToSignString(int i)
        {
            var retval = "0";
            if (i != 0)
            {
                retval = i > 0 ? "+" + i : i.ToString();
            }
            return retval;
        }

        public static string GetPercentage(int num, int totalNum)
        {
            return Convert.ToDouble(num / (double)totalNum).ToString("0.00%");
        }

        /// <summary>
        /// 去除HTML标记
        /// </summary>
        /// <param name="htmlstring">包括HTML的源码 </param>
        /// <returns>已经去除后的文字</returns>
        public static string NoHtml(string htmlstring)
        {
            //删除脚本
            htmlstring = htmlstring.Replace("\r\n", "");
            htmlstring = Regex.Replace(htmlstring, @"<script.*?</script>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<style.*?</style>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<.*?>", "", RegexOptions.IgnoreCase);
            //删除HTML
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlstring = htmlstring.Replace("&ldquo;", "\"");
            htmlstring = htmlstring.Replace("&rdquo;", "\"");
            htmlstring = htmlstring.Replace("<", "");
            htmlstring = htmlstring.Replace(">", "");
            htmlstring = htmlstring.Replace("\r\n", "");
            htmlstring = HttpContext.Current.Server.HtmlEncode(htmlstring).Trim();
            return htmlstring;
        }

        public static string GetTrueImageHtml(string isDefaultStr)
        {
            return GetTrueImageHtml(TranslateUtils.ToBool(isDefaultStr));
        }

        public static string GetTrueImageHtml(bool isDefault)
        {
            var retval = string.Empty;
            if (isDefault)
            {
                retval = "<img src='../pic/icon/right.gif' border='0'/>";
            }
            return retval;
        }

        public static string GetFalseImageHtml(string isDefaultStr)
        {
            return GetFalseImageHtml(TranslateUtils.ToBool(isDefaultStr));
        }

        public static string GetFalseImageHtml(bool isDefault)
        {
            var retval = string.Empty;
            if (isDefault == false)
            {
                retval = "<img src='../pic/icon/wrong.gif' border='0'/>";
            }
            return retval;
        }

        public static string GetTrueOrFalseImageHtml(string isDefaultStr)
        {
            return GetTrueOrFalseImageHtml(TranslateUtils.ToBool(isDefaultStr));
        }

        public static string GetTrueOrFalseImageHtml(bool isDefault)
        {
            return isDefault ? "<img src='../pic/icon/right.gif' border='0'/>" : "<img src='../pic/icon/wrong.gif' border='0'/>";
        }
        

        public static string UpperFirst(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static string LowerFirst(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.First().ToString().ToLower() + input.Substring(1);
        }
    }
}
