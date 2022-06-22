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
        internal Vector3 position = Vector3.Zero;

        internal Quaternion rotation = Quaternion.Identity;

        public void Rotate(Quaternion rotation)
        {
            this.rotation = this.rotation * rotation;
        }

        public void Translate(Vector3 translation)
        {
            this.position += translation;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }
    }
}
