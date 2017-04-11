@setlocal
@call setenv.cmd %* skip2nd

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%:52380/paynet/api/v2/payout/250
@set accountNumber=0987654321

@set headerData=Authorization: OAuth realm="",oauth_version="1.0",oauth_consumer_key="merchantlogin",oauth_timestamp="1490718302",oauth_nonce="1eK7t1kIhTx",oauth_signature_method="HMAC-SHA1",oauth_signature="DDhwSEj2e7nZ2a07S5p%2Fizfe3Rg%3D"
@set requestData=account_number=%accountNumber%^&amount=100^&bank_branch=test_branch^&bank_name=test_bank^&client_orderid=%paramClientOrderId%^&currency=USD

curl %optProxy% --trace-ascii %fileTraceLog% --trace-time -H 'Authorization: OAuth realm="",oauth_version="1.0",oauth_consumer_key="merchantlogin",oauth_timestamp="1490718302",oauth_nonce="1eK7t1kIhTx",oauth_signature_method="HMAC-SHA1",oauth_signature="DDhwSEj2e7nZ2a07S5p%2Fizfe3Rg%3D"' --data 'account_number=%accountNumber%^&amount=100^&bank_branch=test_branch^&bank_name=test_bank^&client_orderid=%paramClientOrderId%^&currency=USD^&oauth_version=1.0^&oauth_consumer_key=merchantlogin^&oauth_timestamp=1490718302^&oauth_nonce=1eK7t1kIhTx^&oauth_signature_method=HMAC-SHA1' %url%
