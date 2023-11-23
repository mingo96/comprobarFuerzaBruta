using System.Text;
using System.Security.Cryptography;


class Pruebas
{

    private static bool _found;
    
    private static string _word = "";
    
    private static SHA256 ShaConvert = SHA256.Create();
    
    static void Main()
    {
        var lines = File.ReadAllLines(@"2151220-passwords.txt");

        Console.WriteLine("introduce tu hash a comparar");

        var hash = Console.ReadLine()!.ToUpper();

        Console.WriteLine("en cuantos hilos lo quieres dividir");
        
        var threadsNumber = Convert.ToInt32( Console.ReadLine());

        var threads = new List<Thread>();

        var fractionOfLines = lines.Length/threadsNumber;

        for (var number = 0; number < threadsNumber; number++)
        {
            var start = fractionOfLines*number;
            var end = fractionOfLines* (number + 1) ;
            
            var section = lines[new Range(start,end)];

            threads.Add(new Thread(()=>
                        {
                            Console.WriteLine("nuevo hilo");
                            var foundInThisThread = SearchForSha256(section, hash);
                            if(foundInThisThread) _found = true;
                            Console.WriteLine(!foundInThisThread? "no encontrado de entrada " + start +" a entrada "+ end : "encontrado de entrada " + start +" a entrada "+ end);
                            
                        }
                    )
            );
        }
        
        var startMoment = DateTime.Now;
        var everyThreadIsDone = false;
        
        foreach (var thread in threads)
        {
            thread.Start();
        }
        
        while (!everyThreadIsDone || !_found)
        {
            everyThreadIsDone = true;
            foreach (var thread in threads)
            {
                if (thread.IsAlive)
                {
                    everyThreadIsDone = false;
                }
                
            }
            
        }

        Console.WriteLine("hemos tardado " + (DateTime.Now - startMoment));
        
        Console.WriteLine("el hash coincide con el de la combinacion " + _word);

        Console.WriteLine("Presione cualquier tecla para salir.");  
        
        Console.ReadKey();
        
    }



    private static bool SearchForSha256(string[] lines, string? obj)
    {
        foreach (var line in lines)
        {
            var encrypted = Encrypt(line);
            if (encrypted == obj)
            {
                _word = line;
                return true;
            }

            if (_found)
            {
                break;
            }
        }

        return false;
    }

    private static string Encrypt(string originalString)
    {
        var result = string.Empty;
        
        //hay que bloquear el conversor, si no, da fallo por acceso multiple
        lock (ShaConvert)
        {
            var hashValue = ShaConvert.ComputeHash(Encoding.UTF8.GetBytes(originalString));
                    foreach (byte b in hashValue)
                    {
                        result += $"{b:X2}";
                    }
        }
        
        

        return result;
    }

}