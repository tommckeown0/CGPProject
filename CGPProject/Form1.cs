using CGPProject;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CGPProject
{
    public partial class GrafPack : Form
    {
        private MainMenu mainMenu;
        private bool selectSquareStatus = false;
        private bool selectTriangleStatus = false;
        private bool selectCircleStatus = false;

        private int clicknumber = 0;
        private Point one;
        private Point two;
        private Point three;

        private Shape[] shapes = new Shape[100];
        private int shapeCount = 0;
        private int selectedShapeIndex = -1;
        private bool selectMode = false;
        private bool dragAndDropMode = false;
        private bool isDragging = false;
        private Point previousLocation;

        private Label infoLabel;
        private Label selectModeLabel;
        private Label dragAndDropLabel;

        public GrafPack()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.White;

            // The following approach uses menu items coupled with mouse clicks
            MainMenu mainMenu = new MainMenu();
            MenuItem createItem = new MenuItem();
            MenuItem selectItem = new MenuItem();
            MenuItem squareItem = new MenuItem();
            MenuItem triangleItem = new MenuItem();
            MenuItem circleItem = new MenuItem();
            MenuItem transformItem = new MenuItem();
            MenuItem rotateItem = new MenuItem();
            MenuItem moveItem = new MenuItem();
            MenuItem dragAndDropItem = new MenuItem();
            MenuItem deleteItem = new MenuItem();
            MenuItem exitItem = new MenuItem();

            createItem.Text = "&Create";
            squareItem.Text = "&Square";
            triangleItem.Text = "&Triangle";
            circleItem.Text = "&Circle";
            selectItem.Text = "&Select";
            transformItem.Text = "&Transform";
            rotateItem.Text = "&Rotate";
            moveItem.Text = "&Move...";
            dragAndDropItem.Text = "&Drag and Drop";
            deleteItem.Text = "&Delete";
            exitItem.Text = "&Exit";

            infoLabel = new Label();
            infoLabel.Text = "Shapes: 0, Clicks: 0";
            infoLabel.AutoSize = true;
            infoLabel.Location = new Point(10, 10);
            Controls.Add(infoLabel);

            selectModeLabel = new Label();
            selectModeLabel.Text = "Select Mode: Off";
            selectModeLabel.AutoSize = true;
            selectModeLabel.Location = new Point(10, 30);
            Controls.Add(selectModeLabel);

            dragAndDropLabel = new Label();
            dragAndDropLabel.Text = "Drag and Drop: Off";
            dragAndDropLabel.AutoSize = true;
            dragAndDropLabel.Location = new Point(10, 50);
            Controls.Add(dragAndDropLabel);

            mainMenu.MenuItems.Add(createItem);
            mainMenu.MenuItems.Add(selectItem);
            mainMenu.MenuItems.Add(transformItem);
            mainMenu.MenuItems.Add(deleteItem);
            mainMenu.MenuItems.Add(exitItem);
            createItem.MenuItems.Add(squareItem);
            createItem.MenuItems.Add(triangleItem);
            createItem.MenuItems.Add(circleItem);
            transformItem.MenuItems.Add(rotateItem);
            transformItem.MenuItems.Add(moveItem);
            transformItem.MenuItems.Add(dragAndDropItem);

            selectItem.Click += new System.EventHandler(this.selectShape);
            squareItem.Click += new System.EventHandler(this.selectSquare);
            triangleItem.Click += new System.EventHandler(this.selectTriangle);
            circleItem.Click += new System.EventHandler(this.selectCircle);
            this.KeyDown += new KeyEventHandler(this.handleKeyDown);
            rotateItem.Click += new System.EventHandler(this.rotateShape);
            moveItem.Click += new System.EventHandler(this.moveShape);
            dragAndDropItem.Click += new System.EventHandler(this.dragAndDrop);
            deleteItem.Click += new System.EventHandler(this.deleteShape);
            exitItem.Click += (sender, e) =>
            {
                if (MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Exit();
                }
            };
            this.MouseDown += new MouseEventHandler(this.Form_MouseDown);
            this.MouseMove += new MouseEventHandler(this.Form_MouseMove);
            this.MouseUp += new MouseEventHandler(this.Form_MouseUp);

            this.Menu = mainMenu;
            this.MouseClick += mouseClick;
        }

        // Generally, all methods of the form are usually private
        private void selectSquare(object sender, EventArgs e)
        {
            selectSquareStatus = true;
            MessageBox.Show("Click OK and then click once each at two locations to create a square");
        }

        private void selectTriangle(object sender, EventArgs e)
        {
            selectTriangleStatus = true;
            MessageBox.Show("Click OK and then click once each at three locations to create a triangle");
        }

        private void selectCircle(object sender, EventArgs e)
        {
            selectCircleStatus = true;
            MessageBox.Show("Click OK and then click once each at two locations to create a circle");
        }

        private void selectShape(object sender, EventArgs e)
        {
            if (shapeCount == 0)
            {
                MessageBox.Show("Please draw some shapes to select first.");
                return;
            }
            MessageBox.Show("Select mode active, select shapes by clicking or with up/down arrows and pressing enter." +
                " Press enter when you have selected your shape. Press Esc to exit select mode.");
            selectMode = true;
            selectModeLabel.Text = "Select Mode: On";
        }

        // This method is quite important and detects all mouse clicks - other methods may need
        // to be implemented to detect other kinds of event handling eg keyboard presses.
        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (dragAndDropMode)
            {
                for (int i = 0; i < shapeCount; i++)
                {
                    if (shapes[i].Contains(e.Location))
                    {
                        selectedShapeIndex = i;
                        break;
                    }
                }
                if (selectedShapeIndex >= 0)
                {
                    if (shapes[selectedShapeIndex] is Square)
                    {
                        Square square = (Square)shapes[selectedShapeIndex];
                        square.MoveShape(20, 20);
                    }
                    else if (shapes[selectedShapeIndex] is Triangle)
                    {
                        Triangle triangle = (Triangle)shapes[selectedShapeIndex];
                        triangle.MoveShape(20, 20);
                    }
                    else if (shapes[selectedShapeIndex] is Circle)
                    {
                        Circle circle = (Circle)shapes[selectedShapeIndex];
                        circle.MoveShape(20, 20);
                    }

                    Graphics g = this.CreateGraphics();
                    Pen blackpen = new Pen(Color.Black);
                    g.Clear(this.BackColor);
                    for (int i = 0; i < shapeCount; i++)
                    {
                        shapes[i].Draw(g, blackpen, i);
                    }
                    g.Dispose();
                }
            }
            else if (selectMode)
            {
                for (int i = 0; i < shapeCount; i++)
                {
                    if (shapes[i].Contains(e.Location))
                    {
                        selectedShapeIndex = i;
                        // Redraw all shapes without highlight
                        Graphics g = this.CreateGraphics();
                        Pen blackpen = new Pen(Color.Black);
                        g.Clear(this.BackColor);
                        for (int j = 0; j < shapeCount; j++)
                        {
                            shapes[j].Draw(g, blackpen, j);
                        }
                        // Highlight the selected shape
                        shapes[selectedShapeIndex].Draw(this.CreateGraphics(), new Pen(Color.Red, 3), selectedShapeIndex);
                        g.Dispose();
                        break;
                    }
                }
            }
            else if (!selectMode)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // 'if' statements can distinguish different selected menu operations to implement.
                    // There may be other (better, more efficient) approaches to event handling,
                    // but this approach works.
                    if (selectSquareStatus == true)
                    {
                        if (clicknumber == 0)
                        {
                            one = new Point(e.X, e.Y);
                            clicknumber = 1;
                            infoLabel.Text =
                            infoLabel.Text = $"Shapes: {shapeCount}, Clicks: {clicknumber}";
                        }
                        else
                        {
                            two = new Point(e.X, e.Y);
                            clicknumber = 0;
                            selectSquareStatus = false;

                            Graphics g = this.CreateGraphics();
                            Pen blackpen = new Pen(Color.Black);

                            Square aShape = new Square(one, two);
                            aShape.Draw(g, blackpen, shapeCount);

                            addShapeToArray(aShape);
                            infoLabel.Text = $"Shapes: {shapeCount}, Clicks: {clicknumber}";
                        }
                    }
                    if (selectTriangleStatus == true)
                    {
                        if (clicknumber == 0)
                        {
                            one = new Point(e.X, e.Y);
                            clicknumber = 1;
                            infoLabel.Text = $"Shapes: {shapeCount}, Clicks: {clicknumber}";
                        }
                        else if (clicknumber == 1)
                        {
                            two = new Point(e.X, e.Y);
                            clicknumber = 2;
                            infoLabel.Text = $"Shapes: {shapeCount}, Clicks: {clicknumber}";
                        }
                        else
                        {
                            three = new Point(e.X, e.Y);
                            clicknumber = 0;
                            selectTriangleStatus = false;

                            Graphics g = this.CreateGraphics();
                            Pen blackpen = new Pen(Color.Black);

                            Triangle aShape = new Triangle(one, two, three);
                            aShape.Draw(g, blackpen, shapeCount);

                            addShapeToArray(aShape);
                            infoLabel.Text = $"Shapes: {shapeCount}, Clicks: {clicknumber}";
                        }
                    }
                    if (selectCircleStatus == true)
                    {
                        if (clicknumber == 0)
                        {
                            one = new Point(e.X, e.Y);
                            clicknumber = 1;
                            infoLabel.Text = $"Shapes: {shapeCount}, Clicks: {clicknumber}";
                        }
                        else
                        {
                            two = new Point(e.X, e.Y);
                            clicknumber = 0;
                            selectCircleStatus = false;

                            Graphics g = this.CreateGraphics();
                            Pen blackpen = new Pen(Color.Black);

                            Circle aShape = new Circle(one, two);
                            aShape.Draw(g, blackpen, shapeCount);

                            addShapeToArray(aShape);
                            infoLabel.Text = $"Shapes: {shapeCount}, Clicks: {clicknumber}";
                        }
                    }
                }
            }
        }
        private void handleKeyDown(object sender, KeyEventArgs e)
        {
            if (selectMode)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    selectMode = false;
                    MessageBox.Show("You selected shape: " + (selectedShapeIndex + 1));
                    selectModeLabel.Text = "Select Mode: Off";
                }
                else if (e.KeyCode == Keys.Up && selectedShapeIndex > 0)
                {
                    selectedShapeIndex--;
                }
                else if (e.KeyCode == Keys.Down && selectedShapeIndex < shapeCount - 1)
                {
                    selectedShapeIndex++;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    selectMode = false;
                    selectModeLabel.Text = "Select Mode: Off";
                    selectedShapeIndex = -1;
                }

                // draw all shapes without highlight
                Graphics g = this.CreateGraphics();
                Pen blackpen = new Pen(Color.Black);

                g.Clear(this.BackColor);

                for (int i = 0; i < shapeCount; i++)
                {
                    shapes[i].Draw(g, blackpen, i);
                }

                // Highlight the shape at the selected index with a highlight
                //MessageBox.Show("Selected shape index: " + selectedShapeIndex);
                if (selectedShapeIndex >= 0 && selectedShapeIndex < shapeCount)
                {
                    shapes[selectedShapeIndex].Draw(this.CreateGraphics(), new Pen(Color.Red, 3), selectedShapeIndex);
                }

                g.Dispose();
            }
        }

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (dragAndDropMode)
            {
                for (int i = 0; i < shapeCount; i++)
                {
                    if (shapes[i].Contains(e.Location))
                    {
                        selectedShapeIndex = i;
                        isDragging = true;
                        previousLocation = e.Location;
                        break;
                    }
                }
            }
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragAndDropMode && isDragging)
            {
                int dx = e.Location.X - previousLocation.X;
                int dy = e.Location.Y - previousLocation.Y;

                if (shapes[selectedShapeIndex] is Square)
                {
                    Square square = (Square)shapes[selectedShapeIndex];
                    square.MoveShape(dx, dy);
                }
                else if (shapes[selectedShapeIndex] is Triangle)
                {
                    Triangle triangle = (Triangle)shapes[selectedShapeIndex];
                    triangle.MoveShape(dx, dy);
                }
                else if (shapes[selectedShapeIndex] is Circle)
                {
                    Circle circle = (Circle)shapes[selectedShapeIndex];
                    circle.MoveShape(dx, dy);
                }

                Graphics g = this.CreateGraphics();
                Pen blackpen = new Pen(Color.Black);
                g.Clear(this.BackColor);
                for (int i = 0; i < shapeCount; i++)
                {
                    shapes[i].Draw(g, blackpen, i);
                }
                g.Dispose();

                previousLocation = e.Location;
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging && dragAndDropMode)
            {
                isDragging = false;
                selectedShapeIndex = -1;
            }
        }


        private void rotateShape(object sender, EventArgs e)
        {
            if (selectedShapeIndex < 0)
            {
                MessageBox.Show("Please select a shape first.");
                return;
            }
            if (selectedShapeIndex >= 0)
            {
                Shape selectedShape = shapes[selectedShapeIndex];
                var angle = PromptForAngle();
                if (selectedShape is Square)
                {
                    MessageBox.Show($"Rotate shape {selectedShapeIndex + 1} by {angle} degrees.");
                    Shape shape = shapes[selectedShapeIndex];
                    shape.Rotate(angle);

                    Graphics g = this.CreateGraphics();
                    Pen blackpen = new Pen(Color.Black);
                    g.Clear(this.BackColor);
                    for (int i = 0; i < shapeCount; i++)
                    {
                        shapes[i].Draw(g, blackpen, i);
                    }
                    shape.Draw(g, blackpen, selectedShapeIndex);
                    selectedShapeIndex = -1;
                    selectMode = false;
                    selectModeLabel.Text = "Select Mode: Off";
                }
                else if (selectedShape is Triangle)
                {
                    MessageBox.Show($"Rotate shape {selectedShapeIndex + 1} by {angle} degrees.");
                    Shape shape = shapes[selectedShapeIndex];
                    shape.Rotate(angle);

                    Graphics g = this.CreateGraphics();
                    Pen blackpen = new Pen(Color.Black);
                    g.Clear(this.BackColor);
                    for (int i = 0; i < shapeCount; i++)
                    {
                        shapes[i].Draw(g, blackpen, i);
                    }
                    shape.Draw(g, blackpen, selectedShapeIndex);
                    selectedShapeIndex = -1;
                    selectMode = false;
                    selectModeLabel.Text = "Select Mode: Off";
                }
                else if (selectedShape is Circle)
                {
                    MessageBox.Show("Rotating a circle seems a bit silly!");
                    Shape shape = shapes[selectedShapeIndex];
                    Graphics g = this.CreateGraphics();
                    Pen blackpen = new Pen(Color.Black);
                    g.Clear(this.BackColor);
                    for (int i = 0; i < shapeCount; i++)
                    {
                        shapes[i].Draw(g, blackpen, i);
                    }
                    shape.Draw(g, blackpen, selectedShapeIndex);
                    selectedShapeIndex = -1;
                    selectMode = false;
                    selectModeLabel.Text = "Select Mode: Off";
                }
            }
        }

        private void moveShape(object sender, EventArgs e)
        {
            if (selectedShapeIndex < 0)
            {
                MessageBox.Show("Please select a shape first.");
                return;
            }

            Form inputForm = new Form();
            {
                Width = 300;
                Height = 220;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                Text = "Move Shape";
                StartPosition = FormStartPosition.CenterScreen;
            }

            Label textLabel = new Label() { Left = 50, Top = 10, Width = 200, Text = "Enter the amount to move the shape by:" };
            Label xLabel = new Label() { Left = 50, Top = 50, Width = 50, Text = "X:" };
            TextBox textBoxX = new TextBox() { Left = 100, Top = 50, Width = 150 };
            Label yLabel = new Label() { Left = 50, Top = 80, Width = 50, Text = "Y:" };
            TextBox textBoxY = new TextBox() { Left = 100, Top = 80, Width = 150 };
            Button confirmationButton = new Button() { Text = "Move", Left = 50, Width = 200, Top = 120, DialogResult = DialogResult.OK };
            confirmationButton.Click += (sender2, e2) => { inputForm.Close(); };

            inputForm.Controls.Add(textLabel);
            inputForm.Controls.Add(textBoxX);
            inputForm.Controls.Add(textBoxY);
            inputForm.Controls.Add(confirmationButton);
            inputForm.Controls.Add(xLabel);
            inputForm.Controls.Add(yLabel);
            inputForm.AcceptButton = confirmationButton;

            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                if (textBoxX.Text == "" && textBoxY.Text == "")
                {
                    MessageBox.Show("Please enter a value for X or Y.");
                    return;
                }
            }

            if (selectedShapeIndex >= 0)
            {
                Shape selectedShape = shapes[selectedShapeIndex];
                if (selectedShape is Square)
                {
                    Square square = (Square)shapes[selectedShapeIndex];
                    square.MoveShape(textBoxX.Text != "" ? int.Parse(textBoxX.Text) : 0, textBoxY.Text != "" ? int.Parse(textBoxY.Text) : 0);

                    Graphics g = this.CreateGraphics();
                    Pen blackpen = new Pen(Color.Black);
                    g.Clear(this.BackColor);
                    for (int i = 0; i < shapeCount; i++)
                    {
                        shapes[i].Draw(g, blackpen, i);
                    }
                    square.Draw(g, blackpen, selectedShapeIndex);

                    selectedShapeIndex = -1;
                    selectMode = false;
                    selectModeLabel.Text = "Select Mode: Off";
                }
                else if (selectedShape is Triangle)
                {
                    Triangle triangle = (Triangle)shapes[selectedShapeIndex];
                    triangle.MoveShape(textBoxX.Text != "" ? int.Parse(textBoxX.Text) : 0, textBoxY.Text != "" ? int.Parse(textBoxY.Text) : 0);

                    Graphics g = this.CreateGraphics();
                    Pen blackpen = new Pen(Color.Black);
                    g.Clear(this.BackColor);
                    for (int i = 0; i < shapeCount; i++)
                    {
                        shapes[i].Draw(g, blackpen, i);
                    }
                    triangle.Draw(g, blackpen, selectedShapeIndex);

                    selectedShapeIndex = -1;
                    selectMode = false;
                    selectModeLabel.Text = "Select Mode: Off";
                }
                else if (selectedShape is Circle)
                {
                    Circle circle = (Circle)shapes[selectedShapeIndex];
                    circle.MoveShape(textBoxX.Text != "" ? int.Parse(textBoxX.Text) : 0, textBoxY.Text != "" ? int.Parse(textBoxY.Text) : 0);

                    Graphics g = this.CreateGraphics();
                    Pen blackpen = new Pen(Color.Black);
                    g.Clear(this.BackColor);
                    for (int i = 0; i < shapeCount; i++)
                    {
                        shapes[i].Draw(g, blackpen, i);
                    }
                    circle.Draw(g, blackpen, selectedShapeIndex);

                    selectedShapeIndex = -1;
                    selectMode = false;
                    selectModeLabel.Text = "Select Mode: Off";
                }
            }
        }

        private void dragAndDrop(object sender, EventArgs e)
        {
            if (!dragAndDropMode && shapeCount > 0)
            {
                dragAndDropMode = true;
                dragAndDropLabel.Text = "Drag and Drop: On";
                MessageBox.Show("Drag and Drop mode active, click and drag a shape to move it. Click drag and drop in the menu again to turn it off.");
            }
            else if (dragAndDropMode && shapeCount > 0)
            {
                dragAndDropMode = false;
                dragAndDropLabel.Text = "Drag and Drop: Off";
            }
            else
            {
                MessageBox.Show("Please draw some shapes first.");
            }
        }

        private void deleteShape(object sender, EventArgs e)
        {
            if (selectedShapeIndex < 0)
            {
                MessageBox.Show("Please select a shape first.");
                return;
            }
            if (selectedShapeIndex >= 0)
            {
                MessageBox.Show("Deleting shape " + (selectedShapeIndex + 1));
                for (int i = selectedShapeIndex; i < shapeCount - 1; i++)
                {
                    shapes[i] = shapes[i + 1];
                }
                shapeCount--;

                Graphics g = this.CreateGraphics();
                Pen blackpen = new Pen(Color.Black);
                g.Clear(this.BackColor);
                for (int i = 0; i < shapeCount; i++)
                {
                    shapes[i].Draw(g, blackpen, i);
                }

                infoLabel.Text = $"Shapes: {shapeCount}, Clicks: {clicknumber}";
                selectedShapeIndex = -1;
                selectMode = false;
                selectModeLabel.Text = "Select Mode: Off";
            }
        }

        private void addShapeToArray(Shape aShape)
        {
            // This needs to take into account resizing the array if necessary
            shapes[shapeCount] = aShape;
            shapeCount++;
        }

        private int PromptForAngle()
        {
            using (Form inputForm = new Form())
            {
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.Text = "Rotate Shape";
                inputForm.StartPosition = FormStartPosition.CenterScreen;

                Label textLabel = new Label() { Left = 50, Top = 20, Width = 200, Text = "Enter the rotation angle in degrees:" };
                TextBox textBoxAngle = new TextBox() { Left = 50, Top = 45, Width = 200 };
                Button confirmationButton = new Button() { Text = "Rotate", Left = 50, Top = 75, Width = 200, DialogResult = DialogResult.OK };

                inputForm.Controls.Add(textLabel);
                inputForm.Controls.Add(textBoxAngle);
                inputForm.Controls.Add(confirmationButton);
                inputForm.AcceptButton = confirmationButton;

                DialogResult result = inputForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    int angle;
                    if (int.TryParse(textBoxAngle.Text, out angle))
                    {
                        return angle;
                    }
                    else
                    {
                        MessageBox.Show("Invalid angle. Please enter a valid integer.");
                    }
                }
                return 0;
            }
        }
    }
}


