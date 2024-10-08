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

# What do I need to know to do some automation
Optional: Basic knowledge about electricy (if you want to controll low voltage like 5V or 12V don't worry, it won't hit you. ** If you don't know anything about electricy - please don't do anything with 230V+! That may be dangerous! **)
Optional: Basic knowledge about electronic device
Optional: Basic English to read documents/stackoverflow

Raspberry Pi is designed that way, that nothing should happen when you connect incorrect pins or put 5V somewhere. But keep in mind that:
1. Higher voltage than 5V may break Raspberry
2. GPIO pins should control "small" devices only. They shouldn't consume more than 10-20mA. That means if you want to control some external device (like electric strike), you should use external power supply + relay.

## And what is relay
No worries - there is no silly question. Relay is basic element that you're going to use in most of the project. Long story short (for people who knows how it works - please don't kill/hit me), this is small tool that allows to controll circut. It has "three pins" - fist one is something that doesn't change. OOTB first leg is connected to the second one, but you may control it, to create connection between leg two instead. It' pretty simple to understand that using image below (image source: [FAQ – Relay](https://www.glomore.co.in/faq-replay/)). As I mentioned - COM and NC connection is default and you may switch it to COM and NO. 

![Diagram how relay looks like](/relay_faq_1.png)


# ZDoorAutomation
This is very simple project that I prepared to show you, how you can create something using Raspberry PI and .NET. Initial idea was to create tool, that allows to control doors using face recognition. Controller will have photos of allowed people and (using relay) open door using electric strike (I'll describe that later). Once I finished part related to the FaceRecognition, I extended this project to support buttons, thermometer and OLED display. Why not?

## Face Recognition
In the current version, I prepared two flows
1. Add user to the "valid" list:
    - user press button number 4
    - application save user photo to the filesystem (using standard, usb webcam)
2. User want to open door:
    - user press button number 1
    - application grab user photo
    - application get first correct photo from filesystem
    - application sends correct photo and current user photo to AWS ImageRekognition service (FaceCompare)
    - if face is correct, it activate relay that enable electric strike

How to use AWS Face Recognition [Comparing faces in images - Amazon Rekognition](https://docs.aws.amazon.com/rekognition/latest/dg/faces-comparefaces.html)

Simple diagram how physical connection looks like:
![ZDoorAutomation - electric strike](/ZHomeAutomation-ElectricStrike.drawio.png)

This module use matrix buttons to control what application should do. Matrix button connection is showed below
![ZDoorAutomation - matrix buttons](/ZHomeAutomation-MatrixButtons.drawio.png)

## Current temperature (OneWire)
As a part of this project, I prepared simple example, how to integrate with OneWire devices. OneWire is pretty nice serial bus that allows to connect multiple devices that uses one wire to send messages. For project purposes, I decided to use DS18B20 thermometers.
To make it works, you need to activate OneWire interface in your Raspberry. Manual how to do that [Czujnik temperatury DS18B20](https://forbot.pl/blog/kurs-raspberry-pi-czujnik-temperatury-ds18b20-id26430):
1. TL;DR: type 'sudo raspi-config`
2. Interfacing Options/1-Wire, yes

In the diagram below, there is a note how does it work and how to connect it to the Raspberry:
![ZDoorAutomation - thermometer](/ZHomeAutomation-OneWire Temperature.drawio.png)

Next element that I used here is my old I2C OLED display. I2C is serial bus that allows to connect more sophisticated devices that requires two-way connecton. In my project I used OLED display: SSD1306. Why? It's cheap and I used it before. The problem is that I didn't find any official .net package that allows to use it. But I used to work with this device before using old Windows IoT and I managed to re-use core code that was provided by Microsoft. I didn't change anything there. I attached entire code to the project as well.
How to connect I2C SSD1306 OLED display:

![ZDoorAutomation - I2C SSD1306 OLED display](/ZHomeAutomation-I2C OLED.drawio.png)

## Reed Switch


