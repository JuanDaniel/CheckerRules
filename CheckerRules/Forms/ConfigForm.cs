using BBI.JD.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CheckerRules.Properties;

namespace BBI.JD.Forms
{
    public partial class ConfigForm : Form
    {
        private CheckerRules formParent;
        private bool changed;

        public ConfigForm(CheckerRules formParent)
        {
            this.formParent = formParent;
            this.changed = false;

            InitializeComponent();
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {
            LoadAddins();
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;

            if (changed)
            {
                changed = false;
                formParent.LoadRules();
            }
        }

        private void grid_Addins_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            // Column update button
            if (e.ColumnIndex == 5)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Resources.btn_update.Width;
                var h = Resources.btn_update.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Resources.btn_update, new Rectangle(x, y, w, h));
                e.Handled = true;
            }
        }

        private void grid_Addins_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            // Column update button
            if (e.ColumnIndex == 5)
            {
                UpdateAddin(grid_Addins.Rows[e.RowIndex].Cells["cID"].Value.ToString());
            }
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                switch (Core.LoadAddin(openFileDialog1.FileName, false))
                {
                    case LoadResultType.SUCCESS:
                        MessageBox.Show("The addition was successfully completed.", "Successful add", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAddins();
                        changed = true;
                        break;
                    case LoadResultType.NO_IMPLEMENTED_RULE:
                        MessageBox.Show("The addin does not implement any checking rules.", "No checking rules", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                    case LoadResultType.ALREADY_ADDED:
                        MessageBox.Show("The addin is already loaded.", "Already added", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                    case LoadResultType.FILE_NOT_FOUND:
                        MessageBox.Show("Addin file not found.", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadAddins()
        {
            grid_Addins.Rows.Clear();
            
            foreach (AddinsElement addin in Config.GetAddinsLoaded().Cast<AddinsElement>())
            {
                grid_Addins.Rows.Add(
                    addin.Id,
                    Path.GetFileNameWithoutExtension(addin.Path),
                    string.Join(", ", addin.Rules.Cast<RulesElement>().Select(x => x.Name)),
                    addin.Version,
                    addin.Path
                );
            }
        }

        private void UpdateAddin(string id)
        {
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                switch (Core.UpdateAddin(id, openFileDialog1.FileName))
                {
                    case UpdateResultType.SUCCESS:
                        MessageBox.Show("The update was successfully completed.", "Successful update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAddins();
                        changed = true;
                        break;
                    case UpdateResultType.DIFFERENT_ADDIN:
                        MessageBox.Show("The update provided does not correspond to the current addin.", "Different addins", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        break;
                    case UpdateResultType.SAME_VERSION:
                        MessageBox.Show("Nothing to do, this is the same version.", "Same version", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    case UpdateResultType.LESS_VERSION:
                        MessageBox.Show("Nothing to do, it is an inferior version.", "Less version", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
