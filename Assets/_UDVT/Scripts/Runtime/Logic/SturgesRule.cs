/*using System;
using UnityEngine;
using System.Linq;

public class SturgesRule 
{
        private double[] data;
        private int numBins;
        private double binSize;
        private double binStart;
        private double binEnd;
        private double[] binRanges;
        private double[] frequencies;

        public SturgesRule(double[] data)
        {
            this.data = data;
            numBins = (int)CalculateNumBins(data.Length);
            binSize = CalculateBinSize(data, numBins);
            binRanges = CalculateBinSizeAndRange(data, numBins, binSize);
            frequencies = CalculateFrequencies(data);
        }

        public double[] GetData()
        {
            return data;
        }

        public int GetNumBins()
        {
            return numBins;
        }

        public double GetBinSize()
        {
            return binSize;
        }

        public double GetBinStart()
        {
            return binStart;
        }

        public double GetBinEnd()
        {
            return binEnd;
        }

        public double[] GetFrequencies()
        {
            return frequencies;
        }
        
        public double[] GetBinRanges()
        {
            return binRanges;
        }

        private double CalculateNumBins(double n)
        {
            // Sturges' formula: k = 1 + log2(n)
            return (double)Math.Ceiling(1 + Math.Log(n, 2));
        }

        private double CalculateBinSize(double[] data, int numBins)
        {
            int n = (int)data.Length;
            return (data.Max() - data.Min()) / (double)numBins;
        }

        private double[] CalculateBinSizeAndRange(double[] data, int numBins, double binSize/*double min, double max*/
        //)
     //   {
            /*binSize = (double)(max - min) / numBins;
            binStart = min;
            binEnd = binStart + binSize;*/
            //double [] posData;
            //foreach( double b in data) { if (b>0) {
            //posData.Append(b);} }
            /*
            double min = data.Min();
            double max = data.Max();
            double[] binRange = new double[numBins + 1];
            for (int i = 0; i <= numBins; i++)
            {
                double t = i * binSize;
                binRange[i] = min + t;
            }
         

            return binRange;
        }

        private double[] CalculateFrequencies(double[] data)
        {
            double[] frequencies = new double[(int)numBins];
            foreach (double value in data)
            {
                int binIndex = (int)Math.Floor((value - binStart) / binSize);
                if (binIndex >= 0 && binIndex < numBins)
                {
                    frequencies[binIndex]++;
                }
            }
            return frequencies;
        }
}
*/

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
        int numBins = (int)Math.Ceiling(Math.Log(data.Length, 2) + 1);
        double min = data.Min();
        double max = data.Max();
        double binSize = (max - min + 1) / numBins;

        double[] binRanges = new double[numBins + 1];
        for (int i = 0; i <= numBins; i++)
        {
            binRanges[i] = min + i * binSize;
        }
        Debug.Log("Data set length in Sturges:: " + data.Length);
        Debug.Log("Selected bin ranges length in Sturges: " + binRanges.Length);
        return binRanges;
    }

    public double[] GetFrequencies()
    {
        double[] binRanges = GetBinRanges();
        double[] frequencies = new double[binRanges.Length - 1];

        for (int i = 0; i < frequencies.Length; i++)
        {
            frequencies[i] = data.Count(x => x >= binRanges[i] && x < binRanges[i + 1]);
        }
        Debug.Log("Selected frequencies length in Sturges: " + frequencies.Length);
        return frequencies;
    }
    
    
    
}
