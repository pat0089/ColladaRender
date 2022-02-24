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
            //Console.Out.WriteLine(JsonConvert.SerializeObject(model, Formatting.Indented, new JsonConverter[] {new StringEnumConverter()}));
            
            var unprocessedPositions = new Vector3[] { };
            var unprocessedNormals = new Vector3[] { };
            var unprocessedTexCoords = new Vector2[] { };
            var unprocessedColors = new Vector3[] { };
            var unprocessedIndices = new int[] { };
            
            int positionOffset = -1;
            int normalOffset = -1;
            int texCoordOffset = -1;
            int colorOffset = -1;

            int numAttributes = 1;
            
            var vertexList = new List<Vertex>();
            int stride = 0;

            //Iterate on the object array in Items[] for library_geometries
            foreach (var item in model.Items)
            {
                //Skip if is not library_geometries 
                if (item is not library_geometries geometries) continue;
                
                //Iterate in the library_geometries for mesh data
                foreach (var geom in geometries.geometry)
                {
                    //Skip if is not mesh data
                    if (geom.Item is not mesh mesh) continue;

                    //Search Items[] for geom indices
                    foreach (var meshItem in mesh.Items)
                    {
                        if (meshItem is triangles)
                        {
                            var triangles = meshItem as triangles;
                            foreach (var inputItem in triangles.input)
                            {
                                if (inputItem.semantic == "VERTEX")
                                {
                                    positionOffset = (int) inputItem.offset;
                                }

                                if (inputItem.semantic == "NORMAL")
                                {
                                    normalOffset = (int) inputItem.offset;
                                }

                                if (inputItem.semantic == "TEXCOORD")
                                {
                                    texCoordOffset = (int) inputItem.offset;
                                }

                                if (inputItem.semantic == "COLOR")
                                {
                                    colorOffset = (int) inputItem.offset;
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
                                    positionOffset = (int) inputItem.offset;
                                }

                                if (inputItem.semantic == "NORMAL")
                                {
                                    normalOffset = (int) inputItem.offset;
                                }

                                if (inputItem.semantic == "TEXCOORD")
                                {
                                    texCoordOffset = (int) inputItem.offset;
                                }

                                if (inputItem.semantic == "COLOR")
                                {
                                    colorOffset = (int) inputItem.offset;
                                }
                            }
                            
                            numAttributes = polylist.input.Length;
                            unprocessedIndices = polylist.p.Split(' ').Select(n => Convert.ToInt32(n)).ToArray();
                        }
                    }

                    //Iterate on the source[] for the float array information for each attribute
                    foreach (var source in mesh.source)
                    {
                        if (source.Item is float_array positionArray && positionArray.id.EndsWith("positions") | positionArray.id.EndsWith("positions-array"))
                        {
                            unprocessedPositions = toVec3Array(positionArray.Values);
                            if (stride >= 0)
                                stride++;
                        }

                        if (source.Item is float_array normalArray && normalArray.id.EndsWith("normals") | normalArray.id.EndsWith("normals-array"))
                        {
                            unprocessedNormals = toVec3Array(normalArray.Values);
                            if (stride >= 0)
                                stride++;
                        }

                        if (source.Item is float_array texCoordArray && texCoordArray.id.EndsWith("map-0") | texCoordArray.id.EndsWith("map-0-array"))
                        { 
                            unprocessedTexCoords = toVec2Array(texCoordArray.Values);
                            if (stride >= 0)
                                stride++;
                        }
                        
                        if (source.Item is float_array colorArray)
                        {
                            if (source.name is not null)
                            {
                                if (!colorArray.id.EndsWith(source.name) && !colorArray.id.EndsWith(source.name + "-array")) continue;
                            }
                            else if (!colorArray.id.EndsWith("color") && !colorArray.id.EndsWith("color-array"))
                                continue;
                            unprocessedColors = toVec3Array(colorArray.Values);
                            if (stride >= 0)
                                stride++;
                        }
                    }

                    //Zip all of the positions, normals, texture coordinates, & colors into a Vertex
                    //Add it to the list of vertices to be sent to the Model constructor
                    for (var i = 0; i < unprocessedIndices.Length; i+=stride)
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
            }
            return new Model(vertexList);
        }
    }
}