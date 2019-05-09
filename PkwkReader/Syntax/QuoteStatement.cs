using System;
using System.Collections.Generic;
using System.Linq;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 引用を表します。
    /// </summary>
	public class QuoteStatement : WikiStatement
    {
        int level;

        /// <summary>
        /// 引用の入れ子段階を取得または設定します。
        /// </summary>
        public int Level
        {
            get => level;
            set
            {
                if (level < 1) throw new ArgumentException($"Value of {nameof(Level)} must be greater than zero.", nameof(value));

                level = value;
            }
        }

        /// <summary>
        /// 内容となる子要素を取得または設定します。
        /// </summary>
        public IList<WikiStatement> Content { get; set; }

        /// <summary>
        /// 引用の入れ子段階、および内容となる子要素を指定して、<see cref="QuoteStatement"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="level">引用の入れ子段階。</param>
        /// <param name="content">内容となる子要素。</param>
		public QuoteStatement(int level, IList<WikiStatement> content)
        {
            if (level < 1) throw new ArgumentException($"Value of {nameof(Level)} must be greater than zero.", nameof(level));

            Level = level;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        /// 指定したコンテキストから引用を表す <see cref="QuoteStatement"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られた引用を表す <see cref="QuoteStatement"/>。</returns>
		public static new QuoteStatement Parse(ParseContext context)
        {
            var level = context.TakeWhile(() => context.Current == '>').Length;

            context.Index -= level;

            var content = new List<WikiStatement>();

            do
            {
                var prefix = context.TakeWhile(() => context.Current == '>' || context.Current == '<');

                if (prefix[0] == '<')
                {
                    context.TakeUntilAny("\n", "\0");

                    break;
                }
                else if (prefix.Length == level)
                {
                    context.SkipWhiteSpaces();

                    if (context.Current == '\0' ||
                        context.Current == '\n')
                        context.TakeUntilAny("\n");
                    else
                        content.Add(WikiStatement.Parse(context));
                }
                else if (prefix.Length > level)
                {
                    context.Index -= prefix.Length;
                    content.Add(Parse(context));
                }
                else if (prefix.Length == 0)
                    if (context.Current == '\0' ||
                        context.Current == '\n')
                    {
                        context.TakeUntilAny("\n");

                        break;
                    }
                    else
                        content.Add(WikiStatement.Parse(context));
                else
                    break;
            }
            while (context.Current != '\n'
                && context.Current != '\0');

            return new QuoteStatement(level, content);
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
        public override string Convert(WikiContext context) =>
            $"<blockquote>{string.Join("\n", Content.Select(i => i.Convert(context)))}</blockquote>";

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
        public override string ToWikiString()
        {
            var prefix = new string('>', Level);

            return string.Join("\n", Content.Select(i => i is QuoteStatement ? i.ToWikiString() : prefix + " " + i.ToWikiString()));
        }
    }
}
