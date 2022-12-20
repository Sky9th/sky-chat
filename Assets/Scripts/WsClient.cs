using UnityEngine;
using System.Net.WebSockets;
using System;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

public class WsClient
{
    // WebSocket
    private GameObject gc;
    private ClientWebSocket ws;
    private UTF8Encoding encoder; // For websocket text message encoding.
    private const UInt64 MAXREADSIZE = 1 * 1024 * 1024;
    // Server address
    private Uri serverUri;
    private Thread connectThread;

    // Queues
    public ConcurrentQueue<String> receiveQueue { get; set; }
    public BlockingCollection<ArraySegment<byte>> sendQueue { get; set; }
    // Threads 
    private Thread receiveThread { get; set; }
    private Thread sendThread { get; set; }

    public bool isConnected = false;

    void receive()
    {
        // Check if server send new messages
        string msg;
        while (receiveQueue.TryPeek(out msg))
        {
            // Parse newly received messages
            receiveQueue.TryDequeue(out msg);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:WsClient"/> class.
    /// </summary>
    /// <param name="serverURL">Server URL.</param>
    public void init(string serverURL)
    {
        encoder = new UTF8Encoding();
        ws = new ClientWebSocket();
        serverUri = new Uri(serverURL);
        connectThread = new Thread(RunConnect);
        connectThread.Start();
        receiveQueue = new ConcurrentQueue<string>();
        receiveThread = new Thread(RunReceive);
        receiveThread.Start();
        sendQueue = new BlockingCollection<ArraySegment<byte>>();
        sendThread = new Thread(RunSend);
        sendThread.Start();
    }

    private async void RunConnect()
    {
        await Connect();
    }

    /// <summary>
    /// Method which connects client to the server.
    /// </summary>
    /// <returns>The connect.</returns>
    public async Task Connect()
    {
        await ws.ConnectAsync(serverUri, CancellationToken.None);
        while (IsConnecting())
        {
            Task.Delay(50).Wait();
        }
        isConnected = IsConnectionOpen();
    }

    #region [Status]
    /// <summary>
    /// Return if is connecting to the server.
    /// </summary>
    /// <returns><c>true</c>, if is connecting to the server, <c>false</c> otherwise.</returns>
    public bool IsConnecting()
    {
        return ws.State == WebSocketState.Connecting;
    }
    /// <summary>
    /// Return if connection with server is open.
    /// </summary>
    /// <returns><c>true</c>, if connection with server is open, <c>false</c> otherwise.</returns>
    public bool IsConnectionOpen()
    {
        return ws.State == WebSocketState.Open;
    }
    #endregion
    #region [Send]
    /// <summary>
    /// Method used to send a message to the server.
    /// </summary>
    /// <param name="message">Message.</param>
    public void Send(string message)
    {
        byte[] buffer = encoder.GetBytes(message);
        var sendBuf = new ArraySegment<byte>(buffer);
        sendQueue.Add(sendBuf);
    }
    /// <summary>
    /// Method for other thread, which sends messages to the server.
    /// </summary>
    private async void RunSend()
    {
        ArraySegment<byte> msg;
        while (true)
        {
            while (!sendQueue.IsCompleted)
            {
                msg = sendQueue.Take();
                //Debug.Log("Dequeued this message to send: " + msg);
                await ws.SendAsync(msg, WebSocketMessageType.Text, true /* is last part of message */, CancellationToken.None);
            }
        }
    }
    #endregion
    #region [Receive]
    /// <summary>
    /// Reads the message from the server.
    /// </summary>
    /// <returns>The message.</returns>
    /// <param name="maxSize">Max size.</param>
    private async Task<string> Receive(UInt64 maxSize = MAXREADSIZE)
    {
        // A read buffer, and a memory stream to stuff unknown number of chunks into:
        byte[] buf = new byte[4 * 1024];
        var ms = new MemoryStream();
        ArraySegment<byte> arrayBuf = new ArraySegment<byte>(buf);
        WebSocketReceiveResult chunkResult = null;
        if (IsConnectionOpen())
        {
            do
            {
                chunkResult = await ws.ReceiveAsync(arrayBuf, CancellationToken.None);
                ms.Write(arrayBuf.Array, arrayBuf.Offset, chunkResult.Count);
                //Debug.Log("Size of Chunk message: " + chunkResult.Count);
                if ((UInt64)(chunkResult.Count) > MAXREADSIZE)
                {
                    Console.Error.WriteLine("Warning: Message is bigger than expected!");
                }
            } while (!chunkResult.EndOfMessage);
            ms.Seek(0, SeekOrigin.Begin);
            // Looking for UTF-8 JSON type messages.
            if (chunkResult.MessageType == WebSocketMessageType.Text)
            {
                return StreamToString(ms, Encoding.UTF8);
            }
        }
        return "";
    }
    /// <summary>
    /// Method for other thread, which receives messages from the server.
    /// </summary>
    private async void RunReceive()
    {
        string result;
        while (true)
        {
            //Debug.Log("Awaiting Receive...");
            result = await Receive();
            if (result != null && result.Length > 0)
            {
                receiveQueue.Enqueue(result);
            }
            else
            {
                Task.Delay(50).Wait();
            }
        }
    }
    #endregion

    public string StreamToString(MemoryStream ms, Encoding encoding)
    {
        string readString = "";
        if (encoding == Encoding.UTF8)
        {
            using (var reader = new StreamReader(ms, encoding))
            {
                readString = reader.ReadToEnd();
            }
        }
        return readString;
    }
}