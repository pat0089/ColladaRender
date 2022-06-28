//ColladaLoader: defines functions for loading a COLLADA object from a file and for passing its vertex data to a Model object constructor

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ColladaRender.RenderEngine.Core.EncapsulatedTypes;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes
{
    public partial class COLLADA
    {
        /// <summary>
        /// Loads a COLLADA file from its filename
        /// </summary>
        /// <param name="filename">Path to the COLLADA file</param>
        /// <returns>The COLLADA file information object</returns>
        public static COLLADA Load(string filename)
        {
            COLLADA value;
            FileStream fs = new FileStream(filename, FileMode.Open);
            value = Deserialize(fs);
            fs.Close();
            return value;
        }

        /// <summary>
        /// Deserializes a COLLADA object from a Stream
        /// </summary>
        /// <param name="str">Stream object to load from</param>
        /// <returns>The deserialized COLLADA model information object</returns>
        private static COLLADA Deserialize(Stream str)
        {
            var reader = new StreamReader(str);
            var serializer = new XmlSerializer(typeof(COLLADA));
            return (COLLADA)serializer.Deserialize(reader);
        }
    }
}

namespace ColladaRender.RenderEngine.Core.RenderableObjects
{

    public partial class Model : RenderableObject
    {
        /// <summary>
        /// Loads a Model object from a COLLADA model information object
        /// </summary>
        /// <param name="model">COLLADA model information loaded from its file</param>
        /// <returns>Model from the information in the COLLADA object</returns>
        public static Model Load(COLLADA model)
        {
            #region Mesh

            var unprocessedPositions = new Vector3[] { };
            var unprocessedNormals = new Vector3[] { };
            var unprocessedTexCoords = new Vector2[] { };
            var unprocessedColors = new Vector3[] { };
            var unprocessedIndices = new int[] { };

            string positionSourceID = null;
            string normalSourceID = null;
            string texCoordSourceID = null;
            string colorSourceID = null;

            int positionOffset = -1;
            int normalOffset = -1;
            int texCoordOffset = -1;
            int colorOffset = -1;

            int numAttributes = 1;

            var vertexList = new List<Vertex>();

            #endregion

            #region Materials

            var material_ids = new List<int>();
            var material_names = new Dictionary<int, string>();
            var material_effects = new Dictionary<int, string>();
            var material_textures = new Dictionary<int, string>();

            #endregion

            //Iterate on the object array in Items[] for library_geometries
            foreach (var item in model.Items)
            {
                switch (item)
                {
                    case library_geometries lib:
                        //Iterate in the library_geometries for mesh data
                        foreach (var geom in lib.geometry)
                        {
                            //Skip if is not mesh data
                            if (geom.Item is not mesh mesh) continue;

                            //Search Items[] for geom indices
                            foreach (var meshItem in mesh.Items)
                            {
                                positionSourceID = mesh.vertices.input[0].source;

                                if (meshItem is triangles)
                                {
                                    var triangles = meshItem as triangles;
                                    foreach (var inputItem in triangles.input)
                                    {
                                        if (inputItem.semantic == "VERTEX")
                                        {
                                            positionOffset = (int)inputItem.offset;
                                        }

                                        if (inputItem.semantic == "NORMAL")
                                        {
                                            normalOffset = (int)inputItem.offset;
                                            normalSourceID = inputItem.source;
                                        }

                                        if (inputItem.semantic == "TEXCOORD")
                                        {
                                            texCoordOffset = (int)inputItem.offset;
                                            texCoordSourceID = inputItem.source;
                                        }

                                        if (inputItem.semantic == "COLOR")
                                        {
                                            colorOffset = (int)inputItem.offset;
                                            colorSourceID = inputItem.source;
                                        }
                                    }
                                    numAttributes = triangles.input.Length;
                                    unprocessedIndices = triangles.p.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();

                                }
                                else if (meshItem is polylist)
                                {
                                    var polylist = meshItem as polylist;
                                    foreach (var inputItem in polylist.input)
                                    {
                                        if (inputItem.semantic == "VERTEX")
                                        {
                                            positionOffset = (int)inputItem.offset;
                                        }

                                        if (inputItem.semantic == "NORMAL")
                                        {
                                            normalOffset = (int)inputItem.offset;
                                            normalSourceID = inputItem.source;
                                        }

                                        if (inputItem.semantic == "TEXCOORD")
                                        {
                                            texCoordOffset = (int)inputItem.offset;
                                            texCoordSourceID = inputItem.source;
                                        }

                                        if (inputItem.semantic == "COLOR")
                                        {
                                            colorOffset = (int)inputItem.offset;
                                            colorSourceID = inputItem.source;
                                        }
                                    }

                                    numAttributes = polylist.input.Length;
                                    unprocessedIndices = polylist.p.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();
                                }
                            }

                            //Iterate on the source[] for the float array information for each attribute
                            foreach (var source in mesh.source)
                            {
                                if (positionSourceID.EndsWith(source.id))
                                {
                                    var positionArray = source.Item as float_array;
                                    unprocessedPositions = toVec3Array(positionArray.Values);
                                }

                                if (normalSourceID.EndsWith(source.id))
                                {
                                    var normalArray = source.Item as float_array;
                                    unprocessedNormals = toVec3Array(normalArray.Values);
                                }

                                if (texCoordSourceID.EndsWith(source.id))
                                {
                                    var texCoordArray = source.Item as float_array;
                                    unprocessedTexCoords = toVec2Array(texCoordArray.Values);
                                }

                                if (colorSourceID.EndsWith(source.id))
                                {
                                    var colorArray = source.Item as float_array;
                                    unprocessedColors = toVec3Array(colorArray.Values);
                                }
                            }

                            //Zip all of the positions, normals, texture coordinates, & colors into a Vertex
                            //Add it to the list of vertices to be sent to the Model constructor
                            for (var i = 0; i < unprocessedIndices.Length; i += numAttributes)
                            {
                                var tempVertex = new Vertex();
                                if (positionOffset != -1)
                                {
                                    tempVertex.Position = unprocessedPositions[unprocessedIndices[i + positionOffset] % unprocessedPositions.Length];
                                }

                                if (normalOffset != -1)
                                {
                                    tempVertex.Normal = unprocessedNormals[unprocessedIndices[i + normalOffset] % unprocessedNormals.Length];
                                }

                                if (texCoordOffset != -1)
                                {
                                    tempVertex.TexCoord = unprocessedTexCoords[unprocessedIndices[i + texCoordOffset] % unprocessedTexCoords.Length];
                                }

                                if (colorOffset != -1)
                                {
                                    tempVertex.Color = unprocessedColors[unprocessedIndices[i + colorOffset] % unprocessedColors.Length];
                                }
                                vertexList.Add(tempVertex);
                            }

                        }
                        break;
                    case library_materials lib:
                        break;
                    case library_images lib:
                        break;
                    case library_effects lib:
                        break;
                    case library_animations lib:
                        break;
                    case library_animation_clips lib:
                        break;
                    default:
                        continue;
                }
            }

            return new Model(new Mesh(vertexList));
        }
        
    }
}