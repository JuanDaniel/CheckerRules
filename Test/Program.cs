﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBI.JD.Util;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            /*using (var form = new Tools.Forms.Form2())
            {
                form.ShowDialog();
            }*/

            Core.LoadAddin(@"D:\CODE\Visual Studio 2017\Projects\CheckerRules\AR_Rules\bin\Debug\AR_Rules.dll");
        }
    }
}