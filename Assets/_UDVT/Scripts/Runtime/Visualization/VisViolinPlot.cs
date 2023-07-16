using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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
        // Get the data from the data set
        double[] data = dataSets[0].ElementAt(0).Value;
        
        // Parameters for KDE method
        double sigma = 1; // Default value mentioned in Wikipedia about KDE
        int nsteps = 100; // Default value mentioned in the KDE class

        // Call the KDE method to calculate points
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

            // Set the negative y values to be able to represent the rotated continuous distribution in the Grid XY.
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

            var dataInstance = marks[i].GetDataMarkInstance();
            dataInstance.SetActive(false); // Hide the default sphere marks
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
            mirrorPositions[i] = new Vector3(positions[i].x, -positions[i].y + minY * 2, positions[i].z);
        }

        int temp = positions.Length;
        Vector3[] Finalpositions = new Vector3[2 * marks.Count];
        for (int i = 0; i < marks.Count; i++)
        {
            Finalpositions[i] = positions[i];
            Finalpositions[i + nsteps] = mirrorPositions[marks.Count - i - 1];
        }

        lineRenderer.positionCount = 2 * positions.Length; // Set the number of vertices for the line renderer
        lineRenderer.SetPositions(Finalpositions); // Set the positions of the line renderer

        // Calculate statistics for violin plot
        ViolinStatistics statistics = new ViolinStatistics(data_x);

        violinContainer = new GameObject("ViolinContainer");
        violinContainer.transform.SetParent(container.transform);

        // Create new GameObjects for drawing the quartiles (median, 25%, 75%), maximum, and minimum
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

        float medianY = (float)statistics.getMedian();

        Vector3[] lineUpperQuartileMaxValue = new Vector3[2] { new Vector3((float)statistics.getUpperQuartile(), medianY, 0), new Vector3((float)statistics.getMaxValue(), medianY, 0)};
        LineRenderer lineRenderer2 = violinContainer.AddComponent<LineRenderer>();
        lineRenderer2.startWidth = 0.005f;
        lineRenderer2.endWidth = 0.005f;
        lineRenderer2.material.color = Color.white;
        lineRenderer2.positionCount = 2;
        lineRenderer2.SetPositions(lineUpperQuartileMaxValue);

        Vector3[] lineMinValueLowerQuartile = new Vector3[2] { new Vector3((float)statistics.getLowerQuartile(), medianY, 0), new Vector3((float)statistics.getMinValue(), medianY, 0)};
        LineRenderer lineMinValueLowerQuartileRender = q1Mark.AddComponent<LineRenderer>();
        lineMinValueLowerQuartileRender.startWidth = 0.005f;
        lineMinValueLowerQuartileRender.endWidth = 0.005f;
        lineMinValueLowerQuartileRender.material.color = Color.white;
        lineMinValueLowerQuartileRender.positionCount = 2;
        lineMinValueLowerQuartileRender.SetPositions(lineMinValueLowerQuartile);

        Vector3[] lineMaxValue = new Vector3[2] { new Vector3((float)statistics.getMaxValue(), medianY + 0.015f, 0), new Vector3((float)statistics.getMaxValue(), medianY - 0.015f, 0f)};
        LineRenderer lineRenderer3 = maxMark.AddComponent<LineRenderer>();
        lineRenderer3.startWidth = 0.005f;
        lineRenderer3.endWidth = 0.005f;
        lineRenderer3.positionCount = 2;
        lineRenderer3.SetPositions(lineMaxValue);

        Vector3[] violinBox = new Vector3[4] { new Vector3((float)statistics.getLowerQuartile(),medianY - 0.020f, 0), new Vector3((float)statistics.getLowerQuartile(), medianY + 0.020f, 0f), 
        new Vector3((float)statistics.getUpperQuartile(), medianY + 0.020f, 0f),  new Vector3((float)statistics.getUpperQuartile(), medianY - 0.020f, 0f)};
        LineRenderer lineviolinBox = q3Mark.AddComponent<LineRenderer>();
        lineviolinBox.startWidth = 0.005f;
        lineviolinBox.endWidth = 0.005f;
        lineviolinBox.loop=true;
        lineviolinBox.positionCount = 4;
        lineviolinBox.SetPositions(violinBox);

        Vector3[] medianLine = new Vector3[2] { new Vector3((float)statistics.getMedian(), medianY + 0.020f, 0f),  new Vector3((float)statistics.getMedian(), medianY - 0.020f, 0f)};
        LineRenderer medianLineRender = medianMark.AddComponent<LineRenderer>();
        medianLineRender.startWidth = 0.005f;
        medianLineRender.endWidth = 0.005f;
        medianLineRender.material.color = Color.red;
        medianLineRender.positionCount = 2;
        medianLineRender.SetPositions(medianLine);

        Vector3[] minValueLine = new Vector3[2] { new Vector3((float)statistics.getMinValue(), medianY + 0.015f, 0f),  new Vector3((float)statistics.getMinValue(), medianY - 0.015f, 0f)};
        LineRenderer minValueLineRender = minMark.AddComponent<LineRenderer>();
        minValueLineRender.startWidth = 0.005f;
        minValueLineRender.endWidth = 0.005f;
        minValueLineRender.positionCount = 2;
        minValueLineRender.SetPositions(minValueLine);

        // Color the area between the data marks and the median line
        MeshRenderer meshRenderer = medianMark.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = Color.gray; 
    
        MeshFilter meshFilter = medianMark.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        // Calculate the vertices for the area between the data marks and the median line
        int numVertices = marks.Count * 2 + 3; 
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[(numVertices - 2) * 3]; // Each triangle consists of 3 vertices
        Color[] colors = new Color[numVertices];

        // Initialize index counters for vertices and triangles
        int vertexIndex = 0;
        int triangleIndex = 0;

        // Set the vertices and colors for the area below the median line
        for (int i = 0; i < mirrorPositions.Length; i++)
        {
            if (mirrorPositions[i].y < medianY)
            {
                vertices[vertexIndex] = mirrorPositions[i];
                vertices[vertexIndex + 1] = new Vector3(mirrorPositions[i].x, medianY, mirrorPositions[i].z);

                // Set the triangles for the area below the median line
                if (i < mirrorPositions.Length - 1 )
                {
                    // First Triangle
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 2] = vertexIndex + 2;

                    // Second triangle
                    triangles[triangleIndex + 3] = vertexIndex + 1;
                    triangles[triangleIndex + 4] = vertexIndex + 3;
                    triangles[triangleIndex + 5] = vertexIndex + 2;
                    triangleIndex += 6;              
                }

                vertexIndex += 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        mesh.uv = uvs;
        meshFilter.mesh = mesh;

        vertexIndex = 0;
        triangleIndex = 0;

        violinContainer = new GameObject("ViolinContainer");
        violinContainer.transform.SetParent(container.transform);

        MeshRenderer meshRenderer2 = violinContainer.AddComponent<MeshRenderer>();
        meshRenderer2.material = new Material(Shader.Find("Standard"));
        meshRenderer2.material.color = Color.gray;

        MeshFilter meshFilter2 = violinContainer.AddComponent<MeshFilter>();
        Mesh mesh2 = meshFilter2.mesh;
        Vector3[] vertices2 = new Vector3[numVertices];
        int[] triangles2 = new int[(numVertices - 2) * 3];

        // Set the vertices and colors for the area above the median line
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].y > medianY)
            { 
                vertices2[vertexIndex] = positions[i];
                vertices2[vertexIndex + 1] = new Vector3(positions[i].x, medianY, positions[i].z);

                // Set the triangles for the area above the median line
                if (i < positions.Length - 1)
                {    
                    // First Triangle
                    triangles2[triangleIndex] = vertexIndex;
                    triangles2[triangleIndex + 1] = vertexIndex + 2;
                    triangles2[triangleIndex + 2] = vertexIndex + 1;

                    // Second Triangle
                    triangles2[triangleIndex + 3] = vertexIndex + 1;
                    triangles2[triangleIndex + 4] = vertexIndex + 2;
                    triangles2[triangleIndex + 5] = vertexIndex + 3;

                    triangleIndex += 6;                 
                }

                vertexIndex += 2;
            }              
        }

        mesh2.vertices = vertices2;
        mesh2.triangles = triangles2;

        Vector2[] uvs2 = new Vector2[vertices2.Length];

        for (int i = 0; i < uvs2.Length; i++)
        {
            uvs2[i] = new Vector2(vertices2[i].x, vertices2[i].z);
        }

        mesh2.uv = uvs2;
        meshFilter2.mesh = mesh2;

        return visContainerObject;
    }
}
