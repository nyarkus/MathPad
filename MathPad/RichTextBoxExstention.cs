// MathPad Copyright (C) 2024 nyarkus_, All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathNotepad;

public static class RichTextBoxExtensions
{
    public static void AppendText(this RichTextBox box, string text, Color color)
    {
        box.SuspendLayout();
        box.SelectionColor = color;

        box.AppendText(text);

        box.ScrollToCaret();
        box.ResumeLayout();
    }
}