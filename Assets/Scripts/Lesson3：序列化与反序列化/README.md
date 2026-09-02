# 一、网络通信中的数据
网络通信最终传输的是**字节数据 `byte[]`**。类对象如果需要通过网络发送，需要先转换为字节数组。
```
发送端：
	类对象
	↓
	序列化
	↓
	byte[]
	↓
	网络传输

接收端：
	byte[]
	↓
	反序列化
	↓
	类对象
```

# 二、序列化与反序列化
## 1.序列化
序列化就是将程序中的数据转换为**可以保存或传输的格式**。例如：
```
PlayerInfo——byte[]
```
## 2.反序列化
反序列化是序列化的逆过程：
```
byte[]——PlayerInfo
```
网络通信双方必须约定相同的数据格式，例如：
```
前4字节   → 玩家ID
接下来4字节 → 名字长度
接下来N字节 → 名字
接下来4字节 → HP
```
否则接收方无法正确解析数据。

# 三、BitConverter
`BitConverter`用于**基础数据类型与字节数组之间的转换**。
## 1.数据转 byte[]
```
int value = 100;
byte[] bytes = BitConverter.GetBytes(value);
```
常用类型：
```
BitConverter.GetBytes(int);
BitConverter.GetBytes(float);
BitConverter.GetBytes(double);
BitConverter.GetBytes(bool);
BitConverter.GetBytes(long);
```

## 2.byte[] 还原数据
```
int value = BitConverter.ToInt32(bytes, 0);
```
第二个参数表示：
```
从 byte[] 的哪个位置开始读取
```
例如：
```
byte[] bytes = new byte[20];
int id = BitConverter.ToInt32(bytes, 0);
float hp = BitConverter.ToSingle(bytes, 4);
```
> 实际网络协议需要统一**字节序 Endianness**，不能默认双方机器的字节序一定相同。

# 四、Encoding
`Encoding`主要负责**字符串与字节数组之间的转换**。
网络开发通常使用：
```
Encoding.UTF8
```
## 1.string 转 byte[]
```
string str = "软件黑马王子";
byte[] bytes = Encoding.UTF8.GetBytes(str);
```
## 2.byte[] 转 string
```
string str = Encoding.UTF8.GetString(bytes);
```
字符串不能直接固定占用几个字节，因为：
```
"A"
"Unity"
"你好"
```
UTF-8 编码后的字节数量都可能不同。所以自定义二进制协议通常使用：
```
字符串长度 + 字符串内容
```
例如：
```
4字节
↓
字符串 byte 数量

N字节
↓
字符串数据
```

# 五、MemoryStream
`MemoryStream`表示内存中的字节流，可以用于组织一段连续的二进制数据。课件也将其列为二进制数据处理中常用的流对象。
例如：
```
using MemoryStream stream = new MemoryStream();

stream.Write(bytes, 0, bytes.Length);

byte[] result = stream.ToArray();
```
在网络协议中可以利用它依次拼接：
```
ID
+
名字长度
+
名字
+
HP
```
最终得到完整：
```
byte[]
```

# 六、BinaryFormatter
网络通信中不应该使用 `BinaryFormatter` 进行对象序列化，因为它不适合作为跨语言的网络通信格式。
现在还需要补充：
```
BinaryFormatter 不仅不适合网络通信 而且已经属于不安全、淘汰的 API
```
Microsoft 明确建议停止使用 `BinaryFormatter`；从 `.NET 9` 开始，运行时内置实现调用时会直接抛出异常。网络开发更常见的是：
```
自定义二进制协议
JSON
Protobuf
MessagePack
```