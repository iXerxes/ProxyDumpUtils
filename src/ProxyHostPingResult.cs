using System.Net;

namespace ProxyDumpUtils {
    public class ProxyHostPingResult {

        /// <summary>
        /// The proxy address that was tested.
        /// </summary>
        public readonly Uri Address;

        /// <summary>
        /// The response code of ping request.<br/>
        /// null if the ping request resulted in an error.
        /// </summary>
        public readonly HttpStatusCode? Code;

        /// <summary>
        /// Whether or not the response resulted in a success status code.
        /// </summary>
        public readonly bool IsSuccessStatusCode;

        /// <summary>
        /// The response time in milliseconds.
        /// </summary>
        public readonly long ResponseTime;


        /// <summary>
        /// The error thrown while making the ping request.<br/>
        /// Only preset if the rquest resulted in an error.
        /// </summary>
        public readonly Exception? Error;

        public ProxyHostPingResult(Uri host, HttpStatusCode? code, bool isSuccessStatusCode, long responseTime, Exception? exception = null) {
            Address = host;
            Code = code;
            IsSuccessStatusCode = isSuccessStatusCode;
            ResponseTime = responseTime;
            Error = exception;
        }

        public override string ToString() {
            string shortResult = IsSuccessStatusCode ? "PASS" : "FAIL";

            return Error == null
                ? $"[{shortResult} : {Code}] {Address.Host}:{Address.Port}  ¦  {ResponseTime}ms"
                : $"[{shortResult} : ERROR] {Address.Host}:{Address.Port}   ¦  {ResponseTime}ms  ¦  [{Error.GetType().Name}] {Error.Message}";

        }

    }
}
