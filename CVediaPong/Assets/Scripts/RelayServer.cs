using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

// State object for reading client data asynchronously  
public class AsyncObject
{
    // Client  socket.  
    public Socket workSocket = null;
    // Size of receive buffer.  
    public const int BufferSize = 1024;
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];
    // Received data string.  
    public string sb = string.Empty;
}

// Just a couple functions to speed up and optimize the server code
public static class Helpers
{
    // Shifts a byte array down and resets and remaining bytes by a index
    public static void ShiftRemove(this byte[] lst, int shifts)
    {
        Array.Clear(lst, 0, shifts);
        for (int i = shifts; i < lst.Length; i++)
        {
            lst[i - shifts] = lst[i];
        }

        for (int i = lst.Length - shifts; i < lst.Length; i++)
        {
            lst[i] = default(byte);
        }
    }

    // Cuts a string based off of the indexs
    public static string Slice(this string source, int start, int end)
    {
        if (end < 0) // Keep this for negative end support
        {
            end = source.Length + end;
        }
        int len = end - start;               // Calculate length
        return source.Substring(start, len); // Return Substring of length
    }

    // Get bytes is basically Encoding.ASCII.GetBytes but faster because it uses the same byte array and avoid memory issues
    // on large servers that use bytes over around 2048
    public static byte[] GetBytes(this string source, byte[] bytes)
    {
        Array.Clear(bytes, 0, bytes.Length);
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] <= 0x7f)
            {
                bytes[i] = (byte)source[i];
            }
            else
            {
                // Verify the fallback character for non-ASCII chars
                bytes[i] = (byte)'?';
            }
        }
        return bytes;
    }
}

// A Asynchronous Relay Server the server is asynchronous so the
// server is non-blocking and it doesnt require threads and is faster.
public class RelayServer : MonoBehaviour {

    // The server socket
    public Socket listener;
    public bool ClientConnected = false;
    public Socket client;
    // The ping pong manager which we get the positions of the paddles and ball
    public PingPongManager PPM;

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

        // Start an asynchronous socket to listen for connections.  
        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
        UnityEngine.Debug.Log("Server started...");
    }
    // Async Server callback
    public void AcceptCallback(IAsyncResult ar)
    {
        // Get the socket that handles the client request.  
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        if (client == null)
            handler = client;

        // Create the state object.  
        AsyncObject state = new AsyncObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, AsyncObject.BufferSize, 0,new AsyncCallback(ReceiveCallback), state);
    }
    public void ReceiveCallback(IAsyncResult AR)
    {

        // Retrieve the state object and the handler socket  
        // from the asynchronous state object.  
        AsyncObject state = (AsyncObject)AR.AsyncState;
        Socket current = state.workSocket;
        int received;

        try
        {
            received = current.EndReceive(AR);
        }
        catch (Exception ex)
        {
            // Don't shutdown because the socket may be disposed and its disconnected anyway.
            return;
        }

        state.sb += (Encoding.ASCII.GetString(state.buffer, 0, received));
        state.sb = state.sb.Replace("\0", string.Empty);
        string text = state.sb;
        int endindex = text.IndexOf('^');

        // Not all data has been recived
        if (endindex == -1)
        {
            current.BeginReceive(state.buffer, 0, AsyncObject.BufferSize, SocketFlags.None, ReceiveCallback, state);
            return;
        }
        // Enough data has been recived for it to be a message
        else
        {
            string segment = text.Slice(0, endindex);
            string[] datarecieved = segment.Split(';');
            state.sb = state.sb.Remove(0, endindex + 1);
            state.buffer.ShiftRemove(endindex + 1);

            if (datarecieved.Length > 1 && datarecieved[1] == "START")
            {
                ClientConnected = true;
                goto EndOfRead;
            }
        }

        EndOfRead:

        try
        {
            state.workSocket.BeginReceive(state.buffer, 0, AsyncObject.BufferSize, SocketFlags.None, ReceiveCallback, state);
        }
        catch (ObjectDisposedException e)
        {
            //Form1.Self.AddToLog("Client forcefully disconnected");
            // Don't shutdown because the socket may be disposed and its disconnected anyway.
            state.workSocket.Close();
        }

    }

}
