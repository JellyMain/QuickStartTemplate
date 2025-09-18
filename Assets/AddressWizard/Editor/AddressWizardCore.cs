using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AddressWizard.Data;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;


namespace AddressWizard.Editor
{
    [InitializeOnLoad]
    public static class AddressWizardCore
    {
        private static MonoScript generalSelectedScript;
        private static MonoScript prefabsSelectedScript;
        private static MonoScript scriptableObjectsSelectedScript;
        private static int prefabsSelectedClassIndex;
        private static int scriptableObjectsSelectedClassIndex;
        private static int generalSelectedClassIndex;

        private static readonly Dictionary<AddressableAssetType, List<string>> generatedConstants =
            new Dictionary<AddressableAssetType, List<string>>();

        private static Type[] prefabsAvailableClasses;
        private static Type[] scriptableObjectsAvailableClasses;
        private static Type[] generalAvailableClasses;
        private static Type prefabsSelectedClass;
        private static Type scriptableObjectsSelectedClass;
        private static Type generalSelectedClass;
        private static List<AddressableAssetGroup> groups;
        private static AddressWizardData addressWizardData;
        private static bool autoSimplifyAddressableNames;
        private static bool autoAddConstants;
        private static ScriptSelectionType scriptSelectionType;
        private static readonly List<string> logMessages = new List<string>();


        static AddressWizardCore()
        {
            UpdateData();
            EditorApplication.delayCall += CheckFirstRun;
        }


        private static void CheckFirstRun()
        {
            if (addressWizardData.firstRun)
            {
                addressWizardData.firstRun = false;
                AddressWizardWindow.ShowWindow();
                AddressWizardSaver.SaveData(addressWizardData);
            }
        }


        public static List<string> GetLogMessages()
        {
            return logMessages;
        }


        public static void ClearLogMessages()
        {
            logMessages.Clear();
        }


        public static void UpdateData()
        {
            addressWizardData = AddressWizardSaver.GetSavedData();
            generalSelectedScript =
                AssetDatabase.LoadAssetAtPath<MonoScript>(addressWizardData.generalAddressableTypeData
                    .selectedScriptPath);
            prefabsSelectedScript =
                AssetDatabase.LoadAssetAtPath<MonoScript>(addressWizardData.prefabsAddressableTypeData
                    .selectedScriptPath);
            scriptableObjectsSelectedScript =
                AssetDatabase.LoadAssetAtPath<MonoScript>(addressWizardData.soAddressableTypeData.selectedScriptPath);
            prefabsSelectedClassIndex = addressWizardData.prefabsAddressableTypeData.selectedClassIndex;
            scriptableObjectsSelectedClassIndex = addressWizardData.soAddressableTypeData.selectedClassIndex;
            generalSelectedClassIndex = addressWizardData.generalAddressableTypeData.selectedClassIndex;
            autoSimplifyAddressableNames = addressWizardData.autoSimplifyAddressableNames;
            autoAddConstants = addressWizardData.autoAddConstants;
            scriptSelectionType = addressWizardData.scriptSelectionType;

            if (generalSelectedScript != null)
            {
                GetAvailableClassesNames(generalSelectedScript);
            }

            if (prefabsSelectedScript != null)
            {
                GetAvailableClassesNames(prefabsSelectedScript);
            }

            if (scriptableObjectsSelectedScript != null)
            {
                GetAvailableClassesNames(scriptableObjectsSelectedScript);
            }

            AddressableAssetSettings.OnModificationGlobal -= SimplifyAddressablesNames;
            AddressableAssetSettings.OnModificationGlobal -= AutoAddConstants;

            if (autoSimplifyAddressableNames)
            {
                AddressableAssetSettings.OnModificationGlobal += SimplifyAddressablesNames;
            }

            if (autoAddConstants)
            {
                AddressableAssetSettings.OnModificationGlobal += AutoAddConstants;
            }
        }


