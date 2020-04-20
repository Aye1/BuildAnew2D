using System;
using UnityEngine;

// Dependecies to other managers:
// None

// Not a real Manager (yet)
// Must be instantiated in the canvas at the moment, which is not possible with our current architecture

public class DialogManager : MonoBehaviour
{
#pragma warning disable 0649
    [Header("Prefab bindings")]
    [SerializeField] private DialogBox _dialogTemplate;
    [Header("UI parameters")]
    [SerializeField] private Vector3 _dialogBoxPosition;
#pragma warning restore 0649

    public bool IsDialogOpen { get; private set; }

    // Event sent when the dialog is closing
    public EventHandler dialogClosing;

    public static DialogManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartDialog(DialogLine[] dialogLines)
    {
        if (!IsDialogOpen)
        {
            DialogBox currentDialog = Instantiate(_dialogTemplate, Vector3.zero, Quaternion.identity, transform);
            currentDialog.transform.localPosition = _dialogBoxPosition;
            currentDialog.transform.localScale = Vector3.one;
            currentDialog.Init(dialogLines);
            IsDialogOpen = true;
            currentDialog.OnDialogClosing += DialogClosed;
        }
    }

    private void DialogClosed(object sender, EventArgs args)
    {
        DialogBox dialog = (DialogBox)sender;
        dialog.OnDialogClosing -= DialogClosed;
        IsDialogOpen = false;
        dialogClosing?.Invoke(this, EventArgs.Empty);
        Destroy(dialog.gameObject);
    }
}