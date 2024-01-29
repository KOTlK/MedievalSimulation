using System;
using Simulation.Runtime.Entities;
using Simulation.Runtime.TimeManagement;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using static Simulation.Runtime.Entities.GameWorld;
using static Simulation.Runtime.View.Rendering;
using static Simulation.Runtime.Entities.Farming;
using static Simulation.Runtime.Entities.Mining;
using static Simulation.Runtime.Entities.Units;
using static Simulation.Runtime.Entities.EntityManager;

namespace Simulation.Runtime
{
    public class Main : MonoBehaviour
    {
        public int FarmersCount;
        public Vector2Int WorldSize;
        public float DayLength = 5f;
        public float TimeSpeedMultiplier = 1f;
        public TMP_Text DateText;
        public TMP_Text SeasonText;

        private float _timePassed;
        
        private void Start()
        {
            FillMap();
            SpawnFarmers(FarmersCount);
            PlayerInput.Input.EnableGameplayScheme();
            Clock.SetStartDate();
            DateText.text = Clock.CurrentDate.ToString();
            SeasonText.text = Clock.CurrentSeason.ToString();
        }

        private void Update()
        {
            TickPlants(Plants, PlantsCount);
            
            //date test
            _timePassed += Time.deltaTime * TimeSpeedMultiplier;

            if (_timePassed >= DayLength)
            {
                Clock.IncreaseDaysCount();
                DateText.text = Clock.CurrentDate.ToString();
                SeasonText.text = Clock.CurrentSeason.ToString();
                _timePassed = 0;
            }

            if (Keyboard.current.spaceKey.isPressed)
            {
                TimeSpeedMultiplier = 10f;
            }


            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            {
                TimeSpeedMultiplier = 1f;
            }
            

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                var position = Mouse.current.position.ReadValue();
                var worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(position);
                var cellPosition = new Vector3Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
                
                CreatePlant(new Plant
                {
                    ResourcesPerHarvest = 10,
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

                if (cell.ContainsContent)
                {
                    switch (cell.Content)
                    {
                        case CellContent.Plants:
                            DestroyPlant(GetPlantIdByEntity(cell.ContentEntity));
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