        private static void AutoAddConstants(AddressableAssetSettings addressableAssetSettings,
            AddressableAssetSettings.ModificationEvent modificationEvent, object addedEntries)
        {
            if (modificationEvent is AddressableAssetSettings.ModificationEvent.EntryAdded
                or AddressableAssetSettings.ModificationEvent.EntryCreated)
            {
                if (scriptSelectionType == ScriptSelectionType.General)
                {
                    GetAllGroups();
                    GetAvailableClassesNames(generalSelectedScript);
                    CreateConstants();
                    SyncConstants();
                }
                else if (scriptSelectionType == ScriptSelectionType.ByAddressableType)
                {
                    GetAllGroups();
                    GetAvailableClassesNames(prefabsSelectedScript);
                    GetAvailableClassesNames(scriptableObjectsSelectedScript);
                    CreateConstants();
                    SyncConstants();
                }
            }
        }



        private static void SimplifyAddressablesNames(AddressableAssetSettings addressableAssetSettings,
            AddressableAssetSettings.ModificationEvent modificationEvent, object addedEntries)
        {
            if (modificationEvent is AddressableAssetSettings.ModificationEvent.EntryAdded
                or AddressableAssetSettings.ModificationEvent.EntryCreated)
            {
                List<AddressableAssetEntry> entries = addedEntries as List<AddressableAssetEntry>;

                if (entries != null)
                {
                    foreach (AddressableAssetEntry entry in entries)
                    {
                        string simplifiedName = entry.MainAsset.name;
                        entry.SetAddress(simplifiedName);
                    }
                }
            }
        }



        public static List<AddressableAssetGroup> GetAllGroups()
        {
            groups = new List<AddressableAssetGroup>();

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings.groups.Count < 1)
            {
                logMessages.Add("There should be at least one group in the Addressable Assets settings".AddCurrentTime()
                    .ToRed());
                return null;
            }

            for (int i = 1; i < settings.groups.Count; i++)
            {
                AddressableAssetGroup group = settings.groups[i];
                groups.Add(group);
            }

            return groups;
        }


        public static void SyncConstants()
        {
            if (generatedConstants.Count == 0)
            {
                logMessages.Add(
                    "No constants generated. Please check if there are addressable assets with valid addresses."
                        .AddCurrentTime().ToRed());
                return;
            }

            if (scriptSelectionType == ScriptSelectionType.General)
            {
                if (generalSelectedScript == null)
                {
                    logMessages.Add("Script is not selected".AddCurrentTime().ToRed());
                    return;
                }

                if (generalSelectedClass == null)
                {
                    logMessages.Add("Class is not selected".AddCurrentTime().ToRed());
                    return;
                }

                string scriptPath = AssetDatabase.GetAssetPath(generalSelectedScript);

                if (string.IsNullOrEmpty(scriptPath))
                {
                    logMessages.Add("Could not find script file path".AddCurrentTime().ToRed());
                    return;
                }

                string scriptContent = File.ReadAllText(scriptPath);

                StringBuilder constantsBlockBuilder = new StringBuilder();

                int constantsAdded = 0;

                foreach (List<string> constants in generatedConstants.Values)
                {
                    foreach (string constant in constants)
                    {
                        string constantPattern = $"{constant}";

                        if (Regex.IsMatch(scriptContent, constantPattern))
                        {
                            logMessages.Add($"Constant '{constant}' already exists in the script".AddCurrentTime()
                                .ToYellow());
                            continue;
                        }

                        constantsAdded++;
                        constantsBlockBuilder.AppendLine("        " + constant);
                    }
                }


                string className = generalSelectedClass.Name;

                string pattern = $@"(class\s+{Regex.Escape(className)}\s*{{)";

                if (Regex.IsMatch(scriptContent, pattern))
                {
                    string updatedContent = Regex.Replace(
                        scriptContent,
                        pattern,
                        "$1\n" + constantsBlockBuilder
                    );

                    File.WriteAllText(scriptPath, updatedContent);

                    logMessages.Add($"Successfully added {constantsAdded} constants to {className}".AddCurrentTime()
                        .ToGreen());
                }
                else
                {
                    logMessages.Add($"Could not find class '{className}' in the script".AddCurrentTime().ToRed());
                }
            }
            else if (scriptSelectionType == ScriptSelectionType.ByAddressableType)
            {
                if (prefabsSelectedScript == null)
                {
                    logMessages.Add("Prefab script is not selected".AddCurrentTime().ToRed());
                    return;
                }

                if (scriptableObjectsSelectedScript == null)
                {
                    logMessages.Add("Scriptable Object script is not selected".AddCurrentTime().ToRed());
                    return;
                }

                if (prefabsSelectedClass == null)
                {
                    logMessages.Add("Prefab class is not selected or could not be found".AddCurrentTime().ToRed());
                    return;
                }

                if (scriptableObjectsSelectedClass == null)
                {
                    logMessages.Add("Scriptable Object class is not selected or could not be found".AddCurrentTime()
                        .ToRed());
                    return;
                }

                string prefabsScriptsPath = AssetDatabase.GetAssetPath(prefabsSelectedScript);
                string scriptableObjectsScriptsPath = AssetDatabase.GetAssetPath(scriptableObjectsSelectedScript);

                if (string.IsNullOrEmpty(prefabsScriptsPath))
                {
                    logMessages.Add("Could not find script prefabs file path".AddCurrentTime().ToRed());
                    return;
                }

                if (string.IsNullOrEmpty(scriptableObjectsScriptsPath))
                {
                    logMessages.Add("Could not find scriptable objects file path".AddCurrentTime().ToRed());
                    return;
                }

                bool sameFile = prefabsScriptsPath == scriptableObjectsScriptsPath;

                if (sameFile)
                {
                    HandleSameScriptFile(prefabsScriptsPath);
                }
                else
                {
                    HandleSeparateScriptFiles(prefabsScriptsPath, scriptableObjectsScriptsPath);
                }
            }
        }


