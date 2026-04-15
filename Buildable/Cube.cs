using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Buildable
{
    [Space]
    [Space]
    [Space]
    [Header("Localization")]
    public LocalizationKeys nameKey;
    public LocalizationKeys descKey;


    [Space]
    [Space]
    [Space]
    public bool isDestroyable = true;
    public Platform connectedPlatform;
    public List<Platform> childPlatforms = new List<Platform> { };

    public void PlayerDestroy()
    {
        for (int i = 0; i < childPlatforms.Count; i++)
        {
            if (childPlatforms[i].connectedTower)
            {
                childPlatforms[i].connectedTower.PlayerDestroy();
            }
            else if (childPlatforms[i].connectedCube)
            {
                childPlatforms[i].connectedCube.DisconnectCube(this);
            }
        }
        PoolManager.GetInstance().ReleaseBuild(this);
    }

    public void Destroy()
    {
        for (int i = 0; i < childPlatforms.Count; i++)
        {
            if (childPlatforms[i].connectedTower)
            {
                childPlatforms[i].connectedTower.Destroy();
            }
            else if (childPlatforms[i].connectedCube)
            {
                childPlatforms[i].connectedCube.DisconnectCube(this);
            }
        }
        PoolManager.GetInstance().ReleaseBuild(this);
    }

    public Dictionary<string, string> GetSubstitutions(IButtonTextTemplate template)
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string> { };

        if (template is CubeDestroyTextTemplate)
        {
            substitutions.Add(TextMarks.Cost.ToString(), (cost * cashbackPercent).ToString());
        }
        if (template is BuildableTextTemplate)
        {
            substitutions.Add(TextMarks.Cost.ToString(), cost.ToString());
        }


        return substitutions;
    }


    public void DisconnectCube(Cube cube)
    {
        for (int i = 0; i < childPlatforms.Count; i++)
        {
            if (childPlatforms[i].connectedCube is not null && childPlatforms[i].connectedCube == cube)
            {
                childPlatforms[i].connectedCube = null;
            }
        }
    }
}
