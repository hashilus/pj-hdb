using System.Collections;
using System.Net.Sockets;
using UnityEngine;

public class AirController : MonoBehaviour
{
    [SerializeField]
    bool isLController;

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

    string[] airCommands = new string[] { "air 0", "air 1", "air 2" };

    void Start()
    {
        if (!Settings.System.IsUseTracker) return;

        var haccAddress = isLController ? Settings.System.HACCAddressL : Settings.System.HACCAddressR;
        var haccPort = isLController ? Settings.System.HACCPortL : Settings.System.HACCPortR;

        Init(haccAddress, haccPort);
    }

    void Init(string host, int port)
    {
        this.host = host;
        this.port = port;
        udpClient = new UdpClient();
        StopBlow();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SendUDP(Flow.Air0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SendUDP(Flow.Air1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SendUDP(Flow.Air2);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SendMistUDP();
        }
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
        if(udpClient == null)
        {
            return;
        }

        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(airCommands[(int)flow]);
        udpClient.Send(sendBytes, sendBytes.Length, host, port);
    }

    void SendMistUDP()
    {
        if(udpClient == null)
        {
            return;
        }
        
        byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes("mist 1000");
        udpClient.Send(sendBytes, sendBytes.Length, host, port);
    }

    void OnDestroy()
    {
        if (!Settings.System.IsUseTracker) return;

        StopBlow();
    }
}
