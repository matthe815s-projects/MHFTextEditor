namespace MHFQuestEditor
{
    internal static class Program
    {
        static Form mainWindow = new ToolSelector();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(mainWindow);
        }

        public static void OpenQuestEditor()
        {
            mainWindow.Hide();
            Form questEditor = new QuestSelector();

            questEditor.FormClosed += (s, args) =>
            {
                mainWindow.Show();
            };

            questEditor.ShowDialog();
        }
    }
}