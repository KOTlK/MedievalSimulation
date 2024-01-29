using System;
using Simulation.Runtime.Entities;
using UnityEngine;
using Object = UnityEngine.Object;
using static Simulation.Runtime.View.SpriteCatalog;
using static Simulation.Runtime.Vars;

namespace Simulation.Runtime.View
{
    public static class Rendering
    {
        private static EntityView[] _viewsDense = new EntityView[100];
        private static PlantView[] _cropsDense = new PlantView[100];

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

        public static void InstantiateFarmer(int entity, bool male, Vector3 position)
        {
            var resource = Resources.Load<EntityView>(male ? "Prefabs/Units/FarmerMale" : "Prefabs/Units/FarmerFemale");

            AddView(entity, Object.Instantiate(resource, position, Quaternion.identity));
        }

        public static void ReleaseView(int entity)
        {
            if (_viewsDense[entity])
            {
                Object.Destroy(_viewsDense[entity].gameObject);
                _viewsDense[entity] = null;
            }
        }

        public static void DrawWorldFirst()
        {
            _worldParent = new GameObject();
            for (var y = 0; y < WorldSize.y; ++y)
            {
                for (var x = 0; x < WorldSize.x; ++x)
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

        public static void DrawPlant(ref Plant plant)
        {
            var view = _cropsDense[plant.Entity];

            view.SwitchStage(plant.Stage);
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
                case CellContent.Plants:
                {
                    var resource = Resources.Load<PlantView>(path);
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

        public static void ReleasePlant(int entity)
        {
            _cropsDense[entity] = null;
            ReleaseView(entity);
        }
    }
}