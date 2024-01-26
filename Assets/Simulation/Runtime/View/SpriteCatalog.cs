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
        private static SpriteAtlas _plants;
        
        public static Sprite GetSprite(string spriteName)
        {
            return _map[spriteName];
        }

        public static Sprite GetPlantSprite(string name)
        {
            return _plants.GetSprite(name);
        }

        private void Awake()
        {
            Init();
            _plants = Resources.Load<SpriteAtlas>("SpriteAtlasses/Crops");
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