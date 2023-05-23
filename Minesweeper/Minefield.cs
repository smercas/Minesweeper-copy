using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    class Minefield : UserControl
    {
        #region constants
        public const int FIELD_MAX_WIDTH = 50;
        public const int FIELD_MAX_HEIGHT = 50;
        #endregion

        #region non-static attributes
        private bool isImageField = false;
        private bool blockedInteraction = false;
        private Tile[,] tile;
        private int cWidth, cHeight, mineCount;
        private readonly int[] vx = { -1, -1, -1, 0, 1, 1,  1,  0 };
        private readonly int[] vy = { -1,  0,  1, 1, 1, 0, -1, -1 };
        private bool firstClick, debugView = false;
        private bool gameover = true;

        public int flagsPlaced = 0, unsolvedTiles = 0;
        public int cSize;
        public bool mouseEnterDownEnabled ,questionMarkEnabled = false;
        public Tile highlightedField;
        #endregion

        #region static attributes
        /*
        private static readonly Bitmap textureTileNumbers = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 0, 16, 16 * 8), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureField = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 8 * 16, 16, 16), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureFlag = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 9 * 16, 16, 16), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureQuestionmark = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 10 * 16, 16, 16), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureMine = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 11 * 16, 16, 16), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureFlaggedRight = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 12 * 16, 16, 16), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureFlaggedWrong = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 13 * 16, 16, 16), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureBlank = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 14 * 16, 16, 16), PixelFormat.Format32bppArgb);
        */
        private static readonly Bitmap textureTileNumbers = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 0, 64, 64 * 8), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureField = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 8 * 64, 64, 64), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureFlag = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 9 * 64, 64, 64), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureQuestionmark = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 10 * 64, 64, 64), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureMine = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 11 * 64, 64, 64), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureFlaggedRight = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 12 * 64, 64, 64), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureFlaggedWrong = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 13 * 64, 64, 64), PixelFormat.Format32bppArgb);
        private static readonly Bitmap textureBlank = Minesweeper.Properties.Resources.tiletextures.Clone(new Rectangle(0, 14 * 64, 64, 64), PixelFormat.Format32bppArgb);

        #endregion

        #region custom events
        public delegate void GameOverHandler(object sender, GameOverArgs e);
        public event EventHandler Gamestart;
        public event EventHandler TileClick;
        public event EventHandler TileFlag;
        public event GameOverHandler GameOver;
        public event EventHandler StartGenerating;
        public event EventHandler EndGenerating;

        public class GameOverArgs : EventArgs
        {
            public bool won;
            public GameOverArgs(bool _won)
            {
                won = _won;
            }
        }
        protected virtual void OnStartGenerating(EventArgs e)
        {
            if (StartGenerating != null)
                StartGenerating(this, e);
        }
        protected virtual void OnEndGenerating(EventArgs e)
        {
            if (EndGenerating != null)
                EndGenerating(this, e);
        }
        protected virtual void OnGameOver(GameOverArgs e)
        {
            if (GameOver != null)
                GameOver(this, e);
        }
        protected virtual void OnGamestart(EventArgs e)
        {
            if (Gamestart != null)
                Gamestart(this, e);
        }
        protected virtual void OnTileClick(EventArgs e)
        {
            if (TileClick != null)
                TileClick(this, e);
        }
        public virtual void OnTileFlag(EventArgs e)
        {
            if (TileFlag != null)
                TileFlag(this, e);
        }
        #endregion

        #region getters and setters
        public bool IsImageField
        {
            get
            {
                return isImageField;
            }
            set
            {
                isImageField = value;
            }
        }
        public bool BlockedInteraction
        {
            get
            {
                return blockedInteraction;
            }
            set
            {
                blockedInteraction = value;
                this.Refresh();
            }
        }
        public int MinesHidden
        {
            get
            {
                return mineCount - flagsPlaced;
            }
        }
        public bool IsFirstClick
        {
            get
            {
                return firstClick;
            }
        }
        public bool IsGameOver
        {
            get
            {
                return gameover;
            }
        }
        public int MineCount
        {
            get
            {
                return mineCount;
            }
        }
        public Size FieldSize
        {
            get
            {
                return new Size(cWidth, cHeight);
            }
            set
            {
                if (value.Width <= FIELD_MAX_WIDTH && value.Height <= FIELD_MAX_HEIGHT)
                {
                    cWidth = value.Width;
                    cHeight = value.Height;
                    tile = new Tile[cWidth, cHeight];
                    this.Size = new System.Drawing.Size(cWidth * cSize, cHeight * cSize);
                    this.Refresh();
                }
            }
        }
        public bool DebugView
        {
            get
            {
                return debugView;
            }
            set
            {
                debugView = value; this.Refresh();
            }
        }
        #endregion

        #region constructors and overrides
        public Minefield()
        {
            this.DoubleBuffered = true;
            cWidth = 16;
            cHeight = 16;
            mineCount = 40;
            tile = new Tile[cWidth, cHeight];
            this.Size = new System.Drawing.Size(cWidth * cSize, cHeight * cSize);
        }

        public override void Refresh()
        {
            if (!blockedInteraction)
            {
                base.Refresh();
            }
        }
        protected override void OnResize(EventArgs e)
        {
            this.Size = new System.Drawing.Size(cWidth * cSize, cHeight * cSize);

        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!blockedInteraction && !gameover)
            {
                base.OnMouseDown(e);
                int x = (int)Math.Floor(e.X / (float) cSize);
                int y = (int)Math.Floor(e.Y / (float) cSize);
                if (x >= 0 && x < cWidth && y >= 0 && y < cHeight)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (tile[x, y].isCovered && !tile[x, y].isFlagged)
                        {
                            tile[x, y].isHeldDown = true;
                        }
                        if (!tile[x, y].isCovered)
                        {
                            int ax, ay;
                            for (int i = 0; i < 8; i++)
                            {
                                ax = x + vx[i]; ay = y + vy[i];
                                if (ax >= 0 && ax < cWidth && ay >=0 && ay < cHeight && 
                                    tile[ax, ay].isCovered && !tile[ax, ay].isFlagged)
                                {
                                    tile[ax, ay].isHeldDown = true;
                                }
                            }
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        if (tile[x, y].isCovered)
                        {
                            int ax, ay;
                            if (tile[x, y].isFlagged)
                            {
                                tile[x, y].isFlagged = false;
                                flagsPlaced--;
                                for (int i = 0; i < 8; i++)
                                {
                                    ax = x + vx[i]; ay = y + vy[i];
                                    if (ax >= 0 && ax < cWidth && ay >= 0 && ay < cHeight)
                                    {
                                        tile[ax, ay].neighboringFlags--;
                                    }
                                }
                                if (questionMarkEnabled) tile[x, y].hasQuestionMark = true;
                            }
                            else if (tile[x, y].hasQuestionMark)
                            {
                                tile[x, y].hasQuestionMark = false;
                            }
                            else
                            {
                                tile[x, y].isFlagged = true;
                                flagsPlaced++;
                                for (int i = 0; i < 8; i++)
                                {
                                    ax = x + vx[i]; ay = y + vy[i];
                                    if (ax >= 0 && ax < cWidth && ay >= 0 && ay < cHeight)
                                    {
                                        tile[ax, ay].neighboringFlags++;
                                    }
                                }
                            }
                            OnTileFlag(new EventArgs());
                        }
                    }
                }
                this.Refresh();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!blockedInteraction && !gameover)
            {
                base.OnMouseUp(e);
                if (e.X >= 0 && e.Y >= 0 && e.X < this.Width && e.Y < this.Height)
                {
                    int x = (int)Math.Floor(e.X / (float) cSize);
                    int y = (int)Math.Floor(e.Y / (float) cSize);
                    if (e.Button == MouseButtons.Left)
                    {
                        if (tile[x, y].isCovered && !tile[x, y].isFlagged)
                        {
                            tile[x, y].isHeldDown = false;
                            DiscoverTiles(x, y, true);
                        }
                        else if (!tile[x, y].isCovered)
                        {
                            int ax, ay;
                            for (int i = 0; i < 8; i++)
                            {
                                ax = x + vx[i]; ay = y + vy[i];
                                if (ax >= 0 && ax < cWidth && ay >= 0 && ay < cHeight)
                                {
                                    if (tile[ax, ay].isHeldDown) tile[ax, ay].isHeldDown = false;
                                }
                            }
                            if (tile[x, y].clueNumber <= tile[x, y].neighboringFlags)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    ax = x + vx[i]; ay = y + vy[i];
                                    if (ax >= 0 && ax < cWidth && ay >= 0 && ay < cHeight && 
                                        !gameover && !tile[ax, ay].isFlagged)
                                    {
                                        DiscoverTiles(ax, ay);
                                    }
                                }
                            }
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        tile[x, y].isHeldDown = false;
                    }
                }
                this.Refresh();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
                
            if (!blockedInteraction && !gameover)
            {
                base.OnMouseMove(e);
                if (e.X >= 0 && e.Y >= 0 && e.X < this.Width && e.Y < this.Height)
                {
                    int x = (int)Math.Floor(e.X / (float) cSize);
                    int y = (int)Math.Floor(e.Y / (float) cSize);
                    if (e.Button == MouseButtons.Left)
                    {
                        for (int i = x - 2; i <= x + 2; i++)
                        {
                            for (int j = y - 2; j <= y + 2; j++)
                            {
                                if (i >= 0 && i < cWidth && j >= 0 && j < cHeight)
                                {
                                    tile[i, j].isHeldDown = false;
                                }
                            }
                        }
                        if (!tile[x, y].isHeldDown)
                        {
                            if (tile[x, y].isCovered && !tile[x, y].isFlagged) tile[x, y].isHeldDown = true;
                            else if (!tile[x, y].isCovered && (tile[x, y].isBlank || tile[x, y].isClue))
                            {
                                int ax, ay;
                                for (int i = 0; i < 8; i++)
                                {
                                    ax = x + vx[i]; ay = y + vy[i];
                                    if (ax >= 0 && ax < cWidth && ay >= 0 && ay < cHeight &&
                                        tile[ax, ay].isCovered && !tile[ax, ay].isFlagged)
                                    {
                                        tile[ax, ay].isHeldDown = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < cWidth; i++)
                    {
                        tile[i, 0].isHeldDown = false;
                        tile[i, 1].isHeldDown = false;
                        tile[i, cHeight - 2].isHeldDown = false;
                        tile[i, cHeight - 1].isHeldDown = false;
                    }
                    for (int j = 0; j < cHeight; j++)
                    {
                        tile[0, j].isHeldDown = false;
                        tile[1, j].isHeldDown = false;
                        tile[cWidth - 2, j].isHeldDown = false;
                        tile[cWidth - 1, j].isHeldDown = false;
                    }
                }
            }
            this.Refresh();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (tile != null)
            {
                for (int i = 0; i < cWidth; i++)
                {
                    tile[i, 0].isHeldDown = false;
                    tile[i, 1].isHeldDown = false;
                    tile[i, cHeight - 2].isHeldDown = false;
                    tile[i, cHeight - 1].isHeldDown = false;
                }
                for (int j = 0; j < cHeight; j++)
                {
                    tile[0, j].isHeldDown = false;
                    tile[1, j].isHeldDown = false;
                    tile[cWidth - 2, j].isHeldDown = false;
                    tile[cWidth - 1, j].isHeldDown = false;
                }
            }
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!blockedInteraction && System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                e.Graphics.Clear(System.Drawing.Color.FromArgb(198, 198, 198));
                Pen line = new Pen(Color.FromArgb(128, 128, 128));

                for (int x = 0; x <= this.Width; x += cSize)
                {
                    e.Graphics.DrawLine(line, x, 0, x, this.Height);
                }
                for (int y = 0; y <= this.Height; y += cSize)
                {
                    e.Graphics.DrawLine(line, 0, y, this.Width, y);
                }
                
                for (int x = 0; x < (this.Width / cSize); x++)
                {
                    for (int y = 0; y < (this.Height / cSize); y++)
                    {
                        if (tile[x, y] != null)
                        {
                            DrawTile(x, y, e.Graphics);
                            if (tile[x, y].clueNumber < tile[x, y].neighboringFlags && !tile[x, y].isCovered && !tile[x, y].isFlagged && !tile[x, y].isMine)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 255, 0, 0)), new System.Drawing.Rectangle(x * cSize, y * cSize, cSize, cSize));
                            }
                            if (tile[x, y] == highlightedField)
                            {
                                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(150, 255, 0, 255)), new System.Drawing.Rectangle(x * cSize, y * cSize, cSize, cSize));

                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region private functions
        /*
            helper function for OnPaint override
            Draws one single tile
        */
        private void DrawTile(int x, int y, Graphics g)
        {
            if (tile[x, y].isHeldDown)
            {
                g.DrawImage(textureBlank, new Rectangle(x * cSize, y * cSize, cSize, cSize));
            }
            else if (tile[x, y].isCovered)
            {
                g.DrawImage(textureField, new Rectangle(x * cSize, y * cSize, cSize, cSize));
                if (tile[x, y].isFlagged)
                {
                    g.DrawImage(textureFlag, new Rectangle(x * cSize, y * cSize, cSize, cSize));
                }
                else if (tile[x, y].hasQuestionMark)
                {
                    g.DrawImage(textureQuestionmark, new Rectangle(x * cSize, y * cSize, cSize, cSize));
                }
            }
            else if (!tile[x, y].isCovered)
            {
                if (tile[x, y].isMine)
                {
                    if (tile[x, y].isBlownUp)
                    {
                        g.FillRectangle(System.Drawing.Brushes.Red, new System.Drawing.Rectangle(x * cSize + 1, y * cSize + 1, cSize - 1, cSize - 1));
                        g.DrawImage(textureMine, new Rectangle(x * cSize, y * cSize, cSize, cSize));
                    }
                    else if (tile[x, y].isFlaggedRight)
                    {
                        g.DrawImage(textureFlaggedRight, new Rectangle(x * cSize, y * cSize, cSize, cSize));
                    }
                    else g.DrawImage(textureMine, new Rectangle(x * cSize, y * cSize, cSize, cSize));
                }
                else if (tile[x, y].isFlaggedWrong)
                {
                    g.DrawImage(textureFlaggedWrong, new Rectangle(x * cSize, y * cSize, cSize, cSize));
                }
                else if (tile[x, y].isBlank)
                {
                    g.DrawImage(textureBlank, new Rectangle(x * cSize, y * cSize, cSize, cSize));
                }
                else if (tile[x, y].isClue)
                {
                    g.DrawImage(textureTileNumbers, new Rectangle(x * cSize, y * cSize, cSize, cSize), 0, (8 - tile[x, y].clueNumber) * 64, 64, 64, GraphicsUnit.Pixel);
                }
            }

            if (debugView && tile[x, y].isCovered)
            {
                if (tile[x, y].isMine)
                {
                    g.FillRectangle(System.Drawing.Brushes.Red, new System.Drawing.Rectangle(x * cSize, y * cSize, cSize / 4, cSize / 4));
                }
                else
                {
                    g.DrawString(tile[x, y].clueNumber.ToString(), new Font("Verdana", cSize * 10 / 16, FontStyle.Bold), Brushes.Black, new Rectangle(x * cSize, y * cSize, cSize, cSize));
                }
            }
        }

        /*
            Outputs the number of mines neighboring the tile
        */
        private int GenClueNumber(int x, int y)
        {
            int output = 0;
            int ax, ay;
            for (int i = 0; i < 8; i++)
            {
                ax = x + vx[i];
                ay = y + vy[i];
                if (ax >= 0 && ax < cWidth && ay >= 0 && ay < cHeight && 
                    tile[ax, ay].isMine)
                {
                    output++;
                }
            }
            return output;
        }

        private int GenClueNumber(int x, int y, Tile[,] specialtile)
        {
            int output = 0;
            int ax, ay;
            for (int i = 0; i < 8; i++)
            {
                ax = x + vx[i];
                ay = y + vy[i];
                if (ax >= 0 && ax < cWidth && ay >= 0 && ay < cHeight &&
                    specialtile[ax, ay] != null && specialtile[ax, ay].isMine)
                {
                    output++;
                }
            }
            return output;
        }

        /*
            Recalculates the numbers of mines for all tiles neighboring the tile given via the parameters
        */
        private void RemakeCluesAround(int x, int y)
        {
            int ax, ay;
            for (int i = 0; i < 8; i++)
            {
                ax = x + vx[i];
                ay = y + vy[i];
                if (ax >= 0 && ax < cWidth && ay >= 0 && ay < cHeight &&
                    !tile[ax, ay].isMine)
                {
                    tile[ax, ay] = new Tile(GenClueNumber(ax, ay));
                }
            }
        }

        /*
            checks if the game has been won
        */
        private bool CheckWin()
        {
            for (int x = 0; x < cWidth; x++)
            {
                for (int y = 0; y < cHeight; y++)
                {
                    if (!tile[x, y].isMine && tile[x, y].isCovered) return false;
                }
            }
            return true;
        }

        /*
            Reveals the tile given via parameters.
            If tile has 0 as clueNumber, revealss all adjacent fields recursively.
            If all blank and clue tiles have been flipped, calls RevealBoard and ends game.

            If the first tile revealed by the player is a amine, the mine will be moved to the first free field, in order to prevent the game from ending on the first click
        */
        private void DiscoverTiles(int x, int y, bool triggerEvent = false)
        {
            if (tile[x, y].isCovered && !tile[x, y].isFlagged)
            {
                if (firstClick && !isImageField && Properties.Settings.Default.alwaysSolvableEnabled)
                {
                    blockedInteraction = true;
                    OnStartGenerating(new EventArgs());
                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        GenerateField(MineCount, new Point(x, y));
                        this.MainThreadInvoke(() =>
                        {
                            OnEndGenerating(new EventArgs());
                            firstClick = false;
                            DiscoverTiles(x, y, true);
                            blockedInteraction = false;
                            this.Refresh();
                        });
                    });
                    return;
                }

                if (triggerEvent)
                {
                    OnTileClick(new EventArgs());
                }

                if (!tile[x, y].isMine)
                {
                    tile[x, y].isCovered = false;
                    unsolvedTiles--;
                    if (tile[x, y].clueNumber == 0)
                    {
                        int ax, ay;
                        for (int i = 0; i < 8; i++)
                        {
                            ax = x + vx[i]; ay = y + vy[i];
                            if (ax >= 0 && ax < cWidth && ay >= 0 && ay < cHeight && 
                                tile[ax, ay].isCovered && !tile[ax, ay].isFlagged)
                            {
                                DiscoverTiles(ax, ay);
                            }
                        }
                    }
                    if (CheckWin())
                    {
                        RevealBoard(true);
                    }
                }
                else
                {
                    /*
                        Removes mine from the first tile clicked if it's an image
                    */

                    /*
                    if (firstClick && isImageField)
                    {
                        for (int sx = 0; sx < cWidth; sx++)
                        {
                            for (int sy = 0; sy < cHeight; sy++)
                            {
                                if (!tile[sx, sy].isMine)
                                {
                                    tile[x, y] = new Tile(GenClueNumber(x, y));
                                    tile[sx, sy] = new Tile(-1);
                                    RemakeCluesAround(x, y);
                                    RemakeCluesAround(sx, sy);
                                    DiscoverTiles(x, y);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    */
                    {
                        tile[x, y].isBlownUp = true;
                        RevealBoard(false);
                    }
                }
                if (firstClick) firstClick = false;
            }
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Minefield
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Name = "Minefield";
            this.Size = new System.Drawing.Size(0, 0);
            this.ResumeLayout(false);

        }
        private Tile[,] FindValidField(CancellationToken? token, Point? solverOrigin = null)
        {
            Random rnd = new Random();
            MinefieldSolver solver = new MinefieldSolver();
            Tile[,] fieldToCalculate;
            do
            {
                fieldToCalculate = new Tile[cWidth, cHeight];

                for (int i = 0; i < mineCount; i++)
                {
                    int x = rnd.Next(0, cWidth);
                    int y = rnd.Next(0, cHeight);
                    if (fieldToCalculate[x, y] == null)
                    {
                        fieldToCalculate[x, y] = new Tile(-1);
                    }
                    else
                    {
                        i--;
                    }
                }
                for (int x = 0; x < cWidth; x++)
                {
                    for (int y = 0; y < cHeight; y++)
                    {
                        if (fieldToCalculate[x, y] == null)
                        {
                            fieldToCalculate[x, y] = new Tile(GenClueNumber(x, y, fieldToCalculate));
                        }
                    }
                }
            } while (
            solverOrigin.HasValue && !(token.HasValue && token.Value.IsCancellationRequested) && !(
            fieldToCalculate[solverOrigin.Value.X, solverOrigin.Value.Y].isBlank &&
            solver.TryToSolve(ref fieldToCalculate, FieldSize, solverOrigin.Value)
            ));
            return fieldToCalculate;
        }
        #endregion

        #region public functions
        /*
            generates a field from an input bitmap
        */
        public void GenerateFieldfromInput(Bitmap input)
        {
            if (input.Width > FIELD_MAX_WIDTH || input.Height > FIELD_MAX_HEIGHT)
            {
                throw new ArgumentException(String.Format("Images can't be larger than {0}px by {1}px", FIELD_MAX_WIDTH, FIELD_MAX_HEIGHT));
            }

            FieldSize = new Size(input.Width, input.Height);
            tile = new Tile[input.Width, input.Height];

            flagsPlaced = 0;
            unsolvedTiles = 0;

            for (int x = 0; x < cWidth; x++)
            {
                for (int y = 0; y < cHeight; y++)
                {
                    tile[x, y] = new Tile(0);
                }
            }

            BitmapData bData = input.LockBits(new Rectangle(0, 0, input.Width, input.Height), ImageLockMode.ReadWrite, input.PixelFormat);
            int bitsPerPixel = Image.GetPixelFormatSize(input.PixelFormat);
            int size = bData.Stride * input.Height;
            byte[] data = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(bData.Scan0, data, 0, size);
            int b = 0;
            for (int y = 0; y < cHeight; y++)
            {
                for (int x = 0; x < cWidth; x++)
                {
                    if (!(data[x * (bitsPerPixel / 8) + y * bData.Stride] == 255 && data[x * (bitsPerPixel / 8) + 1 + y * bData.Stride] == 255 && data[x * (bitsPerPixel / 8) + 2 + y * bData.Stride] == 255))
                    {
                        tile[x, y] = new Tile(-1);
                        b++;
                    }
                }
            }

            input.UnlockBits(bData);
            input.Dispose();
            mineCount = b;
            for (int x = 0; x < cWidth; x++)
            {
                for (int y = 0; y < cHeight; y++)
                {
                    if (!tile[x, y].isMine)
                    {
                        tile[x, y] = new Tile(GenClueNumber(x, y));
                    }
                }
            }
            isImageField = true;
            gameover = false;
            firstClick = true;
            this.Refresh();
            OnGamestart(new EventArgs());
        }

        /*
            generates a new random field with the given amount of mines
        */
        public void GenerateField(int mines, Point? solverOrigin = null)
        {
            mineCount = mines;
            flagsPlaced = 0;
            unsolvedTiles = 0;
            tile = null;
            if (solverOrigin.HasValue)
            {
                Task[] tasks = new Task[10];
                CancellationTokenSource ct = new CancellationTokenSource();
                ct.CancelAfter(20000);
                for (int i = 0; i < 10; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() =>
                    {
                        Tile[,] temp = FindValidField(ct.Token, solverOrigin);
                        if (tile == null)
                        {
                            tile = temp;
                        }
                    }, ct.Token);
                }
                Task.WaitAny(tasks);
                if (ct.IsCancellationRequested)
                {
                    System.Threading.Thread.Sleep(50);
                    MessageBox.Show("Couldn't find solvable field in under 20 Seconds." + Environment.NewLine +
                        "The generated board will require guessing at some point.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    ct.Cancel();
                }
                for (int x = 0; x < cWidth; x++)
                {
                    for (int y = 0; y < cHeight; y++)
                    {
                        tile[x, y].isCovered = true;
                        tile[x, y].isFlagged = false;
                        tile[x, y].neighboringFlags = 0;
                    }
                }
            }
            else
            {
                tile = FindValidField(null);
            }

            isImageField = false;
            gameover = false;
            firstClick = true;
            this.MainThreadInvoke(() =>
            {
                OnGamestart(new EventArgs());
            });
        }

        /*
            Ends the game and reveals tiles, checking if they were flagged correctly or not
        */
        public void RevealBoard(bool won)
        {
            gameover = true;
            OnGameOver(new GameOverArgs(won));
            for (int x = 0; x < cWidth; x++)
            {
                for (int y = 0; y < cHeight; y++)
                {
                    if ((tile[x, y].isFlagged || won) && tile[x, y].isMine)
                    {
                        tile[x, y].isCovered = false;
                        tile[x, y].isFlaggedRight = true;
                    }
                    else if (tile[x, y].isFlagged && !tile[x, y].isMine)
                    {
                        tile[x, y].isCovered = false;
                        tile[x, y].isFlaggedWrong = true;
                    }
                    else if (tile[x, y].isMine)
                    {
                        tile[x, y].isCovered = false;
                    }
                }
            }
            this.Refresh();
        }

        /*
            reveals all clues
        */
        public void RevealCluesAndBlanks()
        {
            for (int x = 0; x < cWidth; x++)
            {
                for (int y = 0; y < cHeight; y++)
                {
                    if (tile[x, y].clueNumber >= 0 && !tile[x, y].isMine) tile[x, y].isCovered = false;
                }
            }
            Refresh();
        }

        /*
            flags all tiles that have a mine on them
        */
        public void FlagAllMines()
        {
            if (!gameover && mineCount > 0)
            {
                for (int x = 0; x < cWidth; x++)
                {
                    for (int y = 0; y < cHeight; y++)
                    {
                        if (tile[x, y].isMine && tile[x, y].isCovered && !tile[x, y].isFlagged)
                        {
                            tile[x, y].isFlagged = true;
                            flagsPlaced++;
                        }
                    }
                }
                Refresh();
            }
        }

        public void RunVisualSolver()
        {
            OnStartGenerating(new EventArgs());
            MinefieldSolver mfs = new MinefieldSolver();
            this.blockedInteraction = true;
            Task.Factory.StartNew(() =>
            {
                mfs.TryToSolve(ref tile, FieldSize, Point.Empty, this);
                this.MainThreadInvoke(() =>
                {
                    highlightedField = null;
                    blockedInteraction = false;
                    OnEndGenerating(new EventArgs());
                    if (CheckWin())
                    {
                        RevealBoard(true);
                    }
                });
            });
        }
        #endregion
    }
}
