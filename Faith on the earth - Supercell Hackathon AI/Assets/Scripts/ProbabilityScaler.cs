using System;

public class ProbabilityScaler
{
    // Use a single Random instance to avoid seeding issues
    private static readonly Random _random = new Random();

    public static int GetWeightedNumber(int inputValue, double sensitivity = 0.1)
    {
        // 1. Calculate a Power Factor that ranges from Negative to Positive.
        // Math.Exp(-sens * val) goes from 1 (at val=0) to 0 (at val=infinite).
        // We want a range of roughly -5 (favors 1) to +5 (favors 10).
        
        // This formula maps input 0 -> -5, and input Infinite -> +5
        double rawScale = 1.0 - Math.Exp(-sensitivity * inputValue); // 0.0 to 1.0
        double powerFactor = (rawScale * 10.0) - 5.0; 

        double totalWeight = 0;
        double[] weights = new double[10]; // Array is faster than Dictionary

        // 2. Calculate Weights
        for (int i = 0; i < 10; i++)
        {
            // Number is i+1 (1 to 10)
            int number = i + 1;
            
            // Math.Pow(number, negative) makes small numbers heavier.
            // Math.Pow(number, positive) makes large numbers heavier.
            weights[i] = Math.Pow(number, powerFactor);
            totalWeight += weights[i];
        }

        // 3. Select Weighted Random
        double randomValue = _random.NextDouble() * totalWeight;
        double currentSum = 0;

        for (int i = 0; i < 10; i++)
        {
            currentSum += weights[i];
            if (randomValue <= currentSum)
            {
                return i + 1;
            }
        }

        return 10;
    }
}