#region Using directives
using System;
using FTOptix.CoreBase;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.Core;
using FTOptix.UI;
using FTOptix.RAEtherNetIP;
using FTOptix.CommunicationDriver;
using FTOptix.Modbus;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.Recipe;
using FTOptix.Report;
using FTOptix.WebUI;
using FTOptix.Store;
#endregion

public class LoginButtonLogic : BaseNetLogic
{
    public override void Start()
    {
        ComboBox comboBox = Owner.Owner.Get<ComboBox>("Username");
        if (Project.Current.AuthenticationMode == AuthenticationMode.ModelOnly)
        {
            comboBox.Mode = ComboBoxMode.Normal;
        }
        else
        {
            comboBox.Mode = ComboBoxMode.Editable;
        }
    }

    public override void Stop()
    {

    }

    [ExportMethod]
    public void PerformLogin(string username, string password)
    {
        var usersAlias = LogicObject.GetAlias("Users");
        if (usersAlias == null || usersAlias.NodeId == NodeId.Empty)
        {
            Log.Error("LoginButtonLogic", "Missing Users alias");
            return;
        }

        var passwordExpiredDialogType = LogicObject.GetAlias("PasswordExpiredDialogType") as DialogType;
        if (passwordExpiredDialogType == null)
        {
            Log.Error("LoginButtonLogic", "Missing PasswordExpiredDialogType alias");
            return;
        }

        Button loginButton = (Button)Owner;
        loginButton.Enabled = false;

        try
        {
            var loginResult = Session.Login(username, password);
            if (loginResult.ResultCode == ChangeUserResultCode.PasswordExpired)
            {
                loginButton.Enabled = true;
                LogIntoAudit("User Password Expired", username, "UserLoginEvent");
                var user = usersAlias.Get<User>(username);
                var ownerButton = (Button)Owner;
                passwordExpiredDialogType.GetVariable("ShowCurrentUser").Value = false;
                passwordExpiredDialogType.GetVariable("ShowLableText").Value = true;
                ownerButton.OpenDialog(passwordExpiredDialogType, user.NodeId);
                return;
            }
            else if (loginResult.ResultCode != ChangeUserResultCode.Success)
            {
                loginButton.Enabled = true;
                Log.Error("LoginButtonLogic", "Authentication failed");
                LogIntoAudit("User Login Failed", username, "UserLoginEvent");
            }

            if (loginResult.ResultCode != ChangeUserResultCode.Success)
            {
                var outputMessageLabel = Owner.Owner.GetObject("LoginFormOutputMessage");
                var outputMessageLogic = outputMessageLabel.GetObject("LoginFormOutputMessageLogic");
                outputMessageLogic.ExecuteMethod("SetOutputMessage", new object[] { (int)loginResult.ResultCode });
            }
        }
        catch (Exception e)
        {
            Log.Error("LoginButtonLogic", e.Message);
        }

        loginButton.Enabled = true;
    }

    private void LogIntoAudit(string clSourceName, string clMessage, string clEventName)
	{
		Store myStore = Project.Current.Get<Store>("DataStores/EDB_AuditTrail");
        Table myTable = myStore.Tables.Get<Table>("SigningEventLogger");

        string curruser = Session.User.BrowseName; //Project.Current.GetVariable("Model/CurrentUser").Value;
        object[,] rawValues = new object [1,8]; // insert 1 row with 3 columns
        rawValues[0,0] = DateTime.Now;
        rawValues[0,1] = clSourceName;
        rawValues[0,2] = clMessage;
        rawValues[0,3] = curruser;
        rawValues[0,4] = "";
        rawValues[0,5] = "";
        rawValues[0,6] = "";
        rawValues[0,7] = clEventName;
        string[] columns = new string[8] {"LocalTimeStamp", "SourceName", "Message", "ClientUserId", "ClientUserNote", "SecondClientUserId", "SecondClientUserNote", "EventType"};
        myTable.Insert(columns,rawValues);
	}
}
