using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.Serialization;
using MerchantAPI.Models;
using MerchantAPI.Helpers;
using MerchantAPI.Controllers.Factories;

namespace MerchantAPI.CommDoo.BackEnd.Requests
{
    [Serializable()]
    [XmlRoot("Request")]
    public class ReserveAmountRequest : StartRequest
    {
        public static ReserveAmountRequest createRequestByModel(AccountVerificationRequestModel model, int endpointId, string fibonatixId) {
            ReserveAmountRequest request = new ReserveAmountRequest() {
                Client = new ClientData() {
                    ClientID = WebApiConfig.Settings.GetClientID(endpointId),
                    SharedSecret = WebApiConfig.Settings.GetSharedSecret(endpointId),
                },
                Payment = new PaymentData() {
                    PaymentType = "CreditCard",
                    Amount = "100",
                    Currency = "EUR",
                    ReferenceID = model.client_orderid + "-" + DateTime.Now.ToString("yyyyMMddHHmmss.fff"),
                },
                Customer = new CustomerData() {
                    Person = new PersonData() {
                        FirstName = model.first_name,
                        LastName = model.last_name,
                        DateOfBirth = Helpers.CommDooTargetConverter.ConvertBirthdayToString(model.birthday),
                    },
                    Address = new AddressData() {
                        Street = model.address1,
                        PostalCode = model.zip_code,
                        City = model.city,
                        State = model.state,
                        Country = Helpers.CountryConverter.countryAlpha3String(model.country),
                        PhoneNumber = model.phone,
                        EmailAddress = model.email,
                        IPAddress = model.ipaddress,
                    },
                    CreditCard = new CreditCardData() {
                        CreditCardNumber = model.credit_card_number,
                        CreditCardExpirationYear = model.expire_year.ToString(),
                        CreditCardExpirationMonth = model.expire_month.ToString(),
                        CreditCardValidationValue = model.cvv2.ToString(),
                        CreditCardType = Helpers.CommDooTargetConverter.getCardType(model.credit_card_number),
                    }
                }
            };
            if( !String.IsNullOrEmpty(model.server_callback_url) ) {
                request.Notification = new NotificationData() {
                    NotificationURL = CommDooFrontendFactory.ResolveInternalNotificationUrl(endpointId, CommDooFrontendFactory.SUCC_EXTRA_PATH)
                        + CommDooFrontendFactory.CreateNotifyParams(model.server_callback_url, fibonatixId),
                };
            }
            return request;
        }

        public override string executeRequest() {
            string requestURL = WebApiConfig.Settings.BackendServiceUrlMain + "/ReserveAmount";
            return sendRequest(requestURL);
        }
    }
}