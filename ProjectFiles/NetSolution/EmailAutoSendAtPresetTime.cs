#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.CoreBase;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.Alarm;
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
#endregion

public class EmailAutoSendAtPresetTime : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        periodicTask = new PeriodicTask(CheckEmialSendTime, 60000, LogicObject);
		periodicTask.Start();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        periodicTask.Dispose();
		periodicTask = null;
    }

    private void CheckEmialSendTime()
    {
        DateTime EmailSendPresetTime = Project.Current.GetVariable("Model/Email/AutoEmailTime").Value;
        
        int PresetHour = EmailSendPresetTime.Hour;
        int PresetMinute = EmailSendPresetTime.Minute;
		int ActHour = DateTime.Now.Hour;
		int ActMinute = DateTime.Now.Minute;

        if ((PresetHour == ActHour) && (PresetMinute == ActMinute))
        {
            Project.Current.GetVariable("Model/Email/SendEmail").Value = true;
        }
        else
        {
            Project.Current.GetVariable("Model/Email/SendEmail").Value = false;
        }
        
    }

    private PeriodicTask periodicTask;
}
