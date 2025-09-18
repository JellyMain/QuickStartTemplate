using AddressWizard.Data;
using UnityEditor;
using UnityEngine;


namespace AddressWizard.Editor
{
    [InitializeOnLoad]
    public static class AddressWizardSaver
    {
        private const string SAVED_DATA_KEY = "SavedDataKey";
        private static AddressWizardData addressWizardData;



        static AddressWizardSaver()
        {
            LoadSavedData();
        }


        private static void LoadSavedData()
        {
            addressWizardData =
                JsonUtility.FromJson<AddressWizardData>(EditorPrefs.GetString(SAVED_DATA_KEY)) ??
                new AddressWizardData();
        }


        public static AddressWizardData GetSavedData()
        {
            return addressWizardData;
        }


        public static void SaveData(AddressWizardData data)
        {
            string json = JsonUtility.ToJson(data);
            EditorPrefs.SetString(SAVED_DATA_KEY, json);
        }
    }
}
