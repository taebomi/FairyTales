using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FairyTales.Room
{
    public class RoomObject : MonoBehaviour
    {
        public RoomObjectInitData initData;

        public void ApplyInitData()
        {
            var saveManager = SaveManager.Instance;
            var dataArr = initData.dataArr;
            for (var i = dataArr.Length - 1; i >= 0; i--)
            {
                if (!saveManager.CheckEventCleared(dataArr[i].eventName))
                {
                    continue;
                }

                var willActivate = dataArr[i].willActivate;
                if (willActivate)
                {
                    transform.localPosition = dataArr[i].position;
                }

                gameObject.SetActive(willActivate);
                break;
            }
            
            gameObject.SetActive(!dataArr[0].willActivate);
        }
    }
}