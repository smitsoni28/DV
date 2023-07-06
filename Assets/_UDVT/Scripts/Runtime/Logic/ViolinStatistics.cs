// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class ViolinStatistics
// {
//     private double[] data;

//     public ViolinStatistics(double[] data)
//     {
//         this.data = data;
//         CalculateStatistics(this.data);
//     }

//     private void CalculateStatistics(double[] data)
//     {
//         // Sort the data in ascending order
//         Array.Sort(data);
//     }

//     // Calculate the median
//     public static double CalculateMedian(List<double> data)
//     {
//         int n = data.Count;
//         if (n % 2 == 0)
//         {
//             return (data[n / 2 - 1] + data[n / 2]) / 2.0;
//         }
//         else
//         {
//             return data[n / 2];
//         }
//     }

//     // Calculate the quartiles
//     public static Tuple<double, double, double> CalculateQuartiles(List<double> data)
//     {
//         var sortedData = data.OrderBy(x => x).ToList();
//         int n = sortedData.Count;
//         double q1 = sortedData[n / 4];
//         double q2 = CalculateMedian(sortedData);
//         double q3 = sortedData[(n * 3) / 4];

//         return Tuple.Create(q1, q2, q3);
//     }

//     // Calculate outliers
//     public static List<double> CalculateOutliers(List<double> data, double lowerFence, double upperFence)
//     {
//         return data.Where(x => x < lowerFence || x > upperFence).ToList();
//     }
// }



using System;

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

        // Calculate minimum and maximum values
        minValue = data[0];
        maxValue = data[data.Length - 1];
        //stati.Add(median);

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