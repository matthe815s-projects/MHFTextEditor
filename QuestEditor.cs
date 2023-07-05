using System.Text;
using MHFQuestEditor.JPK;

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
            loadQuest(result);
        }

        private void loadQuest(DialogResult result)
        {
            switch (result)
            {
                case DialogResult.OK:
                    fileName = openFileDialog1.SafeFileName.Substring(0, openFileDialog1.SafeFileName.Length - 6);
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
                    break;
            }
        }

        private void LoadIfExists(string questId, string meta)
        {
            if (File.Exists(string.Format("{0}{1}.bin", questId, meta)))
            {
                files.Add(new Quest(questId, meta));
            }
        }

        public static string ReadNullTerminated(Stream file)
        {
            StreamReader reader = new StreamReader(file, Encoding.GetEncoding(932));
            var stringBuilder = new StringBuilder();

            int nextChar;
            while ((nextChar = reader.Read()) > 0)
            {
                switch ((byte)nextChar)
                {
                    case 10:
                        stringBuilder.AppendLine("\n");
                        break;
                    default:
                        stringBuilder.Append((char)nextChar);
                        break;
                }
            }

            return stringBuilder.ToString();
        }

        string ReformatString(string text)
        {
            text = text.Replace("\n\r\n", "\n");
            text = text.Replace("\r\n", "\n");
            text = text.Replace("", "");
            return text;
        }

        void RewriteFile()
        {
            files.ForEach(quest =>
            {
                BinaryWriter writer = new BinaryWriter(File.Open(quest.fileName + ".backup", FileMode.Create), Encoding.GetEncoding(932));

                writer.Write(quest.questCode);

                var pointerStarter = quest.questCode.Length + 32 + 28;

                var title = ReformatString(textBox1.Text);
                var main = ReformatString(textBox2.Text);
                var sub1 = ReformatString(textBox3.Text);
                var sub2 = ReformatString(textBox4.Text);
                var clear = ReformatString(textBox5.Text);
                var fail = ReformatString(textBox6.Text);
                var giver = ReformatString(textBox7.Text);
                var descrip = ReformatString(textBox8.Text);

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

                //writer.Write(originalStrings);
                //writer.Write(0x69);

                writer.Write(quest.extraText);
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
    }
}