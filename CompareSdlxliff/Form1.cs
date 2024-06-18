using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CompareSdlxliffLib;
using TextComparer;

namespace CompareSdlxliff
{
	public partial class Form1 : Form
	{
		SdlxliffComparer comparer = new SdlxliffComparer();
		bool mtOnly = true;
		bool isTarget = true;
		ComparisonType comparisonType = ComparisonType.Words;

		public Form1()
		{
			InitializeComponent();
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			listView1.Items.Clear();

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "*.sdlxliff";
			openFileDialog.Title = "Select sdlxliff file";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string path = openFileDialog.FileName;
				if (path != null)
					listView1.Items.Add(path);
				string[] paths = openFileDialog.FileNames;
				if (paths != null && paths.Length > 0)
				{
					foreach (string p in paths)
					{
						listView1.Items.Add(p);
					}

				}

			}
		}

		private void listView2_DoubleClick(object sender, EventArgs e)
		{
			listView2.Items.Clear();

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "*.sdlxliff";
			openFileDialog.Title = "Select sdlxliff file";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string path = openFileDialog.FileName;
				if (path != null)
					listView2.Items.Add(path);
				string[] paths = openFileDialog.FileNames;
				if (paths != null && paths.Length > 0)
				{
					foreach (string p in paths)
					{
						listView2.Items.Add(p);
					}

				}

			}
		}

		private void listView1_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

				listView1.Items.Clear();

				for (int i = 0; i < paths.Length; i++)
				{
					if (File.Exists(paths[i]))
					{
						if (Path.GetExtension(paths[i]).ToLower() == ".sdlxliff")
						{
							listView1.Items.Add(paths[i]);
						}
					}
					else if (Directory.Exists(paths[i]))
					{
						string[] files = Directory.GetFiles(paths[i], "*.sdlxliff", SearchOption.TopDirectoryOnly);
						for (int j = 0; j < files.Length; j++)
						{
							listView1.Items.Add(files[j]);
						}
					}
				}
			}
		}

		private void listView1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void listView1_KeyDown(object sender, KeyEventArgs e)
		{
			if (Keys.Delete == e.KeyCode)
			{
				foreach (ListViewItem listViewItem in ((ListView)sender).SelectedItems)
				{
					listViewItem.Remove();
				}
			}
		}



		private void listView2_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

				listView2.Items.Clear();

				for (int i = 0; i < paths.Length; i++)
				{
					if (File.Exists(paths[i]))
					{
						if (Path.GetExtension(paths[i]).ToLower() == ".sdlxliff")
						{
							listView2.Items.Add(paths[i]);
						}
					}
					else if (Directory.Exists(paths[i]))
					{
						string[] files = Directory.GetFiles(paths[i], "*.sdlxliff", SearchOption.TopDirectoryOnly);
						for (int j = 0; j < files.Length; j++)
						{
							listView2.Items.Add(files[j]);
						}
					}
				}
			}
		}

		private void listView2_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void listView2_KeyDown(object sender, KeyEventArgs e)
		{
			if (Keys.Delete == e.KeyCode)
			{
				foreach (ListViewItem listViewItem in ((ListView)sender).SelectedItems)
				{
					listViewItem.Remove();
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			progressBar1.Value = 0;

			ListView.ListViewItemCollection collection1 = listView1.Items;
			ListView.ListViewItemCollection collection2 = listView2.Items;

			if (collection1.Count != collection2.Count)
			{
				MessageBox.Show(this, "File amount not match", "Sdlxliff Comparer", MessageBoxButtons.OK, MessageBoxIcon.Error);
			} else
			{
				progressBar1.Maximum = collection1.Count;
				progressBar1.Value = 0;

				string masterReportfile = collection1[0].Text;
				masterReportfile = Path.Combine(Path.GetDirectoryName(masterReportfile), "MasterReport.txt");
				if(File.Exists(masterReportfile))
					File.Delete(masterReportfile);

				using (StreamWriter sw = new StreamWriter(masterReportfile, false, Encoding.UTF8))
				{
					for (int i = 0; i < collection1.Count; i++)
					{
						ListViewItem item1 = (ListViewItem)collection1[i];
						ListViewItem item2 = (ListViewItem)collection2[i];

						string file1 = item1.Text;
						string file2 = item2.Text;

						if (File.Exists(file1) && File.Exists(file2))
						{
							ChangeRate changeRate = comparer.Compare(file1, file2, mtOnly, isTarget, comparisonType);

							if (changeRate.Original != 0)
							{
								sw.WriteLine(file1 + "\t" + changeRate.Added + "/" + changeRate.Original + "\t" + Math.Round((decimal)changeRate.Added / changeRate.Original * 100, 2) + "%"
									);
								sw.WriteLine(file1 + "\t" + changeRate.Removed + "/" + changeRate.Original + "\t" + Math.Round((decimal)changeRate.Removed / changeRate.Original * 100, 2) + "%"
									);
							}
						}
						

						progressBar1.Value++;
					}
				}
			}
		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton1.Checked)
				mtOnly = true;
		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton2.Checked)
				mtOnly = false;
		}

		private void radioButton4_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton4.Checked)
				isTarget = false;
		}

		private void radioButton3_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton3.Checked)
				isTarget = true;
		}

		private void radioButton6_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton6.Checked)
				comparisonType = ComparisonType.Words;
		}

		private void radioButton5_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButton5.Checked)
				comparisonType = ComparisonType.Characters;
		}
	}
}
