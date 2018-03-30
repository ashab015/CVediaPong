using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A Relay Server that handles the network communication
/// The server is non-blocking and and runs on a background thread.
/// </summary>
public class RelayServer : MonoBehaviour {

    /// <summary> The server socket. </summary>
    public Socket listener;
    /// <summary> True if client connected. </summary>
    public bool ClientConnected = false;
    /// <summary> The client. </summary>
    public Socket client;
    /// <summary> Incoming data from client. </summary>
    public string data = null;
    /// <summary> The bytes. </summary>
    byte[] bytes = new Byte[1024];
    /// <summary> The ping pong manager which we get the positions of the paddles and ball. </summary>
    public PingPongManager PPM;
    /// <summary> Label of the host button. </summary>
    public UILabel HostLabel;
    /// <summary> The game user interface panel. </summary>
    public UIPanel GameUIPanel;
    /// <summary> The menu user interface. </summary>
    public UIPanel MenuUI;
    /// <summary> Main server background thread. </summary>
    public Thread ServerThread;
    /// <summary> The actions from the background thread. </summary>
    public List<Action> Actions = new List<Action>();
    /// <summary> True if game started. </summary>
    public bool GameStarted = false;

    /// <summary> Starts the main server. </summary>
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
    /// <summary> Updates this object. </summary>
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
    /// <summary> Server loop. </summary>
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
                    PPM.Paddle2.transform.position = Vector3.Lerp(PPM.Paddle2.transform.position, new Vector3(float.Parse(posdata[0]), float.Parse(posdata[1]), float.Parse(posdata[2])), 6 * Time.fixedDeltaTime);
                });
            }
        }


    }
    /// <summary> Executes the application quit action. </summary>
    public void OnApplicationQuit()
    {
        // Make sure the thread is stopped before unity quits or in the editor unity can keep the
        // thread running and cause problems.
        ServerThread.Abort();
        ServerThread = null;
    }

}
