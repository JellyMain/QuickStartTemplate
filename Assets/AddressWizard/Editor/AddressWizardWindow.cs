using System.Collections.Generic;
using AddressWizard.Data;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;


namespace AddressWizard.Editor
{
    public class AddressWizardWindow : EditorWindow
    {
        private List<AddressableAssetGroup> groups = new List<AddressableAssetGroup>();
        private bool autoSimplifyAddressableNames;
        private bool autoAddConstants;
        private AddressWizardData addressWizardData;
        private ScriptSelectionType scriptSelectionType;
        private readonly AddressableTypeWindowData generalAddressableTypeWindowData = new AddressableTypeWindowData();
        private readonly AddressableTypeWindowData prefabsAddressableTypeWindowData = new AddressableTypeWindowData();
        private readonly AddressableTypeWindowData soAddressableTypeWindowData = new AddressableTypeWindowData();
        private Vector2 groupsScrollPosition = Vector2.zero;
        private Vector2 logScrollPosition = Vector2.zero;
        private int lastLogCount;

        private static readonly Color HeaderColor = new Color(0.2f, 0.4f, 0.8f, 1f);

        private GUIStyle headerStyle;
        private GUIStyle sectionStyle;
        private GUIStyle buttonStyle;
        private GUIStyle richTextStyle;
        private GUIStyle logStyle;
        private GUIStyle introStyle;



        private void ResetValues()
        {
            groups.Clear();
        }


        private void InitializeStyles()
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 16;
            headerStyle.normal.textColor = HeaderColor;

            sectionStyle = new GUIStyle(EditorStyles.helpBox);
            sectionStyle.padding = new RectOffset(15, 15, 10, 10);
            sectionStyle.margin = new RectOffset(5, 5, 5, 10);

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 12;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.padding = new RectOffset(20, 20, 8, 8);

            richTextStyle = new GUIStyle(EditorStyles.label);
            richTextStyle.richText = true;
            richTextStyle.wordWrap = true;

            logStyle = new GUIStyle(EditorStyles.label);
            logStyle.fontSize = 16;

            introStyle = new GUIStyle(EditorStyles.helpBox);
            introStyle.padding = new RectOffset(15, 15, 15, 15);
            introStyle.margin = new RectOffset(5, 5, 10, 15);
            introStyle.fontSize = 12;
            introStyle.wordWrap = true;
            introStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
        }



