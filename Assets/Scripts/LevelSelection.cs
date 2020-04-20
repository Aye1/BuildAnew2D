using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private GenericButton _genericButton;
    [SerializeField] private LayoutGroup _panel;

#pragma warning restore 0649
    #endregion
    private Dictionary<GenericButton, int> _levelDico;


    // Start is called before the first frame update
    void Start()
    {
        _levelDico = new Dictionary<GenericButton,int>();

        List<LevelBinding> bindings = LevelManager.Instance.GetLevelBindings();
        int index = 0;
        foreach(LevelBinding binding in bindings)
        {
            GenericButton levelButton = Instantiate(_genericButton, Vector3.zero, Quaternion.identity, _panel.transform);
            levelButton.SetText(binding.levelData.GetLevelName());
            levelButton.SetOnClickCallback(OnLevelButtonClick);
            _levelDico.Add( levelButton, index);
            index++;
        }
    }
    void OnLevelButtonClick(GenericButton button)
    {
        int index = 0;
        _levelDico.TryGetValue(button, out index);
        LevelManager.Instance.SetLevel(index);
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }
}
