@setlocal
call setenv.cmd

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%:52380/paynet/api/v2/status/250
@set /p paramClientOrderId=client_orderid:
@set requestData="login=logic&client_orderid=%paramClientOrderId%&orderid=83ece2b9-53c7-4bd2-9a35-c28123edd243&control=d1146ab9-c978-42af-b0f9-ced949d23f5e"

curl %optProxy% --trace-ascii .\trace-%~n0.log --trace-time --data %requestData% %url%
