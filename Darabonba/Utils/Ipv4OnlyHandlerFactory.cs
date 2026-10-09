#if !NET45
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Darabonba.Utils
{
    /// <summary>
    /// Builds an HttpMessageHandler whose outgoing TCP connections only use IPv4 (AddressFamily.InterNetwork).
    /// The library targets netstandard, so SocketsHttpHandler.ConnectCallback (.NET 5+) is bound at runtime
    /// through reflection. When it is unavailable the factory fails closed instead of falling back to dual-stack.
    /// </summary>
    internal static class Ipv4OnlyHandlerFactory
    {
        internal const string NotSupportedMessage =
            "ipv4Only requires .NET 5 or later (SocketsHttpHandler.ConnectCallback). IPv4-only connections cannot be enforced on the current runtime, so the request is refused instead of falling back to dual-stack.";

        private static readonly Func<string, CancellationToken, Task<IPAddress[]>> resolveIpv4Addresses = CreateIpv4Resolver();

        internal static Type ResolveSocketsHttpHandlerType()
        {
            return typeof(HttpClientHandler).Assembly.GetType("System.Net.Http.SocketsHttpHandler", false);
        }

        internal static HttpMessageHandler Create(string proxyUrl, bool ignoreSSL, string ca)
        {
            return Create(ResolveSocketsHttpHandlerType(), proxyUrl, ignoreSSL, ca);
        }

        internal static HttpMessageHandler Create(Type handlerType, string proxyUrl, bool ignoreSSL, string ca)
        {
            PropertyInfo connectCallback = handlerType == null ? null : handlerType.GetProperty("ConnectCallback");
            if (connectCallback == null || !typeof(HttpMessageHandler).IsAssignableFrom(handlerType))
            {
                throw new NotSupportedException(NotSupportedMessage);
            }

            Delegate callback = BuildConnectCallback(connectCallback.PropertyType);
            var handler = (HttpMessageHandler)Activator.CreateInstance(handlerType);
            connectCallback.SetValue(handler, callback, null);

            if (!string.IsNullOrWhiteSpace(proxyUrl))
            {
                handlerType.GetProperty("Proxy").SetValue(handler, new WebProxy(new Uri(proxyUrl)), null);
            }
            else
            {
                handlerType.GetProperty("UseProxy").SetValue(handler, false, null);
            }

            RemoteCertificateValidationCallback validation = BuildCertificateValidation(ignoreSSL, ca);
            if (validation != null)
            {
                object sslOptions = handlerType.GetProperty("SslOptions").GetValue(handler, null);
                sslOptions.GetType().GetProperty("RemoteCertificateValidationCallback").SetValue(sslOptions, validation, null);
            }

            return handler;
        }

        // Compiles (context, token) => new ValueTask<Stream>(ConnectAsync(context.DnsEndPoint, token))
        // against the runtime's Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>>.
        internal static Delegate BuildConnectCallback(Type delegateType)
        {
            if (!delegateType.IsGenericType)
            {
                throw new NotSupportedException(NotSupportedMessage);
            }

            Type[] arguments = delegateType.GetGenericArguments();
            if (arguments.Length != 3 || arguments[1] != typeof(CancellationToken))
            {
                throw new NotSupportedException(NotSupportedMessage);
            }

            PropertyInfo dnsEndPoint = arguments[0].GetProperty("DnsEndPoint");
            ConstructorInfo resultCtor = arguments[2].GetConstructor(new[] { typeof(Task<Stream>) });
            if (dnsEndPoint == null || dnsEndPoint.PropertyType != typeof(DnsEndPoint) || resultCtor == null)
            {
                throw new NotSupportedException(NotSupportedMessage);
            }

            MethodInfo connect = typeof(Ipv4OnlyHandlerFactory).GetMethod("ConnectAsync",
                BindingFlags.Static | BindingFlags.NonPublic, null,
                new[] { typeof(DnsEndPoint), typeof(CancellationToken) }, null);
            ParameterExpression context = Expression.Parameter(arguments[0], "context");
            ParameterExpression token = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
            Expression body = Expression.New(resultCtor,
                Expression.Call(connect, Expression.Property(context, dnsEndPoint), token));
            return Expression.Lambda(delegateType, body, context, token).Compile();
        }

        internal static RemoteCertificateValidationCallback BuildCertificateValidation(bool ignoreSSL, string ca)
        {
            if (ignoreSSL)
            {
                return delegate { return true; };
            }

            if (string.IsNullOrWhiteSpace(ca))
            {
                return null;
            }

            return (sender, cert, chain, sslPolicyErrors) =>
            {
                var collection = new X509Certificate2Collection();
                collection.Add(new X509Certificate2(Encoding.UTF8.GetBytes(ca)));
                if (HttpClientUtils.ServerCertificateCustomValidationCallback != null)
                {
                    return HttpClientUtils.ServerCertificateCustomValidationCallback(sender, collection, cert, chain, sslPolicyErrors);
                }
                return HttpClientUtils.CertificateValidationCallBack(sender, collection, cert, chain, sslPolicyErrors);
            };
        }

        internal static Task<Stream> ConnectAsync(DnsEndPoint endPoint, CancellationToken cancellationToken)
        {
            return ConnectAsync(endPoint.Host, endPoint.Port, cancellationToken);
        }

        internal static async Task<Stream> ConnectAsync(string host, int port, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IPAddress literal;
            IPAddress[] candidates = IPAddress.TryParse(host, out literal)
                ? new[] { literal }
                : await resolveIpv4Addresses(host, cancellationToken).ConfigureAwait(false);
            IPAddress[] addresses = candidates.Where(a => a.AddressFamily == AddressFamily.InterNetwork).ToArray();
            if (addresses.Length == 0)
            {
                throw new IOException(string.Format(
                    "ipv4Only is enabled but no IPv4 address was found for host '{0}'; IPv6 is not used.", host));
            }

            Exception lastError = null;
            foreach (IPAddress address in addresses)
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.NoDelay = true;
                try
                {
                    using (cancellationToken.Register(s => ((Socket)s).Dispose(), socket))
                    {
                        await socket.ConnectAsync(address, port).ConfigureAwait(false);
                    }
                    return new NetworkStream(socket, true);
                }
                catch (Exception e)
                {
                    socket.Dispose();
                    cancellationToken.ThrowIfCancellationRequested();
                    lastError = e;
                }
            }

            throw new IOException(string.Format(
                "ipv4Only is enabled but connecting to '{0}:{1}' over IPv4 failed.", host, port), lastError);
        }

        // Dns.GetHostAddressesAsync(string, AddressFamily, CancellationToken) (.NET 6+) queries A records only;
        // on older runtimes all addresses are resolved and ConnectAsync discards non-IPv4 results.
        internal static Func<string, CancellationToken, Task<IPAddress[]>> CreateIpv4Resolver()
        {
            MethodInfo byFamily = typeof(Dns).GetMethod("GetHostAddressesAsync",
                new[] { typeof(string), typeof(AddressFamily), typeof(CancellationToken) });
            if (byFamily == null)
            {
                return (host, token) => Dns.GetHostAddressesAsync(host);
            }

            var resolve = (Func<string, AddressFamily, CancellationToken, Task<IPAddress[]>>)Delegate.CreateDelegate(
                typeof(Func<string, AddressFamily, CancellationToken, Task<IPAddress[]>>), byFamily);
            return (host, token) => resolve(host, AddressFamily.InterNetwork, token);
        }
    }
}
#endif
