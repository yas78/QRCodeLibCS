# QRCodeLibCS
QRCodeLibCSは、C#で書かれたQRコード生成ライブラリです。  
JIS X 0510に基づくモデル２コードシンボルを生成します。

## 特徴
- 数字・英数字・8ビットバイト・漢字モードに対応しています
- 分割QRコードを作成可能です
- 1bppまたは24bpp BMPファイル(DIB)へ保存可能です
- 1bppまたは24bpp Imageオブジェクトとして取得可能です  
- 画像の配色(前景色・背景色)を指定可能です
- 8ビットバイトモードでの文字コードを指定可能です


## クイックスタート
QRCodeLibプロジェクト、またはビルドした QRCodeLib.dll を参照設定してください。


## 使用方法
### 例１．単一シンボルで構成される(分割QRコードではない)QRコードの、最小限のコードを示します。

```csharp
using Ys.QRCode;
using System.Drawing;

public void Example()
{
    Symbols symbols = new Symbols();
    symbols.AppendString("012345abcdefg");

    Image image = symbols[0].Get24bppImage();
}
```

### 例２．誤り訂正レベルを指定する
Symbolsクラスのコンストラクタ引数に、ErrorCorrectionLevel列挙型の値を設定します。

```csharp
Symbols symbols = new Symbols(ErrorCorrectionLevel.H);
```

### 例３．型番の上限を指定する
Symbolsクラスのコンストラクタで設定します。
```csharp
Symbols symbols = new Symbols(maxVersion: 10);
```

### 例４．8ビットバイトモードで使用する文字コードを指定する
Symbolsクラスのコンストラクタで設定します。
```csharp
Symbols symbols = new Symbols(byteModeEncoding: "utf-8");
```

### 例５．分割QRコードを作成する
Symbolsクラスのコンストラクタで設定します。型番の上限を指定しない場合は、型番40を上限として分割されます。

```csharp
Symbols symbols = new Symbols(allowStructuredAppend: true);
```

型番1を超える場合に分割し、各QRコードのImageオブジェクトを取得する例を示します。

```csharp
Symbols symbols = new Symbols(maxVersion: 1, allowStructuredAppend: true);
symbols.AppendString("abcdefghijklmnopqrstuvwxyz");

foreach (var symbol in symbols)
{
    Image image = symbol.Get24bppImage();
}
```

### 例６．BMPファイルへ保存する
SymbolクラスのSave1bppDIB、またはSave24bppDIBメソッドを使用します。

```csharp
Symbols symbols = new Symbols();
symbols.AppendString("012345abcdefg");

symbols[0].Save1bppDIB(@"D:\qrcode1bpp1.bmp");
symbols[0].Save1bppDIB(@"D:\qrcode1bpp2.bmp", 10); // 10 pixels per module
symbols[0].Save24bppDIB(@"D:\qrcode24bpp1.bmp");
symbols[0].Save24bppDIB(@"D:\qrcode24bpp2.bmp", 10); // 10 pixels per module
```

### 例７．様々な画像形式で保存する
ImageオブジェクトのSaveメソッドを使用します。

```csharp
using System.Drawing;
using System.Drawing.Imaging;

Symbols symbols = new Symbols();
symbols.AppendString("012345");

Image image = symbols[0].Get24bppImage();
// PNG
image.Save(@"D:\qrcode.png", ImageFormat.Png);
// GIF
image.Save(@"D:\qrcode.gif", ImageFormat.Gif);
// JPEG
image.Save(@"D:\qrcode.jpg", ImageFormat.Jpeg);
```
