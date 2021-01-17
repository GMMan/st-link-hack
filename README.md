Dumping the ST-LINK/V2-1
========================

This repo contains the code and a writeup regarding my attempt at dumping the
full flash contents of a ST-LINK/V2-1.

Read the writeup [here](https://github.com/GMMan/st-link-hack/blob/master/paper/paper.md)!

**New!** Upgrade your ST-LINK/V2 clone to ST-LINK/V2-1, and some info on the
layout of the ST-LINK firmware and command line options for STLinkUpgrade.
Read [here](https://github.com/GMMan/st-link-hack/blob/master/upgrade/upgrade.md).

Associated code:
- [Host application](host_app): .NET Core application that sends the payload to
  the ST-Link and receives the dump via serial
- [Payload](payload): Runs on the device and sends the flash contents over UART
- [ST-LINK/V2 payload](https://github.com/GMMan/st-link-hack/tree/stlink-v2/payload):
  A different payload for V2 bootloader, outputting on NRST pin

Feel free to [@ me on Twitter](https://twitter.com/GMMan_BZFlag) or open an
issue if you have questions or comments.
