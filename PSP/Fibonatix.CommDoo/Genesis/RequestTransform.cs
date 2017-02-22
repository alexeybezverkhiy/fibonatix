using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Genesis.Net.Common;
using Genesis.Net.Entities;
using Genesis.Net.Entities.Requests.Initial;
using Genesis.Net.Entities.Requests.Initial.ThreeD;
using Genesis.Net.Entities.Requests.Query;
using Genesis.Net.Entities.Requests.Referential;
using Genesis.Net.Errors;
using Genesis.Net.Specs;
using Genesis.Net.Specs.Mocks;
using Fibonatix.CommDoo.Requests;
using Fibonatix.CommDoo.Helpers;

namespace Fibonatix.CommDoo.Genesis
{
    internal class RequestTransform
    {
        public static Authorize getGenesisAuthorize(PreauthRequest commDooAuth) { // exception

            commDooAuth.verification(); // exception

            var authorize = new Authorize() {
                Id = commDooAuth.preAuth.transaction.reference_id,
                Usage = commDooAuth.preAuth.transaction.usage,
                Amount = Convertors.MinorAmountToMajor((int)commDooAuth.preAuth.transaction.amount, Currencies.currencyCodeFromString(commDooAuth.preAuth.transaction.currency)),
                Currency = Currencies.currencyCodeFromString(commDooAuth.preAuth.transaction.currency),
                RemoteIp = commDooAuth.preAuth.transaction.customer_data != null ? commDooAuth.preAuth.transaction.customer_data.ipaddress : "127.0.0.1",
                CardHolder = commDooAuth.preAuth.transaction.cred_card_data.cardholder_name,
                ExpirationMonth = commDooAuth.preAuth.transaction.cred_card_data.expiration_month,
                ExpirationYear = commDooAuth.preAuth.transaction.cred_card_data.expuration_year,
                CustomerEmail = commDooAuth.preAuth.transaction.customer_data != null ? commDooAuth.preAuth.transaction.customer_data.email : null,
                CustomerPhone = commDooAuth.preAuth.transaction.customer_data != null ? commDooAuth.preAuth.transaction.customer_data.phone : null,
                CardNumber = commDooAuth.preAuth.transaction.cred_card_data.credit_card_number,
                Cvv = commDooAuth.preAuth.transaction.cred_card_data.cvv,
                
                BillingAddress = new Address()
                {
                    Address1 = commDooAuth.preAuth.transaction.customer_data != null ? commDooAuth.preAuth.transaction.customer_data.street : null,
                    Address2 = null,
                    City = commDooAuth.preAuth.transaction.customer_data != null ? commDooAuth.preAuth.transaction.customer_data.city : null,
                    Country = commDooAuth.preAuth.transaction.customer_data != null ? Countries.countryCodeFromString(commDooAuth.preAuth.transaction.customer_data.country) : Iso3166CountryCodes.IL,
                    FirstName = commDooAuth.preAuth.transaction.customer_data != null ? commDooAuth.preAuth.transaction.customer_data.firstname : null,
                    LastName = commDooAuth.preAuth.transaction.customer_data != null ? commDooAuth.preAuth.transaction.customer_data.lastname : null,
                    State = commDooAuth.preAuth.transaction.customer_data != null ? commDooAuth.preAuth.transaction.customer_data.state : null,
                    ZipCode = commDooAuth.preAuth.transaction.customer_data != null ? commDooAuth.preAuth.transaction.customer_data.postalcode : null,
                },
                /*
                ShippingAddress = new Address()
                {
                    Address1 = "shipping address1",
                    Address2 = "shipping address2",
                    City = "shipping city",
                    Country = Iso3166CountryCodes.BG,
                    FirstName = "shipping first name",
                    LastName = "shipping last name",
                    State = "BS",
                    ZipCode = "1000"
                },
                RiskParams = new RiskParams()
                {
                    Email = "hello@world.com",
                    MacAddress = "mac address",
                    Phone = "phone",
                    RemoteIp = "255.10.100.10",
                    SerialNumber = "serial number",
                    SessionId = "session id",
                    Ssn = "ssn",
                    UserId = "user id",
                    UserLevel = "user level"
                },
                DynamicDescriptorParams = new DynamicDescriptor()
                {
                    MerchantName = "Test Merchant",
                    MerchantCity = "Testing Town"
                }
                */
            };
            return authorize;
        }

