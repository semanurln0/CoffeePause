using System.Drawing;
using System.Windows.Forms;

namespace CoffeePause;

public class SudokuForm : Form
{
    private int[,] board = new int[9,9];
    private bool[,] fixedCells = new bool[9,9];
    private int[,] draft = new int[9,9]; // 0 for none, else draft value
    private int selectedX=0, selectedY=0;
    private const int CellSize = 50;
    private Panel gamePanel = null!;
    private DateTime startTime;

    public SudokuForm() { InitializeComponent(); NewRandomPuzzle(); startTime = DateTime.Now; }

    private void InitializeComponent()
    {
    this.Text = "Sudoku"; this.Size = new Size(520,600); this.StartPosition = FormStartPosition.CenterScreen; this.FormBorderStyle = FormBorderStyle.FixedSingle; this.MaximizeBox=false; this.KeyPreview=true; this.KeyDown += SudokuForm_KeyDown;
        var infoLabel = new Label { Text = "Left-click cell + numpad = place. Right-click + numpad = draft (temporary)", Location=new Point(10,10), Size=new Size(480,25), Font=new Font("Arial",10)}; this.Controls.Add(infoLabel);
        gamePanel = new Panel { Location=new Point(10,40), Size=new Size(9*CellSize,9*CellSize), BorderStyle=BorderStyle.FixedSingle }; gamePanel.Paint += GamePanel_Paint; gamePanel.MouseClick += GamePanel_MouseClick; this.Controls.Add(gamePanel);
        var newBtn = new Button { Text = "New Puzzle", Location = new Point(10, 480), Size = new Size(120,30) }; newBtn.Click += (s,e) => { NewRandomPuzzle(); this.Invalidate(); }; this.Controls.Add(newBtn);
    }

    private void GamePanel_MouseClick(object? sender, MouseEventArgs e) { selectedX = Math.Clamp(e.X / CellSize, 0, 8); selectedY = Math.Clamp(e.Y / CellSize, 0, 8); gamePanel.Invalidate(); }

    private void SudokuForm_KeyDown(object? sender, KeyEventArgs e)
    {
        // Right-click draft handled by checking Control key as alternative (user requested right-click + numpad; Right-click events not delivering numpad easily), so support both: if right mouse button is down flag or Control held -> draft
        bool draftMode = Control.ModifierKeys.HasFlag(Keys.Control) || MouseButtons == MouseButtons.Right;

        if (fixedCells[selectedY, selectedX]) return;
        if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9) { int val = e.KeyCode - Keys.D0; if (draftMode) { draft[selectedY, selectedX] = val; } else { board[selectedY, selectedX] = val; draft[selectedY, selectedX] = 0; } gamePanel.Invalidate(); CheckWin(); }
        else if (e.KeyCode >= Keys.NumPad1 && e.KeyCode <= Keys.NumPad9) { int val = e.KeyCode - Keys.NumPad0; if (draftMode) { draft[selectedY, selectedX] = val; } else { board[selectedY, selectedX] = val; draft[selectedY, selectedX] = 0; } gamePanel.Invalidate(); CheckWin(); }
        else if (e.KeyCode == Keys.D0 || e.KeyCode == Keys.NumPad0 || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete) { board[selectedY, selectedX] = 0; draft[selectedY, selectedX] = 0; gamePanel.Invalidate(); }
    }

    private void NewRandomPuzzle()
    {
        // simple backtracking generator: make full solved board then remove cells based on difficulty
        var generator = new SudokuGenerator();
        var puzzle = generator.Generate();
        for (int y=0;y<9;y++) for (int x=0;x<9;x++) { board[y,x] = puzzle[y,x]; fixedCells[y,x] = board[y,x] != 0; draft[y,x] = 0; }
    }

    private void CheckWin()
    {
    if (!IsBoardComplete()) return; if (!IsValidSolution()) return; var elapsed = DateTime.Now - startTime; HighScoreManager.Instance.AddScore("Sudoku", 1000, elapsed); var result = MessageBox.Show($"Congratulations! You solved the Sudoku!\nTime: {elapsed:mm\\:ss}\n\nPlay again?","Victory!",MessageBoxButtons.YesNo,MessageBoxIcon.Information); if (result==DialogResult.Yes) { startTime = DateTime.Now; NewRandomPuzzle(); gamePanel.Invalidate(); } else this.Close();
    }

    private bool IsBoardComplete() { for (int y=0;y<9;y++) for (int x=0;x<9;x++) if (board[y,x]==0) return false; return true; }

    private bool IsValidSolution()
    {
        for (int y=0;y<9;y++) { var seen=new HashSet<int>(); for (int x=0;x<9;x++) if (!seen.Add(board[y,x])) return false; }
        for (int x=0;x<9;x++) { var seen=new HashSet<int>(); for (int y=0;y<9;y++) if (!seen.Add(board[y,x])) return false; }
        for (int by=0;by<3;by++) for (int bx=0;bx<3;bx++) { var seen=new HashSet<int>(); for (int y=0;y<3;y++) for (int x=0;x<3;x++) if (!seen.Add(board[by*3+y,bx*3+x])) return false; }
        return true;
    }

    private void GamePanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics; for (int y=0;y<9;y++) for (int x=0;x<9;x++) { int drawX = x*CellSize; int drawY = y*CellSize; if (x==selectedX && y==selectedY) g.FillRectangle(Brushes.LightYellow, drawX, drawY, CellSize, CellSize); else g.FillRectangle(Brushes.White, drawX, drawY, CellSize, CellSize); if (board[y,x]!=0) { var brush = fixedCells[y,x] ? Brushes.Black : Brushes.Blue; var font=new Font("Arial",20,FontStyle.Bold); g.DrawString(board[y,x].ToString(), font, brush, drawX+15, drawY+10); } else if (draft[y,x]!=0) { var brush = Brushes.Gray; var font=new Font("Arial",12,FontStyle.Italic); g.DrawString(draft[y,x].ToString(), font, brush, drawX+5, drawY+5); } g.DrawRectangle(Pens.Gray, drawX, drawY, CellSize, CellSize); }
        var thickPen = new Pen(Color.Black,3); for (int i=0;i<=9;i+=3) { g.DrawLine(thickPen, i*CellSize, 0, i*CellSize, 9*CellSize); g.DrawLine(thickPen, 0, i*CellSize, 9*CellSize, i*CellSize); }
    }
}

