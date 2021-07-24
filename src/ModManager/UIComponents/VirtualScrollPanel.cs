﻿using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModManager
{
    // TODO: This assumes that children won't flex vertically.
    // There should probably be some way of enforcing this, or xml documentation at minimum.
    public class VirtualScrollPanel : PContainer, IDynamicSizable
    {
        public TextAnchor Alignment { get; set; }
        public PanelDirection Direction { get; set; }
        public bool DynamicSize { get; set; }
        public int Spacing { get; set; }
        public IEnumerable<IUISource> Children { get; set; }

        private VirtualPanelChildManager manager;

        public VirtualScrollPanel(string name = null) : base(name ?? nameof(VirtualScrollPanel))
        {
            Alignment = TextAnchor.MiddleCenter;
            Direction = PanelDirection.Vertical;
            DynamicSize = false;
            Spacing = 0;
        }

        public override GameObject Build()
        {
            if (Children == null)
                throw new InvalidOperationException("No Children set.");

            var panel = PUIElements.CreateUI(null, Name);
            SetImage(panel);

            var lg = panel.AddComponent<BoxLayoutGroup>();
            lg.Params = new BoxLayoutParams()
            {
                Direction = Direction,
                Alignment = Alignment,
                Spacing = Spacing,
                Margin = Margin
            };

            lg.flexibleWidth = FlexSize.x;
            lg.flexibleHeight = FlexSize.y;

            manager = panel.AddComponent<VirtualPanelChildManager>();
            UpdateChildren(Children);

            InvokeRealize(panel);
            return panel;
        }

        public void UpdateChildren(IEnumerable<IUISource> children) => manager.UpdateChildren(Children = children);

        public List<GameObject> GetBuiltChildren() => manager.GetBuiltChildren();
        public IUISource GetUISourceForGO(GameObject go) => manager.GetUISourceForGO(go);
    }
}