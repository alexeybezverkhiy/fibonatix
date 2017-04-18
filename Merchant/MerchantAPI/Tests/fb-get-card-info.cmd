@setlocal
@call setenv.cmd skipAll

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%/paynet/api/v2/get-card-info/250
@set requestData="login=logic&cardrefid=111000111&control=fade000000000000000000000000000000000000"

curl --trace-ascii %fileTraceLog% --trace-time --data %requestData% %url%
