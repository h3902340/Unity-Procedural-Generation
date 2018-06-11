using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public Material leaf_mat;
    public void BuildPlane()
    {
        if (GetComponent<MeshFilter>() == null) gameObject.AddComponent<MeshFilter>();
        if (GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
            GetComponent<MeshRenderer>().material = leaf_mat;
        }

        float length = 0.25f;
        float width = 0.5f;
        int resX = 2; // 2 minimum
        int resZ = 2;

        #region Vertices		
        Vector3[] vertices = new Vector3[2 * resX * resZ];
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1) - .5f) * length;
            for (int x = 0; x < resX; x++)
            {
                // [ -width / 2, width / 2 ]
                float xPos = ((float)x / (resX - 1) - .5f) * width;
                vertices[x + z * resX] = new Vector3(xPos, 0f, zPos);
            }
        }
        #endregion

        #region Normales
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < resX * resZ; n++)
            normales[n] = Vector3.up;
        #endregion

        #region UVs		
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }
        #endregion

        #region Triangles
        int nbFaces = 2 * (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
        for (int face = 0; face < (resX - 1) * (resZ - 1); face++)
        {
            // Retrieve lower left corner from face ind
            int i = face % (resX - 1) + (face / (resZ - 1) * resX);

            triangles[t++] = i + resX;
            triangles[t++] = i + 1;
            triangles[t++] = i;

            triangles[t++] = i + resX;
            triangles[t++] = i + resX + 1;
            triangles[t++] = i + 1;
        }
        #endregion

        // the other side
        #region Vertices		
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1) - .5f) * length;
            for (int x = 0; x < resX; x++)
            {
                // [ -width / 2, width / 2 ]
                float xPos = ((float)x / (resX - 1) - .5f) * width;
                vertices[z + x * resZ + resX * resZ] = new Vector3(xPos, -0.01f, zPos);
            }
        }
        #endregion

        #region Normales
        for (int n = resX * resZ; n < 2 * resX * resZ; n++)
            normales[n] = Vector3.up;
        #endregion

        #region UVs		
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[v + u * resZ + resX * resZ] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }
        #endregion

        #region Triangles
        for (int face = (resX - 1) * (resZ - 1); face < 2 * (resX - 1) * (resZ - 1); face++)
        {
            // Retrieve lower left corner from face ind
            int i = face % (resX - 1) + (face / (resZ - 1) * resX);

            triangles[t++] = i +5;
            triangles[t++] = i +3;
            triangles[t++] = i+2;

            triangles[t++] = i+4;
            triangles[t++] = i + 5;
            triangles[t++] = i+2;
        }
        #endregion

#if UNITY_EDITOR
        //Only do this in the editor
        //a better way of getting the meshfilter using Generics
        MeshFilter mf = GetComponent<MeshFilter>();                  //Assign the copy to the meshes
#else
     //do this in play mode
     mesh = GetComponent<MeshFilter>().mesh;
#endif
        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mf.mesh = mesh;
    }
}
