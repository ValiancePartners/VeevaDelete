// MP, 07/01/2019, Mantis 0001782, Vault Binder to Binder Links, merge into MigrationUtils repository
//-----------------------------------------------------------------------
// <copyright file="ThrottledConnection.cs" company="Valiance Partners">
//     Copyright (c) Valiance Partners. All rights reserved.
// </copyright>
// <summary>
// This file contains ThrottledConnection class.
// </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace RestUtility.Api
{
    public abstract class ThrottledConnection : IRestConnection
    {
        // MP, 10/23/2019, Mantis 0001835, Add timeout handling
        /// <summary>
        /// the event handler to handle to no respoinse from API before timeout limit
        /// </summary>
        public event EventHandler<TimeoutEventArgs> TimeoutExpired;
        // end changes by MP on 10/23/2019

        /// <summary>
        /// the event handler to respond to API limit setting event
        /// </summary>
        public event EventHandler<LimitEventArgs> LimitUpdated;

        /// <summary>
        /// the event handler to respond to pause on API limit reached event
        /// </summary>
        public event EventHandler<ThrottleEventArgs> LimitThrottling;

        /// <summary>
        /// the event handler to respond to resuming after API limit reached pause
        /// </summary>
        public event EventHandler<ThrottleEventArgs> LimitThrottled;

        /// <summary>
        /// Gets the default base Url prepended to any query when the connection is open
        /// </summary>
        public Uri DefaultUrl { get; private set; } = null;

        ///// <summary>
        ///// Gets the session id of an open connection
        ///// </summary>
        public string SessionId { get; set; } = string.Empty;

        // MP, 09/16/2019, Mantis 0001817, Support Vault API Client ID, record clientId connection parameter
        /// <summary>
        /// Gets the headers which will be included in every REST request
        /// </summary>
        public string ClientId { get; private set; }
        // end changes by MP on 09/16/2019

        /// <summary>
        /// Gets or sets the number of API calls to reserve : will not go below this limit.
        /// </summary>
        public long DailyReserve { get; set; }

        /// <summary>
        /// Gets or sets the number of API calls to reserve : will not go below this limit.
        /// </summary>
        public long BurstReserve { get; set; }

        /// <summary>
        /// Gets the maximum size for batch deletes
        /// </summary>
        public int MaxBatchSize => 500;

        // MP, 10/23/2019, Mantis 0001835, Add timeout handling
        /// <summary>
        /// Gets or sets the response timeout length
        /// </summary>
        public int TimeoutLength { get; set; } = 300000;

        /// <summary>
        /// Gets or sets the number of times to retry a request if the response times out
        /// </summary>
        public int TimeoutRetries { get; set; } = 1;
        // end changes by MP on 10/23/2019

        /// <summary>
        /// Gets the session id of an open connection
        /// </summary>
        private string username = string.Empty;

        /// <summary>
        /// Gets the session id of an open connection
        /// </summary>
        private string password = string.Empty;

        /// <summary>
        /// Gets the maximum size for batch deletes
        /// </summary>
        public decimal ApiVersion => this.apiVersion;

        // private bool xmlError = false;
        // private string xmlApiError = string.Empty;
        private decimal apiVersion;

        /// <summary>
        /// daily API calls remaining
        /// </summary>
        private long? dailyLimit = null;

        /// <summary>
        /// burst (to next 5 min checkpoint) API call limit remaining
        /// </summary>
        private long? burstLimit = null;


        private readonly string burstLimitTag;
        private readonly string dailyLimitTag;
        private readonly int burstSliceSeconds;
        private readonly int dailySliceSeconds;

        public ThrottledConnection(Tuple<string, string> limitTags, Tuple<int, int> sliceSeconds)
        {
            this.burstLimitTag = limitTags.Item1;
            this.dailyLimitTag = limitTags.Item2;
            this.burstSliceSeconds = sliceSeconds.Item1;
            this.dailySliceSeconds = sliceSeconds.Item2;
        }
        /// <summary>
        /// calculate the next available time slice for burst or daily limits
        /// </summary>
        /// <param name="from">the time in the current time slice</param>
        /// <param name="forDaily">indicates if daily limit is exceeded rather than burst limit</param>
        /// <returns>the time at which API calls can be resumed</returns>
        public DateTime NextTimeSlice(DateTime from, bool forDaily)
        {
            DateTime returnValue = from.AddMilliseconds(
                MilliSecondsToNextSlice(from, forDaily ? this.dailySliceSeconds : this.burstSliceSeconds));
            return returnValue;
        }

        /// <summary>
        /// calculate the number of milliseconds to the next time slice (of the specified number of minutes)
        /// </summary>
        /// <param name="from">the time in the current time slice</param>
        /// <param name="sliceSeconds">the time slice size in seconds. There should be an whole number of slices per hour</param>
        /// <returns>the number of millisecond to the next time slice of the specified size></returns>
        public static int MilliSecondsToNextSlice(DateTime from, int sliceSeconds)
        {
            if (!(sliceSeconds != 0 || sliceSeconds > 3600))
            {
                throw new ArgumentOutOfRangeException(nameof(sliceSeconds));
            }
            if (3600 % sliceSeconds != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sliceSeconds));
            }

            Contract.EndContractBlock();
            int remainingSeconds = sliceSeconds - (((from.Minute * 60) + from.Second) % sliceSeconds);
            int returnValue = (remainingSeconds * 1000) - from.Millisecond;
            return returnValue;
        }


        /// <summary>
        /// Scan the response headers for limit notifications
        /// </summary>
        /// <param name="headers">the response headers</param>
        /// <returns>limit event arguments if the headers include a limit notification, or null otherwise</returns>
        public static LimitEventArgs FindLimitUpdate(NameValueCollection headers, string burstLimitTag, string dailyLimitTag)
        {
            if (!(headers != null))
            {
                throw new ArgumentNullException(nameof(headers));
            }

            Contract.EndContractBlock();

            long? burstLimit = null, dailyLimit = null;
            bool limitReported = false;
            for (int i = 0; i < headers.Count; i++)
            {
                if (headers.Keys[i] == burstLimitTag)
                {
                    if (long.TryParse(headers[i], out long limit))
                    {
                        burstLimit = limit;
                        limitReported = true;
                    }
                    else
                    {
                        throw new UnexpectedResponseRestApiException(
                            string.Format("Burst Limit is not a number: {0}", headers[i]));
                    }
                }

                if (headers.Keys[i] == dailyLimitTag)
                {
                    if (long.TryParse(headers[i], out long limit))
                    {
                        dailyLimit = limit;
                        limitReported = true;
                    }
                    else
                    {
                        throw new UnexpectedResponseRestApiException(
                            string.Format("Daily Limit is not a number: {0}", headers[i]));
                    }
                }
            }

            if (limitReported)
            {
                // call the function to raise the event
                return new LimitEventArgs(burstLimit, dailyLimit);
            }

            return null;
        }


        // MP, 09/16/2019, Mantis 0001817, Support Vault API Client ID, record clientId connection parameter
        /// <summary>
        /// Register a new connection, ALL this does is set some class level fields from the parameters
        /// </summary>
        /// <param name=apiVersion">the API Version for the connection</param>
        /// <param name="baseUrl">the base Url for the connection</param>
        /// <param name="username">the user name for the connection</param>
        /// <param name="password">the user name for the connection</param>
        /// <param name="clientId">the client Id for the connection</param>
        public void SetupConnection(decimal apiVersion, Uri baseUrl, string username, string password, string clientId = null)
        //public void SetupConnection(decimal apiVersion, Uri baseUrl, string username, string password)
        // end changes by MP on 09/16/2019
        {
            this.apiVersion = apiVersion;
            this.DefaultUrl = baseUrl;
            this.username = username;
            this.password = password;
            // MP, 09/16/2019, Mantis 0001817, Support Vault API Client ID, record clientId connection parameter
            this.ClientId = clientId;
            // end changes by MP on 09/16/2019
        }

        /// <summary>
        /// Register a new connection
        /// </summary>
        /// <param name="username">the user name for the connection</param>
        /// <param name="password">the user name for the connection</param>
        public void GetCredentials(out string username, out string password)
        {
            username = this.username;
            password = this.password;
        }

        /// <summary>
        /// Start a new session
        /// </summary>
        public void StartSession(string sessionID)
        {
            this.SessionId = sessionID;
        }

        /// <summary>
        /// Pass instruction to vault and return result
        /// </summary>
        /// <param name="method">HTTP Method</param>
        /// <param name="vaultUrl">Absolute or relative Url</param>
        /// <param name="content">optional CSV content to send</param>
        /// <returns>Result of HTTP method</returns>
        public T ExecuteApi<T>(Dictionary<string, Func<Stream, T>> responseProcessors, string method, string vaultUrl, string content = null)
        {
            // VeevaDelete vdObject = new VeevaDelete();
            // LimitUpdate += new EventHandler<LimitArgs>(vdObject.UpdateLimits);
            // Append Default Url to vaultUrl if does not exitst. This the case with VQL Queries
            if (!Uri.TryCreate(vaultUrl, UriKind.RelativeOrAbsolute, out Uri validatedUri))
            {
                throw new UnsupportedRequestRestApiContractException(
                    string.Format("Incorrect URL format for Vault Connection: {0}", this.DefaultUrl));
            }

            if (this.DefaultUrl != null)
            {
                validatedUri = new Uri(this.DefaultUrl, validatedUri);
            }

            if (!validatedUri.IsAbsoluteUri)
            {
                throw new UnsupportedRequestRestApiContractException(
                    string.Format("Missing Site in URI: {0}", validatedUri.OriginalString));
            }

            // if the URI is valid, the scheme might not be http or https
            if (validatedUri.Scheme != Uri.UriSchemeHttp && validatedUri.Scheme != Uri.UriSchemeHttps)
            {
                // if the scheme is invalid it must be the open session request
                throw new UnsupportedRequestRestApiContractException(
                    string.Format("Expected HTTP(s) URI Scheme in Vault API: {0}", validatedUri.OriginalString));
            }

            this.Prevalidate();

            string responseTypes = string.Join(";", responseProcessors.Keys);

            // MP, 10/23/2019, Mantis 0001835, Add timeout handling
            for (int retry = 0; ; ++retry)
            {
                try
                {
                    using (WebResponse response = GetApiResponse(method, vaultUrl, content, validatedUri, responseTypes))
                    {
                        //WebResponse response = GetApiResponse(method, vaultUrl, content, validatedUri, responseTypes);
                        // end changes by MP on 10/23/2019

                        string responseType = response.ContentType.Split(';')[0];
                        T returnValue = default(T);
                        if (responseProcessors.TryGetValue(responseType, out Func<Stream, T> responseProcessor))
                        {
                            // MP, 10/23/2019, Mantis 0001835, Add optional timeout length setting
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                //Stream responseStream = response.GetResponseStream();
                                // end changes by MP on 10/23/2019
                                returnValue = responseProcessor(responseStream);
                                // MP, 10/23/2019, Mantis 0001835, Add optional timeout length setting
                            }
                            // end changes by MP on 10/23/2019
                        }
                        else
                        {
                            throw new UnexpectedResponseRestApiException($"Response Content Type: {responseType}");
                        }

                        //AK, 10/1/2020, do not reset limits when auth running authorization API, burst limits are different
                        if (validatedUri.ToString().IndexOf("/auth") == -1)
                        {
                            this.PostValidate(response.Headers);
                        }
                        //this.PostValidate(response.Headers);
                        //end changes by AK on 10/1/2020

                        response.Close();

                        return returnValue;
                        // MP, 10/23/2019, Mantis 0001835, Add timeout handling
                    }

                }
                catch (WebException e) when (e.Status == WebExceptionStatus.Timeout)
                {
                    TimeoutEventArgs timedout = new TimeoutEventArgs(retry);
                    OnTimeeoutExpired(this, timedout);
                    if (timedout.Cancel)
                    {
                        string errorResponseMessage = null;
                        if (e.Response != null)
                        {
                            using (WebResponse errorResponse = e.Response)
                            {
                                using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                                {
                                    errorResponseMessage = reader.ReadToEnd();
                                }
                            }
                        }
                        throw new SlowRestApiResponseException($"Response not ready before timeout {TimeoutLength}. Response: {errorResponseMessage}");
                    }
                }

            }

            // end changes by MP on 10/23/2019
        }

        public void PostValidate(NameValueCollection headers)
        {
            LimitEventArgs limitUpdate = FindLimitUpdate(headers, this.burstLimitTag, this.dailyLimitTag);

            if (limitUpdate != null)
            {
                if (limitUpdate.BurstLimit.HasValue)
                {
                    this.burstLimit = limitUpdate.BurstLimit;
                }
                //AK, 10/1/2020, fix if
                if (limitUpdate.DailyLimit.HasValue)
                //if (limitUpdate.BurstLimit.HasValue)
                //end changes by AK on 10/1/2020
                {
                    this.dailyLimit = limitUpdate.DailyLimit;
                }

                // call the function to raise the event
                this.OnLimitUpdated(this, limitUpdate);
            }
        }

        private void Prevalidate()
        {
            if (this.dailyLimit <= this.DailyReserve || this.burstLimit < this.BurstReserve)
            {
                //TODO: reserve < 0?
                this.OnLimitReached(this, new LimitEventArgs(this.burstLimit, this.dailyLimit));
            }

        }

        // MP, 10/23/2019, Mantis 0001835, Add timeout handling
        /// <summary>
        /// Hook to handle API response timeouts
        /// </summary>
        /// <param name="sender">The vault connection that timed out</param>
        /// <param name="e">the timeout event with the retry count</param>
        internal void OnTimeeoutExpired(object sender, TimeoutEventArgs e)
        {
            this.TimeoutExpired?.Invoke(sender, e);
        }
        // end changes by MP on 10/23/2019

        /// <summary>
        /// Hook to handle changes to the API limits
        /// </summary>
        /// <param name="sender">The vault connection where the API limit is changing</param>
        /// <param name="e">the new API limits</param>
        internal void OnLimitUpdated(object sender, LimitEventArgs e)
        {
            this.LimitUpdated?.Invoke(sender, e);
        }

        /// <summary>
        /// Hook to handle API limit being reached
        /// </summary>
        /// <param name="sender">The vault connection where the API limit is changing</param>
        /// <param name="e">the new API limits</param>
        internal void OnLimitReached(object sender, LimitEventArgs e)
        {
            DateTime now = DateTime.Now;

            //TODO: mechanism to modify behaviour: cancel wait/throw/etc?
            EventHandler<ThrottleEventArgs> throttleHandler = this.LimitThrottling;
            DateTime resumeAt = NextTimeSlice(now, e.DailyLimit < this.DailyReserve);
            ThrottleEventArgs throttleEventArgs = new ThrottleEventArgs(resumeAt, e.BurstLimit, e.DailyLimit);
            throttleHandler?.Invoke(sender, throttleEventArgs);

            WaitUntil(resumeAt);

            throttleHandler = this.LimitThrottled;
            throttleHandler?.Invoke(sender, throttleEventArgs);
        }

        /// <summary>
        /// Wait until the speciified time, ensuring UI events are processed.
        /// </summary>
        /// <param name="resumeAt">The time to wait for</param>
        private static void WaitUntil(DateTime resumeAt)
        {
            while (DateTime.Now < resumeAt)
            {
                // if we are running on a UI thread, don't block it
                if (System.Windows.Forms.Application.OpenForms.Count > 0 &&
                    System.Windows.Forms.Application.OpenForms[0].InvokeRequired)
                {
                    Thread.Sleep(1);
                    System.Windows.Forms.Application.DoEvents();
                }
                else
                {
                    Thread.Sleep(5000);
                }
            }
        }
        protected abstract WebResponse GetApiResponse(string method, string vaultUrl, string content, Uri validatedUri, string responseTypes);

    }
}
// end changes by MP on 07/01/2019