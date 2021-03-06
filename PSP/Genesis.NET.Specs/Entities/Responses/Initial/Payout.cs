﻿using System;
using System.Linq;
using Genesis.Net.Common;
using Genesis.Net.Entities.Responses.Error;
using Genesis.Net.Entities.Responses.Successful;
using Genesis.Net.Errors;
using NSpec;
using Genesis.Net.Specs.Mocks;

namespace Genesis.Net.Specs.Entities.Responses
{
    class describe_payout_responses : nspec
    {
        void it_should_parse_success_response()
        {
            var successResponseMock = ResponseMocksFactory.CreatePayoutSuccessResponse();
            successResponseMock.Instance.should_be_parsable_from<PayoutSuccessResponse>(successResponseMock.Xml);
        }

        void it_should_parse_error_response()
        {
            var errorResponseMock = ResponseMocksFactory.CreatePayoutErrorResponse();
            errorResponseMock.Instance.should_be_parsable_from<PayoutErrorResponse>(errorResponseMock.Xml);
        }
    }
}