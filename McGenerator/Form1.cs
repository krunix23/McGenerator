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
        private Logger logger_;
        private ConfigHandler cfg_;
        public Form1()
        {
            InitializeComponent();
            WriteLogMessage("GUI initialized", LogCategory.lcMessage);
            logger_ = new Logger(this);
            cfg_ = new ConfigHandler(logger_);

            string[] sTypes = cfg_.GetAllTypes();
            for(int i = 0; i < sTypes.Length; i++ )
            {
                typeComboBox.Items.Add(sTypes[i]);
            }
            typeComboBox.SelectedIndex = 0;
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
            string sType = typeComboBox.SelectedItem.ToString();
            string sMac = cfg_.GetDefaultMAC(sType);
            logger_.Log(string.Format("Selected camera type: {0}", sType), LogCategory.lcMessage);
            if( sMac != string.Empty)
            {
                macTextBox.Text = sMac;
            }
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            string Mac = macTextBox.Text;
            string sMac = ConfigHandler.MACInt64ToString(ConfigHandler.MACStringToInt64(Mac));
            string sType = typeComboBox.SelectedItem.ToString();
            cfg_.GenerateSpreadsheet(sType, sMac, numericUpDown.Value);
            //logger.Log(string.Format("{0}",smac), LogCategory.lcMessage);
        }
    }
}
