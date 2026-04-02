// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading;
// using Cysharp.Threading.Tasks;
// using DG.Tweening;
// using FairyTales.EventSystem;
// using FairyTales.Layer;
// using RotaryHeart.Lib.SerializableDictionary;
// using UnityEngine;
// using UnityEngine.Playables;
// using UnityEngine.Timeline;
// using UnityEngine.UI;
//
// namespace FairyTales.Trash
// {
//     public class DeathEventManager : Singleton<DeathEventManager>
//     {
//         [Serializable]
//         public class GameOverEventDictionary : SerializableDictionaryBase<string, GameObject>
//         {
//         }
//
//         [SerializeField, Header("Game Over")] private Canvas gameOverCanvas;
//
//         [SerializeField] private PlayableDirector playableDirector;
//
//         private DeathEventObjectBase _player;
//         private DeathEventObjectBase _currentEnemy;
//         private GameObject _currentEventObject;
//         private Dictionary<string, Animator> _currentBindingAnimatorDictionary;
//
//         protected override void AwakeAfter()
//         {
//             gameOverCanvas.enabled = false;
//
//             _player = PlayerScript.Instance.DeathEventObjectBase;
//         }
//
//         public async UniTaskVoid Begin(DeathEventData eventData, DeathEventObjectBase deathEventableObject = null)
//         {
//             // 게임 일시정지 시키고 카메라 LateUpdate 대기
//             Time.timeScale = 0f;
//             await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
//             InitializeObjects(eventData.eventObjectsPrefab, deathEventableObject);
//
//             // 화면 및 이벤트 대상 오브젝트 Fade
//             await BlinkScreen();
//
//             // 0,0 위치로 옮겨주기
//             ReadyToStartEvent(eventData);
//             await FadeScreen();
//             // 타임라인 바인딩 해주기
//             playableDirector.playableAsset = eventData.timelineAsset;
//             BindToTimeline();
//             GameManager.Instance.SetFading(1f);
//             // 타임라인 재생
//             playableDirector.Play();
//             await GameManager.Instance.FadeOutScreen(0.15f);
//         }
//
//
//         /// <summary>
//         /// 플레이어, 생성될 오브젝트, 적 오브젝트 초기화
//         /// </summary>
//         private void InitializeObjects(GameObject prefab, DeathEventObjectBase deathEventableObject)
//         {
//             _currentBindingAnimatorDictionary = new Dictionary<string, Animator>();
//
//             var overlayLayer = LayerCache.GetLayerInt(LayerName.Overlay);
//
//             _player.ChangeMaterialAndLayer(DeathEventObjectMaterial, overlayLayer);
//             _currentBindingAnimatorDictionary.Add("Player", _player.BindingAnimator);
//
//             _currentEnemy = deathEventableObject;
//             if (_currentEnemy != null)
//             {
//                 _currentEnemy.ChangeMaterialAndLayer(DeathEventObjectMaterial, overlayLayer);
//                 _currentBindingAnimatorDictionary.Add("Enemy", _currentEnemy.BindingAnimator);
//             }
//
//             if (prefab != null)
//             {
//                 if (!GameManager.Instance.IsDebugMode)
//                 {
//                     _currentEventObject = Instantiate(prefab, transform);
//                 }
//                 else
//                 {
//                     _currentEventObject = transform.GetChild(transform.childCount-1).gameObject;
//                 }
//
//                 var currentEventObjectTransform = _currentEventObject.transform;
//                 for (var i = 0; i < currentEventObjectTransform.childCount; i++)
//                 {
//                     var child = currentEventObjectTransform.GetChild(i);
//                     var animator = child.GetComponent<Animator>();
//                     _currentBindingAnimatorDictionary.Add(child.name, animator);
//                 }
//             }
//         }
//
//         private void ReadyToStartEvent(DeathEventData eventData)
//         {
//             var mainCamPos = CameraManager.Instance.MainCamera.transform.position;
//             CameraManager.Instance.ControlOverlayCamera(Vector2.zero, CameraManager.DefaultCameraSize);
//
//             _player.ChangePositionToCenter(mainCamPos);
//             _player.MoveToEventStartPosition(eventData.playerStartData.startPosition);
//
//             if (_currentEnemy != null)
//             {
//                 _currentEnemy.ChangePositionToCenter(mainCamPos);
//                 _currentEnemy.MoveToEventStartPosition(eventData.enemyStartData.startPosition);
//             }
//         }
//
//         private void BindToTimeline()
//         {
//             var timeline = (TimelineAsset)playableDirector.playableAsset;
//             foreach (var track in timeline.GetOutputTracks())
//             {
//                 if (track.GetType() != typeof(AnimationTrack))
//                 {
//                     continue;
//                 }
//
//                 if (!_currentBindingAnimatorDictionary.TryGetValue(track.name, out var animator))
//                 {
//                     Debug.Log(track.name + "인 애니메이터 존재하지 않음.");
//                     continue;
//                 }
//
//                 playableDirector.SetGenericBinding(track, animator);
//             }
//         }
//
//
//         #region 카메라 활성화 애니메이션
//
//         [SerializeField] private RawImage gameOverFadingRawImage;
//         [field:SerializeField] public Material DeathEventObjectMaterial { get; private set; }
//         private static readonly int HitEffectBlendProperty = Shader.PropertyToID("_HitEffectBlend");
//         private float _currentFadingValue;
//
//         private async UniTask BlinkScreen()
//         {
//
//             // 사망 이벤트 보여지도록
//             gameOverCanvas.enabled = true;
//             CameraManager.Instance.ActivateOverlayCamera(true);
//             CameraManager.Instance.SyncOverlayCameraToMainCamera();
//
//             SetFadingValue(1f);
//             await UniTask.Delay(150, true);
//             SetFadingValue(0f);
//             await UniTask.Delay(150, true);
//             SetFadingValue(1f);
//             await UniTask.Delay(150, true);
//             SetFadingValue(0f);
//             await UniTask.Delay(300, true);
//         }
//
//         private async UniTask FadeScreen()
//         {
//             var fadingCts = new CancellationTokenSource();
//
//             SyncFadingValue(fadingCts).Forget();
//             
//             await DOTween.To(x => _currentFadingValue = x, 0f, 1f, 0.75f)
//                 .SetUpdate(true).Play();
//
//             fadingCts.Cancel();
//             fadingCts.Dispose();
//
//             DeathEventObjectMaterial.SetFloat(HitEffectBlendProperty, _currentFadingValue);
//             gameOverFadingRawImage.color = new Color(0f, 0f, 0f, _currentFadingValue);
//         }
//
//         private async UniTaskVoid SyncFadingValue(CancellationTokenSource cts)
//         {
//             while (true)
//             {
//                 SetFadingValue(_currentFadingValue);
//                 await UniTask.Yield(cts.Token);
//             }
//         }
//
//         private void SetFadingValue(float value)
//         {
//             DeathEventObjectMaterial.SetFloat(HitEffectBlendProperty, value);
//             gameOverFadingRawImage.color = new Color(0f, 0f, 0f, value);
//         }
//
//         #endregion
//
//
//         #region 타임라인 바인딩
//
//         private void BindTrack()
//         {
//         }
//
//         #endregion
//     }
// }