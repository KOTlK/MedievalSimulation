using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Simulation.Runtime.View
{
    public class SpriteCatalog : MonoBehaviour
    {
        [SerializeField] private SpriteReference[] _sprites;

        private static readonly Dictionary<string, Sprite> _map = new();
        private static SpriteAtlas _crops;
        
        public static Sprite GetSprite(string spriteName)
        {
            return _map[spriteName];
        }

        public static Sprite GetCropSprite(string name)
        {
            return _crops.GetSprite(name);
        }

        private void Awake()
        {
            Init();
            _crops = Resources.Load<SpriteAtlas>("SpriteAtlasses/Crops");
        }

        private void Init()
        {
            foreach (var reference in _sprites)
            {
                _map.Add(reference.Name, reference.Sprite);
            }
        }
    }

    [Serializable]
    public struct SpriteReference
    {
        public string Name;
        public Sprite Sprite;
    }
}