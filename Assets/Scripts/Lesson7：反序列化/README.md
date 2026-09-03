# 一、字节数组转基础类型
## 1.BitConverter
`BitConverter` 不仅可以把基础类型转换为 `byte[]`，也可以把 `byte[]` 还原为对应的数据类型。例如：
```
byte[] bytes = BitConverter.GetBytes(99);
int value = BitConverter.ToInt32(bytes, 0);
```
常用反序列化方法：
```
BitConverter.ToInt16()
BitConverter.ToInt32()
BitConverter.ToInt64()
BitConverter.ToSingle()
BitConverter.ToDouble()
BitConverter.ToBoolean()
```
第二个参数表示：
```
从 byte[] 的哪个索引位置开始读取
```
例如：
```
int value = BitConverter.ToInt32(bytes, index);
index += sizeof(int);
```

# 二、字节数组转字符串
## 1.Encoding.UTF8.GetString
字符串需要使用与序列化时相同的字符编码进行反序列化。
序列化：
```
byte[] bytes = Encoding.UTF8.GetBytes("黑马王子");
```
反序列化：
```
string value =Encoding.UTF8.GetString(bytes, 0, bytes.Length);
```
网络通信中通常统一使用：
```
Encoding.UTF8
```

# 三、类对象反序列化
对象反序列化就是把之前序列化得到的 `byte[]`，重新还原成类对象。
最重要的规则：
```
序列化顺序=反序列化顺序
```
例如序列化时：
```
等级
↓
名字长度
↓
名字
↓
攻击力
↓
性别
```
那么反序列化时也必须按照：
```
等级
↓
名字长度
↓
名字
↓
攻击力
↓
性别
```
依次读取。

# 四、index 读取位置
反序列化同样需要使用 `index` 记录当前读取到的位置。
```
int index = 0;
```
读取一个 `int`：
```
int value =BitConverter.ToInt32(bytes, index);
index += sizeof(int);
```
读取一个 `short`：
```
short value =BitConverter.ToInt16(bytes, index);
index += sizeof(short);
```
读取一个 `bool`：
```
bool value =BitConverter.ToBoolean(bytes, index);
index += sizeof(bool);
```
基本规律：
```
读取数据
↓
index += 当前数据占用字节数
↓
继续读取下一个数据
```

# 五、字符串反序列化
字符串长度不固定，所以不能直接读取。
序列化时通常保存：
```
字符串字节长度+字符串实际数据
```
因此反序列化时需要先读取长度：
```
int length =BitConverter.ToInt32(bytes, index);
index += sizeof(int);
```
再读取字符串：
```
string name =Encoding.UTF8.GetString(
        bytes,
        index,
        length
    );
index += length;
```
课件中的对象反序列化也是按照这种方式先读取字符串长度，再读取字符串内容。

# 六、反序列化流
```
获取 byte[]
↓
index = 0
↓
按照序列化顺序读取字段
↓
BitConverter 读取基础类型
↓
先读取字符串长度
↓
Encoding.UTF8 读取字符串
↓
每读取一个字段更新 index
↓
得到完整类对象
```

# 七、完整代码
下面把课件中的玩家数据换成一个**游戏账号信息**案例。数据结构：
```
AccountInfo
│
├─ userId      int
├─ nickname    string
├─ score       short
└─ isVip       bool
```
序列化后的结构：
```
┌────────┬────────────────┬────────────┬───────┬───────┐
│ userId │ nicknameLength │ nickname   │ score │ isVip │
├────────┼────────────────┼────────────┼───────┼───────┤
│ 4 Byte │     4 Byte     │   N Byte   │ 2Byte │ 1Byte │
└────────┴────────────────┴────────────┴───────┴───────┘
```
```
using System;
using System.Text;
using UnityEngine;

/// <summary>
/// 游戏账号信息
/// </summary>
public class AccountInfo
{
    public int userId;
    public string nickname;
    public short score;
    public bool isVip;

    /// <summary>
    /// 将账号信息序列化为 byte[]
    /// </summary>
    public byte[] GetBytes()
    {
        byte[] nameBytes =
            Encoding.UTF8.GetBytes(nickname);

        int byteLength =
            sizeof(int) +              //userId
            sizeof(int) +              //nickname长度
            nameBytes.Length +         //nickname
            sizeof(short) +            //score
            sizeof(bool);              //isVip

        byte[] bytes = new byte[byteLength];

        int index = 0;

        //用户ID
        BitConverter
            .GetBytes(userId)
            .CopyTo(bytes, index);

        index += sizeof(int);


        //昵称长度
        BitConverter
            .GetBytes(nameBytes.Length)
            .CopyTo(bytes, index);

        index += sizeof(int);


        //昵称
        nameBytes.CopyTo(bytes, index);

        index += nameBytes.Length;


        //积分
        BitConverter
            .GetBytes(score)
            .CopyTo(bytes, index);

        index += sizeof(short);


        //VIP状态
        BitConverter
            .GetBytes(isVip)
            .CopyTo(bytes, index);


        return bytes;
    }

    /// <summary>
    /// 将 byte[] 反序列化为账号信息
    /// </summary>
    public void ReadBytes(byte[] bytes)
    {
        int index = 0;

        //用户ID
        userId =
            BitConverter.ToInt32(
                bytes,
                index
            );

        index += sizeof(int);


        //昵称字节长度
        int nameLength =
            BitConverter.ToInt32(
                bytes,
                index
            );

        index += sizeof(int);


        //昵称
        nickname =
            Encoding.UTF8.GetString(
                bytes,
                index,
                nameLength
            );

        index += nameLength;


        //积分
        score =
            BitConverter.ToInt16(
                bytes,
                index
            );

        index += sizeof(short);


        //VIP状态
        isVip =
            BitConverter.ToBoolean(
                bytes,
                index
            );
    }
}


/// <summary>
/// 账号数据使用案例
/// </summary>
public class AccountDataExample : MonoBehaviour
{
    private void Start()
    {
        //原始账号数据
        AccountInfo account = new AccountInfo
        {
            userId = 10086,
            nickname = "黑马王子",
            score = 520,
            isVip = true
        };

        //序列化
        byte[] bytes = account.GetBytes();

        //模拟网络另一端收到 byte[]
        AccountInfo receiveAccount =
            new AccountInfo();

        //反序列化
        receiveAccount.ReadBytes(bytes);

        Debug.Log($"用户ID：{receiveAccount.userId}");
        Debug.Log($"昵称：{receiveAccount.nickname}");
        Debug.Log($"积分：{receiveAccount.score}");
        Debug.Log($"是否VIP：{receiveAccount.isVip}");
    }
}
```
这节课最核心的是：
```
序列化：数据 → byte[]
反序列化：byte[] → 数据
```
并且必须保证：
```
写入顺序 = 读取顺序
写入类型 = 读取类型
字符串编码 = 字符串解码
```
否则就会出现数据错位或解析错误。