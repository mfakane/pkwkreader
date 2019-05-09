﻿using System;

namespace Linearstar.Core.PkwkReader.Syntax
{
    /// <summary>
    /// 単一のインライン要素を含むブロック要素を表します。
    /// </summary>
	public class ExpressionStatement : WikiStatement
    {
        /// <summary>
        /// 現在の要素に含まれる子要素を取得または設定します。
        /// </summary>
		public WikiExpression Content { get; set; }

        /// <summary>
        /// 含まれる子要素を指定して、<see cref="ExpressionStatement"/> の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="content">含まれる子要素。</param>
		public ExpressionStatement(WikiExpression content) =>
            Content = content ?? throw new ArgumentNullException(nameof(content));

        /// <summary>
        /// 指定したコンテキストを使用して、現在の要素を変換します。
        /// </summary>
        /// <param name="context">変換に使用するコンテキスト。</param>
        /// <returns>変換結果を表す文字列。</returns>
		public override string Convert(WikiContext context) =>
            Content.Convert(context);

        /// <summary>
        /// 現在の要素の Wiki 構文表現を取得します。
        /// </summary>
        /// <returns>要素の Wiki 構文表現。</returns>
		public override string ToWikiString() =>
            Content.ToWikiString();
    }
}