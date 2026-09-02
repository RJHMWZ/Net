# 一、IPAddress 类
## 1.IPAddress
`IPAddress` 用于在 C# 中表示一个 `IPv4` 或 `IPv6` 地址，位于：
```
using System.Net;
```
类名：
```
IPAddress
```
源码中主要通过 `IPAddress` 保存 IP 地址信息。
常用创建方式：
```
IPAddress ip = IPAddress.Parse("192.168.1.100");
```
也可以通过字节数组创建：
```
byte[] bytes = { 192, 168, 1, 100 };
IPAddress ip = new IPAddress(bytes);
```
`IPAddress` 当前仍提供 `byte[]`、`ReadOnlySpan<byte>` 和 `long` 等构造方式，但实际开发中字符串解析更加直观。
## 2.Parse 与 TryParse
确定字符串一定合法时：
```
IPAddress ip = IPAddress.Parse("192.168.1.100");
```
如果 IP 地址来自玩家输入、配置文件或服务器配置，更推荐：
```
if (IPAddress.TryParse("192.168.1.100", out IPAddress ip))
{
    Debug.Log(ip);
}
```
区别：
```
Parse     → 格式错误时抛出异常
TryParse  → 返回 true / false
```
## 3.常用特殊地址
### IPv4 回环地址
```
IPAddress.Loopback
```
等价于：
```
127.0.0.1
```
表示本机，常用于本地客户端连接本地服务器。
```
IPAddress ip = IPAddress.Loopback;
```
### IPv4 任意地址
```
IPAddress.Any
```
等价于：
```
0.0.0.0
```
服务器监听时表示：
```
监听本机所有 IPv4 网络接口
```
### IPv6 回环地址
```
IPAddress.IPv6Loopback
```
等价于：
```
::1
```
### IPv6 任意地址
```
IPAddress.IPv6Any
```
等价于：
```
::
```

# 二、IPEndPoint 类
## 1.IPEndPoint
`IPEndPoint` 用于表示一个 **IP 地址 + 端口号**组成的网络端点。源码中使用它组合目标 IP 和端口。
例如：
```
IPAddress ip = IPAddress.Parse("192.168.1.100");

IPEndPoint endPoint = new IPEndPoint(ip, 8080);
```
表示：
```
IP：192.168.1.100
Port：8080

最终端点：
192.168.1.100:8080
```
`IPEndPoint` 官方提供：
```
IPEndPoint(IPAddress address, int port)
IPEndPoint(long address, int port)
```
实际开发优先使用 `IPAddress + Port` 的方式，可读性更好。
## 2.常用属性
```
endPoint.Address
```
获取或设置 IP 地址。
```
endPoint.Port
```
获取或设置端口号。
```
endPoint.AddressFamily
```
获取地址类型，例如：
```
IPv4
IPv6
```
`Address` 属性本身就是一个 `IPAddress` 对象。

# 三、IPAddress 与 IPEndPoint 的关系
```
IPAddress
↓
只表示 IP 地址
192.168.1.100

IPEndPoint
↓
表示 IP + Port
192.168.1.100:8080
```
网络通信通常不会只指定 IP，因为还需要确定目标设备上的具体网络服务。
因此后续使用 `Socket` 连接服务器时，常见写法：
```
IPAddress ip = IPAddress.Parse("192.168.1.100");

IPEndPoint serverPoint =new IPEndPoint(ip, 8080);
```
可以简单记：
```
IPAddress  → 表示主机地址
Port       → 表示应用程序端口
IPEndPoint → IPAddress + Port
```

# 四、Unity 中常见写法
本地服务器测试：
```
IPAddress ip = IPAddress.Loopback;
IPEndPoint endPoint = new IPEndPoint(ip, 8080);
```
局域网服务器：
```
IPAddress ip = IPAddress.Parse("192.168.1.100");
IPEndPoint endPoint = new IPEndPoint(ip, 8080);
```
服务器监听所有 IPv4 网卡：
```
IPEndPoint endPoint =new IPEndPoint(IPAddress.Any, 8080);
```
后续通常会把 `IPEndPoint` 交给 `Socket`：
```
IPAddress
    ↓
IPEndPoint
    ↓
Socket
    ↓
Connect / Bind / Send / Receive
```