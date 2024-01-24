using Simulation.Runtime.Entities;
using UnityEngine;
using static Simulation.Runtime.View.SpriteCatalog;

namespace Simulation.Runtime.View
{
    public class CropView : EntityView
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
            _spriteRenderer.sprite = GetCropSprite(_seeds);
            _currentStage = GrowStage.Seeds;
        }

        public void SwitchStage(GrowStage stage)
        {
            if (_currentStage == stage)
                return;

            _spriteRenderer.sprite = stage switch
            {
                GrowStage.Seeds => GetCropSprite(_seeds),
                GrowStage.Shoots => GetCropSprite(_shoots),
                GrowStage.HalfGrown => GetCropSprite(_halfGrown),
                GrowStage.AlmostGrown => GetCropSprite(_halfGrown),
                GrowStage.FullyGrown => GetCropSprite(_fullyGrown),
                GrowStage.Faded => GetCropSprite(_faded),
                _ => GetCropSprite(_seeds)
            };
            
            _currentStage = stage;
        }
    }
}