        public static Capture getGenesisCapture(CaptureRequest commDooCapture) { // exception

            commDooCapture.verification(); // exception

            var capture = new Capture() {
                Amount = Convertors.MinorAmountToMajor((int)commDooCapture.capture.transaction.amount, Currencies.currencyCodeFromString(commDooCapture.capture.transaction.currency)),
                Currency = Currencies.currencyCodeFromString(commDooCapture.capture.transaction.currency),
                Id = commDooCapture.capture.transaction.reference_id,
                ReferenceId = commDooCapture.capture.transaction.provider_transaction_id,
                RemoteIp = "127.0.0.1",
                Usage = commDooCapture.capture.transaction.usage
            };
            return capture;
        }

        public static Sale getGenesisSale(PurchaseRequest commDooPurchase) { // exception

            commDooPurchase.verification(); // exception

            var sale = new Sale() {
                Id = commDooPurchase.purchase.transaction.reference_id,
                Usage = commDooPurchase.purchase.transaction.usage,
                Amount = Convertors.MinorAmountToMajor((int)commDooPurchase.purchase.transaction.amount, Currencies.currencyCodeFromString(commDooPurchase.purchase.transaction.currency)),
                Currency = Currencies.currencyCodeFromString(commDooPurchase.purchase.transaction.currency),
                RemoteIp = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.ipaddress : "127.0.0.1",
                CardHolder = commDooPurchase.purchase.transaction.cred_card_data.cardholder_name,
                ExpirationMonth = commDooPurchase.purchase.transaction.cred_card_data.expiration_month,
                ExpirationYear = commDooPurchase.purchase.transaction.cred_card_data.expuration_year,
                CustomerEmail = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.email : null,
                CustomerPhone = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.phone : null,
                CardNumber = commDooPurchase.purchase.transaction.cred_card_data.credit_card_number,
                Cvv = commDooPurchase.purchase.transaction.cred_card_data.cvv,

                BillingAddress = new Address() {
                    Address1 = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.street : null,
                    Address2 = null,
                    City = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.city : null,
                    Country = commDooPurchase.purchase.transaction.customer_data != null ? Countries.countryCodeFromString(commDooPurchase.purchase.transaction.customer_data.country) : Iso3166CountryCodes.IL,
                    FirstName = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.firstname : null,
                    LastName = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.lastname : null,
                    State = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.state : null,
                    ZipCode = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.postalcode : null,
                },
                /*
                ShippingAddress = new Address() {
                    Address1 = "shipping address1",
                    Address2 = "shipping address2",
                    City = "shipping city",
                    Country = Iso3166CountryCodes.BG,
                    FirstName = "shipping first name",
                    LastName = "shipping last name",
                    State = "BS",
                    ZipCode = "1000"
                },
                RiskParams = new RiskParams() {
                    Email = "hello@world.com",
                    MacAddress = "mac address",
                    Phone = "phone",
                    RemoteIp = "255.10.100.10",
                    SerialNumber = "serial number",
                    SessionId = "session id",
                    Ssn = "ssn",
                    UserId = "user id",
                    UserLevel = "user level"
                },
                DynamicDescriptorParams = new DynamicDescriptor() {
                    MerchantName = "Test Merchant",
                    MerchantCity = "Testing Town"
                }
                */
            };
            return sale;
        }

