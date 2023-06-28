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

public class ModbusIntToStringConv : BaseNetLogic
{
    [ExportMethod]
    public void ModbusConvertIntToString()
    {
        char Char1 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt1").Value;
        char Char2 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt2").Value;
        char Char3 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt3").Value;
        char Char4 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt4").Value;
        char Char5 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt5").Value;
        char Char6 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt6").Value;
        char Char7 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt7").Value;
        char Char8 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt8").Value;
        char Char9 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt9").Value;
        char Char10 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt10").Value;
        char Char11 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt11").Value;
        char Char12 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt12").Value;
        char Char13 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt13").Value;
        char Char14 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt14").Value;
        char Char15 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt15").Value;
        char Char16 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt16").Value;
        char Char17 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt17").Value;
        char Char18 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt18").Value;
        char Char19 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt19").Value;
        char Char20 = (char) Project.Current.GetVariable("CommDrivers/ModbusDriver1/ModbusStation1/Tags/StringVar1IntArray/S1VarInt20").Value;
        
        Project.Current.GetVariable("Model/ModbusString/StringVar1").Value = Char1.ToString() + Char2.ToString() + Char3.ToString() + Char4.ToString() + Char5.ToString() + Char6.ToString() + Char7.ToString() + Char8.ToString() + Char9.ToString() + Char10.ToString() + Char11.ToString() + Char12.ToString() + Char13.ToString() + Char14.ToString() + Char15.ToString() + Char16.ToString() + Char17.ToString() + Char18.ToString() + Char19.ToString() + Char20.ToString();
    }
}
