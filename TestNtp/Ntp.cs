using System;
using System.Threading.Tasks;
using Sockets.Plugin;
using System.Diagnostics;
using System.Threading;

namespace TestNtp
{
	public static class NtpClient
	{
		public static async Task<DateTime> GetNetworkTimeAsync()
		{

			Debug.WriteLine ("Entro in GetNTP " + DateTime.Now);
			//default Windows time server
			const string ntpServer = "time.windows.com";
			//const string ntpServer = "pool.ntp.org";
			//const string ntpServer = "time.nist.gov";

			// NTP message size - 16 bytes of the digest (RFC 2030)
			var ntpData = new byte[48];

			//Setting the Leap Indicator, Version Number and Mode values
			ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

			var tcs = new TaskCompletionSource<byte[]>();

			using (var socket = new UdpSocketReceiver())
			{
				socket.MessageReceived += async (sender, args) =>
				{
					await socket.StopListeningAsync();
					tcs.SetResult(args.ByteData);
				};

				Debug.WriteLine ("StartListening " + DateTime.Now);

				await socket.StartListeningAsync(); // any free port >1000 will do 

				Debug.WriteLine ("SendTo " + DateTime.Now);
				await socket.SendToAsync (ntpData, ntpServer, 123).ContinueWith(_=> Task.FromResult(true)).TimeoutAfter(TimeSpan.FromSeconds(3)); //.TimeoutAfter(TimeSpan.FromSeconds(3));
				Debug.WriteLine ("SendTo conclusa");

				ntpData = await tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(3));
			}

			//Offset to get to the "Transmit Timestamp" field (time at which the reply 
			//departed the server for the client, in 64-bit timestamp format."
			const byte serverReplyTime = 40;

			//Get the seconds part
			ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

			//Get the seconds fraction
			ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

			//Convert From big-endian to little-endian
			intPart = SwapEndianness(intPart);
			fractPart = SwapEndianness(fractPart);

			var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

			//**UTC** time
			var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

			Debug.WriteLine ("Esco da GetNTP " + DateTime.Now + " - " + networkDateTime + " - LOCALTIME: " + networkDateTime.ToLocalTime() + " - UTC: " + networkDateTime.ToUniversalTime());
			return networkDateTime.ToLocalTime();
		}

		// stackoverflow.com/a/3294698/162671
		static uint SwapEndianness(ulong x)
		{
			return (uint)(((x & 0x000000ff) << 24) +
				((x & 0x0000ff00) << 8) +
				((x & 0x00ff0000) >> 8) +
				((x & 0xff000000) >> 24));
		}


	}

	public static class AsyncExtensions
	{
		public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout, CancellationTokenSource cancellationTokenSource = null)
		{
			if (task == await Task.WhenAny(task, Task.Delay(timeout)))
				return await task;
			else
			{
				if (cancellationTokenSource != null)
					cancellationTokenSource.Cancel();

				throw new TimeoutException();
			}
		}
	}
}

