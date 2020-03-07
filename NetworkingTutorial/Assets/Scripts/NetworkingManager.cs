using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
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

    //Fields for username, ip and errors
    public InputField username;
    public InputField ipAddress;
    public GameObject errorMessage;
    private float errorCounter = 0;
    public float errorTime = 10.0f;

    public GameObject loginCanvas;
    public GameObject chatCanvas;

    // Fields we need later
    public GameObject textboxPrefab;
    public GameObject textboxParent;
    public GameObject userPrefab;
    public GameObject userParent;
    public InputField textinput;

    public float activityTime = 80.0f;
    public float mutexTime = 0.5f;

    private static bool mutex = false;

    static List<MsgToPopulate> msgs = new List<MsgToPopulate>();
    static List<ClientConnectionData> clients = new List<ClientConnectionData>();
    static ClientConnectionData user = new ClientConnectionData();
    private float mutexCounter = 0;
    private float activityCounter = 0;

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
        mutexCounter += Time.fixedDeltaTime;
        activityCounter += Time.fixedDeltaTime;

        if (errorMessage.activeSelf)
        {
            errorCounter += Time.fixedDeltaTime;
        }

        if (mutexCounter >= mutexTime)
        {
            mutex = true;

            UpdateData();

            mutex = false;
        }

        if (activityCounter >= activityTime)
        {
            SendPacketToServer("s;" + user.id.ToString() + ";IDLE");
            activityCounter = 0;
        }

        if (errorCounter >= errorTime)
        {
            errorMessage.SetActive(false);
            username.colors = ColorBlock.defaultColorBlock;
            errorCounter = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendCurrentMessage();
        }
    }

    // Update client and message data
    private void UpdateData()
    {
        if (msgs.Count > 0)
        {
            for (int i = msgs.Count - 1; i >= 0; i--)
            {
                GameObject go = Instantiate(textboxPrefab, textboxParent.transform);
                for (int j = 0; j < clients.Count; j++)
                {
                    if (clients[j].id == msgs[i].id)
                    {
                        go.GetComponent<TextElement>().UpdateText(clients[j].name, msgs[i].msg);
                        

                        break;
                    }
                }
                msgs.Remove(msgs[i]);
            }
        }

        if (clients.Count > 0)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].go == null)
                {
                    clients[i].go = Instantiate(userPrefab, userParent.transform).GetComponent<UserElement>();
                }
                clients[i].go.UpdateUser(clients[i].name, clients[i].status);
            }
        }
    }

    // Init the server
    public void StartServer()
    {
        if (username.text != "")
        {
            if (ipAddress.text == "")
            {
                ipAddress.text = "127.0.0.1";
            }

            InitServer(ipAddress.text, 54000);
        }
    }

    // Init the client
    public void StartClient()
    {
        if (username.text != "")
        {
            user.name = username.text;
            if (ipAddress.text == "")
            {
                ipAddress.text = "127.0.0.1";
            }
            InitClient(ipAddress.text, 54000, user.name);

            loginCanvas.SetActive(false);
            chatCanvas.SetActive(true);
        }
        else
        {
            errorMessage.SetActive(true);
            ColorBlock temp = ColorBlock.defaultColorBlock;
            temp.normalColor = Color.red;
            username.colors = temp;
        }
        
    }
    
    // Where we'll process incoming messages
    public static void MsgReceived(IntPtr p_in)
    {
        string p = Marshal.PtrToStringAnsi(p_in);
        Debug.Log(p);

        while (mutex)
        { } // wait
        // Look up mutex or semaphore

        switch(p[0])
        {
            case 'i':
            {
                //Splits sptring into array, splitting whereever there's a ;
                string[] ar = p.Split(';');
                user.id = int.Parse(ar[1]);
                //May want to use TryParse (avoids breaking shit)

                break;
            }
            case 'c':   //c;NAME;STATUS;00
            {
                ClientConnectionData temp = new ClientConnectionData();
                string[] ar = p.Split(';');

                temp.name = ar[1];
                temp.status = ar[2];
                temp.id = Int16.Parse(ar[3]);

                clients.Add(temp);

                break;
            }
            case 's':   //s;00;STATUS
            {
                string[] ar = p.Split(';');
                int id = int.Parse(ar[1]);

                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].id == id)
                    {
                        clients[i].status = ar[2];
                        
                        return;
                    }
                }

                break;
            }
            case 'm':   //m;00;MESSAGE
            {
                string[] ar = p.Split(';');
                int id = int.Parse(ar[1]);
                string msg = ar[2];

                MsgToPopulate msgp = new MsgToPopulate();
                msgp.msg = msg;
                msgp.id = id;

                msgs.Add(msgp);

                break;
            }
        }
    }

    public void SendCurrentMessage()
    {
        SendPacketToServer("m;" + user.id.ToString() + ";" + textinput.text);
        SendPacketToServer("s;" + user.id.ToString() + ";CHATTING");
        

        GameObject go = Instantiate(textboxPrefab, textboxParent.transform);
        go.GetComponent<TextElement>().UpdateText(user.name, textinput.text);


        textinput.text = "";
        activityCounter = 0;
    }

    private void OnApplicationQuit()
    {
        Cleanup();
        ManualPluginImporter.CloseLibrary(Plugin_Handle);
    }
}
