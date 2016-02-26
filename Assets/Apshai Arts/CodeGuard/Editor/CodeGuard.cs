#if UNITY_4 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
#define UNITY_4_X
#endif

#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ApshaiArts.CodeGuard;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#endregion

[InitializeOnLoad]
public static class CodeGuard
{
    static CodeGuard()
    {
        LoadCodeGuardSettings();
    }

    public static void LoadCodeGuardSettings()
    {
        InitProfiles();
        LoadExcludedMethods();
        LoadAssembliesSetting();
        LoadSkipTypes();
        LoadSelectTypes();
    }

    private static string _projectName
    {
        get { return Application.dataPath; }
    }

    #region Settings

    public static List<CodeGuardProfileSettings> profiles
    {
        get { return _profiles; }
    }
    private static List<CodeGuardProfileSettings> _profiles = new List<CodeGuardProfileSettings>(5);
    private static void InitProfiles()
    {
        CodeGuardProfileSettings noProfile = new CodeGuardProfileSettings
        {
            Name = "None",
            ObfuscatePrivateMembers = false,
            ObfuscatePrivateFieldsAndProperties = false,
            ObfuscateTypeFields = false,
            ObfuscateTypeFieldsAggressively = false,
            ObfuscateProperties = false,
            ObfuscateCustomMethods = false,
            ObfuscateProxyParameters = false,
            ObfuscateMethodParameters = false,
            ObfuscateAggressively = false,
            ProxyUnityMethods = false,
            ProxyExcludedMethods = false,
            ProxyCustomMethods = false,
            StripUnityEngineAttributes = false,
            SkipUnityTypesPublicFields = false,
            RPCsAction = 0,
            SymbolRenamingMode = 1
        };
        //_profiles[0] = noProfile;
        _profiles.Add(noProfile);

        CodeGuardProfileSettings lowProfile = new CodeGuardProfileSettings
        {
            Name = "Low",
            ObfuscatePrivateMembers = false,
            ObfuscatePrivateFieldsAndProperties = true,
            ObfuscateTypeFields = true,
            ObfuscateTypeFieldsAggressively = false,
            ObfuscateProperties = false,
            ObfuscateCustomMethods = false,
            ObfuscateProxyParameters = true,
            ObfuscateMethodParameters = true,
            ObfuscateAggressively = false,
            ProxyUnityMethods = false,
            ProxyExcludedMethods = false,
            ProxyCustomMethods = false,
            StripUnityEngineAttributes = true,
            SkipUnityTypesPublicFields = true,
            RPCsAction = 0,
            SymbolRenamingMode = 1
        };
        //_profiles[1] = lowProfile;
        _profiles.Add(lowProfile);

        CodeGuardProfileSettings medProfile = new CodeGuardProfileSettings
        {
            Name = "Medium",
            ObfuscatePrivateMembers = false,
            ObfuscatePrivateFieldsAndProperties = true,
            ObfuscateTypeFields = true,
            ObfuscateTypeFieldsAggressively = false,
            ObfuscateProperties = true,
            ObfuscateCustomMethods = true,
            ObfuscateProxyParameters = true,
            ObfuscateMethodParameters = true,
            ObfuscateAggressively = false,
            ProxyUnityMethods = true,
            ProxyExcludedMethods = true,
            ProxyCustomMethods = true,
            StripUnityEngineAttributes = true,
            SkipUnityTypesPublicFields = false,
            RPCsAction = 1,
            SymbolRenamingMode = 0
        };
        //_profiles[2] = medProfile;
        _profiles.Add(medProfile);

        CodeGuardProfileSettings higProfile = new CodeGuardProfileSettings
        {
            Name = "High",
            ObfuscatePrivateMembers = true,
            ObfuscatePrivateFieldsAndProperties = true,
            ObfuscateTypeFields = true,
            ObfuscateTypeFieldsAggressively = false,
            ObfuscateProperties = true,
            ObfuscateCustomMethods = true,
            ObfuscateProxyParameters = true,
            ObfuscateMethodParameters = true,
            ObfuscateAggressively = false,
            ProxyUnityMethods = true,
            ProxyExcludedMethods = true,
            ProxyCustomMethods = false,
            StripUnityEngineAttributes = true,
            SkipUnityTypesPublicFields = false,
            RPCsAction = 1,
            SymbolRenamingMode = 0
        };
        //_profiles[3] = higProfile;
        _profiles.Add(higProfile);

        CodeGuardProfileSettings aggProfile = new CodeGuardProfileSettings
        {
            Name = "Aggressive",
            ObfuscatePrivateMembers = true,
            ObfuscatePrivateFieldsAndProperties = true,
            ObfuscateTypeFields = true,
            ObfuscateTypeFieldsAggressively = true,
            ObfuscateProperties = true,
            ObfuscateCustomMethods = true,
            ObfuscateProxyParameters = true,
            ObfuscateMethodParameters = true,
            ObfuscateAggressively = true,
            ProxyUnityMethods = true,
            ProxyExcludedMethods = true,
            ProxyCustomMethods = false,
            StripUnityEngineAttributes = true,
            SkipUnityTypesPublicFields = false,
            RPCsAction = 1,
            SymbolRenamingMode = 0
        };
        //_profiles[4] = aggProfile;
        _profiles.Add(aggProfile);

        CodeGuardProfileSettings webPlayerProfile = new CodeGuardProfileSettings
        {
#if !CodeGuard14
            Name = "WebPlayer",
#else
            Name = "WebPlayer, UnityPackage",
#endif
            ObfuscatePrivateMembers = false,
            ObfuscatePrivateFieldsAndProperties = true,
            ObfuscateTypeFields = true,
            ObfuscateTypeFieldsAggressively = false,
            ObfuscateProperties = true,
            ObfuscateCustomMethods = true,
            ObfuscateProxyParameters = true,
            ObfuscateMethodParameters = true,
            ObfuscateAggressively = false,
            ProxyUnityMethods = true,
            ProxyExcludedMethods = true,
            ProxyCustomMethods = true,
            StripUnityEngineAttributes = true,
            SkipUnityTypesPublicFields = true,
            SkipFieldsWithSerializeFieldAttribute = true,
            RPCsAction = 1,
            SymbolRenamingMode = 1
        };
        //_profiles[5] = webPlayerProfile;
        _profiles.Add(webPlayerProfile);

        // Load Custom Profiles
        int checkCount = 99;
        for (int i = 0; i < checkCount; i++)
        {
            if (EditorPrefs.HasKey("CodeGuard Custom Profile" + i.ToString()))
            {
                CodeGuardProfileSettings setting = new CodeGuardProfileSettings();
                setting.Name = EditorPrefs.GetString("CodeGuard Custom Profile" + i.ToString() + ": Name", "My Profile");
                setting.ObfuscatePrivateMembers = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscatePrivateMembers");
                setting.ObfuscatePrivateFieldsAndProperties = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscatePrivateFieldsAndProperties");
                setting.ObfuscateTypeFields = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscateTypeFields");
                setting.ObfuscateTypeFieldsAggressively = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscateTypeFieldsAggressively");
                setting.ObfuscateProperties = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscateProperties");
                setting.ObfuscateCustomMethods = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscateCustomMethods");
                setting.ObfuscateProxyParameters = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscateProxyParameters");
                setting.ObfuscateMethodParameters = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscateMethodParameters");
                setting.ObfuscateAggressively = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscateAggressively");
                setting.ProxyUnityMethods = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ProxyUnityMethods");
                setting.ProxyExcludedMethods = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ProxyExcludedMethods");
                setting.ProxyCustomMethods = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ProxyCustomMethods");
                setting.ObfuscateMethodParameters = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": ObfuscateMethodParameters");
                setting.StripUnityEngineAttributes = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": StripUnityEngineAttributes");
                setting.SkipUnityTypesPublicFields = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": SkipUnityTypesPublicFields");
                setting.SkipFieldsWithSerializeFieldAttribute = EditorPrefs.GetBool("CodeGuard Custom Profile" + i.ToString() + ": SkipFieldsWithSerializeFieldAttribute");
                setting.RPCsAction = EditorPrefs.GetInt("CodeGuard Custom Profile" + i.ToString() + ": RPCsAction");
                setting.SymbolRenamingMode = EditorPrefs.GetInt("CodeGuard Custom Profile" + i.ToString() + ": SymbolRenamingMode");
                _profiles.Add(setting);
            }
        }
    }
    private static void SetProfile(CodeGuardProfileSettings profile)
    {
        PrivateMembers = profile.ObfuscatePrivateMembers;
        PrivateFieldsAndProperties = profile.ObfuscatePrivateFieldsAndProperties;
        TypeFields = profile.ObfuscateTypeFields;
        TypeFieldsAggressively = profile.ObfuscateTypeFieldsAggressively;
        ObfuscateProperties = profile.ObfuscateProperties;
        ObfuscateCustomMethods = profile.ObfuscateCustomMethods;
        GuardProxyParameters = profile.ObfuscateProxyParameters;
        GuardMethodParameters = profile.ObfuscateMethodParameters;
        GuardAggressively = profile.ObfuscateAggressively;
        ProxyUnityMethods = profile.ProxyUnityMethods;
        ProxyExcludedMethods = profile.ProxyExcludedMethods;
        ProxyCustomMethods = profile.ProxyCustomMethods;
        SkipUnityTypesPublicFields = profile.SkipUnityTypesPublicFields;
        SkipFieldsWithSerializeFieldAttribute = profile.SkipFieldsWithSerializeFieldAttribute;
        RPCsAction = profile.RPCsAction;
        SymbolRenamingMode = profile.SymbolRenamingMode;
    }
    public static void SetProfile(int index)
    {
        SetProfile(_profiles[index]);
    }
    public static int GetProfile()
    {
        EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", false);
        EditorPrefs.SetBool(_projectName + "CodeGuard UnityPackageProfile", false);

        int profileIndex = 6; // custom profile

        for (int index = 0; index < _profiles.Count; index++)
        {
            CodeGuardProfileSettings profile = _profiles[index];
            if (PrivateMembers != profile.ObfuscatePrivateMembers) continue;
            if (PrivateFieldsAndProperties != profile.ObfuscatePrivateFieldsAndProperties) continue;
            if (TypeFields != profile.ObfuscateTypeFields) continue;
            if (TypeFieldsAggressively != profile.ObfuscateTypeFieldsAggressively) continue;
            if (ObfuscateProperties != profile.ObfuscateProperties) continue;
            if (ObfuscateCustomMethods != profile.ObfuscateCustomMethods) continue;
            if (GuardProxyParameters != profile.ObfuscateProxyParameters) continue;
            if (GuardMethodParameters != profile.ObfuscateMethodParameters) continue;
            if (GuardAggressively != profile.ObfuscateAggressively) continue;
            if (ProxyUnityMethods != profile.ProxyUnityMethods) continue;
            if (ProxyExcludedMethods != profile.ProxyExcludedMethods) continue;
            if (ProxyCustomMethods != profile.ProxyCustomMethods) continue;
            if (RPCsAction != profile.RPCsAction) continue;
            if (SkipUnityTypesPublicFields != profile.SkipUnityTypesPublicFields) continue;
            if (SkipUnityTypesPublicFields != profile.SkipUnityTypesPublicFields) continue;
            if (SymbolRenamingMode != profile.SymbolRenamingMode) continue;
            //if (StripUnityEngineAttributes != profile.StripUnityEngineAttributes) continue;
            profileIndex = index;
            break;
        }

        switch (profileIndex)
        {
            case 0: // no profile
                return 0;
            case 1: // low profile
                EditorPrefs.SetBool(_projectName + "CodeGuard LowProfile", true);
                return 1;
            case 2: // medium profile
                EditorPrefs.SetBool(_projectName + "CodeGuard MediumProfile", true);
                return 2;
            case 3: // high profile
                EditorPrefs.SetBool(_projectName + "CodeGuard HighProfile", true);
                return 3;
            case 4: // agg profile
                EditorPrefs.SetBool(_projectName + "CodeGuard AggressiveProfile", true);
                return 4;
            case 5: // unitypackage profile
                EditorPrefs.SetBool(_projectName + "CodeGuard UnityPackageProfile", true);
                return 5;
            default:
                return 6;
        }
    }

    private static List<string> _excludedMethods;
    public static List<string> ExcludedMethods { get { return _excludedMethods; } }

    private static int _excludedMethodsCount;
    public static int ExcludedMethodsCount
    {
        get { return _excludedMethodsCount; }
        set
        {
            int count = _excludedMethods.Count;
            if (count >= value)
            {
                _excludedMethodsCount = value;
            }
            else
            {
                while (count < value)
                {
                    _excludedMethods.Add("");
                    count = _excludedMethods.Count;
                }
                _excludedMethodsCount = _excludedMethods.Count;
            }
        }
    }

    public static void SaveExcludedMethods()
    {
        string saveExcludedMethods = "";
        if (_excludedMethodsCount > 0)
        {
            for (int i = 0; i < _excludedMethodsCount - 1; i++)
            {
                saveExcludedMethods += _excludedMethods[i] + ",";
            }
            saveExcludedMethods += _excludedMethods[_excludedMethodsCount - 1];
        }

        EditorPrefs.SetString(_projectName + "CodeGuard ExcludedMethods Array", saveExcludedMethods);
    }
    private static void LoadExcludedMethods()
    {
        if (_excludedMethods == null) _excludedMethods = new List<string>();
        _excludedMethods.Clear();
        _excludedMethodsCount = 0;

        string savedExcludedMethodsPreferences = EditorPrefs.GetString(
            _projectName + "CodeGuard ExcludedMethods Array", "");
        if (savedExcludedMethodsPreferences != "")
        {
            string[] savedExcludedMethods = savedExcludedMethodsPreferences.Split(",".ToCharArray(), 9999);
            for (int i = 0; i < savedExcludedMethods.Length; i++)
            {
                _excludedMethods.Add(savedExcludedMethods[i]);
            }
            _excludedMethodsCount = _excludedMethods.Count;
        }
    }

    public static List<string> CustomAssembliesList
    {
        get
        {
            List<string> result = new List<string>();
            foreach (string s in _assembliesSettingList)
            {
                if (!s.EndsWith(".dll"))
                {
                    result.Add(s + ".dll");
                }
                else
                {
                    result.Add(s);
                }
            }
            return result;
        }
    }

    private static List<string> _assembliesSettingList;
    public static List<string> AssembliesSettingList { get { return _assembliesSettingList; } }
    private static int _assembliesSettingListCount;
    public static int AssembliesSettingCount
    {
        get { return _assembliesSettingListCount; }
        set
        {
            int count = _assembliesSettingList.Count;
            if (count >= value)
            {
                _assembliesSettingListCount = value;
            }
            else
            {
                while (count < value)
                {
                    _assembliesSettingList.Add("");
                    count = _assembliesSettingList.Count;
                }
                _assembliesSettingListCount = _assembliesSettingList.Count;
            }
        }
    }

    public static void SaveAssembliesSetting()
    {
        string saveassembliesSetting = "";
        if (_assembliesSettingListCount > 0)
        {
            for (int i = 0; i < _assembliesSettingListCount - 1; i++)
            {
                saveassembliesSetting += _assembliesSettingList[i] + ",";
            }
            saveassembliesSetting += _assembliesSettingList[_assembliesSettingListCount - 1];
        }

        EditorPrefs.SetString(_projectName + "CodeGuard Assemblies Setting Array", saveassembliesSetting);
    }
    private static void LoadAssembliesSetting()
    {
        if (_assembliesSettingList == null) _assembliesSettingList = new List<string>();
        _assembliesSettingList.Clear();
        _assembliesSettingListCount = 0;

        string savedassembliesSettingPreferences =
            EditorPrefs.GetString(_projectName + "CodeGuard Assemblies Setting Array", "");
        if (savedassembliesSettingPreferences != "")
        {
            string[] savedassembliesSetting = savedassembliesSettingPreferences.Split(",".ToCharArray(), 9999);
            for (int i = 0; i < savedassembliesSetting.Length; i++)
            {
                _assembliesSettingList.Add(savedassembliesSetting[i]);
            }
            _assembliesSettingListCount = _assembliesSettingList.Count;
        }
    }
    public static int AssembliesSetting
    {
        set { EditorPrefs.SetInt(_projectName + "CodeGuard AssembliesSetting", value); }
        get { return EditorPrefs.GetInt(_projectName + "CodeGuard AssembliesSetting", 0); }
    }

    private static List<string> _selectTypes;
    public static List<string> SelectTypes { get { return _selectTypes; } }
    private static int _selectTypesCount;

    public static int SelectTypesCount
    {
        get { return _selectTypesCount; }
        set
        {
            int count = _selectTypes.Count;
            if (count >= value)
            {
                _selectTypesCount = value;
            }
            else
            {
                while (count < value)
                {
                    _selectTypes.Add("");
                    count = _selectTypes.Count;
                }
                _selectTypesCount = _selectTypes.Count;
            }
        }
    }

    public static void SaveSelectTypes()
    {
        string saveSelectTypes = "";
        if (_selectTypesCount > 0)
        {
            for (int i = 0; i < _selectTypesCount - 1; i++)
            {
                saveSelectTypes += _selectTypes[i] + ",";
            }
            saveSelectTypes += _selectTypes[_selectTypesCount - 1];
        }

        EditorPrefs.SetString(_projectName + "CodeGuard SelectTypes Array", saveSelectTypes);
    }
    private static void LoadSelectTypes()
    {
        if (_selectTypes == null) _selectTypes = new List<string>();
        _selectTypes.Clear();
        _selectTypesCount = 0;

        string savedSelectTypesPreferences = EditorPrefs.GetString(_projectName + "CodeGuard SelectTypes Array", "");
        if (savedSelectTypesPreferences != "")
        {
            string[] savedSelectTypes = savedSelectTypesPreferences.Split(",".ToCharArray(), 9999);
            for (int i = 0; i < savedSelectTypes.Length; i++)
            {
                _selectTypes.Add(savedSelectTypes[i]);
            }
            _selectTypesCount = _selectTypes.Count;
        }
    }

    public static int TypeSelectionMode
    {
        set { EditorPrefs.SetInt(_projectName + "CodeGuard TypeSelectionMode", value); }
        get { return EditorPrefs.GetInt(_projectName + "CodeGuard TypeSelectionMode", 0); }
    }

    public static bool StripUnityEngineAttributes
    {
        set { EditorPrefs.SetBool(_projectName + "CodeGuard StripUnityEngineAttributes", value); }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard StripUnityEngineAttributes", true); }
    }

    private static List<string> _skipTypes;
    public static List<string> SkipTypes { get { return _skipTypes; } }
    private static int _skipTypesCount;

    public static int SkipTypesCount
    {
        get { return _skipTypesCount; }
        set
        {
            int count = _skipTypes.Count;
            if (count >= value)
            {
                _skipTypesCount = value;
            }
            else
            {
                while (count < value)
                {
                    _skipTypes.Add("");
                    count = _skipTypes.Count;
                }
                _skipTypesCount = _skipTypes.Count;
            }
        }
    }

    public static void SaveSkipTypes()
    {
        string saveSkipTypes = "";
        if (_skipTypesCount > 0)
        {
            for (int i = 0; i < _skipTypesCount - 1; i++)
            {
                saveSkipTypes += _skipTypes[i] + ",";
            }
            saveSkipTypes += _skipTypes[_skipTypesCount - 1];
        }

        EditorPrefs.SetString(_projectName + "CodeGuard SkipTypes Array", saveSkipTypes);
    }
    private static void LoadSkipTypes()
    {
        if (_skipTypes == null) _skipTypes = new List<string>();
        _skipTypes.Clear();
        _skipTypesCount = 0;

        string savedSkipTypesPreferences = EditorPrefs.GetString(_projectName + "CodeGuard SkipTypes Array", "");
        if (savedSkipTypesPreferences != "")
        {
            string[] savedSkipTypes = savedSkipTypesPreferences.Split(",".ToCharArray(), 9999);
            for (int i = 0; i < savedSkipTypes.Length; i++)
            {
                _skipTypes.Add(savedSkipTypes[i]);
            }
            _skipTypesCount = _skipTypes.Count;
        }
    }


    public static bool AutoAddScriptAssemblies
    {
        set { EditorPrefs.SetBool(_projectName + "CodeGuard AutoAddScriptAssemblies", value); }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard AutoAddScriptAssemblies", true); }
    }

    public static bool PrivateMembers
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard PrivateMembers", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard PrivateMembers", false); }
    }

    public static bool PrivateFieldsAndProperties
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard PrivateFieldsAndProperties", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard PrivateFieldsAndProperties", false); }
    }

    public static bool TypeFields
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard TypeFields", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard TypeFields", false); }
    }

    public static bool TypeFieldsAggressively
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard TypeFieldsAggressively", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard TypeFieldsAggressively", false); }
    }

    public static bool ObfuscateProperties
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard ObfuscateProperties", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard ObfuscateProperties", false); }
    }

    public static bool ObfuscateCustomMethods
    {
        set { EditorPrefs.SetBool(_projectName + "CodeGuard ObfuscateCustomMethods", value); }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard ObfuscateCustomMethods", false); }
    }

    public static bool ProxyUnityMethods
    {
        set { EditorPrefs.SetBool(_projectName + "CodeGuard ProxyUnityMethods", value); }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard ProxyUnityMethods", false); }
    }

    public static bool ProxyExcludedMethods
    {
        set { EditorPrefs.SetBool(_projectName + "CodeGuard ProxyExcludedMethods", value); }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard ProxyExcludedMethods", false); }
    }

    public static bool ProxyCustomMethods
    {
        set { EditorPrefs.SetBool(_projectName + "CodeGuard ProxyCustomMethods", value); }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard ProxyCustomMethods", false); }
    }

    public static bool GuardProxyParameters
    {
        set { EditorPrefs.SetBool(_projectName + "CodeGuard GuardProxyParameters", value); }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard GuardProxyParameters", false); }
    }

    public static bool GuardMethodParameters
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard GuardMethodParameters", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard GuardMethodParameters", false); }
    }

    public static bool GuardAggressively
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard GuardAggressively", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard GuardAggressively", false); }
    }

    public static bool CreateBackup
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard CreateBackup", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard CreateBackup", true); }
    }

    public static bool PostBuildGuarding
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard IsPostBuildEnabled", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard IsPostBuildEnabled", false); }
    }

    public static bool SkipUnityTypesPublicFields
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard SkipUnityTypesPublicFields", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard SkipUnityTypesPublicFields", false); }
    }

    public static bool SkipUnityTypesPublicStaticFields
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard SkipUnityTypesPublicStaticFields", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard SkipUnityTypesPublicStaticFields", false); }
    }

    public static bool SkipFieldsWithSerializeFieldAttribute
    {
        set
        {
            EditorPrefs.SetBool(_projectName + "CodeGuard SkipFieldsWithSerializeFieldAttribute", value);
        }
        get { return EditorPrefs.GetBool(_projectName + "CodeGuard SkipFieldsWithSerializeFieldAttribute", false); }
    }


    public static int SymbolRenamingMode
    {
        set
        {
            EditorPrefs.SetInt(_projectName + "CodeGuard SymbolRenamingMode", value);
        }
        get { return EditorPrefs.GetInt(_projectName + "CodeGuard SymbolRenamingMode", 0); }
    }

    public static int RPCsAction
    {
        set
        {
            EditorPrefs.SetInt(_projectName + "CodeGuard RPCsAction", value);
        }
        get { return EditorPrefs.GetInt(_projectName + "CodeGuard RPCsAction", 0); }
    }

    #endregion

    public static CodeGuardSetup CodeGuardSetupSettings()
    {
        CodeGuardSetup setup = new CodeGuardSetup();
        setup.obfuscatePrivateMembers = PrivateMembers;
        setup.obfuscatePrivateFieldsAndProperties = PrivateFieldsAndProperties;
        setup.obfuscateTypeFields = TypeFields;
        setup.obfuscateTypeFieldsAggressively = TypeFieldsAggressively;
        setup.obfuscateCustomMethods = ObfuscateCustomMethods;
        setup.obfuscateProxyParameters = GuardProxyParameters;
        setup.obfuscateMethodParameters = GuardMethodParameters;
        setup.obfuscateHeavy = GuardAggressively;
        setup.proxyUnityMethods = ProxyUnityMethods;
        setup.proxyExcludedMethods = ProxyExcludedMethods;
        setup.proxyCustomMethods = ProxyCustomMethods;
        setup.skipUnityTypesPublicFields = SkipUnityTypesPublicFields;
        setup.skipUnityTypesPublicStaticFields = SkipUnityTypesPublicStaticFields;
        setup.skipFieldsWithSerializeFieldAttribute = SkipFieldsWithSerializeFieldAttribute;
        setup.stripUnityEngineAttributes = StripUnityEngineAttributes;

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        setup.symbolRenamingModeLatin = false;
        setup.symbolRenamingModeUnreadableLite = false;
        //if ((buildTarget == BuildTarget.WebPlayer || buildTarget == BuildTarget.WebPlayerStreamed))
        {
            if (SymbolRenamingMode == 2) setup.symbolRenamingModeLatin = true;
            if (SymbolRenamingMode == 1) setup.symbolRenamingModeUnreadableLite = true;
        }

        setup.RPCsAction = RPCsAction;

        for (int index = 0; index < _excludedMethodsCount; index++)
        {
            string excludedMethod = _excludedMethods[index];
            setup.IncludeAsUnityMethod(excludedMethod);
        }

        setup.typeSelectionMode = TypeSelectionMode;

        if (TypeSelectionMode != 1)
        {
            for (int index = 0; index < _skipTypesCount; index++)
            {
                string skipType = _skipTypes[index];
                setup.SkipType(skipType);
            }
        }

        if (TypeSelectionMode != 0)
        {
            for (int index = 0; index < _selectTypesCount; index++)
            {
                string selectType = _selectTypes[index];
                setup.SelectType(selectType);
            }
        }

        setup.assemblySelectionMode = CodeGuard.AssembliesSetting;

        if (buildTarget == BuildTarget.WebPlayer || buildTarget == BuildTarget.WebPlayerStreamed)
        {
            string enginePath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines");
            enginePath = Path.Combine(enginePath, "WebPlayer");
            enginePath = Path.Combine(enginePath, "Managed");
            setup.AddAssemblySearchDirectory(enginePath);
            setup.buildTarget = "WebPlayer";
        }
        else if (buildTarget == BuildTarget.Android)
        {
            string enginePath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines");
            if (EditorUserBuildSettings.development)
            {
                enginePath = Path.Combine(enginePath, "androiddevelopmentplayer");
            }
            else
                enginePath = Path.Combine(enginePath, "androidplayer");
            enginePath = Path.Combine(enginePath, "Managed");
            setup.AddAssemblySearchDirectory(enginePath);
            setup.buildTarget = "Android";
        }
        else if (buildTarget == BuildTarget.iOS)
        {
            string enginePath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines");
            enginePath = Path.Combine(enginePath, "iphoneplayer");
            enginePath = Path.Combine(enginePath, "Managed");
            setup.AddAssemblySearchDirectory(enginePath);
            setup.buildTarget = "iPhone";
        }
        else
        {
            string enginePath = Path.Combine(EditorApplication.applicationContentsPath, "Managed");
            setup.AddAssemblySearchDirectory(enginePath);
            setup.buildTarget = "Standalone";
        }

        return setup;
    }


    private static readonly string[] unityAssemblies =
        {
            "Boo.Lang.dll", "Mono.Security.dll", "mscorlib.dll", "System.dll", "System.Core.dll",
            "UnityEngine.dll", "UnityEditor.dll", "UnityScript.Lang.dll"
        };

    private static bool DoCodeGuard(string fromPath, string resultPath)
    {
        float progress = 0.0f;
        EditorUtility.DisplayProgressBar("CodeGuard", "Obfuscating and protecting code...", progress);

        CodeGuardSetup setup = CodeGuard.CodeGuardSetupSettings();

        string[] files = Directory.GetFiles(fromPath, "*.dll", SearchOption.TopDirectoryOnly);

        if (AssembliesSetting == 0)
        {
            const string unityScriptAssemblies = @"^Assembly-.*\.dll$";
            Regex regex = new Regex(unityScriptAssemblies, RegexOptions.IgnoreCase);

            for (int index = 0; index < files.Length; index++)
            {
                if (regex.IsMatch(Path.GetFileName(files[index])))
                {
                    setup.AddAssembly(files[index]);
                }
            }
        }
        else if (AssembliesSetting == 1)
        {
            for (int index = 0; index < files.Length; index++)
            {
                if (unityAssemblies.Contains(Path.GetFileName(files[index])))
                {
                }
                else
                {
                    setup.AddAssembly(files[index]);
                }
            }
        }
        else if (AssembliesSetting == 2)
        {
            const string unityScriptAssemblies = @"^Assembly-.*\.dll$";
            Regex regex = new Regex(unityScriptAssemblies, RegexOptions.IgnoreCase);

            string[] assemblies = CustomAssembliesList.ToArray();
            for (int index = 0; index < assemblies.Length; index++)
            {
                setup.SelectCustomAssembly(assemblies[index]);
            }

            for (int index = 0; index < files.Length; index++)
            {
                if (AutoAddScriptAssemblies)
                {
                    if (regex.IsMatch(Path.GetFileName(files[index])))
                    {
                        setup.AddAssembly(files[index]);
                    }
                }
                
                setup.AddAssemblyIfInCustomAssemblies(files[index]);
            }
        }

        setup.outputDirectory = resultPath;

        progress = 0.25f;
        EditorUtility.DisplayProgressBar("CodeGuard", "Obfuscating and protecting code...", progress);

        setup.Run();

        EditorUtility.ClearProgressBar();
        return true;
    }

    public static void DoCodeGuardFolder(string folderPath)
    {
        DoCodeGuardFolder(folderPath, false);
    }
    public static void DoCodeGuardFolder(string folderPath, bool createBackup)
    {
        DirectoryInfo assemblyDir = new DirectoryInfo(folderPath);
        string outputPath = assemblyDir.Parent.FullName + Path.DirectorySeparatorChar + "CodeGuarded";

        DoCodeGuard(folderPath, outputPath);

        if (createBackup)
        {
            // Create backup folder
            DirectoryInfo backupDir = assemblyDir.Parent.CreateSubdirectory("CodeGuard Backups");
            CopyFilesFromDirectory(assemblyDir, backupDir);
        }

        // Copy from CodeGuarded to Managed
        DirectoryInfo codeGuardedDir = new DirectoryInfo(outputPath);
        CopyFilesFromDirectory(codeGuardedDir, assemblyDir);

        // Delete CodeGuarded
        codeGuardedDir.Delete(true);

        Debug.Log(CodeGuardReporter.LoggedError
                      ? "CodeGuard: Failed to obfuscate and protect the assemblies."
                      : "CodeGuard: Finished obfuscating and protecting assemblies.");
    }

    public static bool DoCodeGuardAssembly(string assemblyPath, bool createBackup)
    {
        Debug.Log("CodeGuard: Protecting and obfuscating assembly: " + assemblyPath);

        FileInfo assemblyFile = new FileInfo(assemblyPath);
        if (!assemblyFile.Exists)
        {
            Debug.LogError("CodeGuard: Couldn't find assembly: " + assemblyFile.FullName);
            return false;
        }

        float progress = 0.0f;
        EditorUtility.DisplayProgressBar("CodeGuard", "Obfuscating and protecting code...", progress);

        CodeGuardSetup setup = CodeGuard.CodeGuardSetupSettings();

        setup.AddAssembly(assemblyPath);

        DirectoryInfo d = Directory.GetParent(assemblyPath);

        setup.outputDirectory = d.FullName;

        progress = 0.25f;
        EditorUtility.DisplayProgressBar("CodeGuard", "Obfuscating and protecting code...", progress);

        setup.Run();

        EditorUtility.ClearProgressBar();
        return true;
    }
    private static bool DoCodeGuardWebPlayerBuild()
    {
        string monoPath;
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            monoPath = EditorApplication.applicationContentsPath + Path.DirectorySeparatorChar + "Frameworks" +
                       Path.DirectorySeparatorChar + "Mono";
        }
        else
        {
            monoPath = EditorApplication.applicationContentsPath + Path.DirectorySeparatorChar + "Mono";
        }
        monoPath += Path.DirectorySeparatorChar + "lib";
        monoPath += Path.DirectorySeparatorChar + "mono";
        monoPath += Path.DirectorySeparatorChar + "unity_web";

        string enginePath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines");
        enginePath = Path.Combine(enginePath, "WebPlayer");
        enginePath = Path.Combine(enginePath, "Managed");

        string scriptsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library");
        scriptsPath = Path.Combine(scriptsPath, "ScriptAssemblies");

        List<string> files = new List<string>();
        files.AddRange(Directory.GetFiles(monoPath, "*.dll"));
        files.AddRange(Directory.GetFiles(enginePath, "*.dll"));
        files.AddRange(Directory.GetFiles(scriptsPath, "*.dll"));

        string tmpPath = FileUtil.GetUniqueTempPathInProject() + "-TmpWebPlayer";
        Directory.CreateDirectory(tmpPath);

        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            File.Copy(file, Path.Combine(tmpPath, Path.GetFileName(file)), true);
        }

        string codeGuardedTmp = FileUtil.GetUniqueTempPathInProject() + "-CodeGuardedWebPlayer";
        Directory.CreateDirectory(codeGuardedTmp);

        if (!DoCodeGuard(tmpPath, codeGuardedTmp))
        {
            return false;
        }

        string[] scripts = Directory.GetFiles(scriptsPath, "*.dll");
        for (int i = 0; i < scripts.Length; i++)
        {
            string file = scripts[i];
            File.Copy(Path.Combine(codeGuardedTmp, Path.GetFileName(file)), file, true);
        }

        // Delete temporary files
        Directory.Delete(tmpPath, true);
        Directory.Delete(codeGuardedTmp, true);

        return true;
    }

    private static bool DoCodeGuardAndroidBuild()
    {
        string monoPath;
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            monoPath = EditorApplication.applicationContentsPath + Path.DirectorySeparatorChar + "Frameworks" +
                       Path.DirectorySeparatorChar + "Mono";
        }
        else
        {
            monoPath = EditorApplication.applicationContentsPath + Path.DirectorySeparatorChar + "Mono";
        }
        monoPath += Path.DirectorySeparatorChar + "lib";
        monoPath += Path.DirectorySeparatorChar + "mono";

        //if (EditorUserBuildSettings.androidBuildSubtarget)
        monoPath += Path.DirectorySeparatorChar + "unity";

        string enginePath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines");

        if (EditorUserBuildSettings.development)
        {
            enginePath = Path.Combine(enginePath, "androiddevelopmentplayer");
        }
        else
            enginePath = Path.Combine(enginePath, "androidplayer");

        enginePath = Path.Combine(enginePath, "Managed");

        string scriptsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library");
        scriptsPath = Path.Combine(scriptsPath, "ScriptAssemblies");

        List<string> files = new List<string>();
        files.AddRange(Directory.GetFiles(monoPath, "*.dll"));
        //files.AddRange(Directory.GetFiles(enginePath, "*.dll"));
        files.AddRange(Directory.GetFiles(scriptsPath, "*.dll"));

        string tmpPath = FileUtil.GetUniqueTempPathInProject() + "-TmpAndroid";
        Directory.CreateDirectory(tmpPath);

        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            File.Copy(file, Path.Combine(tmpPath, Path.GetFileName(file)), true);
        }

        string codeGuardedTmp = FileUtil.GetUniqueTempPathInProject() + "-CodeGuardedAndroid";
        Directory.CreateDirectory(codeGuardedTmp);

        if (!DoCodeGuard(tmpPath, codeGuardedTmp))
        {
            return false;
        }

        string[] scripts = Directory.GetFiles(scriptsPath, "*.dll");
        for (int i = 0; i < scripts.Length; i++)
        {
            string file = scripts[i];
            File.Copy(Path.Combine(codeGuardedTmp, Path.GetFileName(file)), file, true);
        }

        // Delete temporary files
        Directory.Delete(tmpPath, true);
        Directory.Delete(codeGuardedTmp, true);

        return true;
    }

    private static bool DoCodeGuardiOSBuild()
    {
        string monoPath;
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            monoPath = EditorApplication.applicationContentsPath + Path.DirectorySeparatorChar + "Frameworks" +
                       Path.DirectorySeparatorChar + "Mono";
        }
        else
        {
            monoPath = EditorApplication.applicationContentsPath + Path.DirectorySeparatorChar + "Mono";
        }
        monoPath += Path.DirectorySeparatorChar + "lib";
        monoPath += Path.DirectorySeparatorChar + "mono";

        monoPath += Path.DirectorySeparatorChar + "unity";

        string enginePath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines");

        enginePath = Path.Combine(enginePath, "iphoneplayer");

        enginePath = Path.Combine(enginePath, "Managed");

        string scriptsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library");
        scriptsPath = Path.Combine(scriptsPath, "ScriptAssemblies");

        List<string> files = new List<string>();
        files.AddRange(Directory.GetFiles(monoPath, "*.dll"));
        files.AddRange(Directory.GetFiles(enginePath, "*.dll"));
        files.AddRange(Directory.GetFiles(scriptsPath, "*.dll"));

        string tmpPath = FileUtil.GetUniqueTempPathInProject() + "-TmpiOS";
        Directory.CreateDirectory(tmpPath);

        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            File.Copy(file, Path.Combine(tmpPath, Path.GetFileName(file)), true);
        }

        string codeGuardedTmp = FileUtil.GetUniqueTempPathInProject() + "-CodeGuardediOS";
        Directory.CreateDirectory(codeGuardedTmp);

        if (!DoCodeGuard(tmpPath, codeGuardedTmp))
        {
            return false;
        }

        string[] scripts = Directory.GetFiles(scriptsPath, "*.dll");
        for (int i = 0; i < scripts.Length; i++)
        {
            string file = scripts[i];
            File.Copy(Path.Combine(codeGuardedTmp, Path.GetFileName(file)), file, true);
        }

        // Delete temporary files
        Directory.Delete(tmpPath, true);
        Directory.Delete(codeGuardedTmp, true);

        return true;
    }

    // Post build
    [PostProcessBuild(1000)]
    private static void OnPostprocessBuildPlayer(BuildTarget buildTarget, string buildPath)
    {
        if (!EditorPrefs.GetBool(_projectName + "CodeGuard IsPostBuildEnabled", false)) return;

#if UNITY_4_X
        bool windowsOrLinux = (buildTarget == BuildTarget.StandaloneWindows || buildTarget == BuildTarget.StandaloneWindows64 ||
                               buildTarget == BuildTarget.StandaloneLinux || buildTarget == BuildTarget.StandaloneLinux64 || buildTarget == BuildTarget.StandaloneLinuxUniversal);
#else
        bool windowsOrLinux = (buildTarget == BuildTarget.StandaloneWindows ||
                               buildTarget == BuildTarget.StandaloneWindows64);
#endif

        if (windowsOrLinux)
        {
            var buildDir = new FileInfo(buildPath).Directory;

            DirectoryInfo dataDir = buildDir.GetDirectories(Path.GetFileNameWithoutExtension(buildPath) + "_Data")[0];

            if (CreateBackup)
            {
                // Create backup
                DirectoryInfo backupDir = new DirectoryInfo(dataDir.FullName + " Backup");
                if (!CopyFilesFromDirectory(dataDir, backupDir, true))
                {
                    Debug.LogError("CodeGuard: Failed to create backup, stopping post-build obfuscation and protection.");
                    return;
                }
            }

            DirectoryInfo managedDir = new DirectoryInfo(dataDir.FullName + Path.DirectorySeparatorChar + "Managed");

            DoCodeGuardFolder(managedDir.FullName);
        }
        else if (buildTarget == BuildTarget.StandaloneOSXIntel)
        {
            FileInfo buildFileInfo = new FileInfo(buildPath);

            if (CreateBackup)
            {
                // Create backup
                DirectoryInfo appDir = new DirectoryInfo(buildFileInfo.FullName);
                DirectoryInfo backupDir = new DirectoryInfo(buildFileInfo.FullName + " Backup");
                if (!CopyFilesFromDirectory(appDir, backupDir, true))
                {
                    Debug.LogError("CodeGuard: Failed to create backup, stopping post-build obfuscation and protection.");
                    return;
                }
            }

            DirectoryInfo dataDir =
                new DirectoryInfo(buildFileInfo.FullName + Path.DirectorySeparatorChar + "Contents" +
                                  Path.DirectorySeparatorChar + "Data");

            DirectoryInfo managedDir = new DirectoryInfo(dataDir.FullName + Path.DirectorySeparatorChar + "Managed");

            DoCodeGuardFolder(managedDir.FullName);
        }
        else if (buildTarget == BuildTarget.WebPlayer || buildTarget == BuildTarget.WebPlayerStreamed)
        {
            _hasMidCodeGuarded = false;
        }
        else if (buildTarget == BuildTarget.Android || buildTarget == BuildTarget.iOS)
        {
            _hasMidCodeGuarded = false;
        }
        else
        {
            Debug.LogWarning("CodeGuard: Post-build obfuscation is not yet implemented for: " + buildTarget);
            return;
        }

        Debug.Log("CodeGuard: Post-build obfuscation and protection finished.");
    }

    private static bool _hasMidCodeGuarded;

    [PostProcessScene]
    private static void MidCodeGuarding()
    {
        if (!EditorPrefs.GetBool(_projectName + "CodeGuard IsPostBuildEnabled", false)) return;
        if (_hasMidCodeGuarded) return;

        // Don't CodeGuard when in Editor and pressing Play
        if (Application.isPlaying || EditorApplication.isPlaying) return;
        //if (!EditorApplication.isCompiling) return;

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        if (buildTarget == BuildTarget.WebPlayer || buildTarget == BuildTarget.WebPlayerStreamed)
        {
            if (DoCodeGuardWebPlayerBuild())
            {
                _hasMidCodeGuarded = true;
            }
            else
            {
                Debug.LogWarning("CodeGuard: Failed to guard WebPlayer!");
            }
        }
        else if (buildTarget == BuildTarget.Android)
        {
            if (DoCodeGuardAndroidBuild())
            {
                _hasMidCodeGuarded = true;
            }
            else
            {
                Debug.LogWarning("CodeGuard: Failed to guard Android build!");
            }
        }
        else if (buildTarget == BuildTarget.iOS)
        {
            if (DoCodeGuardiOSBuild())
            {
                _hasMidCodeGuarded = true;
            }
            else
            {
                Debug.LogWarning("CodeGuard: Failed to guard iOS build!");
            }
        }
    }


    // Helper methods
    private static bool CopyFilesFromDirectory(DirectoryInfo source, DirectoryInfo destination)
    {
        return CopyFilesFromDirectory(source, destination, false, true);
    }
    private static bool CopyFilesFromDirectory(DirectoryInfo source, DirectoryInfo destination, bool copyDirectories)
    {
        return CopyFilesFromDirectory(source, destination, copyDirectories, true);
    }

    private static bool CopyFilesFromDirectory(DirectoryInfo source, DirectoryInfo destination,
                                               bool copyDirectories, bool replace)
    {
        if (!source.Exists)
        {
            Debug.LogError("CodeGuard: Cannot copy from " + source + " since it doesn't exists!");
            return false;
        }

        if (!destination.Exists)
        {
            destination.Create();
        }

        // Copy all files.
        FileInfo[] files = source.GetFiles();
        for (int index = 0; index < files.Length; index++)
        {
            FileInfo file = files[index];
            file.CopyTo(Path.Combine(destination.FullName,
                                     file.Name), replace);
        }

        if (copyDirectories)
        {
            DirectoryInfo[] dirs = source.GetDirectories();
            for (int index = 0; index < dirs.Length; index++)
            {
                DirectoryInfo directory = dirs[index];
                string destinationDir = Path.Combine(destination.FullName, directory.Name);

                CopyFilesFromDirectory(directory, new DirectoryInfo(destinationDir), true, replace);
            }
        }

        return true;
    }
}

public struct CodeGuardProfileSettings
{
    public string Name;
    public bool ObfuscatePrivateMembers;
    public bool ObfuscatePrivateFieldsAndProperties;
    public bool ObfuscateTypeFields;
    public bool ObfuscateTypeFieldsAggressively;
    public bool ObfuscateProperties;
    public bool ObfuscateCustomMethods;
    public bool ObfuscateProxyParameters;
    public bool ObfuscateMethodParameters;
    public bool ObfuscateAggressively;

    public bool ProxyUnityMethods;
    public bool ProxyExcludedMethods;
    public bool ProxyCustomMethods;

    public int RPCsAction;

    public bool SkipUnityTypesPublicFields;

    public bool SkipFieldsWithSerializeFieldAttribute;

    public bool StripUnityEngineAttributes;

    public int SymbolRenamingMode;
}