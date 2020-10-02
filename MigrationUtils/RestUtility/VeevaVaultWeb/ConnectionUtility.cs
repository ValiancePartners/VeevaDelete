// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="ConnectionUtility.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains ConnectionUtility class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Text;
using RestUtility.Api;

namespace RestUtility.VeevaVaultWeb
{
    /// <summary>
    /// The vault connection implementation
    /// </summary>
    public sealed class ConnectionUtility : ThrottledConnection
    {
        public ConnectionUtility() : base(
            new Tuple<string, string>("X-VaultAPI-BurstLimitRemaining", "X-VaultAPI-DailyLimitRemaining"),
            new Tuple<int, int>(300, 3600)) { }

        protected override WebResponse GetApiResponse(string method, string vaultUrl, string content, Uri validatedUri, string responseTypes)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(validatedUri);

            request.Method = method;
            request.Accept = responseTypes;

            if (this.SessionId != null)
            {
                request.Headers.Add("authorization", this.SessionId);
            }

            // MP, 09/16/2019, Mantis 0001817, Support Vault API Client ID, handle clientId connection parameter
            if (!string.IsNullOrEmpty(this.ClientId))
            {
                request.Headers.Add("X-VaultAPI-ClientID", this.ClientId);
            }
            // end changes by MP on 09/16/2019

            // MP, 10/23/2019, Mantis 0001835, Add optional timeout length setting
            request.Timeout = TimeoutLength;
            //request.Timeout = 300000;
            // end changes by MP on 10/23/2019
            if (content == null)
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            else
            {
                request.ContentType = "text/csv";

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(content);
                request.ContentLength = bytes.Length;

                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                Contract.Assume(request.ContentLength == bytes.Length);
            }

            WebResponse response = null;

            string webErrorStatus = string.Empty;
            int count = 0;
            for (; ; )
            {
                try
                {
                    count++;
                    response = request.GetResponse();
                    break;
                }
                catch (WebException webExcp)
                {
                    WebExceptionStatus status = webExcp.Status;

                    switch (webExcp.Status)
                    {
                        case WebExceptionStatus.ConnectFailure:
                            if (count < 10)
                            {
                                System.Threading.Thread.Sleep(100);
                                continue;
                            }

                            break;
                        case WebExceptionStatus.ProtocolError:
                            throw new RestApiException(
                                string.Format("Vault Protocol Error: {0}", vaultUrl),
                                webExcp);
                        case WebExceptionStatus.SendFailure:
                            throw new RestApiException(
                                string.Format("Vault Not Listening: {0}", vaultUrl),
                                webExcp);
                    }

                    throw;
                }
            }

            return response;
        }
    }
}
// end changes by MP on 07/01/2019