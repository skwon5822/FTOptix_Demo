#region Using directives
using System;
using System.Linq;
using FTOptix.HMIProject;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.NetLogic;
using FTOptix.UI;
using FTOptix.Store;
using FTOptix.RAEtherNetIP;
using FTOptix.CommunicationDriver;
using FTOptix.Modbus;
using FTOptix.Alarm;
using FTOptix.DataLogger;
using FTOptix.Recipe;
using FTOptix.Report;
using FTOptix.WebUI;
#endregion

public class CreateUserPanelLogic : BaseNetLogic
{
	string usergroupname = "";

	[ExportMethod]
    public void CreateUser(string username, string password, string locale, out NodeId result)
    {
		result = NodeId.Empty;

		if (string.IsNullOrEmpty(username))
		{
			ShowMessage(1);
			Log.Error("EditUserDetailPanelLogic", "Cannot create user with empty username");
			return;
		}

		result = GenerateUser(username, password, locale);
    }

    private NodeId GenerateUser(string username, string password, string locale)
    {
		var users = GetUsers();
		if (users == null)
		{
			ShowMessage(2);
			Log.Error("EditUserDetailPanelLogic", "Unable to get users");
			return NodeId.Empty;
		}

		foreach (var child in users.Children.OfType<FTOptix.Core.User>())
		{
			if (child.BrowseName.Equals(username, StringComparison.OrdinalIgnoreCase))
			{
				ShowMessage(10);
				Log.Error("EditUserDetailPanelLogic", "Username already exists");
				return NodeId.Empty;
			}
		}

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
		

		var user = InformationModel.MakeObject<FTOptix.Core.User>(username);
		users.Add(user);

		//Apply LocaleId
		if (!string.IsNullOrEmpty(locale))
			user.LocaleId = locale;

		//Apply groups
		ApplyGroups(user);

		//Apply password
		var result = Session.ChangePassword(username, password, string.Empty);

		switch (result.ResultCode)
		{
			case FTOptix.Core.ChangePasswordResultCode.Success:
				string g1 = "Administrator";
             	string g2 = "Manager";
             	string g3 = "Engineer";
				string g4 = "Operator";
             if (usergroupname.Contains(g1))
				usergroupname = g1;
			 else if (usergroupname.Contains(g2))
				usergroupname = g2;
			 else if (usergroupname.Contains(g3))
				usergroupname = g3;
			 else if (usergroupname.Contains(g4))
				usergroupname = g4;
			 else
			 	usergroupname = "Unauthorized";
				
				LogIntoAudit("New User Created", "'" + username + "'" + " with " + usergroupname + " Group", "UserCreationEvent");
				break;
			case FTOptix.Core.ChangePasswordResultCode.WrongOldPassword:
				//Not applicable
				break;
			case FTOptix.Core.ChangePasswordResultCode.PasswordAlreadyUsed:
				//Not applicable
				break;
			case FTOptix.Core.ChangePasswordResultCode.PasswordChangedTooRecently:
				//Not applicable
				break;
			case FTOptix.Core.ChangePasswordResultCode.PasswordTooShort:
				ShowMessage(6);
				users.Remove(user);
				return NodeId.Empty;
			case FTOptix.Core.ChangePasswordResultCode.UserNotFound:
				//Not applicable
				break;
			case FTOptix.Core.ChangePasswordResultCode.UnsupportedOperation:
				ShowMessage(8);
				users.Remove(user);
				return NodeId.Empty;

		}

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
				usergroupname = usergroupname + group.BrowseName + ",";
			}
		}
	}

	private bool UserHasGroup(IUANode user, NodeId groupNodeId)
	{
		if (user == null)
			return false;
		return user.Refs.GetObjects(FTOptix.Core.ReferenceTypes.HasGroup, false).Any(u => u.NodeId == groupNodeId);
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
