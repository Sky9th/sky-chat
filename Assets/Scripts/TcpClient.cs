using UnityEngine;
using System.Collections;
//�����
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;

public class TcpClient : MonoBehaviour
{
    string editString = "hello wolrd"; //�༭������

    public Socket serverSocket; //��������socket
    IPAddress ip; //����ip
    IPEndPoint ipEnd;
    string recvStr; //���յ��ַ���
    string sendStr; //���͵��ַ���
    byte[] recvData = new byte[1024]; //���յ����ݣ�����Ϊ�ֽ�
    byte[] sendData = new byte[1024]; //���͵����ݣ�����Ϊ�ֽ�
    int recvLen; //���յ����ݳ���
    Thread connectThread; //�����߳�

    public bool isConnected = false;

    //��ʼ��
    void InitSocket()
    {
        //�����������IP�Ͷ˿ڣ��˿����������Ӧ
        ip = IPAddress.Parse("127.0.0.1"); //�����Ǿ�����������ip���˴��Ǳ���
        ipEnd = new IPEndPoint(ip, 6666);


        //����һ���߳����ӣ�����ģ��������߳̿���
        connectThread = new Thread(new ThreadStart(SocketReceive));
        connectThread.Start();
    }

    void SocketConnet()
    {
        if (serverSocket != null)
            serverSocket.Close();
        //�����׽�������,���������߳��ж���
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //����
        serverSocket.Connect(ipEnd);
        isConnected = serverSocket.Connected;

        //������������յ����ַ���
        recvLen = serverSocket.Receive(recvData);
        recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);

        if (recvStr.Length > 0)
        {
            print(recvStr);
        }
    }

    public void SocketSend(string sendStr)
    {
        //��շ��ͻ���
        sendData = new byte[1024];
        //��������ת��
        sendData = Encoding.UTF8.GetBytes(sendStr + Environment.NewLine);
        //����
        serverSocket.Send(sendData, sendData.Length, SocketFlags.None);
        Debug.Log("Client send:" + sendStr);
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
            Debug.Log("Server send:" + recvStr);
        }
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

    // Use this for initialization
    void Start()
    {
        InitSocket();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //�����˳���ر�����
    void OnApplicationQuit()
    {
        SocketQuit();
    }
}