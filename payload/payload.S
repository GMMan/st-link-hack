.thumb
.section .text

.align 4
.rept 0x180
	.word	0x70000000
	.word	0x20000f81
.endr

.rept 0x40
	nop
.endr

main:
	@ Enable UART clock
	ldr	r0, =0x4002101c
	ldr	r1, [r0]
	ldr	r2, =0x00020000
	orr	r1, r1, r2
	str	r1, [r0]

	@ Set UART GPIO
	ldr	r0, =0x40010800	@ GPIO port A registers
	ldr	r2, [r0]
	ldr	r3, =0x00000f00
	bic	r2, r2, r3
	ldr	r3, =0xb00
	orr	r2, r2, r3
	str	r2, [r0]

	@ Set up USART2
	ldr	r0, =0x40004400	@ USART2 registers
	ldr	r1, =0x0138	@ 115200 baud @ 36MHz
	str	r1, [r0, #0x08]	@ Set baud rate
	ldr	r1, =0x2008
	str	r1, [r0, #0x0c]

	@ Send data
	ldr	r1, ='s'
	bl	send_byte
	ldr	r1, ='t'
	bl	send_byte
	ldr	r1, ='a'
	bl	send_byte
	ldr	r1, ='r'
	bl	send_byte
	ldr	r1, ='t'
	bl	send_byte

	ldr	r3, =0x08000000
	ldr	r4, =0x08020000
1:
	ldrb	r1, [r3]
	bl	send_byte
	add	r3, r3, #1
	cmp	r3, r4
	bne	1b

	b	.

send_byte:
	str	r1, [r0, #0x04]
1:
	ldr	r1, [r0, #0x00]
	lsl	r1, r1, #0x18
	bpl	1b
	bx	lr
		

trailer:
	.short	0xff
	b	.