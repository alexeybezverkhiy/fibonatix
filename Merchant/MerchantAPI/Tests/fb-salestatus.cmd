@setlocal
call setenv.cmd

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%:52380/paynet/api/v2/status/250
@set requestData="login=logic&client_orderid=inv0001001&orderid=4849d60c-0a8c-49a0-b946-cb6c94c14fa9&control=d1146ab9-c978-42af-b0f9-ced949d23f5e"

curl --trace-ascii .\trace-%~n0.log --trace-time --data %requestData% %url%
