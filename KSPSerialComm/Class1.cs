using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.IO.Ports;

//For DataRecieve
using System.Text;

[KSPAddon(KSPAddon.Startup.Flight, false)]
public class KSPSerialComm : MonoBehaviour
{
    //Setup parameters to connect to Arduino
    public static SerialPort sp = new SerialPort("COM2", 57600, Parity.None, 8, StopBits.One);

    public static string strIn;
    public static Vessel vessel;

    // Use this for initialization
    void Start()
    {
        print(" ----- KSPSerialComm Started ----- ");
        //sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        vessel = FlightGlobals.ActiveVessel;
        OpenConnection();
    }

    bool on = true;
    int count = 0;
    int count_limit = 100;
    void Update()
    {
        count++;

        if (count == count_limit)
        {
            count = 0;
            if (!on)
            {
                Console.WriteLine("Sending a 1");
                print("Sending a 1");
                // Sends the text as a byte.
                //sp.Write(new byte[] { Convert.ToByte("1") }, 0, 1);
                sp.Write("1");
                on = true;
            }
            else
            {
                Console.WriteLine("Sending a 0");
                print("Sending a 0");
                // Sends the text as a byte.
                //sp.Write(new byte[] { Convert.ToByte("0") }, 0, 1);
                sp.Write("0");
                on = false;
            }
        }
        /*if (count == (count_limit/2))
        {
            print("Mission Time: " + vessel.missionTime.ToString());
            //sp.Write(new byte[] { Convert.ToByte(vessel.missionTime) }, 0, 1);
            sp.Write(vessel.missionTime.ToString());
        }*/
        sp.Write(vessel.missionTime.ToString());
        try
        {
            string indata = sp.ReadExisting();
            if(indata != ""){
                print("Input recieved:" + indata + "\n");
                //FlightGlobals.Vessels.
                //GameSettings.SAS_TOGGLE.Equals(true);
                //ControlTypes.SAS
                //Vessel vessel;
                if (indata.Equals("A"))
                {
                    print("SAS-On");
                    //fcs.killRot = true;
                    //GameSettings.SAS_TOGGLE.Equals("true");
                    //print(GameSettings.SAS_TOGGLE.Equals("true"));
                    vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, true);
                    vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, true);
                }
                if (indata.Equals("B"))
                {
                    print("SAS-Off");
                    //fcs.killRot = false;
                    //GameSettings.SAS_TOGGLE.Equals("false");
                    //print(GameSettings.SAS_TOGGLE.Equals("false"));
                    vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, false);
                    vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, false);
                }
            }
        }
        catch (Exception ex) {print(ex.Message.ToString());}
        
    }

    //Function connecting to Arduino
    public void OpenConnection()
    {
        if (sp != null)
        {
            if (sp.IsOpen)
            {
                sp.Close();
                print("Closing port, because it was already open!");
            }
            else
            {
                sp.DtrEnable = true; //added for arduino communication (?)
                sp.Open();  // opens the connection
                sp.ReadTimeout = 200;  // sets the timeout value before reporting error
                print("Port Opened!");
            }
        }
        else
        {
            if (sp.IsOpen)
            {
                print("Port is already open");
            }
            else
            {
                print("Port == null");
            }
        }
    }

    void OnApplicationQuit()
    {
        sp.Close();
    }

    //Does not get called...
    private static void DataReceivedHandler(object sender,
                                   SerialDataReceivedEventArgs e)
    {
        print("DataRecieved Handler called");
        /*
        SerialPort spL = (SerialPort)sender;
        byte[] buf = new byte[spL.BytesToRead];
        //Console.WriteLine("DATA RECEIVED!");
        print("DATA RECEIVED!");
        spL.Read(buf, 0, buf.Length);
        foreach (Byte b in buf)
        {
            //Console.Write(b.ToString());
            print("Input recieved:" + b.ToString() + "\n");
        }
        //Console.WriteLine();
         */
        try
        {
            //string indata = sp.ReadExisting();

            string indata = sp.ReadLine();

            print("Input recieved:" + indata + "\n");
        }
        catch (Exception ex) { print(ex.Message.ToString()); }
    }
}