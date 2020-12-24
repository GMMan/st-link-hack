BINPATH	:= ..\gcc-arm-none-eabi-9-2020-q2-update-win32\bin

CC	= ${BINPATH}/arm-none-eabi-gcc
CFLAGS	+= -g -mcpu=cortex-m3 -ffreestanding

LD	= ${BINPATH}/arm-none-eabi-ld
OBJCOPY	= ${BINPATH}/arm-none-eabi-objcopy

.PHONY: all

all: payload.bin

%.bin: %.elf
		$(OBJCOPY) -O binary $< $@

payload.elf: stm32f1x.ld payload.o
		$(LD) -T stm32f1x.ld payload.o -o $@

%.o: %.S
		$(CC) -o $@ $(CFLAGS) -c $<

clean:
		$(RM) *.o *.bin *.elf
