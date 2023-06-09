using System;
using System.Linq;
using UnityEngine;

public static class ArrayExtensions
{
    public static double StandardDeviation(this double[] values)
    {
        double mean = values.Average();
        double sumOfSquares = values.Sum(x => Math.Pow(x - mean, 2));
        double variance = sumOfSquares / values.Length;
        double standardDeviation = Math.Sqrt(variance);
        return standardDeviation;
    }
}

public class ScottRule
{
    private double[] data;

    public ScottRule(double[] data)
    {
        this.data = data;
    }

    public double[] GetBinRanges()
    {
        double h = 3.5 * data.StandardDeviation() / Math.Pow(data.Length, 1.0 / 3.0);
        int numBins = (int)Math.Ceiling((data.Max() - data.Min()) / h);
        double min = data.Min();
        double max = data.Max();
        double binSize = (max - min) / numBins;

        double[] binRanges = new double[numBins + 1];
        for (int i = 0; i <= numBins; i++)
        {
            binRanges[i] = min + i * binSize;
        }
        Debug.Log("Data set length in Scott:: " + data.Length);
        Debug.Log("Selected bin ranges length in Scott: " + binRanges.Length);
    
        

        return binRanges;
    }

    public double[] GetFrequencies()
    {
        double[] binRanges = GetBinRanges();
        double[] frequencies = new double[binRanges.Length - 1];

        for (int i = 0; i < frequencies.Length; i++)
        {
            frequencies[i] = data.Count(x => x >= binRanges[i] && x < binRanges[i + 1]);
        }
        Debug.Log("Selected frequencies length in Scott: " + frequencies.Length);
        return frequencies;
    }
}
