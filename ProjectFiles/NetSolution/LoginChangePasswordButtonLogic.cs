#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.NativeUI;
using FTOptix.HMIProject;
using FTOptix.UI;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.Store;
using FTOptix.Retentivity;
using FTOptix.RAEtherNetIP;
using FTOptix.CommunicationDriver;
using FTOptix.Modbus;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.Recipe;
using FTOptix.Report;
using FTOptix.WebUI;
#endregion

public class LoginChangePasswordButtonLogic : BaseNetLogic
{
    [ExportMethod]
    public void PerformChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        var outputMessageLabel = Owner.Owner.GetObject("ChangePasswordFormOutputMessage");
        var outputMessageLogic = outputMessageLabel.GetObject("LoginChangePasswordFormOutputMessageLogic");

        // Check Password for Complaxity
		//--------------------------------
		string EnteredPassword = newPassword;
		/*
		// Check Password Length
        int PassLength = 0;
        if (EnteredPassword.Length >= 8)
        {
            PassLength += 1;
        }
		*/
                
		// Check For Special Character in Password
        string specialCh = "~`!@#$%^&*()_-+={[}]|:;'<,>.?/";
        char[] CharArray = specialCh.ToCharArray();
        int SplChrCheck = 0;
        foreach (char ch1 in CharArray) 
        {
            if (EnteredPassword.Contains(ch1))
            {
                SplChrCheck += 1;
            }
        }
        
		// Check For Upper Case Character in Password
        string upercasechar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] upCharArray = upercasechar.ToCharArray();
        int upercasecheck = 0;
        foreach (char ch2 in upCharArray) 
        {
            if (EnteredPassword.Contains(ch2))
            {
                upercasecheck += 1;
            }
        }

		// Check For Lower Case Character in Password
        string lowercasechar = "abcdefghijklmnopqrstuvwxyz";
        char[] loCharArray = lowercasechar.ToCharArray();
        int lowercasecheck = 0;
        foreach (char ch3 in loCharArray) 
        {
            if (EnteredPassword.Contains(ch3))
            {
                lowercasecheck += 1;
            }
        }

		// Check For Numeric Character in Password
        string numchar = "0123456789";
        char[] numCharArray = numchar.ToCharArray();
        int numcheck = 0;
        foreach (char ch4 in numCharArray) 
        {
            if (EnteredPassword.Contains(ch4))
            {
                numcheck += 1;
            }
        }

        /*
        if ((SplChrCheck <= 0) || (upercasecheck <= 0) || (lowercasecheck <= 0) || (numcheck <= 0))
        {
            ShowMessage(20);
			Log.Error("EditUserDetailPanelLogic", "Entered Password is not Complex");
			return NodeId.Empty;
        }
        */

        if ((SplChrCheck <= 0) || (upercasecheck <= 0) || (lowercasecheck <= 0) || (numcheck <= 0))
        {
            outputMessageLogic.ExecuteMethod("SetOutputMessage", new object[] { 20 });
        }
        else if (newPassword != confirmPassword)
        {
            outputMessageLogic.ExecuteMethod("SetOutputMessage", new object[] { 7 });
        }
        else
        {
            var username = Session.User.BrowseName;
            try
            {
                var userWithExpiredPassword = Owner.GetAlias("UserWithExpiredPassword");
                if (userWithExpiredPassword != null)
                    username = userWithExpiredPassword.BrowseName;
            }
            catch
            {
            }

            var result = Session.ChangePassword(username, newPassword, oldPassword);
            if (result.ResultCode == ChangePasswordResultCode.Success)
            {
                LogIntoAudit("User Modified", "'" + username + "'" + " Password Chaged", "UserPasswordChangeEvent");
                var parentDialog = Owner.Owner?.Owner?.Owner as Dialog;
                if (parentDialog != null && result.Success)
                    parentDialog.Close();
            }
            else
            {
                outputMessageLogic.ExecuteMethod("SetOutputMessage", new object[] { (int)result.ResultCode });
            }
        }
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
