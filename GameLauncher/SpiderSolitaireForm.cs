using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GameLauncher
{
    public partial class SpiderSolitaireForm : Form
    {
        private const int CardWidth = 80;
        private const int CardHeight = 120;
        private const int CardSpacing = 20;
        
        private List<List<Card>> tableau;
        private List<Card> stock;
        private Stack<List<Card>> undoStack;
        private int suits = 1;
        private int score;
        private bool isPaused;

        public SpiderSolitaireForm()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeComponent()
        {
            this.Text = "Spider Solitaire";
            this.ClientSize = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;
            this.DoubleBuffered = true;

            this.KeyDown += SpiderSolitaireForm_KeyDown;
            this.Paint += SpiderSolitaireForm_Paint;
            this.MouseDown += SpiderSolitaireForm_MouseDown;

            Button settingsBtn = new Button
            {
                Text = "Settings",
                Location = new Point(900, 20),
                Size = new Size(180, 40)
            };
            settingsBtn.Click += (s, e) => ShowSettings();
            this.Controls.Add(settingsBtn);

            Button scoreboardBtn = new Button
            {
                Text = "Scoreboard",
                Location = new Point(900, 70),
                Size = new Size(180, 40)
            };
            scoreboardBtn.Click += (s, e) => ShowScoreboard();
            this.Controls.Add(scoreboardBtn);

            Button newGameBtn = new Button
            {
                Text = "New Game",
                Location = new Point(900, 120),
                Size = new Size(180, 40)
            };
            newGameBtn.Click += (s, e) => InitializeGame();
            this.Controls.Add(newGameBtn);

            Button hintBtn = new Button
            {
                Text = "Hint",
                Location = new Point(900, 170),
                Size = new Size(180, 40)
            };
            hintBtn.Click += (s, e) => ShowHint();
            this.Controls.Add(hintBtn);

            Button undoBtn = new Button
            {
                Text = "Undo",
                Location = new Point(900, 220),
                Size = new Size(180, 40)
            };
            undoBtn.Click += (s, e) => UndoMove();
            this.Controls.Add(undoBtn);
        }

        private void InitializeGame()
        {
            tableau = new List<List<Card>>();
            stock = new List<Card>();
            undoStack = new Stack<List<Card>>();
            score = 500;

            // Create deck
            List<Card> deck = new List<Card>();
            string[] suitTypes = suits == 1 ? new[] { "Spades" } : 
                                suits == 2 ? new[] { "Spades", "Hearts" } : 
                                new[] { "Spades", "Hearts", "Diamonds", "Clubs" };

            for (int i = 0; i < 8; i++) // 8 decks for spider solitaire
            {
                foreach (string suit in suitTypes)
                {
                    for (int rank = 1; rank <= 13; rank++)
                    {
                        deck.Add(new Card { Suit = suit, Rank = rank });
                    }
                }
            }

            // Shuffle
            Random rand = new Random();
            deck = deck.OrderBy(c => rand.Next()).ToList();

            // Deal to tableau (10 columns)
            for (int i = 0; i < 10; i++)
            {
                tableau.Add(new List<Card>());
            }

            int cardIndex = 0;
            // First 4 columns get 6 cards, rest get 5
            for (int col = 0; col < 10; col++)
            {
                int cardsInColumn = col < 4 ? 6 : 5;
                for (int row = 0; row < cardsInColumn; row++)
                {
                    Card card = deck[cardIndex++];
                    card.FaceUp = (row == cardsInColumn - 1);
                    tableau[col].Add(card);
                }
            }

            // Rest goes to stock
            while (cardIndex < deck.Count)
            {
                stock.Add(deck[cardIndex++]);
            }

            this.Invalidate();
        }

        private void SpiderSolitaireForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.P)
            {
                isPaused = !isPaused;
                this.Invalidate();
            }
        }

        private void SpiderSolitaireForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.FromArgb(34, 139, 34));

            // Draw tableau
            for (int col = 0; col < tableau.Count; col++)
            {
                int x = 20 + col * (CardWidth + 10);
                int y = 50;

                if (tableau[col].Count == 0)
                {
                    g.DrawRectangle(Pens.White, x, y, CardWidth, CardHeight);
                }
                else
                {
                    for (int i = 0; i < tableau[col].Count; i++)
                    {
                        DrawCard(g, tableau[col][i], x, y + i * CardSpacing);
                    }
                }
            }

            // Draw stock indicator
            g.DrawString($"Stock: {stock.Count}", new Font("Arial", 12), Brushes.White, 20, 10);
            g.DrawString($"Score: {score}", new Font("Arial", 12), Brushes.White, 150, 10);

            // Draw pause overlay
            if (isPaused)
            {
                using (SolidBrush pauseBrush = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                {
                    g.FillRectangle(pauseBrush, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
                }
                g.DrawString("PAUSED", new Font("Arial", 48, FontStyle.Bold), Brushes.White, 
                    this.ClientSize.Width / 2 - 120, this.ClientSize.Height / 2 - 30);
            }
        }

        private void DrawCard(Graphics g, Card card, int x, int y)
        {
            Rectangle rect = new Rectangle(x, y, CardWidth, CardHeight);
            
            if (card.FaceUp)
            {
                g.FillRectangle(Brushes.White, rect);
                g.DrawRectangle(Pens.Black, rect);
                
                Color color = (card.Suit == "Hearts" || card.Suit == "Diamonds") ? Color.Red : Color.Black;
                string rankStr = card.Rank == 1 ? "A" : 
                                card.Rank == 11 ? "J" : 
                                card.Rank == 12 ? "Q" : 
                                card.Rank == 13 ? "K" : 
                                card.Rank.ToString();
                
                g.DrawString(rankStr, new Font("Arial", 16, FontStyle.Bold), 
                    new SolidBrush(color), x + 5, y + 5);
                g.DrawString(card.Suit[0].ToString(), new Font("Arial", 12), 
                    new SolidBrush(color), x + 5, y + 30);
            }
            else
            {
                g.FillRectangle(Brushes.DarkBlue, rect);
                g.DrawRectangle(Pens.White, rect);
            }
        }

        private void SpiderSolitaireForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (isPaused) return;

            // Check if stock clicked
            if (e.Y < 40 && e.X < 120)
            {
                DealFromStock();
                return;
            }

            // Simple click to move cards
            for (int col = 0; col < tableau.Count; col++)
            {
                int x = 20 + col * (CardWidth + 10);
                
                if (tableau[col].Count > 0)
                {
                    int topY = 50 + (tableau[col].Count - 1) * CardSpacing;
                    Rectangle rect = new Rectangle(x, topY, CardWidth, CardHeight);
                    
                    if (rect.Contains(e.Location))
                    {
                        // Try to move this card to another column
                        TryAutoMove(col);
                        break;
                    }
                }
            }
        }

        private void DealFromStock()
        {
            if (stock.Count >= 10)
            {
                SaveUndo();
                for (int i = 0; i < 10; i++)
                {
                    Card card = stock[0];
                    stock.RemoveAt(0);
                    card.FaceUp = true;
                    tableau[i].Add(card);
                }
                score -= 5;
                CheckForCompleteSequences();
                this.Invalidate();
            }
        }

        private void TryAutoMove(int fromCol)
        {
            if (tableau[fromCol].Count == 0) return;

            Card topCard = tableau[fromCol][tableau[fromCol].Count - 1];
            
            // Try to find a valid destination
            for (int toCol = 0; toCol < tableau.Count; toCol++)
            {
                if (toCol == fromCol) continue;
                
                if (tableau[toCol].Count == 0)
                {
                    if (fromCol != toCol)
                    {
                        SaveUndo();
                        tableau[toCol].Add(topCard);
                        tableau[fromCol].RemoveAt(tableau[fromCol].Count - 1);
                        
                        // Flip top card if needed
                        if (tableau[fromCol].Count > 0)
                            tableau[fromCol][tableau[fromCol].Count - 1].FaceUp = true;
                        
                        CheckForCompleteSequences();
                        this.Invalidate();
                        return;
                    }
                }
                else
                {
                    Card destCard = tableau[toCol][tableau[toCol].Count - 1];
                    if (destCard.Rank == topCard.Rank + 1)
                    {
                        SaveUndo();
                        tableau[toCol].Add(topCard);
                        tableau[fromCol].RemoveAt(tableau[fromCol].Count - 1);
                        
                        // Flip top card if needed
                        if (tableau[fromCol].Count > 0)
                            tableau[fromCol][tableau[fromCol].Count - 1].FaceUp = true;
                        
                        CheckForCompleteSequences();
                        this.Invalidate();
                        return;
                    }
                }
            }
        }

        private void CheckForCompleteSequences()
        {
            foreach (var column in tableau)
            {
                if (column.Count >= 13)
                {
                    // Check if last 13 cards form a complete sequence of same suit
                    bool isSequence = true;
                    string suit = column[column.Count - 13].Suit;
                    
                    for (int i = 0; i < 13; i++)
                    {
                        Card card = column[column.Count - 13 + i];
                        if (card.Rank != 13 - i || card.Suit != suit)
                        {
                            isSequence = false;
                            break;
                        }
                    }

                    if (isSequence)
                    {
                        column.RemoveRange(column.Count - 13, 13);
                        score += 100;
                        
                        // Flip top card if needed
                        if (column.Count > 0)
                            column[column.Count - 1].FaceUp = true;
                    }
                }
            }

            // Check win condition
            bool allEmpty = tableau.All(col => col.Count == 0);
            if (allEmpty && stock.Count == 0)
            {
                GameWon();
            }
        }

        private void SaveUndo()
        {
            // Save current state
            List<Card> state = new List<Card>();
            foreach (var col in tableau)
            {
                foreach (var card in col)
                {
                    state.Add(new Card 
                    { 
                        Suit = card.Suit, 
                        Rank = card.Rank, 
                        FaceUp = card.FaceUp 
                    });
                }
            }
            
            undoStack.Push(state);
            if (undoStack.Count > 10)
            {
                var items = undoStack.ToList();
                items.RemoveAt(items.Count - 1);
                undoStack = new Stack<List<Card>>(items.AsEnumerable().Reverse());
            }
        }

        private void UndoMove()
        {
            if (undoStack.Count > 0)
            {
                // This is a simplified undo - just notify user
                MessageBox.Show("Undo feature is simplified in this version.", "Undo");
            }
        }

        private void ShowHint()
        {
            // Simple hint: find any valid move
            for (int fromCol = 0; fromCol < tableau.Count; fromCol++)
            {
                if (tableau[fromCol].Count == 0) continue;
                
                Card topCard = tableau[fromCol][tableau[fromCol].Count - 1];
                
                for (int toCol = 0; toCol < tableau.Count; toCol++)
                {
                    if (toCol == fromCol) continue;
                    
                    if (tableau[toCol].Count == 0)
                    {
                        MessageBox.Show($"You can move from column {fromCol + 1} to column {toCol + 1}", "Hint");
                        return;
                    }
                    else
                    {
                        Card destCard = tableau[toCol][tableau[toCol].Count - 1];
                        if (destCard.Rank == topCard.Rank + 1)
                        {
                            MessageBox.Show($"You can move from column {fromCol + 1} to column {toCol + 1}", "Hint");
                            return;
                        }
                    }
                }
            }
            
            if (stock.Count >= 10)
            {
                MessageBox.Show("You can deal from the stock", "Hint");
            }
            else
            {
                MessageBox.Show("No obvious moves available", "Hint");
            }
        }

        private void GameWon()
        {
            HighScoreManager.SaveScore("SpiderSolitaire", score);
            var result = MessageBox.Show($"You Won! Score: {score}\n\nWanna Try Again?", "Victory", 
                MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
                InitializeGame();
            else
                this.Close();
        }

        private void ShowSettings()
        {
            Form settingsForm = new Form
            {
                Text = "Spider Solitaire Settings",
                Size = new Size(300, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            Label label = new Label
            {
                Text = "Number of Suits:",
                Location = new Point(20, 20),
                AutoSize = true
            };
            settingsForm.Controls.Add(label);

            ComboBox suitsBox = new ComboBox
            {
                Location = new Point(20, 50),
                Size = new Size(240, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            suitsBox.Items.AddRange(new object[] { 1, 2, 4 });
            suitsBox.SelectedItem = suits;
            settingsForm.Controls.Add(suitsBox);

            Button okBtn = new Button
            {
                Text = "OK",
                Location = new Point(100, 110),
                Size = new Size(80, 30)
            };
            okBtn.Click += (s, e) =>
            {
                suits = (int)(suitsBox.SelectedItem ?? 1);
                settingsForm.Close();
                InitializeGame();
            };
            settingsForm.Controls.Add(okBtn);

            settingsForm.ShowDialog(this);
        }

        private void ShowScoreboard()
        {
            var scores = HighScoreManager.LoadScores("SpiderSolitaire");
            string message = "Top Scores:\n\n";
            for (int i = 0; i < Math.Min(10, scores.Count); i++)
            {
                message += $"{i + 1}. {scores[i].Score} - {scores[i].Date:yyyy-MM-dd}\n";
            }
            if (scores.Count == 0)
                message = "No scores yet!";
            
            MessageBox.Show(message, "Scoreboard");
        }

        private class Card
        {
            public string Suit { get; set; } = "";
            public int Rank { get; set; }
            public bool FaceUp { get; set; }
        }
    }
}
