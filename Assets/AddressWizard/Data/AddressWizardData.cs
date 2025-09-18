using System;


namespace AddressWizard.Data
{
    [Serializable]
    public class AddressWizardData
    {
        public bool autoSimplifyAddressableNames;
        public bool autoAddConstants;
        public ScriptSelectionType scriptSelectionType;
        public AddressableTypeData prefabsAddressableTypeData;
        public AddressableTypeData soAddressableTypeData;
        public AddressableTypeData generalAddressableTypeData;
        public bool firstRun = true;


        public AddressWizardData()
        {
            prefabsAddressableTypeData = new AddressableTypeData();
            soAddressableTypeData = new AddressableTypeData();
            generalAddressableTypeData = new AddressableTypeData();
            firstRun = true;
        }
    }
}