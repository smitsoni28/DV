using System;
using System.Collections.Generic;
using System.Linq;

public class ScottHistogramCalculator
{
    public int CalculateNumBins(List<double> data)
    {
        double stdDev = CalculateStandardDeviation(data);
        int numBins = (int)Math.Ceiling((data.Max() - data.Min()) / (3.5 * stdDev * Math.Pow(data.Count, -1.0 / 3.0)));
        return numBins;
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
