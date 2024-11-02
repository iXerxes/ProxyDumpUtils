namespace ProxyDumpUtils {

    internal enum EProxyDumpType {
        Http,
        Socks4,
        Socks5
    }

    public static class ProxyHostDump {

        // Credit: TheSpeedX - https://github.com/TheSpeedX/PROXY-List
        internal const string HttpDumpUrl = "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/http.txt";
        internal const string Socks4DumpUrl = "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/socks4.txt";
        internal const string Socks5DumpUrl = "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/socks5.txt";

        internal static async Task<List<Uri>> Fetch(EProxyDumpType dumpType) {
            using (HttpClient client = new HttpClient()) {

                HttpResponseMessage? resp = null;
                switch (dumpType) {
                    case EProxyDumpType.Http:
                        resp = await client.GetAsync(HttpDumpUrl);
                        break;
                    case EProxyDumpType.Socks4:
                        resp = await client.GetAsync(Socks4DumpUrl);
                        break;
                    case EProxyDumpType.Socks5:
                        resp = await client.GetAsync(Socks5DumpUrl);
                        break;
                }
                if (resp == null) throw new Exception("No response while fetching proxy dump.");

                string[] proxyAddresses = (await resp.Content.ReadAsStringAsync()).Split("\n");

                List<Uri> urls = new List<Uri>();
                foreach (var host in proxyAddresses) {
                    try { urls.Add(new Uri($"http://{host}")); } catch (Exception) { /** Ignore any malformed hosts **/ }
                }

                return urls;
            }
        }

        public static async Task<List<Uri>> FetchHttp() {
            return await Fetch(EProxyDumpType.Http);
        }

        public static async Task<List<Uri>> FetchSocks4() {
            return await Fetch(EProxyDumpType.Socks4);
        }

        public static async Task<List<Uri>> FetchSocks5() {
            return await Fetch(EProxyDumpType.Socks5);
        }

    }
}