        public static InitRecurringSale getGenesisInitRecurringSale(PurchaseRequest commDooPurchase) { // exception

            commDooPurchase.verification(); // exception

            var sale = new InitRecurringSale() {
                Id = commDooPurchase.purchase.transaction.reference_id,
                Usage = commDooPurchase.purchase.transaction.usage,
                Amount = Convertors.MinorAmountToMajor((int)commDooPurchase.purchase.transaction.amount, Currencies.currencyCodeFromString(commDooPurchase.purchase.transaction.currency)),
                Currency = Currencies.currencyCodeFromString(commDooPurchase.purchase.transaction.currency),
                RemoteIp = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.ipaddress : "127.0.0.1",
                CardHolder = commDooPurchase.purchase.transaction.cred_card_data.cardholder_name,
                ExpirationMonth = commDooPurchase.purchase.transaction.cred_card_data.expiration_month,
                ExpirationYear = commDooPurchase.purchase.transaction.cred_card_data.expuration_year,
                CustomerEmail = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.email : null,
                CustomerPhone = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.phone : null,
                CardNumber = commDooPurchase.purchase.transaction.cred_card_data.credit_card_number,
                Cvv = commDooPurchase.purchase.transaction.cred_card_data.cvv,

                BillingAddress = new Address() {
                    Address1 = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.street : null,
                    Address2 = null,
                    City = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.city : null,
                    Country = commDooPurchase.purchase.transaction.customer_data != null ? Countries.countryCodeFromString(commDooPurchase.purchase.transaction.customer_data.country) : Iso3166CountryCodes.IL,
                    FirstName = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.firstname : null,
                    LastName = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.lastname : null,
                    State = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.state : null,
                    ZipCode = commDooPurchase.purchase.transaction.customer_data != null ? commDooPurchase.purchase.transaction.customer_data.postalcode : null,
                },
                /*
                ShippingAddress = new Address() {
                    Address1 = "shipping address1",
                    Address2 = "shipping address2",
                    City = "shipping city",
                    Country = Iso3166CountryCodes.BG,
                    FirstName = "shipping first name",
                    LastName = "shipping last name",
                    State = "BS",
                    ZipCode = "1000"
                },
                RiskParams = new RiskParams() {
                    Email = "hello@world.com",
                    MacAddress = "mac address",
                    Phone = "phone",
                    RemoteIp = "255.10.100.10",
                    SerialNumber = "serial number",
                    SessionId = "session id",
                    Ssn = "ssn",
                    UserId = "user id",
                    UserLevel = "user level"
                },
                DynamicDescriptorParams = new DynamicDescriptor() {
                    MerchantName = "Test Merchant",
                    MerchantCity = "Testing Town"
                }
                */
            };
            return sale;
        }

        public static RecurringSale getGenesisRecurringSale(PurchaseRequest commDooPurchase) { // exception

            commDooPurchase.verification(); // exception

            var sale = new RecurringSale() {
                Id = commDooPurchase.purchase.transaction.reference_id,
                Usage = commDooPurchase.purchase.transaction.usage,
                Amount = Convertors.MinorAmountToMajor((int)commDooPurchase.purchase.transaction.amount, Currencies.currencyCodeFromString(commDooPurchase.purchase.transaction.currency)),
                Currency = Currencies.currencyCodeFromString(commDooPurchase.purchase.transaction.currency),
                RemoteIp = "127.0.0.1",
                ReferenceId = commDooPurchase.purchase.transaction.provider_transaction_id
            };
            return sale;
        }

        public static Refund getGenesisRefund(RefundRequest commDooRefund) { // exception

            commDooRefund.verification(); // exception

            var refund = new Refund() {
                Amount = Convertors.MinorAmountToMajor((int)commDooRefund.refund.transaction.amount, Currencies.currencyCodeFromString(commDooRefund.refund.transaction.currency)),
                Currency = Currencies.currencyCodeFromString(commDooRefund.refund.transaction.currency),
                Id = commDooRefund.refund.transaction.reference_id,
                ReferenceId = commDooRefund.refund.transaction.provider_transaction_id,
                RemoteIp = "127.0.0.1",
                Usage = commDooRefund.refund.transaction.usage
            };
            return refund;
        }

