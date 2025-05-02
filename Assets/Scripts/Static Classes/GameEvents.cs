using System;

namespace Static_Classes
{
    public static class GameEvents
    {
        public static Action ChestOpened;
        public static Action DuckDefeated;
        public static Action<bool> ActivateCursor;
    }
}