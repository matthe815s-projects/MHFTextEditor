using MHFQuestEditor.JPK;
using MHFQuestEditor.Utils;

namespace MHFQuestEditor
{
    class Quest
    {
        public string fileName = "";

        public string title = "";
        public string mainObjective = "";
        public string subObjective1 = "";
        public string subObjective2 = "";
        public string clearConditions = "";
        public string failureConditions = "";
        public string questContractor = "";
        public string questDescription = "";

        public string extraText = "";

        private MemoryStream decryptedData;
        public byte[] questCode = new byte[0];

        public Quest (string questId, string meta)
        {
            Stream questData = LoadFileAndInitializeData(questId, meta);
            DecryptData(questData);
            LocatePointersAndReadBody();
            ReadQuestStrings();
            questData.Close();
        }

        Stream LoadFileAndInitializeData (string questId, string meta)
        {
            Stream file = File.Open(string.Format("{0}{1}.bin", questId, meta), FileMode.Open);
            fileName = string.Format("{0}{1}.bin", questId, meta);
            return file;
        }

        void DecryptData (Stream file)
        {
            byte[] data = new byte[file.Length];
            file.Read(data, 0, data.Length);

            data = new JPKDecoder().UnpackSimple(data);
            decryptedData = new MemoryStream(data);
        }

        void LocatePointersAndReadBody ()
        {
            int mainPropsPointer = decryptedData.ReadByte();
            decryptedData.Seek(mainPropsPointer, SeekOrigin.Begin);
            decryptedData.Seek(40, SeekOrigin.Current);

            byte[] stringPointer = new byte[4];
            decryptedData.Read(stringPointer, 0, 4);

            // Seek to zero and read all the way to the string pointer - this will be copied one to one.
            decryptedData.Seek(0, SeekOrigin.Begin);
            questCode = new byte[BitConverter.ToInt32(stringPointer)];
            decryptedData.Read(questCode, 0, BitConverter.ToInt32(stringPointer));
        }

        void ReadQuestStrings ()
        {
            byte[] titlePointer = new byte[4];
            decryptedData.Read(titlePointer, 0, 4);
            byte[] mainObjectivePointer = new byte[4];
            decryptedData.Read(mainObjectivePointer, 0, 4);
            byte[] subObjectiveAPointer = new byte[4];
            decryptedData.Read(subObjectiveAPointer, 0, 4);
            byte[] subObjectiveBPointer = new byte[4];
            decryptedData.Read(subObjectiveBPointer, 0, 4);
            byte[] clearConditionsPointer = new byte[4];
            decryptedData.Read(clearConditionsPointer, 0, 4);
            byte[] failConditionsPointer = new byte[4];
            decryptedData.Read(failConditionsPointer, 0, 4);
            byte[] questContractorPointer = new byte[4];
            decryptedData.Read(questContractorPointer, 0, 4);
            byte[] questDescriptionPointer = new byte[4];
            decryptedData.Read(questDescriptionPointer, 0, 4);

            // Quest title
            decryptedData.Seek(BitConverter.ToInt32(titlePointer), SeekOrigin.Begin);
            title = Utility.ReadNullTerminated(decryptedData);

            // Quest main
            decryptedData.Seek(BitConverter.ToInt32(mainObjectivePointer), SeekOrigin.Begin);
            mainObjective = Utility.ReadNullTerminated(decryptedData);

            // Quest subobjective 1
            decryptedData.Seek(BitConverter.ToInt32(subObjectiveAPointer), SeekOrigin.Begin);
            subObjective1 = Utility.ReadNullTerminated(decryptedData);

            // Quest subobjective 2
            decryptedData.Seek(BitConverter.ToInt32(subObjectiveBPointer), SeekOrigin.Begin);
            subObjective2 = Utility.ReadNullTerminated(decryptedData);

            // Quest clear conditions
            decryptedData.Seek(BitConverter.ToInt32(clearConditionsPointer), SeekOrigin.Begin);
            clearConditions = Utility.ReadNullTerminated(decryptedData);

            // Quest failure conditions
            decryptedData.Seek(BitConverter.ToInt32(failConditionsPointer), SeekOrigin.Begin);
            failureConditions = Utility.ReadNullTerminated(decryptedData);

            // Quest contractor
            decryptedData.Seek(BitConverter.ToInt32(questContractorPointer), SeekOrigin.Begin);
            questContractor = Utility.ReadNullTerminated(decryptedData);

            // Quest description
            decryptedData.Seek(BitConverter.ToInt32(questDescriptionPointer), SeekOrigin.Begin);
            questDescription = Utility.ReadNullTerminated(decryptedData);
        }
    }
}
