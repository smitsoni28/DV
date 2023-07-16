using System.Linq;
using UnityEngine;

public class VisHistogram : Vis
{
    public VisHistogram()
    {
        title = "Histogram";
        // Define Data Mark and Tick Prefab
        // Load the cube and tick mark prefabs from the Resources folder
        dataMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/Marks/Cube");
        tickMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/VisContainer/Tick");
    }

    public override GameObject CreateVis(GameObject container)
    {
        // Calculate bin sizes using different binning rules
        // Create instances of binning rule classes and pass the first value of the first dataset
        // to calculate bin ranges and frequencies
        SturgesRule sturgesRule = new SturgesRule(dataSets[0].ElementAt(0).Value);
        RiceRule riceRule = new RiceRule(dataSets[0].ElementAt(0).Value);
        ScottRule scottRule = new ScottRule(dataSets[0].ElementAt(0).Value);

        // Get bin ranges and frequencies from each binning rule
        double[] sturgesBinRanges = sturgesRule.GetBinRanges();
        double[] riceBinRanges = riceRule.GetBinRanges();
        double[] scottBinRanges = scottRule.GetBinRanges();

        double[] sturgesFrequencies = sturgesRule.GetFrequencies();
        double[] riceFrequencies = riceRule.GetFrequencies();
        double[] scottFrequencies = scottRule.GetFrequencies();

        // Select the best binning rule based on a criterion (e.g., number of bins, frequency distribution, etc.)
        double[] selectedBinRanges;
        double[] selectedFrequencies;

        //Compare the binning rules and select the one with the highest number of bins
        if (sturgesBinRanges.Length >= riceBinRanges.Length && sturgesBinRanges.Length >= scottBinRanges.Length)
        {
            selectedBinRanges = sturgesBinRanges;
            selectedFrequencies = sturgesFrequencies;
        }
        else if (riceBinRanges.Length >= sturgesBinRanges.Length && riceBinRanges.Length >= scottBinRanges.Length)
        {
            selectedBinRanges = riceBinRanges;
            selectedFrequencies = riceFrequencies;
        }
        else
        {
            selectedBinRanges = scottBinRanges;
            selectedFrequencies = scottFrequencies;
        }

        // Create Axes and Grids for the selected binning rule
        // Set the number of ticks for each axis based on the selected bin ranges
        base.xyzTicks = new int[] { selectedBinRanges.Length, 10, 10 };
        // Call the base class method to create the visualization container
        base.CreateVis(container);

        // Create X and Y axes using the selected bin ranges and frequencies
        visContainer.CreateAxis(dataSets[0].ElementAt(0).Key, selectedBinRanges, Direction.X);
        visContainer.CreateAxis("Frequency", selectedFrequencies, Direction.Y);
        // Create grids based on the X and Y axes
        visContainer.CreateGrid(Direction.X, Direction.Y);

        // Set the color channel based on the selected frequencies
        visContainer.SetChannel(VisChannel.Color, selectedFrequencies);

        // Set Remaining Vis Channels (Color, etc.) for the selected binning rule
        // Set X position channel to the selected bin ranges
        visContainer.SetChannel(VisChannel.XPos, selectedBinRanges);
        // Set Y size channel to the selected frequencies
        visContainer.SetChannel(VisChannel.YSize, selectedFrequencies);

        // Draw all Data Points with the provided Channels for the selected binning rule
        // Create data marks using the data mark prefab
        visContainer.CreateDataMarks(dataMarkPrefab);

        // Adjust the scale of the visualization container
        visContainerObject.transform.localScale = new Vector3(width, height, depth);

        return visContainerObject;
    }
}
