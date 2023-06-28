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

public class PeriodicExecutionScriptToCopyVar : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        periodicTask = new PeriodicTask(CopyVar, 500, LogicObject);
        periodicTask.Start();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        periodicTask.Dispose();
        periodicTask = null;
    }

    private void CopyVar() 
    {
        // Copy Variables 
        Project.Current.GetVariable("Model/NetLogic/ActualVariable").Value = Project.Current.GetVariable("Model/NetLogic/SetVariable").Value;
        Project.Current.GetVariable("Model/NetLogic/ActualText").Value = Project.Current.GetVariable("Model/NetLogic/SetText").Value;

        // Addition Operation
        float add1 = Project.Current.GetVariable("Model/NetLogic/AddOper1").Value;
        float add2 = Project.Current.GetVariable("Model/NetLogic/AddOper2").Value;
        Project.Current.GetVariable("Model/NetLogic/AddResult").Value = add1 + add2;

        // Subtraction Operation
        float sub1 = Project.Current.GetVariable("Model/NetLogic/SubOper1").Value;
        float sub2 = Project.Current.GetVariable("Model/NetLogic/SubOper2").Value;
        Project.Current.GetVariable("Model/NetLogic/SubResult").Value = sub1 - sub2;

        // Multiplication Operation
        float mul1 = Project.Current.GetVariable("Model/NetLogic/MulOper1").Value;
        float mul2 = Project.Current.GetVariable("Model/NetLogic/MulOper2").Value;
        Project.Current.GetVariable("Model/NetLogic/MulResult").Value = mul1 * mul2;

        // Division Operation
        float div1 = Project.Current.GetVariable("Model/NetLogic/DivOper1").Value;
        float div2 = Project.Current.GetVariable("Model/NetLogic/DivOper2").Value;
        Project.Current.GetVariable("Model/NetLogic/DivResult").Value = div1 / div2;

    }

    private PeriodicTask periodicTask;
}
