using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathNotepad;

public class TS
{
    private static bool _saved = true;
    public static bool Saved
    {
        get
        {
            return _saved;
        }
        set
        {
            if(value != _saved)
            {
                _saved = value;
                if (value)
                    form1.Text = form1.Text.TrimEnd('*', ' ');
                else
                    form1.Text += " *";
            }
            
        }
    }
    public static string LoadedFrom = "";
#nullable disable
    public static Form1 form1;
#nullable enable

    internal static void New(object? sender, EventArgs e)
    {
        if(!Saved)
        {
            DialogResult result = MessageBox.Show(
            "Документ не был сохранен. Хотите сохранить изменения?",
            "Создание нового документа",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

            if(result == DialogResult.Yes)
            {
                Save(sender, e);
                if (!Saved)
                    return;
            }
            else if(result == DialogResult.Cancel)
            {
                return;
            }
        }

        form1.rtb.Text = "";
        form1.Text = "Безымянный";
        Saved = true;
    }

    internal static void Open(object? sender, EventArgs e)
    {
        var ofd = new OpenFileDialog();

        ofd.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
        ofd.DefaultExt = ".txt";

        if(ofd.ShowDialog() == DialogResult.OK)
        {
            string selectedFilePath = ofd.FileName;

            form1.rtb.Text = File.ReadAllText(selectedFilePath);

            LoadedFrom = selectedFilePath;
            form1.Text = LoadedFrom;

            Saved = true;
        }
    }

    internal static void Save(object? sender, EventArgs e)
    {
        if(LoadedFrom == "")
            SaveAs(sender, e);
        
        File.WriteAllText(LoadedFrom, form1.rtb.Text);

        Saved = true;
    }

    internal static void SaveAs(object? sender, EventArgs e)
    {
        var sfd = new SaveFileDialog();

        sfd.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
        sfd.DefaultExt = ".txt";
        sfd.FileName = "Новый файл";

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            string selectedFilePath = sfd.FileName;

            File.WriteAllText(selectedFilePath, form1.rtb.Text);

            LoadedFrom = selectedFilePath;

            Saved = true;
        }
    }
}
