using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShimmerFramework
{
    public class AudioManager : BaseManager<AudioManager>
    {

        private AudioSource musicSources = null;
        private float audioVolume = 1;
        private float musicVolume = 1;

        private GameObject soundGameobject = null;

        private List<AudioSource> audioList = new List<AudioSource>();

        public AudioManager()
        {
            MonoManager.GetInstance().AddUpdateAction(Update);

        }

        private void Update()
        {
            for (int i = 0; i < audioList.Count; i++)
            {
                if (!audioList[i].isPlaying)
                {
                    GameObject.Destroy(audioList[i]);
                    audioList.RemoveAt(i);

                }
            }
        }
        public void ChangeMusicVolume(float volume)
        {
            this.musicVolume = volume;

            if (musicSources == null)
            {
                return;
            }
            musicSources.volume = musicVolume;

        }

        public void PlayBackMusic(string musicName)
        {
            if (musicSources == null)
            {
                GameObject audioPlayer = new GameObject("MusicPlayer");

                audioPlayer.AddComponent<DontDestoryOnLoad>();
                musicSources = audioPlayer.AddComponent<AudioSource>();
            }
            ResourcesManager.GetInstance().LoadAssetAsync<AudioClip>(
#if Addressable
                musicName, 
#else
                "Audio/" + musicName,
#endif
            (clip) =>
            {
                musicSources.clip = clip;
                musicSources.loop = true;
                musicSources.volume = musicVolume;
                musicSources.Play();
            });
        }

        public void StopBackMusic()
        {
            if (musicSources == null)
            {
                return;
            }
            musicSources.Stop();
        }
        public void PauseBackMusic()
        {
            if (musicSources == null)
            {
                return;
            }
            musicSources.Pause();
        }



        public void PlayAudio(string audioName, UnityAction<AudioSource> callback = null)
        {
            if (soundGameobject == null)
            {
                soundGameobject = new GameObject("AudioPlayer");
                soundGameobject.AddComponent<DontDestoryOnLoad>();
            }

            ResourcesManager.GetInstance().LoadAssetAsync<AudioClip>(
#if Addressable
                audioName, 
#else
                "Audio/" + audioName,
#endif
                (clip) =>
                {
                AudioSource audioSource = soundGameobject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.Play();

                audioSource.volume = audioVolume;

                audioList.Add(audioSource);
                if (callback != null)
                {
                    callback(audioSource);
                }
            });
        }
        public void PlayAudio(string audioName, bool isloop, UnityAction<AudioSource> callback = null)
        {
            if (soundGameobject == null)
            {
                soundGameobject = new GameObject("AudioPlayer");
                soundGameobject.AddComponent<DontDestoryOnLoad>();
            }

            ResourcesManager.GetInstance().LoadAssetAsync<AudioClip>("Audio/" +
#if Addressable
                audioName, 
#else
                "Audio/" + audioName,
#endif
            (clip) =>
            {
                AudioSource audioSource = soundGameobject.AddComponent<AudioSource>();
                audioSource.clip = clip;
                audioSource.Play();
                audioSource.loop = isloop;

                audioSource.volume = audioVolume;

                audioList.Add(audioSource);
                if (callback != null)
                {
                    callback(audioSource);
                }
            });
        }

        public void ChangeAudioVolume(float volume)
        {
            this.audioVolume = volume;
            for (int i = 0; i < audioList.Count; i++)
            {
                audioList[i].volume = audioVolume;
            }
        }

        public void StopAudio(AudioSource audioSource)
        {
            if (audioList.Contains(audioSource))
            {
                audioList.Remove(audioSource);
                audioSource.Stop();
                GameObject.Destroy(audioSource);
            }
        }
    }

}

