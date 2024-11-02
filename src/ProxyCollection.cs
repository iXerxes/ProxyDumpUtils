using System.Net;

namespace ProxyDumpUtils {
    public class ProxyCollection {

        private static readonly Random _Random = new Random();

        public readonly List<WebProxy> Proxies = new List<WebProxy>();

        public ProxyCollection(List<WebProxy>? proxies = null) {
            Proxies = proxies == null ? new List<WebProxy>() : proxies;
        }

        public WebProxy GetRandom() {
            return Proxies.ElementAt(_Random.Next(0, Proxies.Count));
        }



        //
        // -- --
        //


        public static ProxyCollection FromHostPingResults(List<ProxyHostPingResult> pingResults, long? maxResponseTime = null) {
            List<WebProxy> proxies = new List<WebProxy>();

            if (maxResponseTime == null) {
                foreach (var result in pingResults) { if (result.IsSuccessStatusCode) proxies.Add(new WebProxy(result.Address));  }
            } else {
                foreach (var result in pingResults) if (result.IsSuccessStatusCode && result.ResponseTime <= maxResponseTime) proxies.Add(new WebProxy(result.Address));
            }

            return new ProxyCollection(proxies);
        }

    }
}
