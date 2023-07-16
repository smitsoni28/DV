using System;
using System.Linq;
using UnityEngine;

public static class ArrayExtensions
{
    // Extension method to calculate the standard deviation of an array of doubles
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

    // Calculate bin ranges using Scott's Rule
    public double[] GetBinRanges()
    {
        double h = 3.5 * data.StandardDeviation() / Math.Pow(data.Length, 1.0 / 3.0);
        int numBins = (int)Math.Ceiling((data.Max() - data.Min()) / h);
        double min = data.Min();
        double max = data.Max();
        double binSize = (max - min + 1) / numBins;

        double[] binRanges = new double[numBins];
        for (int i = 0; i < numBins; i++)
        {
            binRanges[i] = (1 + i) * binSize;
        }
        return binRanges;
    }

    // Calculate frequencies for each bin using the bin ranges
    public double[] GetFrequencies()
    {
        double[] binRanges = GetBinRanges();
        double[] frequencies = new double[binRanges.Length];

        for (int i = 0; i < frequencies.Length; i++)
        {
            if (i == 0)
            {
                // Calculate frequency for the first bin
                frequencies[i] = data.Count(x => x >= data.Min() && x < binRanges[0]);
            }
            else
            {
                // Calculate frequency for the remaining bins
                frequencies[i] = data.Count(x => x >= binRanges[i] && x < binRanges[i + 1]);
            }
        }
        return frequencies;
    }
}
