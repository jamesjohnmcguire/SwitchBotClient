/////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////
using Newtonsoft.Json;
using SwitchBotLibrary;
using System.Net;
using WebTools;

namespace SwitchBot
{
	internal sealed class Program
	{
		public static async Task Main(string[] args)
		{
			Console.WriteLine("SwitchBot Device Testing");

			if (args.Length < 2)
			{
				Console.WriteLine("Invalid Arguments");
			}
			else
			{
				string token = args[0];
				string secretKey = args[1];

				using HttpClient client = new ();

				using SwitchBotApi switchBot = new (client);

				IList<Device>? devices = await switchBot.GetDevices(
					token, secretKey).ConfigureAwait(false);

				if (devices == null)
				{
					Console.WriteLine(
						"Warning - No Devices Found");
				}
				else
				{
					for (int index = devices.Count - 1; index >= 0; index--)
					{
						Device? device = devices[index];
						string? deviceType = device.DeviceType;

						if (!deviceType!.Contains(
							"Hub Mini", StringComparison.OrdinalIgnoreCase))
						{
							string? deviceId = device.DeviceId;

							DeviceData? deviceData = await switchBot.GetDeviceData(
								deviceId, token, secretKey).ConfigureAwait(false);

							if (deviceData == null)
							{
								Console.WriteLine(
									"Warning - No Device Data Found for: {0}",
									deviceId);
							}
							else
							{
								deviceData.DeviceName = device.DeviceName;
								deviceData.HubDeviceId = device.HubDeviceId;

								string sendData =
									JsonConvert.SerializeObject(deviceData);

								Console.WriteLine("Device data: {0}", sendData);
							}

							Console.WriteLine();
						}
					}
				}
			}
		}
	}
}
