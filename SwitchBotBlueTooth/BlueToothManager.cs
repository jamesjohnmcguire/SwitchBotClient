/////////////////////////////////////////////////////////////////////////////
// <copyright file="BlueToothManager.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////
#if USE_BLUETOOTH_CLASSIC
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
#endif
using InTheHand.Bluetooth;
using System;
using System.Reflection.PortableExecutable;

namespace SwitchBotBlueTooth
{
	/// <summary>
	/// Bluetooth class.
	/// </summary>
	public class BlueToothManager
	{
#pragma warning disable SA1011 // ClosingSquareBracketsMustBeSpacedCorrectly
		private byte[]? eventValues;
#pragma warning restore SA1011 // ClosingSquareBracketsMustBeSpacedCorrectly

#if USE_BLUETOOTH_CLASSIC
		public static IReadOnlyCollection<BluetoothDeviceInfo> Discover()
		{
			BluetoothClient client = new BluetoothClient();
			IReadOnlyCollection<BluetoothDeviceInfo> devices = client.DiscoverDevices();

			return devices;
		}
#else
		/// <summary>
		/// Discovers this instance.
		/// </summary>
		/// <returns>A list of discovered devices.</returns>
		public static async Task<IReadOnlyCollection<BluetoothDevice>>
			Discover()
		{
			IReadOnlyCollection<BluetoothDevice> discoveredDevices =
				await Bluetooth.ScanForDevicesAsync().ConfigureAwait(false);

			return discoveredDevices;
		}
#endif

		/// <summary>
		/// Get characteristics.
		/// </summary>
		/// <param name="service">The service to use.</param>
		/// <returns>The list of characteristics.</returns>
		public static async Task<IReadOnlyList<GattCharacteristic>?>
			GetCharacteristics(
				GattService service)
		{
			IReadOnlyList<GattCharacteristic>? characteristics = null;

			if (service != null)
			{
				Console.WriteLine("Checking: " + service.Uuid);

				characteristics =
					await service.GetCharacteristicsAsync().
						ConfigureAwait(false);
			}

			return characteristics;
		}

		private static async Task<GattCharacteristic?>
		GetTemperatureCharacteristic(
			GattService service)
		{
			BluetoothUuid temperatureUuid =
				BluetoothUuid.GetCharacteristic("temperature");

			GattCharacteristic? characteristic =
				await service.GetCharacteristicAsync(
					temperatureUuid).ConfigureAwait(false);

			return characteristic;
		}

		/// <summary>
		/// Get temperature characteristic.
		/// </summary>
		/// <param name="service">The service to use.</param>
		/// <returns>A <see cref="Task{TResult}"/> representing
		/// the result of the asynchronous operation.</returns>
		public static async Task<byte[]?> GetTemperatureCharacteristicValue(
			GattService? service)
		{
			byte[]? characteristicValues = null;

			if (service != null)
			{
				BluetoothUuid temperatureUuid =
					BluetoothUuid.GetCharacteristic("temperature");

				GattCharacteristic characteristic =
					await service.GetCharacteristicAsync(
						temperatureUuid).ConfigureAwait(false);

				if (characteristic != null)
				{
					characteristicValues = characteristic.Value;
				}
			}

			return characteristicValues;
		}

		/// <summary>
		/// Sets the notifications.
		/// </summary>
		/// <param name="characteristic">The characteristic.</param>
		/// <returns>A value indicating whether the notifications was set
		/// or not.</returns>
		public async Task<bool> SetNotifications(
			GattCharacteristic characteristic)
		{
			bool notificationSet = false;

			if (characteristic != null)
			{
				try
				{
					characteristic.CharacteristicValueChanged +=
						CharacteristicValueChanged;
					await characteristic.StartNotificationsAsync().
						ConfigureAwait(false);

					notificationSet = true;
				}
				catch (NotSupportedException)
				{
					Console.WriteLine("NotSupportedException");
				}
			}

			return notificationSet;
		}

		private static string? GetCharacteristicTextValue(
			GattCharacteristic? characteristic)
		{
			string? textValue = null;

			byte[]? characteristicValue = GetCharacteristicValue(
				characteristic);

			if (characteristicValue != null)
			{
				textValue =
					System.Text.Encoding.UTF8.GetString(
						characteristicValue, 0, characteristicValue.Length);
			}

			return textValue;
		}

		private static byte[]? GetCharacteristicValue(
			GattCharacteristic? characteristic)
		{
			byte[]? characteristicValue = null;

			if (characteristic != null)
			{
				characteristicValue = characteristic.Value;

			}

			return characteristicValue;
		}

		private static async Task<byte[]?> ReadCharacteristicValue(
			GattCharacteristic? characteristic)
		{
			byte[]? characteristicValue = null;

			if (characteristic != null)
			{
				characteristicValue =
					await characteristic.ReadValueAsync().
						ConfigureAwait(false);
			}

			return characteristicValue;
		}

		private void CharacteristicValueChanged(
			object? sender, GattCharacteristicValueChangedEventArgs? eventData)
		{
			if (eventData != null)
			{
				eventValues = eventData.Value;

				if (eventValues != null)
				{
					byte eventValue = eventValues[0];

					Console.WriteLine("Changed: " + eventValue);
				}
			}
		}
	}
}
