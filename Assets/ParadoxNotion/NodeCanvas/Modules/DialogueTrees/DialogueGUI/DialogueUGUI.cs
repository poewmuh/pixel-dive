using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Gameplay.Dialogs;
using _Project.Gameplay.Dialogs.Core;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace NodeCanvas.DialogueTrees.UI.Examples
{

    public class DialogueUGUI : MonoBehaviour
    {
        [SerializeField] private LocalizedString _mermaidActor;
        [SerializeField] private EmotionsSpriteConfig _spriteConfig;
        [SerializeField] private Animator _animator;
        [SerializeField] private TMPTextAnimator _textAnimator;
        
        public Locales language;

        [System.Serializable]
        public class SubtitleDelays
        {
            public float characterDelay = 0.05f;
            public float sentenceDelay = 0.5f;
            public float commaDelay = 0.1f;
            public float finalDelay = 1.2f;
        }

        //Options...
        [Header("Input Options")]
        public bool skipOnInput;
        public bool waitForInput;

        //Group...
        [Header("Subtitles")]
        public RectTransform subtitlesGroup;
        public TextMeshProUGUI actorSpeech;
        public TextMeshProUGUI actorName;
        public Image actorPortrait;
        public RectTransform waitInputIndicator;
        public SubtitleDelays subtitleDelays = new SubtitleDelays();
        public List<AudioClip> typingSounds;

        //Group...
        [Header("Multiple Choice")]
        public RectTransform optionsGroup;
        public Button optionButton;
        private Dictionary<Button, int> cachedButtons;
        private Vector2 originalSubsPosition;
        private bool isWaitingChoice;
        private bool isStartedByGraph;
        private AudioSource _sfxAudioSource;

        public void SetAnyKeyDown() => anyKeyDown = true;
        public void SetupAudioSource(AudioSource audioSource) => _sfxAudioSource = audioSource;
        private bool anyKeyDown;
        void LateUpdate() => anyKeyDown = false;


        void Awake() { Subscribe(); Hide(); }
        void OnEnable() { UnSubscribe(); Subscribe(); }
        void OnDisable() { UnSubscribe(); }

        void Subscribe() {
            DialogueTree.OnDialogueStarted += OnDialogueStarted;
            DialogueTree.OnDialoguePaused += OnDialoguePaused;
            DialogueTree.OnDialogueFinished += OnDialogueFinished;
            DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
            DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;
        }

        void UnSubscribe() {
            DialogueTree.OnDialogueStarted -= OnDialogueStarted;
            DialogueTree.OnDialoguePaused -= OnDialoguePaused;
            DialogueTree.OnDialogueFinished -= OnDialogueFinished;
            DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest;
            DialogueTree.OnMultipleChoiceRequest -= OnMultipleChoiceRequest;
        }

        void Hide() {
            subtitlesGroup.gameObject.SetActive(false);
            optionsGroup.gameObject.SetActive(false);
            optionButton.gameObject.SetActive(false);
            originalSubsPosition = subtitlesGroup.transform.position;
        }

        void OnDialogueStarted(DialogueTree dlg)
        {
            isStartedByGraph = true;
        }

        void OnDialoguePaused(DialogueTree dlg) {
            subtitlesGroup.gameObject.SetActive(false);
            optionsGroup.gameObject.SetActive(false);
            StopAllCoroutines();
        }

        void OnDialogueFinished(DialogueTree dlg) {
            subtitlesGroup.gameObject.SetActive(false);
            optionsGroup.gameObject.SetActive(false);
            if ( cachedButtons != null ) {
                foreach ( var tempBtn in cachedButtons.Keys ) {
                    if ( tempBtn != null ) {
                        Destroy(tempBtn.gameObject);
                    }
                }
                cachedButtons = null;
            }
            StopAllCoroutines();
        }

        ///----------------------------------------------------------------------------------------------

        void OnSubtitlesRequest(SubtitlesRequestInfo info) {
            StartCoroutine(Internal_OnSubtitlesRequestInfo(info));
        }

        IEnumerator Internal_OnSubtitlesRequestInfo(SubtitlesRequestInfo info) {

            var text = info.statement.GetLocalizedText(language);
            var audio = info.statement.GetLocalizedAudio(language);
            var portraitSprite = _spriteConfig.GetSpriteForEmotion(info.statement.emotionType);

            subtitlesGroup.gameObject.SetActive(true);
            subtitlesGroup.position = originalSubsPosition;
            actorSpeech.text = "";
            _animator.Play("ChangeDialog");
            if (info.statement.emotionType is EmotionType.VeryAngry)
            {
                UniTask.WaitForSeconds(1.3f).ContinueWith(() => { _animator.Play("Angry"); });
            }

            actorName.text = _mermaidActor.GetLocalizedString();
            actorSpeech.color = Color.antiqueWhite;

            actorPortrait.gameObject.SetActive(portraitSprite != null);
            actorPortrait.sprite = portraitSprite;

            if ( audio != null ) {
                actorSpeech.text = text;
                var timer = 0f;
                while ( timer < audio.length ) {
                    if ( skipOnInput && anyKeyDown ) {
                        break;
                    }
                    timer += Time.deltaTime;
                    yield return null;
                }
            }

            if ( audio == null ) {
                var tempText = string.Empty;
                var inputDown = false;
                if ( skipOnInput ) {
                    StartCoroutine(CheckInput(() => { inputDown = true; }));
                }

                for ( int i = 0; i < text.Length; i++ ) {

                    if ( skipOnInput && inputDown ) {
                        actorSpeech.text = text;
                        yield return null;
                        break;
                    }

                    if ( subtitlesGroup.gameObject.activeSelf == false ) {
                        yield break;
                    }

                    char c = text[i];
                    tempText += c;
                    yield return StartCoroutine(DelayPrint(subtitleDelays.characterDelay));
                    PlayTypeSound();
                    if ( c == '.' || c == '!' || c == '?' ) {
                        yield return StartCoroutine(DelayPrint(subtitleDelays.sentenceDelay));
                        PlayTypeSound();
                    }
                    if ( c == ',' ) {
                        yield return StartCoroutine(DelayPrint(subtitleDelays.commaDelay));
                        PlayTypeSound();
                    }

                    actorSpeech.text = tempText;
                    _textAnimator?.AnimateLastCharacter();
                }

                if ( !waitForInput ) {
                    yield return StartCoroutine(DelayPrint(subtitleDelays.finalDelay));
                }
            }

            if ( waitForInput ) {
                while ( !anyKeyDown ) {
                    yield return null;
                }
            }

            yield return null;
            subtitlesGroup.gameObject.SetActive(false);
            info.Continue();
        }

        void PlayTypeSound() {
            if ( typingSounds.Count > 0 ) {
                var sound = typingSounds[Random.Range(0, typingSounds.Count)];
                if ( sound != null ) {
                    _sfxAudioSource.PlayOneShot(sound, Random.Range(0.6f, 1f));
                }
            }
        }

        IEnumerator CheckInput(System.Action Do) {
            while ( !anyKeyDown ) {
                yield return null;
            }
            Do();
        }

        IEnumerator DelayPrint(float time) {
            var timer = 0f;
            while ( timer < time ) {
                timer += Time.deltaTime;
                yield return null;
            }
        }

        ///----------------------------------------------------------------------------------------------

        void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info) {

            optionsGroup.gameObject.SetActive(true);
            var buttonHeight = optionButton.GetComponent<RectTransform>().rect.height;
            optionsGroup.sizeDelta = new Vector2(optionsGroup.sizeDelta.x, ( info.options.Values.Count * buttonHeight ) + 20);

            cachedButtons = new Dictionary<Button, int>();
            int i = 0;

            foreach ( KeyValuePair<IStatement, int> pair in info.options ) {
                var btn = (Button)Instantiate(optionButton);
                btn.gameObject.SetActive(true);
                btn.transform.SetParent(optionsGroup.transform, false);
                btn.transform.localPosition = (Vector3)optionButton.transform.localPosition - new Vector3(0, buttonHeight * i, 0);
                btn.GetComponentInChildren<Text>().text = pair.Key.GetLocalizedText(language);
                cachedButtons.Add(btn, pair.Value);
                btn.onClick.AddListener(() => { Finalize(info, cachedButtons[btn]); });
                i++;
            }

            if ( info.showLastStatement ) {
                subtitlesGroup.gameObject.SetActive(true);
                var newY = optionsGroup.anchoredPosition.y + optionsGroup.sizeDelta.y + 1;
                subtitlesGroup.anchoredPosition = new Vector2(subtitlesGroup.anchoredPosition.x, newY);
            }

            if ( info.availableTime > 0 ) {
                StartCoroutine(CountDown(info));
            }
        }

        IEnumerator CountDown(MultipleChoiceRequestInfo info) {
            isWaitingChoice = true;
            var timer = 0f;
            while ( timer < info.availableTime ) {
                if ( isWaitingChoice == false ) {
                    yield break;
                }
                timer += Time.deltaTime;
                SetMassAlpha(optionsGroup, Mathf.Lerp(1, 0, timer / info.availableTime));
                yield return null;
            }

            if ( isWaitingChoice ) {
                Finalize(info, info.options.Values.Last());
            }
        }

        void Finalize(MultipleChoiceRequestInfo info, int index) {
            isWaitingChoice = false;
            SetMassAlpha(optionsGroup, 1f);
            optionsGroup.gameObject.SetActive(false);
            subtitlesGroup.gameObject.SetActive(false);
            foreach ( var tempBtn in cachedButtons.Keys ) {
                Destroy(tempBtn.gameObject);
            }
            info.SelectOption(index);
        }

        void SetMassAlpha(RectTransform root, float alpha) {
            foreach ( var graphic in root.GetComponentsInChildren<CanvasRenderer>() ) {
                graphic.SetAlpha(alpha);
            }
        }
    }
}