using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

public class VisHorizonGraph : Vis
{   
    public GameObject violinContainer;
    public GameObject minMark;
    public GameObject maxMark;
    public GameObject medianMark;
    public GameObject iqrMark;
    public GameObject q1Mark;
    public GameObject q3Mark;

    public VisHorizonGraph()
    {
        title = "HorizonGraph";
        dataMarkPrefab = Resources.Load<GameObject>("Prefabs/DataVisPrefabs/Marks");
        tickMarkPrefab = Resources.Load<GameObject>("Prefabs/DataVisPrefabs/VisContainer/Tick");
    }

    // Create the Horizon Graph visualization
    public override GameObject CreateVis(GameObject container)
    {
        // Retrieve the data from the dataset
        double[] data = dataSets[0].ElementAt(0).Value;
        Array.Sort(data);

        // Create a new GameObject as the visContainerObject
        GameObject visContainerObject = new GameObject("VisContainer");
        visContainerObject.transform.SetParent(container.transform);

        // Set parameters for KDE method
        double sigma = 1; // Default value mentioned in Wikipedia about KDE
        int nsteps = 100; // Default value mentioned in the KDE class

        // Call the KDE method to calculate points
        double[,] calculated_points = KernelDensityEstimation.KDE(data, sigma, nsteps);

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
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;

        // Get the positions of the data marks and set them as the positions for the line renderer
        var marks = visContainer.dataMarkList;
        Vector3[] positions = new Vector3[marks.Count+1];
        double[] data_x = new double[marks.Count]; // set the scaled x points for calculating or getting the quartiles (median, 25%, 75%), maximum and minimum values.
        for (int i = 0; i < marks.Count; ++i)
        {
            positions[i] = marks[i].GetDataMarkChannel().position;
            data_x[i] = positions[i].x;
          
            var dataInstance = marks[i].GetDataMarkInstance();
            dataInstance.SetActive(false); // Hide the default sphere marks
        }
        
        float minY = 10000; // Initialize minY with a large value

        // Find the minimum y value
        for (int i = 1; i < positions.Length; i++)
        {
            if (positions[i].y < minY)
            {
                minY = positions[i].y;
            }
        }

        // Add a straight line at the median value
        ViolinStatistics statistics = new ViolinStatistics(data_x);
        float medianY = (float)statistics.getLowerQuartile(); // Assuming the median value is stored in the first dataset, first element
        Vector3 medianPosition = new Vector3(positions[0].x, medianY, positions[0].z);
        positions[0] = medianPosition;
        medianPosition = new Vector3(positions[marks.Count-1].x, medianY, positions[marks.Count-1].z);
        positions[marks.Count] = medianPosition;
        lineRenderer.positionCount = positions.Length; // Set the number of vertices for the line renderer
        lineRenderer.SetPositions(positions); // Set the positions of the line renderer

        Vector3[] medianLinePositions = new Vector3[2]
        {
            new Vector3(positions[0].x, medianY, positions[0].z),
            new Vector3(positions[positions.Length - 2].x, medianY, positions[positions.Length - 2].z)
        };

        // Create a line renderer for the median line
        LineRenderer medianLineRenderer = container.AddComponent<LineRenderer>();
        medianLineRenderer.material.color = Color.black;
        medianLineRenderer.startWidth = 0.005f;
        medianLineRenderer.endWidth = 0.005f;
        medianLineRenderer.positionCount = 2;
        medianLineRenderer.SetPositions(medianLinePositions);

        // Color the area between the data marks and the median line
        MeshRenderer meshRenderer = visContainerObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material.color = Color.blue; 
        
        MeshFilter meshFilter = visContainerObject.AddComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        // Calculate the vertices for the area between the data marks and the median line
        int numVertices = marks.Count * 2 + 3; // Each data mark and median line position contributes 2 vertices
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[(numVertices - 2) * 3]; // Each triangle consists of 3 vertices
        Color[] colors = new Color[numVertices];

        // Initialize index counters for vertices and triangles
        int vertexIndex = 0;
        int triangleIndex = 0;

        // Set the vertices and colors for the area below the median line
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i].y <= medianY)
            {
                vertices[vertexIndex] = positions[i];
                vertices[vertexIndex + 1] = new Vector3(positions[i].x, medianY, positions[i].z);

                colors[vertexIndex] = Color.blue;
                colors[vertexIndex + 1] = Color.blue;

                // Set the triangles for the area below the median line
                if (i < positions.Length - 1 && positions[i+1].y <= medianY)
                {
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
        meshFilter.mesh.colors = colors;

        vertexIndex = 0;
        triangleIndex = 0;

        // Create a container for the violin shape
        violinContainer = new GameObject("ViolinContainer");
        violinContainer.transform.SetParent(container.transform);

        // Set the mesh renderer properties for the violin shape
        MeshRenderer meshRenderer2 = violinContainer.AddComponent<MeshRenderer>();
        meshRenderer2.material = new Material(Shader.Find("Standard"));
        meshRenderer2.material.color = Color.red;

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

                colors[vertexIndex] = Color.red;
                colors[vertexIndex + 1] = Color.red;
                
                // Set the triangles for the area above the median line
                if (i < positions.Length - 1)
                {    
                    //First Triangle
                    triangles2[triangleIndex] = vertexIndex;
                    triangles2[triangleIndex + 1] = vertexIndex + 2;
                    triangles2[triangleIndex + 2] = vertexIndex + 1;

                    //Second Triangle
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
        meshFilter2.mesh.colors = colors;

        return visContainerObject;
    }
}
