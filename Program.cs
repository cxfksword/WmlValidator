using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WmlValidatorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = WmlValidator.VerifyFile("check.xml");

            if (result.Success)
            {
                Console.WriteLine("wml格式合法.");
            }
            else
            {
                Console.WriteLine("错误位置：\r\n" + result.ErrorPosition);
                Console.WriteLine("错误消息：\r\n" + result.ErrorMessage);
            }

            Console.ReadKey();
        }
    }
}
