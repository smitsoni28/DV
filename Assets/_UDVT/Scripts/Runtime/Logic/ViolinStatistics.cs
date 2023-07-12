using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ViolinStatistics
{
    private double[] data;
    private double median;
    private double lowerQuartile;
    private double upperQuartile;
    private double iqr;
    private double minValue;
    private double maxValue;

    public ViolinStatistics(double[] data)
    {
        this.data = data;
        CalculateStatistics(this.data);

    }

    public double getMedian() { return median; }
    public double getLowerQuartile() { return lowerQuartile; }
    public double getUpperQuartile() { return upperQuartile; }
    public double getIQR() { return iqr; }
    public double getMinValue() { return minValue; }
    public double getMaxValue() { return maxValue; }

    private void CalculateStatistics(double[] data)
    {

        // Sort the data in ascending order double[] stati = new double[10];
        Array.Sort(data);

        median = CalculatePercentile(50, data);

        // Calculate lower and upper quartiles
        lowerQuartile = CalculatePercentile(25, data);
        upperQuartile = CalculatePercentile(75, data);

        // Calculate interquartile range (IQR)
        iqr = upperQuartile - lowerQuartile;
        
        minValue = data[0];
        maxValue= data[data.Length - 1];
       
    }

    private double CalculatePercentile(double percentile, double[] data)
    {


        // Calculate the value at the given percentile
        double position = (data.Length - 1) * percentile / 100;
        int lowerIndex = (int)Math.Floor(position);
        int upperIndex = (int)Math.Ceiling(position);

        if (lowerIndex == upperIndex)
            return data[lowerIndex];

        double lowerValue = data[lowerIndex];
        double upperValue = data[upperIndex];
        return lowerValue + (upperValue - lowerValue) * (position - lowerIndex);
    }

}