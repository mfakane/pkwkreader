# PkwkReader

PukiWiki などで使用されている Wiki 構文をパースするライブラリです。

## 使い方

```
// using Linearstar.Core.PkwkReader;

var config = new WikiConfiguration();
var doc = WikiDocument.FromFile("source.txt", config);
```