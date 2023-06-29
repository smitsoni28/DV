using System.Linq;
using UnityEngine;

public class VisDensityplot : Vis
{
    public VisDensityplot()
    {
        title = "VisDensityplot";
        // Define Data Mark and Tick Prefab
        dataMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/Marks/Sphere");// Sphere is choosen to obtain the circle density plot
        tickMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/VisContainer/Tick");
    }

    public override GameObject CreateVis(GameObject container)
    {
        // parameters for KDE method
        double sigma = 1; //default value mentioned in wikipedia about KDE
        int nsteps = 100; //default value mentioned in the KDE class 

        // Call the KDE method to calculate points
        double[,] calculated_points = KernelDensityEstimation.KDE(dataSets[0].ElementAt(0).Value, sigma, nsteps);


        // Extract the x and y values from the calculated_points
        double[] calculatedPoints_XValues = new double[nsteps];
        double[] calculatedPoints_YValues = new double[nsteps];
        for (int i = 0; i < nsteps; i++)
        {
            calculatedPoints_XValues[i] = calculated_points[i, 0];
            calculatedPoints_YValues[i] = calculated_points[i, 1];
        }

        // Display the visualization in Unity
        /*for (int i = 0; i < nsteps; i++)
        {
            // Create a data mark at each x, y point
            GameObject dataMark = Instantiate(dataMarkPrefab, new Vector3((float)x[i], (float)y[i], 0f), Quaternion.identity);
            // Set the data mark's position and scale according to your preference

            // Connect the points to form a continuous distribution line
            lineRenderer.positionCount = nsteps;
            lineRenderer.SetPosition(i, new Vector3((float)x[i], (float)y[i], 0f));
        }*/
        
        base.CreateVis(container);
        
        visContainer.CreateAxis(dataSets[0].ElementAt(0).Key, calculatedPoints_XValues, Direction.X);
        visContainer.CreateAxis(" ", calculatedPoints_YValues, Direction.Y);
        visContainer.CreateGrid(Direction.X, Direction.Y);

        
        visContainer.SetChannel(VisChannel.Color, calculatedPoints_YValues);


        visContainer.SetChannel(VisChannel.XPos, calculatedPoints_XValues);
        visContainer.SetChannel(VisChannel.YPos, calculatedPoints_YValues);

        // Draw a line connecting all the calculated points

        visContainer.CreateDataMarks(dataMarkPrefab);


        // Rescale Chart
        visContainerObject.transform.localScale = new Vector3(width, height, depth);

        return visContainerObject;
    }
}

