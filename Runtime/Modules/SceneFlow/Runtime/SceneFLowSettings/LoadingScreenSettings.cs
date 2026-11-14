using UnityEngine;
using UnityEngine.UIElements;

namespace FinalShip.Modules
{
    [CreateAssetMenu(fileName = "LoadingScreenSettings", menuName = "Loading/LoadingScreenSettings")]
    public class LoadingScreenSettings : ScriptableObject
    {
        [SerializeField] private VisualTreeAsset screenAsset;
        [SerializeField] private Texture2D backgroundImage;
        [SerializeField] private GameObject backgroundPrefab;

        [SerializeField] private string continueHintName = "continueHintName";
        [SerializeField] private string continueClassName = "loading-element--hidden";
        [SerializeField] private string screenClassName = "loading-screen--hidden";

        [SerializeField] private bool showTitle = true;
        [SerializeField] private bool showProgress = true;
        [SerializeField] private bool showStatus = true;
        [SerializeField] private bool waitingForContinue;

        public VisualTreeAsset ScreenAsset => screenAsset;
        public Texture2D BackgroundImage => backgroundImage;
        public GameObject BackgroundPrefab => backgroundPrefab;

        public string ContinueHintName => continueHintName;
        public string ContinueClassName => continueClassName;
        public string ScreenClassName => screenClassName;

        public bool ShowTitle => showTitle;
        public bool ShowProgress => showProgress;
        public bool ShowStatus => showStatus;
        public bool WaitForContinue => waitingForContinue;
    }
}
