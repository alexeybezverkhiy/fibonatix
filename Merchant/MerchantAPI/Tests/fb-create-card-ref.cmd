@setlocal
@call setenv.cmd %*

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%/paynet/api/v2/create-card-ref/250
@set requestData="client_orderid=%paramClientOrderId%&login=logic&orderid=%paramOrderId%&control=fade000000000000000000000000000000000000"

curl --trace-ascii %fileTraceLog% --trace-time --data %requestData% %url%
