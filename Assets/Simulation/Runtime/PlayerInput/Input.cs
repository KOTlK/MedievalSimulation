namespace Simulation.Runtime.PlayerInput
{
    public static class Input
    {
        public static PlayerControls Controls = new();

        public static void EnableGameplayScheme()
        {
            Controls.GamePlay.Enable();
        }
    }
}