using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.UI;

public struct CS_to_Plugin_Functions
{
    //public IntPtr testingfunc;
    //public IntPtr functionTwo;
    public IntPtr MsgReceivedPtr;

    // The functions don't need to be the same though
    // Init isn't in C++
    public bool Init()
    {
        MsgReceivedPtr = Marshal.GetFunctionPointerForDelegate(new Action<IntPtr>(NetworkingManager.MsgReceived));

        return true;
    }
}

public class ClientConnectionData
{
    public string name;
    public string status;
    public int id;
    public UserElement go;
}

public class MsgToPopulate
{
    public string msg;
    public int id;
}

public class NetworkingManager : MonoBehaviour
{
    // Same old DLL init stuff
    private const string path = "/Plugins/NetworkingTutorialDLL.dll";

    private IntPtr Plugin_Handle;
    private CS_to_Plugin_Functions Plugin_Functions;

    public delegate void InitDLLDelegate(CS_to_Plugin_Functions funcs);
    public InitDLLDelegate InitDLL;

    public delegate void InitServerDelegate(string IP, int port);
    public InitServerDelegate InitServer;

    public delegate void InitClientDelegate(string IP, int port, string name);
    public InitClientDelegate InitClient;

    public delegate void SendPacketToServerDelegate(string msg);
    public SendPacketToServerDelegate SendPacketToServer;

    public delegate void CleanupDelegate();
    public CleanupDelegate Cleanup;

    // MUST be called before you call any of the DLL functions
    private void InitDLLFunctions()
    {
        InitDLL = ManualPluginImporter.GetDelegate<InitDLLDelegate>(Plugin_Handle, "InitDLL");
        InitServer = ManualPluginImporter.GetDelegate<InitServerDelegate>(Plugin_Handle, "InitServer");
        InitClient = ManualPluginImporter.GetDelegate<InitClientDelegate>(Plugin_Handle, "InitClient");
        SendPacketToServer = ManualPluginImporter.GetDelegate<SendPacketToServerDelegate>(Plugin_Handle, "SendPacketToServer");
        Cleanup = ManualPluginImporter.GetDelegate<CleanupDelegate>(Plugin_Handle, "Cleanup");
    }

    // Fields we need later
    public GameObject textboxPrefab;
    public GameObject textboxParent;
    public GameObject userPrefab;
    public GameObject userParent;
    public InputField textinput;

    // Init the DLL
    private void Awake()
    {
        Plugin_Handle = ManualPluginImporter.OpenLibrary(Application.dataPath + path);
        Plugin_Functions.Init();

        InitDLLFunctions();

        InitDLL(Plugin_Functions);
    }

    private void Update()
    {
    }

    // Update client and message data
    private void UpdateData()
    {
    }

    // Init the server
    public void StartServer()
    {
    }

    // Init the client
    public void StartClient()
    {
    }
    
    // Where we'll process incoming messages
    public static void MsgReceived(IntPtr p_in)
    {
    }

    public void SendCurrentMessage()
    {
    }

    private void OnApplicationQuit()
    {
        Cleanup();
        ManualPluginImporter.CloseLibrary(Plugin_Handle);
    }
}
