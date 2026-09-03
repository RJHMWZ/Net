# 一、反序列化基类封装
上一节已经把不同数据类型的**序列化**操作封装到了 `BaseData` 中，这一节继续把**反序列化**也统一封装。所有需要进行二进制转换的数据类统一继承：
```
BaseData
```
基类需要提供三个核心方法：
```
GetBytesNum()   //计算序列化后的字节数
Writing()       //对象 → byte[]
Reading()       //byte[] → 对象
```

# 二、Reading 方法
## 1.Reading
`Reading()` 用于将字节数组中的数据反序列化到当前对象：
```
public abstract int Reading(
    byte[] bytes,
    int beginIndex = 0
);
```
参数：
```
bytes      → 需要解析的字节数组
beginIndex → 从 byte[] 的哪个位置开始读取
```
返回值：
```
本次对象一共读取了多少个字节
```
例如：
```
int index = beginIndex;
//读取成员变量
...
return index - beginIndex;
```
使用 `beginIndex` 的主要原因是，一个对象可能被嵌套在另一个对象的 `byte[]` 中，并不一定从索引 `0` 开始读取。

# 三、基础类型读取封装
序列化时：
```
数据
↓
BitConverter.GetBytes()
↓
byte[]
```
反序列化时则相反：
```
byte[]
↓
BitConverter.ToXXX()
↓
数据
```
例如读取 `int`：
```
protected int ReadInt(
    byte[] bytes,
    ref int index)
{
    int value =
        BitConverter.ToInt32(bytes, index);
    index += sizeof(int);
    return value;
}
```
调用：
```
hp = ReadInt(bytes, ref index);
```
读取完成后 `index` 会自动移动到下一个数据的位置。
常用方法：
```
ReadInt()
ReadShort()
ReadLong()
ReadFloat()
ReadByte()
ReadBool()
```

# 四、字符串读取
序列化字符串时的数据结构为：
```
字符串字节长度+UTF-8 字符串数据
```
因此读取时必须先读取长度：
```
int length =ReadInt(bytes, ref index);
```
再按照长度读取字符串：
```
string value =
    Encoding.UTF8.GetString(
        bytes,
        index,
        length
    );

index += length;
```
完整封装：
```
protected string ReadString(
    byte[] bytes,
    ref int index)
{
    int length =
        ReadInt(bytes, ref index);

    string value =
        Encoding.UTF8.GetString(
            bytes,
            index,
            length
        );

    index += length;

    return value;
}
```

# 五、自定义对象读取
数据类内部还可能包含其他 `BaseData` 对象。
例如：
```
PlayerProfile
│
├─ level
├─ hp
├─ nickname
└─ WeaponInfo
      ├─ weaponId
      └─ attack
```
可以使用泛型方法统一读取：
```
protected T ReadData<T>(
    byte[] bytes,
    ref int index)
    where T : BaseData, new()
{
    T value = new T();

    index += value.Reading(bytes, index);

    return value;
}
```
其中：
```
where T : BaseData, new()
```
表示 `T`：
```
必须继承 BaseData
必须具有可访问的无参构造函数
```
然后就可以直接：
```
weapon =
    ReadData<WeaponInfo>(
        bytes,
        ref index
    );
```

# 六、序列化与反序列化对应关系
一个数据类中的：
```
Writing()
```
和：
```
Reading()
```
字段顺序必须完全一致。
例如序列化：
```
WriteShort(bytes, level, ref index);
WriteData(bytes, weapon, ref index);
WriteInt(bytes, hp, ref index);
WriteString(bytes, nickname, ref index);
WriteBool(bytes, isOnline, ref index);
```
反序列化就必须：
```
level =ReadShort(bytes, ref index);
weapon =ReadData<WeaponInfo>(bytes, ref index);
hp =ReadInt(bytes, ref index);
nickname =ReadString(bytes, ref index);
isOnline =ReadBool(bytes, ref index);
```
课件中的数据类也是按照相同顺序完成 `Writing()` 与 `Reading()`。

