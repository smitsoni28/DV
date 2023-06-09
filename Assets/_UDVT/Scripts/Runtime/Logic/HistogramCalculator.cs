using System;
using System.Collections.Generic;
using System.Linq; 

public class HistogramCalculator
{
    public int CalculateNumBinsSturges(List<double> data)
    {
        int numBins = (int)(1 + Math.Log(data.Count, 2));
        return numBins;
    }

    public int CalculateNumBinsScott(List<double> data)
    {
        double stdDev = CalculateStandardDeviation(data);
        int numBins = (int)Math.Ceiling((data.Max() - data.Min()) / (3.5 * stdDev * Math.Pow(data.Count, -1.0 / 3.0)));
        return numBins;
    }

    public int CalculateNumBinsFreedmanDiaconis(List<double> data)
    {
        double iqr = CalculateInterquartileRange(data);
        int numBins = (int)Math.Ceiling((data.Max() - data.Min()) / (2.0 * iqr * Math.Pow(data.Count, -1.0 / 3.0)));
        return numBins;
    }

    public double CalculateBinSize(List<double> data, int numBins)
    {
        double dataRange = data.Max() - data.Min();
        double binSize = dataRange / numBins;
        return binSize;
    }

    public Dictionary<double, int> CalculateFrequencyCounts(List<double> data, int numBins, double binSize)
    {
        Dictionary<double, int> frequencyCounts = new Dictionary<double, int>();

        // Initialize the frequency counts dictionary
        for (int i = 0; i < numBins; i++)
        {
            double binStart = data.Min() + (i * binSize);
            frequencyCounts.Add(binStart, 0);
        }

        // Count the frequency of data points in each bin
        foreach (double value in data)
        {
            int binIndex = (int)((value - data.Min()) / binSize);
            frequencyCounts[data.Min() + (binIndex * binSize)]++;
        }

        return frequencyCounts;
    }

    private double CalculateStandardDeviation(List<double> data)
    {
        double mean = CalculateMean(data);
        double sumOfSquaredDifferences = 0.0;
        foreach (double value in data)
        {
            double difference = value - mean;
            sumOfSquaredDifferences += difference * difference;
        }
        double variance = sumOfSquaredDifferences / data.Count;
        double stdDev = Math.Sqrt(variance);
        return stdDev;
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

    private double CalculateMean(List<double> data)
    {
        double sum = 0.0;
        foreach (double value in data)
        {
            sum += value;
        }
        double mean = sum / data.Count;
        return mean;
    }
}
