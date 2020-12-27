# TemperatureMonitor

Temperature Monitor 10 reads temperatures from DS18B20 sensors in grain bins. It consists of a server and multiple clients (controlboxes). The server is installed on a PC on the LAN. Each controlbox has a Wemos D1 Mini to send data over wifi to the server. TM10 is designed to only allow one controlbox to send at a time to improve reliability of receiving the data. Another way to improve reliability is to use only short strings of sensors, one controlbox for every grain bin. The controlboxes are powered by 24V DC.

On startup if a network connection can't be made a web page is started at 192.168.4.1 . Here the network name, password, unique controlbox ID (0-255) and sleep interval (0-1439 minutes) can be set. The sketch can be updated with Over-The-Air wireless update.  
  
Hookup Schematic:
<img src="https://user-images.githubusercontent.com/13328609/101184844-cc516980-3616-11eb-823c-0e8157ac7562.PNG" width="100%"></img> 
