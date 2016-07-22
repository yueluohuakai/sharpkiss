using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Security;
using System.Text;

namespace Kiss.Utils.Net
{
    public class HttpRequest
    {
        private const int defaultTimeout_ = 60000;
        private static string referer_ = string.Empty;

        //public static readonly string UserAgent = String.Format("Openlab {0} ({1}; .NET CLR {2};)", SiteStatistics.VersionVersionInfo, Environment.OSVersion.ToString(), Environment.Version.ToString());

        /// <summary>
        /// Creates a new HttpRequest with the default Referral value
        /// </summary>
        public static HttpWebRequest CreateRequest(string url)
        {
            return CreateRequest(url, referer_);
        }

        /// <summary>
        /// Creates a new HttpRequest and sets the referral value.
        /// </summary>
        public static HttpWebRequest CreateRequest(string url, string referral)
        {
            ICredentials credentials = null;

            //note this code will not work if there is an @ or : in the username or password
            if (url.IndexOf('@') > 0)
            {
                string[] urlparts = url.Split('@');
                if (urlparts.Length >= 2)
                {
                    string[] userparts = urlparts[0].Split(':');

                    if (userparts.Length == 3)
                    {
                        string protocol = userparts[0];
                        string username = userparts[1].TrimStart('/');
                        string password = userparts[2];

                        credentials = new NetworkCredential(username, password);
                        url = url.Replace(string.Format("{0}:{1}@", username, password), "");
                    }

                }
            }
            else
            {
                credentials = CredentialCache.DefaultCredentials;
            }

            WebRequest req;

            // This may throw a SecurityException if under medium trust... should set it to null so it will return instead of error out.
            try { req = WebRequest.Create(url); }
            catch (SecurityException) { req = null; }

            HttpWebRequest wreq = req as HttpWebRequest;
            if (null != wreq)
            {
                //wreq.UserAgent = UserAgent;
                wreq.Referer = referral;
                wreq.Timeout = defaultTimeout_;

                if (credentials != null)
                    wreq.Credentials = credentials;
            }
            return wreq;
        }

        public static HttpWebResponse MakeHttpPost(string url, string referral, params HttpPostItem[] items)
        {
            return MakeHttpPost(CreateRequest(url, referral), items);
        }

        public static HttpWebResponse MakeHttpPost(string url, params HttpPostItem[] items)
        {
            return MakeHttpPost(url, null, items);
        }

        public static HttpWebResponse MakeHttpPost(HttpWebRequest wreq, params HttpPostItem[] items)
        {
            if (wreq == null)
                throw new Exception("HttpWebRequest is not initialized");

            if (items == null || items.Length == 0)
                throw new Exception("No HttpPostItems");

            StringBuilder parameters = new StringBuilder();

            foreach (HttpPostItem item in items)
            {
                parameters.Append("&" + item.ToString());
            }

            byte[] payload = Encoding.UTF8.GetBytes(parameters.ToString().Substring(1));

            wreq.Method = "POST";
            wreq.ContentLength = payload.Length;
            wreq.ContentType = "application/x-www-form-urlencoded";
            wreq.KeepAlive = false;

            using (Stream st = wreq.GetRequestStream())
            {
                st.Write(payload, 0, payload.Length);
                st.Close();

                return wreq.GetResponse() as HttpWebResponse;
            }
        }

        /// <summary>
        /// Gets the HttpResponse using the default referral
        /// </summary>
        public static HttpWebResponse GetResponse(string url)
        {
            WebExceptionStatus status;
            return GetResponse(url, referer_, out status);
        }

        /// <summary>
        /// Gets the HttpResponse using the supplied referral
        /// </summary>
        public static HttpWebResponse GetResponse(string url, string referral, out WebExceptionStatus status)
        {
            HttpWebRequest request = CreateRequest(url, referral);
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                status = WebExceptionStatus.Success;
            }
            catch (WebException e)
            {
                status = e.Status;
            }
            catch
            {
                status = WebExceptionStatus.UnknownError;
            }
            return response;
        }

        public static string GetPageText(string url)
        {
            return GetPageText(url, referer_);
        }

        /// <summary>
        /// Gets the full text at the url parameter
        /// </summary>
        public static string GetPageText(string url, Encoding encode, out WebExceptionStatus status)
        {
            return GetPageText(url, referer_, encode, out status);
        }

        /// <summary>
        /// Gets the full text at the url parameter
        /// </summary>
        public static string GetPageText(string url, string referral, Encoding encode, out WebExceptionStatus status)
        {
            HttpWebResponse response = GetResponse(url, referral, out status);
            if (response == null)
                return null;
            return GetPageText(response, encode);
        }

        public static string GetPageText(string url, string referral)
        {
            WebExceptionStatus status;

            HttpWebResponse response = GetResponse(url, referral, out status);
            return GetPageText(response);
        }

        /// <summary>
        /// Gets the full text at the url parameter
        /// </summary>
        public static string GetPageText(HttpWebResponse response)
        {
            Encoding encode;
            string enc = string.Empty;

            if (!string.IsNullOrWhiteSpace(response.ContentEncoding))
                enc = response.ContentEncoding.Trim();

            if (string.IsNullOrEmpty(enc) && StringUtil.HasText(response.CharacterSet))
                enc = response.CharacterSet;

            if (string.IsNullOrEmpty(enc) && StringUtil.HasText(response.ContentType))
            {
                string contentType = response.ContentType;
                string c = "charset=";
                enc = contentType.Substring(contentType.IndexOf(c) + c.Length);
            }

            if (string.IsNullOrEmpty(enc))
                encode = Encoding.UTF8;
            else
                encode = Encoding.GetEncoding(enc);

            return GetPageText(response, encode);
        }

        public static string GetPageText(HttpWebResponse response, Encoding encode)
        {
            using (Stream s = response.GetResponseStream())
            {
                string html;
                using (StreamReader sr = new StreamReader(s, encode))
                {
                    html = sr.ReadToEnd();
                }
                return html;
            }
        }

        public static string UploadFile(string url, string contentType, string paramName, string file, NameValueCollection nvc)
        {
            return UploadFile(url, contentType, paramName, Path.GetFileName(file), File.ReadAllBytes(file), nvc);
        }

        public static string UploadFile(string url, string contentType, string paramName, string filename, byte[] buffer, NameValueCollection nvc)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] boundarybytesF = System.Text.Encoding.ASCII.GetBytes("--" + boundary + "\r\n");  // the first time it itereates, you need to make sure it doesn't put too many new paragraphs down or it completely messes up poor webbrick.  

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            wr.Headers.Add("Accepts-Language", "en-us,en;q=0.5");
            wr.ContentType = "multipart/form-data; boundary=" + boundary;

            using (Stream rs = wr.GetRequestStream())
            {
                bool firstLoop = true;
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                if (nvc != null)
                {
                    foreach (string key in nvc.Keys)
                    {
                        if (firstLoop)
                        {
                            rs.Write(boundarybytesF, 0, boundarybytesF.Length);
                            firstLoop = false;
                        }
                        else
                        {
                            rs.Write(boundarybytes, 0, boundarybytes.Length);
                        }
                        string formitem = string.Format(formdataTemplate, key, nvc[key]);
                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                        rs.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, paramName, filename, contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                rs.Write(buffer, 0, buffer.Length);

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
            }

            WebResponse wresp = null;
            try
            {
                using (wresp = wr.GetResponse())
                {
                    using (Stream stream2 = wresp.GetResponseStream())
                    {
                        using (StreamReader reader2 = new StreamReader(stream2))
                        {
                            return reader2.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            return null;
        }
    }
}
