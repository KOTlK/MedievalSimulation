using System;
using UnityEngine;

namespace Simulation.Runtime.View
{
    public class EntityView : MonoBehaviour
    {
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetOrientation(float orientation)
        {
            var scale = transform.localScale;

            scale.x = Math.Sign(orientation);

            transform.localScale = scale;
        }
    }
}