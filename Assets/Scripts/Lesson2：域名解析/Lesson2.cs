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