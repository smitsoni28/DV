// using System.Linq;
// using UnityEngine;

// public class VisViolinplot : Vis
// {
//     public VisViolinplot()
//     {
//         title = "VisViolinplot";
//         // Define Data Mark and Tick Prefab
//         dataMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/Marks/Cube");// cube is choosen to obtain the box plot
//         tickMarkPrefab = (GameObject)Resources.Load("Prefabs/DataVisPrefabs/VisContainer/Tick");
//     }

//     public override GameObject CreateVis(GameObject container)
//     {
        
//         // parameters for KDE method
//         double sigma = 1; // Default value mentioned in Wikipedia about KDE
//         int nsteps = 100; // Default value mentioned in the KDE class

//         // Call the KDE method to calculate points
//         // Pass the value of the first element in the first dataset, sigma, and nsteps to calculate points using KDE
//         double[,] calculated_points = KernelDensityEstimation.KDE(dataSets[0].ElementAt(0).Value, sigma, nsteps);
//         ViolinStatistics violinPlotCalculation = new ViolinStatistics(dataSets[0].ElementAt(0).Value);
    
        

//         // Set the positions of the line renderer to the calculated points
//         Vector3[] linePositions = new Vector3[nsteps];

//         // Extract the x and y values from the calculated_points
//         double[] calculatedPoints_XValues = new double[nsteps];
//         double[] calculatedPoints_YValues = new double[nsteps];
//         for (int i = 0; i < nsteps; i++)
//         {
//             calculatedPoints_XValues[i] = calculated_points[i, 0];
//             calculatedPoints_YValues[i] = calculated_points[i, 1];
//         }

//         // Create Axes and Grids
//         // Call the base class method to create the visualization container
//         base.CreateVis(container);

//         // Create X and Y axes using the calculated x and y values
//         //visContainer.CreateAxis(dataSets[0].ElementAt(0).Key, calculatedPoints_XValues, Direction.X);

//         visContainer.CreateAxis(" ", calculatedPoints_YValues, Direction.Y);

//         // Create grids based on the X and Y axes
//         //visContainer.CreateGrid(Direction.X, Direction.Y);

//         // Set colors and Vis channel
//         // Set the color channel based on the calculated y values
//         visContainer.SetChannel(VisChannel.Color, calculatedPoints_YValues);

//         // Set X and Y position channels to the calculated x and y values
//         visContainer.SetChannel(VisChannel.XPos, calculatedPoints_XValues);
//         visContainer.SetChannel(VisChannel.YPos, calculatedPoints_YValues);

//         // Plot all calculated points
//         // Create data marks using the data mark prefab
//         visContainer.CreateDataMarks(dataMarkPrefab);

//         // Create quartile mark
//         GameObject quartileMark = new GameObject("QuartileMark");
//         quartileMark.name = "QuartileMark";
//         quartileMark.transform.SetParent(visContainer.transform);
//         quartileMark.transform.localScale = new Vector3(0.5f, (float)violinPlotCalculation.getIQR(), 0.5f);
//         quartileMark.transform.position = new Vector3(0f, (float)violinPlotCalculation.getMedian(), 0f);

//         // Create min/max mark
//         minMaxMark = GameObject.CreatePrimitive(PrimitiveType.Cube);
//         minMaxMark.name = "MinMaxMark";
//         minMaxMark.transform.SetParent(visContainer.transform);
//         minMaxMark.transform.localScale = new Vector3(1f, (float)violinPlotCalculation.getMaxValue() - (float)violinPlotCalculation.getMinValue(), 0.5f);
//         minMaxMark.transform.position = new Vector3(0f, ((float)violinPlotCalculation.getMinValue() + (float)violinPlotCalculation.getMaxValue()) / 2f, 0f);

//         // Create a line renderer to draw the line connecting the data marks
//         LineRenderer lineRenderer = visContainerObject.AddComponent<LineRenderer>();
//         lineRenderer.material.color = Color.black;
//         lineRenderer.startWidth = 0.005f;
//         lineRenderer.endWidth = 0.005f;

//         // Get the positions of the data marks and set them as the positions for the line renderer
//         var marks = visContainer.dataMarkList;
//         Vector3[] positions = new Vector3[marks.Count];
//         for (int i = 0; i < marks.Count; ++i)
//         {
//             positions[i] = marks[i].GetDataMarkChannel().position;
//         }

