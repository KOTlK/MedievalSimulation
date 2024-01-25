namespace Simulation.Runtime.Entities
{
    public struct ResourceDeposit
    {
        public DepositType Type;
        public int Entity;
        public int ResourcesLeft;
    }
    
    public enum DepositType
    {
        Iron,
        Coal,
    }
    
    public static class Mining
    {
        
    }
}