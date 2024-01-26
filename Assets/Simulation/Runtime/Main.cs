using System;
using Simulation.Runtime.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using static Simulation.Runtime.Entities.GameWorld;
using static Simulation.Runtime.View.Rendering;
using static Simulation.Runtime.Entities.Farming;
using static Simulation.Runtime.Entities.Mining;
using static Simulation.Runtime.Entities.Units;

namespace Simulation.Runtime
{
    public class Main : MonoBehaviour
    {
        public int FarmersCount;
        public Vector2Int WorldSize;
        
        private void Start()
        {
            FillMap();
            SpawnFarmers(FarmersCount);
            PlayerInput.Input.EnableGameplayScheme();
        }

        private void Update()
        {
            TickCrops(Crops, CropsCount);
            

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                var position = Mouse.current.position.ReadValue();
                var worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(position);
                var cellPosition = new Vector3Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
                
                CreateCrop(new Crop
                {
                    CropsPerHarvest = 10,
                    TimeToGrow = 15f,
                    TimeToDry = 20,
                    Type = CropsType.Wheat
                },
                    cellPosition);
            }

            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                var position = Mouse.current.position.ReadValue();
                var worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(position);
                var cellPosition = new Vector3Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));

                ref var cell = ref GetCellReference(cellPosition.x, cellPosition.y);

                Debug.Log($"({cellPosition.x.ToString()}, {cellPosition.y.ToString()}), {cell.ContainsContent.ToString()}, {cell.Content.ToString()}, {cell.ContentEntity.ToString()}");
                if (cell.ContainsContent)
                {
                    switch (cell.Content)
                    {
                        case CellContent.Crops:
                            DestroyCrop(GetCropIdByEntity(cell.ContentEntity));
                            break;
                        case CellContent.ResourceDeposit:
                            DestroyResourceDeposit(GetResourceIdByEntity(cell.ContentEntity));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            
            DrawFarmers(Farmers, FarmersCount);
        }

        private void SpawnFarmers(int count)
        {
            for (var i = 0; i < count; ++i)
            {
                CreateFarmer(
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
