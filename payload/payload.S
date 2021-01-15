.thumb
.section .text

.align 4
.rept 0x180
	.word	0x70000000
	.word	0x2000174D
.endr

.rept 0x40
	nop
.endr

main:
	@ Enable UART clock, I/O port B clock, and AFIO clock
	ldr	r0, =0x40021018	@ RCC_APB2ENR
	ldr	r1, [r0]
	ldr	r2, =0x00004009	@ USART1_EN | IOPB_EN | AFIO_EN
	orr	r1, r1, r2
	str	r1, [r0]

	@ Set UART GPIO
	@ PB6: SWIM_RST
	ldr	r0, =0x40010C00	@ GPIO port B registers
	@ fastest output, AF push/pull
	ldr	r2, [r0, #0]	@ GPIOx_CRL
	ldr	r3, =0x0f000000
	bic	r2, r2, r3
	ldr	r3, =0x0b000000
	orr	r2, r2, r3
	str	r2, [r0, #0]

	@ Enable AFIO remap for USART1
	ldr	r0, =0x40010000	@ AFIO registers
	ldr	r2, [r0, #4]	@ AFIO_MAPR
	ldr	r3, =0x4		@ USART1_REMAP
	orr	r2, r2, r3
	str	r2, [r0, #4]

	@ Set up USART1
	ldr	r0, =0x40013800	@ USART1 registers
	ldr	r1, =0x0138	@ 115200 baud @ 36MHz
	str	r1, [r0, #0x08]	@ Set baud rate
	ldr	r1, =0x2008	@ Enable USART and TX
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
	.short	0xff
	b	.
