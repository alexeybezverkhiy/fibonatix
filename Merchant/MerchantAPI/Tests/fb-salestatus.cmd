@setlocal
@call setenv.cmd %*

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%:52380/paynet/api/v2/status/250

@set requestData="login=logic&client_orderid=%paramClientOrderId%&orderid=%paramOrderId%&control=d1146ab9-c978-42af-b0f9-ced949d23f5e"

curl %optProxy% --trace-ascii %fileTraceLog% --trace-time --data %requestData% %url%