// Simple Sudoku generator (backtracking)
public class SudokuGenerator
{
    private int[,] board = new int[9,9];
    private Random rand = new Random();
    public int[,] Generate()
    {
        FillDiagonal(); FillRemaining(0,3);
        // remove numbers to create puzzle (moderate difficulty)
        var attempts = 40; // remove 40 cells
        while (attempts>0) { int r=rand.Next(9), c=rand.Next(9); if (board[r,c]==0) continue; var backup=board[r,c]; board[r,c]=0; attempts--; }
        return (int[,])board.Clone();
    }
    private void FillDiagonal() { for (int i=0;i<9;i+=3) FillBox(i,i); }
    private void FillBox(int row,int col) { var nums = Enumerable.Range(1,9).OrderBy(x=>rand.Next()).ToList(); int idx=0; for (int i=0;i<3;i++) for (int j=0;j<3;j++) board[row+i,col+j]=nums[idx++]; }
    private bool CheckIfSafe(int i,int j,int num) { return !UsedInRow(i,num) && !UsedInCol(j,num) && !UsedInBox(i - i%3, j - j%3, num); }
    private bool UsedInRow(int i,int num) { for (int j=0;j<9;j++) if (board[i,j]==num) return true; return false; }
    private bool UsedInCol(int j,int num) { for (int i=0;i<9;i++) if (board[i,j]==num) return true; return false; }
    private bool UsedInBox(int rowStart,int colStart,int num) { for (int i=0;i<3;i++) for (int j=0;j<3;j++) if (board[rowStart+i,colStart+j]==num) return true; return false; }
    private bool FillRemaining(int i,int j)
    {
        if (j>=9 && i<8) { i++; j=0; }
        if (i>=9 && j>=9) return true;
        if (i<3) { if (j<3) j=3; }
        else if (i<9 && j== (i/3)*3) j = j+3;
        for (int num=1; num<=9; num++) { if (CheckIfSafe(i,j,num)) { board[i,j]=num; if (FillRemaining(i, j+1)) return true; board[i,j]=0; } }
        return false;
    }
}