using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class pruebas
{

    private static bool Encontrado = false;
    private static String Palabra = "";
    
    static void Main()
    {
        var lines = System.IO.File.ReadAllLines(@"C:\users\eloy\downloads\2151220-passwords.txt");

        Console.WriteLine("introduce tu hash a comparar");

        var hash = Console.ReadLine();

        Console.WriteLine("en cuantos hilos lo quieres dividir");
        
        var ThreadsNumber = Convert.ToInt32( Console.ReadLine());

        var hilos = new List<Thread>();


        for (int Number = 0; Number < ThreadsNumber; Number++)
        {
            var fraccion = lines.Length/ThreadsNumber;
            var start = fraccion*Number;
            var end = fraccion* (Number + 1) ;
            
            var section = lines[new Range(start,end)];

            hilos.Add(new Thread(()=>
                        {
                            Console.WriteLine("nuevo hilo");
                            var encontradoEnEsta = BuscarSha256(section, hash);
                            if(encontradoEnEsta) Encontrado = true;
                            Console.WriteLine(!encontradoEnEsta? "no encontrado de entrada " + start +" a entrada "+ end : "encontrado de entrada " + start +" a entrada "+ end);
                        }
                    )
            );
        }
        
        
        var StartMoment = DateTime.Now;
        var todosTerminados = false;
        foreach (var hilo in hilos)
        {
            hilo.Start();
        }
        while (!todosTerminados)
        {
            todosTerminados = true;
            foreach (var hilo in hilos)
            {
                if (hilo.IsAlive)
                {
                    todosTerminados = false;
                }
            }
            if (Encontrado)
            {
                break;
            }
        }

        Console.WriteLine("hemos tardado " + (DateTime.Now - StartMoment));
        
        Console.WriteLine("el hash coincide con el de la combinacion " + Palabra);

        Console.WriteLine("Presione cualquier tecla para salir.");
        System.Console.ReadKey();
        
    }



    static bool BuscarSha256(string[] lines, string obj)
    {
        foreach (var line in lines)
        {
            var encriptado = Encriptar(line);
            if (encriptado == obj)
            {
                Palabra = line;
                return true;
            }

            if (Encontrado)
            {
                break;
            }
        }

        return false;
    }

    static string Encriptar(string cadena)
    {
        var resultado = string.Empty;
        var convert = SHA256.Create();

        var hashValue = convert.ComputeHash(Encoding.UTF8.GetBytes(cadena));
        foreach (byte b in hashValue)
        {
            resultado += $"{b:X2}";
        }

        return resultado;
    }

}