        private void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += ResetValues;
            UpdateData();
        }


        private void OnDisable()
        {
            SaveData();
            AddressWizardCore.ClearLogMessages();
        }


        [MenuItem("Tools/Address Wizard")]
        public static void ShowWindow()
        {
            GetWindow<AddressWizardWindow>("Address Wizard");
        }


        private void OnGUI()
        {
            if (headerStyle == null)
            {
                InitializeStyles();
            }

            DrawWindowHeader();

            EditorGUILayout.Space(20);

            DrawIntroductionSection();
            EditorGUILayout.Space(15);

            DrawSettingsSection();
            EditorGUILayout.Space(15);

            DrawScriptSelectionSection();
            EditorGUILayout.Space(15);

            DrawAddressableGroupsSection();

            EditorGUILayout.Space(15);
            DrawLog();
        }



        private void DrawWindowHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUIStyle titleStyle = new GUIStyle(EditorStyles.largeLabel);
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.normal.textColor = HeaderColor;
            titleStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField("Address Wizard", titleStyle, GUILayout.Height(30));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            Rect rect = GUILayoutUtility.GetRect(1, 2);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        }



        private void DrawSettingsSection()
        {
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();
            autoSimplifyAddressableNames = EditorGUILayout.Toggle(
                new GUIContent("Auto Simplify Names",
                    "Automatically simplify newly added addressable asset names"),
                autoSimplifyAddressableNames);

            if (EditorGUI.EndChangeCheck())
            {
                addressWizardData.autoSimplifyAddressableNames = autoSimplifyAddressableNames;
                SaveData();
            }

            EditorGUI.BeginChangeCheck();

            autoAddConstants = EditorGUILayout.Toggle(
                new GUIContent("Auto Add Constants",
                    "Automatically generate constants for newly added addressable assets"),
                autoAddConstants);

            if (EditorGUI.EndChangeCheck())
            {
                addressWizardData.autoAddConstants = autoAddConstants;
                SaveData();
            }

            EditorGUILayout.EndVertical();
        }


        private void DrawIntroductionSection()
        {
            EditorGUILayout.BeginVertical(introStyle);

            GUIStyle welcomeStyle = new GUIStyle(EditorStyles.boldLabel);
            welcomeStyle.fontSize = 14;
            welcomeStyle.normal.textColor = HeaderColor;
            EditorGUILayout.LabelField("Welcome to Address Wizard!", welcomeStyle);

            EditorGUILayout.Space(5);

            GUIStyle descriptionStyle = new GUIStyle(EditorStyles.label);
            descriptionStyle.wordWrap = true;
            descriptionStyle.fontSize = 11;

            EditorGUILayout.LabelField(
                "Address Wizard helps you manage Unity Addressables by automatically generating constants for your addressable assets. " +
                "This eliminates typos and saves you a little bit of time :)",
                descriptionStyle);

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("Quick Start Guide:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("1. Configure your settings below",
                descriptionStyle);
            EditorGUILayout.LabelField("2. Select your target script where constants will be generated",
                descriptionStyle);
            EditorGUILayout.LabelField("3. Click 'Get Addressable Groups' to scan your project", descriptionStyle);
            EditorGUILayout.LabelField("4. Click 'Sync Addressables' to generate constants in your selected script",
                descriptionStyle);
            EditorGUILayout.LabelField("You can open Address Wizard window by clicking Tools -> Address Wizard",
                descriptionStyle);

            EditorGUILayout.EndVertical();
        }



        private void DrawScriptSelectionSection()
        {
            EditorGUILayout.LabelField("Script Selection", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();
            scriptSelectionType = (ScriptSelectionType)EditorGUILayout.EnumPopup(
                new GUIContent("Script Selection Type",
                    "Choose how to select target scripts: General (single script for all types) or By Addressable Type (separate scripts for prefabs and scriptable objects)"),
                scriptSelectionType);

            if (EditorGUI.EndChangeCheck())
            {
                addressWizardData.scriptSelectionType = scriptSelectionType;
                SaveData();
            }

            if (scriptSelectionType == ScriptSelectionType.General)
            {
                EditorGUI.BeginChangeCheck();
                generalAddressableTypeWindowData.selectedScript = EditorGUILayout.ObjectField(
                    "Target Script",
                    generalAddressableTypeWindowData.selectedScript,
                    typeof(MonoScript),
                    false) as MonoScript;

                if (EditorGUI.EndChangeCheck())
                {
                    addressWizardData.generalAddressableTypeData.selectedScriptPath =
                        AssetDatabase.GetAssetPath(generalAddressableTypeWindowData.selectedScript);
                    generalAddressableTypeWindowData.availableClassNames =
                        AddressWizardCore.GetAvailableClassesNames(generalAddressableTypeWindowData.selectedScript);
                    SaveData();
                }

                ShowGeneralFoundClasses();
            }

            if (scriptSelectionType == ScriptSelectionType.ByAddressableType)
            {
                DrawScriptSelectionByAddressableTypeSection();
            }

            EditorGUILayout.EndVertical();
        }


        private void DrawAddressableGroupsSection()
        {
            EditorGUILayout.LabelField("Addressable Groups", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (GUILayout.Button("Get Addressable Groups"))
            {
                groups = AddressWizardCore.GetAllGroups();
            }

            if (groups.Count > 0)
            {
                ShowFoundGroups();

                if (GUILayout.Button("Sync Addressables"))
                {
                    AddressWizardCore.CreateConstants();
                    AddressWizardCore.SyncConstants();
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "No addressable groups were found. Click 'Get Addressable Groups' to retrieve them.",
                    MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }


        private void SaveData()
        {
            AddressWizardSaver.SaveData(addressWizardData);
            AddressWizardCore.UpdateData();
        }


        private void UpdateData()
        {
            addressWizardData = AddressWizardSaver.GetSavedData();
            autoSimplifyAddressableNames = addressWizardData.autoSimplifyAddressableNames;
            autoAddConstants = addressWizardData.autoAddConstants;
            scriptSelectionType = addressWizardData.scriptSelectionType;
            generalAddressableTypeWindowData.selectedScript =
                AssetDatabase.LoadAssetAtPath<MonoScript>(addressWizardData.generalAddressableTypeData
                    .selectedScriptPath);

            prefabsAddressableTypeWindowData.selectedScript =
                AssetDatabase.LoadAssetAtPath<MonoScript>(addressWizardData.prefabsAddressableTypeData
                    .selectedScriptPath);


            soAddressableTypeWindowData.selectedScript =
                AssetDatabase.LoadAssetAtPath<MonoScript>(addressWizardData.soAddressableTypeData
                    .selectedScriptPath);


            if (generalAddressableTypeWindowData.selectedScript != null)
            {
                generalAddressableTypeWindowData.availableClassNames =
                    AddressWizardCore.GetAvailableClassesNames(generalAddressableTypeWindowData.selectedScript);

                generalAddressableTypeWindowData.selectedClassIndex =
                    addressWizardData.generalAddressableTypeData.selectedClassIndex;
            }

            if (prefabsAddressableTypeWindowData.selectedScript != null)
            {
                prefabsAddressableTypeWindowData.availableClassNames =
                    AddressWizardCore.GetAvailableClassesNames(prefabsAddressableTypeWindowData.selectedScript);
                prefabsAddressableTypeWindowData.selectedClassIndex =
                    addressWizardData.prefabsAddressableTypeData.selectedClassIndex;
            }

            if (soAddressableTypeWindowData.selectedScript != null)
            {
                soAddressableTypeWindowData.availableClassNames =
                    AddressWizardCore.GetAvailableClassesNames(soAddressableTypeWindowData.selectedScript);
                soAddressableTypeWindowData.selectedClassIndex =
                    addressWizardData.soAddressableTypeData.selectedClassIndex;
            }
        }


        private void ShowGeneralFoundClasses()
        {
            if (generalAddressableTypeWindowData.availableClassNames == null ||
                generalAddressableTypeWindowData.availableClassNames.Length == 0)
            {
                if (generalAddressableTypeWindowData.selectedScript != null)
                {
                    EditorGUILayout.HelpBox("No classes found in the selected script.", MessageType.Warning);
                }

                return;
            }

            EditorGUI.BeginChangeCheck();

            generalAddressableTypeWindowData.selectedClassIndex = EditorGUILayout.Popup(
                "Target Class",
                generalAddressableTypeWindowData.selectedClassIndex,
                generalAddressableTypeWindowData.availableClassNames);

            if (EditorGUI.EndChangeCheck())
            {
                addressWizardData.generalAddressableTypeData.selectedClassIndex =
                    generalAddressableTypeWindowData.selectedClassIndex;
                SaveData();
            }
        }


        private void DrawScriptSelectionByAddressableTypeSection()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            prefabsAddressableTypeWindowData.selectedScript = EditorGUILayout.ObjectField(
                "Target Prefabs Script",
                prefabsAddressableTypeWindowData.selectedScript,
                typeof(MonoScript),
                false) as MonoScript;

            if (prefabsAddressableTypeWindowData.selectedScript != null &&
                prefabsAddressableTypeWindowData.availableClassNames != null &&
                prefabsAddressableTypeWindowData.availableClassNames.Length > 0)
            {
                prefabsAddressableTypeWindowData.selectedClassIndex = EditorGUILayout.Popup(
                    prefabsAddressableTypeWindowData.selectedClassIndex,
                    prefabsAddressableTypeWindowData.availableClassNames,
                    GUILayout.Width(150));
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            soAddressableTypeWindowData.selectedScript = EditorGUILayout.ObjectField(
                "Target Scriptable Objects Script",
                soAddressableTypeWindowData.selectedScript,
                typeof(MonoScript),
                false) as MonoScript;

            if (soAddressableTypeWindowData.selectedScript != null &&
                soAddressableTypeWindowData.availableClassNames != null &&
                soAddressableTypeWindowData.availableClassNames.Length > 0)
            {
                soAddressableTypeWindowData.selectedClassIndex = EditorGUILayout.Popup(
                    soAddressableTypeWindowData.selectedClassIndex,
                    soAddressableTypeWindowData.availableClassNames,
                    GUILayout.Width(150));
            }

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                if (prefabsAddressableTypeWindowData.selectedScript != null)
                {
                    prefabsAddressableTypeWindowData.availableClassNames =
                        AddressWizardCore.GetAvailableClassesNames(prefabsAddressableTypeWindowData.selectedScript);
                    addressWizardData.prefabsAddressableTypeData.selectedScriptPath =
                        AssetDatabase.GetAssetPath(prefabsAddressableTypeWindowData.selectedScript);
                    addressWizardData.prefabsAddressableTypeData.selectedClassIndex =
                        prefabsAddressableTypeWindowData.selectedClassIndex;
                }

                if (soAddressableTypeWindowData.selectedScript != null)
                {
                    soAddressableTypeWindowData.availableClassNames =
                        AddressWizardCore.GetAvailableClassesNames(soAddressableTypeWindowData.selectedScript);
                    addressWizardData.soAddressableTypeData.selectedScriptPath =
                        AssetDatabase.GetAssetPath(soAddressableTypeWindowData.selectedScript);
                    addressWizardData.soAddressableTypeData.selectedClassIndex =
                        soAddressableTypeWindowData.selectedClassIndex;
                }

                SaveData();
            }
        }


        private void ShowFoundGroups()
        {
            EditorGUILayout.LabelField("Found Addressable Groups:", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);


            groupsScrollPosition = EditorGUILayout.BeginScrollView(groupsScrollPosition, GUILayout.Height(300));

            foreach (var group in groups)
            {
                EditorGUILayout.LabelField($"• {group.Name}:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                foreach (AddressableAssetEntry entry in group.entries)
                {
                    EditorGUILayout.LabelField($"- {entry.address}");
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space(10);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }


        private void DrawLog()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Log:", logStyle);

            richTextStyle.richText = true;

            List<string> logMessages = AddressWizardCore.GetLogMessages();

            if (logMessages.Count > lastLogCount)
            {
                logScrollPosition.y = float.MaxValue;
                lastLogCount = logMessages.Count;
            }

            logScrollPosition = EditorGUILayout.BeginScrollView(logScrollPosition, GUILayout.ExpandHeight(true));

            foreach (string log in logMessages)
            {
                EditorGUILayout.LabelField(log, richTextStyle);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}
