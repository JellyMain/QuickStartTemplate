using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace Editor
{
    public class FactoryWarmUpGenerator : EditorWindow
    {
        private List<FactoryInfo> factoryInfos = new List<FactoryInfo>();
        private Vector2 scrollPosition;
        private bool hasAnalyzed;
        private DefaultAsset searchDirectoryAsset;
        private string searchDirectory = "Assets/Scripts";




        [MenuItem("Tools/Factory Warm Up Generator")]
        public static void ShowWindow()
        {
            GetWindow<FactoryWarmUpGenerator>("Factory Warm Up Generator");
        }


        private void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Select Search Directory", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(searchDirectory, EditorStyles.textField, GUILayout.ExpandWidth(true));

            searchDirectoryAsset = EditorGUILayout.ObjectField(
                searchDirectoryAsset, typeof(DefaultAsset), false, GUILayout.Width(100)) as DefaultAsset;

            if (searchDirectoryAsset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(searchDirectoryAsset);

                if (assetPath != "" && Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), assetPath)))
                {
                    searchDirectory = assetPath;
                }
            }

            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space();

            if (GUILayout.Button("Find all factories"))
            {
                if (!hasAnalyzed || factoryInfos.Count == 0)
                {
                    AnalyzeFactories();
                    hasAnalyzed = true;
                }
                else if (EditorUtility.DisplayDialog("Confirm",
                             "Factories have already been analyzed. Do you want to refresh the analysis?",
                             "Yes", "No"))
                {
                    AnalyzeFactories();
                }
            }


            EditorGUILayout.Space();

            if (factoryInfos.Count > 0)
            {
                if (GUILayout.Button("Generate WarmUpPrefabs methods"))
                {
                    foreach (FactoryInfo factoryInfo in factoryInfos)
                    {
                        ModifyFactory(factoryInfo);
                    }
                }

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                foreach (var factoryInfo in factoryInfos)
                {
                    EditorGUILayout.LabelField(factoryInfo.factoryName, EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    EditorGUILayout.LabelField("File path: " + factoryInfo.factoryPath);
                    EditorGUILayout.LabelField("Prefab addresses found:");

                    EditorGUI.indentLevel++;

                    foreach (string address in factoryInfo.prefabPaths)
                    {
                        EditorGUILayout.LabelField("• " + address);
                    }

                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("No factories found", EditorStyles.boldLabel);
            }

            EditorGUILayout.Space();
        }


        private void ModifyFactory(FactoryInfo factoryInfo)
        {
            string path = factoryInfo.factoryPath;
            string scriptContent = File.ReadAllText(path);
            List<string> prefabPaths = factoryInfo.prefabPaths;
            string modifiedFactory = ReplaceWarmUpPrefabsMethod(scriptContent, prefabPaths);
            File.WriteAllText(path, modifiedFactory);
        }


        private void AnalyzeFactories()
        {
            factoryInfos = new List<FactoryInfo>();

            string[] allScriptsGUIDs = AssetDatabase.FindAssets("t:Script", new[] { searchDirectory });

            foreach (string guid in allScriptsGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.EndsWith(".cs") && !path.Contains("Editor"))
                {
                    string scriptContent = File.ReadAllText(path);

                    if (IsFactoryClass(scriptContent))
                    {
                        string className = ExtractClassName(scriptContent);
                        List<string> prefabPaths = ExtractPrefabPaths(scriptContent);


                        FactoryInfo factoryInfo = new FactoryInfo
                        {
                            prefabPaths = prefabPaths,
                            factoryName = className,
                            factoryPath = path
                        };

                        factoryInfos.Add(factoryInfo);
                    }
                }
            }
        }


        private bool IsFactoryClass(string scriptContent)
        {
            return Regex.IsMatch(scriptContent, @"class\s*\w+\s*:\s*BaseFactory");
        }


        private List<string> ExtractPrefabPaths(string scriptContent)
        {
            List<string> prefabPaths = new List<string>();

            MatchCollection matchCollection = Regex.Matches(scriptContent,
                @"InstantiatePrefab(?:WithComponent\<\w+\>)?\s*\(\s*(RuntimeConstants\.PrefabAddresses\.\w+)\s*\)",
                RegexOptions.Compiled);


            for (int i = 0; i < matchCollection.Count; i++)
            {
                prefabPaths.Add(matchCollection[i].Groups[1].Value);
            }

            return prefabPaths;
        }


        private string ExtractClassName(string scriptContent)
        {
            Match match = Regex.Match(scriptContent, @"class\s*(\w+)\s*:\s*BaseFactory", RegexOptions.Compiled);

            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }

            return "UnknownFactory";
        }


        private string ReplaceWarmUpPrefabsMethod(string scriptContent, List<string> prefabPaths)
        {
            string newMethodContent = GenerateWarmUpPrefabsMethod(prefabPaths);

            return Regex.Replace(scriptContent, @"protected\s+override\s+void\s+WarmUpPrefabs\s*\(\s*\)\s*\{[^\}]*\}",
                newMethodContent);
        }


        private string GenerateWarmUpPrefabsMethod(List<string> prefabPaths)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("protected override void WarmUpPrefabs()");
            stringBuilder.Append("\n        {");

            foreach (string path in prefabPaths)
            {
                stringBuilder.AppendLine($"\n            WarmUpPrefab({path});");
            }

            stringBuilder.AppendLine("\n        }");

            return stringBuilder.ToString();
        }
    }
}
