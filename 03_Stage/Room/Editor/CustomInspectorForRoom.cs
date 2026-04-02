using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Room))]
[CanEditMultipleObjects]
public class CustomInspectorForRoom : Editor
{
    private Vector2 _minLocation;
    private Vector2 _maxLocation;

    private RoomData _roomData;
    private BoxCollider2D _boxCollider2D;

    private void OnEnable()
    {
        _roomData = ((Room) target).ThisRoomData;
        _boxCollider2D = ((Room) target).GetComponent<BoxCollider2D>();

        // 방 정보 받아서 값 대입해둠
        _minLocation = _roomData.position - new Vector2(_roomData.halfWidth, _roomData.halfHeight);
        _maxLocation = _roomData.position + new Vector2(_roomData.halfWidth, _roomData.halfHeight);
    }

    public override void OnInspectorGUI()
    {
        // 기존 UI
        base.OnInspectorGUI();

        // 커스텀
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("빠른 설정");
        // 입력창 만들기, 초기값은 위에 Enable될 때 할당해둔 값
        _minLocation = EditorGUILayout.Vector2Field("최소 좌표", _minLocation);
        _maxLocation = EditorGUILayout.Vector2Field("최대 좌표", _maxLocation);

        // 버튼 생성
        if (GUILayout.Button("설정하기"))
        {
            // 입력한 최소 최대 좌표로부터 중앙 위치와 절반 길이/높이 구함
            var center = (_minLocation + _maxLocation) * 0.5f;
            var halfWidth = Mathf.Abs(_minLocation.x - _maxLocation.x) * 0.5f;
            var halfHeight = Mathf.Abs(_minLocation.y - _maxLocation.y) * 0.5f;

            // 구한 값을 대입해줌
            _roomData.position = center;
            _roomData.halfWidth = halfWidth;
            _roomData.halfHeight = halfHeight;

            _boxCollider2D.transform.position = center;
            _boxCollider2D.size = new Vector2(halfWidth * 2, halfHeight * 2);
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button("▲", GUILayout.Width(45f), GUILayout.Height(45f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button("◀", GUILayout.Width(45f), GUILayout.Height(45f));
        GUILayout.Button("SET", GUILayout.Width(45f), GUILayout.Height(45f));
        GUILayout.Button("▶", GUILayout.Width(45f), GUILayout.Height(45f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button("▼", GUILayout.Width(45f), GUILayout.Height(45f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // todo: 좌표 편하게 옮길수 있게 -1 +1 각 방향으로 해주는 버튼 추가하면 좋을듯
    }


    private void UpdateRoomSize()
    {
        
    }
}