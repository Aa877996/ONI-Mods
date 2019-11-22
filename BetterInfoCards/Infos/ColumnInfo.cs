﻿using System.Collections.Generic;
using System.Linq;

namespace BetterInfoCards
{
    public class ColumnInfo
    {
        private const float isOverlappedThreshold = 10f;

        public float offsetX = 0f;
        public float offsetY = 0f;
        public List<InfoCard> infoCards = new List<InfoCard>();
        public float maxXInCol = 0f;
        public float YMin { get { return infoCards.Last().YMin; } }

        public void MoveAndResize(float colToRightYMin)
        {
            foreach (var card in infoCards)
            {
                card.Translate(offsetX, offsetY);

                if (colToRightYMin < card.YMax - isOverlappedThreshold)
                    card.Resize(maxXInCol);
            }
        }
    }
}