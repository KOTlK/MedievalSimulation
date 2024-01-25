using Simulation.Runtime.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using static Simulation.Runtime.Entities.GameWorld;
using static Simulation.Runtime.View.Rendering;
using static Simulation.Runtime.Entities.Farming;
using static Simulation.Runtime.Entities.Mining;

namespace Simulation.Runtime
{
    public class Main : MonoBehaviour
    {
        public int FarmersCount;
        public Vector2Int WorldSize;
        
        private void Start()
        {
            InitializeFarming();
            InitializeMining();
            FillMap();
            SpawnFarmers(FarmersCount);
            PlayerInput.Input.EnableGameplayScheme();
        }

        private void Update()
        {
            TickCrops(Crops, CropsCount);
            DrawFarmers(EntityManager.Farmers, EntityManager.FarmersCount);

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                var position = Mouse.current.position.ReadValue();
                var worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(position);
                var cellPosition = new Vector3(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
                
                CreateCrop(new Crop
                {
                    CropsPerHarvest = 10,
                    TimeToGrow = 15f,
                    TimeToDry = 20,
                    Type = CropsType.Wheat
                },
                    cellPosition);
            }
        }

        private void SpawnFarmers(int count)
        {
            for (var i = 0; i < count; ++i)
            {
                EntityManager.CreateFarmer(
                    new Farmer() 
                    { 
                        Sex = Random.Range(0, 2) == 0
                    }, 
                    
                    Random.insideUnitCircle * 2f);
            }
        }

        private void FillMap()
        {
            CreateWorld(WorldSize, PerlinNoise);
            DrawWorldFirst();
        }
    }
}
