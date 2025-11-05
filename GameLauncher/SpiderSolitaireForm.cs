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
    private int selectedColumn = -1;
    private int selectedCardIndex = -1;

    public SpiderSolitaireForm()
    {
        InitializeComponent();
        InitializeGame();
    }

    private void InitializeComponent()
    {
        this.Text = "Spider Solitaire";
        this.Size = new Size(900, 650);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Paint += SpiderSolitaireForm_Paint;
        this.MouseClick += SpiderSolitaireForm_MouseClick;
        this.DoubleBuffered = true;

        var drawButton = new Button
        {
            Text = "Draw from Deck",
            Location = new Point(10, 10),
            Size = new Size(120, 30),
            Font = new Font("Arial", 10)
        };
        drawButton.Click += DrawButton_Click;
        this.Controls.Add(drawButton);

        var scoreLabel = new Label
        {
            Text = $"Score: {score} | Moves: {moves}",
            Location = new Point(150, 15),
            Size = new Size(300, 25),
            Font = new Font("Arial", 11),
            Name = "scoreLabel"
        };
        this.Controls.Add(scoreLabel);

        var newGameButton = new Button
        {
            Text = "New Game",
            Location = new Point(460, 10),
            Size = new Size(100, 30),
            Font = new Font("Arial", 10)
        };
        newGameButton.Click += (s, e) => {
            columns.Clear();
            deck.Clear();
            InitializeGame();
            this.Invalidate();
        };
        this.Controls.Add(newGameButton);
    }

    private void InitializeGame()
    {
        score = 500;
        moves = 0;
        startTime = DateTime.Now;
        selectedColumn = -1;
        selectedCardIndex = -1;
        columns.Clear();
        deck.Clear();
        
        string[] suits = { "♠", "♥", "♦", "♣" };
        string[] values = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        List<Card> allCards = new List<Card>();
        foreach (var suit in suits)
        {
            foreach (var value in values)
            {
                allCards.Add(new Card { Suit = suit, Value = value });
            }
        }

        Random rand = new Random();
        allCards = allCards.OrderBy(x => rand.Next()).ToList();

        for (int i = 0; i < 7; i++)
        {
            columns.Add(new List<Card>());
        }

        int cardIndex = 0;
        for (int i = 0; i < 7; i++)
        {
            int cardsInColumn = i < 3 ? 5 : 4;
            for (int j = 0; j < cardsInColumn; j++)
            {
                if (cardIndex < allCards.Count)
                {
                    columns[i].Add(allCards[cardIndex]);
                    cardIndex++;
                }
            }
        }

        while (cardIndex < allCards.Count)
        {
            deck.Add(allCards[cardIndex]);
            cardIndex++;
        }
        
        UpdateScore();
    }

    private void SpiderSolitaireForm_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Y < 60) return;

        int column = e.X / (CardWidth + 20);
        if (column >= 7 || column < 0) return;

        if (columns[column].Count == 0)
        {
            // Clicked on empty column - move selected cards here
            if (selectedColumn != -1 && selectedCardIndex != -1)
            {
                MoveCardsToColumn(selectedColumn, selectedCardIndex, column);
            }
            return;
        }

        // Calculate which card was clicked
        int cardIndex = -1;
        int y = 60;
        for (int i = 0; i < columns[column].Count; i++)
        {
            int cardY = y + i * CardSpacing;
            if (e.Y >= cardY && e.Y < cardY + (i == columns[column].Count - 1 ? CardHeight : CardSpacing))
            {
                cardIndex = i;
            }
        }

        if (cardIndex == -1) return;

        if (selectedColumn == -1)
        {
            // Select this card
            selectedColumn = column;
            selectedCardIndex = cardIndex;
        }
        else
        {
            // Try to move selected cards to this column
            if (column != selectedColumn)
            {
                MoveCardsToColumn(selectedColumn, selectedCardIndex, column);
            }
            selectedColumn = -1;
            selectedCardIndex = -1;
        }

        this.Invalidate();
    }

    private void MoveCardsToColumn(int fromColumn, int fromCardIndex, int toColumn)
    {
        if (fromColumn < 0 || fromColumn >= 7 || toColumn < 0 || toColumn >= 7) return;
        if (fromCardIndex < 0 || fromCardIndex >= columns[fromColumn].Count) return;

        // Get the cards to move
        List<Card> cardsToMove = new List<Card>();
        for (int i = fromCardIndex; i < columns[fromColumn].Count; i++)
        {
            cardsToMove.Add(columns[fromColumn][i]);
        }

        // Check if move is valid (can stack on empty or descending order)
        bool canMove = false;
        if (columns[toColumn].Count == 0)
        {
            canMove = true;
        }
        else
        {
            var topCard = columns[toColumn][columns[toColumn].Count - 1];
            var movingCard = cardsToMove[0];
            // Allow stacking in descending order
            if (GetCardValue(topCard.Value) == GetCardValue(movingCard.Value) + 1)
            {
                canMove = true;
            }
        }

        if (canMove)
        {
            // Move the cards
            foreach (var card in cardsToMove)
            {
                columns[toColumn].Add(card);
            }
            columns[fromColumn].RemoveRange(fromCardIndex, cardsToMove.Count);
            
            moves++;
            score -= 1;
            UpdateScore();
            CheckForCompletedSequence();
            CheckWin();
        }
        
        selectedColumn = -1;
        selectedCardIndex = -1;
    }

    private int GetCardValue(string value)
    {
        switch (value)
        {
            case "A": return 1;
            case "J": return 11;
            case "Q": return 12;
            case "K": return 13;
            default: return int.Parse(value);
        }
    }

    private void CheckForCompletedSequence()
    {
        // Check each column for a complete sequence K->A of same suit
        foreach (var column in columns)
        {
            if (column.Count >= 13)
            {
                var lastCards = column.Skip(column.Count - 13).ToList();
                if (IsCompleteSequence(lastCards))
                {
                    // Remove the sequence
                    column.RemoveRange(column.Count - 13, 13);
                    score += 100;
                    UpdateScore();
                }
            }
        }
    }

    private bool IsCompleteSequence(List<Card> cards)
    {
        if (cards.Count != 13) return false;
        
        string suit = cards[0].Suit;
        for (int i = 0; i < 13; i++)
        {
            if (cards[i].Suit != suit) return false;
            int expectedValue = 13 - i; // K=13 down to A=1
            if (GetCardValue(cards[i].Value) != expectedValue) return false;
        }
        return true;
    }

    private void DrawButton_Click(object? sender, EventArgs e)
    {
        if (deck.Count == 0)
        {
            MessageBox.Show("Deck is empty!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        for (int i = 0; i < 7 && deck.Count > 0; i++)
        {
            columns[i].Add(deck[0]);
            deck.RemoveAt(0);
        }
        
        score -= 5;
        moves++;
        UpdateScore();
        this.Invalidate();
    }

    private void UpdateScore()
    {
        var scoreLabel = this.Controls.Find("scoreLabel", false).FirstOrDefault() as Label;
        if (scoreLabel != null)
        {
            scoreLabel.Text = $"Score: {score} | Moves: {moves} | Deck: {deck.Count}";
        }
    }

    private void CheckWin()
    {
        if (columns.All(c => c.Count == 0) && deck.Count == 0)
        {
            var elapsed = DateTime.Now - startTime;
            HighScoreManager.Instance.AddScore("Spider Solitaire", score, elapsed);
            
            var result = MessageBox.Show($"Congratulations! You won!\nScore: {score}\nMoves: {moves}\nTime: {elapsed:mm\\:ss}\n\nPlay again?", 
                "Victory!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            
            if (result == DialogResult.Yes)
            {
                columns.Clear();
                deck.Clear();
                InitializeGame();
                this.Invalidate();
            }
            else
            {
                this.Close();
            }
        }
    }

    private void SpiderSolitaireForm_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;

        for (int col = 0; col < 7; col++)
        {
            int x = col * (CardWidth + 20) + 10;
            int y = 60;

            // Draw column background
            g.DrawRectangle(Pens.Gray, x, y, CardWidth, CardHeight);

            // Draw cards in column
            for (int i = 0; i < columns[col].Count; i++)
            {
                var card = columns[col][i];
                int cardY = y + i * CardSpacing;

                // Highlight selected cards
                bool isSelected = (col == selectedColumn && i >= selectedCardIndex);

                // Draw simple card
                var fillBrush = isSelected ? Brushes.LightYellow : Brushes.White;
                g.FillRectangle(fillBrush, x, cardY, CardWidth, CardHeight);
                g.DrawRectangle(Pens.Black, x, cardY, CardWidth, CardHeight);
                
                var color = (card.Suit == "♥" || card.Suit == "♦") ? Brushes.Red : Brushes.Black;
                var font = new Font("Arial", 16, FontStyle.Bold);
                g.DrawString($"{card.Value}{card.Suit}", font, color, x + 5, cardY + 5);
            }
        }

        // Draw instructions
        g.DrawString("Click a card to select, click another column to move. Match descending sequences!", 
            new Font("Arial", 9), Brushes.Black, new PointF(10, 580));
    }
}

public class Card
{
    public string Suit { get; set; } = "";
    public string Value { get; set; } = "";
}
