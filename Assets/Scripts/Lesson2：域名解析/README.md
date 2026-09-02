# 一、域名解析
## 1.DNS
`DNS（Domain Name System）`用于建立**域名与 IP 地址之间的映射关系**。
例如：
```
www.baidu.com
↓ DNS 解析
对应的 IP 地址
```
实际网络通信最终仍然需要使用 IP 地址，域名主要是为了方便人类记忆。
```
域名 → DNS解析 → IP地址 → 建立网络连接
```

# 二、IPHostEntry 类
## 1.IPHostEntry
`IPHostEntry` 用于保存一次主机 / 域名解析得到的信息。命名空间：
```
using System.Net;
```
常用属性：

|属性|作用|
|---|---|
|`AddressList`|获取解析出的 IP 地址列表|
|`Aliases`|获取主机别名列表|
|`HostName`|获取主机名称|
例如：
```
IPHostEntry entry = await Dns.GetHostEntryAsync("www.baidu.com");
foreach (IPAddress ip in entry.AddressList)
{
    Debug.Log(ip);
}
```
一个域名可能对应**多个 IP 地址**，因此 `AddressList` 是数组。

# 三、Dns 类
## 1.Dns
`Dns` 是 `System.Net` 中用于进行 DNS 相关操作的静态类。
常用方法：
```
Dns.GetHostName();
Dns.GetHostEntry();
Dns.GetHostEntryAsync();
```
## 2.GetHostName
获取当前计算机的主机名：
```
string hostName = Dns.GetHostName();
Debug.Log(hostName);
```
## 3.GetHostEntry
同步解析域名：
```
IPHostEntry entry =Dns.GetHostEntry("www.baidu.com");
```
然后可以获取：
```
entry.AddressList;
entry.Aliases;
entry.HostName;
```
由于 DNS 查询可能涉及网络通信，同步调用可能阻塞当前线程，因此 Unity 中通常更适合使用异步版本。
## 4.GetHostEntryAsync
异步解析域名：
```
IPHostEntry entry =await Dns.GetHostEntryAsync("www.baidu.com");
```
相比：
```
Dns.GetHostEntry();
```
异步方式不会在等待 DNS 查询结果时一直阻塞当前执行流程，更适合网络请求。

# 四、测试代码
```
using System;
using System.Net;
using UnityEngine;

public class Lesson2 : MonoBehaviour
{
    private async void Start()
    {
        // 1.获取本机主机名
        string localHostName = Dns.GetHostName();
        Debug.Log($"本机主机名：{localHostName}");

        // 2.准备需要解析的域名
        string domain = "www.baidu.com";

        try
        {
            // 3.异步进行 DNS 解析
            IPHostEntry entry = await Dns.GetHostEntryAsync(domain);

            Debug.Log($"解析域名：{domain}");

            // 4.获取主机名称
            Debug.Log($"主机名称：{entry.HostName}");

            // 5.获取域名对应的所有 IP 地址
            Debug.Log("IP 地址列表：");

            foreach (IPAddress ip in entry.AddressList)
            {
                Debug.Log($"IP：{ip}");
            }

            // 6.获取主机别名
            if (entry.Aliases.Length > 0)
            {
                Debug.Log("主机别名：");

                foreach (string alias in entry.Aliases)
                {
                    Debug.Log(alias);
                }
            }
            else
            {
                Debug.Log("没有主机别名");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"DNS解析失败：{e.Message}");
        }
    }
}
```
整个流程可以记成：
```
域名
↓
Dns.GetHostEntryAsync()
↓
IPHostEntry
↓
AddressList
↓
IPAddress
↓
获得服务器 IP 地址
↓
后续通过 IP + Port 建立网络连接
```