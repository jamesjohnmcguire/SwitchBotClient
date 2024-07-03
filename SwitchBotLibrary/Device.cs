/////////////////////////////////////////////////////////////////////////////
// <copyright file="Device.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace SwitchBotLibrary
{
	/// <summary>
	/// Represents a device.
	/// </summary>
	public class Device
	{
		/// <summary>
		/// Gets or sets the device identifier.
		/// </summary>
		/// <value>The device identifier.</value>
		public string? DeviceId { get; set; }

		/// <summary>
		/// Gets or sets the name of the device.
		/// </summary>
		/// <value>The name of the device.</value>
		public string? DeviceName { get; set; }

		/// <summary>
		/// Gets or sets the type of the device.
		/// </summary>
		/// <value>The type of the device.</value>
		public string? DeviceType { get; set; }

		/// <summary>
		/// Gets or sets the hub device identifier.
		/// </summary>
		/// <value> The hub device identifier.</value>
		public string? HubDeviceId { get; set; }
	}
}
