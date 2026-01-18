using System;
using RunTime.Extensions.RunTime.Utilities;
using RunTime.Keys;
using UnityEngine;

namespace RunTime.Signals
{
    public class InputSignals : MonoSingleton<InputSignals>
    {
        public Action<InputParamKeys> onSendInputParams = delegate { };
        public Action<GameObject> onSendSelectedObject = delegate { };
        public Action onSelectedObjectReleased = delegate { };
    }
}