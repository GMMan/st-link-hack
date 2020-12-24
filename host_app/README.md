Host application for dumping ST-LINK/V2-1
=========================================

This is the .NET Core app for dumping the firmware.

It has dependencies on Device.Net. My version was patched to resolve a problem
with freeing devices, but I don't have it uploaded anywhere and it's behind
the current version of Device.Net as of this writing, so you will have to source
your own version.

To use, provide payload path, serial port path, stack address you want to start
overwriting at, and where you want your dumped file to be saved to. For example:

```
dotnet StLinkHack.dll ..\payload\payload.bin COM6 0x20001c9c firmware.bin
```

Be sure to plug in your ST-Link and UART adapter before running.
