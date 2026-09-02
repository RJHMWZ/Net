using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Lesson1 : MonoBehaviour
{
    void Start()
    {
        IPAddress ipAddress =IPAddress.Loopback;
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8080);
    }
}
