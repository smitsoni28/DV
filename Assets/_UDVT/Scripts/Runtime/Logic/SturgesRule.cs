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
        // Debug.Log("Data set length in Sturges:: " + data.Length);
        // Debug.Log("Selected bin ranges length in Sturges: " + binRanges.Length);
        return binRanges;
    }

    public double[] GetFrequencies()
    {
        double[] binRanges = GetBinRanges();
        double[] frequencies = new double[binRanges.Length];

        for (int i = 0; i < frequencies.Length; i++)
        {
            if (i == 0) {
                frequencies[i] = data.Count(x => x >= this.data.Min() && x < binRanges[0]);
            }

            frequencies[i] = data.Count(x => x >= binRanges[i] && x < binRanges[i + 1]);
        }
        //Debug.Log("Selected frequencies length in Sturges: " + frequencies.Length);
        return frequencies;
    }
    
    
    
}
