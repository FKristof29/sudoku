using System;
using System.Drawing;
using System.Windows.Forms;

namespace SudokuApp
{
    public class MainForm : Form
    {
        private SudokuBoard board      = new SudokuBoard();
        private TextBox[,]  cells      = new TextBox[9, 9];
        private int[,]      puzzle;

        private Panel     gridPanel;
        private ComboBox  difficultyBox;
        private Button    newGameBtn;
        private Button    checkBtn;
        private Button    solveBtn;
        private Label     statusLabel;
        private Label     titleLabel;

        private const int CELL_SIZE  = 52;
        private const int GRID_START = 10;

        public MainForm()
        {
            InitializeForm();
            StartNewGame();
        }


        private void InitializeForm()
        {
            this.Text            = "Sudoku";
            this.Size            = new Size(530, 640);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.BackColor       = Color.FromArgb(245, 245, 248);
            this.StartPosition   = FormStartPosition.CenterScreen;

            titleLabel = new Label
            {
                Text      = "SUDOKU",
                Font      = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 180),
                AutoSize  = true,
                Location  = new Point(170, 16)
            };

            gridPanel = new Panel
            {
                Location  = new Point(28, 70),
                Size      = new Size(CELL_SIZE * 9 + 4, CELL_SIZE * 9 + 4),
                BackColor = Color.FromArgb(50, 50, 80)
            };
            BuildGrid();

            int ctrlY = gridPanel.Bottom + 18;

            Label diffLabel = new Label
            {
                Text      = "Nehézség:",
                Font      = new Font("Segoe UI", 10),
                Location  = new Point(28, ctrlY + 4),
                AutoSize  = true,
                ForeColor = Color.FromArgb(60, 60, 90)
            };

            difficultyBox = new ComboBox
            {
                Location      = new Point(110, ctrlY),
                Size          = new Size(110, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 10),
                BackColor     = Color.White
            };
            difficultyBox.Items.AddRange(new object[] { "Könnyű", "Közepes", "Nehéz" });
            difficultyBox.SelectedIndex = 0;

            newGameBtn = MakeButton("Új játék", 240, ctrlY, Color.FromArgb(30, 120, 200));
            newGameBtn.Click += (s, e) => StartNewGame();

            ctrlY += 50;

            checkBtn = MakeButton("Ellenőrzés ✓", 28, ctrlY, Color.FromArgb(34, 160, 80));
            checkBtn.Size  = new Size(140, 38);
            checkBtn.Click += OnCheck;

            solveBtn = MakeButton("Megoldás", 185, ctrlY, Color.FromArgb(180, 80, 30));
            solveBtn.Click += OnSolve;

            ctrlY += 52;

            statusLabel = new Label
            {
                Text      = "",
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                Location  = new Point(28, ctrlY),
                Size      = new Size(460, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Controls.AddRange(new Control[] {
                titleLabel, gridPanel, diffLabel, difficultyBox,
                newGameBtn, checkBtn, solveBtn, statusLabel
            });
        }

        private Button MakeButton(string text, int x, int y, Color color)
        {
            return new Button
            {
                Text      = text,
                Location  = new Point(x, y),
                Size      = new Size(120, 38),
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
        }

        private void BuildGrid()
        {
            gridPanel.Controls.Clear();

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    // Calculate position with thick borders for 3x3 boxes
                    int extraX = (col / 3) * 2;
                    int extraY = (row / 3) * 2;
                    int x = GRID_START + col * CELL_SIZE + extraX;
                    int y = GRID_START + row * CELL_SIZE + extraY;

                    var cell = new TextBox
                    {
                        Location  = new Point(x, y),
                        Size      = new Size(CELL_SIZE - 2, CELL_SIZE - 2),
                        Font      = new Font("Segoe UI", 18, FontStyle.Bold),
                        TextAlign = HorizontalAlignment.Center,
                        MaxLength = 1,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(30, 80, 180),
                        Tag       = new Point(row, col)
                    };

                    cell.KeyPress  += OnCellKeyPress;
                    cell.TextChanged += OnCellChanged;
                    cells[row, col] = cell;
                    gridPanel.Controls.Add(cell);
                }
            }
        }


        private void StartNewGame()
        {
            Difficulty diff = difficultyBox.SelectedIndex switch
            {
                0 => Difficulty.Easy,
                1 => Difficulty.Medium,
                2 => Difficulty.Hard,
                _ => Difficulty.Easy
            };

            board.Generate(diff);
            puzzle = board.Puzzle;
            statusLabel.Text = "";

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    var cell = cells[r, c];
                    int val  = puzzle[r, c];

                    cell.Text      = val == 0 ? "" : val.ToString();
                    cell.ReadOnly  = val != 0;
                    cell.BackColor = val != 0
                        ? Color.FromArgb(235, 238, 250)
                        : Color.White;
                    cell.ForeColor = val != 0
                        ? Color.FromArgb(30, 30, 80)
                        : Color.FromArgb(30, 80, 180);
                    cell.Font = val != 0
                        ? new Font("Segoe UI", 18, FontStyle.Bold)
                        : new Font("Segoe UI", 18, FontStyle.Regular);
                }
            }
        }

        private void OnCheck(object sender, EventArgs e)
        {
            int[,] userBoard = GetUserBoard();
            if (userBoard == null)
            {
                ShowStatus("Töltsd ki az összes mezőt!", Color.FromArgb(180, 80, 30));
                return;
            }

            if (board.CheckSolution(userBoard))
            {
                ShowStatus("Gratulálok! Helyes megoldás!", Color.FromArgb(34, 160, 80));
                DisableAll();
            }
            else
            {
                ShowStatus("Vannak hibás cellák, próbáld újra!", Color.FromArgb(200, 50, 50));
                HighlightErrors(userBoard);
            }
        }

        private void OnSolve(object sender, EventArgs e)
        {
            int[,] sol = board.Solution;
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (board.IsPuzzleCell(r, c))
                    {
                        cells[r, c].Text      = sol[r, c].ToString();
                        cells[r, c].ForeColor = Color.FromArgb(34, 160, 80);
                        cells[r, c].ReadOnly  = true;
                    }

            ShowStatus("Megoldás megjelenítve.", Color.FromArgb(30, 80, 180));
        }

        private int[,] GetUserBoard()
        {
            int[,] result = new int[9, 9];
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                {
                    if (!int.TryParse(cells[r, c].Text, out int val) || val < 1 || val > 9)
                        return null;
                    result[r, c] = val;
                }
            return result;
        }

        private void HighlightErrors(int[,] userBoard)
        {
            int[,] sol = board.Solution;
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (board.IsPuzzleCell(r, c))
                        cells[r, c].BackColor = userBoard[r, c] != sol[r, c]
                            ? Color.FromArgb(255, 220, 220)
                            : Color.White;
        }

        private void DisableAll()
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    cells[r, c].ReadOnly = true;
        }

        private void ShowStatus(string text, Color color)
        {
            statusLabel.Text      = text;
            statusLabel.ForeColor = color;
        }


        private void OnCellKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && (e.KeyChar < '1' || e.KeyChar > '9'))
                e.Handled = true;
        }

        private void OnCellChanged(object sender, EventArgs e)
        {
            var cell = (TextBox) sender;
            cell.BackColor = Color.White;
            statusLabel.Text = "";
        }


        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