# 七、完整代码
下面使用**角色信息 + 武器信息**替换课件中的 `TestInfo + Player`。
数据结构：
```
PlayerProfile
│
├─ level       short
├─ weapon      WeaponInfo
├─ hp          int
├─ nickname    string
└─ isOnline    bool

WeaponInfo
│
├─ weaponId    int
└─ attack      short
```
## 1.BaseData.cs
```
using System;
using System.Text;

/// <summary>
/// 网络二进制数据基类
/// 统一负责序列化和反序列化的基础操作
/// </summary>
public abstract class BaseData
{
    /// <summary>
    /// 获取当前对象序列化后的总字节数
    /// </summary>
    public abstract int GetBytesNum();

    /// <summary>
    /// 将当前对象序列化为 byte[]
    /// </summary>
    public abstract byte[] Writing();

    /// <summary>
    /// 将 byte[] 反序列化到当前对象
    /// </summary>
    /// <param name="bytes">需要解析的字节数组</param>
    /// <param name="beginIndex">开始读取的位置</param>
    /// <returns>本次读取的总字节数</returns>
    public abstract int Reading(
        byte[] bytes,
        int beginIndex = 0
    );


    #region 写入数据

    protected void WriteInt(
        byte[] bytes,
        int value,
        ref int index)
    {
        BitConverter
            .GetBytes(value)
            .CopyTo(bytes, index);

        index += sizeof(int);
    }

    protected void WriteShort(
        byte[] bytes,
        short value,
        ref int index)
    {
        BitConverter
            .GetBytes(value)
            .CopyTo(bytes, index);

        index += sizeof(short);
    }

    protected void WriteLong(
        byte[] bytes,
        long value,
        ref int index)
    {
        BitConverter
            .GetBytes(value)
            .CopyTo(bytes, index);

        index += sizeof(long);
    }

    protected void WriteFloat(
        byte[] bytes,
        float value,
        ref int index)
    {
        BitConverter
            .GetBytes(value)
            .CopyTo(bytes, index);

        index += sizeof(float);
    }

    protected void WriteByte(
        byte[] bytes,
        byte value,
        ref int index)
    {
        bytes[index] = value;

        index += sizeof(byte);
    }

    protected void WriteBool(
        byte[] bytes,
        bool value,
        ref int index)
    {
        BitConverter
            .GetBytes(value)
            .CopyTo(bytes, index);

        index += sizeof(bool);
    }

    protected void WriteString(
        byte[] bytes,
        string value,
        ref int index)
    {
        byte[] strBytes =
            Encoding.UTF8.GetBytes(value);

        //先写字符串字节长度
        WriteInt(
            bytes,
            strBytes.Length,
            ref index
        );

        //再写字符串内容
        strBytes.CopyTo(bytes, index);

        index += strBytes.Length;
    }

    protected void WriteData(
        byte[] bytes,
        BaseData data,
        ref int index)
    {
        byte[] dataBytes = data.Writing();

        dataBytes.CopyTo(bytes, index);

        index += dataBytes.Length;
    }

    #endregion


    #region 读取数据

    protected int ReadInt(
        byte[] bytes,
        ref int index)
    {
        int value =
            BitConverter.ToInt32(
                bytes,
                index
            );

        index += sizeof(int);

        return value;
    }

    protected short ReadShort(
        byte[] bytes,
        ref int index)
    {
        short value =
            BitConverter.ToInt16(
                bytes,
                index
            );

        index += sizeof(short);

        return value;
    }

    protected long ReadLong(
        byte[] bytes,
        ref int index)
    {
        long value =
            BitConverter.ToInt64(
                bytes,
                index
            );

        index += sizeof(long);

        return value;
    }

    protected float ReadFloat(
        byte[] bytes,
        ref int index)
    {
        float value =
            BitConverter.ToSingle(
                bytes,
                index
            );

        index += sizeof(float);

        return value;
    }

    protected byte ReadByte(
        byte[] bytes,
        ref int index)
    {
        byte value = bytes[index];

        index += sizeof(byte);

        return value;
    }

    protected bool ReadBool(
        byte[] bytes,
        ref int index)
    {
        bool value =
            BitConverter.ToBoolean(
                bytes,
                index
            );

        index += sizeof(bool);

        return value;
    }

    protected string ReadString(
        byte[] bytes,
        ref int index)
    {
        //读取字符串的字节长度
        int length =
            ReadInt(bytes, ref index);

        //读取字符串内容
        string value =
            Encoding.UTF8.GetString(
                bytes,
                index,
                length
            );

        index += length;

        return value;
    }

    protected T ReadData<T>(
        byte[] bytes,
        ref int index)
        where T : BaseData, new()
    {
        T value = new T();

        //Reading 返回该对象读取的字节数量
        index += value.Reading(
            bytes,
            index
        );

        return value;
    }

    #endregion
}
```
## 2.WeaponInfo.cs
```
/// <summary>
/// 角色武器信息
/// </summary>
public class WeaponInfo : BaseData
{
    public int weaponId;
    public short attack;

    /// <summary>
    /// 计算武器数据占用字节数
    /// </summary>
    public override int GetBytesNum()
    {
        return
            sizeof(int) +
            sizeof(short);
    }

    /// <summary>
    /// 武器数据序列化
    /// </summary>
    public override byte[] Writing()
    {
        byte[] bytes =
            new byte[GetBytesNum()];

        int index = 0;

        WriteInt(
            bytes,
            weaponId,
            ref index
        );

        WriteShort(
            bytes,
            attack,
            ref index
        );

        return bytes;
    }

    /// <summary>
    /// 武器数据反序列化
    /// </summary>
    public override int Reading(
        byte[] bytes,
        int beginIndex = 0)
    {
        int index = beginIndex;

        weaponId =
            ReadInt(bytes, ref index);

        attack =
            ReadShort(bytes, ref index);

        return index - beginIndex;
    }
}
```

