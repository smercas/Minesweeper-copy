using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    class MinefieldSolver
    {
        private LinkedList<Tuple<int, int>> revealedNumbers;
        private Size fieldSize;
        private Tile[,] field;
        private Point solverOrigin;
        private Minefield minefield;
        private readonly int[] vx = { -1, -1, -1,  0,  1,  1,  1,  0 };
        private readonly int[] vy = { -1,  0,  1,  1,  1,  0, -1, -1 };
        private int[] temp;
        private bool[,] comb;
        private Tile[,] neighborhood;
        private int[,] mines;
        private int[,] discoverables;

        public bool TryToSolve(ref Tile[,] _field, Size _fieldSize, Point _solverOrigin, Minefield _minefield = null)
        {
            revealedNumbers = new LinkedList<Tuple<int, int>>();
            field = _field;
            fieldSize = _fieldSize;
            solverOrigin = _solverOrigin;
            minefield = _minefield;

            //minefield == null when not using RunVisualSolver, it discovers tiles then places them in the list

            //minefield != null when the field has already been generated, just places the clues in the list
            if (minefield == null)
            {
                RevealField(solverOrigin.X, solverOrigin.Y);
            }
            else
            {
                for (int x = 0; x < fieldSize.Width; x++)
                {
                    for (int y = 0; y < fieldSize.Height; y++)
                    {
                        if (field[x, y].isFlagged)
                        {
                            minefield.MainThreadInvoke(() =>
                            {
                                minefield.flagsPlaced--;
                                minefield.OnTileFlag(new EventArgs());
                                minefield.BlockedInteraction = false;
                                minefield.BlockedInteraction = true;
                            });
                            System.Threading.Thread.Sleep(10);
                        }
                        field[x, y].isFlagged = false;
                        field[x, y].neighboringFlags = 0;
                        if (field[x, y].clueNumber > 0 && !field[x, y].isCovered)
                        {
                            revealedNumbers.AddFirst(Tuple.Create(x, y));
                        }
                    }
                }
            }

            SolveViaRevealedNumbers();

            return CheckWin();
        }

        private void RevealField(int x, int y)
        {
            field[x, y].isCovered = false;
            if (field[x, y].isClue)
            {
                revealedNumbers.AddFirst(Tuple.Create(x, y));
            }
            if (field[x, y].isBlank)
            {
                int ax, ay;
                for (int i = 0; i < 8; i++)
                {
                    ax = x + vx[i]; ay = y + vy[i];
                    if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height &&
                        field[ax, ay].isCovered)
                        RevealField(ax, ay);
                }
            }
            
        }

        private void SolveViaRevealedNumbers()
        {
            bool change;
            do
            {
                change = false;
                int x, y, ax, ay;
                for (LinkedListNode<Tuple<int, int>> k = revealedNumbers.First; k != null; k = k.Next)
                {
                    int countUnflagged = 0;
                    int countCoveredAdjacentFields = 0;
                    x = k.Value.Item1;
                    y = k.Value.Item2;

                    for (int i = 0; i < 8; i++)
                    {
                        ax = x + vx[i]; ay = y + vy[i];
                        if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height && 
                            field[ax, ay].isCovered && !field[ax, ay].isFlagged)
                        {
                            countCoveredAdjacentFields++;
                        }
                    }

                    countUnflagged = field[x, y].clueNumber - field[x, y].neighboringFlags;

                    if (countUnflagged == countCoveredAdjacentFields && countUnflagged > 0)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            ax = x + vx[i]; ay = y + vy[i];
                            if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height &&
                                field[ax, ay].isCovered && !field[ax, ay].isFlagged)
                            {
                                Flag(ax, ay);
                            }
                        }
                        change = true;
                        revealedNumbers.Remove(k);
                    }
                    else if (field[x, y].neighboringFlags == field[x, y].clueNumber)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            ax = x + vx[i]; ay = y + vy[i];
                            if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height &&
                                field[ax, ay].isCovered && !field[ax, ay].isFlagged)
                            {
                                RevealField(ax, ay);
                                if (minefield != null)
                                {
                                    minefield.MainThreadInvoke(() =>
                                    {
                                        minefield.highlightedField = field[ax, ay];
                                        minefield.BlockedInteraction = false;
                                        minefield.BlockedInteraction = true;
                                    });
                                    System.Threading.Thread.Sleep(10);
                                }
                            }
                        }
                        change = true;
                        revealedNumbers.Remove(k);
                    }
                }
                for (LinkedListNode<Tuple<int, int>> k = revealedNumbers.First; k != null && change == false; k = k.Next)
                {
                    int countUnflagged = 0;
                    int countCoveredAdjacentFields = 0;
                    x = k.Value.Item1;
                    y = k.Value.Item2;
                    int bx, by;
                    int nr = 0;
                    comb = new bool[70, 8];
                    temp = new int[9];
                    mines = new int[fieldSize.Width, fieldSize.Height];
                    discoverables = new int[fieldSize.Width, fieldSize.Height];

                    for (int i = 0; i < 8; i++)
                    {
                        ax = x + vx[i]; ay = y + vy[i];
                        if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height &&
                            field[ax, ay].isCovered && !field[ax, ay].isFlagged)
                        {
                            countCoveredAdjacentFields++;
                        }
                    }

                    countUnflagged = field[x, y].clueNumber - field[x, y].neighboringFlags;

                    if (field[x, y].clueNumber != field[x, y].neighboringFlags && countUnflagged != countCoveredAdjacentFields)
                    {
                        Bt(countCoveredAdjacentFields, countUnflagged, 1, ref nr);

                        #region shows all possible mine combinations for a clue
                        /*
                         * Console.WriteLine("Field [" + x + ", " + y + "](" + field[x, y].clueNumber + ") has " 
                            + countCoveredAdjacentFields + " untouched fields around, " + field[x, y].neighboringFlags 
                            + " neighboring flags and " + countUnflagged + " unflagged mines left");
                        Console.WriteLine("Possible combinations for: " + x + ", " + y);
                        Console.WriteLine(nr);
                        for (int i = 0; i < nr; i++)
                        {
                            for (int j = 0; j < countCoveredAdjacentFields; j++)
                            {
                                if (comb[i, j]) Console.Write("1 ");
                                else Console.Write("0 ");
                            }
                            Console.WriteLine();
                        }
                        */
                        #endregion

                        int successfulCases = nr;

                        for (int i = 0; i < nr; i++)
                        {
                            int a = 0; 
                            bool abort = false;
                            neighborhood = new Tile[fieldSize.Width, fieldSize.Height];

                            for (int j = x - 2; j <= x + 2; j++)
                            {
                                for (int l = y - 2; l <= y + 2; l++)
                                {
                                    if (j >= 0 && j < fieldSize.Width && l >= 0 && l < fieldSize.Height)
                                    {
                                        neighborhood[j, l] = new Tile(field[j, l].clueNumber);
                                        neighborhood[j, l].isCovered = field[j, l].isCovered;
                                        neighborhood[j, l].isFlagged = field[j, l].isFlagged;
                                        neighborhood[j, l].neighboringFlags = field[j, l].neighboringFlags;
                                    }
                                }
                            }

                            for (int j = 0; j < 8 && !abort; j++)
                            {
                                ax = x + vx[j]; ay = y + vy[j];
                                if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height
                                        && neighborhood[ax, ay].isCovered && !neighborhood[ax, ay].isFlagged)
                                {
                                    if (comb[i, a])
                                    {
                                        neighborhood[ax, ay].isFlagged = true;
                                        for (int l = 0; l < 8 && !abort; l++)
                                        {
                                            bx = ax + vx[l]; by = ay + vy[l];
                                            if (bx >= 0 && bx < fieldSize.Width && by >= 0 && by < fieldSize.Height)
                                            {
                                                neighborhood[bx, by].neighboringFlags++;
                                                if (neighborhood[bx, by].neighboringFlags > neighborhood[bx, by].clueNumber 
                                                    && !neighborhood[bx, by].isMine && !neighborhood[bx, by].isCovered)
                                                {
                                                    abort = true;
                                                    successfulCases--;
                                                }
                                            }
                                        }
                                    }
                                    a++;
                                }
                            }
                            if (abort == false)
                            {
                                AddPossibility(x, y);
                            }

                            #region show neighborhood
                            /*
                            Console.WriteLine("Clues for: " + x + ", " + y + "     " + "Flags for: " + x + ", " + y + "     " + "Discovered spaces for: " + x + ", " + y);
                            for (int l = y - 2; l <= y + 2; l++)
                            {
                                //Clues
                                for (int j = x - 2; j <= x + 2; j++)
                                {
                                    if (j >= 0 && j < fieldSize.Width && l >= 0 && l < fieldSize.Height)
                                    {
                                        if (neighborhood[j, l].clueNumber != -1) Console.Write(neighborhood[j, l].clueNumber + " ");
                                        else Console.Write("X ");
                                    }
                                    else Console.Write("- ");
                                }

                                Console.Write("          ");

                                //Flags
                                for (int j = x - 2; j <= x + 2; j++)
                                {
                                    if (j >= 0 && j < fieldSize.Width && l >= 0 && l < fieldSize.Height)
                                    {
                                        if (neighborhood[j, l].isFlagged) Console.Write("1 ");
                                        else Console.Write("0 ");
                                    }
                                    else Console.Write("- ");
                                }

                                Console.Write("          ");

                                //Discovered Spaces
                                for (int j = x - 2; j <= x + 2; j++)
                                {
                                    if (j >= 0 && j < fieldSize.Width && l >= 0 && l < fieldSize.Height)
                                    {
                                        if (neighborhood[j, l].isCovered) Console.Write("X ");
                                        else Console.Write("O ");
                                    }
                                    else Console.Write("- ");
                                }


                                Console.WriteLine();
                            }
                            Console.WriteLine();
                            */
                            #endregion

                        }

                        #region successful cases and more
                        /*
                        Console.WriteLine();

                        Console.WriteLine("Number of Successful cases: " + successfulCases);

                        Console.WriteLine("Possible Mine placements for " + x + ", " + y);
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            for (int i = x - 1; i <= x + 1; i++)
                            {
                                if (i >= 0 && i < fieldSize.Width && j >= 0 && j < fieldSize.Height) Console.Write(mines[i, j] + " ");
                                else Console.Write("- ");
                            }
                            Console.WriteLine();
                        }

                        Console.WriteLine("Possible discoverable fields for " + x + ", " + y);
                        for (int j = y - 2; j <= y + 2; j++)
                        {
                            for (int i = x - 2; i <= x + 2; i++)
                            {
                                if (i >= 0 && i < fieldSize.Width && j >= 0 && j < fieldSize.Height) Console.Write(discoverables[i, j] + " ");
                                else Console.Write("- ");
                            }
                            Console.WriteLine();
                        }
                        */
                        #endregion

                        for (int i = 0; i < 8; i++)
                        {
                            ax = x + vx[i]; ay = y + vy[i];
                            if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height)
                            {
                                if (mines[ax, ay] == successfulCases && !field[ax, ay].isFlagged)
                                {
                                    Flag(ax, ay);
                                    change = true;
                                }
                            }
                        }

                        for (int i = x - 2; i <= x + 2; i++)
                        {
                            for (int j = y - 2; j <= y + 2; j++)
                            {
                                if (i >= 0 && i < fieldSize.Width && j >= 0 && j < fieldSize.Height)
                                {
                                    int count = 0;
                                    for (int l = 0; l < 8; l++)
                                    {
                                        ax = i + vx[l]; ay = j + vy[l];
                                        if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height && 
                                            field[ax, ay].isCovered && !field[ax, ay].isFlagged && mines[ax, ay] == 0)
                                        {
                                            count++;
                                        }
                                    }
                                    if (discoverables[i, j] == successfulCases && count != 0)
                                    {                                        
                                        SpecialReveal(i, j, successfulCases);
                                        if (minefield != null)
                                        {
                                            minefield.MainThreadInvoke(() =>
                                            {
                                                minefield.highlightedField = field[i, j];
                                                minefield.BlockedInteraction = false;
                                                minefield.BlockedInteraction = true;
                                            });
                                            System.Threading.Thread.Sleep(10);
                                        }
                                        change = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if (change == false && (minefield == null || (minefield != null && !minefield.IsImageField)))
                {
                    Tuple<int, int> mineToMove = null;
                    for (LinkedListNode<Tuple<int, int>> k = revealedNumbers.First; k != null && change == false; k = k.Next)
                    {
                        x = k.Value.Item1;
                        y = k.Value.Item2;
                        for (int i = 0; i < 8 && mineToMove == null; i++)
                        {
                            ax = x + vx[i];
                            ay = y + vy[i];
                            if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height && 
                                field[ax, ay].isMine && !field[ax, ay].isFlagged)
                            {
                                mineToMove = Tuple.Create(ax, ay);
                            }
                        }
                        if (mineToMove != null)
                        {
                            for(x = 0; x < fieldSize.Width && change == false; x++)
                            {
                                for (y = 0; y < fieldSize.Height && change == false; y++)
                                {
                                    if (!field[x, y].isMine && field[x, y].isCovered)
                                    {
                                        int countUncovered = 0;
                                        for (int i = 0; i < 8 && countUncovered == 0; i++)
                                        {
                                            ax = x + vx[i];
                                            ay = y + vy[i];
                                            if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height &&
                                                !field[ax, ay].isCovered) countUncovered++;
                                        }
                                        if (countUncovered == 0)
                                        {
                                            field[mineToMove.Item1, mineToMove.Item2] = new Tile(0);
                                            field[x, y] = new Tile(-1);
                                            for (int i = 0; i < 8; i++)
                                            {
                                                ax = mineToMove.Item1 + vx[i];
                                                ay = mineToMove.Item2 + vy[i];
                                                if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height)
                                                {
                                                    if (field[ax, ay].isMine)
                                                    {
                                                        field[mineToMove.Item1, mineToMove.Item2].isBlank = false;
                                                        field[mineToMove.Item1, mineToMove.Item2].isClue = true;
                                                        field[mineToMove.Item1, mineToMove.Item2].clueNumber++;
                                                    }
                                                    else if (field[ax, ay].isClue)
                                                    {
                                                        if (field[ax, ay].clueNumber == 1)
                                                        {
                                                            field[ax, ay].isClue = false;
                                                            field[ax, ay].isBlank = true;
                                                        }
                                                        field[ax, ay].clueNumber--;
                                                    }
                                                    if (field[ax, ay].isFlagged)
                                                    {
                                                        field[mineToMove.Item1, mineToMove.Item2].neighboringFlags++;
                                                    }
                                                }
                                            }
                                            for (int i = 0; i < 8; i++)
                                            {
                                                ax = x + vx[i];
                                                ay = y + vy[i];
                                                if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height 
                                                    && !field[ax, ay].isMine && !(ax == mineToMove.Item1 && ay == mineToMove.Item2))
                                                {
                                                    if (field[ax, ay].isBlank)
                                                    {
                                                        field[ax, ay].isBlank = false;
                                                        field[ax, ay].isClue = true;
                                                    }
                                                    field[ax, ay].clueNumber++;
                                                }
                                            }
                                            change = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            } while (change == true);
        }

        private bool CheckWin()
        {
            for (int x = 0; x < fieldSize.Width; x++)
            {
                for (int y = 0; y < fieldSize.Height; y++)
                {
                    if (!field[x, y].isMine && field[x, y].isCovered) return false;
                }
            }
            return true;
        }
        private void SpecialReveal(int x, int y, int s)
        {
            int ax, ay;
            for (int i = 0; i < 8; i++)
            {
                ax = x + vx[i]; ay = y + vy[i];
                if (ax >= 0 && ax < fieldSize.Width && ay >= 0 && ay < fieldSize.Height)
                {
                    if (field[ax, ay].isCovered && !field[ax, ay].isFlagged && mines[ax, ay] == 0)
                    {
                        field[ax, ay].isCovered = false;
                        revealedNumbers.AddFirst(Tuple.Create(ax, ay));
                        if (discoverables[ax, ay] == s)
                        {
                            SpecialReveal(ax, ay, s);
                        }
                    }
                }
            }
        }
        private void Bt(int n, int k, int pos, ref int nr)
        {
            for (int i = temp[pos - 1] + 1; i <= n; i++)
            {
                temp[pos] = i;
                if (pos == k)
                {
                    for (int j = 1; j <= k; j++)
                    {
                        comb[nr, temp[j] - 1] = true;
                        //Console.Write(temp[j] + " ");
                    }
                    //Console.WriteLine();
                    nr++;
                }
                else
                {
                    Bt(n, k, pos + 1, ref nr);
                }
            }
        }

        private void AddPossibility(int x, int y)
        {
            for (int i = x - 2; i <= x + 2; i++)
            {
                for (int j = y - 2; j <= y + 2; j++)
                {
                    if (i >= 0 && i < fieldSize.Width && j >= 0 && j < fieldSize.Height)
                    {
                        if (neighborhood[i, j].isFlagged)
                        {
                            mines[i, j]++;
                        }
                        else if (neighborhood[i, j].clueNumber == neighborhood[i, j].neighboringFlags && (i != x || j != y) && 
                            neighborhood[i, j].isClue && !neighborhood[i, j].isCovered && 
                            field[i, j].clueNumber > field[i, j].neighboringFlags)
                        {
                            discoverables[i, j]++;
                        }
                    }
                }
            }
        }

        private void Flag(int x, int y)
        {
            field[x, y].isFlagged = true;
            int bx, by;
            for (int i = 0; i < 8; i++)
            {
                bx = x + vx[i]; by = y + vy[i];
                if (bx >= 0 && bx < fieldSize.Width && by >= 0 && by < fieldSize.Height)
                    field[bx, by].neighboringFlags++;
            }
            if (minefield != null)
            {
                minefield.MainThreadInvoke(() =>
                {
                    minefield.flagsPlaced++;
                    minefield.OnTileFlag(new EventArgs());
                    minefield.BlockedInteraction = false;
                    minefield.BlockedInteraction = true;
                });
                System.Threading.Thread.Sleep(10);
            }
        }
    }
}
