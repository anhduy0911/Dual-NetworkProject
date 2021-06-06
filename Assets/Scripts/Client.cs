using System;
using System.Runtime.InteropServices;
using Google.Protobuf;
using GameMessage;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client clientInstance = null;
    public static int player_id = 1;
    public static int winState = 0;

    public int bulletLeft;
    private void Awake()
    {
        Debug.Log("This is awake!");

        if (clientInstance == null)
        {
            Debug.Log("Now we are here!");
            clientInstance = this;

            if (clientInstance.clientPtr == IntPtr.Zero)
            {
                clientInstance.clientPtr = createClientInstance(9999);
            }
            //persist the instance of client
            message = new Message();
            bulletLeft = 5;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.Log("The client ptr: " + clientInstance.clientPtr.ToString());
            Destroy(gameObject);
        }
    }

    
    void OnDisable()
    {
        Debug.Log("Dispose this client instance!");
        int status = closeConnection(clientPtr);
        Debug.Log("Status of closing connection: " + status.ToString());
    }

    private IntPtr clientPtr;

    private Message message;


    [DllImport("client", EntryPoint = "createClientInstance")]
    //[DllImport("/Users/anhduy0911/Projects/School/NetworkProgramming/midProject/bin/client.dylib", EntryPoint = "createClientInstance")]
    private static extern IntPtr createClientInstance(int port);

    [DllImport("client", EntryPoint = "startConnection")]
    //[DllImport("/Users/anhduy0911/Projects/School/NetworkProgramming/midProject/bin/client.dylib", EntryPoint = "startConnection")]
    private static extern int startConnection(IntPtr client);

    [DllImport("client", EntryPoint = "closeConnection")]
    //[DllImport("/Users/anhduy0911/Projects/School/NetworkProgramming/midProject/bin/client.dylib", EntryPoint = "closeConnection")]
    private static extern int closeConnection(IntPtr client);

    [DllImport("client", EntryPoint = "sendDirection")]
    //[DllImport("/Users/anhduy0911/Projects/School/NetworkProgramming/midProject/bin/client.dylib", EntryPoint = "sendDirection")]
    private static extern int sendDirection(IntPtr client, byte[] buffer, int size);

    [DllImport("client", EntryPoint = "sendShoot")]
    //[DllImport("/Users/anhduy0911/Projects/School/NetworkProgramming/midProject/bin/client.dylib", EntryPoint = "sendShoot")]
    private static extern int sendShoot(IntPtr client, byte[] buffer, int size, int flag);

    [DllImport("client", EntryPoint = "sendAcknowledge")]
    //[DllImport("/Users/anhduy0911/Projects/School/NetworkProgramming/midProject/bin/client.dylib", EntryPoint = "sendAcknowledge")]
    private static extern int sendAcknowledge(IntPtr client, byte[] buffer, int size);

    [DllImport("client", EntryPoint = "receiveData")]
    //[DllImport("/Users/anhduy0911/Projects/School/NetworkProgramming/midProject/bin/client.dylib", EntryPoint = "receiveData")]
    private static extern int receiveData(IntPtr client, byte[] buffer, int size);

    public static int startConnection()
    {
        Debug.Log("Start connecting...");
        return startConnection(clientInstance.clientPtr);
    }

    public static int closeConnection()
    {
        Debug.Log("Stop connecting...");
        int s = closeConnection(clientInstance.clientPtr);
        Debug.Log("Status of closing connection: " + s.ToString());
        return s;
    }

    public static int sendDirection(MovingDirection md)
    {
        int size = md.CalculateSize();
        byte[] buffer = md.ToByteArray();
        Debug.Log("Message length: " + size.ToString());
        return sendDirection(clientInstance.clientPtr, buffer, size);
    }

    public static int sendShoot(IsShot isShot, int flag)
    {
        int size = isShot.CalculateSize();
        byte[] buffer = isShot.ToByteArray();
        Debug.Log("Send shoot - Message length: " + size.ToString());

        return sendShoot(clientInstance.clientPtr, buffer, size, flag);
    }

    public static int sendAcknowledgeWrapper()
    {
        IsShot isShot = new IsShot();
        int size = isShot.CalculateSize();
        byte[] buffer = isShot.ToByteArray();
        Debug.Log("Send ack - Message length: " + size.ToString());

        return sendAcknowledge(clientInstance.clientPtr, buffer, size);
    }

    public int getMessageFlag() { return clientInstance.message.flag; }

    public int getMessageLength() { return clientInstance.message.length; }

    public byte[] getMessageData() { return clientInstance.message.data; }


    public int parseReceivedMessage()
    {
        byte[] bufferRecv = new byte[4095 + 5];
        int flag = receiveData(clientPtr, bufferRecv, 4096 + 5);

        if (flag > 0)
        {
            switch (bufferRecv[0])
            {
                case 0:
                    Debug.Log("Receive playerStat");
                    message.flag = 0; //receive playerStat
                    message.length = BitConverter.ToInt32(bufferRecv, 1);
                    Debug.Log("Length: " + message.length);
                    if (message.length > 0)
                        Array.Copy(bufferRecv, 1 + sizeof(int), message.data, 0, message.length);
                    break;
                case 1:
                    Debug.Log("Receive status");
                    message.flag = 1; //receive status
                    message.length = BitConverter.ToInt32(bufferRecv, 1);
                    Debug.Log("Length: " + message.length);

                    if (message.length > 0)
                        Array.Copy(bufferRecv, 1 + sizeof(int), message.data, 0, message.length);
                    else
                    {
                        player_id = 0; // Have to wait -> player 1
                        Debug.Log("Receive status 0 - waiting");
                    }
                    break;
            }
            return message.flag;
        }

        return -1;
    }

    public Status parseStatus()
    {
        if (message.length > 0)
        {
            Status status = Status.Parser.ParseFrom(message.data, 0, message.length);
            return status;
        }

        return new Status();
    }

    public GameStateMessage parseGameState()
    {
        if (message.length > 0)
        {
            GameStateMessage gameState = GameStateMessage.Parser.ParseFrom(message.data, 0, message.length);
            return gameState;
        }
        return new GameStateMessage();
    }

}