        private static void HandleSameScriptFile(string scriptPath)
        {
            string scriptContent = File.ReadAllText(scriptPath);
            string updatedContent = scriptContent;

            if (generatedConstants.ContainsKey(AddressableAssetType.Prefab) &&
                generatedConstants[AddressableAssetType.Prefab].Count > 0)
            {
                StringBuilder prefabConstantsBuilder = new StringBuilder();
                int prefabConstantsAdded = 0;

                foreach (string constant in generatedConstants[AddressableAssetType.Prefab])
                {
                    if (Regex.IsMatch(scriptContent, Regex.Escape(constant)))
                    {
                        logMessages.Add($"Prefab constant '{constant}' already exists in the script".AddCurrentTime()
                            .ToYellow());
                        continue;
                    }

                    prefabConstantsAdded++;
                    prefabConstantsBuilder.AppendLine("        " + constant);
                }

                if (prefabConstantsAdded > 0)
                {
                    string prefabClassName = prefabsSelectedClass.Name;
                    string prefabPattern = $@"(class\s+{Regex.Escape(prefabClassName)}\s*{{)";

                    if (Regex.IsMatch(updatedContent, prefabPattern))
                    {
                        updatedContent = Regex.Replace(
                            updatedContent,
                            prefabPattern,
                            "$1\n" + prefabConstantsBuilder
                        );
                        logMessages.Add(
                            $"Successfully prepared {prefabConstantsAdded} prefab constants for {prefabClassName}"
                                .AddCurrentTime().ToGreen());
                    }
                    else
                    {
                        logMessages.Add($"Could not find prefab class '{prefabClassName}' in the script"
                            .AddCurrentTime()
                            .ToRed());
                    }
                }
            }


            if (generatedConstants.ContainsKey(AddressableAssetType.ScriptableObject) &&
                generatedConstants[AddressableAssetType.ScriptableObject].Count > 0)
            {
                StringBuilder soConstantsBuilder = new StringBuilder();
                int soConstantsAdded = 0;

                foreach (string constant in generatedConstants[AddressableAssetType.ScriptableObject])
                {
                    if (Regex.IsMatch(updatedContent, Regex.Escape(constant)))
                    {
                        logMessages.Add(
                            $"ScriptableObject constant '{constant}' already exists in the script".AddCurrentTime()
                                .ToYellow());
                        continue;
                    }

                    soConstantsAdded++;
                    soConstantsBuilder.AppendLine("        " + constant);
                }

                if (soConstantsAdded > 0)
                {
                    string soClassName = scriptableObjectsSelectedClass.Name;
                    string soPattern = $@"(class\s+{Regex.Escape(soClassName)}\s*{{)";

                    if (Regex.IsMatch(updatedContent, soPattern))
                    {
                        updatedContent = Regex.Replace(
                            updatedContent,
                            soPattern,
                            "$1\n" + soConstantsBuilder
                        );
                        logMessages.Add(
                            $"Successfully prepared {soConstantsAdded} ScriptableObject constants for {soClassName}"
                                .AddCurrentTime().ToGreen());
                    }
                    else
                    {
                        logMessages.Add($"Could not find ScriptableObject class '{soClassName}' in the script"
                            .AddCurrentTime().ToRed());
                    }
                }
            }


            File.WriteAllText(scriptPath, updatedContent);
            logMessages.Add($"Successfully updated script file: {scriptPath}".AddCurrentTime().ToGreen());
        }


