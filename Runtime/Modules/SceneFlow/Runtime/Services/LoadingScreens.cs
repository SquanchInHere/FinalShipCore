using UnityEngine;
using System;
using UnityEngine.UIElements;

namespace FinalShip.Modules
{
    public class LoadingScreens
    {
        private VisualElement loadingScreen;
        private Label loadingStatus;
        private Label loadingText;
        private Label WaitingForContinue;
        private ProgressBar loadingProgress;
        private LoadingScreenSettings loadingScreenSettings;

        public void Bind(VisualElement root, LoadingScreenSettings settings)
        {
            WaitingForContinue = null;
            if (root == null) throw new ArgumentNullException(nameof(root));

            if (settings == null) throw new InvalidOperationException("Loading Screen: settings not was implemented!");

            loadingScreen = FindViewElement(root, "loading-screen");
            loadingText = FindLable(loadingScreen, "loading-title");
            loadingStatus = FindLable(loadingScreen, "loading-status");
            loadingProgress = FindProgressBar(loadingScreen, "loading-progress");

            loadingScreenSettings = settings;

            if (loadingScreenSettings.WaitForContinue)
            {
                if (string.IsNullOrWhiteSpace(loadingScreenSettings.ContinueHintName)) 
                    throw new InvalidOperationException("Loading Screen: WaitForContinue is enabled but ContinueHintName is empty!");

                WaitingForContinue = FindLable(loadingScreen, loadingScreenSettings.ContinueHintName);
            }

            SetAwaitingContinue(false);
        }

        private static Label FindLable(VisualElement root, string name)
        {
            Label element = root.Q<Label>(name);

            if (element == null) throw new InvalidOperationException($"Loading Screen: element Label '{name}' was not found.");

            return element;
        }

        private static VisualElement FindViewElement(VisualElement root, string name)
        {
            VisualElement element = root.Q<VisualElement>(name);

            if (element == null) throw new InvalidOperationException($"Loading Screen: element VisualElement '{name}' was not found.");

            return element;
        }

        private static ProgressBar FindProgressBar(VisualElement root, string name)
        {
            ProgressBar bar = root.Q<ProgressBar>(name);

            if (bar == null) throw new InvalidOperationException($"Loading Screen: element ProgressBar '{name}' was not found.");

            return bar;
        }

        public void SetProgress(float value)
        {
            loadingProgress.value = value;
        }

        public void Setstatus(string state)
        {
            loadingStatus.text = state;
        }

        public void SetAwaitingContinue(bool waiting)
        {
            if (WaitingForContinue == null) return;

            if (string.IsNullOrWhiteSpace(loadingScreenSettings.ContinueClassName)) 
                throw new InvalidOperationException("Loading Screen: WaitForContinue is enabled but ContinueClassName is empty!");

            WaitingForContinue.EnableInClassList(loadingScreenSettings.ContinueClassName, !waiting);
        }

        public void Show()
        {
            loadingScreen.EnableInClassList(loadingScreenSettings.ScreenClassName, false);
        }

        public void Hide()
        {
            loadingScreen.EnableInClassList(loadingScreenSettings.ScreenClassName, true);
        }
    }
}
