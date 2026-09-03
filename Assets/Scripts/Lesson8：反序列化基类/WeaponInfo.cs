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