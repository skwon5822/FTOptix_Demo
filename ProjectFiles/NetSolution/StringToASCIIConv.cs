#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Alarm;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.Recipe;
using FTOptix.EventLogger;
using FTOptix.DataLogger;
using FTOptix.SQLiteStore;
using FTOptix.Store;
using FTOptix.Report;
using FTOptix.RAEtherNetIP;
using FTOptix.Modbus;
using FTOptix.Retentivity;
using FTOptix.AuditSigning;
using FTOptix.CommunicationDriver;
using FTOptix.Core;
using FTOptix.WebUI;
#endregion

public class StringToASCIIConv : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    [ExportMethod]
    public void ConvertStringToInt()
    {
        string inString = Project.Current.GetVariable("Model/InpString").Value;
        char[] AsciiValues = inString.ToCharArray();
        int[] IntValues = new int[20];
        int len = inString.Length;
        int cont = 0;
        if (len < 20)
        {
            cont = len;
        }
        else
        {
            cont = 20;
        }
        for (int i1 = 0;i1 < cont;i1++)
        {
            IntValues[i1] = Convert.ToInt16(AsciiValues[i1]);
        }
        Project.Current.GetVariable("Model/OutIntArray").Value = IntValues;
    }
}