        public static VoidRequest getGenesisVoid(ReversalRequest commDooReversal) { // exception

            commDooReversal.verification(); // exception

            var voidReq = new VoidRequest() {
                Id = commDooReversal.reversal.transaction.reference_id,
                ReferenceId = commDooReversal.reversal.transaction.provider_transaction_id,
                RemoteIp = "127.0.0.1",
                Usage = "",
            };
            return voidReq;
        }


        public static void getGenesisEnrollmentCheck3D(EnrollmentCheck3DRequest enrollment3D) { // exception
            throw new System.ComponentModel.DataAnnotations.ValidationException("Function not supported").SetCode((int)ErrorCodes.WorkflowError);
        }

        public static Authorize3dAsync getGenesisAuthorize3D(Preauth3DRequest commDooAuth3D) { // exception

            commDooAuth3D.verification(); // exception

            var authorize = new Authorize3dAsync() {
                Id = commDooAuth3D.preAuth3D.transaction.reference_id,
                Usage = commDooAuth3D.preAuth3D.transaction.usage,
                Amount = Convertors.MinorAmountToMajor((int)commDooAuth3D.preAuth3D.transaction.amount, Currencies.currencyCodeFromString(commDooAuth3D.preAuth3D.transaction.currency)),
                Currency = Currencies.currencyCodeFromString(commDooAuth3D.preAuth3D.transaction.currency),
                RemoteIp = commDooAuth3D.preAuth3D.transaction.customer_data != null ? commDooAuth3D.preAuth3D.transaction.customer_data.ipaddress : "127.0.0.1",
                CardHolder = commDooAuth3D.preAuth3D.transaction.cred_card_data.cardholder_name,
                ExpirationMonth = commDooAuth3D.preAuth3D.transaction.cred_card_data.expiration_month,
                ExpirationYear = commDooAuth3D.preAuth3D.transaction.cred_card_data.expuration_year,
                CustomerEmail = commDooAuth3D.preAuth3D.transaction.customer_data != null ? commDooAuth3D.preAuth3D.transaction.customer_data.email : null,
                CustomerPhone = commDooAuth3D.preAuth3D.transaction.customer_data != null ? commDooAuth3D.preAuth3D.transaction.customer_data.phone : null,
                CardNumber = commDooAuth3D.preAuth3D.transaction.cred_card_data.credit_card_number,
                Cvv = commDooAuth3D.preAuth3D.transaction.cred_card_data.cvv,
                NotificationUrl = commDooAuth3D.preAuth3D.transaction.communication3D.notification_url,
                ReturnSuccessUrl = commDooAuth3D.preAuth3D.transaction.communication3D.success_url,
                ReturnFailureUrl = commDooAuth3D.preAuth3D.transaction.communication3D.fail_url,

                BillingAddress = new Address() {
                    Address1 = commDooAuth3D.preAuth3D.transaction.customer_data != null ? commDooAuth3D.preAuth3D.transaction.customer_data.street : null,
                    Address2 = null,
                    City = commDooAuth3D.preAuth3D.transaction.customer_data != null ? commDooAuth3D.preAuth3D.transaction.customer_data.city : null,
                    Country = commDooAuth3D.preAuth3D.transaction.customer_data != null ? Countries.countryCodeFromString(commDooAuth3D.preAuth3D.transaction.customer_data.country) : Iso3166CountryCodes.IL,
                    FirstName = commDooAuth3D.preAuth3D.transaction.customer_data != null ? commDooAuth3D.preAuth3D.transaction.customer_data.firstname : null,
                    LastName = commDooAuth3D.preAuth3D.transaction.customer_data != null ? commDooAuth3D.preAuth3D.transaction.customer_data.lastname : null,
                    State = commDooAuth3D.preAuth3D.transaction.customer_data != null ? commDooAuth3D.preAuth3D.transaction.customer_data.state : null,
                    ZipCode = commDooAuth3D.preAuth3D.transaction.customer_data != null ? commDooAuth3D.preAuth3D.transaction.customer_data.postalcode : null,
                },
                /*
                ShippingAddress = new Address()
                {
                    Address1 = "shipping address1",
                    Address2 = "shipping address2",
                    City = "shipping city",
                    Country = Iso3166CountryCodes.BG,
                    FirstName = "shipping first name",
                    LastName = "shipping last name",
                    State = "BS",
                    ZipCode = "1000"
                },
                RiskParams = new RiskParams()
                {
                    Email = "hello@world.com",
                    MacAddress = "mac address",
                    Phone = "phone",
                    RemoteIp = "255.10.100.10",
                    SerialNumber = "serial number",
                    SessionId = "session id",
                    Ssn = "ssn",
                    UserId = "user id",
                    UserLevel = "user level"
                },
                DynamicDescriptorParams = new DynamicDescriptor()
                {
                    MerchantName = "Test Merchant",
                    MerchantCity = "Testing Town"
                }
                */
            };
            return authorize;
        }

