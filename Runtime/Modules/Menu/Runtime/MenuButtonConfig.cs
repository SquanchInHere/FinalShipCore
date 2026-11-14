using System;
using UnityEngine;
using UnityEngine.Localization;

namespace FinalShipCore.Runtime.Modules
{
    [Serializable]
    public class MenuButtonConfig
    {
        [SerializeField] private LocalizedString label = new LocalizedString();
        [SerializeField] private MenuAction action;
        [SerializeField] private bool enabled = true;
        [SerializeField] private bool showButton = true;

        public LocalizedString Label => label;
        public MenuAction Action => action;
        public bool Enabled => enabled;
        public bool ShowButton => showButton;
    }
}
