//#define CodeGuardCustomProfiles

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class CodeGuardWindow : EditorWindow
{

    private static string _projectName
    {
        get { return Application.dataPath; }
    }

    #region Window

    private static CodeGuardWindow _window;

    [MenuItem("Window/CodeGuard/Settings", false, 0)]
    private static void Init()
    {
        _window = (CodeGuardWindow)EditorWindow.GetWindow(typeof(CodeGuardWindow));
    }

    public CodeGuardWindow()
    {
        // Load Custom Profiles
    }

    private List<GUIContent> _guiProfiles
    {
        get
        {
            List<GUIContent> result = new List<GUIContent>();
            foreach (var profile in CodeGuard.profiles)
            {
                result.Add(new GUIContent(profile.Name));
            }
            result.Add(new GUIContent("Custom"));
            return result;
        }
    }

    private static int _guiProfileIndex
    {
        get { return CodeGuard.GetProfile(); }
    }

    private readonly GUIContent[] _assembliesSetting =
        {
            new GUIContent("Script Assemblies"), new GUIContent("Automatically All"), new GUIContent("Custom")
        };

    private readonly GUIContent[] _symbolRenamingMode =
        {
            new GUIContent("Unreadable"), new GUIContent("Unreadable Lite"), new GUIContent("Latin")
        };

    private readonly GUIContent[] _RPCsAction =
        {
            new GUIContent("Skip"), new GUIContent("Proxy")//, new GUIContent("Obfuscate")
        };

    private readonly GUIContent[] _typeSelectionMode =
        {
            new GUIContent("All Except Skip Types"), new GUIContent("Only Selected"), new GUIContent("Combination")
        };


    private Vector2 _scrollPosition;
    private void OnGUI()
    {
        //if (_excludedMethods == null) LoadExcludedMethods();
        //if (_assembliesSettingList == null) LoadAssembliesSetting();
        //if (_skipTypes == null) LoadSkipTypes();

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        GUILayout.Label("CodeGuard Settings:", EditorStyles.boldLabel);
        GUILayout.Space(10f);

        int chosenProfileIndex = EditorGUILayout.Popup(new GUIContent("Profile"), _guiProfileIndex, _guiProfiles.ToArray());
        if (chosenProfileIndex != _guiProfileIndex)
        {
            switch (chosenProfileIndex)
            {
                case 0:
                    EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard WebPlayerProfile", false);
                    CodeGuardSetupProfile();
                    break;
                case 1:
                    EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", true);
                    EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard WebPlayerProfile", false);
                    CodeGuardSetupProfile();
                    break;
                case 2:
                    EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", true);
                    EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard WebPlayerProfile", false);
                    CodeGuardSetupProfile();
                    break;
                case 3:
                    EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", true);
                    EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard WebPlayerProfile", false);
                    CodeGuardSetupProfile();
                    break;
                case 4:
                    EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", true);
                    EditorPrefs.SetBool(_projectName + "CodeGuard WebPlayerProfile", false);
                    CodeGuardSetupProfile();
                    break;
                case 5:
                    EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard WebPlayerProfile", true);
                    CodeGuardSetupProfile();
                    break;
                default:
                    EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
                    EditorPrefs.SetBool(_projectName + "CodeGuard WebPlayerProfile", false);
                    break;
            }
        }
#if CodeGuardCustomProfiles
        if (_guiProfiles[chosenProfileIndex].text=="Custom")
        {
            if (GUILayout.Button("Save Profile as..."))
            {
                var w = new CodeGuardSaveProfileWindow();
                w.ShowUtility();
            }
        } else if (chosenProfileIndex>5)
        {
            if (GUILayout.Button("Save Profile")) // Should compare settings to see if they are different
            {
            }
            else if (GUILayout.Button("Delete Profile"))
            {
            }
        }
#endif
        GUILayout.Space(10f);

        BeginSection("Assemblies:");
        CodeGuard.AssembliesSetting =
            EditorGUILayout.Popup(
                new GUIContent("Protect:",
                               "Script Assemblies: Automatically adds the Unity Projects Script Assemblies." +
                               Environment.NewLine +
                               "Automatically All: Automatically adds all Assemblies used in the project." +
                               Environment.NewLine + "Custom: Specify exactly which Assemblies that should be added."),
                CodeGuard.AssembliesSetting, _assembliesSetting);

        if (CodeGuard.AssembliesSetting == 2)
        {
            CodeGuard.AutoAddScriptAssemblies = Toggle("Script Assemblies:",
                                             "If enabled, automatically adds the Unity Projects Script Assemblies.",
                                             CodeGuard.AutoAddScriptAssemblies);
            CodeGuard.AssembliesSettingCount = EditorGUILayout.IntField(new GUIContent("Count"), CodeGuard.AssembliesSettingCount);
            AssembliesSettingFields();
        }

        EndSection();

        BeginSection("Types:");
        CodeGuard.TypeSelectionMode =
            EditorGUILayout.Popup(
                new GUIContent("Protect:",
                               "All Except Skip Types: All Types (Components, MonoBehaviours) except those in Skip Types." +
                               Environment.NewLine +
                               "Selected Types: Only those in Select Types." +
                               Environment.NewLine + "Combination: Only those in Select Types that is not in Skip Types."),
                CodeGuard.TypeSelectionMode, _typeSelectionMode);

        if (CodeGuard.TypeSelectionMode>0)
        {
            EditorGUILayout.LabelField(new GUIContent("Select Types:",
                                                      "Protects and obfuscates these types. You can use * as a wildcard, for example if you select type: Test*this then the type Testorthis will be protected as well as TestNotthis. Case sensitive."));

            CodeGuard.SelectTypesCount = EditorGUILayout.IntField(new GUIContent("Count"), CodeGuard.SelectTypesCount);
            SelectTypesFields();
        }

        EndSection();

        BeginSection("Obfuscate:");
        CodeGuard.PrivateMembers = Toggle("Private Members", "If enabled, obfuscates all Private Members (Fields, Methods and Properties).",
                                CodeGuard.PrivateMembers);
        if (!CodeGuard.PrivateMembers)
        {
            CodeGuard.PrivateFieldsAndProperties = Toggle("Private Fields & Properties", "If enabled, obfuscates all Private Fields and Properties).",
                                    CodeGuard.PrivateFieldsAndProperties);
        }
        else
        {
            EditorGUI.BeginDisabledGroup(true);
            CodeGuard.PrivateFieldsAndProperties = Toggle("Private Fields & Properties", "If enabled, obfuscates all Private Fields and Properties.",
                                    true);
            EditorGUI.EndDisabledGroup();
        }
        CodeGuard.TypeFields = Toggle("Type Fields",
                            "If enabled, obfuscates Fields of all Types. Some Fields might be skipped to ensure the Unity Project still works.",
                            CodeGuard.TypeFields);
        CodeGuard.TypeFieldsAggressively = Toggle("Type Fields Aggressively",
                                        "If enabled, private, protected and public obfuscates Fields of all Types. This might break some Unity projects.",
                                        CodeGuard.TypeFieldsAggressively);

        CodeGuard.ObfuscateProperties = Toggle("Properties",
                            "If enabled, obfuscates private, protected and public Properties of all Types. Some Properties might be skipped to ensure the Unity Project still works.",
                            CodeGuard.ObfuscateProperties);

        CodeGuard.ObfuscateCustomMethods = Toggle("Custom Methods",
                                        "If enabled, obfuscates Custom Methods (ie non-Unity standard methods) of all Types. Some Methods might be skipped to ensure the Unity Project still works, sometimes this protection fails and this will result in a broken projects.",
                                        CodeGuard.ObfuscateCustomMethods);

        if (CodeGuard.ProxyUnityMethods || CodeGuard.ProxyExcludedMethods || CodeGuard.ProxyCustomMethods)
            CodeGuard.GuardProxyParameters = Toggle("Proxy Parameters", "If enabled, obfuscates the Parameters of Proxy Methods.",
                                          CodeGuard.GuardProxyParameters);
        CodeGuard.GuardMethodParameters = Toggle("Method Parameters",
                                       "If enabled, obfuscates the Parameters of all non Proxy Methods.",
                                       CodeGuard.GuardMethodParameters);
        CodeGuard.GuardAggressively = Toggle("Aggressively",
                                   "If enabled, obfuscates as much as possible. This might break projects which rely on SendMessage or similar calls.",
                                   CodeGuard.GuardAggressively);
        EndSection();


        BeginSection("Exclude Method Names:",
                "If you are using Custom Methods in SendMessage calls or similar, then you might need to add their names (short: OnDamage, or full: Enemy.OnDamage) here. You can use * as a wildcard, for example if you exclude method: Test*this then the method Testorthis will be skipped as well as TestNotthi. Case sensitive." +
                Environment.NewLine +
                " CodeGuard searches your code for SendMessage (or similar) calls and automatically excludes their names from obfuscation, but if you use String variables in your calls, like SendMessage(stringVar), CodeGuard might not find them. An alternative to this option is to use the System.Reflection.ObfuscationAttribute on those Methods instead.");
        CodeGuard.ExcludedMethodsCount = EditorGUILayout.IntField(new GUIContent("Count"), CodeGuard.ExcludedMethodsCount);
        ExcludedMethodsFields();
        EndSection();

        BeginSection("Proxy:");
        CodeGuard.ProxyUnityMethods = Toggle("Unity Methods",
                                   "If enabled, creates Proxy Methods of Unity Methods (such as Update() or OnGUI()).",
                                   CodeGuard.ProxyUnityMethods);

        CodeGuard.ProxyExcludedMethods = Toggle("Excluded Method Names",
                                      "If enabled, creates Proxy Methods of Excluded Methods (such as ones CodeGuard automatically finds and ones you've setup, like OnDamage(float damage) etc).",
                                      CodeGuard.ProxyExcludedMethods);

        CodeGuard.ProxyCustomMethods = Toggle("Custom Methods",
                                    "If enabled, creates Proxy Methods of Custom Methods (except Excluded Methods).",
                                    CodeGuard.ProxyCustomMethods);

        EndSection();


        BeginSection("RPCs:", "Works with: Unity networking, Photon, uLink and TNet RPCs/RFCs.");
        CodeGuard.RPCsAction =
            EditorGUILayout.Popup(
                new GUIContent("Action:",
                               "Skip: Skips obfsucating the name of RPC methods (parameters will be obfuscated if Obfuscate Method Parameters is enabled." +
                               Environment.NewLine + "Proxy: Proxies RPC methods (and removes the RPC Attribute from the original method)."),
                CodeGuard.RPCsAction, _RPCsAction);
        EndSection();

        BeginSection("Skip:");
        CodeGuard.SkipUnityTypesPublicFields = Toggle("Unity Types Public Fields",
                                        "Skips obfuscation of Unity Types (MonoBehaviours, Components etc) public Fields. In WebPlayer builds, if the Fields are set in the Inspector they usually cannot be obfuscated. An alternative to this option is to use the System.Reflection.ObfuscationAttribute on those Fields instead.",
                                        CodeGuard.SkipUnityTypesPublicFields);
        CodeGuard.SkipUnityTypesPublicStaticFields = Toggle("Unity Types Public Static Fields",
                                        "Skips obfuscation of Unity Types (MonoBehaviours, Components etc) public static Fields. An alternative to this option is to use the System.Reflection.ObfuscationAttribute on the Fields instead.",
                                        CodeGuard.SkipUnityTypesPublicStaticFields);

        CodeGuard.SkipFieldsWithSerializeFieldAttribute = Toggle("Fields With SerializeField Attribute",
                                        "Skips obfuscation of Fields with the SerializeField attribute.",
                                        CodeGuard.SkipFieldsWithSerializeFieldAttribute);

        if (CodeGuard.TypeSelectionMode != 1)
        {
            EditorGUILayout.LabelField(new GUIContent("Skip Types:",
                                                      "Skips obfuscation and protection of these types. You can use * as a wildcard, for example if you skip type: Test*this then the type Testorthis will be skipped as well as TestNotthis. Case sensitive."));

            CodeGuard.SkipTypesCount = EditorGUILayout.IntField(new GUIContent("Count"), CodeGuard.SkipTypesCount);
            SkipTypesFields();
        }

        EndSection();

        EndSection();

        BeginSection("Misc:");
        //if (buildTarget == BuildTarget.WebPlayer || buildTarget == BuildTarget.WebPlayerStreamed)
        {
            //int lastSymbolRenamingMode = CodeGuard.SymbolRenamingMode;
            CodeGuard.SymbolRenamingMode =
                EditorGUILayout.Popup(
                    new GUIContent("Symbol Renaming Mode:",
                                   "The new Web Player doesn't like certain unreadable characters used by CodeGuard. If Web Player builds freeze at start, select either Unreadable Lite or Latin." +
                                   Environment.NewLine + "Unreadable: Standard mode." +
                                   Environment.NewLine + "Unreadable Lite: Uses a smaller subset of unreadable characters." +
                                   Environment.NewLine + "Latin: Uses latin characters for obfuscation."),
                    CodeGuard.SymbolRenamingMode, _symbolRenamingMode);
            //if (lastSymbolRenamingMode == 0 && CodeGuard.SymbolRenamingMode > 0)
            //    CodeGuard.SkipUnityTypesPublicFields = true;
        }
        CodeGuard.PostBuildGuarding = Toggle("Post-Build Guarding",
                                   "Automatic obfuscation and protection via postprocess build actions. Available for Windows, Mac, Linux, WebPlayer, Android and iOS/iPhone builds.",
                                   CodeGuard.PostBuildGuarding);

        if (buildTarget == BuildTarget.WebPlayer || buildTarget == BuildTarget.WebPlayerStreamed ||
            buildTarget == BuildTarget.Android || buildTarget == BuildTarget.iOS)
        {
            EditorGUI.BeginDisabledGroup(true);
            Toggle("Create Backup",
                                  "Creates a backup before obfuscating and protecting. On Windows and Linux builds, backups the Data folder. On Mac OS X builds, backups the Application. WebPlayer, Android and iOS builds will not be backed up.",
                                  false);
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            CodeGuard.CreateBackup = Toggle("Create Backup",
                                  "Creates a backup before obfuscating and protecting. On Windows and Linux builds, backups the Data folder. On Mac OS X builds, backups the Application. WebPlayer, Android and iOS builds will not be backed up.",
                                  CodeGuard.CreateBackup);
        }
        EndSection();

        EditorGUILayout.EndScrollView();
    }

    private static void ExcludedMethodsFields()
    {
        List<string> excludedMethods = CodeGuard.ExcludedMethods;
        for (int i = 0; i < CodeGuard.ExcludedMethodsCount; i++)
        {
            excludedMethods[i] = EditorGUILayout.TextField("Excluded Method Name " + (i + 1).ToString() + ":",
                                                            excludedMethods[i]);
        }
        CodeGuard.SaveExcludedMethods();
    }

    private static void AssembliesSettingFields()
    {
        List<string> assembliesSettingList = CodeGuard.AssembliesSettingList;
        for (int i = 0; i < CodeGuard.AssembliesSettingCount; i++)
        {
            assembliesSettingList[i] = EditorGUILayout.TextField("Assembly " + (i + 1).ToString() + ":",
                                                                  assembliesSettingList[i]);
        }
        CodeGuard.SaveAssembliesSetting();
    }

    private static void SkipTypesFields()
    {
        List<string> skipTypes = CodeGuard.SkipTypes;
        for (int i = 0; i < CodeGuard.SkipTypesCount; i++)
        {
            skipTypes[i] = EditorGUILayout.TextField("Skip Type " + (i + 1).ToString() + ":", skipTypes[i]);
        }
        CodeGuard.SaveSkipTypes();
    }

    private static void SelectTypesFields()
    {
        List<string> selectTypes = CodeGuard.SelectTypes;
        for (int i = 0; i < CodeGuard.SelectTypesCount; i++)
        {
            selectTypes[i] = EditorGUILayout.TextField("Select Type " + (i + 1).ToString() + ":", selectTypes[i]);
        }
        CodeGuard.SaveSelectTypes();
    }


    public static bool Toggle(string name, string description, bool value)
    {
        return EditorGUILayout.Toggle(new GUIContent(name, description), value);
    }

    public static void BeginSection(string name)
    {
        BeginSection(name, "");
    }
    public static void BeginSection(string name, string description)
    {
        EditorGUILayout.LabelField(new GUIContent(name, description), EditorStyles.boldLabel);
        //GUILayout.Label(new GUIContent(name, description), EditorStyles.boldLabel);
        GUILayout.Space(2f);
    }
    public static void EndSection()
    {
        GUILayout.Space(10f);
    }
    #endregion

    #region MenuItems

    [MenuItem("Window/CodeGuard/Guard Unity Project Assemblies", false, 1000)]
    private static void CodeGuardFolder()
    {
        string assemblyPath;

        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            assemblyPath = EditorUtility.OpenFilePanel("Unity Project Application", Application.dataPath,
                                                       Path.GetDirectoryName(Application.dataPath));
        }
        else
            assemblyPath = EditorUtility.OpenFolderPanel("Unity Project Assemblies", Application.dataPath, "Managed");

        if (string.IsNullOrEmpty(assemblyPath) || !Directory.Exists(assemblyPath))
        {
            Debug.LogError("CodeGuard: Couldn't find Unity Project assemblies.");
            return;
        }

        string dirName = Path.GetFileName(assemblyPath);
        if (dirName != "Managed")
        {
            if (dirName.EndsWith("_Data"))
            {
                assemblyPath = assemblyPath + Path.DirectorySeparatorChar + "Managed";
            }
            else if (dirName.EndsWith(".app"))
            {
                assemblyPath += Path.DirectorySeparatorChar + "Contents" + Path.DirectorySeparatorChar + "Data" +
                                Path.DirectorySeparatorChar + "Managed";
            }
        }

        if (string.IsNullOrEmpty(assemblyPath) || !Directory.Exists(assemblyPath))
        {
            Debug.LogError("CodeGuard: Couldn't find Unity Project assemblies.");
            return;
        }

        CodeGuard.DoCodeGuardFolder(assemblyPath, true);
    }

    #region Profiles

    [MenuItem("Window/CodeGuard/Profiles/Low", true, 1)]
    private static bool ValidateLowProfile()
    {
        return !EditorPrefs.GetBool(_projectName + "CodeGuard LowProfile", false);
    }

    [MenuItem("Window/CodeGuard/Profiles/Low", false, 1)]
    private static void EnableLowProfile()
    {
        EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", true);
        EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
        CodeGuardSetupProfile();
        Debug.Log("CodeGuard: Changed to Low profile.");
    }

    [MenuItem("Window/CodeGuard/Profiles/Medium", true, 1)]
    private static bool ValidateMediumProfile()
    {
        return !EditorPrefs.GetBool(_projectName + "CodeGuard MediumProfile", false);
    }

    [MenuItem("Window/CodeGuard/Profiles/Medium", false, 1)]
    private static void EnableMediumProfile()
    {
        EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", true);
        EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
        CodeGuardSetupProfile();
        Debug.Log("CodeGuard: Changed to Medium profile.");
    }

    [MenuItem("Window/CodeGuard/Profiles/High", true, 1)]
    private static bool ValidateHighProfile()
    {
        return !EditorPrefs.GetBool(_projectName + "CodeGuard HighProfile", false);
    }

    [MenuItem("Window/CodeGuard/Profiles/High", false, 1)]
    private static void EnableHighProfile()
    {
        EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", true);
        EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
        CodeGuardSetupProfile();
        Debug.Log("CodeGuard: Changed to High profile.");
    }

    [MenuItem("Window/CodeGuard/Profiles/Aggressive", true, 1)]
    private static bool ValidateAggressiveProfile()
    {
        return !EditorPrefs.GetBool(_projectName + "CodeGuard AggressiveProfile", false);
    }

    [MenuItem("Window/CodeGuard/Profiles/Aggressive", false, 1)]
    private static void EnableAggressiveProfile()
    {
        EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", true);
        CodeGuardSetupProfile();
        Debug.Log("CodeGuard: Changed to Aggressive profile.");
    }

