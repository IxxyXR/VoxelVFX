using System.IO;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class VoxelVolume : MonoBehaviour
{

    public string path;
    
    private VisualEffect _vfx;
    
    // Start is called before the first frame update
    void Start()
    {
        _vfx = gameObject.GetComponent<VisualEffect>();
        DrawVoxel();
    }
    
    public void DrawVoxel()
    {
        MVMainChunk chunk = LoadVOX($"Assets\\{path}", false);
        var texture = new Texture3D(chunk.sizeX, chunk.sizeY, chunk.sizeZ, TextureFormat.RGBAFloat, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        
        for (int x=0; x<chunk.sizeX; x++) 
        {
            for (int y = 0; y < chunk.sizeY; y++)
            {
                for (int z = 0; z < chunk.sizeZ; z++)
                {
                    var voxel = chunk.voxelChunk.voxels[x, y, z];
                    var color = chunk.palatte[voxel];
                    color.a = voxel == 0 ? 0f : 2f;
                    texture.SetPixel(x, y, z, color);
//                    Debug.Log($"{x},{y},{z}: {voxel} - {color}");
                }
            }
        }        
        texture.Apply();
        _vfx.SetTexture("Voxel Texture", texture);
        _vfx.SetVector3("Voxel Texture Size", new Vector3(texture.width, texture.height, texture.depth));
    }
    
    public static MVMainChunk LoadVOX(string path, bool generateFaces = true)
    {
        byte[] bytes = File.ReadAllBytes (path);
        if (bytes [0] != 'V' ||
            bytes [1] != 'O' ||
            bytes [2] != 'X' ||
            bytes [3] != ' ') {
            Debug.LogError ("Invalid VOX file, magic number mismatch");
            return null;
        }

        return MVImporter.LoadVOXFromData (bytes, generateFaces);
    }

}