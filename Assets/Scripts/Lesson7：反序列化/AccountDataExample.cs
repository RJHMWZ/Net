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