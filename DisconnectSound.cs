using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ABI_RC.Core.Networking;
using DarkRift.Client;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;

using MelonLoader;
using System;
using System.Collections;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace DisconnectSound
{
    public class DisconnectSound : MelonMod
    {
        private AudioSource _audioSource;
        private void Test(object sender, DisconnectedEventArgs e)
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


            if (File.Exists("UserData/dc.ogg"))
            {
                using (var uwr = UnityWebRequestMultimedia.GetAudioClip($"file://{Path.Combine(Environment.CurrentDirectory, "UserData/dc.ogg")}", AudioType.OGGVORBIS)) {
                    yield return uwr.SendWebRequest();
                    if (uwr.isNetworkError || uwr.isHttpError) {
                        Debug.LogError(uwr.error);
                        yield break;
                    }
                    AudioClip testAudio = DownloadHandlerAudioClip.GetContent(uwr);
                    testAudio.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                    _audioSource = GameObject.Find("SteamManager").AddComponent<AudioSource>();
                    _audioSource.clip = testAudio;
                    _audioSource.volume = 0.35f;
                    NetworkManager.Instance.GameNetwork.Disconnected += Test;
                }
            }
            else
            {
                LoggerInstance.Warning("Please put a dc.ogg in the UserData folder");
            }
            

                
                

        }

    }
}