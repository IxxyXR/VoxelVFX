using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class VoxelSprite : MonoBehaviour
{

    public string path = "VoxFiles";
    public string filenameMask = "chr_*.vox";
    public bool animate = true;
    
    private VisualEffect _vfx;
    private int fileIndex;
    private FileInfo[] files;
    

    private const int MAX_TEXTURE_WIDTH = 4096;
    
    void Start()
    {
        _vfx = gameObject.GetComponent<VisualEffect>();
        if (animate)
        {
            DrawAllVoxels($"Assets\\{path}");
            
        }
        else
        {
            DrawVoxel($"Assets\\{path}");            
        }

    }

    public void DrawAllVoxels(string dirPath)
    {
        fileIndex = 0;
        var info = new DirectoryInfo(dirPath);
        var fileInfo = info.GetFiles(filenameMask);
        files = fileInfo.ToArray();
        InvokeRepeating(nameof(DrawNextVoxel), 0, 2);
    }

    public void DrawNextVoxel()
    {
        if (fileIndex < files.Length)
        {
            string filePath = $"{files[fileIndex]}";
            DrawVoxel(filePath);

            fileIndex++;
        }
        else
        {
            fileIndex = 0;
        }
    }

    public Vector2 unpackTexIndex(int index)
    {
        return new Vector2(index % MAX_TEXTURE_WIDTH, index / MAX_TEXTURE_WIDTH);
    }
    
    public void DrawVoxel(string filePath)
    {
        MVMainChunk chunk = LoadVOX(filePath, false);

        var paletteTexture = new Texture2D(chunk.palatte.Length, 1, TextureFormat.RGBAFloat, false);
        paletteTexture.wrapMode = TextureWrapMode.Clamp;
        paletteTexture.filterMode = FilterMode.Point;

        for (var i = 0; i < chunk.palatte.Length; i++)
        {
            var color = chunk.palatte[i];
            paletteTexture.SetPixel(i, 1, color);
        }
        paletteTexture.Apply();
        
        var colorArray = new List<Color>();
        
        for (int x=0; x<chunk.sizeX; x++) 
        {
            for (int y = 0; y < chunk.sizeY; y++)
            {
                for (int z = 0; z < chunk.sizeZ; z++)
                {
                    var voxel = chunk.voxelChunk.voxels[x, y, z];                    
                    if (voxel != 0)
                    {
                        colorArray.Add(new Color(x, y, z, voxel));
                    }
                }
            }
        }

        int voxelCount = colorArray.Count;
        Vector2 voxelTextureDimensions = unpackTexIndex(voxelCount);
        voxelTextureDimensions.x = voxelTextureDimensions.y<1?voxelTextureDimensions.x + 1:MAX_TEXTURE_WIDTH;
        voxelTextureDimensions.y = (int)voxelTextureDimensions.y + 1;
        var voxelTexture = new Texture2D((int)voxelTextureDimensions.x, (int)voxelTextureDimensions.y, TextureFormat.RGBAFloat, false);
        voxelTexture.wrapMode = TextureWrapMode.Clamp;
        voxelTexture.filterMode = FilterMode.Point;

        for (var i = 0; i < colorArray.Count; i++)
        {
            var color = colorArray[i];
            Vector2 voxelTextureCoords = unpackTexIndex(i);
            voxelTexture.SetPixel((int) voxelTextureCoords.x, (int) voxelTextureCoords.y, color);
        }

        voxelTexture.Apply();
        _vfx.SetTexture("Voxel Texture", voxelTexture);
        _vfx.SetTexture("Palette Texture", paletteTexture);
        _vfx.SetInt("Voxel Count", voxelCount);
        _vfx.SetVector3("Voxel Scene Dimensions", new Vector3(chunk.sizeX, chunk.sizeY, chunk.sizeZ));
        _vfx.SetVector2("Voxel Texture Dimensions", voxelTextureDimensions);
        _vfx.SetInt("Palette Size", paletteTexture.width);
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