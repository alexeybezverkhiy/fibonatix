@setlocal
@call setenv.cmd %* skip2nd

@set fileTraceLog=%~n0.trace.log
@set url=http://%targetHost%/paynet/api/v2/sale/250

@set requestData="client_orderid=%paramClientOrderId%&order_desc=Test+Order+Description&first_name=John&last_name=Smith&ssn=1267&birthday=19820115&address1=100+Main+st&city=Seattle&state=WA&zip_code=98102&country=US&phone=%2b12063582043&cell_phone=%2b19023384543&amount=11.33&email=john.smith@gmail.com&currency=USD&ipaddress=65.153.12.232&site_url=www.google.com&credit_card_number=4538977399606732&card_printed_name=CARD HOLDER&expire_month=12&expire_year=2099&cvv2=123&purpose=www.twitch.tv/dreadztv&redirect_url=http://%targetHost%:52380/redirect-emulator&server_callback_url=http://%targetHost%:52380/callback-emulator&merchant_data=VIP+customer&control=709be79dd85191fad169fb8082927b9842e87200"

curl %optProxy% --trace-ascii %fileTraceLog% --trace-time --data %requestData% %url%
