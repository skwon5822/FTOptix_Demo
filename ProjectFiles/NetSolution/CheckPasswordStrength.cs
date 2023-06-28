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

public class CheckPasswordStrength : BaseNetLogic
{
    [ExportMethod]
    public void CheckPassword(NodeId PassTextBox, NodeId ApplyButton)
    {
        var Inpassword = InformationModel.Get<TextBox>(PassTextBox);
        var AppButton = InformationModel.Get<Button>(ApplyButton);
        string EnteredPassword = Inpassword.Text;

        int PassLength = 0;
        if (EnteredPassword.Length >= 8)
        {
            PassLength += 1;
        }
                
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

        if ((PassLength > 0) && (SplChrCheck > 0) && (upercasecheck > 0) && (lowercasecheck > 0) && (numcheck > 0))
        {
            AppButton.Enabled = true;
        }
        else
        {
            AppButton.Enabled = false;
            Log.Warning(LogicObject.BrowseName, "Entered Password is weak");
        }
    }
}
