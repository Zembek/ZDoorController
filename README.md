# Let's have fun with Raspberry Pi automation
Keep in mind this is something that I prepared based on my limited time and knowledge, but I hope that will be useful for you to convince you, that RPi is pretty awesome (and powerful) device!
I didn't prepare that for any commercial use - it's just my hobby that I'd like to share with you.

# But why Raspberry Pi?
I used to be .net developer and this language is first thing that I always have in my mind. I'm not crossing out Java, TypeScript or Python, but .net is something that I always have on my laptop and I can easily prepare something with limited amout of time that I have. And to be honest - Raspberry Pi that was something that changed idea of automation. It used to be complicated and became available for everyone that knows something about application development. You don't need to know low-level languages or try to figure it out why C liberary doesn't work. You can use any modern language, it'll be supported and have dozens of available ready-to-use packages. And Raspberry Pi price is affordable.

# How to start?
It's simple - buy Raspberry pi (I use 4B version), charger (it uses usb-c port, I recommend 5V, 3A at least), micro-sd card (class 10, 32gb min), mini hdmi to standard hdmi dongle, some old keyboard, mouse and screen ant that's all. Everything is here: [Getting started](https://www.raspberrypi.com/documentation/computers/getting-started.html).
To "setup" OS to the RPi you should use dedicated tool called [Raspberry Pi Imager](https://www.raspberrypi.com/documentation/computers/getting-started.html#raspberry-pi-imager). It allows you to create selected OS on the microsd card (it's ultra simple, don't worry). I use Raspbian (Raspberry Pi OS) - it's Debian based OS, that contains tons of manuals or guides in the Internet.

If you would like to create .net core apps (like I do), you should setup .NET sdk - currently I use .NET 8. Please check website to get more details how to setup it: [How to setup .NET 8](https://www.petecodes.co.uk/install-and-use-microsoft-dot-net-8-with-the-raspberry-pi/).
Once you setup .net (and restart RPi), you can build and execute .net based apps - that's it.

# How to debug
This is tricky question actually. Most of the manuals in the internet are pretty complicated (takes time to setup) or offers limited features. As I mentioned before - I don't have time and I like to debug apps like I used to do that in the standard Visual Studio. And I'm lazy :)
How do I debug then?
1. Create code repository (it's pretty obvious)
2. Connect to the RPi using ssh :)
3. Follow ssh debug manual [Setup SSH Debug](https://learn.microsoft.com/en-us/dotnet/iot/debugging?tabs=self-contained&pivots=vscode)
    - TL;DR - run command: 
    `curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l ~/vsdbg`
    - create directory .vs-debugger under homepage 
    `mkdir ~/.vs-debugger`
4. Clone repo on the Raspberry Pi
5. Build code on Raspberry Pi directly (dotnet build)
6. Run the code, open visualstudio, press Debug/Attach To Process, select SSH from "Connection type" dropdown and put RPi details (ip address, user, password)
7. That's all - you have debug active.

TIP: I always create file called "buldDebug.sh" that have steps in one place

