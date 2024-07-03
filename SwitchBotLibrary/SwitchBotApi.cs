// <copyright file="SwitchBotApi.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SwitchBotLibrary
{
	/// <summary>
	/// SwitchBot API class.
	/// </summary>
	public class SwitchBotApi : IDisposable
	{
		private readonly HttpClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="SwitchBotApi"/> class.
		/// </summary>
		public SwitchBotApi()
		{
			client = new HttpClient();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SwitchBotApi"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public SwitchBotApi(HttpClient client)
		{
			this.client = client;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing,
		/// releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Gets the device data.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="token">The token.</param>
		/// <param name="secretKey">The secret key.</param>
		/// <returns>The device data.</returns>
		public async Task<DeviceData?> GetDeviceData(
			string? deviceId, string token, string secretKey)
		{
			DeviceData? deviceData = null;

			string response = await GetDeviceDataResponse(
				deviceId, token, secretKey).ConfigureAwait(false);

			JsonSerializerSettings jsonSerializerSettings =
				new ()
				{
					FloatParseHandling = FloatParseHandling.Double
				};

			DeviceDataResponse? deviceDataResponse =
				(DeviceDataResponse?)JsonConvert.DeserializeObject(
					response,
					typeof(DeviceDataResponse),
					jsonSerializerSettings);

			if (deviceDataResponse != null && deviceDataResponse.Body != null)
			{
				deviceData = deviceDataResponse.Body;
			}

			return deviceData;
		}

		/// <summary>
		/// Gets the list of devices.
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="secretKey">The secret key.</param>
		/// <returns>The list of devices.</returns>
		public async Task<IList<Device>?> GetDevices(
			string token, string secretKey)
		{
			IList<Device>? devices = null;

			string devicesResponse = await GetDeviceList(
				token, secretKey).ConfigureAwait(false);

			DeviceListRespone? deviceListRespone =
				(DeviceListRespone?)JsonConvert.DeserializeObject(
					devicesResponse, typeof(DeviceListRespone));

			if (deviceListRespone != null && deviceListRespone.Body != null &&
				deviceListRespone.Body.DeviceList != null)
			{
				devices = deviceListRespone.Body.DeviceList;
			}

			return devices;
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and
		/// unmanaged resources; <c>false</c> to release only unmanaged
		/// resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Dispose managed resources.
				client.Dispose();
			}
		}

		private static HttpRequestMessage GetRequest(
			string token, string secretKey, string url)
		{
			DateTime date1970 = new (1970, 1, 1);
			DateTime current = DateTime.Now;
			TimeSpan span = current - date1970;
			long time = Convert.ToInt64(span.TotalMilliseconds);

			string nonce = Guid.NewGuid().ToString();
			string data =
				token + time.ToString(CultureInfo.InvariantCulture) + nonce;

			Encoding utf8 = Encoding.UTF8;
			byte[] secretBytes = utf8.GetBytes(secretKey);
			using HMACSHA256 hmac = new (secretBytes);

			byte[] dataBytes = utf8.GetBytes(data);
			byte[] hmacHash = hmac.ComputeHash(dataBytes);
			string signature = Convert.ToBase64String(hmacHash);

			HttpRequestMessage request = new (HttpMethod.Get, url);

			request.Headers.TryAddWithoutValidation(@"Authorization", token);
			request.Headers.TryAddWithoutValidation(@"sign", signature);
			request.Headers.TryAddWithoutValidation(@"nonce", nonce);
			request.Headers.TryAddWithoutValidation(
				@"t", time.ToString(CultureInfo.InvariantCulture));

			return request;
		}

		/// <summary>
		/// Gets the device data.
		/// </summary>
		/// <param name="deviceId">The device identifier.</param>
		/// <param name="token">The token.</param>
		/// <param name="secretKey">The secret key.</param>
		/// <returns>The device data.</returns>
		private async Task<string> GetDeviceDataResponse(
			string? deviceId, string token, string secretKey)
		{
			string url =
				$"https://api.switch-bot.com/v1.1/devices/{deviceId}/status";

			using HttpRequestMessage request =
				GetRequest(token, secretKey, url);

			HttpResponseMessage response = await client.SendAsync(request).
				ConfigureAwait(false);
			string deviceData = await response.Content.ReadAsStringAsync().
				ConfigureAwait(false);

			return deviceData;
		}

		/// <summary>
		/// Gets the list of devices.
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="secretKey">The secret key.</param>
		/// <returns>The list of devices.</returns>
		private async Task<string> GetDeviceList(
			string token, string secretKey)
		{
			string url = "https://api.switch-bot.com/v1.1/devices";

			using HttpRequestMessage request =
				GetRequest(token, secretKey, url);

			HttpResponseMessage response = await client.SendAsync(request).
				ConfigureAwait(false);
			string deviceData = await response.Content.ReadAsStringAsync().
				ConfigureAwait(false);

			return deviceData;
		}
	}
}
