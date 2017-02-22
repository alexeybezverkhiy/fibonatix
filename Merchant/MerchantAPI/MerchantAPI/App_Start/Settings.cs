using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace MerchantAPI.App_Start
{
    public enum ApplicationMode
    {
        TESTING,
        STAGE,
        PRODUCTION
    }

    public interface ISettings
    {
        string Version { get; }
        string PublicServerName { get; }
        string ClientId { get; }
        string SharedSecret { get; }
        NameValueCollection MerchantControlKeys { get; }
        ApplicationMode ApplicationMode { get; }
        string PaymentASPXEndpoint { get; }
        int CacheSlidingExpirationSeconds { get; }
        string PaymentKey { get;  }
    }

    public class TestSettings : ISettings
    {
        public ApplicationMode ApplicationMode => ApplicationMode.TESTING;
        public string Version => "1.0.0.0";
        public string ClientId => "99999999";
        public string SharedSecret => "test";
        // public string PublicServerName => "5.149.150.98";
        public string PublicServerName => "87.117.3.242";
        public string PaymentASPXEndpoint => "https://frontend.payment-transaction.net/payment.aspx";
        public int CacheSlidingExpirationSeconds => 600;
        public string PaymentKey => "creditcard_fibonatix";
        public NameValueCollection MerchantControlKeys => MerchantControlKeyCollection;

        private static readonly NameValueCollection MerchantControlKeyCollection = new NameValueCollection
        {
            {"250", "DDA76C34-0621-47DD-ADF2-F9D594ADCE2E"},
            {"251", "DDA76C34-0621-47DD-ADF2-F9D594ADCE2E"}
        };
    }

    public class StageSettings : ISettings
    {
        public ApplicationMode ApplicationMode => ApplicationMode.STAGE;
        public string Version => "1.0.0.0";
        public string ClientId => "99999999";
        public string SharedSecret => "test";
        // public string PublicServerName => "5.149.150.98";
        public string PublicServerName => "x.x.x.x";
        public string PaymentASPXEndpoint => "https://frontend.payment-transaction.net/payment.aspx";
        public int CacheSlidingExpirationSeconds => 600;
        public string PaymentKey => "creditcard_fibonatix";
        public NameValueCollection MerchantControlKeys => MerchantControlKeyCollection;

        private static readonly NameValueCollection MerchantControlKeyCollection = new NameValueCollection
        {
            {"250", "DDA76C34-0621-47DD-ADF2-F9D594ADCE2E"},
            {"251", "DDA76C34-0621-47DD-ADF2-F9D594ADCE2E"}
        };
    }

    public class ProductionSettings : ISettings
    {
        public ApplicationMode ApplicationMode => ApplicationMode.PRODUCTION;
        public string Version => "1.0.0.0";
        public string PublicServerName
        {
            get { return ""; }
        }
        public string ClientId
        {
            get { return ""; }
        }
        public string SharedSecret
        {
            get { return ""; }
        }
        public string PaymentASPXEndpoint
        {
            get { return "https://frontend.payment-transaction.net/payment.aspx"; }
        }

        public int CacheSlidingExpirationSeconds
        {
            get { return 600; }
        }

        public string PaymentKey
        {
            get { return "creditcard_fibonatix"; }
        }

        public NameValueCollection MerchantControlKeys => MerchantControlKeyCollection;
        private static readonly NameValueCollection MerchantControlKeyCollection = new NameValueCollection();
    }

    public class SettingsFactory
    {
        private ISettings _settings;
        
        private SettingsFactory() { }

        public SettingsFactory(ISettings settings)
        {
            _settings = settings;
        }

        public TimeSpan CreateCacheSlidingExpiration()
        {
            return new TimeSpan(0, 0, _settings.CacheSlidingExpirationSeconds);
        }
    }
}