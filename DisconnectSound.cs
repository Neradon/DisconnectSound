using System;
using System.Collections;
using System.IO;
using ABI_RC.Core.Networking;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System.Linq;

namespace DisconnectSound
{
    public class DisconnectSound : MelonMod
    {
        private AudioSource _audioSource;
        private void Disconnect(object sender, DarkRift.Client.DisconnectedEventArgs e)
        {
            if (e == null) return;
            LoggerInstance.Msg("Playing Disconnected Sound");
            _audioSource.Play();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex == 3)
                MelonCoroutines.Start(InitAudio());
        }
        private IEnumerator InitAudio()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "//DisconnectedSound"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "//DisconnectedSound");

            if (Directory.GetFiles(Directory.GetCurrentDirectory() + "//DisconnectedSound").Length == 0)
            {
                using (WebClient wc = new WebClient())
                    wc.DownloadFile("https://github.com/Edward7s/DisconnectSound/raw/main/Sound/BipSound.mp3", Directory.GetCurrentDirectory() + "//DisconnectedSound//Audio.mp3");
            }
            string path = Directory.GetFiles(Directory.GetCurrentDirectory() + "//DisconnectedSound").FirstOrDefault();

            if (path.EndsWith(".mp3"))
            {
                WWW webReq = new WWW("File://" + path);
                yield return webReq;
                GenerateAudioSource(webReq.GetAudioClip(false, false));
                yield break;
            }
            if (path.EndsWith(".ogg"))
            {
                using (var uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS))
                {
                    yield return uwr.SendWebRequest();
                    if (uwr.isNetworkError || uwr.isHttpError)
                    {
                        LoggerInstance.Error(uwr.error);
                        yield break;
                    }
                    AudioClip testAudio = DownloadHandlerAudioClip.GetContent(uwr);
                    testAudio.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                    GenerateAudioSource(testAudio);
                    yield break;
                }
            }
            LoggerInstance.Warning("Please put a dc.ogg in the UserData folder");
        }

        private void GenerateAudioSource(AudioClip audioClip)
        {
            _audioSource = GameObject.Find("SteamManager").AddComponent<AudioSource>();
            _audioSource.clip = audioClip;
            _audioSource.volume = 0.35f;
            NetworkManager.Instance.GameNetwork.Disconnected += Disconnect;
        }
    }
}