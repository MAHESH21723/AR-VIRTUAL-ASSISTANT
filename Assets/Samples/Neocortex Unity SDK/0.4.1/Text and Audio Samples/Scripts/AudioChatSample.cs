using UnityEngine;
using Neocortex.Data;
using UnityEngine.UI;

namespace Neocortex.Samples
{
    public class AudioChatSample : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        [Header("Neocortex Components")]
        [SerializeField] private AudioReceiver audioReceiver;
        [SerializeField] private NeocortexSmartAgent agent;
        [SerializeField] private NeocortexThinkingIndicator thinking;
        [SerializeField] private NeocortexAudioChatInput audioChatInput;

        [SerializeField] private Animator animator;

        [Header("UI Button")]
        [SerializeField] private Button micButton;
        [SerializeField] private Image micIcon;
        [SerializeField] private Sprite micOnSprite;
        [SerializeField] private Sprite micOffSprite;

        private bool isListening = false;

        private void Start()
        {
            agent.OnChatResponseReceived.AddListener(OnChatResponseReceived);
            agent.OnAudioResponseReceived.AddListener(OnAudioResponseReceived);
            audioReceiver.OnAudioRecorded.AddListener(OnAudioRecorded);

            if (micButton != null)
                micButton.onClick.AddListener(ToggleMic);

            // Mic won't start automatically anymore
        }

        public void ToggleMic()
        {
            if (!isListening)
            {
                StartMicrophone();
            }
            else
            {
                StopMicrophone();
            }
        }

        private void StartMicrophone()
        {
            audioReceiver.StartMicrophone();
            isListening = true;
            if (micIcon && micOnSprite) micIcon.sprite = micOnSprite;
            Debug.Log("Microphone started");
        }

        private void StopMicrophone()
        {
            audioReceiver.StopMicrophone();
            isListening = false;
            if (micIcon && micOffSprite) micIcon.sprite = micOffSprite;
            Debug.Log("Microphone stopped");
        }

        private void OnAudioRecorded(AudioClip clip)
        {
            agent.AudioToAudio(clip);
            thinking.Display(true);
            audioChatInput.SetChatState(false);
        }

        private void OnChatResponseReceived(ChatResponse response)
        {
            string action = response.action;
            if (!string.IsNullOrEmpty(action))
            {
                if (action == "DANCE")
                {
                    animator.SetTrigger("dancing");
                }
                else if (action == "BACKFLIP")
                {
                    animator.SetTrigger("backflip");
                }
            }

            Emotions emotion = response.emotion;
            if (emotion != Emotions.Neutral)
            {
                switch (emotion)
                {
                    case Emotions.Happy: animator.SetTrigger("happy"); break;
                    case Emotions.Angry: animator.SetTrigger("angry"); break;
                    case Emotions.Upset: animator.SetTrigger("upset"); break;
                    case Emotions.Amazed: animator.SetTrigger("amazed"); break;
                    case Emotions.Alarmed: animator.SetTrigger("alarmed"); break;
                    case Emotions.Fascinated: animator.SetTrigger("fascinated"); break;
                    case Emotions.Confident: animator.SetTrigger("confident"); break;
                    case Emotions.Scared: animator.SetTrigger("scared"); break;
                    default:
                        animator.SetTrigger(Random.Range(0, 1) > 0.5 ? "talking 1" : "talking 2");
                        break;
                }
            }
        }

        private void OnAudioResponseReceived(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();

            // Don’t auto-restart mic after reply

           

            thinking.Display(false);
            audioChatInput.SetChatState(true);
        }
    }
}
