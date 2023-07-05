using System.Text;

namespace MHFQuestEditor.Utils
{
    class Utility
    {
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

        public static string ReformatString(string text)
        {
            text = text.Replace("\n\r\n", "\n");
            text = text.Replace("\r\n", "\n");
            text = text.Replace("", "");
            return text;
        }
    }
}
