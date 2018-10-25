using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringSounds : MonoBehaviour
{
    public AudioSource onestar;
    public AudioSource twostar;
    public AudioSource threestar;
    public AudioSource fourstar;
    public AudioSource fivestar;

    public AudioSource GetStarTrack(int stars)
    {
        switch(stars)
        {
            case 1: return onestar;
            case 2: return twostar;
            case 3: return threestar;
            case 4: return fourstar;
            case 5: return fivestar;
            default: throw new System.ArgumentException("Cannot have more than 5 stars. Or less than 1. Something is off.");
        }
    }
}
