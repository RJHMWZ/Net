# 一、字符编码
## 1.什么是字符编码
计算机最终存储和传输的是二进制数据，因此文字需要按照一定规则转换成数字，这种规则就是**字符编码**。简单理解：
```
字符
↓
编码
↓
byte[]
↓
存储 / 网络传输
```
接收时：
```
byte[]
↓
解码
↓
字符
```
例如 ASCII 中：
```
'A'
↓
十进制：65
↓
二进制：0100 0001
```
常见字符编码：
```
ASCII
GB2312 / GBK
Shift_JIS
UTF-8
UTF-16
UTF-32
```

# 二、乱码
## 1.乱码产生原因
乱码最常见的原因是：编码时使用的规则≠解码时使用的规则
例如：
```
字符串
↓
UTF-8 编码
↓
byte[]
↓
使用其他编码错误解码
↓
乱码
```
所以网络通信中必须保证：
```
客户端编码格式=服务端解码格式
```
通常统一使用：
```
UTF-8
```

# 三、ASCII
## 1.ASCII 编码
`ASCII（American Standard Code for Information Interchange）`是早期用于英文字符的编码标准。ASCII 使用 `7 bit`，一共定义：
```
2^7 = 128 个字符
```
范围0 ～ 127
例如：
```
'A' → 65
'a' → 97
'0' → 48
'1' → 49
```
虽然通常存放在一个 `byte` 中，但最高位保持为 `0`。
```
A
十进制：65
二进制：0100 0001
```
ASCII 只能表示：
```
英文字母
数字
标点符号
控制字符
```
不能直接表示中文、日文等字符。

# 四、传统字符编码
ASCII 无法表示世界各国语言，因此不同国家和地区曾发展出自己的字符编码。
例如：
```
简体中文 → GB2312 / GBK
繁体中文 → Big5
日文     → Shift_JIS
韩文     → EUC-KR
```
这些编码通常兼容 ASCII 的 `0～127` 范围，但 ASCII 之外的字符编码规则并不统一。
因此：
```
相同 byte[]+不同编码方式解码=可能得到完全不同的字符
```
这也是早期乱码问题非常常见的重要原因。
> `GB2312` 并不是简单的“任意两个字节都可以表示一个汉字”，它有自己规定的有效字节范围和字符表，因此不能直接用 `256 × 256` 理解实际可表示字符数量。

# 五、Unicode
## 1.Unicode
`Unicode` 是统一的**字符集和字符编码标准**，目标是为世界上的字符分配统一的编号。
这个编号称为：
```
Code Point
代码点
```
例如：
```
A → U+0041
中 → U+4E2D
```
注意：
```
Unicode ≠ UTF-8
```
Unicode 主要规定：
```
字符
↓
对应哪个 Unicode Code Point
```
而具体如何转换成字节进行存储和传输，由编码形式决定。常见 Unicode 编码形式：
```
UTF-8
UTF-16
UTF-32
```

# 六、UTF-8
## 1.UTF-8
`UTF-8` 是 Unicode 最常用的编码形式之一，也是网络和 Web 开发中最常见的文本编码。
UTF-8 使用**变长编码**：
```
1 ～ 4 Byte
```
不同字符占用的字节数不同。
常见情况：
```
ASCII 字符        → 1 Byte
部分欧洲字符      → 2 Byte
常用中文字符      → 3 Byte
部分特殊字符/Emoji → 4 Byte
```
例如：
```
"A"UTF-8 → 1 Byte

"中"UTF-8 → 3 Byte
```
UTF-8 最大的优势之一是：
```
ASCII 0～127与 UTF-8 完全兼容
```
因此英文文本使用 UTF-8 时不会产生额外的编码变化。

# 七、UTF-16 与 UTF-32
## 1.UTF-16
UTF-16 使用：
```
2 Byte或4 Byte
```
表示一个 Unicode 代码点。C# 的 `string` 和 `char` 采用 UTF-16 编码单元表示，因此：
```
char
```
占：
```
16 bit = 2 Byte
```
但要注意，一个 Unicode 字符**不一定等于一个 `char`**。例如部分 Emoji 需要两个 UTF-16 `char` 共同表示。
## 2.UTF-32
UTF-32 通常使用：
```
4 Byte
```
表示一个 Unicode 代码点。
优点：
```
编码长度固定
处理代码点较直接
```
缺点：
```
占用空间较大
```
因此网络通信通常更常使用 UTF-8。

# 八、C# Encoding 类
## 1.Encoding
C# 中主要通过：
```
System.Text.Encoding
```
完成字符串和字节数组之间的转换。
常用编码：
```
Encoding.ASCII
Encoding.UTF8
Encoding.Unicode
Encoding.UTF32
```
其中：
```
Encoding.Unicode
```
在 .NET 中特指：
```
UTF-16 Little Endian
```
并不是“所有 Unicode 编码”的意思。
## 2.string 转 byte[]
UTF-8 编码：
```
string str = "Unity网络开发";
byte[] bytes = Encoding.UTF8.GetBytes(str);
```
过程：
```
string
↓
Encoding.UTF8.GetBytes()
↓
byte[]
```
## 3.byte[] 转 string
```
string str = Encoding.UTF8.GetString(bytes);
```
过程：
```
byte[]
↓
Encoding.UTF8.GetString()
↓
string
```
网络通信中通常使用：
```
Encoding.UTF8
```
保证客户端和服务端使用相同编码即可避免大部分文本乱码问题。

# 九、字符串长度与字节长度
这是网络开发中需要特别注意的地方。
例如：
```
string str = "ABC你好";
```
字符数量：
```
str.Length
```
和 UTF-8 编码后的字节数量：
```
Encoding.UTF8.GetByteCount(str)
```
**不一定相同。**
例如：
```
"A"  → 1 个 UTF-8 字节
"你" → 3 个 UTF-8 字节
```
所以设计网络协议时，字符串长度一般应该记录：
```
字符串编码后的 byte 长度
```
而不是直接记录：
```
str.Length
```
例如：
```
4 Byte字符串字节长度
N ByteUTF-8 字符串数据
```