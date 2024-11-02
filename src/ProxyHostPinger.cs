using System.Collections.Concurrent;
using System.Net;

namespace ProxyDumpUtils {
    public class ProxyHostPinger {

        internal static readonly Uri _DefaultPingUrl = new Uri("https://api.ipify.org/");
        internal static readonly TimeSpan _DefaultRequestTimeout = new TimeSpan(0, 0, 10);

        static ProxyHostPinger() {
            ThreadPool.SetMinThreads(500, 1);
        }



        public TimeSpan Timeout = _DefaultRequestTimeout;
        public Uri PingUrl = _DefaultPingUrl;

        private bool IsWorking = false;
        private CancellationTokenSource? WorkerCts;
        private int ExpectedWorkerResultCount = 0;
        public bool IsReady => !IsWorking;
        private ConcurrentBag<ProxyHostPingResult> WorkerResults = new ConcurrentBag<ProxyHostPingResult>();

        public List<ProxyHostPingResult>? Results;


        public ProxyHostPinger(TimeSpan? timeout = null,  Uri? pingUrl = null) {
            Timeout = timeout ?? Timeout;
            PingUrl = pingUrl ?? PingUrl;
        }

        public async Task<ProxyHostPingResult> TestHost(Uri host) {

            WebProxy proxy;
            try {
                proxy = new WebProxy(host);
            } catch (Exception ex) {
                return new ProxyHostPingResult(host, null, false, -1, ex);
            }

            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = new WebProxy(host);

            long reqTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            try {
                using (HttpClient client = new HttpClient(handler, true)) {
                    client.Timeout = Timeout;
                    HttpResponseMessage resp = await client.GetAsync(PingUrl);
                    return new ProxyHostPingResult(host, resp.StatusCode, resp.IsSuccessStatusCode, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - reqTimestamp);
                }
            } catch (Exception ex) {
                return new ProxyHostPingResult(host, null, false, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - reqTimestamp, ex);
            }

        }

        internal async void _ThreadedTestHost(object? state) {

            object[] args = state as object[];
            if (args[0] is Uri host) {
                ProxyHostPingResult result = await TestHost(host);
                WorkerResults.Add(result);
                if (WorkerResults.Count == ExpectedWorkerResultCount) WorkerCts.Cancel();
            }

        }

        public async Task<List<ProxyHostPingResult>> TestAllHosts(Uri[] hosts) {
            if (IsWorking) throw new InvalidOperationException("Proxy pinger already processing job.");
            if (hosts.Length == 0) return new List<ProxyHostPingResult>();
            IsWorking = true;
            WorkerCts = new CancellationTokenSource();
            WorkerResults.Clear();
            ExpectedWorkerResultCount = hosts.Length;

            foreach (var host in hosts) ThreadPool.QueueUserWorkItem(_ThreadedTestHost, new object[] { host }); // Queue the hosts for ping requests.

            while (!WorkerCts.IsCancellationRequested) WorkerCts.Token.WaitHandle.WaitOne(5000);
            WorkerCts.Dispose(); WorkerCts = null;
            IsWorking = false;

            Results = WorkerResults.ToList<ProxyHostPingResult>().OrderBy(x => x.ResponseTime).ToList();
            return Results;

        }

    }
}