        public static Sale3dAsync getGenesisSale3D(Purchase3DRequest commDooPurchase) { // exception

            commDooPurchase.verification(); // exception

            var sale = new Sale3dAsync() {
                Id = commDooPurchase.purchase3D.transaction.reference_id,
                Usage = commDooPurchase.purchase3D.transaction.usage,
                Amount = Convertors.MinorAmountToMajor((int)commDooPurchase.purchase3D.transaction.amount, Currencies.currencyCodeFromString(commDooPurchase.purchase3D.transaction.currency)),
                Currency = Currencies.currencyCodeFromString(commDooPurchase.purchase3D.transaction.currency),
                RemoteIp = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.ipaddress : "127.0.0.1",
                CardHolder = commDooPurchase.purchase3D.transaction.cred_card_data.cardholder_name,
                ExpirationMonth = commDooPurchase.purchase3D.transaction.cred_card_data.expiration_month,
                ExpirationYear = commDooPurchase.purchase3D.transaction.cred_card_data.expuration_year,
                CustomerEmail = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.email : null,
                CustomerPhone = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.phone : null,
                CardNumber = commDooPurchase.purchase3D.transaction.cred_card_data.credit_card_number,
                Cvv = commDooPurchase.purchase3D.transaction.cred_card_data.cvv,
                NotificationUrl = commDooPurchase.purchase3D.transaction.communication3D.notification_url,
                ReturnSuccessUrl = commDooPurchase.purchase3D.transaction.communication3D.success_url,
                ReturnFailureUrl = commDooPurchase.purchase3D.transaction.communication3D.fail_url,

                BillingAddress = new Address() {
                    Address1 = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.street : null,
                    Address2 = null,
                    City = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.city : null,
                    Country = commDooPurchase.purchase3D.transaction.customer_data != null ? Countries.countryCodeFromString(commDooPurchase.purchase3D.transaction.customer_data.country) : Iso3166CountryCodes.IL,
                    FirstName = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.firstname : null,
                    LastName = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.lastname : null,
                    State = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.state : null,
                    ZipCode = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.postalcode : null,
                },
                /*
                ShippingAddress = new Address() {
                    Address1 = "shipping address1",
                    Address2 = "shipping address2",
                    City = "shipping city",
                    Country = Iso3166CountryCodes.BG,
                    FirstName = "shipping first name",
                    LastName = "shipping last name",
                    State = "BS",
                    ZipCode = "1000"
                },
                RiskParams = new RiskParams() {
                    Email = "hello@world.com",
                    MacAddress = "mac address",
                    Phone = "phone",
                    RemoteIp = "255.10.100.10",
                    SerialNumber = "serial number",
                    SessionId = "session id",
                    Ssn = "ssn",
                    UserId = "user id",
                    UserLevel = "user level"
                },
                DynamicDescriptorParams = new DynamicDescriptor() {
                    MerchantName = "Test Merchant",
                    MerchantCity = "Testing Town"
                }
                */
            };
            return sale;
        }

