using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public byte[] questCode = new byte[0];


        public void Load (string questId, string meta)
        {
            Stream file = File.Open(string.Format("{0}{1}.bin", questId, meta), FileMode.Open);
            fileName = string.Format("{0}{1}.bin", questId, meta);

            byte[] data = new byte[file.Length];
            file.Read(data, 0, data.Length);

            data = new JPKDecoder().UnpackSimple(data);
            MemoryStream decryptedData = new MemoryStream(data);

            int mainPropsPointer = decryptedData.ReadByte();
            decryptedData.Seek(mainPropsPointer, SeekOrigin.Begin);
            decryptedData.Seek(40, SeekOrigin.Current);
            byte[] stringPointer = new byte[4];
            decryptedData.Read(stringPointer, 0, 4);
            decryptedData.Seek(0, SeekOrigin.Begin);

            questCode = new byte[BitConverter.ToInt32(stringPointer)];

            decryptedData.Read(questCode, 0, BitConverter.ToInt32(stringPointer));

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

            decryptedData.Seek(BitConverter.ToInt32(titlePointer), SeekOrigin.Begin);

            title = QuestSelector.ReadNullTerminated(decryptedData);

            decryptedData.Seek(BitConverter.ToInt32(mainObjectivePointer), SeekOrigin.Begin);

            mainObjective = QuestSelector.ReadNullTerminated(decryptedData);

            decryptedData.Seek(BitConverter.ToInt32(subObjectiveAPointer), SeekOrigin.Begin);

            subObjective1 = QuestSelector.ReadNullTerminated(decryptedData);

            decryptedData.Seek(BitConverter.ToInt32(subObjectiveBPointer), SeekOrigin.Begin);

            subObjective2 = QuestSelector.ReadNullTerminated(decryptedData);

            decryptedData.Seek(BitConverter.ToInt32(clearConditionsPointer), SeekOrigin.Begin);

            clearConditions = QuestSelector.ReadNullTerminated(decryptedData);

            decryptedData.Seek(BitConverter.ToInt32(failConditionsPointer), SeekOrigin.Begin);

            failureConditions = QuestSelector.ReadNullTerminated(decryptedData);

            decryptedData.Seek(BitConverter.ToInt32(questContractorPointer), SeekOrigin.Begin);

            questContractor = QuestSelector.ReadNullTerminated(decryptedData);

            decryptedData.Seek(BitConverter.ToInt32(questDescriptionPointer), SeekOrigin.Begin);

            questDescription = QuestSelector.ReadNullTerminated(decryptedData);

            file.Close();
        }
    }
}
