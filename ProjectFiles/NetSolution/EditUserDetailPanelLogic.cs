#region Using directives
using System;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.Store;
using FTOptix.WebUI;
using System.Linq;
using FTOptix.RAEtherNetIP;
using FTOptix.CommunicationDriver;
using FTOptix.Modbus;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.Recipe;
using FTOptix.Report;
#endregion

public class EditUserDetailPanelLogic : BaseNetLogic
{
	[ExportMethod]
	public void SaveUser(string username, string password, string locale, out NodeId result)
	{
        result = NodeId.Empty;

		if (string.IsNullOrEmpty(username))
		{
			ShowMessage(1);
			Log.Error("EditUserDetailPanelLogic", "Cannot create user with empty username");
			return;
		}

        result = ApplyUser(username, password, locale);
	}

	private NodeId ApplyUser(string username, string password, string locale)
	{
		var users = GetUsers();
		if (users == null)
		{
			ShowMessage(2);
			Log.Error("EditUserDetailPanelLogic", "Unable to get users");
			return NodeId.Empty;
		}

		var user = users.Get<FTOptix.Core.User>(username);
		if (user == null)
		{
			ShowMessage(3);
			Log.Error("EditUserDetailPanelLogic", "User not found");
			return NodeId.Empty;
		}

		//Apply LocaleId
		if(!string.IsNullOrEmpty(locale))
			user.LocaleId = locale;

		//Apply groups
		ApplyGroups(user);

		
		// Check Password for Complaxity
		//--------------------------------
		string EnteredPassword = password;
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

        if ((SplChrCheck <= 0) || (upercasecheck <= 0) || (lowercasecheck <= 0) || (numcheck <= 0))
        {
            ShowMessage(20);
			Log.Error("EditUserDetailPanelLogic", "Entered Password is not Complex");
			return NodeId.Empty;
        }

        
		// Apply Password
		if (!string.IsNullOrEmpty(password))
		{
			var result = Session.ChangePasswordInternal(username, password);

			switch (result.ResultCode)
			{
				case FTOptix.Core.ChangePasswordResultCode.Success:
					LogIntoAudit("User Modified", "'" + username + "'" + " Password Chaged", "UserPasswordChangeEvent");
					var editPasswordTextboxPtr = LogicObject.GetVariable("PasswordTextbox");
					if (editPasswordTextboxPtr == null)
						Log.Error("EditUserDetailPanelLogic", "PasswordTextbox variable not found");

					var nodeId = (NodeId)editPasswordTextboxPtr.Value;
					if (nodeId == null)
						Log.Error("EditUserDetailPanelLogic", "PasswordTextbox not set");

					var editPasswordTextbox = (TextBox)InformationModel.Get(nodeId);
					if (editPasswordTextbox == null)
						Log.Error("EditUserDetailPanelLogic", "EditPasswordTextbox not found");

					editPasswordTextbox.Text = string.Empty;
					break;
				case FTOptix.Core.ChangePasswordResultCode.WrongOldPassword:
					//Not applicable
					break;
				case FTOptix.Core.ChangePasswordResultCode.PasswordAlreadyUsed:
					ShowMessage(4);
					return NodeId.Empty;
				case FTOptix.Core.ChangePasswordResultCode.PasswordChangedTooRecently:
					ShowMessage(5);
					return NodeId.Empty;
				case FTOptix.Core.ChangePasswordResultCode.PasswordTooShort:
					ShowMessage(6);
					return NodeId.Empty;
				case FTOptix.Core.ChangePasswordResultCode.UserNotFound:
					ShowMessage(7);
					return NodeId.Empty;
				case FTOptix.Core.ChangePasswordResultCode.UnsupportedOperation:
					ShowMessage(8);
					return NodeId.Empty;

			}
		}


		ShowMessage(9);
		return user.NodeId;
	}

	private void ApplyGroups(FTOptix.Core.User user)
	{
		Panel groupsPanel = Owner.Get<Panel>("HorizontalLayout1/GroupsPanel1");
		IUAVariable editable = groupsPanel.GetVariable("Editable");
		IUANode groups = groupsPanel.GetAlias("Groups");
		var panel = groupsPanel.Children.Get("ScrollView").Get("Container");

        if (editable.Value == false)
            return;

		if (user == null || groups == null || panel == null)
			return;

		var userNode = InformationModel.Get(user.NodeId);
        if (userNode == null)
            return;

		var groupCheckBoxes = panel.Refs.GetObjects(OpcUa.ReferenceTypes.HasOrderedComponent, false);
		string usergroupsselected = "";
		foreach (var groupCheckBoxNode in groupCheckBoxes)
		{
			var group = groups.Get(groupCheckBoxNode.BrowseName);
			if (group == null)
				return;

			bool userHasGroup = UserHasGroup(user, group.NodeId);

			if (groupCheckBoxNode.GetVariable("Checked").Value && !userHasGroup)
			{
				userNode.Refs.AddReference(FTOptix.Core.ReferenceTypes.HasGroup, group);
			}
			else if (!groupCheckBoxNode.GetVariable("Checked").Value && userHasGroup)
			{
				userNode.Refs.RemoveReference(FTOptix.Core.ReferenceTypes.HasGroup, group.NodeId, false);
			}
			if (groupCheckBoxNode.GetVariable("Checked").Value)
			{
				usergroupsselected = usergroupsselected + group.BrowseName + ",";
			}

		}
		string g1 = "Administrator";
             	string g2 = "Manager";
             	string g3 = "Engineer";
				string g4 = "Operator";
             if (usergroupsselected.Contains(g1))
				usergroupsselected = g1;
			 else if (usergroupsselected.Contains(g2))
				usergroupsselected = g2;
			 else if (usergroupsselected.Contains(g3))
				usergroupsselected = g3;
			 else if (usergroupsselected.Contains(g4))
				usergroupsselected = g4;
			 else
			 	usergroupsselected = "Unauthorized";
		LogIntoAudit("User Modified", "'" + user.BrowseName + "'" + " Group Chnaged To " + usergroupsselected, "UserGroupChangeEvent");
	}

	private bool UserHasGroup(IUANode user, NodeId groupNodeId)
	{
		if (user == null)
			return false;
		var userGroups = user.Refs.GetObjects(FTOptix.Core.ReferenceTypes.HasGroup, false);
		foreach (var userGroup in userGroups)
		{
			if (userGroup.NodeId == groupNodeId)
				return true;
		}
		return false;
	}

	private IUANode GetUsers()
	{
		var pathResolverResult = LogicObject.Context.ResolvePath(LogicObject, "{Users}");
		if (pathResolverResult == null)
			return null;
		if (pathResolverResult.ResolvedNode == null)
			return null;

		return pathResolverResult.ResolvedNode;
	}

	
	private void ShowMessage(int message)
	{
		var errorMessageVariable = LogicObject.GetVariable("ErrorMessage");
		if (errorMessageVariable != null)
			errorMessageVariable.Value = message;

		if (delayedTask != null)
			delayedTask?.Dispose();
		
		delayedTask = new DelayedTask(DelayedAction, 5000, LogicObject);
		delayedTask.Start();
	}

	private void DelayedAction(DelayedTask task)
	{
		if (task.IsCancellationRequested)
			return;

		var errorMessageVariable = LogicObject.GetVariable("ErrorMessage");
		if (errorMessageVariable != null)
        {
			errorMessageVariable.Value = 0;
		}
		delayedTask?.Dispose();
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

	private DelayedTask delayedTask;
}
