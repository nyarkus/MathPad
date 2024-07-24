// MathPad Copyright (C) 2024 Yahor Klimenko, All Rights Reserved.
// Licensed under the MIT License.

using System.Threading.Channels;

namespace MathNotepad
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool changed = false;
        int started = 0;
        string old = "";
        private async void Text_TextChanged(object sender, EventArgs e)
        {
            if (rtb.Text != old)
                TS.Saved = false;

            old = rtb.Text;

            started++;
            await Task.Delay(300);
            if (changed)
            {
                changed = false;
                started--;
                return;
            }
            if (started == 1)
                Calculate();
            started--;
        }
        public void Calculate()
        {
            try
            {
                int select = rtb.SelectionStart;

                List<string> strings = new List<string>();
                Dictionary<string, decimal> variables = new();
                foreach (var str in rtb.Lines)
                {
                    if (str.StartsWith("//"))
                    {
                        strings.Add(str);
                        continue;
                    }
                    if (str.IndexOf("=") != -1)
                    {
                        var splited = str.Split('=');
                        if (splited.Length != 2)
                        {
                            foreach (var v in splited)
                                strings.Add(v.Trim(' '));
                        }
                        if (StringToMath.IsExpression(splited[0].Trim(' '), variables))
                            splited[1] = "";

                        if (string.IsNullOrWhiteSpace(splited[1]))
                        {
                            if (variables.ContainsKey(splited[0].Trim(' ')))
                            {
                                strings.Add(splited[0].TrimEnd(' ') + " = " + variables[splited[0].Trim(' ')]);
                                continue;
                            }
                            else
                                strings.Add(splited[0].Trim(' ') + " = " + (decimal)StringToMath.ParseExpression(splited[0], variables));
                        }
                        else if (!StringToMath.IsExpression(splited[0].Trim(' '), variables))
                        {
                            variables.Add(splited[0].Trim(' '), (decimal)StringToMath.ParseExpression(splited[1], variables));
                            strings.Add(splited[0].Trim(' ') + " = " + splited[1].Trim(' '));
                        }
                        else
                            strings.Add(str);
                    }
                    else
                        strings.Add(str);
                }
                changed = true;
                rtb.Lines = strings.ToArray();

                rtb.SelectionStart = select;
            }
            catch { }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            TS.form1 = this;

            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("File");

            // Создаем вложенные пункты меню "Файл"
            ToolStripMenuItem newMenuItem = new ToolStripMenuItem("New")
            {
                ShortcutKeys = Keys.Control | Keys.N,
                ShowShortcutKeys = true
            };
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Open")
            {
                ShortcutKeys = Keys.Control | Keys.O,
                ShowShortcutKeys = true
            };
            ToolStripMenuItem saveMenuItem = new ToolStripMenuItem("Save")
            {
                ShortcutKeys = Keys.Control | Keys.S,
                ShowShortcutKeys = true
            };
            ToolStripMenuItem saveAsMenuItem = new ToolStripMenuItem("Save as")
            {
                ShortcutKeys = Keys.Control | Keys.Shift | Keys.S,
                ShowShortcutKeys = true
            };

            fileMenuItem.DropDownItems.Add(newMenuItem);
            fileMenuItem.DropDownItems.Add(openMenuItem);
            fileMenuItem.DropDownItems.Add(saveMenuItem);
            fileMenuItem.DropDownItems.Add(saveAsMenuItem);

            menuStrip.Items.Add(fileMenuItem);

            newMenuItem.Click += TS.New;
            openMenuItem.Click += TS.Open;
            saveMenuItem.Click += TS.Save;
            saveAsMenuItem.Click += TS.SaveAs;
#nullable disable
            TS.New(null, null);
#nullable enable
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!TS.Saved)
            {
                DialogResult result = MessageBox.Show(
                    "The project has not been saved. Do you want to save the changes?",
                    "Exit",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
#nullable disable
                        TS.Save(null, null);
#nullable enable
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
        }
    }
}
