@setlocal
@call setenv.cmd %* skip2nd

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%/paynet/api/v2/sync/make-rebill-sale/250
@set requestData="login=logic&client_orderid=%paramClientOrderId%&cardrefid=111000111&amount=5.11&currency=EUR=&cvv2=321&ipaddress=34.129.65.12&comment=Information+about+Rebill=&order_desc=Rebill+order+description&control=fade000000000000000000000000000000000000"

curl --trace-ascii %fileTraceLog% --trace-time --data %requestData% %url%
