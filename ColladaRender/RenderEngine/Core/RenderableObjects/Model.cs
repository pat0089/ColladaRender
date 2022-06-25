using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColladaRender.RenderEngine.Core.EncapsulatedTypes;
using ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.RenderableObjects
{
    /// <summary>
    /// A wrapper for loading lists of Vertex objects into a renderable Object
    /// </summary>
    public partial class Model : RenderableObject
    {
        public class Mesh
        {
            internal List<Vertex> _vertices = new();

            internal List<int> _indices = new();
            public Mesh(List<Vertex> vertices)
            {
                var processedPositions = new List<Vector3>();
                var processedNormals = new List<Vector3>();
                var processedTexCoords = new List<Vector2>();
                var processedColors = new List<Vector3>();
                var processedVertexList = new Dictionary<Vertex, int>();

                //Algorithm for removing unused vertices in preparation for rendering (O(n)):
                //Iterate on the vertices of toLoad
                var vertexSet = new HashSet<Vertex>();
                foreach (var curVertex in vertices)
                {
                    //If we can add it to the HashSet, it does not exist
                    if (vertexSet.Add(curVertex))
                    {
                        //Add the index of the new final vertex and add it to the vertex-index dictionary
                        _indices.Add(processedVertexList.Count);
                        processedVertexList.Add(curVertex, processedVertexList.Count);
                    }
                    //otherwise it exists in the HashSet
                    else
                    {
                        //Add the corresponding index from the vertex-index dictionary
                        _indices.Add(processedVertexList[curVertex]);
                    }
                }

                //Strip the values from each vertex
                foreach (var curVertex in vertexSet)
                {
                    processedPositions.Add(curVertex.Position);
                    processedNormals.Add(curVertex.Normal);
                    processedTexCoords.Add(curVertex.TexCoord);
                    processedColors.Add(Vector3.One);
                }

                _vertices = vertexSet.ToList();
                ToModelSpace(ref _vertices);
            }
        }

        Mesh _mesh = null;


        
        /// <summary>
        /// Creates a Model object based on a list of Vertex objects (which may contain duplicates) for stitching together of sides
        /// </summary>
        /// <param name="toLoad">Vertex list to load data from</param>
        public Model(List<Vertex> toLoad)
        {
            _vao = new VertexArrayObject();
            _shader = DefaultResources.ModelShader;
            _mesh = new Mesh(toLoad);

            var processedPositions = new List<Vector3>();
            var processedNormals = new List<Vector3>();
            var processedTexCoords = new List<Vector2>();
            var processedColors = new List<Vector3>();

            //Strip the values from each vertex
            foreach (var curVertex in _mesh._vertices)
            {
                processedPositions.Add(curVertex.Position);
                processedNormals.Add(curVertex.Normal);
                processedTexCoords.Add(curVertex.TexCoord);
                processedColors.Add(Vector3.One);
            }

            _vao.Bind();
            _vao.CreateFloatAttribute(processedPositions.ToArray(), _shader.GetAttributeLocation("aVertexPosition"));
            _vao.CreateFloatAttribute(processedNormals.ToArray(), _shader.GetAttributeLocation("aVertexNormal"));
            _vao.CreateFloatAttribute(processedTexCoords.ToArray(), _shader.GetAttributeLocation("aVertexTexCoord"));
            _vao.CreateFloatAttribute(processedColors.ToArray(), _shader.GetAttributeLocation("aVertexColor"));
            _vao.CreateIndexBuffer(_mesh._indices.ToArray());
            _numIndices = _mesh._indices.Count;
        }
        
        /// <summary>
        /// Transforms the input vertices into Model space
        /// </summary>
        /// <param name="vertices">vertices to confine to ((-1.0, 1.0), (-1.0, 1.0), (-1.0, 1.0))</param>
        /// <returns>vertices in Model space</returns>
        protected static void ToModelSpace(ref List<Vertex> vertices)
        {
            /*To convert any model into the space of the unit Cube ((-1.0, 1.0), (-1.0, 1.0), (-1.0, 1.0))
             * one must find the local minimums and maximums and scale on that 
             */
            
            Vector3 minPos = Vector3.PositiveInfinity;
            Vector3 maxPos = Vector3.NegativeInfinity;
            Vector3 avgPos = Vector3.Zero;
            foreach (var vertex in vertices)
            {
                maxPos.X = Math.Max(vertex.Position.X, maxPos.X);
                maxPos.Y = Math.Max(vertex.Position.Y, maxPos.Y);
                maxPos.Z = Math.Max(vertex.Position.Z, maxPos.Z);

                minPos.X = Math.Min(vertex.Position.X, minPos.X);
                minPos.Y = Math.Min(vertex.Position.Y, minPos.Y);
                minPos.Z = Math.Min(vertex.Position.Z, minPos.Z);

                avgPos += vertex.Position;

            }

            //this calculates the position of the model relative to the origin when loading in, this is ignored in the final model (for now)
            avgPos /= vertices.Count;
            
            //we use the position to translate the model in its space to its space's origin
            maxPos -= avgPos;
            minPos -= avgPos;

            //now that we're at (0, 0, 0), we can calculate the scale and scale it down by the largest dimension
            var scaleX = maxPos.X - minPos.X;
            var scaleY = maxPos.Y - minPos.Y;
            var scaleZ = maxPos.Z - minPos.Z;

            var scale = Math.Max(Math.Max(scaleX, scaleY), scaleZ);

            foreach (var vertex in vertices)
            {
                vertex.Position /= Vector3.One * scale;
                vertex.Position -= avgPos;
            }
            
        }

        /// <summary>
        /// Helper function to convert an array of doubles to an array of Vector3
        /// </summary>
        /// <param name="data">double array to be converted</param>
        /// <returns>Converted Vector3 array</returns>
        public static Vector3[] toVec3Array(double[] data)
        {
            var toReturn = new Vector3[data.Length / 3];
            for (var i = 0; i < data.Length / 3; i++)
            {
                toReturn[i] = new Vector3((float)data[i * 3],
                    (float)data[i * 3 + 1],
                    (float)data[i * 3 + 2]);
            }

            return toReturn;
        }

        /// <summary>
        /// Helper function to convert an array of doubles to an array of Vector2
        /// </summary>
        /// <param name="data">double array to be converted</param>
        /// <returns>Converted Vector2 array</returns>
        public static Vector2[] toVec2Array(double[] data)
        {
            var toReturn = new Vector2[data.Length / 2];
            for (var i = 0; i < data.Length / 2; i++)
            {
                toReturn[i] = new Vector2((float)data[i * 2],
                    (float)data[i * 2 + 1]);
            }

            return toReturn;
        }

    }
    
}