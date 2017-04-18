@set targetHost=localhost:8060
::@set targetHost=5.149.150.98:8060

@rem Fiddler settings
@set optProxy=--proxy localhost:8888

@if "%1" == "skipAll" goto :skipAll
@if "%1" == "skip2nd" goto :only1st

:bothParam
@if "%1" == "" (@set /p paramClientOrderId="client_orderid:") else (@set paramClientOrderId=%1)
@if "%2" == "" (@set /p paramOrderId="orderid:") else (@set paramOrderId=%2)
@goto :eof

:only1st
@set /p paramClientOrderId="client_orderid:"
@goto :eof

:skipAll
@goto :eof
