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