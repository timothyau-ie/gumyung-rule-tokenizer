using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;



namespace GumYungAnalyzer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<Link> _links;
        string[] _lines;
        Analyzer _analyzer;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Load links";

            if (false)
            {
                listBox1.ForeColor = Color.SeaShell;
                //tbNames.ForeColor = Color.SeaShell;
                //tbGraph.ForeColor = Color.SeaShell;
                lbNames.ForeColor = Color.SeaShell;
                lbGraph.ForeColor = Color.SeaShell;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            if (_links == null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = Application.StartupPath;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //links = new List<Link>();

                    string str = File.ReadAllText(dialog.FileName);

                    List<Token> tokens = new Tokenizer().Scan(str);

                    _links = new Parser(tokens).Parse();

                    _lines = File.ReadAllLines(dialog.FileName);
                    _analyzer = new Analyzer(_links, _lines);
                    Refresh();
                    btnFF.Enabled = true;
                    btnSkip.Enabled = true;
                }
            }
            else
            {
                while (_analyzer.AnalyzerState != AnalyzerStates.Finish)
                {
                    _analyzer.Step();
                    if (_analyzer.LastTryResult == TryResults.Added)
                    {
                        break;
                    }
                }
                Refresh();
            }
            button1.Enabled = true;
            button1.Focus();
        }

        void Refresh()
        {
            if (_analyzer.LinksToShow == null)
            {
                listBox1.Items.Clear();
            }
            else
            {
                if (_analyzer.LinksToShow.Any(l => !listBox1.Items.Contains(l)))
                {
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(_analyzer.LinksToShow.ToArray());
                }
                try
                {
                    listBox1.SelectedIndex = _analyzer.LastStepCounter;
                }
                catch
                {
                    listBox1.SelectedIndex = -1;
                }
            }

            int showMax = 60;
            








            StringBuilder sbGraph = new StringBuilder();
            StringBuilder sbNames = new StringBuilder();

            sbGraph.AppendLine("0123456789012345678901234567890123456789012345678901234567890");
            sbNames.AppendLine();

            //Dictionary<string, FighterResult> results = _analyzer.BaseSet.FighterResults;
            foreach (string fighter in _analyzer.BaseSet.FighterKeys)
            {
                int lower = _analyzer.BaseSet.GetFighterResult(fighter).Lower ?? 0;
                int higher = _analyzer.BaseSet.GetFighterResult(fighter).Higher ?? showMax;
                sbNames.AppendLine(fighter);
                sbGraph.Append(new String(' ', lower));
                sbGraph.AppendLine(new String('-', higher - lower + 1));
            }

            string names = sbNames.ToString();
            string graph = sbGraph.ToString();
            if (tbNames.Text != names)
            {
                tbNames.Text = names;
            }
            if (tbGraph.Text != graph)
            {
                tbGraph.Text = graph;
            }

            string text = "";
            if (_analyzer.AnalyzerState == AnalyzerStates.AbsoluteCertain)
            {
                text = "Create base set - absolute certain";
            }
            else if (_analyzer.AnalyzerState == AnalyzerStates.RelativeCertain)
            {
                text = "Create base set - relative certain";
            }
            if (this.Text != text)
            {
                this.Text = text;
            }
        }

        private void btnFF_Click(object sender, EventArgs e)
        {
            btnFF.Enabled = false;
            btnStop.Enabled = true;

            timer1.Interval = 100;
            timer1.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnFF.Enabled = true;
            btnStop.Enabled = false;

            timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button1_Click(null, null);
            if (_analyzer.AnalyzerState == AnalyzerStates.Finish)
            {
                btnStop_Click(null, null);
            }
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            AnalyzerStates state = _analyzer.AnalyzerState;
            int count = 0;
            while (_analyzer.AnalyzerState == state && count < 10000)
            {
                _analyzer.Step();
                count++;
            }

            Refresh();
            if (count >= 1000)
            {
                MessageBox.Show("1000 steps and state is not changed!");
            }
        }

    }
}