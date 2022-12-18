using UnityEngine;
//�����
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
    public Socket serverSocket; //��������socket
    IPAddress ip; //����ip
    IPEndPoint ipEnd;
    string recvStr; //���յ��ַ���
    byte[] recvData = new byte[1024]; //���յ����ݣ�����Ϊ�ֽ�
    byte[] sendData = new byte[1024]; //���͵����ݣ�����Ϊ�ֽ�
    int recvLen; //���յ����ݳ���
    Thread connectThread; //�����߳�

    public bool isConnected = false;

    public ConcurrentQueue<string> recDatas { get; set; }

    //��ʼ��
    public void InitSocket(string Uri, int port)
    {
        //�����������IP�Ͷ˿ڣ��˿����������Ӧ
        ip = IPAddress.Parse(Uri); //�����Ǿ�����������ip���˴��Ǳ���
        ipEnd = new IPEndPoint(ip, port);

        recDatas = new ConcurrentQueue<string>();
        //����һ���߳����ӣ�����ģ��������߳̿���
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketReceive()
    {
        SocketConnet();
        Debug.Log("start tcp connection");
        //���Ͻ��շ���������������
        while (true)
        {
            recvData = new byte[1024];
            recvLen = serverSocket.Receive(recvData);
            recvStr = Encoding.UTF8.GetString(recvData, 0, recvLen);
            if (recvStr.Length > 0)
            {
                recDatas.Enqueue(recvStr);
            }
        }
    }

    void SocketConnet()
    {
        if (serverSocket != null)
            serverSocket.Close();
        //�����׽�������,���������߳��ж���
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //����
        Debug.Log(isConnected);
        while (!isConnected)
        {
            serverSocket.Connect(ipEnd);
            isConnected = serverSocket.Connected;
            Task.Delay(50).Wait();
        }

        //������������յ����ַ���
        recvLen = serverSocket.Receive(recvData);
        recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
        if (recvStr.Length > 0)
        {
            Debug.Log(recvStr);
        }
    }

    public void SocketSend(string sendStr)
    {
        //��շ��ͻ���
        sendData = new byte[1024];
        //��������ת��
        sendData = Encoding.UTF8.GetBytes(sendStr + Environment.NewLine);
        //����
        Debug.Log("Client send:" + sendStr);    
        serverSocket.Send(sendData, sendData.Length, SocketFlags.None);
    }

    void SocketQuit()
    {
        //�ر��߳�
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
            isConnected = serverSocket.Connected;
        }
        //���رշ�����
        if (serverSocket != null)
            serverSocket.Close();
        Debug.Log("Disconnect");
    }
}