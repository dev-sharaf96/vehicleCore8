using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Tameenk.PaymentNotificationsApi.Domain.WafierAndWaffer;

namespace Tameenk.PaymentNotificationsApi.Contexts
{
    public class WafierAndWafferDbContext : IdentityDbContext
    {
        public WafierAndWafferDbContext(string applicationName)
            : base("name=" + applicationName + "Connection")
        {
        }
        public DbSet<Invoices> Invoices { get; set; }
        public DbSet<SadadApiMessage> MessageWebsrevices { get; set; }
        public DbSet<PaymentMethods> PaymentMethods { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<PaymentRequests> PaymentRequests { get; set; }
        public DbSet<PaymentRequestDetails> PaymentRequestDetails { get; set; }
        public DbSet<PaymentResponse> PaymentResponse { get; set; }
        public DbSet<PaymentResponseDetails> PaymentResponseDetails { get; set; }
        public DbSet<PaymentConfigurations> PaymentConfigurations { get; set; }
    }
}