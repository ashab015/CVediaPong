using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

// A Relay Server that handles the network communication
// The server is non-blocking and and runs on a background thread.
public class RelayServer : MonoBehaviour {

    // The server socket
    public Socket listener;
    public bool ClientConnected = false;
    public Socket client;
    // Incoming data from client
    public string data = null;
    byte[] bytes = new Byte[1024];
    // The ping pong manager which we get the positions of the paddles and ball
    public PingPongManager PPM;
    // Label of the host button
    public UILabel HostLabel;
    public UIPanel GameUIPanel;
    public UIPanel MenuUI;
    // Main server background thread
    public Thread ServerThread;
    // The actions from the background thread
    public List<Action> Actions = new List<Action>();
    public bool GameStarted = false;

    // Starts the main server
    public void StartServer()
    {
        // Data buffer for incoming data.  
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);

        // Create a TCP/IP socket.  
        listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.  
        listener.Bind(localEndPoint);
        listener.Listen(100);

        // The background server thread that will process all of the client messages
        ServerThread = new Thread(() => ServerLoop());
        ServerThread.IsBackground = true;
        ServerThread.Start();

        HostLabel.text = "Waiting for client...";

    }
    public void Update()
    {
        // Since unity isnt threadsafe this invokes the functions
        // called from the background thread and runs them on the main thread
        if (Actions.Count > 1000)
            Actions.Clear();
        for (int x = 0; Actions.Count > x; x++)
        {
            Actions[x].Invoke();
        }
        // If game started send the position of the ping pong ball and our server paddle
        if (GameStarted == true)
        {
            Vector3 bp = PPM.Ball.transform.position;
            Vector3 pd = PPM.Paddle1.transform.position;
            client.Send(Encoding.ASCII.GetBytes(bp.x.ToString() + "|" + bp.y.ToString() + "|" + bp.z.ToString() + "|;BALL"));
            client.Send(Encoding.ASCII.GetBytes(pd.x.ToString() + "|" + pd.y.ToString() + "|" + pd.z.ToString() + "|;PADDLE"));
        }
    }
    public void ServerLoop()
    {
        Debug.Log("Waiting for a connection...");
        // Thread is suspended while waiting for an incoming connection.  
        Socket handler = listener.Accept();
        client = handler;

        Actions.Add(delegate ()
        {
            HostLabel.text = "Client joined!";
        });

        Debug.Log("connected!!!");

        while (true)
        {

            bytes = new byte[1024];
            int bytesRec = handler.Receive(bytes);
            string message = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            string[] datarecieved = message.Split(';');

            Debug.Log(message);

            // START = message to start the ping pong game
            // PADDLE = the position of the oppisite paddle

            if (datarecieved.Length > 1 && datarecieved[1] == "START")
            {
                Actions.Add(delegate ()
                {
                    MenuUI.gameObject.SetActive(false);
                    GameUIPanel.gameObject.SetActive(true);
                    client.Send(Encoding.ASCII.GetBytes(";START"));
                    GameStarted = true;
                });
            }
            if (datarecieved.Length > 1 && datarecieved[1] == "PADDLE")
            {
                Actions.Add(delegate ()
                {
                    string[] posdata = datarecieved[0].Split('|');
                    PPM.Paddle2.transform.position = Vector3.Lerp(PPM.Paddle2.transform.position, new Vector3(float.Parse(posdata[0]), float.Parse(posdata[1]), float.Parse(posdata[2])), 6 * Time.deltaTime);
                });
            }
        }


    }
    public void OnApplicationQuit()
    {
        // Make sure the thread is stopped before unity quits or in the editor unity can keep the
        // thread running and cause problems.
        ServerThread.Abort();
        ServerThread = null;
    }

}