## 3.PlayerProfile.cs

```
using System.Text;

/// <summary>
/// 玩家角色信息
/// </summary>
public class PlayerProfile : BaseData
{
    public short level;

    public WeaponInfo weapon;

    public int hp;

    public string nickname;

    public bool isOnline;

    /// <summary>
    /// 计算完整角色数据占用的字节数
    /// </summary>
    public override int GetBytesNum()
    {
        return
            sizeof(short) +                       //level
            weapon.GetBytesNum() +                //weapon
            sizeof(int) +                         //hp
            sizeof(int) +                         //nickname长度
            Encoding.UTF8.GetByteCount(nickname) +//nickname
            sizeof(bool);                         //isOnline
    }

    /// <summary>
    /// 玩家数据序列化
    /// </summary>
    public override byte[] Writing()
    {
        byte[] bytes =
            new byte[GetBytesNum()];

        int index = 0;

        WriteShort(
            bytes,
            level,
            ref index
        );

        WriteData(
            bytes,
            weapon,
            ref index
        );

        WriteInt(
            bytes,
            hp,
            ref index
        );

        WriteString(
            bytes,
            nickname,
            ref index
        );

        WriteBool(
            bytes,
            isOnline,
            ref index
        );

        return bytes;
    }

    /// <summary>
    /// 玩家数据反序列化
    /// </summary>
    public override int Reading(
        byte[] bytes,
        int beginIndex = 0)
    {
        int index = beginIndex;

        level =
            ReadShort(
                bytes,
                ref index
            );

        weapon =
            ReadData<WeaponInfo>(
                bytes,
                ref index
            );

        hp =
            ReadInt(
                bytes,
                ref index
            );

        nickname =
            ReadString(
                bytes,
                ref index
            );

        isOnline =
            ReadBool(
                bytes,
                ref index
            );

        return index - beginIndex;
    }
}
```

## 4.实际使用

例如服务器发送一个角色的数据：

```
PlayerProfile player = new PlayerProfile();

player.level = 35;

player.weapon = new WeaponInfo();
player.weapon.weaponId = 2003;
player.weapon.attack = 156;

player.hp = 850;
player.nickname = "暗夜剑士";
player.isOnline = true;


//对象 → byte[]
byte[] sendBytes = player.Writing();
```

客户端收到：

```
byte[] receiveBytes = sendBytes;

PlayerProfile receivePlayer =
    new PlayerProfile();

//byte[] → 对象
receivePlayer.Reading(receiveBytes);
```

现在：

```
receivePlayer.level;
receivePlayer.weapon.weaponId;
receivePlayer.weapon.attack;
receivePlayer.hp;
receivePlayer.nickname;
receivePlayer.isOnline;
```

就已经恢复成发送之前的数据。

整个结构可以记成：

```
                    BaseData
                       │
        ┌──────────────┴──────────────┐
        │                             │
     Writing                       Reading
        │                             │
    对象 → byte[]                 byte[] → 对象
        │                             │
   WriteInt()                     ReadInt()
   WriteShort()                   ReadShort()
   WriteString()                  ReadString()
   WriteData()                    ReadData<T>()
```

这里新增的核心就是 `Reading()` 和 `ReadData<T>()`：前者让每个数据类拥有统一的反序列化入口，后者解决了**对象中嵌套其他自定义对象时的反序列化问题**。