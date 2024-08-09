/////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////
using InTheHand.Bluetooth;
using SwitchBotLibrary;

namespace SwitchBot
{
	internal sealed class Program
	{
		public Program()
		{
		}

		public static async Task Main(string[] args)
		{
			Console.WriteLine("SwitchBot Device Testing");

			Program program = new ();

			await program.Run().ConfigureAwait(false);
		}

		private void CharacteristicValueChanged(
			object? sender, GattCharacteristicValueChangedEventArgs? eventData)
		{
			if (eventData != null)
			{
#pragma warning disable SA1011 // ClosingSquareBracketsMustBeSpacedCorrectly
				byte[]? eventValues = eventData.Value;
#pragma warning restore SA1011 // ClosingSquareBracketsMustBeSpacedCorrectly

				if (eventValues != null)
				{
					byte eventValue = eventValues[0];

					Console.WriteLine("Changed: " + eventValue);
				}
			}
		}

		private async Task PrintCharacteristics(
			IReadOnlyList<GattCharacteristic> characteristics)
		{
			foreach (GattCharacteristic characteristic2 in characteristics)
			{
				string something = string.Empty;
				byte[] some = characteristic2.Value;

				if (some != null)
				{
					something =
						System.Text.Encoding.UTF8.GetString(
							some, 0, some.Length);
				}
				else
				{
					something = "(null)";
				}

				Console.WriteLine("Checking Characteristic :" +
					characteristic2.Uuid + " Value: " + something);

				some = await characteristic2.ReadValueAsync().
					ConfigureAwait(false);

				if (some != null)
				{
					something =
						System.Text.Encoding.UTF8.GetString(
							some, 0, some.Length);
				}
				else
				{
					something = "(null)";
				}

				Console.WriteLine("Checking Async Characteristic :" +
					characteristic2.Uuid + " Value: " + something);

				try
				{
					characteristic2.CharacteristicValueChanged +=
						CharacteristicValueChanged;
					await characteristic2.StartNotificationsAsync().
						ConfigureAwait(false);
				}
				catch (NotSupportedException)
				{
					Console.WriteLine("NotSupportedException");
				}
			}
		}

		private async Task Run()
		{
			IReadOnlyCollection<BluetoothDevice> devices =
				await BlueToothManager.Discover().ConfigureAwait(false);

			if (devices != null)
			{
				Console.WriteLine($"found {devices.Count} devices");

				foreach (BluetoothDevice device in devices)
				{
					Console.WriteLine(
						"Device Id: " + device.Id + " Name: " + device.Name);
				}

				BluetoothDevice? woSensordevice = WoSensor.GetDevice(devices);
				await TestDevice(woSensordevice).ConfigureAwait(false);
			}
		}

		private async Task TestDevice(BluetoothDevice? device)
		{
			if (device != null)
			{
				BluetoothUuid environment =
					GattServiceUuids.EnvironmentalSensing;

				RemoteGattServer gatt = device.Gatt;

				List<GattService> services =
					await gatt.GetPrimaryServicesAsync(null).
						ConfigureAwait(false);

				foreach (GattService service in services)
				{
					await TestService(service).ConfigureAwait(false);
				}

				GattService environmentService =
					await gatt.GetPrimaryServiceAsync(environment).
						ConfigureAwait(false);

				if (environmentService != null)
				{
					await TestService(environmentService).
						ConfigureAwait(false);
				}
			}
		}

		private async Task TestService(GattService service)
		{
			if (service != null)
			{
				Console.WriteLine("Checking: " + service.Uuid);

				BluetoothUuid temperatureUuid =
					BluetoothUuid.GetCharacteristic("temperature");

				GattCharacteristic characteristic =
					await service.GetCharacteristicAsync(
						temperatureUuid).ConfigureAwait(false);

				if (characteristic != null)
				{
					byte[] characteristicValues = characteristic.Value;
				}
				else
				{
					Console.WriteLine(
						"No temperature characteristic for this service");
				}

				IReadOnlyList<GattCharacteristic> characteristics =
					await service.GetCharacteristicsAsync().
					ConfigureAwait(false);

				await PrintCharacteristics(characteristics).
					ConfigureAwait(false);

				IReadOnlyList<GattService> includedServices =
					await service.GetIncludedServicesAsync().ConfigureAwait(false);

				if (includedServices == null)
				{
					Console.WriteLine(
						"No included services for this service");
				}

				characteristic = await service.GetCharacteristicAsync(
					temperatureUuid).ConfigureAwait(false);

				if (characteristic == null)
				{
					Console.WriteLine(
						"No temperature characteristic found for this service");
				}
			}
		}
	}
}
