@setlocal
@call setenv.cmd skipAll

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%/paynet/api/v2/mgmt/settings

@set requestData=""

curl %optProxy% --trace-ascii %fileTraceLog% --trace-time %url%
