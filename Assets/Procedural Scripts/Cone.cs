using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Cone : MonoBehaviour
{
    [HideInInspector]
    public int HeightSegment = 8;
    [HideInInspector]
    public float bottomRadius = .25f;
    [HideInInspector]
    public float topRadius = .05f;
    [HideInInspector]
    public Material bark_mat;
    int hs;
    float hsf;
    float hs_main;
    float bottom_r;
    float top_r;
    public int BranchLowerBound = 2;
    public int BranchUpperBound = 2;
    public int BranchInterval = 1;

    public void GrowLeaf()
    {
        for(float i = 0; i < HeightSegment-1; i+= .5f)
        {
            for(int j = 0; j < 3; j++)
            {
                GameObject leaf = new GameObject();
                leaf.transform.SetParent(transform);

                leaf.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 90)+90*j+30, 0);
                leaf.transform.localPosition = new Vector3(0, (i + 0.9f) / 3, 0);
                leaf.transform.position += -leaf.transform.right * (0.225f + (topRadius * (i + 0.9f) / HeightSegment + bottomRadius * (1 - (i + 0.9f) / HeightSegment)));
                leaf.AddComponent<Plane>();
                leaf.GetComponent<Plane>().BuildPlane();
            }
        } 
    }

    public void GrowBranch()
    {
        if (HeightSegment <= BranchUpperBound + BranchLowerBound)
        {
            GrowLeaf();
            return;
        }
        for (int hs = BranchUpperBound; hs < HeightSegment - BranchLowerBound; hs += BranchInterval)
        {
            Branch(HeightSegment, hs, bottomRadius, 120 * hs+Random.Range(0,30));
        }
    }

    public void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void Branch(float hs_main, int hs, float bottomRadius, float y_rot)
    {
        hsf = hs;
        bottom_r = (bottomRadius * hsf / hs_main + topRadius * (1 - hsf / hs_main));
        top_r = 0;
        if (bottom_r <= 0 || top_r < 0) return;
        if (bottom_r <= top_r) top_r = 0;
        GameObject branch = new GameObject();
        branch.transform.SetParent(transform);
        branch.transform.localPosition = new Vector3(0, (hs_main - hsf) / 3, 0);
        branch.transform.localRotation = Quaternion.Euler(60, y_rot, 0);

        branch.AddComponent<Cone>();
        branch.GetComponent<Cone>().HeightSegment = hs;
        branch.GetComponent<Cone>().bottomRadius = bottom_r;
        branch.GetComponent<Cone>().topRadius = top_r;
        branch.GetComponent<Cone>().BuildCone(branch, hs, bottom_r, top_r);
        branch.GetComponent<Cone>().GrowBranch();
    }

    public void BuildCone(GameObject g, int HeightSegment, float bottomRadius, float topRadius)
    {
        if (g.GetComponent<MeshFilter>() == null) g.AddComponent<MeshFilter>();
        if (g.GetComponent<MeshRenderer>() == null)
        {
            g.AddComponent<MeshRenderer>();
            g.GetComponent<MeshRenderer>().material = bark_mat;
        }


        float nbH = HeightSegment;
        float height = nbH / 3;

        int nbSides = 18;

        int nbVerticesCap = nbSides + 1;
        #region Vertices

        // bottom + top + sides
        Vector3[] vertices = new Vector3[nbVerticesCap + nbVerticesCap + nbSides * HeightSegment * 2 + 2];
        int vert = 0;
        float _2pi = Mathf.PI * 2f;

        // Bottom cap
        vertices[vert++] = new Vector3(0f, 0f, 0f);
        while (vert <= nbSides)
        {
            float rad = (float)vert / nbSides * _2pi;
            vertices[vert] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0f, Mathf.Sin(rad) * bottomRadius);
            vert++;
        }

        // Top cap
        vertices[vert++] = new Vector3(0f, height, 0f);
        while (vert <= nbSides * 2 + 1)
        {
            float rad = (float)(vert - nbSides - 1) / nbSides * _2pi;
            vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
            vert++;
        }

        // Sides
        int v = 0;
        float ii = 1;
        while (vert <= vertices.Length - 4)
        {
            float rad = (float)v / nbSides * _2pi;

            vertices[vert] =  new Vector3(Mathf.Cos(rad) * (topRadius * ii + bottomRadius * (nbH - ii)) / nbH, height * ii / nbH, Mathf.Sin(rad) * (topRadius * ii + bottomRadius * (nbH - ii)) / nbH);
            vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * (topRadius * (ii - 1) + bottomRadius * (nbH - ii + 1)) / nbH, height * (ii - 1) / nbH, Mathf.Sin(rad) * (topRadius * (ii - 1) + bottomRadius * (nbH - ii + 1)) / nbH);
            vert += 2;
            v++;
            if (v % nbSides == 0)
            {
                if (ii < nbH)
                {
                    ii++;
                }
            }
        }
        vertices[vert] = vertices[HeightSegment * nbSides * 2 + 2];
        vertices[vert + 1] = vertices[HeightSegment * nbSides * 2 + 3];
        #endregion

        #region Normales

        // bottom + top + sides
        Vector3[] normales = new Vector3[vertices.Length];
        vert = 0;

        // Bottom cap
        while (vert <= nbSides)
        {
            normales[vert++] = Vector3.down;
        }

        // Top cap
        while (vert <= nbSides * 2 + 1)
        {
            normales[vert++] = Vector3.up;
        }

        // Sides
        v = 0;
        while (vert <= vertices.Length - 4)
        {
            float rad = (float)v / nbSides * _2pi;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);

            normales[vert] = new Vector3(cos, 0f, sin);
            normales[vert + 1] = normales[vert];

            vert += 2;
            v++;
        }
        normales[vert] = normales[HeightSegment * nbSides * 2 + 2];
        normales[vert + 1] = normales[HeightSegment * nbSides * 2 + 3];
        #endregion

        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];

        // Bottom cap
        int u = 0;
        uvs[u++] = new Vector2(0.5f, 0.5f);
        while (u <= nbSides)
        {
            float rad = (float)u / nbSides * _2pi;
            uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
            u++;
        }

        // Top cap
        uvs[u++] = new Vector2(0.5f, 0.5f);
        while (u <= nbSides * 2 + 1)
        {
            float rad = (float)u / nbSides * _2pi;
            uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
            u++;
        }

        // Sides
        ii = 1;
        float tile = 1 / 3f;
        while (u <= uvs.Length - 4)
        {
            for (int j = 0; j < 6 * HeightSegment; j++)
            {
                if (j % 6 >= 3)
                {
                    uvs[u + 6 * j] = new Vector3(tile * (6 - (j % 6)), 0);
                    uvs[u + 1 + 6 * j] = new Vector3(tile * (6 - (j % 6)), 1);
                    uvs[u + 2 + 6 * j] = new Vector3(tile * (5 - (j % 6)), 0);
                    uvs[u + 3 + 6 * j] = new Vector3(tile * (5 - (j % 6)), 1);
                    uvs[u + 4 + 6 * j] = new Vector3(tile * (6 - (j % 6)), 0);
                    uvs[u + 5 + 6 * j] = new Vector3(tile * (6 - (j % 6)), 1);
                }
                else
                {
                    uvs[u + 6 * j] = new Vector3(tile * (j % 6), 0);
                    uvs[u + 1 + 6 * j] = new Vector3(tile * (j % 6), 1);
                    uvs[u + 2 + 6 * j] = new Vector3(tile * (j % 6 + 1), 0);
                    uvs[u + 3 + 6 * j] = new Vector3(tile * (j % 6 + 1), 1);
                    uvs[u + 4 + 6 * j] = new Vector3(tile * (j % 6), 0);
                    uvs[u + 5 + 6 * j] = new Vector3(tile * (j % 6), 1);
                }
            }

            break;
        }
        //uvs[u] = new Vector2(1f, 1f);
        //uvs[u + 1] = new Vector2(1f, 0f);
        #endregion

        #region Triangles
        int nbTriangles = nbSides + nbSides + HeightSegment * nbSides * 2;
        int[] triangles = new int[nbTriangles * 3 + 3];

        // Bottom cap
        int tri = 0;
        int i = 0;
        while (tri < nbSides - 1)
        {
            triangles[i] = 0;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = tri + 2;
            tri++;
            i += 3;
        }
        triangles[i] = 0;
        triangles[i + 1] = tri + 1;
        triangles[i + 2] = 1;
        tri++;
        i += 3;

        // Top cap
        //tri++;
        while (tri < nbSides * 2)
        {
            triangles[i] = tri + 2;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = nbVerticesCap;
            tri++;
            i += 3;
        }

        triangles[i] = nbVerticesCap + 1;
        triangles[i + 1] = tri + 1;
        triangles[i + 2] = nbVerticesCap;
        tri++;
        i += 3;
        tri++;

        // Sides
        while (tri <= nbTriangles)
        {
            triangles[i] = tri + 2;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = tri + 0;
            tri++;
            i += 3;

            triangles[i] = tri + 1;
            triangles[i + 1] = tri + 2;
            triangles[i + 2] = tri + 0;
            tri++;
            i += 3;
            if (tri % (nbSides * 2) == 0)
            {
                triangles[i] = tri + 2 - 2 * nbSides;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = tri + 0;
                tri++;
                i += 3;

                triangles[i] = tri + 1 - 2 * nbSides;
                triangles[i + 1] = tri + 2 - 2 * nbSides;
                triangles[i + 2] = tri + 0;
                tri++;
                i += 3;
            }
        }
        #endregion
#if UNITY_EDITOR
        //Only do this in the editor
        //a better way of getting the meshfilter using Generics
        MeshFilter mf = g.GetComponent<MeshFilter>();                  //Assign the copy to the meshes
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
        //mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        Unwrapping.GenerateSecondaryUVSet(mesh);
        mf.mesh = mesh;
    }
}
