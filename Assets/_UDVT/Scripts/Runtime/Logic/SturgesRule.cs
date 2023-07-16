using System;
using System.Linq;
using UnityEngine;

public class SturgesRule
{
    private double[] data;

    public SturgesRule(double[] data)
    {
        this.data = data;
    }

    // Calculate bin ranges using Sturges' Rule
    public double[] GetBinRanges()
    {
        int numBins = (int)Math.Ceiling(Math.Log(data.Length, 2));
        double min = data.Min();
        double max = data.Max();
        double binSize = (max - min + 1) / numBins;

        double[] binRanges = new double[numBins];
        for (int i = 0; i < numBins; i++)
        {
            binRanges[i] = (i + 1) * binSize;
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
