::@set targetHost=localhost
@set targetHost=5.149.150.98

@rem Fiddler settings
@set optProxy=--proxy localhost:8888

@if "%1" == "skipAll" goto :eof

@if "%1" == "" (@set /p paramClientOrderId="client_orderid:") else (@set paramClientOrderId=%1)
@if "%2" == "" (@set /p paramOrderId="orderid:") else (@set paramOrderId=%2)
