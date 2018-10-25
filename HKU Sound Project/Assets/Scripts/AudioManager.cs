using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource leftTrack;
    public AudioSource rightTrack;
    public AudioSource upTrack;
    public TutorialSounds tutSounds;
    public ScoringSounds scoringSounds;
    public CommentarySounds commentarySounds;
    public bool distortOnWrong;
    public bool pitchOnWrong;
    AudioDistortionFilter leftDis;
    AudioDistortionFilter rightDis;
    AudioDistortionFilter upDis;
    public AudioSource upBeep;
    public float upVolume;
    public float leftVolume;
    public float rightVolume;
    float leftBase;
    float rightBase;
    float upBase;
    public long score;
    float timer, totalTimer;
    public int tier = 0;
    int lastPick = 0;
    bool tutorial = true; bool end = false;
    int tutorialProgress = 0; int tutorialVoice = 0; int endProgress = 0;

    // Use this for initialization
    void Start()
    {
        //Tutorial();
        timer = 0.0f; totalTimer = 0.0f; upBase = 0.0f; leftBase = 0.0f; rightBase = 0.0f;
        leftTrack.volume = leftBase; upTrack.volume = upBase; rightTrack.volume = rightBase;
        leftTrack.Play(); rightTrack.Play(); upTrack.Play();
        leftDis = leftTrack.GetComponent<AudioDistortionFilter>();
        rightDis = rightTrack.GetComponent<AudioDistortionFilter>();
        upDis = upTrack.GetComponent<AudioDistortionFilter>();
        leftDis.distortionLevel = 0.0f; rightDis.distortionLevel = 0.0f; upDis.distortionLevel = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!tutorial && !end)
        {
            totalTimer += Time.fixedDeltaTime;
            SetStates();
            HandleInput();
            Scoring();
            if (totalTimer > 90.0f) { end = true; }
        }
        if(tutorial)
        {
            Tutorial();
        }
        if(end)
        {
            End();
        }
        upVolume = upTrack.volume; leftVolume = leftTrack.volume; rightVolume = rightTrack.volume;
    }

    void SetStates() //Determines which tracks are playing and which are not, and how long. Game should switch faster with higher scores, and get more difficult combinations
    {

        ///possible configurations, in tracks playing, in order of difficulty:
        ///
        ///Tier one:
        ///left, right, up, down (player must not press anything)
        ///left, right, up
        ///left, right, down
        ///right, up, down
        ///left, up, down
        ///
        ///Tier two:
        ///left, right, up, down (player must not press anything)
        ///left, right, up
        ///left, right, down
        ///right, up, down
        ///left, up, down
        ///up, down
        ///left, right
        ///
        ///Tier three (basic ones become less common):
        ///left, right, up, down (player must not press anything)
        ///left, right, up
        ///left, right, down
        ///right, up, down
        ///left, up, down
        ///up, down
        ///left, right
        ///up, left
        ///up, right
        ///down, left
        ///down, right

        timer -= Time.fixedDeltaTime;

        if (timer < 0)
        {
            timer = Mathf.Max(1f + Random.Range(1.0f, 1.50f), Random.Range(7.00f - score / 2500, 8.50f - score / 2500));
            tier = Mathf.Min((int)score / 2500, 3);
            int finalTier = 0;

            switch (tier)
            {
                case 0: finalTier = 0; break;
                case 1: finalTier = Random.Range(0, 3); break;
                case 2: finalTier = Random.Range(0, 4); break;
                case 3: finalTier = Random.Range(1, 4); break;
            }

            switch (finalTier)
            {
                case 0:
                case 1:
                    TierOneVolumes(); break;
                case 2:
                case 3:
                    TierTwoVolumes(); break;
            }

            upBeep.Play();
        }
    }

    void TierOneVolumes()
    {
        int pick = MyRandom.NoRepeatRange(0, 4, lastPick);
        lastPick = pick;

        switch (pick)
        {
            case 0: //all playing
                leftBase = 0.5f; rightBase = 0.5f; upBase = 0.5f;
                break;
            case 1: //left not playing
                leftBase = 0.0f; rightBase = 0.5f; upBase = 0.5f;
                break;
            case 2: //right not playing
                leftBase = 0.5f; rightBase = 0.0f; upBase = 0.5f;
                break;
            case 3: //up not playing
                leftBase = 0.5f; rightBase = 0.5f; upBase = 0.0f;
                break;
        }
    }

    void TierTwoVolumes()
    {
        int pick = MyRandom.NoRepeatRange(0, 2, lastPick);
        lastPick = pick;

        switch (pick)
        {
            case 0: //left, right not playing
                leftBase = 0.0f; rightBase = 0.0f; upBase = 0.5f;
                break;
            case 1: //up, left not playing
                leftBase = 0.0f; rightBase = 0.5f; upBase = 0.0f;
                break;
            case 2: //up, right not splaying
                leftBase = 0.5f; rightBase = 0.0f; upBase = 0.0f;
                break;
        }
    }

    void Scoring()
    {
        if (leftTrack.volume == 0.5f && upTrack.volume == 0.5f && rightTrack.volume == 0.5f)
        {
            score += 1;
        }
    }

    void HandleInput()
    {
        if (Input.GetButton("Up")) { upTrack.volume = upBase + 0.5f; } else { upTrack.volume = upBase; }
        if (Input.GetButton("Left")) { leftTrack.volume = leftBase + 0.5f; } else { leftTrack.volume = leftBase; }
        if (Input.GetButton("Right")) { rightTrack.volume = rightBase + 0.5f; } else { rightTrack.volume = rightBase; }

        if (distortOnWrong)
        {
            if (upTrack.volume > 0.5f) { upDis.distortionLevel = 0.75f; } else { upDis.distortionLevel = 0.0f; }
            if (leftTrack.volume > 0.5f) { leftDis.distortionLevel = 0.75f; } else { leftDis.distortionLevel = 0.0f; }
            if (rightTrack.volume > 0.5f) { rightDis.distortionLevel = 0.75f; } else { rightDis.distortionLevel = 0.0f; }
        }

        if (pitchOnWrong)
        {
            if (upTrack.volume > 0.5f) { upTrack.pitch = 1.25f; } else { upTrack.pitch = 1.0f; }
            if (rightTrack.volume > 0.5f) { rightTrack.pitch = 1.25f; } else { rightTrack.pitch = 1.0f; }
            if (leftTrack.volume > 0.5f) { leftTrack.pitch = 1.25f; } else { leftTrack.pitch = 1.0f; }
        }
    }

    void Tutorial()
    {
        switch (tutorialProgress)
        {
            case 0: TutorialLeft(); break;
            case 1: TutorialRight(); break;
            case 2: TutorialUp(); break;
            case 3:
                timer = 0.0f;
                //Play voice clip "Keep the music smooth!"
                tutSounds.tutorial4.Play();
                tutorialProgress++;
                break;
            case 4:
                leftBase = 0.5f; rightBase = 0.5f; upBase = 0.5f;
                leftTrack.volume = leftBase; rightTrack.volume = rightBase; upTrack.volume = upBase;
                timer += Time.fixedDeltaTime;
                if (!tutSounds.tutorial4.isPlaying) { tutorial = false; timer = 0.0f; tutSounds.tutorial5.Play(); }
                break;
        }
    }

    void TutorialLeft()
    {
        if(tutorialVoice <1)
        {
            tutorialVoice = 1;
            //Play voice clip "Hold Left to play the guitar!"
            tutSounds.tutorial1.Play();
        }

        if(Input.GetButton("Left"))
        {
            timer += Time.fixedDeltaTime;
            leftTrack.volume = leftBase + 0.5f;
        }
        else
        { leftTrack.volume = leftBase; }

        if(timer>3.0f) { timer = 0; tutorialProgress++; leftTrack.volume = leftBase; }
    }

    void TutorialRight()
    {
        if (tutorialVoice < 2)
        {
            tutorialVoice = 2;
            //Play voice clip "Hold Right to play the drums!"
            tutSounds.tutorial2.Play();

        }

        if (Input.GetButton("Right"))
        {
            timer += Time.fixedDeltaTime;
            rightTrack.volume = rightBase + 0.5f;
        }
        else
        { rightTrack.volume = rightBase; }

        if (timer > 3.0f) { timer = 0; tutorialProgress++; rightTrack.volume = rightBase; }
    }

    void TutorialUp()
    {
        if (tutorialVoice < 3)
        {
            tutorialVoice = 3;
            //Play voice clip "Hold Up to play the piano!"
            tutSounds.tutorial3.Play();
        }

        if (Input.GetButton("Up"))
        {
            timer += Time.fixedDeltaTime;
            upTrack.volume = upBase + 0.5f;
        }
        else
        { upTrack.volume = upBase; }

        if (timer > 3.0f) { timer = 0; tutorialProgress++; upTrack.volume = upBase; }
    }

    void End()
    {
        leftTrack.volume = 0.5f; upTrack.volume = 0.5f; rightTrack.volume = 0.5f;
        //Max score (but pretty much impossible to get: 18000
        //1 star: 1000 (5 seconds correct)
        //2 stars: 3000
        //3 stars: 5000
        //4 stars: 10000
        //5 stars: 15000

        if (endProgress < 1)
        {
            endProgress = 1;
            int stars = 0;
            if (score >= 0)
            { stars++; }
            if (score >= 1000)
            { stars++; }
            if (score >= 3000)
            { stars++; }
            if (score >= 7500)
            { stars++; }
            if (score >= 12000)
            { stars++; }

            Debug.Log("You got " + stars + " stars!"); //Needs soundline
            scoringSounds.GetStarTrack(stars).Play();
        }

        if(endProgress <2)
        {
            if (true)  //has to become "if voiceline has stopped playing"
            {
                //Soundline "Press left to play again!"
                 endProgress = 2;
            }
        }

        if (endProgress < 3)
        {
            if (true)  //has to become "if voiceline has stopped playing"
            {
                //Soundline "Press right to quit!"
                endProgress = 3;
            }
        }

        if(endProgress <4)
        {
            if(Input.GetButton("Left"))
            {
                //end = false; endProgress = 0; tutorial = true; tutorialProgress = 3; totalTimer = 0.0f; return;
            }

            if(Input.GetButton("Right"))
            {
                //Whatever
            }
        }

    }
}