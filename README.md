Dumping the ST-LINK/V2-1
========================

This repo contains the code and a writeup regarding my attempt at dumping the
full flash contents of a ST-LINK/V2-1.

Read the writeup [here](paper/paper.md)!

Associated code:
- [Host application](host_app): .NET Core application that sends the payload to
  the ST-Link and receives the dump via serial
- [Payload](payload): Runs on the device and sends the flash contents over UART

Feel free to [@ me on Twitter](https://twitter.com/GMMan_BZFlag) or open an
issue if you have questions or comments.
