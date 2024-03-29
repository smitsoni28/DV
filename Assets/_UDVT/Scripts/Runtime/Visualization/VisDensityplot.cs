using System.Linq;
using UnityEngine;

public class VisDensityplot : Vis
{
    public VisDensityplot()
    {
        title = "Densityplot";
        // Define Data Mark and Tick Prefab
        // Load the sphere and tick mark prefabs from the Resources folder
        dataMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/Marks/Sphere");
        tickMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/VisContainer/Tick");
    }

    public override GameObject CreateVis(GameObject container)
    {
        // parameters for KDE method
        double sigma = 1; // Default value mentioned in Wikipedia about KDE
        int nsteps = 100; // Default value mentioned in the KDE class

        // Call the KDE method to calculate points
        // Pass the value of the first element in the first dataset, sigma, and nsteps to calculate points using KDE
        double[,] calculated_points = KernelDensityEstimation.KDE(dataSets[0].ElementAt(0).Value, sigma, nsteps);

        // Set the positions of the line renderer to the calculated points
        Vector3[] linePositions = new Vector3[nsteps];

        // Extract the x and y values from the calculated_points
        double[] calculatedPoints_XValues = new double[nsteps];
        double[] calculatedPoints_YValues = new double[nsteps];
        for (int i = 0; i < nsteps; i++)
        {
            calculatedPoints_XValues[i] = calculated_points[i, 0];
            calculatedPoints_YValues[i] = calculated_points[i, 1];
        }

        // Create Axes and Grids
        // Call the base class method to create the visualization container
        base.CreateVis(container);

        // Create X and Y axes using the calculated x and y values
        visContainer.CreateAxis(dataSets[0].ElementAt(0).Key, calculatedPoints_XValues, Direction.X);
        visContainer.CreateAxis(" ", calculatedPoints_YValues, Direction.Y);
        // Create grids based on the X and Y axes
        visContainer.CreateGrid(Direction.X, Direction.Y);

        // Set colors and Vis channel
        // Set the color channel based on the calculated y values
        visContainer.SetChannel(VisChannel.Color, calculatedPoints_YValues);

        // Set X and Y position channels to the calculated x and y values
        visContainer.SetChannel(VisChannel.XPos, calculatedPoints_XValues);
        visContainer.SetChannel(VisChannel.YPos, calculatedPoints_YValues);

        // Plot all calculated points
        // Create data marks using the data mark prefab
        visContainer.CreateDataMarks(dataMarkPrefab);

        // Create a line renderer to draw the line connecting the data marks
        LineRenderer lineRenderer = visContainerObject.AddComponent<LineRenderer>();
        lineRenderer.material.color = Color.black;
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;

        // Get the positions of the data marks and set them as the positions for the line renderer
        var marks = visContainer.dataMarkList;
        Vector3[] positions = new Vector3[marks.Count];
        for (int i = 0; i < marks.Count; ++i)
        {
            positions[i] = marks[i].GetDataMarkChannel().position;
        }

        lineRenderer.positionCount = positions.Length; // Set the number of vertices for the line renderer
        lineRenderer.SetPositions(positions); // Set the positions of the line renderer

        return visContainerObject;
    }
}
