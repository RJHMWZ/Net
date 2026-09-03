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
        byte[] levBytes = BitConverter.GetBytes(lev);
        levBytes.CopyTo(data, index);
        index += sizeof(int);
        byte[] nameLengthBytes =
            BitConverter.GetBytes(nameBytes.Length);

        nameLengthBytes.CopyTo(data, index);

        index += sizeof(int);

        nameBytes.CopyTo(data, index);

        index += nameBytes.Length;

        byte[] atkBytes = BitConverter.GetBytes(atk);

        atkBytes.CopyTo(data, index);

        index += sizeof(short);

        byte[] sexBytes = BitConverter.GetBytes(sex);

        sexBytes.CopyTo(data, index);

        index += sizeof(bool);

        return data;
    }
}

public class Lesson5 : MonoBehaviour
{
    private void Start()
    {
        PlayerInfo player = new PlayerInfo
        {
            lev = 10,
            name = "软件黑马王子",
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