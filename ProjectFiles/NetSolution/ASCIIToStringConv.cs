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

public class ASCIIToStringConv : BaseNetLogic
{
    [ExportMethod]
    public void ConvertIntToString()
    {
        int [] InintValues = new int [20];
        char [] InAsciiValues = new char[20];
        string OutSt = "";

        InintValues[0] = Project.Current.GetVariable("Model/InInt1").Value;
        InintValues[1] = Project.Current.GetVariable("Model/InInt2").Value;
        InintValues[2] = Project.Current.GetVariable("Model/InInt3").Value;
        InintValues[3] = Project.Current.GetVariable("Model/InInt4").Value;
        InintValues[4] = Project.Current.GetVariable("Model/InInt5").Value;
        for (int i1 = 0;i1 < 20;i1++)
        {
            InAsciiValues[i1] = Convert.ToChar(InintValues[i1]);
        }
        //InAsciiValues[0] = Convert.ToChar(InintValues[0]);
        OutSt = new string(InAsciiValues);
        Project.Current.GetVariable("Model/OutString").Value = OutSt;
        //int IntChar = Project.Current.GetVariable("Model/intChar1").Value;
        //char Char1 = (char) IntChar;
        //Project.Current.GetVariable("Model/OutString").Value = Char1.ToString();
    }
}
