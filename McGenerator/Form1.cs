using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace McGenerator
{
    public partial class Form1 : Form
    {
        private Logger logger;
        private ConfigHandler cfg;
        public Form1()
        {
            InitializeComponent();
            WriteLogMessage("GUI initialized", LogCategory.lcMessage);
            logger = new Logger(this);
            cfg = new ConfigHandler(logger);

            string[] types = cfg.GetAllTypes();
            for(int i = 0; i < types.Length; i++ )
            {
                typeComboBox.Items.Add(types[i]);
            }
            typeComboBox.SelectedIndex = 1;
        }

        public void WriteLogMessage(string msg, LogCategory lc )
        {
            msg += "\n";
            switch(lc)
            {
                case LogCategory.lcWarning:
                    richTextBox.SelectionColor = Color.Orange;
                    break;
                case LogCategory.lcError:
                    richTextBox.SelectionColor = Color.Red;
                    break;
                default:
                    break;
            }
            richTextBox.AppendText(msg);
            richTextBox.ScrollToCaret();
        }

        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string type = typeComboBox.SelectedItem.ToString();
            string mac = cfg.GetDefaultMAC(type);
            logger.Log(string.Format("Selected camera type: {0}", type), LogCategory.lcMessage);
            if( mac != string.Empty)
            {
                macTextBox.Text = mac;
            }
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            logger.Log(string.Format("{0}",macTextBox.Text), LogCategory.lcMessage);
        }
    }
}
