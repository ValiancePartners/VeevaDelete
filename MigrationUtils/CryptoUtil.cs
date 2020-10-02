using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace TRUmigrate_1
{
	/// <summary>
	/// Summary description for CryptoUtil.
	/// </summary>
	public class CryptoUtil
	{
		public CryptoUtil()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Decrypts given string
		/// </summary>
		/// <param name="inputText">Encrypted value</param>
		/// <param name="saltstring">Saltstring used by encryption algorithm</param>
		/// <returns>Decrypted string</returns>
		public static string DecryptString(string inputText, string saltstring)

		{
			string decryptedData="";
			try
			{
				RijndaelManaged  rijndaelCipher = new RijndaelManaged();

				byte[] encryptedData = Convert.FromBase64String(inputText);
				byte[] salt = Encoding.ASCII.GetBytes(saltstring.Length.ToString());


				PasswordDeriveBytes secretKey = new PasswordDeriveBytes(saltstring, salt);


				// Create a decryptor from the existing SecretKey bytes.
				ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
 

				MemoryStream  memoryStream = new MemoryStream(encryptedData);


				// Create a CryptoStream. (always use Read mode for decryption).
				CryptoStream  cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);


				// Since at this point we don't know what the size of decrypted data
				// will be, allocate the buffer long enough to hold EncryptedData;
				// DecryptedData is never longer than EncryptedData.
				byte[] plainText = new byte[encryptedData.Length];


				// Start decrypting.
				int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);

                
				memoryStream.Close();
				cryptoStream.Close();

				// Convert decrypted data into a string. 
				decryptedData = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
			}
			catch(Exception e)
			{
				//thhow exception
				throw e;
			}
			

			// Return decrypted string.   
			return decryptedData;

		}


		/// <summary>
		/// Computes file checksum using MD5 encryption
		/// </summary>
		/// <param name="fileLocation">Local file path</param>
		/// <returns>File checksum converted to string</returns>
		public string ComputeFileChecksum(string fileLocation)
		{
			
			//get file stream of an input file
			FileStream fsIn = new FileStream(fileLocation, FileMode.Open, FileAccess.Read); 
			byte[] filebyte = new byte[fsIn.Length];

			fsIn.Read(filebyte,0,Convert.ToInt32(fsIn.Length));

			//store file stream in a byte array
			//for (int i=0;i<fsIn.Length;i++)
			//{
			//	filebyte[i]=(byte)fsIn.ReadByte();
			//}

			//compute checksum
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(filebyte);

			//return checksum converted to string
			//AK, 1/26/08, BUGAWARE #258
			//use conversion without formatting
			StringBuilder sbOutput = new StringBuilder(result.Length);
			for (int i=0;i<result.Length;i++)
			{
                //HK, 11/19/14, Mantis #0001027, convert results to hexadecimal string instead of a string to match tc
                sbOutput.Append(result[i].ToString("x2"));
                //sbOutput.Append(result[i].ToString());
                //end changes by HK on 11/19/14
			}
			return sbOutput.ToString();
			//return SystemUtils.ByteArrayToString(result);
			//end changes by AK on 1/26/08
		}



	}
}
