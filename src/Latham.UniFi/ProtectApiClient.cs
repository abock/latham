//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Latham.UniFi.Models;

namespace Latham.UniFi
{
    public class ProtectApiClient
    {
        readonly HttpClient httpClient;
        string? username;
        string? password;
        string? bearerToken;
        int authenticationAttempts;

        public Protect? Protect { get; private set; }

        public ProtectApiClient(Uri endpointUri, bool verifyCertificate = false)
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = true
            };

            if (!string.IsNullOrEmpty(endpointUri.UserInfo))
            {
                var userpass = endpointUri.UserInfo.Split(new [] { ':' }, 2);
                if (userpass.Length != 2)
                    throw new ArgumentException(
                        $"The endpoint URI '{endpointUri}' does not contain a valid username:password. " +
                        $"The endpoint URI should be specified as {endpointUri.Scheme}://<user>:<pass>@" +
                        $"{endpointUri.Host}:{endpointUri.Port}",
                        nameof(endpointUri));

                username = userpass[0];
                password = userpass[1];
            }

            if (!verifyCertificate)
                handler.ServerCertificateCustomValidationCallback = delegate { return true; };

            httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri($"{endpointUri.Scheme}://{endpointUri.Host}:{endpointUri.Port}")
            };

            httpClient.DefaultRequestHeaders.Connection.Add("Keep-Alive");
        }

        Task<HttpResponseMessage> GetAsync(
            string requestUri,
            CancellationToken cancellationToken)
            => SendAsync(
                HttpMethod.Get,
                requestUri,
                null,
                cancellationToken);

        Task<HttpResponseMessage> PostAsync(
            string requestUri,
            CancellationToken cancellationToken)
            => SendAsync(
                HttpMethod.Post,
                requestUri,
                null,
                cancellationToken);

        Task<HttpResponseMessage> PostJsonAsync<T>(
            string requestUri,
            T value,
            CancellationToken cancellationToken)
            => SendAsync(
                HttpMethod.Post,
                requestUri,
                new StringContent(
                    JsonConvert.SerializeObject(value),
                    Encoding.UTF8,
                    "application/json"),
                cancellationToken);

        async Task<HttpResponseMessage> SendAsync(
            HttpMethod method,
            string requestUri,
            HttpContent? content,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(method, requestUri);

            if (content is object)
                request.Content = content;

            if (!string.IsNullOrEmpty(bearerToken))
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    bearerToken);

            var response = await httpClient
                .SendAsync(request)
                .ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (username is string &&
                    password is string &&
                    authenticationAttempts == 0)
                {
                    await AuthenticateAsync(
                        username,
                        password,
                        cancellationToken).ConfigureAwait(false);

                    return await SendAsync(
                        method,
                        requestUri,
                        content,
                        cancellationToken).ConfigureAwait(false);
                }

                response.EnsureSuccessStatusCode();
            }

            return response;
        }

        public Task AuthenticateAsync(
            CancellationToken cancellationToken = default)
        {
            if (username is null || password is null)
                throw new InvalidOperationException(
                    $"Username and password must be specified in either the endpoint URI " +
                    $"passed to the constructor or use the {nameof(AuthenticateAsync)} override " +
                    "that accepts explict username and password arguments.");

            return AuthenticateAsync(username, password, cancellationToken);
        }

        public async Task AuthenticateAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            this.username = username
                ?? throw new ArgumentNullException(nameof(username));
            this.password = password
                ?? throw new ArgumentNullException(nameof(password));

            authenticationAttempts++;
            bearerToken = null;

            var response = await PostJsonAsync(
                "/api/auth",
                new
                {
                    username,
                    password
                },
                cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception($"Invalid username/password for endpoint '{httpClient.BaseAddress}'");
            }

            response.EnsureSuccessStatusCode();

            if (response.Headers.TryGetValues("Authorization", out var authorizationHeaders))
            {
                foreach (var header in authorizationHeaders)
                {
                    if (!string.IsNullOrEmpty(header))
                    {
                        authenticationAttempts = 0;
                        bearerToken = header;
                        return;
                    }
                }
            }

            throw new Exception("Received 200 OK response but no Authorization header containing bearer token");
        }

        public Task<Protect?> BootstrapAsync(
            CancellationToken cancellationToken = default)
            => BootstrapAsync(useCached: true, cancellationToken);

        public async Task<Protect?> BootstrapAsync(
            bool useCached,
            CancellationToken cancellationToken = default)
        {
            if (useCached&& Protect is object)
                return Protect;

            var response = await GetAsync(
                "/api/bootstrap",
                cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using var stream = await response
                .Content
                .ReadAsStreamAsync()
                .ConfigureAwait(false);

            var streamReader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(streamReader);
            var json = await JToken
                .LoadAsync(jsonReader, cancellationToken)
                .ConfigureAwait(false);

            Protect = json.ToObject<Protect>(UniFiJsonSerializerSettings.CreateSerializer());
            return Protect;
        }

        public Task DownloadVideoAsync(
            Camera camera,
            Channel channel,
            DateTimeOffset startTime,
            TimeSpan duration,
            string outputPath,
            CancellationToken cancellationToken = default)
            => DownloadVideoAsync(
                camera,
                channel,
                startTime,
                startTime + duration,
                outputPath,
                cancellationToken);

        public async Task DownloadVideoAsync(
            Camera camera,
            Channel channel,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            string outputPath,
            CancellationToken cancellationToken = default)
        {
            if (camera is null)
                throw new ArgumentNullException(nameof(camera));

            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            if (outputPath is null)
                throw new ArgumentNullException(nameof(outputPath));

            if (startTime <= DateTimeOffset.MinValue || startTime >= DateTimeOffset.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(startTime));

            if (endTime <= DateTimeOffset.MinValue || endTime >= DateTimeOffset.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(endTime));

            if (endTime <= startTime)
                throw new ArgumentOutOfRangeException(nameof(endTime));

            var requestUri = $"/api/video/export?" +
                $"camera={camera.Id}&" +
                $"channel={channel.Id}&" +
                $"start={startTime.ToUnixTimeMilliseconds()}&" +
                $"end={endTime.ToUnixTimeMilliseconds()}&" +
                $"format=FMP4";

            var response = await GetAsync(
                requestUri,
                cancellationToken).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using var httpStream = await response
                .Content
                .ReadAsStreamAsync()
                .ConfigureAwait(false);

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            using var outputStream = File.OpenWrite(outputPath);

            var buffer = new byte[64 * 1024 * 1024];

            while (true)
            {
                var read = await httpStream
                    .ReadAsync(buffer, 0, buffer.Length)
                    .ConfigureAwait(false);

                if (read <= 0)
                    break;

                await outputStream
                    .WriteAsync(buffer, 0, read)
                    .ConfigureAwait(false);
            }
        }
    }
}
