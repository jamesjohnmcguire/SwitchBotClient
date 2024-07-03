/////////////////////////////////////////////////////////////////////////////
// <copyright file="DeviceData.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////
using Newtonsoft.Json;

namespace SwitchBotLibrary
{
	/// <summary>
	/// Represents an instance of device data.
	/// </summary>
	public class DeviceData : Device
	{
		/// <summary>
		/// Gets or sets the device battery amount.
		/// </summary>
		/// <value>The device battery amount.</value>
		public int Battery { get; set; }

		/// <summary>
		/// Gets or sets the device humidity amount.
		/// </summary>
		/// <value>The device humidity amount.</value>
		public int Humidity { get; set; }

		/// <summary>
		/// Gets or sets the device temperature reading.
		/// </summary>
		/// <value>The device temperature reading.</value>
		public double Temperature { get; set; }

		/// <summary>
		/// Gets or sets the device version.
		/// </summary>
		/// <value>The device version.</value>
		public string? Version { get; set; }
	}
}
