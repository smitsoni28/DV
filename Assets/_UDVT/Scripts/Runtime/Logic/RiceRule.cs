using System;
using System.Linq;
using UnityEngine;

public class RiceRule
{
    private double[] data;

    public RiceRule(double[] data)
    {
        this.data = data;
    }

    public double[] GetBinRanges()
    {
        int numBins = (int)Math.Ceiling(2 * Math.Pow(data.Length, 1.0 / 3.0));
        double min = data.Min();
        double max = data.Max();
        double binSize = (max - min + 1) / numBins;

        double[] binRanges = new double[numBins];
        for (int i = 0; i < numBins; i++)
        {
            binRanges[i] = (1 + i) * binSize;
        }
        // Debug.Log("Data set length in Rice: " + data.Length);
        // Debug.Log("Selected bin ranges length in Rice: " + binRanges.Length);
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
