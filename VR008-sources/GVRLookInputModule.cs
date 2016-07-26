using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

// original >> https://forums.oculus.com/community/discussion/comment/219978/#Comment_219978

public class GVRLookInputModule : BaseInputModule {

	// public const int kLookId = -3;
	// public string submitButtonName = "Fire1";
	public string controlAxisName = "Horizontal";
	private PointerEventData lookData;

	private PointerEventData GetLookPointerEventData() {
		Vector2 lookPosition;
		lookPosition.x = Screen.width/2;
		lookPosition.y = Screen.height/2;
		if (lookData == null) {
			lookData = new PointerEventData(eventSystem);
		}
		lookData.Reset();
		lookData.delta = Vector2.zero;
		lookData.position = lookPosition;
		lookData.scrollDelta = Vector2.zero;
		eventSystem.RaycastAll(lookData, m_RaycastResultCache);
		lookData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
		m_RaycastResultCache.Clear();
		return lookData;
	}

	private bool SendUpdateEventToSelectedObject() {
		if (eventSystem.currentSelectedGameObject == null)
			return false;
		BaseEventData data = GetBaseEventData ();
		ExecuteEvents.Execute (eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
		return data.used;
	}

	public override void Process() {
		// send update events if there is a selected object (important)
		SendUpdateEventToSelectedObject();
		PointerEventData lookData = GetLookPointerEventData();
		// use built-in enter/exit highlight handler
		HandlePointerExitAndEnter(lookData,lookData.pointerCurrentRaycast.gameObject);
		if (Input.GetMouseButtonDown (0)) {
		// if (Input.GetButtonDown (submitButtonName)) {
			// eventSystem.SetSelectedGameObject(null);
			if (lookData.pointerCurrentRaycast.gameObject != null) {
				GameObject go = lookData.pointerCurrentRaycast.gameObject;
				GameObject newPressed = ExecuteEvents.ExecuteHierarchy (go, lookData, ExecuteEvents.submitHandler);
				if (newPressed == null) {
					// submit handler not found, try select handler instead
					newPressed = ExecuteEvents.ExecuteHierarchy (go, lookData, ExecuteEvents.selectHandler);
				}
				if (newPressed != null) {
					// eventSystem.SetSelectedGameObject(newPressed);
				}
			}
		}
		if (eventSystem.currentSelectedGameObject && controlAxisName != null && controlAxisName != "") {
			float newVal = Input.GetAxis (controlAxisName);
			if (newVal > 0.01f || newVal < -0.01f) {
				AxisEventData axisData = GetAxisEventData(newVal,0.0f,0.0f);
				ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisData, ExecuteEvents.moveHandler);
			}
		}
	}	
}
