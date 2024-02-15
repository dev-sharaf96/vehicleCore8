using Prometheus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PrometheusLib.Classes
{
    public class PrometheusHttpRequestModule : IHttpModule
    {
        private static readonly Counter _globalExceptions = Metrics
          .CreateCounter("global_exceptions", "Number of global exceptions.");

        private static readonly Gauge _httpRequestsInProgress = Metrics
            .CreateGauge("http_requests_in_progress", "The number of HTTP requests currently in progress");

        private static readonly Gauge _httpRequestsTotal = Metrics
            .CreateGauge("http_requests_received_total", "Provides the count of HTTP requests that have been processed by this app",
                new GaugeConfiguration { LabelNames = new[] { "code", "method", "controller", "action" } });

        private static readonly Histogram _httpRequestsDuration = Metrics
            .CreateHistogram("http_request_duration_seconds", "The duration of HTTP requests processed by this app.",
                new HistogramConfiguration { LabelNames = new[] { "code", "method", "controller", "action" } });

        private const string _timerKey = "PrometheusHttpRequestModule.Timer";

        private static readonly Counter _globalCaptcha = Metrics.CreateCounter("Captcha_hits", "Number of global Captcha hits.");

        private static readonly Counter _validateCaptcha = Metrics.CreateCounter("ValidateCaptcha_hits", "Number of global Captcha Validate Hits.");

        private static readonly Counter _quoteHits = Metrics.CreateCounter("Quote_Hits", "Number of Hits on Quotation Method.");

        private static readonly Counter _initIquiryCounter = Metrics.CreateCounter("InitIquiry_Counter", "Number of Init Inquiry Hits.");

        private static readonly Counter _submitInquiryCounter = Metrics.CreateCounter("SubmitInquiry_Counter", "Number of Submit Inquiry hits.");

        private static readonly Counter _checkoutPaidCounter = Metrics.CreateCounter("CheckoutPaid_Counter", "Number of Paid Quotation.");

        public static void IncrementCaptchaCounter()
        {
            _globalCaptcha.Inc();
        }

        public static void IncrementQuotationCounter()
        {
            _quoteHits.Inc();
        }

        public static void IncrementValidateCaptchaCounter()
        {
            _validateCaptcha.Inc();
        }

        public static void IncrementInitInquiryCounter()
        {
            _initIquiryCounter.Inc();
        }

        public static void IncrementSubmitInquiryCounter()
        {
            _submitInquiryCounter.Inc();
        }


        public static void IncrementCheckoutPaidCounter()
        {
            _checkoutPaidCounter.Inc();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpApp"></param>
        public void Init(HttpApplication httpApp)
        {
            httpApp.BeginRequest += OnBeginRequest;
            httpApp.EndRequest += OnEndRequest;
            httpApp.Error += HttpApp_Error;
        }

        private void HttpApp_Error(object sender, EventArgs e)
        {
            _globalExceptions.Inc();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // Record the time of the begin request event.
        public void OnBeginRequest(Object sender, EventArgs e)
        {
            _httpRequestsInProgress.Inc();


            var httpApp = (HttpApplication)sender;
            var timer = new Stopwatch();
            httpApp.Context.Items[_timerKey] = timer;
            timer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnEndRequest(Object sender, EventArgs e)
        {
            _httpRequestsInProgress.Dec();

            var httpApp = (HttpApplication)sender;
            var timer = (Stopwatch)httpApp.Context.Items[_timerKey];

            if (timer != null)
            {
                timer.Stop();

                string code = httpApp.Response.StatusCode.ToString();
                string method = httpApp.Request.HttpMethod;
                var routeData = httpApp.Request.RequestContext?.RouteData?.Values;

                string controller = String.Empty;
                string action = String.Empty;

                if (routeData != null)
                {
                    if (routeData.ContainsKey("controller"))
                        controller = routeData["controller"].ToString();
                    if (routeData.ContainsKey("action"))
                        action = routeData["action"].ToString();
                }

                double timeTakenSecs = timer.ElapsedMilliseconds / 1000d;

                _httpRequestsDuration.WithLabels(code, method, controller, action).Observe(timeTakenSecs);
                _httpRequestsTotal.WithLabels(code, method, controller, action).Inc();
            }

            httpApp.Context.Items.Remove(_timerKey);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose() { /* Not needed */ }
    }
}
