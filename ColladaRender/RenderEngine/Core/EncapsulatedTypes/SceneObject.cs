using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColladaRender.RenderEngine.Core.EncapsulatedTypes
{
    public class SceneObject
    {
        internal Vector3 _position = Vector3.Zero;

        internal Quaternion _rotation = Quaternion.Identity;

        internal Vector3 _scale = Vector3.One;

        internal List<SceneObject> Children = new();

        internal SceneObject Parent;

        public void Rotate(Quaternion rotation)
        {
            this._rotation = this._rotation * rotation;
        }

        public void Translate(Vector3 translation)
        {
            this._position += translation;
        }

        public void SetPosition(Vector3 position)
        {
            this._position = position;
        }

        public void SetScale(Vector3 scale)
        {
            this._scale = scale;
        }
    }
}
