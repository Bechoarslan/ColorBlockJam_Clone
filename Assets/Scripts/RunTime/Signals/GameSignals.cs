using System;
using RunTime.Enums;
using RunTime.Extensions.RunTime.Utilities;

namespace RunTime.Signals
{
    public class GameSignals : MonoSingleton<GameSignals>
    {
        public Action<GameState> onChangeGameState = delegate { };
    }
}