//         lineRenderer.positionCount = positions.Length; // Set the number of vertices for the line renderer
//         lineRenderer.SetPositions(positions); // Set the positions of the line renderer

//         return visContainerObject;
//     }
// }


using System.Linq;
using UnityEngine;
using System;

public class VisViolinPlot : Vis
{
    public VisViolinPlot()
    {
        title = "ViolinPlot";
        dataMarkPrefab = Resources.Load<GameObject>("Prefabs/DataVisPrefabs/Marks/Cube");
        tickMarkPrefab = Resources.Load<GameObject>("Prefabs/DataVisPrefabs/VisContainer/Tick");
    }

    public override GameObject CreateVis(GameObject container)
    {
        base.CreateVis(container);

        double[] data = dataSets[0].ElementAt(0).Value;
        Array.Sort(data);

        // parameters for KDE method
        double sigma = 1; // Default value mentioned in Wikipedia about KDE
        int nsteps = 100; // Default value mentioned in the KDE class

        // Call the KDE method to calculate points
        // Pass the value of the first element in the first dataset, sigma, and nsteps to calculate points using KDE
        double[,] calculated_points = KernelDensityEstimation.KDE(data, sigma, nsteps);

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
            Debug.Log("Pos " + positions[i]);
        }

        float minY = 10000; // Initialize minY with a large value

        for (int i = 1; i < positions.Length; i++)
        {
            if (positions[i].y < minY)
            {
                minY = positions[i].y;
            }
        }
        // Create a mirror image of the positions by negating the X position values
        Vector3[] mirrorPositions = new Vector3[marks.Count];
        for (int i = 0; i < marks.Count; i++)
        {
            mirrorPositions[i] = new Vector3(positions[i].x, minY-positions[i].y, positions[i].z);
            Debug.Log("Pos " + positions[i]);
            Debug.Log("Mirror Pos " + mirrorPositions[i]);
        }

        Debug.Log("Marks Count" + marks.Count);

        int temp = positions.Length;
        Vector3[] Finalpositions = new Vector3[2*marks.Count];
        for (int i = 0; i < marks.Count; i++)
        {
            Finalpositions[i]=positions[i];
            Finalpositions[i+nsteps]=mirrorPositions[marks.Count-i-1];
            Debug.Log("Final Mirror Pos " + Finalpositions[i] + Finalpositions[i+nsteps]);
        }
         lineRenderer.positionCount = 2*positions.Length; // Set the number of vertices for the line renderer
        lineRenderer.SetPositions(Finalpositions); // Set the positions of the line renderer
        
        
        // mirrorLineRenderer.positionCount = mirrorPositions.Length; // Set the number of vertices for the mirror line renderer
        // mirrorLineRenderer.SetPositions(mirrorPositions); // Set the positions of the mirror line renderer
        
        
        // Calculate statistics for violin plot
        //ViolinStatistics statistics = new ViolinStatistics(data);
        
        
        // // Create quartile mark
        // GameObject quartileMark = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // quartileMark.name = "QuartileMark";
        // quartileMark.transform.SetParent(visContainerObject.transform);
        // quartileMark.transform.localScale = new Vector3(0.5f, (float)statistics.getIQR(), 0.5f);
        // quartileMark.transform.position = new Vector3(0f, (float)statistics.getMedian(), 0f);

        // // Create min/max mark
        // GameObject minMaxMark = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // minMaxMark.name = "MinMaxMark";
        // minMaxMark.transform.SetParent(visContainerObject.transform);
        // minMaxMark.transform.localScale = new Vector3(1f, (float)(statistics.getMaxValue() - statistics.getMinValue()), 0.5f);
        // minMaxMark.transform.position = new Vector3(0f, (float)((statistics.getMinValue() + statistics.getMaxValue()) / 2f), 0f);


        // Debug.Log("Median: " + statistics.getMedian());
        // Debug.Log("Lower Quartile: " + statistics.getLowerQuartile());
        // Debug.Log("Upper Quartile: " + statistics.getUpperQuartile());
        // Debug.Log("IQR: " + statistics.getIQR());
        // Debug.Log("Min Value: " + statistics.getMinValue());
        // Debug.Log("Max Value: " + statistics.getMaxValue());
        // // for(int i=0; i<data.Length;i++)
        // {
        //     Debug.Log("Element "+ data[i]);
        // }


        return visContainerObject;
    }
}
