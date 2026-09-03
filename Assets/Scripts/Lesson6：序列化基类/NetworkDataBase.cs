using System;
using System.Text;

/// <summary>
/// 网络二进制数据基类
/// 统一提供基础类型写入 byte[] 的方法
/// </summary>
public abstract class NetworkDataBase
{
    /// <summary>
    /// 获取当前数据序列化后的总字节数
    /// </summary>
    public abstract int GetBytesNum();

    /// <summary>
    /// 将当前对象序列化为 byte[]
    /// </summary>
    public abstract byte[] Writing();


    /// <summary>
    /// 写入 int
    /// </summary>
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

    /// <summary>
    /// 写入 short
    /// </summary>
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

    /// <summary>
    /// 写入 long
    /// </summary>
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

    /// <summary>
    /// 写入 float
    /// </summary>
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

    /// <summary>
    /// 写入 bool
    /// </summary>
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

    /// <summary>
    /// 写入 byte
    /// </summary>
    protected void WriteByte(
        byte[] bytes,
        byte value,
        ref int index)
    {
        bytes[index] = value;

        index += sizeof(byte);
    }

    /// <summary>
    /// 写入 UTF-8 字符串
    /// 数据结构：
    /// 4字节字符串长度 + N字节字符串数据
    /// </summary>
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

    /// <summary>
    /// 写入另一个网络数据对象
    /// </summary>
    protected void WriteData(
        byte[] bytes,
        NetworkDataBase data,
        ref int index)
    {
        byte[] dataBytes = data.Writing();

        dataBytes.CopyTo(bytes, index);

        index += dataBytes.Length;
    }
}