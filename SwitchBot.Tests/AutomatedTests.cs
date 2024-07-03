/////////////////////////////////////////////////////////////////////////////
// <copyright file="AutomatedTests.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////
using InTheHand.Bluetooth;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using SwitchBotBlueTooth;
using System;

namespace SwitchBot.Tests
{
	/// <summary>
	/// The tests class.
	/// </summary>
	public class AutomatedTests
	{
		/// <summary>
		/// Setups this instance.
		/// </summary>
		[SetUp]
		public void Setup()
		{
		}

		/// <summary>
		/// Sanities the check.
		/// </summary>
		[Test]
		public void SanityCheck()
		{
			Assert.Pass();
		}

		/// <summary>
		/// Gets the environment temperature characteristic asynchronous.
		/// </summary>
		/// <returns>A <see cref="Task"/> representing the
		/// asynchronous unit test.</returns>
		[Test]
		public async Task GetEnvironmentTemperatureCharacteristicAsync()
		{
			BluetoothUuid environment =
				GattServiceUuids.EnvironmentalSensing;

			IReadOnlyCollection<BluetoothDevice> devices =
				await BlueToothManager.Discover().ConfigureAwait(false);

			if (devices != null)
			{
				BluetoothDevice? woSensordevice = WoSensor.GetDevice(devices);

				if (woSensordevice != null)
				{
					RemoteGattServer gatt = woSensordevice.Gatt;
					GattService service =
						await gatt.GetPrimaryServiceAsync(environment).
							ConfigureAwait(false);

					if (service != null)
					{
						GattCharacteristic? characteristic =
							await WoSensor.GetTemperatureCharacteristic(
								service).ConfigureAwait(false);

						Assert.That(characteristic, Is.Null);
					}
					else
					{
						// Currently, always comes here.
						Assert.Pass();
					}
				}
			}
		}

		/// <summary>
		/// Gets the included services asynchronous.
		/// </summary>
		/// <returns>A <see cref="Task"/> representing the
		/// asynchronous unit test.</returns>
		[Test]
		public async Task GetIncludedServicesAsync()
		{
			IReadOnlyCollection<BluetoothDevice> devices =
				await BlueToothManager.Discover().ConfigureAwait(false);

			if (devices != null)
			{
				BluetoothDevice? woSensordevice = WoSensor.GetDevice(devices);

				if (woSensordevice != null)
				{
					RemoteGattServer gatt = woSensordevice.Gatt;

					List<GattService> services =
						await gatt.GetPrimaryServicesAsync(null).
							ConfigureAwait(false);

					foreach (GattService service in services)
					{
						IReadOnlyList<GattService> includedService =
							await service.GetIncludedServicesAsync().
							ConfigureAwait(false);

						Assert.That(includedService, Is.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Gets the primary temperature characteristic asynchronous.
		/// </summary>
		/// <returns>A <see cref="Task"/> representing the
		/// asynchronous unit test.</returns>
		[Test]
		public async Task GetPrimaryTemperatureCharacteristicAsync()
		{
			IReadOnlyCollection<BluetoothDevice> devices =
				await BlueToothManager.Discover().ConfigureAwait(false);

			if (devices != null)
			{
				BluetoothDevice? woSensordevice = WoSensor.GetDevice(devices);

				if (woSensordevice != null)
				{
					RemoteGattServer gatt = woSensordevice.Gatt;

					List<GattService> services =
						await gatt.GetPrimaryServicesAsync(null).
							ConfigureAwait(false);

					foreach (GattService service in services)
					{
						GattCharacteristic? characteristic =
							await WoSensor.GetTemperatureCharacteristic(
								service).ConfigureAwait(false);

						Assert.That(characteristic, Is.Null);
					}
				}
			}
		}
	}
}