        private static void HandleSeparateScriptFiles(string prefabsScriptPath, string scriptableObjectsScriptPath)
        {
            string prefabsScriptContent = File.ReadAllText(prefabsScriptPath);
            StringBuilder constantsBlockBuilder = new StringBuilder();
            int constantsAdded = 0;

            foreach (string constant in generatedConstants[AddressableAssetType.Prefab])
            {
                if (Regex.IsMatch(prefabsScriptContent, Regex.Escape(constant)))
                {
                    logMessages.Add($"Constant '{constant}' already exists in the script".AddCurrentTime().ToYellow());
                    continue;
                }

                constantsAdded++;
                constantsBlockBuilder.AppendLine("        " + constant);
            }

            if (constantsAdded > 0)
            {
                string className = prefabsSelectedClass.Name;
                string pattern = $@"(class\s+{Regex.Escape(className)}\s*{{)";

                if (Regex.IsMatch(prefabsScriptContent, pattern))
                {
                    string updatedContent = Regex.Replace(
                        prefabsScriptContent,
                        pattern,
                        "$1\n" + constantsBlockBuilder
                    );

                    File.WriteAllText(prefabsScriptPath, updatedContent);
                    logMessages.Add($"Successfully added {constantsAdded} constants to {className}".AddCurrentTime()
                        .ToGreen());
                }
                else
                {
                    logMessages.Add($"Could not find class '{className}' in the script".AddCurrentTime().ToRed());
                }
            }


            string scriptableObjectsScriptContent = File.ReadAllText(scriptableObjectsScriptPath);
            StringBuilder soConstantsBlockBuilder = new StringBuilder();
            int soConstantsAdded = 0;

            foreach (string constant in generatedConstants[AddressableAssetType.ScriptableObject])
            {
                if (Regex.IsMatch(scriptableObjectsScriptContent, Regex.Escape(constant)))
                {
                    logMessages.Add(
                        $"Constant '{constant}' already exists in the ScriptableObjects script".AddCurrentTime()
                            .ToYellow());
                    continue;
                }

                soConstantsAdded++;
                soConstantsBlockBuilder.AppendLine("        " + constant);
            }

            if (soConstantsAdded > 0)
            {
                string soClassName = scriptableObjectsSelectedClass.Name;
                string soPattern = $@"(class\s+{Regex.Escape(soClassName)}\s*{{)";

                if (Regex.IsMatch(scriptableObjectsScriptContent, soPattern))
                {
                    string soUpdatedContent = Regex.Replace(
                        scriptableObjectsScriptContent,
                        soPattern,
                        "$1\n" + soConstantsBlockBuilder
                    );

                    File.WriteAllText(scriptableObjectsScriptPath, soUpdatedContent);
                    logMessages.Add(
                        $"Successfully added {soConstantsAdded} constants to {soClassName}".AddCurrentTime());
                }
                else
                {
                    logMessages.Add($"Could not find class '{soClassName}' in the ScriptableObjects script"
                        .AddCurrentTime().ToRed());
                }
            }
        }


