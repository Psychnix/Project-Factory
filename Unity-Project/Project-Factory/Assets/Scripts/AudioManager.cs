﻿using System;
using UnityEngine;
using System.Collections;
using UnityEngine.PostProcessing;
using UnityEngine.Experimental.UIElements;
using UnityStandardAssets.Characters.FirstPerson;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static Animator animation;
    public TodController todController;
    public static float volume;
    public Sound[] sounds;
    AudioSource dunkelheit;
    AudioSource hintergrund;
    AudioSource saferoom;
    AudioSource generatorStartend;
    public static bool keepFadingIn;
    public static bool keepFadingOut;
    public static AudioManager instance;
    [HideInInspector]
    private FirstPersonController firstPersonController;
    public bool generatorStarted = false;
    [SerializeField] private bool saferoomAktiv = false;
    [SerializeField] private bool dunkelheitAktiv = false;
    [SerializeField] private bool hintergrundAktiv = false;
    [SerializeField] public GameObject fpController;
    public bool wirdSterben = false;
    private PostProcessingBehaviour pPB;
    private static IEnumerator lastCalled;
    public static Transform camera;
    

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
        }
        AudioListener.volume = HauptMenue.volume;
    }

    void Start()
    {
        camera = fpController.GetComponent<Camera>().transform;
        firstPersonController = GameObject.Find("FPSController").GetComponent<FirstPersonController>();
        pPB = FindObjectOfType<PostProcessingBehaviour>();
        MotionBlurModel.Settings MBS = pPB.profile.motionBlur.settings;
        MBS.frameBlending = 0;
        ColorGradingModel.Settings CGS = pPB.profile.colorGrading.settings;
        CGS.tonemapping.neutralBlackIn = 0.02f;
        CGS.tonemapping.neutralBlackOut = 0;
        CGS.basic.contrast = 1;
        CGS.basic.saturation = 1;
        VignetteModel.Settings VS = pPB.profile.vignette.settings;
        VS.opacity = 0;
        ChromaticAberrationModel.Settings CAS = pPB.profile.chromaticAberration.settings;
        CAS.intensity = 0;
        pPB.profile.motionBlur.settings = MBS;
        pPB.profile.colorGrading.settings = CGS;
        pPB.profile.vignette.settings = VS;
        pPB.profile.chromaticAberration.settings = CAS;
        animation = fpController.GetComponent<Animator>();



        hintergrund = Array.Find(sounds, sound => sound.name == "Hintergrund").source;
        dunkelheit = Array.Find(sounds, sound => sound.name == "InDunkelheit").source;
        saferoom = Array.Find(sounds, sound => sound.name == "Saferoom").source;
        generatorStartend = Array.Find(sounds, sound => sound.name == "GeneratorStartend").source;
        Play("Hintergrund", 0.7f);
        hintergrundAktiv = true;
    }


    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameOverCallerMitPP(2f, pPB);
            //FadeCallerMitPP(dunkelheit, hintergrund, saferoom, 0.9f, 5f, true, pPB);
        }
        
        if (generatorStarted && !generatorStartend.isPlaying)
        {
            generatorStarted = false;
            Play("GeneratorLaufend", 0.7f, generatorStartend.transform.position, false);
        }

    }

    public void DunkelheitAktivieren()
    {
        if (lastCalled != null)
        {
            instance.StopCoroutine(lastCalled);
        }
        FadeCallerMitPP(dunkelheit, hintergrund, saferoom, 0.9f, 5f, true, pPB);
        setHintergrund(false);
        setSaferoom(false);
        setDunkelheit(true);
        
    }

    public void DunkelheitAktivierenMitLampe()
    {
        if (lastCalled != null)
        {
            instance.StopCoroutine(lastCalled);
        }
        FadeCallerMitPP(dunkelheit, hintergrund, saferoom, 0.9f, 5f, true, pPB);
    }

    public void SaferoomAktivieren()
    {
        setSaferoom(true);
        //Event: in Saferoom Hitbox
        if (dunkelheitAktiv) {
            if (lastCalled != null)
            {
                instance.StopCoroutine(lastCalled);
            }
            FadeCallerMitPPBackwards(saferoom, hintergrund, dunkelheit, 0.7f, 1.5f, false, pPB);
            Play("Atmung", 0.2f);
            setDunkelheit(false);
        }
        else if(hintergrundAktiv)
        {
            if (lastCalled != null)
            {
                instance.StopCoroutine(lastCalled);
            }
            FadeCaller(saferoom, dunkelheit, hintergrund, 0.7f, 1.5f, false);
            setHintergrund(false);
        }
        else
        {
            if (lastCalled != null)
            {
                instance.StopCoroutine(lastCalled);
            }
            FadeInCaller(saferoom, 0.7f, 1f, false);
        }

    }
    public void HintergrundAktivieren()
    {
        //Event: In normaler Licht Hitbox
        if (dunkelheitAktiv)
        {
            if (lastCalled != null)
            {
                instance.StopCoroutine(lastCalled);
            }
            FadeCallerMitPPBackwards(hintergrund, dunkelheit, saferoom, 0.7f, 3f, false, pPB);
            setDunkelheit(false);
            setHintergrund(true);
        }
        else if (saferoomAktiv)
        {
            if (lastCalled != null)
            {
                instance.StopCoroutine(lastCalled);
            }
            FadeCaller(hintergrund, saferoom, dunkelheit, 0.7f, 3f, false);
            setSaferoom(false);
            setHintergrund(true);
        }
        else
        {
            if (lastCalled != null)
            {
                instance.StopCoroutine(lastCalled);
            }
            FadeInCaller(hintergrund, 0.7f, 1f, false);
            setHintergrund(true);
        }
    }

    public void HintergrundAktivierenMitLampe()
    {
        //Event: In normaler Licht Hitbox
        
            if (lastCalled != null)
            {
                instance.StopCoroutine(lastCalled);
            }
            FadeCallerMitPPBackwards(hintergrund, dunkelheit, saferoom, 0.7f, 3f, false, pPB);
            setHintergrund(true);

    }

    public static void FadeInCaller(AudioSource toFadeIn, float maxVolume, float time, Boolean startNew)
    {
        lastCalled = fadeIn(toFadeIn, maxVolume, time, startNew);
        instance.StartCoroutine(lastCalled);
    }
    public static void FadeOutCaller(AudioSource toFadeOut, float time)
    {
        lastCalled = fadeOut(toFadeOut, time);
        instance.StartCoroutine(lastCalled);
    }
    public static void FadeCaller(AudioSource toFadeIn, AudioSource toFadeOut, float maxVolume, float time, Boolean startNew)
    {
        lastCalled = fadeSounds(toFadeIn, toFadeOut, maxVolume, time, startNew);
        instance.StartCoroutine(lastCalled);
    }
    public static void FadeCaller(AudioSource toFadeIn, AudioSource toFadeOut, AudioSource toFadeOut2, float maxVolume, float time, Boolean startNew)
    {
        lastCalled = fadeSounds(toFadeIn, toFadeOut, toFadeOut2, maxVolume, time, startNew);
        instance.StartCoroutine(lastCalled);
    }
    public static void FadeCallerMitPP(AudioSource toFadeIn, AudioSource toFadeOut, AudioSource toFadeOut2, float maxVolume, float time, Boolean startNew, PostProcessingBehaviour pPB)
    {
        lastCalled = fadeSoundsMitPP(toFadeIn, toFadeOut, toFadeOut2, maxVolume, time, startNew, pPB);
        instance.StartCoroutine(lastCalled);
    }
    public static void GameOverCallerMitPP(float time, PostProcessingBehaviour pPB)
    {
        lastCalled = GameOverMitPP(time, pPB);
        instance.StartCoroutine(lastCalled);
    }
    public static void FadeCallerMitPPBackwards(AudioSource toFadeIn, AudioSource toFadeOut, AudioSource toFadeOut2, float maxVolume, float time, Boolean startNew, PostProcessingBehaviour pPB)
    {
        lastCalled = fadeSoundsMitPPBackwards(toFadeIn, toFadeOut, toFadeOut2, maxVolume, time, startNew, pPB);
        instance.StartCoroutine(lastCalled);
    }
    static IEnumerator fadeIn(AudioSource toFadeIn, float maxVolume, float time, Boolean startNew)
    {
        keepFadingIn = true;
        keepFadingOut = false;
        if (startNew) toFadeIn.Play();

        while (toFadeIn.volume < maxVolume && keepFadingIn)
        {
            toFadeIn.volume += Time.deltaTime / time;
            yield return null;
        }
    }
    static IEnumerator fadeOut(AudioSource toFadeOut, float time)
    {
        keepFadingIn = false;
        keepFadingOut = true;
        float startVolume = toFadeOut.volume;

        while (toFadeOut.volume > 0 && keepFadingOut)
        {
            toFadeOut.volume -= startVolume * Time.deltaTime / time;

            yield return null;
        }
        toFadeOut.Stop();

    }
    static IEnumerator fadeSounds(AudioSource toFadeIn, AudioSource toFadeOut, float maxVolume, float time, Boolean startNew)
    {
        if (startNew || !toFadeIn.isPlaying) toFadeIn.Play();
        float startVolume = toFadeOut.volume;

        while (toFadeIn.volume < maxVolume || toFadeOut.volume > 0)
        {
            if (toFadeIn.volume < maxVolume) toFadeIn.volume += Time.deltaTime / time;
            toFadeOut.volume -= startVolume * Time.deltaTime / time;
            yield return null;
        }
    }
   
    static IEnumerator fadeSoundsMitPP(AudioSource toFadeIn, AudioSource toFadeOut, AudioSource toFadeOut2, float maxVolume, float time, Boolean startNew, PostProcessingBehaviour pPB)
    {
        if (startNew || !toFadeIn.isPlaying) toFadeIn.Play();
        float startVolume = toFadeOut.volume;
        float startVolume2 = toFadeOut2.volume;
        PostProcessingProfile profile = pPB.profile;
        float pufferzeit = 0f;
        float wartezeit = 1f;
        float wartezeitStartwert = wartezeit;
        float PPFadeZeit = 18f;


        while (toFadeIn.volume < maxVolume || toFadeOut.volume > 0 || toFadeOut2.volume > 0)
        {
            if (toFadeIn.volume < maxVolume) toFadeIn.volume += Time.deltaTime / time;
            toFadeOut.volume -= startVolume * Time.deltaTime / time;
            toFadeOut2.volume -= startVolume2 * Time.deltaTime / time;
            yield return null;
        }
        while (pufferzeit > 0)
        {
            pufferzeit -= wartezeitStartwert * Time.deltaTime / wartezeit;
            yield return null;
        }
        while (profile.motionBlur.settings.frameBlending < 1)
        {
            PPMotionBlur(pPB, profile, PPFadeZeit, true);
            PPColorGrading(pPB, profile, PPFadeZeit, true);
            PPVignette(pPB, profile, PPFadeZeit, true);
            PPChromaticAberration(pPB, profile, PPFadeZeit, true);
            yield return null;
        }
        while (pufferzeit < 3f)
        {
            Vector3 startPosition = camera.localPosition;
            pufferzeit += Time.deltaTime;
            camera.localPosition = startPosition + Random.insideUnitSphere* pufferzeit / 40f;
            yield return null;
        }
        while (pufferzeit > 0f)
        {
            Vector3 startPosition = camera.localPosition;
            pufferzeit -= 2 * Time.deltaTime;
            camera.localPosition = startPosition + Random.insideUnitSphere * pufferzeit / 40f;
            yield return null;
        }
        animation.enabled = true;
        instance.wirdSterben = true;
        while (pufferzeit < 2f)
        {
            Vector3 startPosition = camera.localPosition;
            camera.localPosition = startPosition + Random.insideUnitSphere * pufferzeit / 40f;
            PPColorGradingGameOver(pPB, profile, 2f);
            pufferzeit += Time.deltaTime;
            yield return null;
        }
        instance.todController.setTod();

    }

    static IEnumerator GameOverMitPP(float time, PostProcessingBehaviour pPB)
    {
        PostProcessingProfile profile = pPB.profile;
        float pufferzeit = 0f;

        animation.enabled = true;
        instance.wirdSterben = true;
        while (pufferzeit < time)
        {
            Vector3 startPosition = camera.localPosition;
            camera.localPosition = startPosition + Random.insideUnitSphere * pufferzeit / 40f;
            PPColorGradingGameOver(pPB, profile, 2f);
            pufferzeit += Time.deltaTime;
            yield return null;
        }
        instance.todController.setTod();
    }

    static IEnumerator fadeSoundsMitPPBackwards(AudioSource toFadeIn, AudioSource toFadeOut, AudioSource toFadeOut2, float maxVolume, float time, Boolean startNew, PostProcessingBehaviour pPB)
    {
        if (startNew || !toFadeIn.isPlaying) toFadeIn.Play();
        float startVolume = toFadeOut.volume;
        float startVolume2 = toFadeOut2.volume;
        PostProcessingProfile profile = pPB.profile;
        float PPFadeZeit = 1.5f;


        while (toFadeIn.volume < maxVolume || toFadeOut.volume > 0 || toFadeOut2.volume > 0 || profile.motionBlur.settings.frameBlending > 0.05)
        {
            if (toFadeIn.volume < maxVolume) toFadeIn.volume += Time.deltaTime / time;
            toFadeOut.volume -= startVolume * Time.deltaTime / time;
            toFadeOut2.volume -= startVolume2 * Time.deltaTime / time;
            PPMotionBlur(pPB, profile, PPFadeZeit, false);
            PPColorGrading(pPB, profile, PPFadeZeit, false);
            PPVignette(pPB, profile, PPFadeZeit, false);
            PPChromaticAberration(pPB, profile, PPFadeZeit, false);
            yield return null;
        }
        MotionBlurModel.Settings moBlurSettings = profile.motionBlur.settings;
        moBlurSettings.frameBlending = 0;
        profile.motionBlur.settings = moBlurSettings;
    }
    static IEnumerator fadeSounds(AudioSource toFadeIn, AudioSource toFadeOut, AudioSource toFadeOut2, float maxVolume, float time, Boolean startNew)
    {
        if (startNew || !toFadeIn.isPlaying) toFadeIn.Play();
        float startVolume = toFadeOut.volume;
        float startVolume2 = toFadeOut2.volume;

        while (toFadeIn.volume < maxVolume || toFadeOut.volume > 0 || toFadeOut2.volume > 0)
        {
            if (toFadeIn.volume < maxVolume) toFadeIn.volume += Time.deltaTime / time;
            toFadeOut.volume -= startVolume * Time.deltaTime / time;
            toFadeOut2.volume -= startVolume2 * Time.deltaTime / time;
            yield return null;
        }
    }
    public void Play(string name, float volume, Vector3 position, Boolean clipAtPoint)
    {
        
        AudioSource s = Array.Find(sounds, sound => sound.name == name).source;
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.volume = volume;
        if (clipAtPoint)
        {
            AudioSource.PlayClipAtPoint(s.clip, position, volume);
        }
        else
        {
            s.transform.position = position;
            s.Play();
        }
        
    }
    public void Play(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.volume = volume;
        s.source.Play();
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }
    public Sound getSound(string name)
    {
        return Array.Find(sounds, sound => sound.name == name);
    }
    public float getTime(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.source.clip.length;
    }
    public void setPosition(string name, Vector3 position)
    {
        AudioSource s = Array.Find(sounds, sound => sound.name == name).source;
        s.transform.position = position;
    }
    public static void PPMotionBlur(PostProcessingBehaviour pPB, PostProcessingProfile profile, float time, Boolean forward)
    {
        MotionBlurModel.Settings moBlurSettings = profile.motionBlur.settings;
        float startwert = moBlurSettings.frameBlending;
        if (forward)
        {
            moBlurSettings.frameBlending += Time.deltaTime / time;
            profile.motionBlur.settings = moBlurSettings;
        }
        else
        {
            moBlurSettings.frameBlending -= startwert * Time.deltaTime / time;
            profile.motionBlur.settings = moBlurSettings;
        }

    }
    public static void PPColorGrading(PostProcessingBehaviour pPB, PostProcessingProfile profile, float time, Boolean forward)
    {
        ColorGradingModel.Settings colorGradingSettings = profile.colorGrading.settings;
        float saturationStartwert = colorGradingSettings.basic.saturation;
        float contrastStartwert = colorGradingSettings.basic.contrast;
        if (forward)
        {
            if (colorGradingSettings.basic.contrast < 1.7f)
            {
                colorGradingSettings.basic.contrast += Time.deltaTime / time;
            }
            if (colorGradingSettings.basic.saturation > 0.5f)
            {
                colorGradingSettings.basic.saturation -= saturationStartwert * Time.deltaTime / time;
            }
            profile.colorGrading.settings = colorGradingSettings;
        }
        else
        {
            if (colorGradingSettings.basic.contrast > 1f)
            {
                colorGradingSettings.basic.contrast -= contrastStartwert * Time.deltaTime / time;
            }
            if (colorGradingSettings.basic.saturation < 1f)
            {
                colorGradingSettings.basic.saturation += Time.deltaTime / time;
            }
            profile.colorGrading.settings = colorGradingSettings;
        }

    }
    public static void PPColorGradingGameOver(PostProcessingBehaviour pPB, PostProcessingProfile profile, float time)
    {
        ColorGradingModel.Settings colorGradingSettings = profile.colorGrading.settings;
        float blackInStartwert = colorGradingSettings.tonemapping.neutralBlackIn;
        float blackOutStartwert = colorGradingSettings.tonemapping.neutralBlackOut;

        if (colorGradingSettings.tonemapping.neutralBlackIn < 0.07f)
        {
            colorGradingSettings.tonemapping.neutralBlackIn += Time.deltaTime / 5 * time;
        }
        if (colorGradingSettings.tonemapping.neutralBlackOut > -0.06f)
        {
            colorGradingSettings.tonemapping.neutralBlackOut -= blackOutStartwert * Time.deltaTime / 5 * time;
        }
        profile.colorGrading.settings = colorGradingSettings;
    }
    public static void PPVignette(PostProcessingBehaviour pPB, PostProcessingProfile profile, float time, Boolean forward)
    {
        VignetteModel.Settings vignetteSettings = profile.vignette.settings;
        float startwert = vignetteSettings.opacity;
        if (forward)
        {
            vignetteSettings.opacity += Time.deltaTime / time;
            profile.vignette.settings = vignetteSettings;
        }
        else
        {
            vignetteSettings.opacity -= startwert * Time.deltaTime / time;
            profile.vignette.settings = vignetteSettings;
        }

    }
    public static void PPChromaticAberration(PostProcessingBehaviour pPB, PostProcessingProfile profile, float time, Boolean forward)
    {
        ChromaticAberrationModel.Settings chromaticAberrationSettings = profile.chromaticAberration.settings;
        float startwert = chromaticAberrationSettings.intensity;
        if (forward)
        {
            chromaticAberrationSettings.intensity += Time.deltaTime / time;
            profile.chromaticAberration.settings = chromaticAberrationSettings;
        }
        else
        {
            chromaticAberrationSettings.intensity -= startwert * Time.deltaTime / time;
            profile.chromaticAberration.settings = chromaticAberrationSettings;
        }

    }

    public void setSaferoom(bool aktiv)
    {
        saferoomAktiv = aktiv;
    }

    public bool getSaferoom()
    {
        return saferoomAktiv;
    }

    public void setDunkelheit(bool aktiv)
    {
        dunkelheitAktiv = aktiv;
    }

    public bool getDunkelheit()
    {
        return dunkelheitAktiv;
    }

    public void setHintergrund(bool aktiv)
    {
        hintergrundAktiv = aktiv;
    }

    public bool getHintergrund()
    {
        return hintergrundAktiv;
    }

    public PostProcessingBehaviour getPPB()
    {
        return pPB;
    }

    
}
