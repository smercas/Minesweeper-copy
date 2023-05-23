using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    class Tile
    {
        #region non-static attributes
        public bool isCovered = true;
        public bool isFlagged = false;
        public bool hasQuestionMark = false;
        public bool isBlank = false;
        public bool isClue = false;
        public int clueNumber = 0;
        public bool isMine = false;
        public bool isBlownUp = false;
        public bool isFlaggedWrong = false;
        public bool isFlaggedRight = false;
        public bool isHeldDown = false;
        public int neighboringFlags = 0;
        #endregion

        #region constructors and overrides
        public Tile(int number)
        {
            if (number == -1)
            {
                isMine = true;
            }
            else if (number == 0)
            {
                isBlank = true;
            }
            else if (number < 9)
            {
                isClue = true;
            }
            clueNumber = number;
        }
        #endregion
    }
}
