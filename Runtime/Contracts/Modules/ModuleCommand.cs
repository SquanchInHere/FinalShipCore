namespace FinalShipCore.Runtime.Contracts.Modules
{
    public readonly struct ModuleCommand
    {
        public string Name { get; }

        public string Argument { get; }

        public ModuleCommand (string name, string argument)
        {
            Name = name;
            Argument = argument;
        }
    }
}
