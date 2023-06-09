using System;
using System.Collections.Generic;
using System.Linq;

public class FreedmanDiaconisHistogramCalculator
{
    public int CalculateNumBins(List<double> data)
    {
        double iqr = CalculateInterquartileRange(data);
        int numBins = (int)Math.Ceiling((data.Max() - data.Min()) / (2.0 * iqr * Math.Pow(data.Count, -1.0 / 3.0)));
        return numBins;
    }

    private double CalculateInterquartileRange(List<double> data)
    {
        List<double> sortedData = new List<double>(data);
        sortedData.Sort();

        int lowerIndex = (int)(0.25 * (data.Count - 1));
        int upperIndex = (int)(0.75 * (data.Count - 1));

        double lowerQuartile = sortedData[lowerIndex];
        double upperQuartile = sortedData[upperIndex];

        double iqr = upperQuartile - lowerQuartile;
        return iqr;
    }
}
