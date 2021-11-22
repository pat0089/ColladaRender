using System.Collections.Generic;
using System.Drawing;
using ColladaRender.RenderEngine.Core.EncapsulatedTypes;
using ColladaRender.RenderEngine.Core.EncapsulatedTypes.GLWrapper;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ColladaRender.RenderEngine.Core.RenderableObjects
{
    public class Cube : RenderableObject
    {
        private readonly Vector3[] _positions = new Vector3[]
        {
            //front
            -Vector3.UnitX - Vector3.UnitY + Vector3.UnitZ,
            -Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ,
            Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ,
            Vector3.UnitX - Vector3.UnitY + Vector3.UnitZ,

            //back
            Vector3.UnitX - Vector3.UnitY - Vector3.UnitZ,
            Vector3.UnitX + Vector3.UnitY - Vector3.UnitZ,
            -Vector3.UnitX + Vector3.UnitY - Vector3.UnitZ,
            -Vector3.UnitX - Vector3.UnitY - Vector3.UnitZ,

            //right
            Vector3.UnitX - Vector3.UnitY - Vector3.UnitZ,
            Vector3.UnitX + Vector3.UnitY - Vector3.UnitZ,
            Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ,
            Vector3.UnitX - Vector3.UnitY + Vector3.UnitZ,

            //left
            -Vector3.UnitX - Vector3.UnitY + Vector3.UnitZ,
            -Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ,
            -Vector3.UnitX + Vector3.UnitY - Vector3.UnitZ,
            -Vector3.UnitX - Vector3.UnitY - Vector3.UnitZ,

            //top
            -Vector3.UnitX + Vector3.UnitY - Vector3.UnitZ,
            -Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ,
            Vector3.UnitX + Vector3.UnitY + Vector3.UnitZ,
            Vector3.UnitX + Vector3.UnitY - Vector3.UnitZ,

            //bottom
            -Vector3.UnitX - Vector3.UnitY - Vector3.UnitZ,
            -Vector3.UnitX - Vector3.UnitY + Vector3.UnitZ,
            Vector3.UnitX - Vector3.UnitY + Vector3.UnitZ,
            Vector3.UnitX - Vector3.UnitY - Vector3.UnitZ,
        };

        private readonly Vector3[] _normals = new Vector3[]
        {
            Vector3.UnitZ, //front side
            Vector3.UnitZ, //front side
            Vector3.UnitZ, //front side
            Vector3.UnitZ, //front side

            -Vector3.UnitZ, //back side
            -Vector3.UnitZ, //back side
            -Vector3.UnitZ, //back side
            -Vector3.UnitZ, //back side

            Vector3.UnitX, //right side
            Vector3.UnitX, //right side
            Vector3.UnitX, //right side
            Vector3.UnitX, //right side

            -Vector3.UnitX, //left side
            -Vector3.UnitX, //left side
            -Vector3.UnitX, //left side
            -Vector3.UnitX, //left side

            Vector3.UnitY, //top
            Vector3.UnitY, //top
            Vector3.UnitY, //top
            Vector3.UnitY, //top

            -Vector3.UnitY, //bottom
            -Vector3.UnitY, //bottom
            -Vector3.UnitY, //bottom
            -Vector3.UnitY, //bottom
        };

        private readonly Vector2[] _texcoords = new Vector2[]
        {
            Vector2.Zero, Vector2.UnitY, Vector2.One, Vector2.UnitX,
            Vector2.Zero, Vector2.UnitY, Vector2.One, Vector2.UnitX,
            Vector2.Zero, Vector2.UnitY, Vector2.One, Vector2.UnitX,
            Vector2.Zero, Vector2.UnitY, Vector2.One, Vector2.UnitX,
            Vector2.Zero, Vector2.UnitY, Vector2.One, Vector2.UnitX,
            Vector2.Zero, Vector2.UnitY, Vector2.One, Vector2.UnitX,
        };

        private readonly Vector3[] _colors = new Vector3[]
        {
            new Vector3(1.0f, 0.0f, 0.0f), //red
            new Vector3(1.0f, 1.0f, 0.0f), //yellow
            new Vector3(0.0f, 1.0f, 0.0f), //green
            new Vector3(1.0f, 1.0f, 1.0f), //white

            new Vector3(0.0f, 0.0f, 1.0f), //blue
            new Vector3(0.0f, 1.0f, 1.0f), //cyan
            new Vector3(0.0f, 0.0f, 0.0f), //black
            new Vector3(1.0f, 0.0f, 1.0f), //magenta

            new Vector3(0.0f, 0.0f, 1.0f), //blue
            new Vector3(0.0f, 1.0f, 1.0f), //cyan
            new Vector3(0.0f, 1.0f, 0.0f), //green
            new Vector3(1.0f, 1.0f, 1.0f), //white

            new Vector3(1.0f, 0.0f, 0.0f), //red
            new Vector3(1.0f, 1.0f, 0.0f), //yellow
            new Vector3(0.0f, 0.0f, 0.0f), //black
            new Vector3(1.0f, 0.0f, 1.0f), //magenta

            new Vector3(0.0f, 0.0f, 0.0f), //black
            new Vector3(1.0f, 1.0f, 0.0f), //yellow
            new Vector3(0.0f, 1.0f, 0.0f), //green
            new Vector3(0.0f, 1.0f, 1.0f), //cyan

            new Vector3(1.0f, 0.0f, 1.0f), //magenta
            new Vector3(1.0f, 0.0f, 0.0f), //red
            new Vector3(1.0f, 1.0f, 1.0f), //white
            new Vector3(0.0f, 0.0f, 1.0f), //blue

        };

        private readonly int[] _indices = new int[]
        {
            0, 1, 2, 2, 3, 0, //front
            4, 5, 6, 6, 7, 4, //back
            8, 9, 10, 10, 11, 8, //right
            12, 13, 14, 14, 15, 12, //left
            16, 17, 18, 18, 19, 16, //top
            20, 21, 22, 22, 23, 20, //bottom
        };

        public Cube()
        {
            _vao = new VertexArrayObject();
            _vao.Bind();
            _vao.CreateFloatAttribute(_positions, 0);
            _vao.CreateFloatAttribute(_normals, 1);
            _vao.CreateFloatAttribute(_texcoords, 2);
            _vao.CreateFloatAttribute(_colors, 3);
            _vao.CreateIndexBuffer(_indices);
            _numIndices = _indices.Length;
        }

        private Cube(bool colors, float size, Shader shader)
        {
            _shader = shader;
            _vao = new VertexArrayObject();
            _vao.Bind();
            if (size != 1.0f)
            {
                var sizedPositions = new List<Vector3>();
                foreach (var pos in _positions)
                {
                    sizedPositions.Add(pos * size/2.0f);
                }
                _vao.CreateFloatAttribute(sizedPositions.ToArray(), 0);
            }
            else
            {
                _vao.CreateFloatAttribute(_positions, 0);
            }

            _vao.CreateFloatAttribute(_normals, 1);
            _vao.CreateFloatAttribute(_texcoords, 2);
            if (colors)
            {
                _vao.CreateFloatAttribute(_colors, 3);
            }
            else
            {
                var noColors = new Vector3[_colors.Length];
                for (var i = 0; i < _colors.Length; i++)
                {
                    noColors[i] = Vector3.One;
                }
                _vao.CreateFloatAttribute(noColors, 3);
            }

            _vao.CreateIndexBuffer(_indices);
            _numIndices = _indices.Length;
        }
        
        public static Cube NoColor()
        {
            return new Cube(false, 1.0f, DefaultResources.DefaultShader);
        }

        public static Cube Sized(float size)
        {
            return new Cube(true, size, DefaultResources.DefaultShader);
        }

        public static Cube Generate(bool hasDefaultColors, float size, Shader shader)
        {
            return new Cube(hasDefaultColors, size, shader);
        }
    }
}