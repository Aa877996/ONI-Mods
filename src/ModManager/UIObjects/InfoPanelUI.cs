﻿using PeterHan.PLib.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModManager
{
    class InfoPanelUI : UISource
    {
        public const float titleMaxTextLength = 400f;

        public string TitleText { get; set; } = " ";
        public string BodyText { get; set; } = " ";
        public List<GameObject> CustomGOs { get; set; } = new();

        protected override IUIComponent GetUIComponent()
        {
            var title = new PLabel()
            {
                FlexSize = Vector2.right,
                Text = TitleText,
                TextAlignment = TextAnchor.MiddleCenter
            }
            .AddOnRealize(ConstrainTextLength)
            .LockLayout();

            var body = new PTextArea()
            {
                FlexSize = Vector2.one,
                Text = BodyText
            };

            var extractionPanel = new PPanel()
            {
                FlexSize = Vector2.right,
                Direction = PanelDirection.Horizontal
            }
            .AddOnRealize(AddCustomGOsToPanel);

            return new PPanel("InfoPanel")
            {
                FlexSize = Vector2.one,
                Direction = PanelDirection.Vertical
            }
            .AddChild(title)
            .AddChild(extractionPanel)
            .AddChild(body)
            .AddOnRealize(AddModEntrySelectedTarget);

            void AddModEntrySelectedTarget(GameObject go)
            {
                var target = go.AddComponent<ModEntryClickListener>();
                target.Instance = this;
            }

            void AddCustomGOsToPanel(GameObject go)
            {
                foreach (var cGO in CustomGOs)
                    cGO.transform.SetParent(go.transform);  
            }
        }

        // TODO: This is largely duplicated from ModEntryUI - extract and refactor.
        private void ConstrainTextLength(GameObject go)
        {
            var locText = go.GetComponentInChildren<LocText>();
            locText.overflowMode = TextOverflowModes.Truncate;

            var le = locText.gameObject.AddOrGet<LayoutElement>();
            // Set preferred width so that the LocText knows where to truncate.
            // Set min width so that even if no strings are long, UI is sized correctly.
            le.preferredWidth = le.minWidth = titleMaxTextLength;
        }

        private class ModEntryClickListener : MonoBehaviour, ModEntryUI.IClickHandler
        {
            public InfoPanelUI Instance { get; set; }

            public void OnClick(ModUIExtract mod)
            {
                Instance.TitleText = mod.Mod.title;
                Instance.BodyText = mod.Mod.description;
                Instance.CustomGOs = mod.CustomGOs;
                Instance.RebuildGO();
            }
        }
    }
}
