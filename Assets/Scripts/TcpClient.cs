using UnityEngine;
using System.Collections;
//引入库
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

public class TcpClient : MonoBehaviour
{
    string editString = "hello wolrd"; //编辑框文字

    public Socket serverSocket; //服务器端socket
    IPAddress ip; //主机ip
    IPEndPoint ipEnd;
    string recvStr; //接收的字符串
    string sendStr; //发送的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节
    int recvLen; //接收的数据长度
    Thread connectThread; //连接线程

    public bool isConnected = false;

    //初始化
    void InitSocket()
    {
        //定义服务器的IP和端口，端口与服务器对应
        ip = IPAddress.Parse("127.0.0.1"); //可以是局域网或互联网ip，此处是本机
        ipEnd = new IPEndPoint(ip, 6666);


        //开启一个线程连接，必须的，否则主线程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketConnet()
    {
        if (serverSocket != null)
            serverSocket.Close();
        //定义套接字类型,必须在子线程中定义
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //连接
        serverSocket.Connect(ipEnd);
        isConnected = serverSocket.Connected;

        //输出初次连接收到的字符串
        recvLen = serverSocket.Receive(recvData);
        recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);

        if (recvStr.Length > 0)
        {
            print(recvStr);
        }
    }

    public void SocketSend(string sendStr)
    {
        //清空发送缓存
        sendData = new byte[1024];
        //数据类型转换
        sendData = Encoding.UTF8.GetBytes(sendStr + Environment.NewLine);
        //发送
        serverSocket.Send(sendData, sendData.Length, SocketFlags.None);
        Debug.Log("Client send:" + sendStr);
    }

    void SocketReceive()
    {
        SocketConnet();
        Debug.Log("start tcp connection");
        //不断接收服务器发来的数据
        while (true)
        {
            recvData = new byte[1024];
            recvLen = serverSocket.Receive(recvData);
            recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);
            Debug.Log("Server send:" + recvStr);
        }
    }

    void SocketQuit()
    {
        //关闭线程
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
            isConnected = serverSocket.Connected;
        }
        //最后关闭服务器
        if (serverSocket != null)
            serverSocket.Close();
        Debug.Log("Disconnect");
    }

    // Use this for initialization
    void Start()
    {
        InitSocket();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //程序退出则关闭连接
    void OnApplicationQuit()
    {
        SocketQuit();
    }
}