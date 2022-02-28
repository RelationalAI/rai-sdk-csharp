namespace RAILib
{
    using System;
    using System.Collections.Generic;
   /*
    class HttpError : SystemException
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public static Dictionary<int, string> statusText = new Dictionary<int, string>()
        {
            { 200, "OK" },
            { 201, "Created" },
            { 202, "Accepted" },
            { 204, "No Content" },
            { 400, "Bad Request" },
            { 401, "Unauthorized" },
            { 403, "Forbidden" },
            { 404, "Not Found" },
            { 405, "Method Not Allowed" },
            { 409, "Conflict" },
            { 410, "Gone" },
            { 500, "Internal Server Error" },
            { 501, "Not Implemented" },
            { 502, "Bad Gateway" },
            { 503, "Service Unavailable" },
            { 504, "Gateway Timeout" },
        };

    public HttpError(int statusCode) {
        this.StatusCode = statusCode;
    }

    public HttpError(int statusCode, String message) {
        this.StatusCode = statusCode;
        this.Message = message;
    }

    public String toString() {
        String result = Integer.toString(statusCode);
        if (statusText.containsKey(statusCode))
            result += " " + statusText.get(statusCode);
        if (message != null)
            result += "\n" + message;
        return result;
    }
    }
    */
}