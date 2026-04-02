using UnityEngine;

public class StrayAntNPC : StageObject
{

    private void OnEnable()
    {
        UpdateEventProgress();
    }
    
    //플레이어가 근처에서 대화 시도할경우
    public void Interact()
    {
        // if (_event05.isActiveAndEnabled)
        // {
        //     // RemoveEmotionBox();
        //     // EventManager.Instance.InitiateEvent(_event05);
        // }
        // else if (_event06.isActiveAndEnabled)
        // {
        //     // RemoveEmotionBox();
        //     // EventManager.Instance.InitiateEvent(_event06);
        // }
        // else if (_event07.isActiveAndEnabled)
        // {
        //     // EventManager.Instance.InitiateEvent(_event07);
        // }
    }

    public void UpdateEventProgress()
    {
        // if (!GameManager.Instance.PlayerSaveData.stage01EventSaveData[5].Cleared)
        // {
        //     transform.localPosition = new Vector3(-9.5f, 0.75f, 0f);
        //     _event05.gameObject.SetActive(true);
        //     _event06.gameObject.SetActive(false);
        //     _event07.gameObject.SetActive(false);
        // }
        // else if (!GameManager.Instance.PlayerSaveData.stage01EventSaveData[6].Cleared)
        // {
        //     transform.localPosition = new Vector3(5.25f, 3.7f, 0f);
        //     _event05.gameObject.SetActive(false);
        //     _event06.gameObject.SetActive(true);
        //     _event07.gameObject.SetActive(false);
        // } 
        // else if (!GameManager.Instance.PlayerSaveData.stage01EventSaveData[7].Cleared)
        // {
        //     transform.localPosition = new Vector3(5.25f, 3.7f, 0f);
        //     _event05.gameObject.SetActive(false);
        //     _event06.gameObject.SetActive(false);
        //     _event07.gameObject.SetActive(true);
        // }
    }
}