        public static InitRecurringSale3dAsync getGenesisInitRecurring3DSale(Purchase3DRequest commDooPurchase) { // exception

            commDooPurchase.verification(); // exception

            var sale = new InitRecurringSale3dAsync() {
                Id = commDooPurchase.purchase3D.transaction.reference_id,
                Usage = commDooPurchase.purchase3D.transaction.usage,
                Amount = Convertors.MinorAmountToMajor((int)commDooPurchase.purchase3D.transaction.amount, Currencies.currencyCodeFromString(commDooPurchase.purchase3D.transaction.currency)),
                Currency = Currencies.currencyCodeFromString(commDooPurchase.purchase3D.transaction.currency),
                RemoteIp = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.ipaddress : "127.0.0.1",
                CardHolder = commDooPurchase.purchase3D.transaction.cred_card_data.cardholder_name,
                ExpirationMonth = commDooPurchase.purchase3D.transaction.cred_card_data.expiration_month,
                ExpirationYear = commDooPurchase.purchase3D.transaction.cred_card_data.expuration_year,
                CustomerEmail = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.email : null,
                CustomerPhone = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.phone : null,
                CardNumber = commDooPurchase.purchase3D.transaction.cred_card_data.credit_card_number,
                Cvv = commDooPurchase.purchase3D.transaction.cred_card_data.cvv,
                NotificationUrl = commDooPurchase.purchase3D.transaction.communication3D.notification_url,
                ReturnSuccessUrl = commDooPurchase.purchase3D.transaction.communication3D.success_url,
                ReturnFailureUrl = commDooPurchase.purchase3D.transaction.communication3D.fail_url,

                BillingAddress = new Address() {
                    Address1 = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.street : null,
                    Address2 = null,
                    City = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.city : null,
                    Country = commDooPurchase.purchase3D.transaction.customer_data != null ? Countries.countryCodeFromString(commDooPurchase.purchase3D.transaction.customer_data.country) : Iso3166CountryCodes.IL,
                    FirstName = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.firstname : null,
                    LastName = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.lastname : null,
                    State = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.state : null,
                    ZipCode = commDooPurchase.purchase3D.transaction.customer_data != null ? commDooPurchase.purchase3D.transaction.customer_data.postalcode : null,
                },
                /*
                ShippingAddress = new Address() {
                    Address1 = "shipping address1",
                    Address2 = "shipping address2",
                    City = "shipping city",
                    Country = Iso3166CountryCodes.BG,
                    FirstName = "shipping first name",
                    LastName = "shipping last name",
                    State = "BS",
                    ZipCode = "1000"
                },
                RiskParams = new RiskParams() {
                    Email = "hello@world.com",
                    MacAddress = "mac address",
                    Phone = "phone",
                    RemoteIp = "255.10.100.10",
                    SerialNumber = "serial number",
                    SessionId = "session id",
                    Ssn = "ssn",
                    UserId = "user id",
                    UserLevel = "user level"
                },
                DynamicDescriptorParams = new DynamicDescriptor() {
                    MerchantName = "Test Merchant",
                    MerchantCity = "Testing Town"
                }
                */
            };
            return sale;
        }

        public static SingleReconcile getSingleReconcile(SingleReconcileRequest commDooReconcile) {

            commDooReconcile.verification(); // exception

            var sale = new SingleReconcile() {
                UniqueId = commDooReconcile.reconcile.transaction.provider_transaction_id,                
            };
            return sale;
        }
    }
}
