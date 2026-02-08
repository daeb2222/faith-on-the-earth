using System;
using System.Collections.Generic;

public class ProbabilityScaler
{
    private static readonly Random _random = new Random();

    public static int GetWeightedNumber(double inputValue, double sensitivity = 0.1)
    {
        var weights = new Dictionary<int, double>();
        double totalWeight = 0;


        double powerFactor = (1 - Math.Exp(-sensitivity * inputValue)) * 10;

        for (int i = 1; i <= 10; i++)
        { 
            double weight = Math.Pow(i, powerFactor);
            
            weights[i] = weight;
            totalWeight += weight;
        }
        
        double randomValue = _random.NextDouble() * totalWeight;
        double currentSum = 0;

        for (int i = 1; i <= 10; i++)
        {
            currentSum += weights[i];
            if (randomValue <= currentSum)
            {
                return i;
            }
        }

        return 10;
    }
}