        public static void CreateConstants()
        {
            generatedConstants.Clear();

            if (groups.Count == 0)
            {
                logMessages.Add("There are no groups found. Please add addressable assets to create constants."
                    .AddCurrentTime().ToRed());
                return;
            }

            foreach (AddressableAssetGroup group in groups)
            {
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    string address = entry.address;

                    AddressableAssetType addressableAssetType = AddressableAssetType.Prefab;

                    if (entry.MainAsset is GameObject)
                    {
                        addressableAssetType = AddressableAssetType.Prefab;
                    }
                    else if (entry.MainAsset is ScriptableObject)
                    {
                        addressableAssetType = AddressableAssetType.ScriptableObject;
                    }
                    else
                    {
                        addressableAssetType = AddressableAssetType.Prefab;
                    }

                    string constantName = ToUpperSnakeCase(entry.address);
                    string constantVariable = $"public const string {constantName} = \"{address}\";";

                    if (!generatedConstants.ContainsKey(addressableAssetType))
                    {
                        generatedConstants[addressableAssetType] = new List<string>();
                    }

                    generatedConstants[addressableAssetType].Add(constantVariable);
                }
            }
        }


        public static string[] GetAvailableClassesNames(MonoScript selectedScript)
        {
            if (selectedScript == null)
            {
                logMessages.Add("Selected script is null".AddCurrentTime().ToRed());
                return null;
            }

            Type scriptType = selectedScript.GetClass();

            if (scriptType == null)
            {
                logMessages.Add("Could not find class type".AddCurrentTime().ToRed());
                return null;
            }

            List<Type> typesInScript = new List<Type>();
            typesInScript.Add(scriptType);

            Assembly assembly = scriptType.Assembly;
            Type[] allTypes = assembly.GetTypes();

            foreach (Type type in allTypes)
            {
                if (type.DeclaringType == scriptType && type.IsClass)
                {
                    typesInScript.Add(type);
                }
            }

            if (selectedScript == generalSelectedScript)
            {
                generalAvailableClasses = typesInScript.ToArray();

                if (generalSelectedClassIndex < generalAvailableClasses.Length)
                {
                    generalSelectedClass = generalAvailableClasses[generalSelectedClassIndex];
                }
            }

            if (selectedScript == prefabsSelectedScript)
            {
                prefabsAvailableClasses = typesInScript.ToArray();

                if (prefabsSelectedClassIndex < prefabsAvailableClasses.Length)
                {
                    prefabsSelectedClass = prefabsAvailableClasses[prefabsSelectedClassIndex];
                }
            }

            if (selectedScript == scriptableObjectsSelectedScript)
            {
                scriptableObjectsAvailableClasses = typesInScript.ToArray();

                if (scriptableObjectsSelectedClassIndex < scriptableObjectsAvailableClasses.Length)
                {
                    scriptableObjectsSelectedClass =
                        scriptableObjectsAvailableClasses[scriptableObjectsSelectedClassIndex];
                }
            }

            string[] availableClassNames = new string[typesInScript.Count];

            for (int i = 0; i < typesInScript.Count; i++)
            {
                availableClassNames[i] = typesInScript[i].Name;
            }

            return availableClassNames;
        }


        private static string ToUpperSnakeCase(string originalString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(char.ToUpper(originalString[0]));

            for (int i = 1; i < originalString.Length; i++)
            {
                char currentChar = originalString[i];
                char previousChar = originalString[i - 1];

                if (char.IsLower(previousChar) && char.IsUpper(currentChar))
                {
                    stringBuilder.Append('_');
                }

                stringBuilder.Append(char.ToUpper(currentChar));
            }

            return stringBuilder.ToString();
        }
    }
}
