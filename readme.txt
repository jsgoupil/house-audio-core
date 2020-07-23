Install cmake, it has to be present to get the serialportstream program
sudo ./bootstrap
sudo make
https://askubuntu.com/questions/904948/cmake-has-been-installed-but-when-other-program-need-to-use-it-it-still-says-no

Use https://github.com/jcurl/serialportstream
Install 
$ git clone https://github.com/jcurl/serialportstream.git
$ cd serialportstream/dll/serialunix
$ ./build.sh

Put the export command in .bashrc
export LD_LIBRARY_PATH=/home/bitcoin/serialportstream/dll/serialunix/bin/usr/local/lib:$LD_LIBRARY_PATH

The export has to run on sudo as well if you get this error:
Unable to load shared library 'libnserial.so.1' or one of its dependencies.


See all running processes
ps aux

Publish for dotnet linux
dotnet publish --self-contained --runtime linux-arm

Change the permission for the server file
chmod +x myWebApp

Run the web server
./AudioCoreApi --server.urls http://0.0.0.0:5001

Serial Port is on /dev/ttyUSB0.

If the permission is denied.
Add the user to dialout  so it can talk to the ttyUSB0
sudo usermod -a -G dialout $USER
Restart console.

Change /etc/rc.conf to add the service serial-server
Drop serial-server in /etc/rc.d



Run with content
/home/bitcoin/serial-server/AudioCoreApi --server.urls http://0.0.0.0:5001 --contentRoot /home/bitcoin/serial-server/

