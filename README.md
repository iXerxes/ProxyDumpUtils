# ProxyDumpUtils
A dumb library to pull from a proxy dump, test them and store them in a proxy collection.<br>
Dumps sourced from: [TheSpeedX/PROXY-List](https://github.com/TheSpeedX/PROXY-List)

### Getting a host dump
- [HTTP](https://github.com/TheSpeedX/PROXY-List/blob/master/http.txt) - `ProxyHostDump.FetchHttp()`
- [SOCKS4](https://github.com/TheSpeedX/PROXY-List/blob/master/socks4.txt) - `ProxyHostDump.FetchSocks4()`
- [SOCKS5](https://github.com/TheSpeedX/PROXY-List/blob/master/socks5.txt) - `ProxyHostDump.FetchSocks5()`

> Gets the dump of its respective type and returns them as `List<Uri>`.

---

### Testing proxy hosts

Create a new `ProxyHostPinger` instance:
- `TimeSpan? timeout` indicates how long to wait for a response. Default is 10s.
- `Uri? pingUrl` changes the address to send test request to. Default is https://api.ipify.org/
```c#
ProxyHostPinger pinger = new ProxyHostPinger(new TimeSpan(0,0,10), new Uri("https://api.ipify.org/"));
```

**Test a single host:**
```c#
Uri testHost = new Uri("http//:127.0.0.1:8888");
ProxyHostPingResult result = await pinger.TestHost(testHost)
```
**Test multiple hosts:**
```c#
List<Uri> hosts = await ProxyHostDump.FetchHttp();
List<ProxyHostPingResult> results = await pinger.TestAllHosts(hosts.ToArray());
```
