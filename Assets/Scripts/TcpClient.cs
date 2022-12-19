using UnityEngine;
//引入库
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class TcpClient
{
    public Socket serverSocket; //服务器端socket
    IPAddress ip; //主机ip
    IPEndPoint ipEnd;
    string recvStr; //接收的字符串
    byte[] recvData = new byte[1024]; //接收的数据，必须为字节
    byte[] sendData = new byte[1024]; //发送的数据，必须为字节
    int recvLen; //接收的数据长度
    Thread connectThread; //连接线程

    public bool isConnected = false;

    public ConcurrentQueue<string> recDatas { get; set; }

    //初始化
    public void InitSocket(string Uri, int port)
    {
        //定义服务器的IP和端口，端口与服务器对应
        ip = IPAddress.Parse(Uri); //可以是局域网或互联网ip，此处是本机
        ipEnd = new IPEndPoint(ip, port);

        recDatas = new ConcurrentQueue<string>();
        //开启一个线程连接，必须的，否则主线程卡死
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketReceive()
    {
        SocketConnet();
        Debug.Log("start tcp connection");
        //不断接收服务器发来的数据
        while (true)
        {
            parseMsg();
        }
    }

    void SocketConnet()
    {
        if (serverSocket != null)
            serverSocket.Close();
        //定义套接字类型,必须在子线程中定义
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //连接
        Debug.Log(isConnected);
        while (!isConnected)
        {
            serverSocket.Connect(ipEnd);
            isConnected = serverSocket.Connected;
            Task.Delay(50).Wait();
        }

        parseMsg();
    }

    private void parseMsg ()
    {
        recvData = new byte[1024];
        recvLen = serverSocket.Receive(recvData);
        recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);
        if (recvStr.Length > 0)
        {
            String[] list = recvStr.Split(Environment.NewLine, StringSplitOptions.None);
            foreach (String vo in list)
            {
                recDatas.Enqueue(vo);
            }
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
}