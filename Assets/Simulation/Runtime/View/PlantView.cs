using Simulation.Runtime.Entities;
using UnityEngine;
using static Simulation.Runtime.View.SpriteCatalog;

namespace Simulation.Runtime.View
{
    public class PlantView : EntityView
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private string _seeds = "Seeds",
                                        _shoots = "Shoots",
                                        _halfGrown = "WheatHalf",
                                        _fullyGrown = "Wheatfull",
                                        _faded = "WheatFaded";

        [SerializeField] private GrowStage _currentStage;
        
        private void Awake()
        {
            _spriteRenderer.sprite = GetPlantSprite(_seeds);
            _currentStage = GrowStage.Seeds;
        }

        public void SwitchStage(GrowStage stage)
        {
            if (_currentStage == stage)
                return;

            _spriteRenderer.sprite = stage switch
            {
                GrowStage.Seeds => GetPlantSprite(_seeds),
                GrowStage.Shoots => GetPlantSprite(_shoots),
                GrowStage.HalfGrown => GetPlantSprite(_halfGrown),
                GrowStage.AlmostGrown => GetPlantSprite(_halfGrown),
                GrowStage.FullyGrown => GetPlantSprite(_fullyGrown),
                GrowStage.Faded => GetPlantSprite(_faded),
                _ => GetPlantSprite(_seeds)
            };
            
            _currentStage = stage;
        }
    }
}