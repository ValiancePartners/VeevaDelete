using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestUtility.Api;

namespace RestUtility.Tests
{
    public class MockThrottledConnection : ThrottledConnection
    {
        public IEnumerable<string> ReturnContent { get; set; } = new string[] { string.Empty };

        public WebHeaderCollection Headers { get; set; } = new WebHeaderCollection();

        public MockThrottledConnection() : base(new Tuple<string, string>("Burst", "Daily"), new Tuple<int, int>(1, 2)) { }
        protected override WebResponse GetApiResponse(string method, string vaultUrl, string content, Uri validatedUri, string responseTypes)
        {
            var response = new WebResponseMock(this.ReturnContent.First(), this.Headers);
            this.ReturnContent = this.ReturnContent.Skip(1);
            return response;
        }

        private class WebResponseMock: WebResponse
        {
            private readonly string returnContent;
            private readonly WebHeaderCollection headers;
            public WebResponseMock(string returnContent, WebHeaderCollection headers)
            {
                this.returnContent = returnContent;
                this.headers = headers;
            }
            public override long ContentLength { get; set; }
            public override string ContentType { get; set; } = "application/xml";

            public override Stream GetResponseStream()
            {
                MemoryStream returnValue = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(this.returnContent));
                return returnValue;
            }

            public override WebHeaderCollection Headers {
                get
                {
                    return this.headers;
                }
            }


        }

    }
}
