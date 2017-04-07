using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Nuron
{
    public class Neuron
    {
        public string name;
        public int[,] input;
        public int output;
        public int[,] memory;

        public Neuron(string name)
        {
            this.name = name;
            this.input = new int[30, 30];
            this.output = 0;
            this.memory = new int[30, 30];
            for (int i = 0; i < 30; i++)
                for (int j = 0; j < 30; j++)
                    memory[i, j] = 255;
        }

        /// <summary>
        /// Сохранение памяти нейрона в картинку
        /// </summary>
        public void SaveMemory()
        {
            Bitmap memoryBmp = new Bitmap(30, 30);
            for (int i = 0; i < 30; i++)
                for (int j = 0; j < 30; j++) {
                    Color color = Color.FromArgb(memory[i, j], memory[i, j], memory[i, j]);
                    memoryBmp.SetPixel(i, j, color);
                }
            using (FileStream fs = new FileStream($"{Program.mainPath}/Memory/{name}.png", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                memoryBmp.Save(fs, ImageFormat.Png);
        }

        /// <summary>
        /// Загружает память нейрона
        /// </summary>
        /// <returns>Смогла ли память загрузится</returns>
        public bool LoadMemory()
        {
            try {
                Bitmap memoryBmp;
                using (FileStream fs = new FileStream($"{Program.mainPath}/Memory/{name}.png", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                    memoryBmp = new Bitmap(fs);
                for (int i = 0; i < 30; i++)
                    for (int j = 0; j < 30; j++) {
                        memory[j, i] = memoryBmp.GetPixel(j, i).R;
                    }
                return true;
            } catch {
                return false;
            }
        }
    }

    public class Program
    {
        public static Neuron[] Neuro_Web = new Neuron[10];
        public static string mainPath = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().IndexOf("neuron") + 6);
        public static List<string> ListOfAdress = new List<string>();

        /// <summary>
        /// Обновление списка файлов для распознания
        /// </summary>
        public static void RefreshFiles()
        {
            string[] files = Directory.GetFiles($"{mainPath}/Kappa");
            ListOfAdress.Clear();
            for (int i = 0; i < files.Length; i++)
                ListOfAdress.Add(files[i].Substring(files[i].IndexOf("Kappa") + 6));
        }

        /// <summary>
        /// Править память неправильного нейрона
        /// </summary>
        /// <param name="filePath">Путь до файла распознавания</param>
        /// <param name="nameNeuron">Имя исправляемого нейрона</param>
        public static void CorrectMemory(string filePath, string nameNeuron)
        {
            Bitmap bmp = new Bitmap(filePath);
            foreach (Neuron e in Neuro_Web)
                if (e.name == nameNeuron) {
                    for (int i = 0; i < 30; i++)
                        for (int j = 0; j < 30; j++) {
                            Color picPixel = bmp.GetPixel(j, i);
                            if (picPixel.R < 250)
                                e.memory[j, i] = Math.Abs(picPixel.R - e.memory[j, i]) / 2;
                        }
                    e.SaveMemory();
                }
        }

        /// <summary>
        /// Функция нейронного анализа картинки
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <returns>Нейрон с максимальным весом</returns>
        public static Neuron NeuroAnalize(string filePath)
        {
            Bitmap bmp = new Bitmap(filePath);
            Neuron maxNeuron = Neuro_Web[0];
            foreach (Neuron neo in Neuro_Web) {
                neo.output = 0;
                for (int i = 0; i < 30; i++)
                    for (int j = 0; j < 30; j++) {
                        Color picPixel = bmp.GetPixel(j, i);
                        if (Math.Abs(picPixel.R - neo.memory[j, i]) < 150 && (picPixel.R < 250 && picPixel.G < 250 && picPixel.B < 250)) {
                            neo.output++;
                        }
                    }
                if (maxNeuron.output < neo.output) {
                    maxNeuron = neo;
                }
            }
            return maxNeuron;
        }

        public static void CreateOrLoadWeb()
        {
            for (int i = 0; i < 10; i++) {
                Neuro_Web[i] = new Neuron(i.ToString());
                Neuro_Web[i].LoadMemory();
            }
        }

        static void Main(string[] args)
        {
            CreateOrLoadWeb();
            RefreshFiles();
            while (true) {
                int choose = -1;
                Console.WriteLine("Выберете файл для распознования:");
                Console.WriteLine($"{choose++}) Выход");
                foreach (string s in ListOfAdress)
                    Console.WriteLine($"{choose++}) {s}");
                Console.Write("Ввод: ");
                choose = int.Parse(Console.ReadLine());
                if (choose == -1) return;
                Neuron newAnalize = NeuroAnalize($"{mainPath}/Kappa/{ListOfAdress[choose]}");
                Console.WriteLine($"Сеть решила, что это {newAnalize.name}");
                Console.WriteLine("Верно? (да +, нет -)");
                string needToCorrect = Console.ReadLine();
                if (needToCorrect == "-") {
                    Console.WriteLine("Введите верный ответ:");
                    string correctAnswer = Console.ReadLine();
                    CorrectMemory($"{mainPath}/Kappa/{ListOfAdress[choose]}", correctAnswer);
                } else
                    CorrectMemory($"{mainPath}/Kappa/{ListOfAdress[choose]}", newAnalize.name);
            }
        }
    }
}
