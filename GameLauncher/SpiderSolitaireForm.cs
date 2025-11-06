using System.Drawing;
using System.Windows.Forms;

namespace CoffeePause;

public class SpiderSolitaireForm : Form
{
    private List<List<Card>> columns = new List<List<Card>>();
    private List<Card> deck = new List<Card>();
    private int score = 500;
    private int moves = 0;
    private const int CardWidth = 80;
    private const int CardHeight = 120;
    private const int CardSpacing = 25;
    private DateTime startTime;

    // Drag/drop
    private bool isDragging = false;
    private int dragFromColumn = -1;
    private int dragFromIndex = -1;
    private Point dragOffset;
    private List<Card> dragCards = new List<Card>();
    private Point currentMousePos;

    // Undo (limit 5)
    private Stack<MoveRecord> undoStack = new();

    // Hint
    private (int fromCol, int fromIndex, int toCol)? currentHint = null;

    private Button undoButton = null!;
    private Button hintButton = null!;

    // Suit count setting
    private int suitCount = 1; // 1,2 or 4 (default 1)

    public SpiderSolitaireForm()
    {
        InitializeComponent();
        InitializeGame();
    }

    private void InitializeComponent()
    {
        this.Text = "Spider Solitaire";
        this.Size = new Size(1000, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Paint += SpiderSolitaireForm_Paint;
        this.MouseDown += SpiderSolitaireForm_MouseDown;
        this.MouseMove += SpiderSolitaireForm_MouseMove;
        this.MouseUp += SpiderSolitaireForm_MouseUp;
        this.DoubleBuffered = true;

        this.BackColor = ColorTranslator.FromHtml("#054900");

        var drawButton = new Button { Text = "Draw from Deck", Location = new Point(10, 10), Size = new Size(120, 30) };
        drawButton.Click += DrawButton_Click; this.Controls.Add(drawButton);

        undoButton = new Button { Text = "Undo", Location = new Point(140, 10), Size = new Size(80, 30), Enabled = false };
        undoButton.Click += (s, e) => UndoLastMove(); this.Controls.Add(undoButton);

        hintButton = new Button { Text = "Hint", Location = new Point(230, 10), Size = new Size(80, 30) };
        hintButton.Click += (s, e) => ShowHint(); this.Controls.Add(hintButton);

    var scoreLabel = new Label { Text = $"Score: {score} | Moves: {moves}", Location = new Point(320, 15), Size = new Size(300, 25), Name = "scoreLabel" };
        this.Controls.Add(scoreLabel);

        var newGameButton = new Button { Text = "New Game", Location = new Point(640, 10), Size = new Size(100, 30) };
        newGameButton.Click += (s, e) => { columns.Clear(); deck.Clear(); InitializeGame(); this.Invalidate(); };
        this.Controls.Add(newGameButton);

        // Suit selector
        var suitsLabel = new Label { Text = "Suits:", Location = new Point(760, 15), Size = new Size(40, 20) };
        this.Controls.Add(suitsLabel);
        var suitsCombo = new ComboBox { Location = new Point(800, 10), Size = new Size(80, 30), DropDownStyle = ComboBoxStyle.DropDownList };
        suitsCombo.Items.AddRange(new object[] { "1", "2", "4" }); 
        suitsCombo.SelectedIndex = 0; 
        suitsCombo.SelectedIndexChanged += (s, e) => { 
            if (suitsCombo.SelectedItem != null) 
            { 
                suitCount = int.Parse(suitsCombo.SelectedItem.ToString()!); 
                InitializeGame(); 
            } 
        };
        this.Controls.Add(suitsCombo);
    }

    private void InitializeGame()
    {
        score = 500; moves = 0; startTime = DateTime.Now; undoStack.Clear(); undoButton.Enabled = false; currentHint = null; columns.Clear(); deck.Clear();

        var suitsAll = new List<string> { "♠", "♥", "♦", "♣" };
        var suits = suitsAll.Take(suitCount).ToArray();
        var values = new[] { "A","2","3","4","5","6","7","8","9","10","J","Q","K" };

        var allCards = new List<Card>();
        foreach (var s in suits) foreach (var v in values) allCards.Add(new Card{Suit=s, Value=v});

        var rand = new Random(); allCards = allCards.OrderBy(x => rand.Next()).ToList();
        for (int i = 0; i < 7; i++) columns.Add(new List<Card>());
        int idx = 0; for (int i = 0; i < 7; i++) { int count = i < 3 ? 5 : 4; for (int j = 0; j < count && idx < allCards.Count; j++) columns[i].Add(allCards[idx++]); }
        while (idx < allCards.Count) deck.Add(allCards[idx++]); UpdateScore();
    }

    private void SpiderSolitaireForm_MouseDown(object? sender, MouseEventArgs e)
    {
        currentMousePos = e.Location; if (e.Y < 60) return; int col = e.X / (CardWidth + 20); if (col < 0 || col >= 7) return; if (columns[col].Count == 0) return;
        int cardIndex = -1; int y = 60; for (int i = 0; i < columns[col].Count; i++) { int cardY = y + i * CardSpacing; if (e.Y >= cardY && e.Y < cardY + (i == columns[col].Count - 1 ? CardHeight : CardSpacing)) cardIndex = i; }
        if (cardIndex == -1) return;
        isDragging = true; dragFromColumn = col; dragFromIndex = cardIndex; dragCards = columns[col].Skip(cardIndex).ToList(); dragOffset = new Point(e.X - (col * (CardWidth + 20) + 10), e.Y - (y + cardIndex * CardSpacing));
    }

    private void SpiderSolitaireForm_MouseMove(object? sender, MouseEventArgs e) { if (!isDragging) return; currentMousePos = e.Location; this.Invalidate(); }

    private void SpiderSolitaireForm_MouseUp(object? sender, MouseEventArgs e)
    {
        if (!isDragging) return; isDragging = false; int toCol = e.X / (CardWidth + 20); if (toCol < 0 || toCol >= 7) { dragCards.Clear(); this.Invalidate(); return; }
        if (!IsDescendingSequence(dragCards)) { dragCards.Clear(); this.Invalidate(); return; }
        bool canMove = false; if (columns[toCol].Count == 0) canMove = true; else { var top = columns[toCol].Last(); if (GetCardValue(top.Value) == GetCardValue(dragCards.First().Value) + 1) canMove = true; }
        if (canMove) { var moved = columns[dragFromColumn].Skip(dragFromIndex).ToList(); columns[toCol].AddRange(moved); columns[dragFromColumn].RemoveRange(dragFromIndex, moved.Count); undoStack.Push(new MoveRecord{FromColumn=dragFromColumn, FromIndex=dragFromIndex, ToColumn=toCol, Cards=moved}); while (undoStack.Count>5) undoStack = new Stack<MoveRecord>(undoStack.Reverse().Skip(1).Reverse()); undoButton.Enabled = undoStack.Count>0; moves++; score -= 1; UpdateScore(); CheckForCompletedSequence(); CheckWin(); }
        dragCards.Clear(); this.Invalidate();
    }

    private bool IsDescendingSequence(List<Card> cards) { if (cards.Count<=1) return true; for (int i=0;i<cards.Count-1;i++) if (GetCardValue(cards[i].Value) != GetCardValue(cards[i+1].Value)+1) return false; return true; }

    private void UndoLastMove() { if (undoStack.Count==0) return; var last = undoStack.Pop(); int removeCount = last.Cards.Count; columns[last.ToColumn].RemoveRange(columns[last.ToColumn].Count - removeCount, removeCount); columns[last.FromColumn].InsertRange(last.FromIndex, last.Cards); undoButton.Enabled = undoStack.Count>0; moves++; score += 1; UpdateScore(); this.Invalidate(); }

    private void ShowHint() { for (int from=0; from<7; from++) for (int idx=0; idx<columns[from].Count; idx++) { var moving = columns[from].Skip(idx).ToList(); if (!IsDescendingSequence(moving)) continue; for (int to=0; to<7; to++) { if (to==from) continue; bool canMove = false; if (columns[to].Count==0) canMove=true; else { var top = columns[to].Last(); if (GetCardValue(top.Value) == GetCardValue(moving.First().Value)+1) canMove=true; } if (canMove) { currentHint=(from,idx,to); this.Invalidate(); return; } } } currentHint=null; MessageBox.Show("No valid moves found.", "Hint", MessageBoxButtons.OK, MessageBoxIcon.Information); }

    private void DrawButton_Click(object? sender, EventArgs e) { if (deck.Count==0) { MessageBox.Show("Deck is empty!","Info",MessageBoxButtons.OK,MessageBoxIcon.Information); return; } for (int i=0;i<7 && deck.Count>0;i++) { columns[i].Add(deck[0]); deck.RemoveAt(0); } score -=5; moves++; UpdateScore(); this.Invalidate(); }

    private void UpdateScore() { var scoreLabel = this.Controls.Find("scoreLabel", false).FirstOrDefault() as Label; if (scoreLabel!=null) scoreLabel.Text = $"Score: {score} | Moves: {moves} | Deck: {deck.Count}"; }

    private void CheckForCompletedSequence() { for (int c=0;c<columns.Count;c++) { var column = columns[c]; if (column.Count>=13) { var lastCards = column.Skip(column.Count-13).ToList(); if (IsCompleteSequence(lastCards)) { column.RemoveRange(column.Count-13,13); score += 100; UpdateScore(); } } } }

    private bool IsCompleteSequence(List<Card> cards) { if (cards.Count!=13) return false; string suit = cards[0].Suit; for (int i=0;i<13;i++) { if (cards[i].Suit != suit) return false; int expected = 13 - i; if (GetCardValue(cards[i].Value) != expected) return false; } return true; }

    private int GetCardValue(string value) { switch(value) { case "A": return 1; case "J": return 11; case "Q": return 12; case "K": return 13; default: return int.Parse(value); } }

    private void CheckWin() { if (columns.All(c => c.Count==0) && deck.Count==0) { var elapsed = DateTime.Now - startTime; HighScoreManager.Instance.AddScore("Spider Solitaire", score, elapsed); var result = MessageBox.Show($"Congratulations! You won!\nScore: {score}\nMoves: {moves}\nTime: {elapsed:mm\\:ss}\n\nPlay again?", "Victory!", MessageBoxButtons.YesNo, MessageBoxIcon.Information); if (result==DialogResult.Yes) { columns.Clear(); deck.Clear(); InitializeGame(); this.Invalidate(); } else this.Close(); } }

    private void SpiderSolitaireForm_Paint(object? sender, PaintEventArgs e) { var g = e.Graphics; for (int col=0; col<7; col++) { int x = col * (CardWidth + 20) + 10; int y = 60; g.DrawRectangle(Pens.Gray, x, y, CardWidth, CardHeight); for (int i=0;i<columns[col].Count;i++) { var card = columns[col][i]; int cardY = y + i * CardSpacing; var fillBrush = Brushes.White; g.FillRectangle(fillBrush, x, cardY, CardWidth, CardHeight); g.DrawRectangle(Pens.Black, x, cardY, CardWidth, CardHeight); var color = (card.Suit=="♥"||card.Suit=="♦")? Brushes.Red: Brushes.Black; g.DrawString($"{card.Value}{card.Suit}", new Font("Arial",12,FontStyle.Bold), color, x+5, cardY+5); } }
    if (isDragging && dragCards.Count>0) { var drawX = currentMousePos.X - dragOffset.X; var drawY = currentMousePos.Y - dragOffset.Y; for (int i=0;i<dragCards.Count;i++) { var c = dragCards[i]; var r = new Rectangle(drawX, drawY + i*CardSpacing, CardWidth, CardHeight); g.FillRectangle(Brushes.White, r); g.DrawRectangle(Pens.Black, r); var color = (c.Suit=="♥"||c.Suit=="♦")? Brushes.Red: Brushes.Black; g.DrawString($"{c.Value}{c.Suit}", new Font("Arial",12,FontStyle.Bold), color, r.X+5, r.Y+5); } }
        if (currentHint.HasValue) { var h=currentHint.Value; int hx = h.fromCol * (CardWidth + 20) + 10; int hy = 60 + h.fromIndex * CardSpacing; g.DrawRectangle(new Pen(Color.Lime,3), hx, hy, CardWidth, CardHeight + (dragCards.Count-1)*CardSpacing); }
        g.DrawString("Drag cards to move. Right-click + numpad = draft on Sudoku; Hint shows one move. Undo available (5).", new Font("Arial",9), Brushes.White, new PointF(10, 640)); }
}

public class Card { public string Suit { get; set; } = ""; public string Value { get; set; } = ""; }
public class MoveRecord { public int FromColumn { get; set; } public int FromIndex { get; set; } public int ToColumn { get; set; } public List<Card> Cards { get; set; } = new(); }