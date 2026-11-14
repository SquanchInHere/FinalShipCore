
namespace FinalShipCore.Runtime.Contracts.Modules
{
    public interface IGameModule
    {
        /**
         * Initialize module and check all relation
         */
        void Initialize();

        /**
         * Start after success initialize module
         **/
        void Begin();


        /**
         * Stops operation and releases the module's resources.
         * Must take into account incomplete preparation and re-call.
         * */
        void Shutdown();
    }
}