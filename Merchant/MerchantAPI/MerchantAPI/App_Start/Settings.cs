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

    public class MerchantConfig
    {
        public int EndpointId { get; }
        public string ClientID { get; }
        public string SharedSecret { get; }
        public string MerchantControlKey { get; }
        public bool FrontendRequired { get; }

        public MerchantConfig(int endpointId, string clientID, string sharedSecret, string merchantControlKey, bool frontendRequired)
        {
            this.EndpointId = endpointId;
            this.ClientID = clientID;
            this.SharedSecret = sharedSecret;
            this.MerchantControlKey = merchantControlKey;
            this.FrontendRequired = frontendRequired;
        }
    }

    public interface ISettings
    {
        string Version { get; }
        string PublicServerName { get; }
        ApplicationMode ApplicationMode { get; }
        string PaymentASPXEndpoint { get; }
        string BackendServiceUrl { get; }
        string ConfigurationServiceUrl { get; }
        int CacheSlidingExpirationSeconds { get; }
        string PaymentKey { get; }
        int ConfigurationsLoaded { get; }
        int ConfigurationExpirationSeconds { get; }

        string GetClientID(int endpointId);
        string GetSharedSecret(int endpointId);
        string GetMerchantControlKey(int endpointId);
    }

    public class TestSettings : ISettings
    {
        public ApplicationMode ApplicationMode => ApplicationMode.TESTING;
        public string Version => "1.0.0.0";
        // public string PublicServerName => "5.149.150.98";
        // public string PublicServerName => "87.117.3.242";
        public string PublicServerName => "localhost";
        //public string PaymentASPXEndpoint => "http://localhost:8010/payment.aspx";
        public string PaymentASPXEndpoint => "https://frontend.payment-transaction.net/payment.aspx";
        public string BackendServiceUrl => "https://service.commpay.net/server2server/payment/transaction.asmx";
        public string ConfigurationServiceUrl => "https://service.commpay.net/server2server/security/endpoints.asmx";
        public int CacheSlidingExpirationSeconds => 600;
        public string PaymentKey => "creditcard_fibonatix";
        public int ConfigurationsLoaded => 2;
        public int ConfigurationExpirationSeconds => 1;

        public string GetClientID(int endpointId)
        {
            return "99999999";
        }

        public string GetSharedSecret(int endpointId)
        {
            return "test";
        }

        public string GetMerchantControlKey(int endpointId)
        {
            switch (endpointId)
            {
                case 250:
                    return "DDA76C34-0621-47DD-ADF2-F9D594ADCE2E";
                case 251:
                    return "DDA76C34-0621-47DD-ADF2-F9D594ADCE2E";
            }

            return null;
        }
    }

    public class StageSettings : ISettings
    {
        public ApplicationMode ApplicationMode => ApplicationMode.STAGE;
        public string Version => "1.0.0.0";
        // public string PublicServerName => "5.149.150.98";
        // public string PublicServerName => "x.x.x.x";
        public string PublicServerName => "127.0.0.1";
        public string PaymentASPXEndpoint => "https://frontend.payment-transaction.net/payment.aspx";
        public string BackendServiceUrl => "https://service.commpay.net/server2server/payment/transaction.asmx";
        public string ConfigurationServiceUrl => "https://service.commpay.net/server2server/security/endpoints.asmx";
        public int CacheSlidingExpirationSeconds => 600;
        public string PaymentKey => "creditcard_fibonatix";
        public int ConfigurationsLoaded => 2;
        public int ConfigurationExpirationSeconds => 1;

        public string GetClientID(int endpointId)
        {
            return "99999999";
        }

        public string GetSharedSecret(int endpointId)
        {
            return "test";
        }

        public string GetMerchantControlKey(int endpointId)
        {
            switch (endpointId)
            {
                case 250:
                    return "DDA76C34-0621-47DD-ADF2-F9D594ADCE2E";
                case 251:
                    return "DDA76C34-0621-47DD-ADF2-F9D594ADCE2E";
            }

            return null;
        }
    }

    public class ProductionSettings : ISettings
    {
        private int configsLoaded = 0;

        public ApplicationMode ApplicationMode => ApplicationMode.PRODUCTION;
        public string Version => "1.0.0.0";
        public string PublicServerName
        {
            get { return ""; }
        }

        public string PaymentASPXEndpoint
        {
            get { return "https://frontend.payment-transaction.net/payment.aspx"; }
        }

        public string BackendServiceUrl
        {
            get { return "https://service.commpay.net/server2server/payment/transaction.asmx"; }
        }

        public string ConfigurationServiceUrl
        {
            get { return "https://service.commpay.net/server2server/security/endpoints.asmx"; }
        }

        public int CacheSlidingExpirationSeconds
        {
            get { return 600; }
        }

        public string PaymentKey
        {
            get { return "creditcard_fibonatix"; }
        }

        public int ConfigurationsLoaded
        {
            get { return configsLoaded; }
        }

        public int ConfigurationExpirationSeconds
        {
            get { return 3600; } // 1 hour
        }

        public string GetClientID(int endpointId)
        {
            MerchantConfig config = this.GetMerchantConfig(endpointId);
            if (config != null)
            {
                return config.ClientID;
            }

            return null;
        }

        public string GetSharedSecret(int endpointId)
        {
            MerchantConfig config = this.GetMerchantConfig(endpointId);
            if (config != null)
            {
                return config.SharedSecret;
            }

            return null;
        }

        public string GetMerchantControlKey(int endpointId)
        {
            MerchantConfig config = this.GetMerchantConfig(endpointId);
            if (config != null)
            {
                return config.MerchantControlKey;
            }

            return null;
        }

        private MerchantConfig GetMerchantConfig(int endpointId)
        {
            MerchantConfig config = MerchantAPI.Data.Cache.GetConfiguration(endpointId);
            if (config == null)
            {
                CommDoo.Configuration.Requests.QueryReqeust queryRequest = new CommDoo.Configuration.Requests.QueryReqeust();
                queryRequest.EndPointID = endpointId + "";
                queryRequest.EndPointGroupID = endpointId + "";
                string commdooResponse = queryRequest.executeRequest();

                CommDoo.Configuration.Responses.Response xmlResponse = CommDoo.Configuration.Responses.Response
                    .DeserializeFromString(commdooResponse);
                if (xmlResponse.Error == null && !string.IsNullOrEmpty(xmlResponse.ClientID) && !string.IsNullOrEmpty(xmlResponse.SharedSecret))
                {
                    config = new MerchantConfig(
                        endpointId,
                        xmlResponse.ClientID,
                        xmlResponse.SharedSecret,
                        xmlResponse.SharedSecret,
                        xmlResponse.FrontendRequired);
                    MerchantAPI.Data.Cache.SetConfiguration(endpointId, config);
                }
            }

            return config;
        }
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

        public TimeSpan CreateConfigurationExpirationTimeSpan()
        {
            return new TimeSpan(0, 0, _settings.ConfigurationExpirationSeconds);
        }
    }
}