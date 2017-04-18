@setlocal
@call setenv.cmd skipAll

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%/paynet/api/v2/mgmt/cache

@set requestData=""

curl %optProxy% --trace-ascii %fileTraceLog% --trace-time %url%
