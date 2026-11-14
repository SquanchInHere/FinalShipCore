using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace FinalShipCore.Runtime.Modules
{
    public sealed class MainMenu 
    {
        private VisualElement menuActions;
        private readonly Dictionary<Button, MenuAction> buttonActions = new();
        private bool isEnabled;

        public event Action NewGameRequested;
        public event Action SettingsRequested;
        public event Action QuitRequested;

        public async Task BindAsync(
            VisualElement root,
            VisualTreeAsset buttonAsset,
            IReadOnlyList<MenuButtonConfig> configs,
            string buttonClassName,
            string actionClassName, 
            CancellationToken cancellationToken
        )
        {
            Disable();
            buttonActions.Clear();
            menuActions = root.Q<VisualElement>(className: actionClassName);

            if (menuActions == null) 
                throw new InvalidOperationException("MainMenu: menu-actions is not attached!");

            foreach (MenuButtonConfig config in configs)
            {
                if (config == null)
                    throw new InvalidOperationException("MainMenu: Button Config is empty!");
                
                if (!config.ShowButton) 
                    continue;

                TemplateContainer instance = buttonAsset.Instantiate();
                Button button = instance.Q<Button>(className: buttonClassName);

                if (button == null) 
                    throw new InvalidOperationException($"MainMenu: {buttonClassName} is not exists!");

                var operation = config.Label.GetLocalizedStringAsync();
                await operation.Task;

                cancellationToken.ThrowIfCancellationRequested();

                if (operation.Status != AsyncOperationStatus.Succeeded)
                    throw new InvalidOperationException($"MainMenu: operation {operation.Status}", operation.OperationException);

                button.text = operation.Result;
                button.SetBinding(nameof(Button.text), config.Label);
                button.SetEnabled(config.Enabled);

                menuActions.Add(instance);

                buttonActions.Add(button, config.Action);
            }
        }

        public void Enable()
        {
            if (isEnabled) return; 

            if (menuActions == null) 
                throw new InvalidOperationException("MainMenu: menu-actions is not bound.");

            menuActions.RegisterCallback<ClickEvent>(OnClick);
            isEnabled = true;
        }

        public void Disable()
        {
            if (!isEnabled) return;

            menuActions?.UnregisterCallback<ClickEvent>(OnClick);

            isEnabled = false;
        }

        private void OnClick(ClickEvent evt)
        {
            if (evt.target is not VisualElement target) return;

            Button button = target as Button ?? target.GetFirstAncestorOfType<Button>();

            if (button == null || !button.enabledInHierarchy) return;

            if (buttonActions.TryGetValue(button, out MenuAction actions))
            {
                EventButton(actions);
            }
        }

        private void EventButton(MenuAction action)
        {
            switch (action)
            {
                case MenuAction.NewGame:
                    NewGameRequested?.Invoke();
                    break;

                case MenuAction.Settings:
                    SettingsRequested?.Invoke();
                    break;

                case MenuAction.Quit:
                    QuitRequested?.Invoke();
                    break;

                default:
                    Debug.LogWarning($"[MainMenu] Unsupported action: {action}");
                    break;
            }
        }

        public void ApplyLayout(
            VisualElement screenContainer,
            MenuSettings settings,
            string elementName
         )
        {
            if (screenContainer == null)
                throw new  InvalidOperationException("MainMenu: Screen Container is not attached!");

            if (settings == null)
                throw new InvalidOperationException("MainMenu: Settings is not attached!");

            screenContainer.style.paddingLeft = settings.HorizontalPadding;
            screenContainer.style.paddingRight = settings.HorizontalPadding;

            switch (settings.Alignment)
            {
                case MenuAlignment.Left:
                    screenContainer.style.alignItems = Align.FlexStart;
                    break;
                case MenuAlignment.Center:
                    screenContainer.style.alignItems = Align.Center;
                    break;
                case MenuAlignment.Right:
                    screenContainer.style.alignItems = Align.FlexEnd;
                    break;
                default:
                    Debug.LogWarning($"[MainMenu] Unsupported alignment: {settings.Alignment}");
                    break;
            }

            VisualElement screen = screenContainer.Q<VisualElement>(elementName);

            if (screen == null)
                throw new InvalidOperationException("MainMenu: main-menu-screen is not attached!");

            screen.style.maxWidth = settings.MaxWidth;

        }
    }
}
