// using System;
// using System.Collections.Generic;
// using DG.Tweening;
// using UnityEngine;
//
// /// <summary>
// /// 풀링할 오브젝트 종류를 enum으로 저장
// /// </summary>
// public enum PoolableObjectName
// {
//     // 플레이어
//     PlayerGhostEffect,
//
//
//     EmotionBox,
//
//
//     #region 알라우네
//
//     AlrauneSpike,
//     AlrauneBullet,
//     AlrauneLaser,
//     AlrauneDanmaku,
//
//     #endregion
// }
//
// public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
// {
//     
//     // 각 오브젝트 풀들이 담긴 딕셔너리
//     private readonly Dictionary<PoolableObjectName, PoolableObject> _originalPrefabDictionary = new();
//     private readonly Dictionary<PoolableObjectName, Stack<PoolableObject>> _objectPoolDictionary = new();
//     private readonly Dictionary<PoolableObjectName, Transform> _parentDictionary = new();
//
//     protected override void AwakeAfter()
//     {
//     }
//
//     /// <summary>
//     /// 오브젝트 풀에 풀링할 오브젝트 추가.
//     /// </summary>
//     /// <param name="poolableObjectName">오브젝트 풀에서 식별하기 위한 이름 Enum</param>
//     /// <param name="originalPrefab">원본 프리팹</param>
//     /// <param name="num">추가할 개수</param>
//     /// <param name="parent">부모 트랜스폼, 설정 안할 시 자동으로 ObjectPool이 부모로 설정됨.</param>
//     public void Add(PoolableObjectName poolableObjectName, PoolableObject originalPrefab, int num, Transform parent = null)
//     {        
//         if (!parent) parent = transform;
//
//         // 오브젝트 풀이 존재하지 않는 경우 추가
//         if (!_originalPrefabDictionary.ContainsKey(poolableObjectName))
//         {
//             _originalPrefabDictionary[poolableObjectName] = originalPrefab;
//             _objectPoolDictionary[poolableObjectName] = new Stack<PoolableObject>();
//         }
//
//
//         // 해당 개수만큼 오브젝트 풀에 추가
//         for (var i = 0; i < num; i++)
//         {
//             var pooledObject = Instantiate(originalPrefab, parent, false);
//             pooledObject.gameObject.SetActive(false);
//             _objectPoolDictionary[poolableObjectName].Push(pooledObject);
//         }
//     }
//
//     /// <summary>
//     /// poolableObjectName을 가진 오브젝트 풀과 원본 딕셔너리 및 풀링중인 오브젝트들 모두 삭제.
//     /// 보스전 끝나는 경우와 같이 쓸모없어지는 경우 실행.
//     /// </summary>
//     /// <param name="poolableObjectName"></param>
//     public void Remove(PoolableObjectName poolableObjectName)
//     {
//         if (_originalPrefabDictionary.ContainsKey(poolableObjectName))
//         {
//             foreach (var poolableObject in _objectPoolDictionary[poolableObjectName])
//             {
//                 Destroy(poolableObject);
//             }
//
//             _objectPoolDictionary.Remove(poolableObjectName);
//             _originalPrefabDictionary.Remove(poolableObjectName);
//         }
//         // 없을 리가 없지만 디버깅 용도
// #if UNITY_EDITOR
//         else
//         {
//             Debug.LogAssertion("해당 풀이 존재하지 않는데 삭제를 시도. 빨리 체크해!!!");
//         }
// #endif
//     }
//
//     
//     
//     
//     
//     public PoolableObject Pop(PoolableObjectName poolableObjectName, Transform parent)
//     {        
//         // 오브젝트 풀에서 꺼내서 파라미터 값에 따른 설정, 없을 경우 오리지널 프리팹으로부터 생성
//         if (_objectPoolDictionary[poolableObjectName].TryPop(out var popObject))
//         {
//             popObject.transform.SetParent(parent);
//         }
//         else
//         {
//             popObject = Instantiate(_originalPrefabDictionary[poolableObjectName], parent);
//         }
//
//         popObject.gameObject.SetActive(true);
//         return popObject;
//     }
//
//     public PoolableObject Pop(PoolableObjectName poolableObjectName, Vector3 position, Quaternion rotation,
//         Transform parent = null, bool worldPositionStays = true)
//     {
//         // 오브젝트 풀에서 꺼내서 파라미터 값에 따른 설정, 없을 경우 오리지널 프리팹으로부터 생성
//         if (_objectPoolDictionary[poolableObjectName].TryPop(out var popObject))
//         {
//             popObject.transform.SetPositionAndRotation(position, rotation);
//         }
//         else
//         {
//             popObject = Instantiate(_originalPrefabDictionary[poolableObjectName], position, rotation);
//         }
//
//         // 부모가 없을 경우 : 아무 일 안함 (그대로 ObjectPool의 자식)
//         // 부모가 있을 경우 : 부모 설정 + woorldPositionStyas값에 따라 position, rotation 부모와 상대설정
//         if (parent)
//         {
//             popObject.transform.SetParent(parent, worldPositionStays);
//         }
//
//         popObject.gameObject.SetActive(true);
//         return popObject;
//     }
//
//     /// <summary>
//     /// 
//     /// </summary>
//     /// <param name="poolableObjectName">풀링할 오브젝트 식별 enum</param>
//     /// <param name="willPooledObject">풀링할 오브젝트</param>
//     public void Push(PoolableObjectName poolableObjectName, PoolableObject willPooledObject)
//     {
//         willPooledObject.gameObject.SetActive(false);
//         if (willPooledObject.transform.parent != transform)
//         {
//             willPooledObject.transform.SetParent(transform);
//         }
//
//         _objectPoolDictionary[poolableObjectName].Push(willPooledObject);
//     }
// }