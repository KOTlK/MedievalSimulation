namespace Simulation.Runtime.Entities
{
    public struct Food
    {
        public FoodType Type;
        public float Nutrition;
    }

    public enum FoodType
    {
        Meat,
        Vegetable,
        Fruit,
        Grain
    }
    
    public static class Consumable
    {
        
    }
}