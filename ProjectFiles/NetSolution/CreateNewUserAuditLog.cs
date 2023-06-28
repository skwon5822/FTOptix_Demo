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

public class CreateNewUserAuditLog : BaseNetLogic
{
    //private Store myStore;
    //private Table myTable;
    /*
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        Store myStore = Project.Current.Get<Store>("DataStores/EDB_AuditTrail");
        Table myTable = myStore.Tables.Get<Table>("SigningEventLogger");
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }
    */

    [ExportMethod]
    public void Insert(string username, string currentuser)
    {
        Store myStore = Project.Current.Get<Store>("DataStores/EDB_AuditTrail");
        Table myTable = myStore.Tables.Get<Table>("SigningEventLogger");

        string curruser = Project.Current.GetVariable("Model/CurrentUser").Value;
        object[,] rawValues = new object [1,9]; // insert 1 row with 3 columns
        rawValues[0,0] = DateTime.Now;
        rawValues[0,1] = "New User Created";
        rawValues[0,2] = "";
        rawValues[0,3] = username;
        rawValues[0,4] = curruser;
        rawValues[0,5] = "";
        rawValues[0,6] = "";
        rawValues[0,7] = "";
        rawValues[0,8] = "User Creation Event";
        string[] columns = new string[9] {"LocalTimeStamp", "SourceName", "OldValue", "NewValue", "ClientUserId", "ClientUserNote", "SecondClientUserId", "SecondClientUserNote", "EventType"};
        myTable.Insert(columns,rawValues);
        
    }

}
