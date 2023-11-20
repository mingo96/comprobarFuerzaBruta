using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class pruebas
{

    private static bool Found = false;
    private static String Word = "";
    
    static void Main()
    {
        var lines = System.IO.File.ReadAllLines(@"C:\users\eloy\downloads\2151220-passwords.txt");

        Console.WriteLine("introduce tu hash a comparar");

        var hash = Console.ReadLine();

        Console.WriteLine("en cuantos hilos lo quieres dividir");
        
        var ThreadsNumber = Convert.ToInt32( Console.ReadLine());

        var threads = new List<Thread>();


        for (int Number = 0; Number < ThreadsNumber; Number++)
        {
            var fraction = lines.Length/ThreadsNumber;
            var start = fraction*Number;
            var end = fraction* (Number + 1) ;
            
            var section = lines[new Range(start,end)];

            threads.Add(new Thread(()=>
                        {
                            Console.WriteLine("nuevo hilo");
                            var foundInThisThread = SearchForSha256(section, hash);
                            if(foundInThisThread) Found = true;
                            Console.WriteLine(!foundInThisThread? "no encontrado de entrada " + start +" a entrada "+ end : "encontrado de entrada " + start +" a entrada "+ end);
                        }
                    )
            );
        }
        
        
        var StartMoment = DateTime.Now;
        var everyThreadIsDone = false;
        foreach (var hilo in threads)
        {
            hilo.Start();
        }
        while (!everyThreadIsDone)
        {
            everyThreadIsDone = true;
            foreach (var thread in threads)
            {
                if (thread.IsAlive)
                {
                    everyThreadIsDone = false;
                }
            }
            if (Found)
            {
                break;
            }
        }

        Console.WriteLine("hemos tardado " + (DateTime.Now - StartMoment));
        
        Console.WriteLine("el hash coincide con el de la combinacion " + Word);

        Console.WriteLine("Presione cualquier tecla para salir.");
        System.Console.ReadKey();
        
    }



    static bool SearchForSha256(string[] lines, string obj)
    {
        foreach (var line in lines)
        {
            var encrypted = Encrypt(line);
            if (encrypted == obj)
            {
                Word = line;
                return true;
            }

            if (Found)
            {
                break;
            }
        }

        return false;
    }

    static string Encrypt(string originalString)
    {
        var result = string.Empty;
        var convert = SHA256.Create();

        var hashValue = convert.ComputeHash(Encoding.UTF8.GetBytes(originalString));
        foreach (byte b in hashValue)
        {
            result += $"{b:X2}";
        }

        return result;
    }

}