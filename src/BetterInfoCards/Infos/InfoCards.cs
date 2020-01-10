﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterInfoCards
{
    public class InfoCards
    {
        private List<InfoCard> infoCards;
        private Vector3 cachedMousePos = Vector3.positiveInfinity;
        private InfoCard cachedClosestInfoCard = null;

        private DisplayCards displayCards = new DisplayCards();

        public void UpdateData(ref float[] cachedShadowWidths, ref float[] cachedShadowHeights, List<Entry> shadowBars, List<Entry> iconWidgets, List<Entry> textWidgets, float selectPos, List<KSelectable> hoverHits)
        {
            infoCards = new List<InfoCard>();

            int iconIndex = 0;
            int textIndex = 0;

            cachedShadowWidths = new float[shadowBars.Count];
            cachedShadowHeights = new float[shadowBars.Count];

            // For each shadow bar, create an info card and add the relevant icons and texts.
            for (int i = 0; i < shadowBars.Count; i++)
            {
                List<TextInfo> entries = CollectHoverInfo.Instance.activeStatuses[i];
                KSelectable selectable = CollectHoverInfo.Instance.selectables[i];
                Entry shadowBar = shadowBars[i];

                infoCards.Add(new InfoCard(shadowBar, iconWidgets, textWidgets, entries, ref iconIndex, ref textIndex, selectable));

                cachedShadowWidths[i] = shadowBar.rect.rect.width;
                cachedShadowHeights[i] = shadowBar.rect.rect.height;
            }

            UpdateSelected(selectPos);
        }

        public void Update(List<Entry> selectBorders)
        {
            //if (HasMouseMovedEnough())
            displayCards.UpdateData(infoCards);

            if (selectBorders.Count > 0)
                cachedClosestInfoCard.selectBorder = selectBorders[0];

            displayCards.Update();
        }

        public void UpdateSelected(float borderY)
        {
            if (borderY != float.MaxValue)
            {
                InfoCard closestInfoCard = infoCards.Aggregate((x, y) => Math.Abs(x.YMax - borderY) < Math.Abs(y.YMax - borderY) ? x : y);

                // Clear the old select border
                if (cachedClosestInfoCard != null)
                    cachedClosestInfoCard.selectBorder = new Entry();

                cachedClosestInfoCard = closestInfoCard;
            }
        }

        private bool HasMouseMovedEnough()
        {
            Vector3 cursorPos = Input.mousePosition;

            if (cursorPos != cachedMousePos)
            {
                cachedMousePos = cursorPos;
                return true;
            }
            return false;
        }
    }
}