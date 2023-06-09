using System;
using System.Collections.Generic;
using System.Linq;

public class SturgesHistogramCalculator
{
    public int CalculateNumBins(List<double> data)
    {
        int numBins = (int)(1 + Math.Log(data.Count, 2));
        return numBins;
    }
}
