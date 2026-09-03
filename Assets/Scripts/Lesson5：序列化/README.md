# 一、基础类型转字节数组

## 1.BitConverter
`BitConverter` 位于 `System` 命名空间，主要用于基础类型与 `byte[]` 之间的转换。
```
using System;
```
常见写法：
```
byte[] intBytes = BitConverter.GetBytes(100);
byte[] shortBytes = BitConverter.GetBytes((short)88);
byte[] boolBytes = BitConverter.GetBytes(true);
byte[] floatBytes = BitConverter.GetBytes(99.5f);
```
常见类型占用：

|类型|字节数|
|---|---|
|`bool`|1|
|`short`|2|
|`int`|4|
|`long`|8|
|`float`|4|
|`double`|8|

# 二、字符串转字节数组
## 1.Encoding
字符串使用 `Encoding` 进行编码转换，位于：
```
using System.Text;
```
网络通信通常统一使用：
```
Encoding.UTF8
```
字符串转字节数组：
```
string name = "软件黑马王子";
byte[] nameBytes = Encoding.UTF8.GetBytes(name);
```
字符串长度不能直接使用 `name.Length` 作为网络中的字节长度，应使用编码后的实际长度：
```
byte[] nameBytes = Encoding.UTF8.GetBytes(name);
int byteLength = nameBytes.Length;
```

# 三、类对象序列化为 byte[]
## 1.基本思路
网络通信时，需要将类对象中的各个字段按照约定顺序写入同一个 `byte[]`。
例如：
```
public class PlayerInfo
{
    public int lev;
    public string name;
    public short atk;
    public bool sex;
}
```
定义二进制数据结构：
```
int     lev         → 4 Byte
int     nameLength  → 4 Byte
byte[]  name        → N Byte
short   atk         → 2 Byte
bool    sex         → 1 Byte
```
最终数据排列：
```
┌─────┬────────────┬──────────┬─────┬─────┐
│ lev │ nameLength │   name   │ atk │ sex │
└─────┴────────────┴──────────┴─────┴─────┘
 4Byte     4Byte      N Byte   2Byte 1Byte
```
## 2.为什么要保存字符串长度
字符串是变长数据：
```
"Tom"
"孙悟空"
"软件黑马王子"
```
转换为 UTF-8 后长度都不同。
因此序列化字符串时通常先保存：
```
字符串字节长度
↓
字符串实际字节数据
```
例如：
```
byte[] nameBytes = Encoding.UTF8.GetBytes(name);
int nameLength = nameBytes.Length;
```
写入顺序：
```
nameLength
↓
nameBytes
```
这样后续反序列化时，才能知道应该读取多少字节作为字符串。

# 四、计算字节数组容量
序列化之前，需要先计算最终 `byte[]` 的大小：
```
int length =
    sizeof(int) +
    sizeof(int) +
    Encoding.UTF8.GetBytes(name).Length +
    sizeof(short) +
    sizeof(bool);
```
分别对应：
```
sizeof(int)      → lev
sizeof(int)      → nameLength
nameBytes.Length → name
sizeof(short)    → atk
sizeof(bool)     → sex
```
然后创建：
```
byte[] data = new byte[length];
```

# 五、CopyTo 写入数据
## 1.index
使用 `index` 记录当前应该从字节数组的哪个位置继续写入：
```
int index = 0;
```
每写入一个字段，都需要移动 `index`。例如写入等级：
```
BitConverter.GetBytes(lev).CopyTo(data, index);
index += sizeof(int);
```
写入攻击力：
```
BitConverter.GetBytes(atk).CopyTo(data, index);
index += sizeof(short);
```
`CopyTo` 第二个参数表示：
```
从目标 byte[] 的哪个索引开始写入
```
源码也是通过不断移动 `index`，依次将对象中的字段写入同一个字节数组。

# 六、对象序列化流程
类对象转换为 `byte[]` 的基本流程：
```
1.将字符串提前转换成 UTF-8 byte[]
↓
2.计算最终 byte[] 总长度
↓
3.创建 byte[] 容器
↓
4.创建 index = 0
↓
5.BitConverter 转换基础类型
↓
6.Encoding.UTF8 转换字符串
↓
7.CopyTo 按顺序写入 byte[]
↓
8.每写入一个字段更新 index
↓
9.返回完整 byte[]
```

# 七、完整测试代码

```
using System;
using System.Text;
using UnityEngine;

public class PlayerInfo
{
    public int lev;
    public string name;
    public short atk;
    public bool sex;

    /// <summary>
    /// 将玩家数据序列化为字节数组
    /// </summary>
    public byte[] GetBytes()
    {
        //1.字符串先转换为UTF-8字节数组
        byte[] nameBytes = Encoding.UTF8.GetBytes(name);

        //2.计算最终字节数组长度
        int length =
            sizeof(int) +         //lev
            sizeof(int) +         //name字节长度
            nameBytes.Length +    //name数据
            sizeof(short) +       //atk
            sizeof(bool);         //sex

        //3.创建最终字节数组
        byte[] data = new byte[length];

        //当前写入位置
        int index = 0;

        //========================
        //lev
        //========================

        byte[] levBytes = BitConverter.GetBytes(lev);

        levBytes.CopyTo(data, index);

        index += sizeof(int);


        //========================
        //name长度
        //========================

        byte[] nameLengthBytes =
            BitConverter.GetBytes(nameBytes.Length);

        nameLengthBytes.CopyTo(data, index);

        index += sizeof(int);


        //========================
        //name
        //========================

        nameBytes.CopyTo(data, index);

        index += nameBytes.Length;


        //========================
        //atk
        //========================

        byte[] atkBytes = BitConverter.GetBytes(atk);

        atkBytes.CopyTo(data, index);

        index += sizeof(short);


        //========================
        //sex
        //========================

        byte[] sexBytes = BitConverter.GetBytes(sex);

        sexBytes.CopyTo(data, index);

        index += sizeof(bool);


        return data;
    }
}

public class BinarySerializeTest : MonoBehaviour
{
    private void Start()
    {
        PlayerInfo player = new PlayerInfo
        {
            lev = 10,
            name = "唐老狮",
            atk = 88,
            sex = false
        };

        //对象序列化
        byte[] data = player.GetBytes();

        Debug.Log($"序列化总长度：{data.Length} Byte");

        //打印每一个字节
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            builder.Append(data[i]);

            if (i < data.Length - 1)
            {
                builder.Append(" ");
            }
        }

        Debug.Log($"序列化结果：{builder}");
    }
}
```
这份数据最终在 `byte[]` 中的布局就是：
```
PlayerInfo
↓
┌────────┬────────────┬───────────┬────────┬────────┐
│  lev   │ nameLength │   name    │  atk   │  sex   │
├────────┼────────────┼───────────┼────────┼────────┤
│ 4 Byte │   4 Byte   │  N Byte   │ 2 Byte │ 1 Byte │
└────────┴────────────┴───────────┴────────┴────────┘
↓
byte[]
```