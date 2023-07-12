using System.Linq;
using UnityEngine;
using System;

public class VisViolinPlot : Vis
{   
    public GameObject violinContainer;
    public GameObject minMark;
    public GameObject maxMark;
    public GameObject medianMark;
    public GameObject iqrMark;
    public GameObject q1Mark;
    public GameObject q3Mark;

    public VisViolinPlot()
    {
        title = "ViolinPlot";
        dataMarkPrefab = Resources.Load<GameObject>("Prefabs/DataVisPrefabs/Marks/Sphere");
        tickMarkPrefab = Resources.Load<GameObject>("Prefabs/DataVisPrefabs/VisContainer/Tick");
    }

    public override GameObject CreateVis(GameObject container)
    {

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
        double[] calculatedPoints_YValues = new double[2*nsteps];
        for (int i = 0; i < nsteps; i++)
        {
            calculatedPoints_XValues[i] = calculated_points[i, 0];
            
            calculatedPoints_YValues[i] = calculated_points[i, 1];

            //set the negative y values to be able to represent the rotated continuous distribution in the Grid XY.
            calculatedPoints_YValues[calculatedPoints_YValues.Length - 1 - i] = -calculatedPoints_YValues[i]; 
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
        //visContainer.SetChannel(VisChannel.XRotation, calculatedPoints_XValues);

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
        double[] data_x = new double[marks.Count]; // set the scaled x points for calculating or getting the quartiles (median, 25%, 75%), maximum and minimum values.
        for (int i = 0; i < marks.Count; ++i)
        {
            positions[i] = marks[i].GetDataMarkChannel().position;
            data_x[i] = positions[i].x;
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
            mirrorPositions[i] = new Vector3(positions[i].x, -positions[i].y + minY*2 , positions[i].z);
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
        

        // Calculate statistics for violin plot
        ViolinStatistics statistics = new ViolinStatistics(data_x);
        
        violinContainer = new GameObject("ViolinContainer");
        violinContainer.transform.SetParent(container.transform);

         // create new GameObjects for drawing the quartiles (median, 25%, 75%), maximum and minimum
        medianMark = new GameObject("medianMark");       
        medianMark.transform.SetParent(violinContainer.transform);
              
        minMark = new GameObject("minMark");
        minMark.transform.SetParent(violinContainer.transform);
        
        maxMark = new GameObject("MaxMark");
        maxMark.transform.SetParent(container.transform);
        
        q3Mark = new GameObject("Q3Mark");
        q3Mark.transform.SetParent(container.transform);

        q1Mark = new GameObject("Q1Mark");;
        q1Mark.transform.SetParent(violinContainer.transform);
  

         Debug.Log("Median: " + statistics.getMedian());
         Debug.Log("Lower Quartile: " + statistics.getLowerQuartile());
         Debug.Log("Upper Quartile: " + statistics.getUpperQuartile());
         Debug.Log("IQR: " + statistics.getIQR());
         Debug.Log("Min Value: " + statistics.getMinValue());
         Debug.Log("Max Value: " + statistics.getMaxValue());

        Vector3[] lineUpperQuartileMaxValue = new Vector3[2] { new Vector3((float)statistics.getUpperQuartile(), 0.5f, 0), new Vector3((float)statistics.getMaxValue(), 0.5f, 0)};
        LineRenderer lineRenderer2 = violinContainer.AddComponent<LineRenderer>();
        lineRenderer2.startWidth = 0.005f;
        lineRenderer2.endWidth = 0.005f;
        lineRenderer2.material.color = Color.blue;
        lineRenderer2.positionCount = 2;
        lineRenderer2.SetPositions(lineUpperQuartileMaxValue);

        Vector3[] lineMinValueLowerQuartile = new Vector3[2] { new Vector3((float)statistics.getLowerQuartile(), 0.5f, 0), new Vector3((float)statistics.getMinValue(), 0.5f, 0)};
        LineRenderer lineMinValueLowerQuartileRender = q1Mark.AddComponent<LineRenderer>();
        lineMinValueLowerQuartileRender.startWidth = 0.005f;
        lineMinValueLowerQuartileRender.endWidth = 0.005f;
        lineMinValueLowerQuartileRender.material.color = Color.blue;
        lineMinValueLowerQuartileRender.positionCount = 2;
        lineMinValueLowerQuartileRender.SetPositions(lineMinValueLowerQuartile);

        
        Vector3[] lineMaxValue = new Vector3[2] { new Vector3((float)statistics.getMaxValue(), 0.515f, 0), new Vector3((float)statistics.getMaxValue(), 0.485f, 0f)};
        LineRenderer lineRenderer3 = maxMark.AddComponent<LineRenderer>();
        lineRenderer3.startWidth = 0.005f;
        lineRenderer3.endWidth = 0.005f;
        lineRenderer3.positionCount = 2;
        lineRenderer3.SetPositions(lineMaxValue);

        Vector3[] violinBox = new Vector3[4] { new Vector3((float)statistics.getLowerQuartile(), 0.480f, 0), new Vector3((float)statistics.getLowerQuartile(), 0.520f, 0f), 
            new Vector3((float)statistics.getUpperQuartile(), 0.520f, 0f),  new Vector3((float)statistics.getUpperQuartile(), 0.480f, 0f)};
        LineRenderer lineviolinBox = q3Mark.AddComponent<LineRenderer>();
        lineviolinBox.startWidth = 0.005f;
        lineviolinBox.endWidth = 0.005f;
        lineviolinBox.loop=true;
        lineviolinBox.positionCount = 4;
        lineviolinBox.SetPositions(violinBox);

        Vector3[] medianLine = new Vector3[2] { new Vector3((float)statistics.getMedian(), 0.520f, 0f),  new Vector3((float)statistics.getMedian(), 0.480f, 0f)};
        LineRenderer medianLineRender = medianMark.AddComponent<LineRenderer>();
        medianLineRender.startWidth = 0.005f;
        medianLineRender.endWidth = 0.005f;
        medianLineRender.positionCount = 2;
        medianLineRender.SetPositions(medianLine);

        Vector3[] minValueLine = new Vector3[2] { new Vector3((float)statistics.getMinValue(), 0.515f, 0f),  new Vector3((float)statistics.getMinValue(), 0.485f, 0f)};
        LineRenderer minValueLineRender = minMark.AddComponent<LineRenderer>();
        minValueLineRender.startWidth = 0.005f;
        minValueLineRender.endWidth = 0.005f;
        minValueLineRender.positionCount = 2;
        minValueLineRender.SetPositions(minValueLine);
        
        return visContainerObject;
    }
}
