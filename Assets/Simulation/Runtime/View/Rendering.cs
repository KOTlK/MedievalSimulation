using System;
using Simulation.Runtime.Entities;
using UnityEngine;
using Object = UnityEngine.Object;
using static Simulation.Runtime.View.SpriteCatalog;

namespace Simulation.Runtime.View
{
    public static class Rendering
    {
        private static EntityView[] _viewsDense = new EntityView[100];
        private static CropView[] _cropsDense = new CropView[100];

        private static GameObject _worldParent;

        public static void AddView(int entity, EntityView view)
        {
            if (entity >= _viewsDense.Length)
            {
                Array.Resize(ref _viewsDense, entity << 1);
            }

            _viewsDense[entity] = view;
        }
        
        public static void DrawFarmers(Farmer[] farmers, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                var id = farmers[i].Entity;
                _viewsDense[id].SetPosition(EntityManager.Entities[id].Position);
                _viewsDense[id].SetOrientation(EntityManager.Entities[id].Orientation);
            }
        }

        public static void CreateFarmer(int entity, bool male, Vector3 position)
        {
            var resource = Resources.Load<EntityView>(male ? "Prefabs/Units/FarmerMale" : "Prefabs/Units/FarmerFemale");

            AddView(entity, Object.Instantiate(resource, position, Quaternion.identity));
        }

        public static void DestroyView(int entity)
        {
            Object.Destroy(_viewsDense[entity].gameObject);
            _viewsDense[entity] = null;
        }

        public static void DrawWorldFirst()
        {
            _worldParent = new GameObject();
            for (var y = 0; y < GameWorld.Size.y; ++y)
            {
                for (var x = 0; x < GameWorld.Size.x; ++x)
                {
                    var go = new GameObject();
                    var view = go.AddComponent<CellView>();
                    var spriteRenderer = go.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = GetSprite(GameWorld.GetCell(x, y).Type.ToString());
                    view.transform.position = GameWorld.GetCell(x, y).Position;
                    view.SpriteRenderer = spriteRenderer;
                    go.transform.SetParent(_worldParent.transform);
                    AddView(GameWorld.GetCell(x, y).Entity, view);
                }
            }
        }

       /* public static void DrawCropFirst(ref Crop crop)
        {
            var resource = Resources.Load<CropView>($"Prefabs/Crops/{crop.Type.ToString()}");

            var view = Object.Instantiate(resource, EntityManager.Entities[crop.Entity].Position, Quaternion.identity);

            AddView(crop.Entity, view);

            if (crop.Entity >= _cropsDense.Length)
            {
                Array.Resize(ref _cropsDense, crop.Entity << 1);
            }

            _cropsDense[crop.Entity] = view;
        }*/

        public static void DrawCrop(ref Crop crop)
        {
            var view = _cropsDense[crop.Entity];

            view.SwitchStage(crop.Stage);
        }

        public static void InstantiateCellContentView(CellContent content, int entity, string name)
        {
            var path = $"Prefabs/{content.ToString()}/{name}";

            switch (content)
            {
                case CellContent.ResourceDeposit:
                {
                    var resource = Resources.Load<EntityView>(path);
                    var view = Object.Instantiate(resource, EntityManager.Entities[entity].Position,
                        Quaternion.identity);

                    if (entity >= _cropsDense.Length)
                    {
                        Array.Resize(ref _cropsDense, entity << 1);
                    }

                    AddView(entity, view);
                    break;
                }
                case CellContent.Crops:
                {
                    var resource = Resources.Load<CropView>(path);
                    var view = Object.Instantiate(resource, EntityManager.Entities[entity].Position,
                        Quaternion.identity);

                    if (entity >= _cropsDense.Length)
                    {
                        Array.Resize(ref _cropsDense, entity << 1);
                    }

                    _cropsDense[entity] = view;
                    AddView(entity, view);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(content), content, null);
            }
        }

        public static void DestroyCrop(int entity)
        {
            _cropsDense[entity] = null;
            DestroyView(entity);
        }
    }
}