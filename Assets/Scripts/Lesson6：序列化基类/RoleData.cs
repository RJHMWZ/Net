using System.Text;

/// <summary>
/// 角色网络数据
/// </summary>
public class RoleData : NetworkDataBase
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public int roleId;

    /// <summary>
    /// 等级
    /// </summary>
    public short level;

    /// <summary>
    /// 昵称
    /// </summary>
    public string nickname;

    /// <summary>
    /// 移动速度
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// 是否在线
    /// </summary>
    public bool online;

    /// <summary>
    /// 当前武器
    /// </summary>
    public WeaponData weapon;


    public override int GetBytesNum()
    {
        //字符串真正占用的字节数
        int nameBytesNum =
            Encoding.UTF8.GetByteCount(nickname);

        return
            sizeof(int) +             //roleId
            sizeof(short) +           //level
            sizeof(int) +             //nickname字节长度
            nameBytesNum +            //nickname
            sizeof(float) +           //moveSpeed
            sizeof(bool) +            //online
            weapon.GetBytesNum();      //weapon
    }

    public override byte[] Writing()
    {
        byte[] bytes =
            new byte[GetBytesNum()];

        int index = 0;

        //角色ID
        WriteInt(
            bytes,
            roleId,
            ref index
        );

        //等级
        WriteShort(
            bytes,
            level,
            ref index
        );

        //昵称
        WriteString(
            bytes,
            nickname,
            ref index
        );

        //移动速度
        WriteFloat(
            bytes,
            moveSpeed,
            ref index
        );

        //在线状态
        WriteBool(
            bytes,
            online,
            ref index
        );

        //武器数据
        WriteData(
            bytes,
            weapon,
            ref index
        );

        return bytes;
    }
}