//Structs work but there is an issue with structs in arrays
//Also Array code is too verbose and alot of unneccesary code is generated, can we partially decompile such
//code using some kind of instrinsic probably from ILSPY ?
public static class Program
{
    struct Test
    {
        public int A;
    }

    public static void Main()
    {

        var struct1 = new Test { A = 32 };
        var struct2 = new Test { A = 16 };

        System.Console.WriteLine(struct1.A); 
        System.Console.WriteLine(struct2.A);

        /*
         * The Following doesn't work ...
        var testArray = new Test[]
        {
            new Test { A = 32 },
            new Test { A = 16 },
        };

        System.Console.WriteLine(testArray[1].A);

        var test = testArray[0];
        System.Console.WriteLine(test.A);*/
    }
}