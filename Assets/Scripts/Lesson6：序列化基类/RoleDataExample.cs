using UnityEngine;

/// <summary>
/// 角色网络数据使用示例
/// </summary>
public class RoleDataExample : MonoBehaviour
{
    private void Start()
    {
        WeaponData weapon = new WeaponData
        {
            weaponId = 2001,
            attack = 135
        };

        RoleData role = new RoleData
        {
            roleId = 10001,
            level = 25,
            nickname = "黑马王子",
            moveSpeed = 5.5f,
            online = true,
            weapon = weapon
        };

        //将完整角色信息转换为网络字节数据
        byte[] bytes = role.Writing();

        Debug.Log(
            $"角色数据序列化完成，共 {bytes.Length} Byte"
        );

        //以后可以直接交给 Socket
        //socket.Send(bytes);
    }
}