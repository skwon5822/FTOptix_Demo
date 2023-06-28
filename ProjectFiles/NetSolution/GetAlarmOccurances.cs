#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.WebUI;
using FTOptix.CoreBase;
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

public class GetAlarmOccurances : BaseNetLogic
{
    private Store myStore;
    private Table myTable;

    [ExportMethod]
    public void GetOccurances(string SetSQLQuery)
    {
        myStore = Project.Current.Get<Store>("DataStores/EDB_AlarmLogging");
        myTable = myStore.Tables.Get<Table>("AlarmsEventLogger1");
        
        myStore.Query(SetSQLQuery, out string[] header, out object[,] resultSet);
        
        //int rawnum = resultSet.GetLength(0) - 1;
        //int ColmNum = resultSet.GetLength(1) - 1;

        string Alarm1 = "EmeStopActive";
        string Alarm2 = "SafetyDoorOpen";
        string Alarm3 = "CommLost";
        string Alarm4 = "TempHighAlarm";
        string Alarm5 = "TempLowAlarm";
        string Alarm6 = "PresHighAlarm";
        string Alarm7 = "PresLowAlarm";

        Int32 OccA1 = 0;
        Int32 OccA2 = 0;
        Int32 OccA3 = 0;
        Int32 OccA4 = 0;
        Int32 OccA5 = 0;
        Int32 OccA6 = 0;
        Int32 OccA7 = 0;

        if (resultSet.GetLength(0) > 0)
        {
            for (Int32 i = 0; i < resultSet.GetLength(0); i++)
            {
                //if (resultSet[i,0].ToString() != String.Empty)
                if (resultSet[i,0] != null)
                {
                    string readvalue = resultSet[i,0].ToString();
                    if (readvalue == Alarm1)
                    {
                        OccA1 += 1;
                    }
                    else if (readvalue == Alarm2)
                    {
                        OccA2 += 1;
                    }
                    else if (readvalue == Alarm3)
                    {
                        OccA3 += 1;
                    }
                    else if (readvalue == Alarm4)
                    {
                        OccA4 += 1;
                    }
                    else if (readvalue == Alarm5)
                    {
                        OccA5 += 1;
                    }
                    else if (readvalue == Alarm6)
                    {
                        OccA6 += 1;
                    }
                    else if (readvalue == Alarm7)
                    {
                        OccA7 += 1;
                    }
                }
            }
        }
        
        Project.Current.GetVariable("Model/Alarms/AlarmList/EStop_Active").Value = OccA1;
        Project.Current.GetVariable("Model/Alarms/AlarmList/Safety_Door_Open").Value = OccA2;
        Project.Current.GetVariable("Model/Alarms/AlarmList/Communication_Lost").Value = OccA3;
        Project.Current.GetVariable("Model/Alarms/AlarmList/Temperature_High").Value = OccA4;
        Project.Current.GetVariable("Model/Alarms/AlarmList/Temperature_Low").Value = OccA5;
        Project.Current.GetVariable("Model/Alarms/AlarmList/Pressure_High").Value = OccA6;
        Project.Current.GetVariable("Model/Alarms/AlarmList/Pressure_Low").Value = OccA7;
        
    }
}
