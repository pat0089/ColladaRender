using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper
{
    /// <summary>
    /// Object that manages the data about a 3-D object
    /// </summary>
    /// <remarks>Extends OpenGL wrapper functionality</remarks>
    public class VertexArrayObject
    {
        private int _id;

        private List<VertexBufferObject> _vbos = new();
        private List<int> _attribs = new();

        private ElementArrayBuffer _ebo;
        
        /// <summary>
        /// Generates a VertexArrayObject ID
        /// </summary>
        public VertexArrayObject()
        {
            _id = GL.GenVertexArray();
        }

        /// <summary>
        /// Binds all of the attributes of this VAO to the current context
        /// </summary>
        public void BindAllAttribs()
        {
            Bind(_attribs.ToArray());
        }

        /// <summary>
        /// Unbinds all of the attributes of this VAO
        /// </summary>
        public void UnbindAllAttribs()
        {
            Unbind(_attribs.ToArray());
        }

        /// <summary>
        /// Bind attributes by ID
        /// </summary>
        /// <param name="attributes">List of attributes to bind to the current context</param>
        public void Bind(params int[] attributes)
        {
            Bind();
            for (int i = 0; i < attributes.Length; i++)
            {
                GL.EnableVertexAttribArray(attributes[i]);
            }
        }

        /// <summary>
        /// Unbind attributes by ID
        /// </summary>
        /// <param name="attributes">List of attributes to unbind</param>
        public void Unbind(params int[] attributes)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                GL.DisableVertexAttribArray(attributes[i]);
            }
            Unbind();
        }

        /// <summary>
        /// Deletes the VAO safely
        /// </summary>
        public void Delete()
        {
            GL.DeleteVertexArray(_id);
            foreach (var vbo in _vbos)
            {
                vbo.Delete();
            }
            _ebo.Delete();
        }

        /// <summary>
        /// Binds a new float attribute
        /// </summary>
        /// <param name="data">Attribute data in order</param>
        /// <param name="attributeID">Attribute ID to bind the data to</param>
        public void CreateFloatAttribute(Vector2[] data, int attributeID)
        {
            VertexBufferObject dataVBO = new VertexBufferObject();
            dataVBO.Bind();
            dataVBO.Fill(data);
            GL.VertexAttribPointer(attributeID, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            dataVBO.Unbind();
            _vbos.Add(dataVBO);
            _attribs.Add(attributeID);
        }

        /// <summary>
        /// Binds a new float attribute
        /// </summary>
        /// <param name="data">Attribute data in order</param>
        /// <param name="attributeID">Attribute ID to bind the data to</param>
        public void CreateFloatAttribute(Vector3[] data, int attributeID)
        {
            VertexBufferObject dataVBO = new VertexBufferObject();
            dataVBO.Bind();
            dataVBO.Fill(data);
            GL.VertexAttribPointer(attributeID, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            dataVBO.Unbind();
            _vbos.Add(dataVBO);
            _attribs.Add(attributeID);
        }
        
        /// <summary>
        /// Binds a new float attribute
        /// </summary>
        /// <param name="data">Attribute data in order</param>
        /// <param name="attributeID">Attribute ID to bind the data to</param>
        /// <param name="attributeSize">Attribute size in number of floats</param>
        public void CreateFloatAttribute(float[] data, int attributeID, int attributeSize)
        {
            VertexBufferObject dataVBO = new VertexBufferObject();
            dataVBO.Bind();
            dataVBO.Fill(data);
            GL.VertexAttribPointer(attributeID, attributeSize, VertexAttribPointerType.Float, false, attributeSize * sizeof(float), 0);
            dataVBO.Unbind();
            _vbos.Add(dataVBO);
            _attribs.Add(attributeID);
        }

        /// <summary>
        /// Binds the index buffer
        /// </summary>
        /// <param name="data">List of indices to bind</param>
        public void CreateIndexBuffer(int[] data)
        {
            _ebo = new ElementArrayBuffer();
            _ebo.Bind();
            _ebo.Fill(data);
        }

        /// <summary>
        /// Binds the VAO to the current context
        /// </summary>
        private void Bind()
        {
            GL.BindVertexArray(_id);
        }
        
        /// <summary>
        /// Unbinds the VAO from the current context
        /// </summary>
        private void Unbind()
        {
            GL.BindVertexArray(0);
        }
    }
}