using System.Collections;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class FadeObjectEvan : MonoBehaviour {
    public float timeToFade;
    public bool isTransparent = false;
    public bool fadeInOnEnable = false;
    public bool invokeEventAfterFadeIn = false;
    public bool invokeEventAfterFadeOut = false;
    public bool isButton = false;
    public UnityEngine.Events.UnityEvent eventsToInvokeIn;
    public UnityEngine.Events.UnityEvent eventsToInvokeOut;
    public float percentComplete = 0;

    private void OnEnable()
    {
        if (fadeInOnEnable)
            FadeInObject();
    }

    internal Material materialToFade
    {   
        get
        {
            if (_materialToFade == null || !Application.isPlaying)
                _materialToFade = GetComponent<MeshRenderer>().sharedMaterial;
            return _materialToFade;
        }
    }
    private Material _materialToFade;

    internal CanvasGroup canvasGroup
    {
        get {
            if (_canvasGroup == null || !Application.isPlaying)
                _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }
    public CanvasGroup _canvasGroup;
    public RenderHeads.Media.AVProVideo.CustomDisplayUGUI displayUGui
    {
        get
        {
            if (_displayUGui == null || !Application.isPlaying)
                _displayUGui = GetComponent<RenderHeads.Media.AVProVideo.CustomDisplayUGUI>();
            return _displayUGui;
        }
    }
    public RenderHeads.Media.AVProVideo.CustomDisplayUGUI _displayUGui;
    internal Leap.Unity.InputModule.PhysicsUI physicsUI
    {
        get
        {
            if (_physicsUI == null)
                _physicsUI = GetComponent<Leap.Unity.InputModule.PhysicsUI>();
            return _physicsUI;
        }
    }
    public Leap.Unity.InputModule.PhysicsUI _physicsUI;
    private Collider parentCollider
    {
        get
        {
            if (_parentCollider == null || !Application.isPlaying)
                _parentCollider = GetComponentInParent<Collider>();
            return _parentCollider;
        }
    }
    private Collider _parentCollider;
    private Collider colliderSelf
    {
        get
        {
            if (_colliderSelf == null || !Application.isPlaying)
                _colliderSelf = GetComponent<Collider>();
            return _colliderSelf;
        }
    }
    [SerializeField]
    public Collider _colliderSelf;

    public virtual void FadeOutObject(float time)
    {
        LeanTween.cancel(gameObject);
        DisableInteractions();
        if (canvasGroup != null)
            LeanTween.value(1, 0, time).setOnUpdate(UpdateCanvasGroupAlpha).setOnComplete(CheckInvokeEventFadeOut);
        else
            LeanTween.value(1, 0, time).setOnUpdate(UpdateMaterialColor).setOnComplete(CheckInvokeEventFadeOut);
    }

    public virtual void FadeOutObject()
    {
        LeanTween.cancel(gameObject);
        DisableInteractions();
        if (canvasGroup != null)
            LeanTween.value(1, 0, timeToFade).setOnUpdate(UpdateCanvasGroupAlpha).setOnComplete(CheckInvokeEventFadeOut);
        else
            LeanTween.value(1, 0, timeToFade).setOnUpdate(UpdateMaterialColor).setOnComplete(CheckInvokeEventFadeOut);
    }

    internal void SetFadeToPercent(float percentComplete)
    {
        this.percentComplete = percentComplete;
        SetFadeToPercent();
    }
    internal void SetFadeToPercent()
    {
        if (percentComplete == 1)
        {
            EnableInteractions();
        }
        else
            DisableInteractions();
        if (canvasGroup != null)
            UpdateCanvasGroupAlpha(1 * percentComplete);
        else
            UpdateMaterialColor(1 * percentComplete);
        if (displayUGUIText != null)
            UpdateTextComponentAlpha(1 * percentComplete);
        if (displayUGUIImage != null)
            UpdateImageComponentAlpha(1 * percentComplete);
    }

    private void CheckInvokeEventFadeOut()
    {
        if (invokeEventAfterFadeOut)
        {
            eventsToInvokeOut.Invoke();
        }
    }
    public UnityEngine.UI.Text displayUGUIText;
    public UnityEngine.UI.Image displayUGUIImage;
    public virtual void FadeInObject()
    {
        if (displayUGui != null)
        {
            displayUGUIText = displayUGui.GetComponentInChildren<UnityEngine.UI.Text>();
            displayUGUIImage = displayUGui.GetComponentInChildren<UnityEngine.UI.Image>();
            if (displayUGUIText != null)
            {
                StartCoroutine(DelayFadeInText());
            }
            if(displayUGUIImage != null)
            {
                StartCoroutine(DelayFadeInImage());
            }
        }
        LeanTween.cancel(gameObject);
        if (canvasGroup != null)
            LeanTween.value(0, 1, timeToFade).setOnUpdate(UpdateCanvasGroupAlpha).setOnComplete(EnableInteractionOnComplete);
        else
            LeanTween.value(0, 1, timeToFade).setOnUpdate(UpdateMaterialColor).setOnComplete(EnableInteractionOnComplete);
    }

    private IEnumerator DelayFadeInImage()
    {
        UpdateImageComponentAlpha(0);
        yield return new WaitForSeconds(1.5f);
        LeanTween.value(0, 1, .5f).setOnUpdate(UpdateImageComponentAlpha);
    }

    private IEnumerator DelayFadeInText()
    {
        UpdateTextComponentAlpha(0);
        yield return new WaitForSeconds(1.5f);
        LeanTween.value(0, 1, .5f).setOnUpdate(UpdateTextComponentAlpha);
    }

    private void UpdateImageComponentAlpha(float value)
    {
        displayUGUIImage.color = new Color(displayUGUIImage.color.r, displayUGUIImage.color.g, displayUGUIImage.color.b, value);
    }

    private void UpdateTextComponentAlpha(float value)
    {
        displayUGUIText.color = new Color(displayUGUIText.color.r, displayUGUIText.color.g, displayUGUIText.color.b,value);
    }

    public void DelayFadeObjectIn(float delayAmount)
    {
        StartCoroutine(DelayFadeObjectIn(true, delayAmount));
    }

    private IEnumerator DelayFadeObjectIn(bool v, float delayAmount)
    {
        yield return new WaitForSeconds(delayAmount);
        if(v)
            FadeInObject();
        else
            FadeOutObject();
    }

    public void UpdateCanvasGroupAlpha(float amount)
    {
        canvasGroup.alpha = amount;
    }

    public void UpdateMaterialColor(float amount)
    {
        materialToFade.color = new Color(materialToFade.color.r, materialToFade.color.g, materialToFade.color.b, amount);
    }

    public void EnableInteractionOnComplete()
    {
        EnableInteractions();
    }

    public void EnableInteractions()
    {
        isTransparent = false;
        if (colliderSelf != null)
            colliderSelf.enabled = true;
        if (canvasGroup != null)
            canvasGroup.interactable = true;
        if (isButton)
            parentCollider.enabled = true;
        if (invokeEventAfterFadeIn)
            eventsToInvokeIn.Invoke();
        if (physicsUI != null)
            EnablePhysicsUI();
    }

    private void EnablePhysicsUI()
    {
        physicsUI.GetComponentInChildren<Collider>().enabled = true;
        //physicsUI.enabled = true;
        physicsUI.GetComponent<UnityEngine.UI.Button>().enabled = true;
    }

    private void DisablePhysicsUI()
    {
        physicsUI.ButtonFace.GetComponent<Collider>().enabled = false;
        //physicsUI.enabled = false;
        physicsUI.GetComponent<UnityEngine.UI.Button>().enabled = false;
    }

    public void DisableInteractions()
    {
        isTransparent = true;
        if (colliderSelf != null)
            colliderSelf.enabled = false;
        if (canvasGroup != null)
            canvasGroup.interactable = false;
        if (isButton)
            parentCollider.enabled = false;
        if (physicsUI != null)
            DisablePhysicsUI();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FadeObjectEvan))]
[CanEditMultipleObjects]
public class FadeObjectEditor : Editor
{
    FadeObjectEvan _target;
    SerializedProperty percentComplete;

    private void OnEnable()
    {
        _target = (FadeObjectEvan)target;
        percentComplete = serializedObject.FindProperty("percentComplete");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        percentComplete.floatValue = EditorGUILayout.Slider("Percent Complete", percentComplete.floatValue, 0, 1);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            _target.SetFadeToPercent();
        }
        if (GUILayout.Button("Reset Canvas Group"))
        {
            _target._canvasGroup = null;
        }
    }
}
#endif