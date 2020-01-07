﻿using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterInfoCards
{
    public class DrawnWidgets : WidgetsBase
    {
        private InfoCards infoCards = new InfoCards();
        private static List<KSelectable> hoverHits;

        private float SelectPos
        {
            get
            {
                if(selectBorders.Count > 0)
                    return selectBorders[0].rect.anchoredPosition.y;
                return float.MaxValue;
            }
        }

        private float[] cachedShadowWidths = new float[0];
        private float[] cachedShadowHeights = new float[0];
        private Vector3 cachedSelectBorder = Vector3.positiveInfinity;

        private const float equalsThreshold = 0.001f;

        // This could theoretically fail if objects with the same exact shadow bar width and height were swapped in the same frame...
        private bool IsLayoutChanged()
        {
            if (shadowBars.Count != cachedShadowWidths.Length)
                return true;

            for (int i = 0; i < shadowBars.Count; i++)
            {
                Rect rect = shadowBars[i].rect.rect;

                if (!NearEquals(rect.height, cachedShadowHeights[i], equalsThreshold) || !NearEquals(rect.width, cachedShadowWidths[i], equalsThreshold))
                    return true;
            }
            return false;
        }

        // Could theoretically fail if user clicks and the layout changes in the same frame such that the new pos is identical to the old...
        private bool IsSelectedChanged()
        {
            Vector3 currentSelectBorder;
            if (selectBorders.Count > 0)
                currentSelectBorder = selectBorders[0].rect.anchoredPosition;
            else
                currentSelectBorder = Vector3.positiveInfinity;

            if (Vector3.Distance(currentSelectBorder, cachedSelectBorder) > 0.0001f)
            {
                cachedSelectBorder = currentSelectBorder;
                return true;
            }
            return false;
        }

        public void UpdateCache(List<Entry> cachedEntries, EntryType type, int drawnWidgets)
        {
            List<Entry> drawnEntries = GetEntries(type);

            if (drawnWidgets > drawnEntries.Count)
                drawnEntries.AddRange(cachedEntries.GetRange(drawnEntries.Count, drawnWidgets - drawnEntries.Count));
            if (drawnWidgets < drawnEntries.Count)
                drawnEntries.RemoveRange(drawnWidgets, drawnEntries.Count - drawnWidgets);
        }

        public void Update()
        {
            //if (IsLayoutChanged() && shadowBars.Count > 0)
            infoCards.UpdateData(ref cachedShadowWidths, ref cachedShadowHeights, shadowBars, iconWidgets, textWidgets, SelectPos, hoverHits);

            //if (IsSelectedChanged())
            infoCards.UpdateSelected(SelectPos);

            infoCards.Update(selectBorders);
        }

        private bool NearEquals(float f1, float f2, float diff)
        {
            if (Math.Abs(f1 - f2) < diff)
                return true;
            else
                return false;
        }

        [HarmonyPatch(typeof(InterfaceTool), "UpdateHoverElements")]
        private class GetHits_Patch
        {
            static void Prefix(List<KSelectable> hits)
            {
                hoverHits = hits;
            }
        }
    }
}
