using Simulation.Runtime.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using static Simulation.Runtime.Entities.WorldUtils;
using static Simulation.Runtime.View.Rendering;
using static Simulation.Runtime.Entities.Farming;

namespace Simulation.Runtime
{
    public class Main : MonoBehaviour
    {
        public int FarmsCount;
        public int FarmersCount;
        public Vector2Int WorldSize;
        
        private void Start()
        {
            FillMap(FarmsCount);
            SpawnFarmers(FarmersCount);
            PlayerInput.Input.EnableGameplayScheme();
        }

        private void Update()
        {
            TickCrops(WorldUtils.World.Crops, WorldUtils.World.CropsCount);
            DrawFarmers(EntityManager.Farmers, EntityManager.FarmersCount);

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                var position = Mouse.current.position.ReadValue();
                var worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(position);
                var cellPosition = new Vector3((int)worldPosition.x, (int)worldPosition.y);
                
                CreateCrop(ref WorldUtils.World, new Crop
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

        private void FillMap(int farmsCount)
        {
            CreateWorld(WorldSize, PerlinNoise);
            DrawWorldFirst(ref WorldUtils.World);
        }
    }
}
