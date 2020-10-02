/*
 * MH Valiance Partners 1/23/09 - added this class
 */
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace MigrationUtils
{
	/// <summary>
	/// Summary description for CheckSum.
	/// </summary>
	public class CheckSum
	{
		private string _checksum = null;
		private byte[] _bytes = null;
		
		public CheckSum()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string TheCheckSum
		{
			get 
			{
				if (_checksum == null)
					ComputeCheckSum();

				return _checksum; 
			}
		}
	
		public static CheckSum GenerateCheckSumForFile(string fileName)
		{
			CheckSum cs = new CheckSum(fileName);
			cs.ComputeCheckSum();
			return cs;
		}
	
		public static String GenerateCheckSumForFile(byte[] fileBytes)
		{
			CheckSum cs = new CheckSum(fileBytes);
			cs.ComputeCheckSum();
			return cs.TheCheckSum;
		}

		
		
		public CheckSum(string fileName)
		{
			byte[] fileBytes = ReadAllBytes(fileName);
			this._bytes = fileBytes;
		}

		public CheckSum(byte[] bytes)
		{
			this._bytes = bytes;
		}

		/// <summary>
		/// Computes the Hash and makes it available
		/// </summary>
		private void ComputeCheckSum()
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(_bytes);
			// free the bytes
			_bytes = null;

			_checksum = System.BitConverter.ToString(result);
			_checksum = _checksum.Replace("-", "");
		}
        
		public override bool Equals(object obj)
		{
			if ((obj is CheckSum) && (_checksum != null))
			{
				CheckSum CompareTo = (CheckSum)obj;
				return this._checksum.Equals(CompareTo._checksum);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private byte[] ReadAllBytes(string path)
		{
			byte[] buffer;
			FileStream stream = null;
			try
			{
				stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
				
				int offset = 0;
				long length = stream.Length;
				if (length > 0x7fffffffL)
				{
					throw new IOException("IO.IO_FileTooLong2GB");
				}
				int count = (int) length;
				buffer = new byte[count];
				while (count > 0)
				{
					int num4 = stream.Read(buffer, offset, count);
					if (num4 == 0)
					{
						throw new IOException("unexpected end of file");
						//Error.EndOfFile();
					}
					offset += num4;
					count -= num4;
				}
			}
			finally
			{
				stream.Close();				
			}
			return buffer;
		}

	}
}
