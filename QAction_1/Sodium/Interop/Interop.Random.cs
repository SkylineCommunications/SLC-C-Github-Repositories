namespace Skyline.Protocol.Sodium
{
	using System;
	using System.Runtime.InteropServices;

	internal static partial class Interop
	{
		internal static partial class Libsodium
		{
			[DllImport(Libraries.Libsodium, CallingConvention = CallingConvention.Cdecl)]
			internal static extern void randombytes_buf(
				byte[] buf,
				uint size);

			[DllImport(Libraries.Libsodium, CallingConvention = CallingConvention.Cdecl)]
			internal static extern uint randombytes_uniform(
				uint upper_bound);
		}
	}
}
