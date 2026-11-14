using System.Collections.Generic;
using UnityEngine;

namespace FinalShipCore.Runtime.Modules
{
    [CreateAssetMenu(fileName = "MenuSettings", menuName = "Menu/Settings")]
    public sealed class MenuSettings : ScriptableObject
    {
        [SerializeField] private List<MenuButtonConfig> buttons = new List<MenuButtonConfig>();
        [SerializeField] private MenuAlignment alignment = MenuAlignment.Left;

        [Min(0)]
        [SerializeField] private float horizontalPadding = 80f;

        [Min(0)]
        [SerializeField] private float maxWidth = 400f;

        public IReadOnlyList<MenuButtonConfig> Buttons => buttons;
        public MenuAlignment Alignment => alignment;
        public float HorizontalPadding => horizontalPadding;
        public float MaxWidth => maxWidth;

    }
}
