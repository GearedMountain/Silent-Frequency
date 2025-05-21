using UnityEngine;

public class holePunchShaderManager : MonoBehaviour
{
    public Material material;
    public Transform[] punchers; // Up to 64
    public float radius = 1f;

    void Update()
    {
        if (material == null || punchers == null) return;

        int count = Mathf.Min(punchers.Length, 64);
        Vector4[] centers = new Vector4[64]; // Preallocate all 64
        for (int i = 0; i < count; i++)
        {
            centers[i] = punchers[i].position;
        }

        material.SetVectorArray("_HoleCenters", centers);
        material.SetFloat("_HoleRadius", radius);
        material.SetFloat("_HoleCount", count);
    }
}
