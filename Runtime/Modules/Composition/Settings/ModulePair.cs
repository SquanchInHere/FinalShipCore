using FinalShipCore.Runtime.Core;
using System;

namespace FinalShipCore.Runtime.Modules
{
    [Serializable]
    public class ModulePair
    {
        public string key;
        public GameModule source;
        public GameModule target;
        public ModuleRelationType relationType;

        public string signal;
        public string command;
        public string argument;
    }
}
