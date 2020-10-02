//-----------------------------------------------------------------------
// <copyright file="SystemUtils.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Text;
using System.Collections;
using System.Security.Permissions;
using Microsoft.Win32;
using System.IO;

[assembly: RegistryPermissionAttribute(SecurityAction.RequestMinimum,
ViewAndModify = "HKEY_CURRENT_USER")]

namespace TRUmigrate_1
{
	/// <summary>
	/// Summary description for SystemUtils.
	/// </summary>
	public class SystemUtils
	{
		public SystemUtils()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static string ArrayToString(string[] stringArray, string delimiter)
		{
			StringBuilder rval=new StringBuilder();

			for (int i=0;i<stringArray.Length;i++)
			{
				if (i>0)
					rval.Append(delimiter);
				rval.Append(stringArray[i]);
			}

			return rval.ToString();
		}

		public static string ArrayToString(ArrayList alArray, string delimiter)
		{
			StringBuilder rval=new StringBuilder();

			for (int i=0;i<alArray.Count;i++)
			{
				if (i>0)
					rval.Append(delimiter);
				rval.Append((string)alArray[i]);
			}

			return rval.ToString();
			
		}
		public static string[] StringToArray(string stringVal, string delimiter)
		{
			ArrayList vals= new ArrayList();
			//get delimiter position
			int iPos = stringVal.IndexOf(delimiter);
			//look for values separated by specified delimiter until delimiter is not found
			while (iPos>-1)
			{
				//get value before delimiter
				string val = stringVal.Substring(0,iPos);
				//add to array
				vals.Add(val);
				//get remainder of input string
				stringVal = stringVal.Substring(iPos+delimiter.Length);
				//get position of next delimiter
				iPos = stringVal.IndexOf(delimiter);
			}
			//get last or the only value
			vals.Add(stringVal);

			//convert into string[]
			string[] rval = new string[vals.Count];
			for (int i=0;i<vals.Count;i++)
			{
				rval[i] = (string)vals[i];
			}


			//string[] rval = stringVal.Split(delimiter.ToCharArray());
			//return string[] array
			return rval;
		}

		public static string ByteArrayToString(byte[] byteArray)
		{
			StringBuilder sbOutput = new StringBuilder(byteArray.Length);
			for (int i=0;i<byteArray.Length;i++)
			{
				sbOutput.Append(byteArray[i].ToString("X2"));
			}
			return sbOutput.ToString();
		}

		public static int GetUniqueNumber(ArrayList lngArray, int upperLimit)
		{
			int randomValue;

			System.Random rnd= new Random();
			//get random number within specified range
			randomValue = rnd.Next(upperLimit);
			//Check to see if the number already exists in the array.  If it exists,
			//generate a new random number.
			while (lngArray.Contains(randomValue))
			{
				randomValue = rnd.Next(upperLimit);
			}

			return randomValue;
		}

		public static string ReverseString(string inputString)
		{
			//get char array
			char[] inputChars = inputString.ToCharArray();
			//get its length
			int inputLength = inputChars.Length;
			//reverse array
			char[] reverseInput = new char[inputLength];
			for (int i=0;i<inputLength;i++)
			{
				reverseInput[i] = inputChars[inputLength-i-1];
			}

			//convert reversed back to string
			inputString = new string(reverseInput);
			return inputString;
			
		}

		public static bool RegistryKeyExists(string regKeyName)
		{
			//get local machine registry key
			RegistryKey hklm = Registry.CurrentUser;
			//search registry for given key
			RegistryKey regKey = hklm.OpenSubKey(regKeyName);
			if (regKey==null)
				return false;
			else
				return true;

		}

		public static void CreateRegistryKey(string regKeyName)
		{
			try
			{
				//get current user registry key
				RegistryKey hkcu = Registry.CurrentUser;

				//split subkeys
				string[] subkeys = regKeyName.Split("\\".ToCharArray());
				RegistryKey subkey = hkcu;
				
				for (int i=0;i<subkeys.Length;i++)
				{
					//create subkey
					string tst = "{" + subkeys[i] + "}";
					subkey = subkey.CreateSubKey(subkeys[i]);
				}
			}
			catch 
			{
			}
			
		}

		public static string GetRegistryKeyValue(string regKeyName, string regValueName)
		{
			//get current user registry key
			RegistryKey hkcu = Registry.CurrentUser;
			//search registry for given key
			RegistryKey regKey = hkcu.OpenSubKey(regKeyName);
			if (regKey!=null)
				return regKey.GetValue(regValueName).ToString();
			else
				return null;
		}

		public static void WriteRegistryKeyValue(string regKeyName, string regValueName, string regKeyValue)
		{
			//get current user registry key
			RegistryKey hkcu = Registry.CurrentUser;
			//search registry for given key
			RegistryKey regKey = hkcu.OpenSubKey(regKeyName, true);
			//write key value
			regKey.SetValue(regValueName, regKeyValue);
			
		}

		/// <summary>
		/// Loads DFC from the install location into assembly
		/// </summary>
		public static bool CheckDFC()
		{
			//search registry for dfc installation directory
			RegistryKey hklm = Registry.LocalMachine;
			RegistryKey softKey = hklm.OpenSubKey("SOFTWARE");
			RegistryKey dctmKey = softKey.OpenSubKey ("DOCUMENTUM");
			//exit if Documentum key not found
			if (dctmKey==null)
			{
				return false;
			}

			RegistryKey dfcKey = dctmKey.OpenSubKey("DFC Runtime Environment");
			//exit if DFC key is not found
			if (dfcKey==null)
			{
				return false;
			}

			RegistryKey dfcVersionKey=null;
			//look for version subkey
			string[] dfcsubkeys = dfcKey.GetSubKeyNames();
			for (int i=0;i<dfcsubkeys.Length;i++)
			{
				string keyname = dfcsubkeys[i];
				//Ak, 10/06/09, add DFC 6 to the list
                //AK, 11/20/15, Mantis #1033, add D7 check
                if (keyname.StartsWith("5") || keyname.StartsWith("4") || keyname.StartsWith("6") || keyname.StartsWith("7"))
				{
					dfcVersionKey = dfcKey.OpenSubKey(keyname);
					break;
				}
				
			}

			//exit if version key is not found or old version is found
			if (dfcVersionKey==null)
			{
				return false;
			}
			
			//get Documentum location
			string dctmpath = (string)dfcVersionKey.GetValue("InstallDir", "C:\\Program Files\\Documentum");
			dctmpath = dctmpath + "\\Shared";
			//exit if documentum dfc location folder is not found
			if (!Directory.Exists (dctmpath))
			{
				return false;
			}

			//find dfc dll
			string dfcdllpath = dctmpath + "\\Dfc.dll";
			//exit if dfc dll is not found
			if (!File.Exists(dfcdllpath))
			{
				return false;
			}


			return true;
		}



	}
}
