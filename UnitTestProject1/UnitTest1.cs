using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nuron;
using System.IO;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        static string mainPath = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("neuron") + 6);
        static StreamWriter fs = new StreamWriter($"{mainPath}/Logs/log.txt");
        static int Success = 0;
        static int Fall = 0;

        [TestMethod]
        public void OneRandomNumber()
        {
            Random rand = new Random();
            Program.RefreshFiles();
            Program.CreateOrLoadWeb();
            int setNum = rand.Next(Program.ListOfAdress.Count);
            int chooseNumber = (int)Char.GetNumericValue(Program.ListOfAdress[setNum][1]);
            Neuron newAnalize = Program.NeuroAnalize($"{mainPath}/Kappa/{Program.ListOfAdress[setNum]}");
            try {
                Assert.AreEqual(newAnalize.name, chooseNumber.ToString());
                Success++;
                fs.WriteLine($"Success {Success}/{Success + Fall} {Program.ListOfAdress[setNum]}");
                Program.CorrectMemory($"{mainPath}/Kappa/{Program.ListOfAdress[setNum]}", chooseNumber.ToString());
            } catch {
                Fall++;
                fs.WriteLine($"Fail {Success}/{Success + Fall} {Program.ListOfAdress[setNum]}");
                Program.CorrectMemory($"{mainPath}/Kappa/{Program.ListOfAdress[setNum]}", chooseNumber.ToString());
            }
        }

        [TestMethod]
        public void HungredRandomNumber()
        {
            for (int i = 0; i < 1000; i++)
                OneRandomNumber();
            fs.Close();
        }
    }
}
