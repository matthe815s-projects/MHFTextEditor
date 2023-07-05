using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MHFQuestEditor
{
    public partial class Preview : Form
    {
        public static Quest questReference;

        public Preview(Quest quest)
        {
            Preview.questReference = quest;
            InitializeComponent();
        }

        private void Preview_Load(object sender, EventArgs e)
        {
            label1.Text = Preview.questReference.title;
            label2.Text = Preview.questReference.mainObjective;
            label3.Text = Preview.questReference.title;
            label4.Text = Preview.questReference.title;
            label5.Text = Preview.questReference.questContractor;
            label6.Text = Preview.questReference.questDescription;
            label7.Text = Preview.questReference.clearConditions;
            label8.Text = Preview.questReference.failureConditions;
            label9.Text = Preview.questReference.subObjective1;
            label10.Text = Preview.questReference.subObjective2;

            this.Text = String.Format("Previewing: {0}", questReference.title);
        }
    }
}
