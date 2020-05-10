using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;

namespace CipherMachine
{
    class Program
    {
        static void SetKeys()
        {
            // method for editing the saved cipher keys
            string path = "keys.txt";

            Console.WriteLine("Please enter key #1:");
            Console.Write("  > ");
            string key1 = Console.ReadLine();

            Console.WriteLine("Please enter key #2:");
            Console.Write("  > ");
            string key2 = Console.ReadLine();

            // keys are written in plaintext to txt file for persistance
            File.WriteAllText(path, key1 + "\n" + key2);
            Console.WriteLine("New keys have been set.");
        }

        static string[] GetKeys()
        {
            string path = "keys.txt";
            string[] keys = new string[2];

            string[] contents = File.ReadAllLines(path);

            for (int i = 0; i < 2;i++)
            {
                keys[i] = contents[i];
            }

            return keys;
        }

        static ushort CheckEncoding(int charCode)
        {
            // adjusts for when charCode is outside the bounds of utf-16 (1 code unit only)
            while (charCode < 0)
            {
                charCode += 65536;        // 1x uft-16 code unit: values 0 through 65535
            }
            while (charCode > 65535)
            {
                charCode -= 65536;
            }

            return Convert.ToUInt16(charCode);
        }

        static void Encrypt()
        {
            string[] keys = GetKeys();

            Console.Write("--- Encrypt\n  > ");
            string text = Console.ReadLine();
            string final = "";

            for (int currentIndex = 0; currentIndex < text.Length; currentIndex++)
            {
                // converts to utf-32 encoding of the current character
                int charCode = Convert.ToInt32(text[currentIndex]);

                foreach (string k in keys)
                {
                    int keyIndex = currentIndex;

                    // index wrapping in case the input string is longer than key:
                    while (keyIndex >= k.Length)
                    {
                        keyIndex -= k.Length;
                    }

                    int keyCharCode = Convert.ToInt32(k[keyIndex]);
                    charCode += keyCharCode;
                }

                ushort shortCharCode = CheckEncoding(charCode);
                char outChar = (char)shortCharCode;
                final += outChar;
            }
            Console.WriteLine("\n  " + final);
        }

        static void Decrypt()
        {
            string[] keys = GetKeys();

            Console.Write("--- Decrypt\n  > ");
            string text = Console.ReadLine();
            string final = "";

            for (int currentIndex = 0; currentIndex < text.Length; currentIndex++)
            {
                // converts to utf-16 encoding of the current character
                int charCode = Convert.ToInt32(text[currentIndex]);

                foreach (string k in keys)
                {
                    int keyIndex = currentIndex;

                    // index wrapping in case the input string is longer than key:
                    while (keyIndex >= k.Length)
                    {
                        keyIndex -= k.Length;
                    }

                    int keyCharCode = Convert.ToInt32(k[keyIndex]);
                    charCode -= keyCharCode;
                }

                charCode = CheckEncoding(charCode);
                char outChar = (char)charCode;
                final += outChar;
            }
            Console.WriteLine("\n  " + final);
        }

        static string Menu()
        {
            // menu method

            Console.WriteLine("\n\n--- MENU\n" +
                "1 : encrypt\n" +
                "2 : decrypt\n" +
                "3 : change keys\n" +
                "4 : close\n");
            Console.Write("  > ");
            string x = Console.ReadLine();
            Console.WriteLine("\n");

            switch (x)
            {
                case "1":
                    Encrypt();
                    return x;

                case "2":
                    Decrypt();
                    return x;

                case "3":
                    SetKeys();
                    return x;

                case "4":
                    // close case
                    return x;

                case "":
                    // close case
                    return "4";

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    return x;
            }
        }

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Welcome to ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("casheww's ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Vigenère Cipher Machine!\n");

            string path = "keys.txt";
            try
            {
                string keys = File.ReadAllText(path);
                if (keys == " ") { SetKeys(); }
            }
            catch (FileNotFoundException)
            {
                SetKeys();
            }

            string option = "0";
            while (option != "4")
            {
                option = Menu();
            }
            Console.WriteLine("\n--- closing ---");
            Thread.Sleep(5000);
            Environment.Exit(0);
        }
    }
}
