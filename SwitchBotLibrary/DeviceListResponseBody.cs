/////////////////////////////////////////////////////////////////////////////
// <copyright file="DeviceListResponseBody.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace SwitchBotLibrary
{
	/// <summary>
	/// The device list response body.
	/// </summary>
	public class DeviceListResponseBody
	{
		/// <summary>
		/// Gets or sets the device list.
		/// </summary>
		/// <value>The device list.</value>
#pragma warning disable CA2227
		public IList<Device>? DeviceList { get; set; }
#pragma warning restore CA2227
	}
}