#if CodeGuard14
    [MenuItem("Window/CodeGuard/Profiles/WebPlayer, UnityPackage", true, 1)]
#else
    [MenuItem("Window/CodeGuard/Profiles/WebPlayer", true, 1)]
#endif
    private static bool ValidateWebPlayerProfile()
    {
        return !EditorPrefs.GetBool(_projectName + "CodeGuard WebPlayerProfile", false);
    }

#if CodeGuard14
    [MenuItem("Window/CodeGuard/Profiles/WebPlayer, UnityPackage", true, 1)]
#else
    [MenuItem("Window/CodeGuard/Profiles/WebPlayer", true, 1)]
#endif
    private static void EnableWebPlayerProfile()
    {
        EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard WebPlayerProfile", true);
        CodeGuardSetupProfile();
#if CodeGuard14
        Debug.Log("CodeGuard: Changed to WebPlayer/UnityPackage profile.");
#else
        Debug.Log("CodeGuard: Changed to WebPlayer profile.");
#endif
    }


    private static void DisableProfiles()
    {
        EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard WebPlayerProfile", false);
    }

    private static void CodeGuardSetupProfile()
    {
        bool lowProfile = EditorPrefs.GetBool(_projectName + "CodeGuard LowProfile", false);
        bool mediumProfile = EditorPrefs.GetBool(_projectName + "CodeGuard MediumProfile", false);
        bool highProfile = EditorPrefs.GetBool(_projectName + "CodeGuard HighProfile", false);
        bool aggressiveProfile = EditorPrefs.GetBool(_projectName + "CodeGuard AggressiveProfile", false);
        bool unityPackageProfile = EditorPrefs.GetBool(_projectName + "CodeGuard WebPlayerProfile", false);

        if (lowProfile || mediumProfile || highProfile || aggressiveProfile)
        {
            CodeGuard.PostBuildGuarding = true;
            if (lowProfile)
            {
                CodeGuard.SetProfile(1);
            }
            else if (mediumProfile)
            {
                CodeGuard.SetProfile(2);
            }
            else if (highProfile)
            {
                CodeGuard.SetProfile(3);
            }
            else if (aggressiveProfile)
            {
                CodeGuard.SetProfile(4);
            }
        }
        else if (unityPackageProfile)
        {
            CodeGuard.SetProfile(5);
        }
        else
        {
            CodeGuard.PostBuildGuarding = false;
            CodeGuard.SetProfile(0);
        }

        if (_window) _window.Repaint();
    }

    #endregion

    #endregion


}