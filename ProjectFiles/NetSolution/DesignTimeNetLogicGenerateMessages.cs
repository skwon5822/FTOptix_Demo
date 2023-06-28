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

public class DesignTimeNetLogicGenerateMessages : BaseNetLogic
{
    [ExportMethod]
    public void GenerateMessages()
    {
        // Insert code to be executed by the method
        Log.Info(LogicObject.BrowseName, "Information Message from design time net logic");
        Log.Warning(LogicObject.BrowseName, "Warning Message from design time net logic");
        Log.Error(LogicObject.BrowseName, "Error Message from design time net logic");
    }
}
