using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;
using System.Net.Sockets;

public class Server : MonoBehaviour
{
    public int port = 6321;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    private TcpListener server;
    private bool serverStarted = true;
    private IPAddress localhost = IPAddress.Parse("127.0.0.1");

    string selectA = "Select|";
    string selectB = "Select|";
    string TurnA = "Turn|";
    string TurnB = "Turn|";

    string ShotA = "Aim|";
    string ShotB = "Aim|";

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, 6321);
            server.Start();

            StartListening();

        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }

    private void Update()
    {
        if (!serverStarted)
        {
            return;
        }

        foreach (ServerClient c in clients)
        {
            // Is the client still connected?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                    {
                        OnIncomingData(c, data);
                    }
                }
            }
        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            // Tell our player somebody has disconnected

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;

        string allUsers = "";
        foreach (ServerClient i in clients)
        {
            allUsers += i.clientName + '|';
            Debug.Log(allUsers);
        }

        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);

        StartListening();

        Broadcast("SWHO|" + allUsers, clients[clients.Count - 1]);

    }

    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    // Server Send
    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient sc in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.Log("Write error : " + e.Message);
            }
        }
    }

    private void Broadcast(string data, ServerClient c)
    {
        List<ServerClient> sc = new List<ServerClient> { c };
        Broadcast(data, sc);
    }

    // Server Read
    private void OnIncomingData(ServerClient c, string data)
    {
        Debug.Log("Server : " + data);
        string[] aData = data.Split('|');


        switch (aData[0])
        {
            case "CWHO":
                c.clientName = aData[1];
                c.isHost = (aData[2] == "0") ? false : true;
                Broadcast("SCNN|" + c.clientName, clients);
                break;

            case "CMOV":

                Broadcast("SMOV|" + aData[1] + "|" + aData[2] + "|" + aData[3] + "|" + aData[4], clients);
                break;

            // case "Locate":
            //     Broadcast("Locate|" + );
            //     break;

            case "Start":
                Broadcast("Start|", clients);
                break;

            case "Select":
                if (aData[7] == "a")
                {

                    selectA = selectA + aData[1] + aData[2] + aData[3] + aData[4] + aData[5] + aData[6];
                }
                else
                {
                    selectB = selectB + aData[1] + aData[2] + aData[3] + aData[4] + aData[5] + aData[6];
                }
                

                if (!selectA.Equals("Select|") && !selectB.Equals("Select|"))
                {
                    Broadcast(selectA, clients[1]);
                    Broadcast(selectB, clients[0]);
                }
                break;

            case "Turn":
                
                if (aData[2] == "a")
                {
                    TurnA += aData[1];
                }
                else
                {
                    TurnB += aData[1];
                }
                if (!TurnA.Equals("Turn|") && !TurnB.Equals("Turn|"))
                {
                    Broadcast(TurnA, clients[1]);
                    Broadcast(TurnB, clients[0]);
                    TurnA = "Turn|";
                    TurnB = "Turn|";
                }
                
                break;

            case "Ball":
                if (aData[2] == "1")
                {
                    Broadcast("Ball|" + aData[1], clients[0]);
                }
                else
                {
                    Broadcast("Ball|" + aData[1], clients[1]);
                }
                break;

            case "Shoot":
                Broadcast("Shoot|", clients[1]);
                break;

            case "Aim":
                if (aData[2] == "0")
                {
                    ShotA += aData[1];
                }
                else
                {
                    ShotB += aData[1];
                }
                if (!ShotA.Equals("Aim|") && !ShotB.Equals("Aim|"))
                {
                    Broadcast(ShotA + "|" + ShotB, clients);
                    ShotA = "Aim|";
                    ShotB = "Aim|";
                }
                break;
        }
    }
}

public class ServerClient
{
    public string clientName;
    public TcpClient tcp;
    public bool isHost;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}
