using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Localizer : LazySingleton<Localizer>
{
    protected override bool CheckDependencies() => true;

    private Dictionary<string, Dictionary<string, string>> translations = new();

    public List<string> languageNames = new List<string> { "Russian" };

    private string currentLanguage;

    protected override void OnDependenciesReady()
    {
        Debug.Log("Localizer initialized");
        currentLanguage = Application.systemLanguage.ToString();
        LoadTranslateFromResources("translateEn");
    }

    public void LoadTranslateFromResources(string fileNameWithoutExtension)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fileNameWithoutExtension);
        if (jsonFile == null)
        {
            Debug.LogError($"JSON ���� {fileNameWithoutExtension} �� ������ � ����� Resources.");
            return;
        }

        try
        {
            translations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonFile.text);

            if (translations == null)
            {
                Debug.LogError("�� ������� ��������������� JSON.");
                return;
            }

            int totalKeys = 0;
            foreach (var lang in translations.Values)
                totalKeys += lang.Count;

            Debug.Log($"����������� ���������: {translations.Count} ������, {totalKeys} ������.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"������ ��� �������� JSON: {ex.Message}");
        }
    }

    public string Localize(string key)
    {
        if (translations.TryGetValue(currentLanguage, out var langDict) &&
            langDict.TryGetValue(key, out string translation))
        {
            return translation;
        }

        Debug.LogWarning($"������� �� ������: ���� '{key}', ���� '{currentLanguage}'");
        return "";
    }
}
