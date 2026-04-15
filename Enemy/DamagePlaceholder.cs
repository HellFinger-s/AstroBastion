using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class DamagePlaceholder : MonoBehaviour
{
    public GameObject exclamation;
    public GameObject underline;

    public List<DigitPlace> places;
    public float lifetime = 1f;
    public float elapsed;
    public float startScale = 1f;
    public bool alive => elapsed < lifetime;

    public void Init(Vector3 pos, DamageVisType visType, bool isCrit)
    {
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        elapsed = 0f;
        
        if (isCrit)
        {
            exclamation.SetActive(true);
            startScale = 3f;
        }

        if (visType == DamageVisType.ShieldedDamage)
        {
            underline.SetActive(true);
        }
    }

    public bool UpdateAndCheckAlive(Transform target)
    {
        elapsed += Time.deltaTime;
        if (!alive) return true;

        transform.LookAt(target.position, target.up);

        float t = elapsed / lifetime;
        float height = Mathf.Lerp(0.5f, 0f, t);
        float sway = Mathf.Sin(elapsed * 8f) * 0.1f;

        transform.position += transform.up * height * Time.deltaTime;
        transform.Rotate(transform.forward, sway);

        transform.localScale = Vector3.one * (startScale - t);

        return false;
    }
}
