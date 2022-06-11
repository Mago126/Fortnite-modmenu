using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;

namespace ns0
{
	// Token: 0x02000005 RID: 5
	internal class Class_6
	{
		// Token: 0x06000004 RID: 4 RVA: 0x000021F0 File Offset: 0x000003F0
		static Class_6()
		{
			Class_6.Field_5 = int.MaxValue;
			Class_6.Field_6 = int.MinValue;
			Class_6.Field_7 = new MemoryStream(0);
			Class_6.Field_8 = new MemoryStream(0);
			Class_6.Field_4 = new object();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002240 File Offset: 0x00000440
		private static string Method_8(Assembly assembly_0)
		{
			string text = assembly_0.FullName;
			int num = text.IndexOf(',');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000226C File Offset: 0x0000046C
		private static byte[] Method_9(Assembly assembly_0)
		{
			try
			{
				string fullName = assembly_0.FullName;
				int num = fullName.IndexOf("PublicKeyToken=");
				if (num < 0)
				{
					num = fullName.IndexOf("publickeytoken=");
				}
				if (num < 0)
				{
					return null;
				}
				num += 15;
				if (fullName[num] != 'n')
				{
					if (fullName[num] != 'N')
					{
						string s = fullName.Substring(num, 16);
						long value = long.Parse(s, NumberStyles.HexNumber);
						byte[] bytes = BitConverter.GetBytes(value);
						Array.Reverse(bytes);
						return bytes;
					}
				}
				return null;
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000230C File Offset: 0x0000050C
		internal static byte[] Method_10(Stream stream_0)
		{
			byte[] result;
			lock (Class_6.Field_4)
			{
				result = Class_6.Method_12(97L, stream_0);
			}
			return result;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002350 File Offset: 0x00000550
		internal static byte[] Method_11(long long_0, Stream stream_0)
		{
			byte[] result;
			try
			{
				result = Class_6.Method_10(stream_0);
			}
			catch
			{
				result = Class_6.Method_12(97L, stream_0);
			}
			return result;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000238C File Offset: 0x0000058C
		internal static byte[] Method_12(long long_0, object object_0)
		{
			Stream stream = object_0 as Stream;
			Stream stream2 = stream;
			MemoryStream memoryStream = null;
			for (int i = 1; i < 4; i++)
			{
				stream.ReadByte();
			}
			ushort num = (ushort)stream.ReadByte();
			num = ~num;
			if ((num & 2) != 0)
			{
				DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
				byte[] array = new byte[8];
				stream.Read(array, 0, 8);
				descryptoServiceProvider.IV = array;
				byte[] array2 = new byte[8];
				stream.Read(array2, 0, 8);
				bool flag = true;
				byte[] array3 = array2;
				for (int j = 0; j < array3.Length; j++)
				{
					if (array3[j] != 0)
					{
						flag = false;
						IL_96:
						if (flag)
						{
							array2 = Class_6.Method_9(Assembly.GetExecutingAssembly());
						}
						descryptoServiceProvider.Key = array2;
						if (Class_6.Field_7 == null)
						{
							if (Class_6.Field_5 == 2147483647)
							{
								Class_6.Field_7.Capacity = (int)stream.Length;
							}
							else
							{
								Class_6.Field_7.Capacity = Class_6.Field_5;
							}
						}
						Class_6.Field_7.Position = 0L;
						ICryptoTransform cryptoTransform = descryptoServiceProvider.CreateDecryptor();
						int inputBlockSize = cryptoTransform.InputBlockSize;
						int outputBlockSize = cryptoTransform.OutputBlockSize;
						byte[] array4 = new byte[cryptoTransform.OutputBlockSize];
						byte[] array5 = new byte[cryptoTransform.InputBlockSize];
						int num2 = (int)stream.Position;
						while ((long)(num2 + inputBlockSize) < stream.Length)
						{
							stream.Read(array5, 0, inputBlockSize);
							int count = cryptoTransform.TransformBlock(array5, 0, inputBlockSize, array4, 0);
							Class_6.Field_7.Write(array4, 0, count);
							num2 += inputBlockSize;
						}
						stream.Read(array5, 0, (int)(stream.Length - (long)num2));
						byte[] array6 = cryptoTransform.TransformFinalBlock(array5, 0, (int)(stream.Length - (long)num2));
						Class_6.Field_7.Write(array6, 0, array6.Length);
						stream2 = Class_6.Field_7;
						stream2.Position = 0L;
						memoryStream = Class_6.Field_7;
						goto IL_1D1;
					}
				}
				goto IL_96;
			}
			IL_1D1:
			if ((num & 8) != 0)
			{
				if (Class_6.Field_8 == null)
				{
					if (Class_6.Field_6 == -2147483648)
					{
						Class_6.Field_8.Capacity = (int)stream2.Length * 2;
					}
					else
					{
						Class_6.Field_8.Capacity = Class_6.Field_6;
					}
				}
				Class_6.Field_8.Position = 0L;
				DeflateStream deflateStream = new DeflateStream(stream2, CompressionMode.Decompress);
				int num3 = 1000;
				byte[] buffer = new byte[1000];
				int num4;
				do
				{
					num4 = deflateStream.Read(buffer, 0, num3);
					if (num4 > 0)
					{
						Class_6.Field_8.Write(buffer, 0, num4);
					}
				}
				while (num4 >= num3);
				memoryStream = Class_6.Field_8;
			}
			if (memoryStream != null)
			{
				return memoryStream.ToArray();
			}
			byte[] array7 = new byte[stream.Length - stream.Position];
			stream.Read(array7, 0, array7.Length);
			return array7;
		}

		// Token: 0x04000002 RID: 2
		private static readonly object Field_4;

		// Token: 0x04000003 RID: 3
		private static readonly int Field_5;

		// Token: 0x04000004 RID: 4
		private static readonly int Field_6;

		// Token: 0x04000005 RID: 5
		private static readonly MemoryStream Field_7 = null;

		// Token: 0x04000006 RID: 6
		private static readonly MemoryStream Field_8 = null;

		// Token: 0x04000007 RID: 7
		private static readonly byte Field_9;
	}
}
