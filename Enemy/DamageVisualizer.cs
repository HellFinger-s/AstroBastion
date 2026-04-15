using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


public enum DamageVisType
{
    ShieldedDamage,
    PureDamage
}


public class DamageVisualizer : LazySingleton<DamageVisualizer>
{
    [SerializeField]
    private Transform playerCamTransform;

    public DamagePlaceholder placeholder;
    public int poolCapacity = 64;

    [SerializedDictionary]
    public SerializedDictionary<DamageVisType, Material> materials;

    [Range(0, 100)]
    public int spreadAngle = 10;
    [Range(0, 30)]
    public int spreadLength = 10;

    private Queue<DamagePlaceholder> placeholders = new Queue<DamagePlaceholder> { };
    private List<DamagePlaceholder> alivePlaceholders = new List<DamagePlaceholder> { };

    private int damageMaxLength = 6;


    void Update()
    {
        for (int i = alivePlaceholders.Count - 1; i >= 0; i--)
        {
            if (alivePlaceholders[i].UpdateAndCheckAlive(playerCamTransform))
            {
                ReleasePlaceholder(alivePlaceholders[i]);
                alivePlaceholders.RemoveAt(i);
            }
        }
    }

    private void InitPool()
    {
        for (int i = 0; i < poolCapacity; i++)
        {
            placeholders.Enqueue(Instantiate(placeholder, Vector3.zero, Quaternion.identity));
        }
    }

    public void ShowDamage(Vector3 startPosition, Vector3 direction, int value, DamageVisType visType, bool isCrit)
    {
        if (placeholders.Count == 0)
            ExpandPlaceholdersQueue(placeholders);
        int[] digits = ConvertNumberToDigitArray(value);
        int damageLength = damageMaxLength - Array.LastIndexOf(digits, -1) - 1;
        int k1 = (damageMaxLength - damageLength) / 2;
        for (int i = 0; i < damageMaxLength; i++)
        {
            if (digits[i] != -1)
            {
                placeholders.Peek().places[k1].digits[digits[i]].SetActive(true);
                placeholders.Peek().places[k1].digit = digits[i];
                placeholders.Peek().places[k1].SetMaterial(materials[visType]);
                k1++;
            }
        }
        placeholders.Peek().Init(startPosition + direction.normalized * spreadLength, visType, isCrit);
        alivePlaceholders.Add(placeholders.Dequeue());
    }

    private void ExpandPlaceholdersQueue(Queue<DamagePlaceholder> queue)
    {
        for (int i = 0; i < poolCapacity; i++)
        {
            queue.Enqueue(Instantiate(placeholder, Vector3.zero, Quaternion.identity));
        }
    }


    private int[] ConvertNumberToDigitArray(int number)
    {
        int[] digits = new int[damageMaxLength];
        int index = 0;
        while (index < damageMaxLength)
        {
            if (number > 0)
            {
                digits[damageMaxLength - 1 - index] = number % 10;
                number /= 10;
            }
            else
            {
                digits[damageMaxLength - 1 - index] = -1;
            }
            index++;
        }

        return digits;
    }


    private void ReleasePlaceholder(DamagePlaceholder placeholder)
    {
        for (int i = 0; i < placeholder.places.Count; i++)
        {
            placeholder.places[i].digits[placeholder.places[i].digit].SetActive(false);
        }
        placeholder.exclamation.SetActive(false);
        placeholder.underline.SetActive(false);
        placeholders.Enqueue(placeholder);
    }

    protected override bool CheckDependencies()
    {
        InitPool();
        return true;
    }

    protected override void OnDependenciesReady()
    {
        
        return;
    }
}
