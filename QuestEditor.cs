using System.Text;
using MHFQuestEditor.JPK;
using MHFQuestEditor.Utils;

namespace MHFQuestEditor
{
    public partial class QuestSelector : Form
    {
        string fileName = "";
        List<Quest> files = new List<Quest>();

        public QuestSelector()
        {
            InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = ".bin";
            openFileDialog1.Filter = "binary files (*.bin)|*.bin";
            openFileDialog1.FileName = "";
            openFileDialog1.InitialDirectory = "./";
            DialogResult result = openFileDialog1.ShowDialog();
            processDialog(result);
        }

        private void FillQuestList ()
        {
            string[] files = Directory.GetFiles("./");

            foreach (string file in files)
            {
                if (!file.EndsWith(".bin")) continue;
                treeView1.Nodes.Add(file);
            }
        }

        private void processDialog (DialogResult result)
        {
            switch (result)
            {
                case DialogResult.OK:
                    loadQuest(openFileDialog1.SafeFileName);
                    break;
            }
        }

        private void loadQuest (string fileName)
        {
            files = new List<Quest>();
            fileName = fileName.Substring(0, fileName.Length - 6);
            label2.Text = "Quest ID: " + fileName;

            LoadIfExists(fileName, "d0");
            LoadIfExists(fileName, "d1");
            LoadIfExists(fileName, "d2");
            LoadIfExists(fileName, "n0");
            LoadIfExists(fileName, "n1");
            LoadIfExists(fileName, "n2");

            Quest displayQuest = files[0];

            textBox1.Text = displayQuest.title;
            textBox2.Text = displayQuest.mainObjective;
            textBox3.Text = displayQuest.subObjective1;
            textBox4.Text = displayQuest.subObjective2;
            textBox5.Text = displayQuest.clearConditions;
            textBox6.Text = displayQuest.failureConditions;
            textBox7.Text = displayQuest.questContractor;
            textBox8.Text = displayQuest.questDescription;
        }

        private void LoadIfExists(string questId, string meta)
        {
            if (File.Exists(string.Format("{0}{1}.bin", questId, meta)))
            {
                files.Add(new Quest(questId, meta));
            }
        }

        void RewriteFile ()
        {
            files.ForEach(quest =>
            {
                BinaryWriter writer = new BinaryWriter(File.Open(quest.fileName + ".backup", FileMode.Create), Encoding.GetEncoding(932));

                writer.Write(quest.questCode);

                var pointerStarter = quest.questCode.Length + 32 + 28;

                var title   = Utility.ReformatString(textBox1.Text);
                var main    = Utility.ReformatString(textBox2.Text);
                var sub1    = Utility.ReformatString(textBox3.Text);
                var sub2    = Utility.ReformatString(textBox4.Text);
                var clear   = Utility.ReformatString(textBox5.Text);
                var fail    = Utility.ReformatString(textBox6.Text);
                var giver   = Utility.ReformatString(textBox7.Text);
                var descrip = Utility.ReformatString(textBox8.Text);

                writer.Write((Int32)pointerStarter);
                writer.Write((Int32)pointerStarter + Encoding.GetEncoding(932).GetBytes(title).Length + 1);
                writer.Write((Int32)pointerStarter + Encoding.GetEncoding(932).GetBytes(title).Length + Encoding.GetEncoding(932).GetBytes(main).Length + 2);
                writer.Write((Int32)pointerStarter + Encoding.GetEncoding(932).GetBytes(title).Length + Encoding.GetEncoding(932).GetBytes(main).Length
                    + Encoding.GetEncoding(932).GetBytes(sub1).Length + 3);
                writer.Write((Int32)pointerStarter + Encoding.GetEncoding(932).GetBytes(title).Length + Encoding.GetEncoding(932).GetBytes(main).Length
                    + Encoding.GetEncoding(932).GetBytes(sub1).Length + Encoding.GetEncoding(932).GetBytes(sub2).Length + 4);
                writer.Write((Int32)pointerStarter + Encoding.GetEncoding(932).GetBytes(title).Length + Encoding.GetEncoding(932).GetBytes(main).Length
                    + Encoding.GetEncoding(932).GetBytes(sub1).Length + Encoding.GetEncoding(932).GetBytes(sub2).Length + Encoding.GetEncoding(932).GetBytes(clear).Length + 5);
                writer.Write((Int32)pointerStarter + Encoding.GetEncoding(932).GetBytes(title).Length + Encoding.GetEncoding(932).GetBytes(main).Length
                    + Encoding.GetEncoding(932).GetBytes(sub1).Length + Encoding.GetEncoding(932).GetBytes(sub2).Length + Encoding.GetEncoding(932).GetBytes(clear).Length + Encoding.GetEncoding(932).GetBytes(fail).Length + 6);
                writer.Write((Int32)pointerStarter + Encoding.GetEncoding(932).GetBytes(title).Length + Encoding.GetEncoding(932).GetBytes(main).Length
                    + Encoding.GetEncoding(932).GetBytes(sub1).Length + Encoding.GetEncoding(932).GetBytes(sub2).Length + Encoding.GetEncoding(932).GetBytes(clear).Length + Encoding.GetEncoding(932).GetBytes(fail).Length
                    + Encoding.GetEncoding(932).GetBytes(giver).Length + 7);

                writer.Write("依頼の品を全て納品しました");
                writer.Write((byte)0x00);
                writer.Write(Encoding.GetEncoding(932).GetBytes(title));
                writer.Write((byte)0x00);
                writer.Write(Encoding.GetEncoding(932).GetBytes(main));
                writer.Write((byte)0x00);
                writer.Write(Encoding.GetEncoding(932).GetBytes(sub1));
                writer.Write((byte)0x00);
                writer.Write(Encoding.GetEncoding(932).GetBytes(sub2));
                writer.Write((byte)0x00);
                writer.Write(Encoding.GetEncoding(932).GetBytes(clear));
                writer.Write((byte)0x00);
                writer.Write(Encoding.GetEncoding(932).GetBytes(fail));
                writer.Write((byte)0x00);
                writer.Write(Encoding.GetEncoding(932).GetBytes(giver));
                writer.Write((byte)0x00);
                writer.Write(Encoding.GetEncoding(932).GetBytes(descrip));
                writer.Write((byte)0x00);
                writer.Write((byte)0x00);
                writer.Write((byte)0x00);

                writer.Seek(0, SeekOrigin.Begin);

                byte[] data = new byte[writer.BaseStream.Length];
                writer.BaseStream.Read(data, 0, data.Length);

                writer.Close();

                new JPKEncoder().JPKEncode(data, 3, quest.fileName, 100);
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RewriteFile();
        }

        private void QuestSelector_Load(object sender, EventArgs e)
        {
            FillQuestList();
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            TreeViewHitTestInfo info = treeView1.HitTest(treeView1.PointToClient(Cursor.Position));
            if (info != null)
                loadQuest(info.Node.Text.Substring(2));
        }
    }
}