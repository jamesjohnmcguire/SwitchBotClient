/////////////////////////////////////////////////////////////////////////////
// <copyright file="HttpManager.cs" company="Digital Zen Works">
// Copyright © 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////
using System.Net;
using System.Text;

namespace WebTools
{
	/// <summary>
	/// Vanilla API class.
	/// </summary>
	public class HttpManager
	{
		private readonly HttpClient client;

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpManager"/> class.
		/// </summary>
		/// <param name="client">The client.</param>
		public HttpManager(HttpClient client)
		{
			this.client = client;
		}

		/// <summary>
		/// Sends the data.
		/// </summary>
		/// <param name="uri">The URI to send to.</param>
		/// <param name="content">The content to send.</param>
		/// <param name="delay">The amount of seconds to delay.</param>
		/// <param name="retries">The number of times to retry.</param>
		/// <returns>The response status code.</returns>
		public async Task<HttpStatusCode> SendPostRequest(
			Uri uri, string content, int delay, int retries)
		{
			HttpStatusCode statusCode = HttpStatusCode.BadRequest;

			if (uri != null)
			{
				int tries = 0;
				using StringContent encodedContent =
					new (content, Encoding.UTF8, "application/json");

				do
				{
					if (tries > 0)
					{
						Console.WriteLine(
							"trying again... " + uri.AbsoluteUri);
					}

					using HttpResponseMessage response =
						await client.PostAsync(
							uri, encodedContent).ConfigureAwait(false);

					statusCode = response.StatusCode;
					tries++;

					int milliseconds = delay * 1000;
					await Task.Delay(milliseconds).ConfigureAwait(false);
				}
				while (statusCode != HttpStatusCode.OK && tries < retries);
			}

			return statusCode;
		}

		/// <summary>
		/// Sends the data.
		/// </summary>
		/// <param name="request">The message request.</param>
		/// <returns>The response.</returns>
		public async Task<HttpResponseMessage> SendRequest(
			HttpRequestMessage request)
		{
			HttpResponseMessage response = await client.SendAsync(request).
				ConfigureAwait(false);

			return response;
		}
	}
}
