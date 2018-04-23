using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

//using Microsoft.SPOT.Hardware;

namespace STM32F4DISC_Test
{
    public class Program
    {

        public static void Main()
        {
            //OutputPort ledPort = new OutputPort((Cpu.Pin)62, false);

            //OutputPort resetPort = new OutputPort((Cpu.Pin)26, true);

            OutputPort resetPort = new OutputPort(Cpu.Pin.GPIO_Pin2, true);

            //OutputPort reset1rt = new OutputPort((Cpu.Pin)24, true);


            I2CDevice.Configuration config = new I2CDevice.Configuration(121, 400);
            I2CDevice i2C = new I2CDevice(config);

            resetPort.Write(true);
            Thread.Sleep(1000);
            resetPort.Write(false);
            Thread.Sleep(1000);
            resetPort.Write(true);
            Thread.Sleep(1000);

            WriteInit(i2C);

            OutputPort led = new OutputPort((Cpu.Pin)57, true);
            bool state = true;

            while (true)
            {
                led.Write(state);

                state = !state;

                Thread.Sleep(500);
            }
        }

        private static void GetPorts()
        {
            HardwareProvider hwProvider = HardwareProvider.HwProvider;

            if (hwProvider != null)
            {
                Cpu.Pin scl;
                Cpu.Pin sda;

                hwProvider.GetI2CPins(out scl, out sda);

                if (scl != Cpu.Pin.GPIO_NONE)
                {
                    Port.ReservePin(scl, true);
                }

                if (sda != Cpu.Pin.GPIO_NONE)
                {
                    Port.ReservePin(sda, true);
                }
            }
        }

        private static void WriteInit(I2CDevice i2C)
        {
            write_i(0xAE, i2C); /*display off*/
            write_i(0x00, i2C); /*set lower column address*/
            write_i(0x10, i2C); /*set higher column address*/
            write_i(0x40, i2C); /*set display start line*/

            write_i(0xB0, i2C); /*set page address*/
            write_i(0x81, i2C); /*contract control*/
            write_i(0xcf, i2C); /*128*/

            write_i(0xA1, i2C); /*set segment remap*/

            write_i(0xA6, i2C); /*normal / reverse*/

            write_i(0xA8, i2C); /*multiplex ratio*/
            write_i(0x3F, i2C); /*duty = 1/64*/

            write_i(0xC8, i2C); /*Com scan direction*/

            write_i(0xD3, i2C); /*set display offset*/
            write_i(0x00, i2C);
            write_i(0xD5, i2C); /*set osc division*/
            write_i(0x80, i2C);
            write_i(0xD9, i2C); /*set pre-charge period*/
            write_i(0xf1, i2C);

            write_i(0xDA, i2C); /*set COM pins*/
            write_i(0x12, i2C);

            write_i(0xdb, i2C); /*set vcomh*/
            write_i(0x40, i2C);
            write_i(0x8d, i2C); /*set charge pump disable*/
            write_i(0x10, i2C);

            write_i(0xAF, i2C); /*display ON*/
        }

        private static void write_i(int data, I2CDevice i2C)
        {
            byte[] read = new byte[1];

            I2CDevice.I2CTransaction[] i2CTx = new I2CDevice.I2CTransaction[2];
            i2CTx[0] = I2CDevice.CreateWriteTransaction(BitConverter.GetBytes(data));
            i2CTx[1] = I2CDevice.CreateReadTransaction(read);
            i2C.Execute(i2CTx, 100);
        }
    }
}
