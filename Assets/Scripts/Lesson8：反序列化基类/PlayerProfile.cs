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