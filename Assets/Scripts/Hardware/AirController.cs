using System.Net.Sockets;
using UnityEngine;
using System.Collections;

public class AirController : MonoBehaviour
{
    public enum Flow
    {
        Air0,
        Air1,
        Air2
    }
    
    UdpClient udpClient;
    Coroutine air1Coroutine;

    string host;
    int port;

    public void Init(string host, int port)
    {
        this.host = host;
        this.port = port;
        udpClient = new UdpClient();
    }

    public void StartBlow()
    {
        if (air1Coroutine != null)
        {
            StopCoroutine(air1Coroutine);
        }
        
        SendUDP(Flow.Air2);
        air1Coroutine = StartCoroutine(SendAir1AfterDelay());
    }

    private IEnumerator SendAir1AfterDelay()
    {
        yield return new WaitForSeconds(Settings.System.AirBlowToWeakenWait);
        SendUDP(Flow.Air1);
        air1Coroutine = null;
    }

    public void StopBlow()
    {
        if (air1Coroutine != null)
        {
            StopCoroutine(air1Coroutine);
            air1Coroutine = null;
        }
        SendUDP(Flow.Air0);
    }

    void SendUDP(Flow flow)
    {
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(flow.ToString());
        udpClient.Send(sendBytes, sendBytes.Length, host, port);
    }

    void OnDestroy()
    {
        StopBlow();
    }
}
