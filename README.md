# TemperatureMonitor
Temperature monitor for grain bins.
Temperature Monitor 10 reads temperatures from DS18B20 sensors in grain bins. It consists of a server and multiple clients (controlboxes). The server is installed on a PC on the LAN. The controlboxes send the data over wifi to the server. TM10 is designed to only allow one controlbox to send at a time to improve reliability of receiving the data. Another way to improve reliability is to use only short strings of sensors. One controlbox for every two grain bins. The controlboxes are powered by 24V over cat5 cable.

  For each controlbox to initially connect to wifi, WifiManager https://github.com/tzapu/WiFiManager is used. Once setup it will autoconnect to the wifi. When in the WifiManger setup page the temperatures can be accessed using the "Info" button.
  
Hookup Schematic:
 <img src="https://user-images.githubusercontent.com/13328609/100530908-694a7780-31bd-11eb-8ba6-38418df723d5.PNG" width="90%"></img>
 
 POE pinout:
 
 <img src="https://user-images.githubusercontent.com/13328609/100530910-70718580-31bd-11eb-92cc-305c35fecb62.PNG" width="30%"></img>  
