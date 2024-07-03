/////////////////////////////////////////////////////////////////////////////
// <copyright file="DeviceListRespone.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace SwitchBotLibrary
{
	/// <summary>
	/// The device list response.
	/// </summary>
	public class DeviceListRespone
	{
		/// <summary>
		/// Gets or sets the body.
		/// </summary>
		/// <value>The body.</value>
		public DeviceListResponseBody? Body { get; set; }

		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>The message.</value>
		public string? Message { get; set; }

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public int? StatusCode { get; set; }
	}
}
