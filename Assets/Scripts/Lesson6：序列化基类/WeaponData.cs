/// <summary>
/// 武器数据
/// </summary>
public class WeaponData : NetworkDataBase
{
    /// <summary>
    /// 武器ID
    /// </summary>
    public int weaponId;

    /// <summary>
    /// 攻击力
    /// </summary>
    public short attack;


    public override int GetBytesNum()
    {
        return
            sizeof(int) +
            sizeof(short);
    }

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
}