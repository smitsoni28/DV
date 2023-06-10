 
using System.Linq;
using UnityEngine;

public class Histogram : Vis
{
    public Histogram()
    {
        title = "Histogram";
        // Define Data Mark and Tick Prefab
        dataMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/Marks/Cube");
        tickMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/VisContainer/Tick");
    }

    public override GameObject CreateVis(GameObject container)
    {
       

        // Calculate bin sizes using different binning rules
        SturgesRule sturgesRule = new SturgesRule(dataSets[0].ElementAt(0).Value);
        RiceRule riceRule = new RiceRule(dataSets[0].ElementAt(0).Value);
        ScottRule scottRule = new ScottRule(dataSets[0].ElementAt(0).Value);

        double[] sturgesBinRanges = sturgesRule.GetBinRanges();
        double[] riceBinRanges = riceRule.GetBinRanges();
        double[] scottBinRanges = scottRule.GetBinRanges();

        double[] sturgesFrequencies = sturgesRule.GetFrequencies();
        double[] riceFrequencies = riceRule.GetFrequencies();
        double[] scottFrequencies = scottRule.GetFrequencies();

        // Debug.Log("Sturges bin ranges length: " + sturgesBinRanges.Length);
        // Debug.Log("Rice bin ranges length: " + riceBinRanges.Length);
        // Debug.Log("Scott bin ranges length: " + scottBinRanges.Length);
        // Debug.Log("Sturges frequencies length: " + sturgesFrequencies.Length);
        // Debug.Log("Rice frequencies length: " + riceFrequencies.Length);
        // Debug.Log("Scott frequencies length: " + scottFrequencies.Length);



        // Select the best binning rule based on a criterion (e.g., number of bins, frequency distribution, etc.)
        double[] selectedBinRanges;
        double[] selectedFrequencies;

        // Compare the binning rules and select the best one
        // Example: Select the rule with the highest number of bins
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


        // Calculate the label step size based on the maximum range and the number of labels needed
        //double labelStep = (selectedBinRanges[selectedBinRanges.Length - 1] - selectedBinRanges[0]) / (selectedBinRanges.Length - 1);


        // Create Axes and Grids for the selected binning rule

       // selectedFrequencies = new double[10];

        
        base.xyzTicks = new int[]{selectedBinRanges.Length, 10, 10};
        base.CreateVis(container);
        
        visContainer.CreateAxis(dataSets[0].ElementAt(0).Key, selectedBinRanges, Direction.X);
        visContainer.CreateAxis("Frequency", selectedFrequencies, Direction.Y);
        visContainer.CreateGrid(Direction.X, Direction.Y);

        
        visContainer.SetChannel(VisChannel.Color, selectedFrequencies);

        // // Draw all Data Points with the provided Channels for the selected binning rule
        // visContainer.CreateDataMarks(dataMarkPrefab);



        // Set Remaining Vis Channels (Color, etc.) for the selected binning rule
        Debug.Log("Selected bin ranges length: " + selectedBinRanges.Length);
        Debug.Log("Selected frequencies length: " + selectedFrequencies.Length);
        Debug.Log("Data set length: " + dataSets[0].Count());

        visContainer.SetChannel(VisChannel.XPos, selectedBinRanges);
        visContainer.SetChannel(VisChannel.YSize, selectedFrequencies);
        //visContainer.SetChannel(VisChannel.Color, dataSets[0].ElementAt(0).Value);

        // Draw all Data Points with the provided Channels for the selected binning rule
        visContainer.CreateDataMarks(dataMarkPrefab);





        // Rescale Chart
        visContainerObject.transform.localScale = new Vector3(width, height, depth);

        return visContainerObject;
    }
}

