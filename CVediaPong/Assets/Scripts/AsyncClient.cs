using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// A Client that handles the network communication
/// The client is non-blocking and and runs on a background thread.
/// </summary>
public class AsyncClient : MonoBehaviour {


    /// <summary> True if connected. </summary>
    public bool Connected = false;
    /// <summary> The client. </summary>
    Socket client;
    /// <summary> The join label. </summary>
    public UILabel JoinLabel;
    /// <summary> The menu user interface panel. </summary>
    public GameObject MenuUIPanel;
    /// <summary> The pin ball user interface panel. </summary>
    public GameObject PinBallUIPanel;
    /// <summary> The ping pong manager which we set the positions of the paddles and ball. </summary>
    public PingPongManager PPM;
    /// <summary> The actions from the background thread. </summary>
    public List<Action> Actions = new List<Action>();
    /// <summary> Main client background thread. </summary>
    public Thread ClientThread;
    /// <summary> Data buffer for incoming data. </summary>
    byte[] bytes = new byte[1024];
    /// <summary> True to game running. </summary>
    public bool GameRunning = false;

    /// <summary> Updates this object. </summary>
    public void Update()
    {

        // Since unity isnt threadsafe this invokes the functions
        // called from the background thread and runs them on the main thread
        if (Actions.Count > 1000)
            Actions.Clear();
        for (int x=0; Actions.Count > x; x++)
        {
            Actions[x].Invoke();
        }
        // if the game is running send the position of our paddle
        if (GameRunning == true)
        {
            Vector3 pd = PPM.Paddle2.transform.position;
            client.Send(Encoding.ASCII.GetBytes(pd.x.ToString() + "|" + pd.y.ToString() + "|" + pd.z.ToString() + "|;PADDLE"));
        }

    }
    /// <summary> Starts a client. </summary>
    public void StartClient()
    {

        // After the user pressed the join button once
        // it joins the server then once pressed again
        // it tells the server to start the game
        if (client != null)
        {
            byte[] ba = Encoding.ASCII.GetBytes(";START");
            client.Send(ba, 0, ba.Length, SocketFlags.None);
            return;
        }
            

        // Data buffer for incoming data.  
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);

        // Create a TCP/IP socket.  
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming messages.  
        client.Connect(localEndPoint);

        // Start the thread that processes the server messages
        ClientThread = new Thread(() => ClientLoop());
        ClientThread.IsBackground = true;
        ClientThread.Start();

        JoinLabel.text = "Start the game";

    }
    /// <summary> Client loop. </summary>
    public void ClientLoop()
    {
        try
        {
            while (true)
            {
                // Receive the response from the remote device.  
                int bytesRec = client.Receive(bytes);
                string message = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                string[] datarecieved = message.Split(';');
                // START = message to start the ping pong game
                // BALL = the position of the ball from the server
                // PADDLE = the position of the oppisite paddle
                if (datarecieved.Length > 1 && datarecieved[1] == "START")
                {
                    Actions.Add(delegate ()
                    {
                        MenuUIPanel.gameObject.SetActive(false);
                        PinBallUIPanel.gameObject.SetActive(true);
                        PPM.GameMode = 1;
                        GameRunning = true;
                    });
                }
                if (datarecieved.Length > 1 && datarecieved[1] == "BALL")
                {
                    Actions.Add(delegate ()
                    {
                        string[] posdata = datarecieved[0].Split('|');
                        PPM.Ball.transform.position = Vector3.Lerp(PPM.Ball.transform.position, new Vector3(float.Parse(posdata[0]), float.Parse(posdata[1]), float.Parse(posdata[2])), 6 * Time.fixedDeltaTime);
                    });
                }
                if (datarecieved.Length > 1 && datarecieved[1] == "PADDLE")
                {
                    Actions.Add(delegate ()
                    {
                        string[] posdata = datarecieved[0].Split('|');
                        PPM.Paddle1.transform.position = Vector3.Lerp(PPM.Paddle1.transform.position, new Vector3(float.Parse(posdata[0]), float.Parse(posdata[1]), float.Parse(posdata[2])), 6 * Time.fixedDeltaTime);
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Application.Quit();
        }
        
    }
    /// <summary> Executes the application quit action. </summary>
    public void OnApplicationQuit()
    {
        // Make sure the thread is stopped before unity quits or in the editor unity can keep the
        // thread running and cause problems.
        ClientThread.Abort();
        ClientThread = null;
    }


}
