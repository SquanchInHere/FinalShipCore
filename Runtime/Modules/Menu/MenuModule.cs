using FinalShipCore.Runtime.Contracts.Modules;
using FinalShipCore.Runtime.Contracts.Loading;
using FinalShipCore.Runtime.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace FinalShipCore.Runtime.Modules
{
    public sealed class MenuModule : GameModule, IRequireModuleRelations, IModuleSignalSource, ILoadingTask
    {
        [SerializeField] private VisualTreeAsset mainMenuAsset;
        [SerializeField] private VisualTreeAsset menuButtonAsset;
        [SerializeField] private MenuSettings menuSettings;

        [Header("Frame class name")]
        [SerializeField] private string containerClassName = "screen-container";
        [SerializeField] private string actionsClassName = "menu-actions";
        [SerializeField] private string tagClassName = "menu-button";
        [SerializeField] private string menuScreenName = "main-menu-screen";


        private readonly MainMenu mainMenu = new MainMenu();

        private PanelRenderer document;
        private bool isRunning;
        private bool isUIReady;
        private IModuleRelations relations;

        private VisualElement menuContainer;
        private TemplateContainer menuTree;

        public float? Progress => null;

        public event Action<string> SignalRaised;

        private void OnUiReload(PanelRenderer renderer, VisualElement root, int version)
        {
            menuContainer = root.Q<VisualElement>(containerClassName);

            if (menuContainer == null) 
                throw new InvalidOperationException("MenuModule: VisualElement{screen-container} is not attached.");
            
            if (menuButtonAsset == null)
                throw new InvalidOperationException("MenuModule: [menuButtonAsset]VisualTreeAsset is not attached.");

            mainMenu.Disable();
            menuContainer.Clear();

            if (isRunning) TryShowMenu();
        }

        public override void Initialize()
        {
            isRunning = false;
            isUIReady = false;
            document = GetComponentInChildren<PanelRenderer>();

            ValidateDependencies();

            document.RegisterUIReloadCallback(OnUiReload);
        }

        public override void Begin()
        {
            Debug.Log("Menu: Begin");
            isRunning = true;
            mainMenu.NewGameRequested += OnNewGameRequested;
            mainMenu.SettingsRequested += OnSettingsRequested;
            mainMenu.QuitRequested += OnQuitRequested;

            if (isUIReady) TryShowMenu();
        }

        public override void Shutdown()
        {
            mainMenu.NewGameRequested -= OnNewGameRequested;
            mainMenu.SettingsRequested -= OnSettingsRequested;
            mainMenu.QuitRequested -= OnQuitRequested;
            mainMenu.Disable();
            menuTree.RemoveFromHierarchy();

            isRunning = false;
            if (document != null) document.UnregisterUIReloadCallback(OnUiReload);
            isUIReady = false;
        }

        private void OnNewGameRequested()
        {
            SignalRaised?.Invoke("NewGame");
        }

        private void OnSettingsRequested()
        {
            Debug.Log("[MenuModule] Settings requested.");
        }

        private void OnQuitRequested()
        {
            Debug.Log("[MenuModule] Quit requested.");
        }

        private void ValidateDependencies()
        {
            if (document == null)
                throw new InvalidOperationException("MenuModule: PanelRenderer is not attached.");
            if (mainMenuAsset == null)
                throw new InvalidOperationException("MenuModule: [mainMenuAsset]VisualTreeAsset is not attached.");
            if (menuButtonAsset == null)
                throw new InvalidOperationException("MenuModule: [menuButtonAsset]VisualTreeAsset is not attached.");
            if (menuSettings == null)
                throw new InvalidOperationException("MenuModule: MenuSettings is not attached.");
        }

        public void ConfigureRelations(IModuleRelations relations)
        {
            this.relations = relations;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Debug.Log("Menu: preparation started");
            bool HasSettings = LocalizationSettings.HasSettings;
            
            if (!HasSettings)
            {
                throw new InvalidOperationException("MenuModule: Localization Settings is not exists!");
            }

            var operation = LocalizationSettings.InitializationOperation;
            await operation.Task;

            cancellationToken.ThrowIfCancellationRequested();

            if (operation.Status != AsyncOperationStatus.Succeeded)
                throw new InvalidOperationException("MenuModule: Cannot load loaclization settings!", operation.OperationException);

            menuTree = mainMenuAsset.Instantiate();
            await mainMenu.BindAsync(
                menuTree,
                menuButtonAsset,
                menuSettings.Buttons,
                tagClassName,
                actionsClassName,
                cancellationToken
            );

            cancellationToken.ThrowIfCancellationRequested();

            isUIReady = true;
            Debug.Log("Menu: preparation completed");

        }

        private void TryShowMenu()
        {
            if(isRunning && isUIReady && menuContainer != null)
            {
                if (menuTree != menuContainer)
                {
                    menuContainer.Add(menuTree);
                }

                mainMenu.ApplyLayout(
                        menuContainer,
                        menuSettings,
                        menuScreenName
                    );
                mainMenu.Enable();
            }
        }
    }
}
