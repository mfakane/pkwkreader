using System;
using System.Collections.Generic;
using System.Linq;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// リストを表します。
    /// </summary>
	public class ListStatement : WikiStatement
    {
        int level;

        /// <summary>
        /// リストの入れ子段階を取得または設定します。
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
        /// 順序付きリストであるかどうかを取得または設定します。
        /// </summary>
        public bool IsOrdered { get; set; }

        /// <summary>
        /// 項目となる子要素を取得または設定します。
        /// </summary>
        public IList<WikiStatement> Items { get; set; }

        /// <summary>
        /// リストの入れ子段階、順序付きリストであるかどうか、および項目となる子要素を指定して、<see cref="ListStatement"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="level">リストの入れ子段階。</param>
        /// <param name="isOrdered">順序付きリストであるかどうか。</param>
        /// <param name="items">項目となる子要素。</param>
        public ListStatement(int level, bool isOrdered, IList<WikiStatement> items)
        {
            if (level < 1) throw new ArgumentException($"Value of {nameof(Level)} must be greater than zero.", nameof(level));

            Level = level;
            IsOrdered = isOrdered;
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// 指定したコンテキストからリストを表す <see cref="ListStatement"/> を読み取ります。
        /// </summary>
        /// <param name="context">読み取りに使用するコンテキスト。</param>
        /// <returns>読み取られたリストを表す <see cref="ListStatement"/>。</returns>
        public static new ListStatement Parse(ParseContext context)
        {
            var kind = context.Current;

            if (kind != '+' && kind != '-')
                throw context.CreateTokenExpectedError("+", "-");

            var level = context.TakeWhile(() => context.Current == kind).Length;
            var items = new List<WikiStatement>();
            var currentExpressions = new List<WikiExpression>();

            context.Index -= level;

            while (true)
            {
                if (context.Current == '+' ||
                    context.Current == '-')
                {
                    var prefix = context.TakeWhile(() => context.Current == '+' || context.Current == '-');

                    AddCurrentItem();

                    if (prefix[0] == kind && prefix.Length == level)
                    {
                        context.SkipWhiteSpaces();

                        if (context.Current == '\n')
                        {
                            context.Take("\n");

                            continue;
                        }
                        else if (context.Current == '~')
                            continue;

                        currentExpressions.Add(WikiExpression.Parse(context));

                        if (context.Current != '\n')
                            break;

                        context.Take("\n");
                    }
                    else
                    {
                        context.Index -= prefix.Length;

                        if (prefix.Length > level)
                            items.Add(Parse(context));
                        else
                            break;
                    }
                }
                else if (context.Current == '\n' || context.Current == '\0')
                    break;
                else if (IsStatementPrefix(context))
                {
                    var statement = WikiStatement.Parse(context);

                    if (currentExpressions.Any())
                        items.Add(new CompositeStatement(new[]
                        {
                            new ExpressionStatement(currentExpressions.Count == 1
                                ? currentExpressions[0]
                                : new CompositeExpression(currentExpressions)),
                            statement,
                        }));
                    else
                        items.Add(statement);

                    currentExpressions = new List<WikiExpression>();
                }
                else
                    currentExpressions.Add(WikiExpression.Parse(context));
            }

            AddCurrentItem();

            return new ListStatement(level, kind == '+', items);

            void AddCurrentItem()
            {
                if (currentExpressions.Any())
                {
                    items.Add(new ExpressionStatement(currentExpressions.Count == 1
                        ? currentExpressions[0]
                        : new CompositeExpression(currentExpressions)));
                    currentExpressions = new List<WikiExpression>();
                }
            }
        }

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
        public override string Convert(WikiContext context)
        {
            var tagName = IsOrdered ? "ol" : "ul";

            return $"<{tagName}>{string.Join(null, Items.Select(i => $"<li>{i.Convert(context)}</li>"))}</{tagName}>";
        }

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
        public override string ToWikiString()
        {
            var itemHeader = new string(IsOrdered ? '+' : '-', Level);

            return string.Join("\n", Items.Select(i => i is ListStatement ? i.ToWikiString() : itemHeader + " " + i.ToWikiString()));
        }
    }
}
