Voxel VFX
=======================
 
Render MagicaVoxel .vox files with the Unity Visual Effect Graph

Requirements
------------

Unity 2019.1 or later.

This project uses the HDRP as that's the only way to get lit meshes in the VFX graph at present.

Instructions
------------

There are two VFX graphs and two example projects.

__Volume__: The simpler example. Converts each voxel into a pixel in a texture3D. This means even empty voxels are rendered (as transparent meshes)

__Sprite__: More efficient. The voxels are stored in a texture with rgb=xyx for position and the alpha channel storing an index into a separate pallette texture that provides the color information.

The latter scene file currently scans for 


Credits
-------

* MagicaVoxelUnity: https://github.com/darkfall/MagicaVoxelUnity
* Keijiro's Low Poly Shapes: https://github.com/keijiro/LowPolyShapes (and for huge amounts of inspiration)
* Sample vox files from: https://github.com/mikelovesrobots/mmmm