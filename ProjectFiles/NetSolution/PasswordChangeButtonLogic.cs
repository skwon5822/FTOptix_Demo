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

public class PasswordChangeButtonLogic : BaseNetLogic
{

    [ExportMethod]
    public void ChangePassword()
    {
        
        var passchgdialogbox = LogicObject.GetAlias("PasswordExpiredDialogType") as DialogType;
        if (passchgdialogbox == null)
        {
            Log.Error("PasswordChangeButtonLogic", "Missing PasswordExpiredDialogType alias");
            return;
        }

        var CurrUserName = Session.User;
        var ownerButton = (Button)Owner;
        passchgdialogbox.GetVariable("ShowCurrentUser").Value = true;
        passchgdialogbox.GetVariable("ShowLableText").Value = false;
        ownerButton.OpenDialog(passchgdialogbox, CurrUserName.NodeId);
        
    }
}
