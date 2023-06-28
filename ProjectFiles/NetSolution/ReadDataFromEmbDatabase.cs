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

public class ReadDataFromEmbDatabase : BaseNetLogic
{
    private Store myStore;
    private Table myTable;

    [ExportMethod]
    public void ReadData(string SetSQLQuery)
    {
        myStore = Project.Current.Get<Store>("DataStores/EDB_TempLogger");
        myTable = myStore.Tables.Get<Table>("TempLogger");

        //out object[,] resultSet; 
        //out string[] header;
        //myStore.Query($"SELECT TemperatureDeviation FROM TempLogger WHERE LocalTimestamp BETWEEN '{FromDateTime.ToString()}' AND '{ToDateTime.ToString()}'", out string[] header, out object[,] resultSet);
        //string tempvalue = "100";
        //myStore.Query($"SELECT TemperatureDeviation FROM TempLogger WHERE SetTemperature = '{tempvalue}'", out string[] header, out object[,] resultSet);

        myStore.Query(SetSQLQuery, out string[] header, out object[,] resultSet);
        
        //int rawnum = resultSet.GetLength(0) - 1;
        //int ColmNum = resultSet.GetLength(1) - 1;

        float fromragneG1 = Project.Current.GetVariable("Model/TempLogging/FromValueGroup1").Value;
        float toragneG1 = Project.Current.GetVariable("Model/TempLogging/ToValueGroup1").Value;
        float fromragneG2 = Project.Current.GetVariable("Model/TempLogging/FromValueGroup2").Value;
        float toragneG2 = Project.Current.GetVariable("Model/TempLogging/ToValueGroup2").Value;
        float fromragneG3 = Project.Current.GetVariable("Model/TempLogging/FromValueGroup3").Value;
        float toragneG3 = Project.Current.GetVariable("Model/TempLogging/ToValueGroup3").Value;
        float fromragneG4 = Project.Current.GetVariable("Model/TempLogging/FromValueGroup4").Value;
        float toragneG4 = Project.Current.GetVariable("Model/TempLogging/ToValueGroup4").Value;
        float fromragneG5 = Project.Current.GetVariable("Model/TempLogging/FromValueGroup5").Value;
        float toragneG5 = Project.Current.GetVariable("Model/TempLogging/ToValueGroup5").Value;

        Int32 RangeCountsG1 = 0;
        Int32 RangeCountsG2 = 0;
        Int32 RangeCountsG3 = 0;
        Int32 RangeCountsG4 = 0;
        Int32 RangeCountsG5 = 0;
        if (resultSet.GetLength(0) > 0)
        {
            for (Int32 i = 0; i < resultSet.GetLength(0); i++)
            {
                //if (resultSet[i,0].ToString() != String.Empty)
                if (resultSet[i,0] != null)
                {
                    string readvalue = resultSet[i,0].ToString();
                    //string stValue32 = readvalue.ToString();
                    float TempDevValue = float.Parse(readvalue);
                    if ((TempDevValue >= fromragneG1) && (TempDevValue <= toragneG1))
                    {
                        RangeCountsG1 += 1;
                    }
                    else if ((TempDevValue >= fromragneG2) && (TempDevValue <= toragneG2))
                    {
                        RangeCountsG2 += 1;
                    }
                    else if ((TempDevValue >= fromragneG3) && (TempDevValue <= toragneG3))
                    {
                        RangeCountsG3 += 1;
                    }
                    else if ((TempDevValue >= fromragneG4) && (TempDevValue <= toragneG4))
                    {
                        RangeCountsG4 += 1;
                    }
                    else if ((TempDevValue >= fromragneG5) && (TempDevValue <= toragneG5))
                    {
                        RangeCountsG5 += 1;
                    }
                }
            }
        }
        
        Project.Current.GetVariable("Model/TempLogging/ChartVars/Group1Counts").Value = RangeCountsG1;
        Project.Current.GetVariable("Model/TempLogging/ChartVars/Group2Counts").Value = RangeCountsG2;
        Project.Current.GetVariable("Model/TempLogging/ChartVars/Group3Counts").Value = RangeCountsG3;
        Project.Current.GetVariable("Model/TempLogging/ChartVars/Group4Counts").Value = RangeCountsG4;
        Project.Current.GetVariable("Model/TempLogging/ChartVars/Group5Counts").Value = RangeCountsG5;
        
    }

}
