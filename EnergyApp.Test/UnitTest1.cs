using System;
using Xunit;

namespace EnergyApp.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            System.Console.Out.WriteLineAsync("Hi 1");
        }

        [Fact]
        public void Test2() {
            System.Console.Out.WriteLineAsync("Hi Test2");
        }
    }
}
