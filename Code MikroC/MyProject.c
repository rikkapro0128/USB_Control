// cai dat virtualUSB de mo phong USB tren Proteus
//ve proteus
//tao project tren mikroC
//tao file mo ta USBdsc.c va add vao project
//viet code & test thong qua HID Terminal
//tao giao dien dieu khien tren VS C#.


// khai bao du lieu va bien
#define led_on      1  // muc logic dieu khien trang thai led
#define led_off     0
#define in_size     64 // mac dinh kich thuoc du lieu doc/ghi
#define out_size    64

char count = 0;     //chua so lan nhan SW
bit oldstate;       // co bao trang thai cho SW
unsigned char readbuff[in_size] absolute 0x500;       // bo dem chua du lieu doc/ghi
unsigned char writebuff[out_size] absolute 0x540;

void interrupt()
{
     if (PIR2.USBIF == 1)
     {
        PIR2.USBIF = 0;     // clear interrupt bit
        USB_Interrupt_Proc();  // ISR cho USB
     }
     if (INTCON.INT0IF == 1)
     {
        INTCON.INT0IF = 0;  // clear interrupt bit
        count++;
        writebuff[0] = count;
        oldstate = 1;
     }
}

void main(void)
{
     ADCON1 |= 0x0F;   //tat ca chan analog thanh chan Digital
     CMCON  |= 0x07;   //cam modul Comparators
     // cau hinh port B
     PORTB = 0x00, LATB = 0x00;
     TRISB.TRISB0 = 1;
     INTCON2.RBPU = 0;
     // cau hinh port E
     PORTE = 0x00, LATE = 0x00;
     TRISE.TRISE1 = 0;
     // cau hinh modul USB
     HID_Enable(&readbuff, &writebuff);       // cho phep USB che do HID
     //cau hinh ngat
     INTCON.INT0IF = 0;   // clear interrupt bit
     INTCON.INT0IE = 1;   //Enable external interrupts
     INTCON2.INTEDG0 = 1; // external interrupt on rising edge
     PIR2.USBIF = 0;
     PIE2.USBIE = 1;
     INTCON.GIE = 1;
     INTCON.PEIE = 1;
     while(1)
     {
             if (HID_Read() != 0)     //doc du lieu tu HOST thong qua USB
             {
                if (readbuff[0] == 1)
                {
                   RE1_bit = led_on;  // dieu khien led sang
                   writebuff[8] = 'O';   //gui phan hoi ma bao led sang
                }
                else if (readbuff[0] == 0)
                {
                     RE1_bit = led_off;  //dieu khien led tat
                     writebuff[8] = 'F'; //gui phan hoi ma bao led tat
                }
                HID_Write(&writebuff, out_size);    //gui du lieu HOST thong qua USB
             }
             if (oldstate == 1)
             {
                oldstate = 0;
                HID_Write(&writebuff, out_size);    //gui du lieu HOST thong qua USB
             }
     }
}