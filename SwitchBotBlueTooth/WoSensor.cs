// <copyright file="WoSensor.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////
using InTheHand.Bluetooth;

namespace SwitchBotBlueTooth
{
	/// <summary>
	/// Wo Sensor class.
	/// </summary>
	public static class WoSensor
	{
		/// <summary>
		/// Gets the device.
		/// </summary>
		/// <param name="devices">The devices.</param>
		/// <returns>The bluetooth device.</returns>
		public static BluetoothDevice? GetDevice(
			IReadOnlyCollection<BluetoothDevice> devices)
		{
			BluetoothDevice? woSensordevice = null;

			if (devices != null)
			{
				foreach (BluetoothDevice device in devices)
				{
					if (device.Name.Equals(
						"WoSensorTHP", StringComparison.Ordinal))
					{
						woSensordevice = device;
						break;
					}
				}
			}

			return woSensordevice;
		}

		/// <summary>
		/// Gets the temperature characteristic.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns>The temperature characteristic.</returns>
		public static async Task<GattCharacteristic?>
			GetTemperatureCharacteristic(
			GattService service)
		{
			GattCharacteristic? characteristic = null;

			if (service != null)
			{
				BluetoothUuid temperatureId =
					BluetoothUuid.GetCharacteristic("temperature");
				characteristic = await service.GetCharacteristicAsync(
					temperatureId).ConfigureAwait(false);
			}

			return characteristic;
		}
	}
}
