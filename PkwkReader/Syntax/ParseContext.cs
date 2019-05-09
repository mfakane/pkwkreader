using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 読み取りに必要な情報を表します。
    /// </summary>
	public class ParseContext : IEnumerator<char>
    {
        /// <summary>
        /// 現在の行番号を取得します。
        /// </summary>
		public int Line => Value.Substring(0, Index).Length - Value.Substring(0, Index).Replace("\n", null).Length + 1;

        /// <summary>
        /// 現在の行における文字数を取得します。
        /// </summary>
		public int Column
        {
            get
            {
                var lineStart = Value.LastIndexOf('\n', Index);

                if (lineStart == -1) lineStart = 0;

                return Index - lineStart + 1;
            }
        }

        /// <summary>
        /// 現在の文字を取得します。
        /// </summary>
		public char Current => Index >= Value.Length ? '\0' : Value[Index];
        object IEnumerator.Current => Current;

        /// <summary>
        /// ページのタイトルを取得または設定します。
        /// </summary>
		public string Title { get; set; }

        /// <summary>
        /// 読み取りに使用する構成を取得します。
        /// </summary>
		public WikiConfiguration Configuration { get; }

        /// <summary>
        /// 読み取る対象の文字列を取得します。
        /// </summary>
		public string Value { get; }

        /// <summary>
        /// 現在の位置を取得または設定します。
        /// </summary>
		public int Index { get; set; }

        /// <summary>
        /// 読み取る最大の位置を取得または設定します。
        /// </summary>
		public int? MaxIndex { get; set; }

        /// <summary>
        /// 読み取りに使用する構成、および読み取る対象の文字列を指定し、<see cref="ParseContext"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="config">読み取りに使用する構成。</param>
        /// <param name="value">読み取る対象の文字列。</param>
		public ParseContext(WikiConfiguration config, string value)
        {
            Configuration = config;
            Value = value;
        }

        void IDisposable.Dispose()
        {
        }

        public char? MoveNext() =>
            ++Index < (MaxIndex ?? Value.Length) ? Current : (char?)null;

        public char? PeekNext() =>
            PeekNext(1);

        public char? PeekNext(int offset) =>
            Index + offset < (MaxIndex ?? Value.Length) ? Value[Index + offset] : (char?)null;

        public bool IsMatchAny(params string[] delimiters) =>
            IsMatchAny(true, delimiters);

        public bool IsMatchAny(bool throwErrorOnEnd, params string[] delimiters) =>
            delimiters.Any(delimiter => Enumerable.Range(0, delimiter.Length).All(i =>
                PeekNext(i) is char c
                    ? c == delimiter[i]
                    : throwErrorOnEnd
                        ? throw CreateTokenExpectedError(delimiters)
                        : false));

        public ParseContext Take(string s)
        {
            for (var i = 0; i < s.Length; i++)
                if (Current == s[i])
                    MoveNext();
                else
                    throw CreateTokenExpectedError(s);

            return this;
        }

        public ParseContext Skip(int offset)
        {
            Index += offset;

            return this;
        }

        public ParseContext SkipWhiteSpaces()
        {
            while (Current != '\n' && char.IsWhiteSpace(Current))
                MoveNext();

            return this;
        }

        public string TakeWhile(Func<bool> condition)
        {
            var sb = new StringBuilder();

            if (!condition())
                return null;

            do
                sb.Append(Current);
            while (MoveNext().HasValue && condition());

            return sb.ToString();
        }

        public string TakeUntilAny(params string[] delimiters) =>
            TakeUntilAny(true, delimiters);

        public string TakeUntilAny(bool skipDelimiter, params string[] delimiters)
        {
            var sb = new StringBuilder();

            while (Current != '\0')
            {
                sb.Append(Current);
                MoveNext();

                if (delimiters.FirstOrDefault(sb.ToString().EndsWith) is string delimiter)
                {
                    var rt = sb.Remove(sb.Length - delimiter.Length, delimiter.Length).ToString();

                    if (!skipDelimiter)
                        Index -= delimiter.Length;

                    return rt;
                }
            }

            throw CreateTokenExpectedError(delimiters);
        }

        bool IEnumerator.MoveNext() =>
            ++Index < (MaxIndex ?? Value.Length);

        /// <summary>
        /// 読み取りを開始する前の状態に初期化します。
        /// </summary>
		public void Reset() =>
            Index = -1;

        /// <summary>
        /// 現在の位置に本来あるべきトークンを指定して、現在の位置で読み取りが失敗したことを示す例外を取得します。
        /// </summary>
        /// <param name="expected">現在の位置に本来あるべきトークン。</param>
        /// <returns>現在の位置で読み取りが失敗したことを示す例外。</returns>
		public Exception CreateTokenExpectedError(params string[] expected) =>
            new FormatException($"Unexpected {GetTokenKind(Current.ToString())} at line {Line}, column {Column}, {string.Join(" or ", expected.Select(GetTokenKind))} expected.");

        static string GetTokenKind(string s)
        {
            if (s == "\0")
                return "EOF";

            switch (s)
            {
                case " ":
                    return "SPACE";
                case "\t":
                    return "TAB";
                case "\n":
                    return "NEWLINE";
                default:
                    return s;
            }
        }
    }
}
