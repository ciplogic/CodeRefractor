using System;

class NBody
{
    public static void Main()
    {
        int n = 500000;
        var sourceArray = new float[n];

        var targetArray = new float[n];
        var outputArray = new float[n];
        for (var i = 0; i < n; i++)
        {
            sourceArray[i] = i / (float)n;
            targetArray[i] = 2 * i / (float)n;
        }
        OpenClCompute.InvokeArray(outputArray, sourceArray, targetArray, 3);
    }



    public abstract class OpenClDecorator : Attribute
    {
         
    }
    [OpenClDecorator]
    public class OpenClCompute 
    {

        public static float Invoke(float a, float b, int toAdd)
        {
            int intA = (int)a;
            int intB = (int)b;



            float result;
            if (intA%2 != 0)
            {
                result = intA + intB;
            }
            else
            {
                result = intA + intB + toAdd;
            }
            return result;
        }

        public static void InvokeArray(float[] output, float[] inputA, float[] inputB, int toAdd)
        {
            
        }
    }
}
